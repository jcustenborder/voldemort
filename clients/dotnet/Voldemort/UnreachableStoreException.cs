using System;
using System.Collections.Generic;
using System.Text;

namespace Voldemort
{
    class UnreachableStoreException:VoldemortException
    {
        public UnreachableStoreException(string s)
            : base(s)
        {

        }
        public UnreachableStoreException(string s, Exception innerException):base(s, innerException)

        {

        }
    }
}
