using System;
using System.Collections.Generic;
using System.Text;

namespace Voldemort
{
    public class VoldemortException : ApplicationException
    {
        public VoldemortException(string s)
            : base(s)
        {

        }
        public VoldemortException(string s, Exception innerException)
            : base(s, innerException)
        {

        }
    }
                
}
