using System;
using System.Collections.Generic;
using System.Text;
using Voldemort.Model;

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

        static Random Random = new Random();
        public IList<Node> routeRequest(byte[] key)
        {
            List<Node> nodes =new List<Node>(_Cluster.NodeMap.Values);
            nodes.Sort(delegate(Node a, Node b) { return a.Requests.CompareTo(b.Requests); });
            return nodes;
        }

 
    }
}
