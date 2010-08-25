using System;
using System.Collections.Generic;
using System.Text;

namespace Voldemort
{
    class InconsistentDataException:VoldemortException
    {
        public InconsistentDataException(string s)
            : base(s)
        {

        }
    }
}
