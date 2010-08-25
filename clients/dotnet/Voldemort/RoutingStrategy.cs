using System;
using System.Collections.Generic;
using System.Text;
using Voldemort.Model;

namespace Voldemort
{
    interface RoutingStrategy
    {
        IList<Node> routeRequest(byte[] key);
    }
}
