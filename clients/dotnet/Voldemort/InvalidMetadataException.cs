using System;
using System.Collections.Generic;
using System.Text;

namespace Voldemort
{
    class InvalidMetadataException:VoldemortException
    {
        public InvalidMetadataException(string s)
            : base(s)
        {

        }
    }
}
