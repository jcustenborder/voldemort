using System;
using System.Collections.Generic;
using System.Text;

namespace Voldemort.Model
{
    class NodeComparer : IEqualityComparer<Node>, IComparer<Node>
    {
        public bool Equals(Node x, Node y)
        {
            return x.ID == y.ID;
        }
        public int GetHashCode(Node obj)
        {
            return obj.ID.GetHashCode();
        }
        public int Compare(Node x, Node y)
        {
            return x.ID.CompareTo(y.ID);
        }

        public static readonly NodeComparer Instance = new NodeComparer();
    }
}
