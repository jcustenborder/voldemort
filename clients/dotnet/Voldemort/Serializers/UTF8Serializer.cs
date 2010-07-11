using System;
using System.Collections.Generic;
using System.Text;

namespace Voldemort.Serializers
{
    public class UTF8Serializer:Serializer<string>
    {
        public static readonly Serializer<string> instance = new UTF8Serializer();

        private UTF8Serializer() { }

        public byte[] Serialize(string s)
        {
            return Encoding.UTF8.GetBytes(s);
        }

        public string Deserialize(byte[] value)
        {
            return Encoding.UTF8.GetString(value);
        }   
    }
}
