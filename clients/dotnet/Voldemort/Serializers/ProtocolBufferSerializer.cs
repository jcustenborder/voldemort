﻿using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;
using System.IO;

namespace Voldemort.Serializers
{
    public class ProtocolBufferSerializer<T>:Serializer<T>
    {
        public static readonly Serializer<T> Instance = new ProtocolBufferSerializer<T>();

        private ProtocolBufferSerializer() { }

        public byte[] Serialize(T instance)
        {
            using (MemoryStream iostr = new MemoryStream())
            {
                Serializer.Serialize<T>(iostr, instance);
                return iostr.ToArray();
            }
        }

        public T Deserialize(byte[] value)
        {
            using (MemoryStream iostr = new MemoryStream(value))
            {
                return Serializer.Deserialize<T>(iostr);
            }
        }
    }
}
