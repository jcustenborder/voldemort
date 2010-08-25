using System;
using System.Collections.Generic;
using System.Text;

namespace Voldemort
{
    public partial class VectorClock:IComparable<VectorClock>
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0);

        public VectorClock()
        {
            TimeSpan span = DateTime.Now.Subtract(Epoch);

            _timestamp = ((long)span.TotalSeconds) * 1000L + ((long)span.TotalMilliseconds) / 1000L;
        }

        public int CompareTo(VectorClock other)
        {
            throw new NotImplementedException();
        }
    }
}
