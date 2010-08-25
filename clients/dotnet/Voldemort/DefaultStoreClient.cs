using System;
using System.Collections.Generic;
using System.Text;
using Voldemort.Serializers;

namespace Voldemort
{
    class DefaultStoreClient: StoreClient
    {
        private static readonly Logger log = new Logger();
        private Store _Store;
        private StoreClientFactory _Factory;
        private ClientConfig _config;
        private InconsistencyResolver _Resolver;

        public DefaultStoreClient(Store store, InconsistencyResolver resolver, ClientConfig config, StoreClientFactory factory)
        {
            if (null == store) throw new ArgumentNullException("store", "store cannot be null.");
            //if(null==resolver)throw new ArgumentNullException("resolver", "resolver cannot be null.");
            if (null == config) throw new ArgumentNullException("config", "config cannot be null.");
            if (null == factory) throw new ArgumentNullException("factory", "factory cannot be null.");

            _Factory = factory;
            _Store = store;
            _config = config;
            _Resolver = resolver;
        }



        public byte[] GetValue(byte[] key)
        {
            return GetValue(key, null);
        }

        public byte[] GetValue(byte[] key, byte[] defaultValue)
        {
            Versioned value = Get(key, null);

            if (null == value)
                return defaultValue;
            else
                return value.value;
        }

        public Versioned Get(byte[] key)
        {
            return Get(key, null);
        }

        const int METADATA_REFRESH_ATTEMPTS = 3;

        public Versioned Get(byte[] key, Versioned defaultValue)
        {
            for (int attempts = 0; attempts < METADATA_REFRESH_ATTEMPTS; attempts++)
            {
                try
                {
                    Versioned curValue = null;
                    IList<Versioned> items = _Store.get(key);
                    if (items.Count == 0)
                    {
                        if (null == defaultValue)
                            return null;
                        curValue = defaultValue;
                    }
                    else if (items.Count == 1)
                        curValue = items[0];
                    else
                        throw new InconsistentDataException("Unresolved versions returned from get(" + key + ")");

                    return curValue;
                }
                catch (InvalidMetadataException ex)
                {
                    if (log.IsErrorEnabled) log.Error("Exception thrown", ex);
                    reinit();
                }
            }

            throw new InvalidMetadataException("Exceeded maximum metadata refresh attempts");

        }

        private void reinit()
        {
            Store store = _Factory.GetRawStore(_Store.Name, _Resolver);
            _Store = store;
        }

        public void Put(byte[] key, byte[] value)
        {
            Versioned vv = Get(key);

            if (null == vv)
            {
                try
                {
                    vv = new Versioned() { value = value, version = new VectorClock() };
                }
                catch (InvalidMetadataException ex)
                {
                    if (log.IsErrorEnabled) log.Error("Exception thrown", ex);
                }
            }
            else
            {
                vv.value = value;
            }

            Put(key, vv);
        }

        public void Put(byte[] key, Versioned value)
        {
            for (int attempts = 0; attempts < METADATA_REFRESH_ATTEMPTS; attempts++)
            {
                try
                {
                    _Store.put(key, value);
                    return;
                }
                catch (InvalidMetadataException ex)
                {
                    if (log.IsErrorEnabled) log.Error("Exception thrown while calling put on store " + _Store.Name, ex);
                    reinit();
                }
            }
            throw new InvalidMetadataException("Exceeded maximum metadata refresh attempts");
        }

        public bool PutifNotObsolete(byte[] key, Versioned value)
        {
            try
            {
                Put(key, value);
                return true;
            }
            catch (ObsoleteVersionException)
            {
                //TODO:Revisit this. Fuck throwing an exception if we don't need to.
                return false;
            }
        }

        public bool DeleteKey(byte[] key)
        {
            Versioned vv = Get(key);

            if (null == vv)
                return false;

            return DeleteKey(key, vv);

        }

