using System;
using System.Collections.Generic;
using System.Text;

namespace Voldemort
{
    public interface Store
    {
        IList<Versioned> get(byte[] key);
        IList<KeyedVersions> getAll(IEnumerable<byte[]> keys);
        void put(byte[] key, Versioned value);
        bool deleteKey(byte[] key, Versioned version);
        string Name { get; }
        void close();
    }
    public interface Store<Key, Value>
    {
        IList<Versioned> get(Key key);
        IList<KeyedVersions> getAll(IEnumerable<Key> keys);
        void put(Key key, Versioned<Value> value);
        bool deleteKey(Key key, Versioned<Value> version);
        string Name { get; }
        void close();
    }


}
