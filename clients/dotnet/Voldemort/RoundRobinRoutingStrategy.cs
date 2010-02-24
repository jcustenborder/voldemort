using System;
using System.Collections.Generic;
using System.Text;

namespace Voldemort
{
    class RoundRobinRoutingStrategy:RoutingStrategy
    {
        private ClientConfig _Config;
        private Cluster _Cluster;

        public RoundRobinRoutingStrategy(ClientConfig config, Cluster cluster)
        {
            if (null == config) throw new ArgumentNullException("config", "config cannot be null.");
            if (null == cluster) throw new ArgumentNullException("cluster", "cluster cannot be null.");
            _Config = config;
            _Cluster = cluster;
        }

        

        public IList<Node> routeRequest(byte[] key)
        {
            return new List<Node>(_Cluster.NodeMap.Values);
        }

        
    }
}
