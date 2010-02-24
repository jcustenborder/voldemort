using System;
using System.Collections.Generic;
using System.Text;

namespace Voldemort
{
    public interface Serializer<T>
    {
        byte[] Serialize(T instance);
        T Deserialize(byte[] value);
    }
}
