using System;
using System.Collections.Generic;
using System.Text;

namespace Voldemort
{
    class MetadataStore
    {
        public static readonly byte[] CLUSTER_KEY_BYTES = Encoding.UTF8.GetBytes("cluster.xml");
        public const string METADATA_STORE_NAME = "metadata";
        public const string CLUSTER_KEY = "cluster.xml";
        public const string STORES_KEY = "stores.xml";
        public const string CLUSTER_STATE_KEY = "cluster.state";
        public const string SERVER_STATE_KEY = "server.state";
        public const string NODE_ID_KEY = "node.id";
        public const string REBALANCING_STEAL_INFO = "rebalancing.steal.info.key";
    }
}
