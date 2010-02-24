using System;
using System.Collections.Generic;
using System.Text;

namespace Voldemort
{
    public interface StoreClient
    {
        byte[] getValue(byte[] key);
        byte[] getValue(byte[] key, byte[] defaultValue);
        Versioned get(byte[] key);
        Versioned get(byte[] key, Versioned defaultValue);
        IList<KeyedVersions> getAll(IEnumerable<byte[]> keys);
        void put(byte[] key, byte[] value);
        void put(byte[] key, Versioned value);
        bool putifNotObsolete(byte[] key, Versioned value);
        bool deleteKey(byte[] key);
        bool deleteKey(byte[] key, Versioned version);
    }

    public interface StoreClient<Key, Value>
    {
        Serializer<Key> KeySerializer { get; set; }
        Serializer<Value> ValueSerializer { get; set; }
        Value getValue(Key key);
        Value getValue(Key key, Value defaultValue);
        Versioned get(Key key);
        Versioned get(Key key, Versioned defaultValue);
        IList<KeyedVersions> getAll(IEnumerable<Key> keys);
        void put(Key key, Value value);
        void put(Key key, Versioned value);
        bool putifNotObsolete(Key key, Versioned value);
        bool deleteKey(Key key);
        bool deleteKey(Key key, Versioned version);
    }
}
