using System;
using System.Collections.Generic;
using System.Text;

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



        public byte[] getValue(byte[] key)
        {
            return getValue(key, null);
        }

        public byte[] getValue(byte[] key, byte[] defaultValue)
        {
            Versioned value = get(key, null);

            if (null == value)
                return defaultValue;
            else
                return value.value;
        }

        public Versioned get(byte[] key)
        {
            return get(key, null);
        }

        const int METADATA_REFRESH_ATTEMPTS = 3;

        public Versioned get(byte[] key, Versioned defaultValue)
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
            Store store = _Factory.getRawStore(_Store.Name, _Resolver);
            _Store = store;
        }

        public void put(byte[] key, byte[] value)
        {
            Versioned vv = get(key);

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

            put(key, vv);
        }

        public void put(byte[] key, Versioned value)
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

        public bool putifNotObsolete(byte[] key, Versioned value)
        {
            try
            {
                put(key, value);
                return true;
            }
            catch (ObsoleteVersionException)
            {
                //TODO:Revisit this. Fuck throwing an exception if we don't need to.
                return false;
            }
        }

        public bool deleteKey(byte[] key)
        {
            Versioned vv = get(key);

            if (null == vv)
                return false;

            return deleteKey(key, vv);

        }

        public bool deleteKey(byte[] key, Versioned version)
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

     


        public IList<KeyedVersions> getAll(IEnumerable<byte[]> keys)
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


        public Value getValue(Key key)
        {
            byte[] keyBuffer = KeySerializer.Serialize(key);
            byte[] valueBuffer = base.getValue(keyBuffer);

            if (null == valueBuffer || valueBuffer.Length == 0)
            {
                return default(Value);
            }

            return ValueSerializer.Deserialize(valueBuffer);
        }

        public Value getValue(Key key, Value defaultValue)
        {
            byte[] keyBuffer = KeySerializer.Serialize(key);
            byte[] valueBuffer = base.getValue(keyBuffer);

            if (null == valueBuffer || valueBuffer.Length == 0)
            {
                return defaultValue;
            }

            return ValueSerializer.Deserialize(valueBuffer);
        }

        public Versioned get(Key key)
        {
            throw new NotImplementedException();
        }

        public Versioned get(Key key, Versioned defaultValue)
        {
            throw new NotImplementedException();
        }

        public IList<KeyedVersions> getAll(IEnumerable<Key> keys)
        {
            throw new NotImplementedException();
        }

        public void put(Key key, Value value)
        {
            throw new NotImplementedException();
        }

        public void put(Key key, Versioned value)
        {
            throw new NotImplementedException();
        }

        public bool putifNotObsolete(Key key, Versioned value)
        {
            throw new NotImplementedException();
        }

        public bool deleteKey(Key key)
        {
            throw new NotImplementedException();
        }

        public bool deleteKey(Key key, Versioned version)
        {
            throw new NotImplementedException();
        }

    }
}
