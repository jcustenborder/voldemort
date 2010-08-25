using System;
using System.Collections.Generic;
using System.Text;

namespace Voldemort.Serializers
{
    public interface Serializer<T>
    {
        byte[] Serialize(T instance);
        T Deserialize(byte[] value);
    }
}
