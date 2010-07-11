using System;
using System.Collections.Generic;
using System.Text;
using Voldemort.Model;
using Voldemort.Protocol;

namespace Voldemort
{
    public class SocketStoreClientFactory : StoreClientFactory
    {
        private const string METADATA_STORE_NAME = "metadata";
        private const string STORES_KEY = "stores.xml";
        private const string ROLLBACK_CLUSTER_KEY = "rollback.cluster.xml";

        private static readonly byte[] CLUSTER_KEY = Encoding.UTF8.GetBytes("cluster.xml");

        private static readonly Logger log = new Logger();
        private ClientConfig _Config;
        private ConnectionPool _ConnPool;
        private RequestFormat.RequestFormatType _RequestFormatType = RequestFormat.RequestFormatType.PROTOCOL_BUFFERS;

        public SocketStoreClientFactory(ClientConfig config)
        {
            if (null == config) throw new ArgumentNullException("config", "config cannot be null.");

            _Config = config;
            _ConnPool = new ConnectionPool(_Config);
        }

        public StoreClient<Key, Value> GetStoreClient<Key, Value>(string storeName, Serializers.Serializer<Key> KeySerializer, Serializers.Serializer<Value> ValueSerializer)
        {
            return GetStoreClient<Key, Value>(storeName, KeySerializer, ValueSerializer, null);
        }
        public StoreClient<Key, Value> GetStoreClient<Key, Value>(string storeName, Serializers.Serializer<Key> KeySerializer, Serializers.Serializer<Value> ValueSerializer, InconsistencyResolver resolver)
        {
            if (string.IsNullOrEmpty(storeName)) throw new ArgumentNullException("storeName", "storeName cannot be null.");
            Store store = GetRawStore(storeName, resolver);
            DefaultStoreClient<Key, Value> client= new DefaultStoreClient<Key, Value>(store, resolver, _Config, this);
            client.KeySerializer = KeySerializer;
            client.ValueSerializer = ValueSerializer;
            return client;
        }

        public StoreClient GetStoreClient(string storeName)
        {
            return GetStoreClient(storeName, null);
        }

        public StoreClient GetStoreClient(string storeName, InconsistencyResolver resolver)
        {
            if (string.IsNullOrEmpty(storeName)) throw new ArgumentNullException("storeName", "storeName cannot be null.");

            Store store = GetRawStore(storeName, resolver);

            return new DefaultStoreClient(store, resolver, _Config, this);
        }

        public Store GetRawStore(string storeName, InconsistencyResolver resolver)
        {
            if (string.IsNullOrEmpty(storeName)) throw new ArgumentNullException("storeName", "storeName cannot be null.");

            
            Versioned clustervv = bootstrapMetadata(CLUSTER_KEY);

            Cluster cluster = Cluster.Load(clustervv.value);

            IDictionary<int, Store> clusterMap = new Dictionary<int, Store>();
            IDictionary<int, Node> nodeMap = cluster.NodeMap;
            foreach (Node node in nodeMap.Values)
            {
                Store store = getStore(storeName, node.Host, node.SocketPort, _RequestFormatType, true);
                clusterMap.Add(node.ID, store);
            }

            RoutingStrategy routingStrategy = new RoundRobinRoutingStrategy(_Config, cluster);
            Store routedStore = new RoutedStore(storeName,
                                                  _Config,
                                                  cluster,
                                                  clusterMap,
                                                  routingStrategy);

            InconsistencyResolvingStore conStore = new InconsistencyResolvingStore(routedStore);

            try
            {
                InconsistencyResolver vcResolver = new VectorClockInconsistencyResolver();
                conStore.addResolver(vcResolver);

                if (null != resolver)
                {
                    conStore.addResolver(resolver);
                }
                else
                {
                    conStore.addResolver(new TimeBasedInconsistencyResolver());
                }
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled) log.Error("Exception thrown", ex);
                throw;
            }

            return conStore;
        }

        private Store getStore(string storeName,
                               string host,
                               int port,
                               RequestFormat.RequestFormatType type,
                               bool shouldReroute)
        {
            return new SocketStore(storeName,
                       host,
                       port,
                       _Config,
                       _ConnPool,
                       type,
                       shouldReroute);
        }


        private Versioned bootstrapMetadata(byte[] key)
        {
            foreach (string bootstrapUrl in _Config.BootstrapUrls)
            {
                try
                {
                    UriBuilder builder = new UriBuilder(bootstrapUrl);

                    Store store = getStore(METADATA_STORE_NAME, builder.Host, builder.Port, _RequestFormatType, false);

                    IList<Versioned> vvs = store.get(key);

                    if (vvs.Count == 1)
                        return vvs[0];
                }
                catch (Exception ex)
                {
                    if (log.IsErrorEnabled) log.Error("Could not bootstrap '" + bootstrapUrl + "'", ex);
                }
            }

            throw new BootstrapFailureException("No available bootstrap servers found!");
        }
    }
}
