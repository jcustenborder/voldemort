using System;
using System.Collections.Generic;
using System.Text;

namespace Voldemort.Serializers
{
    public class NullSerializer:Serializer<byte[]>
    {
        private NullSerializer() { }
        public static readonly Serializer<byte[]> Instance = new NullSerializer();
        
        public byte[] Serialize(byte[] value)
        {
            return value;
        }

        public byte[] Deserialize(byte[] value)
        {
            return value;
        }
    }
}
