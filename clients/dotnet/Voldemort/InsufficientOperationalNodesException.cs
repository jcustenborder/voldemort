using System;
using System.Collections.Generic;
using System.Text;

namespace Voldemort
{
    class InsufficientOperationalNodesException:VoldemortException
    {
        public InsufficientOperationalNodesException(string s)
            : base(s)
        {

        }
    }
}
