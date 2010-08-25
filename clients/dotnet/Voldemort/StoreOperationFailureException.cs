using System;
using System.Collections.Generic;
using System.Text;

namespace Voldemort
{
    class StoreOperationFailureException:VoldemortException
    {
        public StoreOperationFailureException(string s)
            : base(s)
        {

        }
    }
}