        public bool DeleteKey(byte[] key, Versioned version)
        {
            for (int attempts = 0; attempts < METADATA_REFRESH_ATTEMPTS; attempts++)
            {
                try
                {
                    return _Store.deleteKey(key, version);
                }
                catch (InvalidMetadataException)
                {
                    reinit();
                }
            }
            throw new InvalidMetadataException("Exceeded maximum metadata refresh attempts");
        }

     


        public IList<KeyedVersions> GetAll(IEnumerable<byte[]> keys)
        {
            for (int attempts = 0; attempts < METADATA_REFRESH_ATTEMPTS; attempts++)
            {
                try
                {
                    return _Store.getAll(keys);
                }
                catch (InvalidMetadataException)
                {
                    reinit();
                }
            }
            throw new InvalidMetadataException("Exceeded maximum metadata refresh attempts");
        }
    }
    class DefaultStoreClient<Key, Value> : DefaultStoreClient, StoreClient<Key, Value>
    {
        public DefaultStoreClient(Store store, InconsistencyResolver resolver, ClientConfig config, StoreClientFactory factory):
            base(store, resolver, config, factory)
        {

        }

        public Serializer<Key> KeySerializer { get; set; }
        public Serializer<Value> ValueSerializer{ get; set; }


        public Value GetValue(Key key)
        {
            byte[] keyBuffer = KeySerializer.Serialize(key);
            byte[] valueBuffer = base.GetValue(keyBuffer);

            if (null == valueBuffer || valueBuffer.Length == 0)
            {
                return default(Value);
            }

            return ValueSerializer.Deserialize(valueBuffer);
        }

        public Value GetValue(Key key, Value defaultValue)
        {
            byte[] keyBuffer = KeySerializer.Serialize(key);
            byte[] valueBuffer = base.GetValue(keyBuffer);

            if (null == valueBuffer || valueBuffer.Length == 0)
            {
                return defaultValue;
            }

            return ValueSerializer.Deserialize(valueBuffer);
        }

        public Versioned Get(Key key)
        {
            byte[] keyBuffer = KeySerializer.Serialize(key);

            return base.Get(keyBuffer);
        }

        public Versioned Get(Key key, Versioned defaultValue)
        {
            byte[] keyBuffer = KeySerializer.Serialize(key);
            return base.Get(keyBuffer, defaultValue);
        }

        public IList<KeyValuePair<Key, Value>> GetAll(IEnumerable<Key> keys)
        {
            List<byte[]> keyBuffers = new List<byte[]>();
            foreach (Key key in keys)
                keyBuffers.Add(KeySerializer.Serialize(key));
            IList<KeyedVersions> versions = base.GetAll(keyBuffers);

            List<KeyValuePair<Key, Value>> results = new List<KeyValuePair<Key, Value>>();
            foreach (KeyedVersions version in versions)
            {
                Key key = KeySerializer.Deserialize(version.key);
                if(version.versions.Count==0)
                    continue;
                Value value = ValueSerializer.Deserialize(version.versions[0].value);

                KeyValuePair<Key, Value> result = new KeyValuePair<Key, Value>(key, value);
                results.Add(result);
            }
            return results;
        }

        public void Put(Key key, Value value)
        {
            byte[] keyBuffer = KeySerializer.Serialize(key);
            byte[] valueBuffer = ValueSerializer.Serialize(value);
            Put(keyBuffer, valueBuffer);
        }

        public void Put(Key key, Versioned value)
        {
            byte[] keyBuffer = KeySerializer.Serialize(key);
            Put(keyBuffer, value);
        }

        public bool PutifNotObsolete(Key key, Versioned value)
        {
            byte[] keyBuffer = KeySerializer.Serialize(key);
            return PutifNotObsolete(key, value);
        }

        public bool DeleteKey(Key key)
        {
            byte[] keyBuffer = KeySerializer.Serialize(key);
            return base.DeleteKey(keyBuffer);
        }

        public bool DeleteKey(Key key, Versioned version)
        {
            byte[] keyBuffer = KeySerializer.Serialize(key);
            return base.DeleteKey(keyBuffer, version);
        }

    }
}
