using System;
using System.Collections.Generic;
using System.Text;
using Voldemort.Serializers;

namespace Voldemort
{
    public interface StoreClient
    {
        byte[] GetValue(byte[] key);
        byte[] GetValue(byte[] key, byte[] defaultValue);
        Versioned Get(byte[] key);
        Versioned Get(byte[] key, Versioned defaultValue);
        IList<KeyedVersions> GetAll(IEnumerable<byte[]> keys);
        void Put(byte[] key, byte[] value);
        void Put(byte[] key, Versioned value);
        bool PutifNotObsolete(byte[] key, Versioned value);
        bool DeleteKey(byte[] key);
        bool DeleteKey(byte[] key, Versioned version);
    }

    public interface StoreClient<Key, Value>
    {
        Serializer<Key> KeySerializer { get; set; }
        Serializer<Value> ValueSerializer { get; set; }
        Value GetValue(Key key);
        Value GetValue(Key key, Value defaultValue);
        Versioned Get(Key key);
        Versioned Get(Key key, Versioned defaultValue);
        IList<KeyValuePair<Key, Value>> GetAll(IEnumerable<Key> keys);
        void Put(Key key, Value value);
        void Put(Key key, Versioned value);
        bool PutifNotObsolete(Key key, Versioned value);
        bool DeleteKey(Key key);
        bool DeleteKey(Key key, Versioned version);
    }
}
