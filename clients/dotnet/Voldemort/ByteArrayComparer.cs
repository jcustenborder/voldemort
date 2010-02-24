using System;
using System.Collections.Generic;
using System.Text;

namespace Voldemort
{

    public class ByteArrayComparer : IComparer<byte[]>, IEqualityComparer<byte[]>
    {

        public static int ComputeHash(params byte[] data)
        {
            unchecked
            {
                const int p = 16777619;
                int hash = (int)2166136261;

                for (int i = 0; i < data.Length; i++)
                    hash = (hash ^ data[i]) * p;

                hash += hash << 13;
                hash ^= hash >> 7;
                hash += hash << 3;
                hash ^= hash >> 17;
                hash += hash << 5;
                return hash;
            }
        }
        
        public int Compare(byte[] x, byte[] y)
        {
            int xHash = ComputeHash(x);
            int yHash = ComputeHash(y);

            return xHash.CompareTo(yHash);
        }

        public static readonly ByteArrayComparer Instance = new ByteArrayComparer();


        public bool Equals(byte[] x, byte[] y)
        {
            int xHash = ComputeHash(x);
            int yHash = ComputeHash(y);
            return xHash == yHash;
        }

        public int GetHashCode(byte[] obj)
        {
            return ComputeHash(obj);
        }

    }
}
