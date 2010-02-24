using System;
using System.Collections.Generic;
using System.Text;

namespace Voldemort
{
    class ObsoleteVersionException:VoldemortException
    {
        public ObsoleteVersionException(string s)
            : base(s)
        {

        }
    }
}
