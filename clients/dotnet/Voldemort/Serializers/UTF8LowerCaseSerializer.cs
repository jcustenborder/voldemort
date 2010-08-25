using System;
using System.Collections.Generic;
using System.Text;

namespace Voldemort.Serializers
{
    public class UTF8LowerCaseSerializer:Serializer<string>
    {
        public static readonly Serializer<string> Instance = new UTF8LowerCaseSerializer();

        private UTF8LowerCaseSerializer() { }

        public byte[] Serialize(string s)
        {
            if (string.IsNullOrEmpty(s)) throw new ArgumentNullException("s", "s cannot be null.");
            string value = s.ToLower();

            return Encoding.UTF8.GetBytes(value);
        }

        public string Deserialize(byte[] value)
        {
            return Encoding.UTF8.GetString(value);
        }   
    }
}
