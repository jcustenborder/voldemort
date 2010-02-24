using System;
using System.Collections.Generic;
using System.Text;

namespace Voldemort
{
    public class UTF8Serializer:Serializer<string>
    {
        public byte[] Serialize(string instance)
        {
            return Encoding.UTF8.GetBytes(instance);
        }

        public string Deserialize(byte[] value)
        {
            return Encoding.UTF8.GetString(value);
        }   
    }
}
