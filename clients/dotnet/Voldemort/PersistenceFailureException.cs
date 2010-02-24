using System;
using System.Collections.Generic;
using System.Text;

namespace Voldemort
{
    class PersistenceFailureException:VoldemortException
    {
        public PersistenceFailureException(string s)
            : base(s)
        {

        }
    }
}
