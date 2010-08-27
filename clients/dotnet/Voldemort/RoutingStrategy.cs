using System;
using System.Collections.Generic;
using System.Text;

namespace Voldemort
{
    interface RoutingStrategy
    {
        IList<Node> routeRequest(byte[] key);
    }
}
