using System;
using System.Collections.Generic;
using System.Text;

namespace Voldemort
{
    public class BootstrapFailureException:VoldemortException
    {
        public BootstrapFailureException(string s)
            : base(s)
        {

        }

        public BootstrapFailureException(string s, Exception innerException)
            : base(s, innerException)
        {

        }
    }
}
