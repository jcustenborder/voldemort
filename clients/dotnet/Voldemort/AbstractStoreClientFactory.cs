using System;
using System.Collections.Generic;
using System.Text;
using RequestFormatType = Voldemort.Protocol.RequestFormatType;

namespace Voldemort
{
    public abstract class AbstractStoreClientFactory//:StoreClientFactory
    {
        private static readonly Logger log = new Logger();

        public ClientConfig ClientConfig { get; private set; }

        public AbstractStoreClientFactory(ClientConfig config)
        {
            if (null == config) throw new ArgumentNullException("config", "config cannot be null.");
            this.ClientConfig = config;


        }

        protected abstract Store<byte[], byte[]> GetStore(string storeName, string host, int port, RequestFormatType type);

        protected abstract int GetPort(Node node);

        protected abstract void ValidateUrl(Uri url);

        public List<Uri> ValidateUrls(IEnumerable<string> urls)
        {
            if (null == urls) throw new ArgumentNullException("urls", "urls cannot be null.");
            
            List<Uri> uris = new List<Uri>();
            int count = 0;
            foreach(string url in urls)
            {
                count++;
                if (string.IsNullOrEmpty(url))
                    throw new ArgumentNullException("Null URL not allowed for bootstrapping!");
                Uri uri = null;
                try
                {
                    uri = new Uri(url, UriKind.Absolute);
                }
                catch (FormatException e)
                {
                    throw new BootstrapFailureException("Uri \"" + url + "\" could not be parsed.", e);
                }

                


                if (string.IsNullOrEmpty(uri.Host))
                    throw new ArgumentException("Illegal scheme in bootstrap URL, must specify a host, URL: " + uri);
                else if (uri.Port< 0)
                    throw new ArgumentException("Must specify a port in bootstrap URL, URL: " + uri);
                else
                    ValidateUrl(uri);

                uris.Add(uri);
            }

            if (count == 0)
                throw new ArgumentException("Must provide at least one bootstrap URL!");

            return uris;
        }

        public void Close()
        {
            
        }

        public string BootstrapMetadataWithRetries(byte[] key, List<Uri> list)
        {
            foreach (string bootstrapUrl in this.ClientConfig.BootstrapUrls)
            {
                try
                {
                    UriBuilder builder = new UriBuilder(bootstrapUrl);

                    Store store = getStore(MetadataStore.METADATA_STORE_NAME, builder.Host, builder.Port, this.ClientConfig.RequestFormatType, false);

                    IList<Versioned> vvs = store.get(key);

                    if (vvs.Count == 1)
                        return Encoding.UTF8.GetString(vvs[0].value);
                }
                catch (Exception ex)
                {
                    if (log.IsErrorEnabled) log.Error("Could not bootstrap '" + bootstrapUrl + "'", ex);
                }
            }

            throw new BootstrapFailureException("No available bootstrap servers found!");
        }

        protected abstract Store getStore(string name, string host, int port, RequestFormatType requestFormatType, bool shouldReroute);

        public Store GetRawStore(string storeName, InconsistencyResolver resolver)
        {
            if (string.IsNullOrEmpty(storeName)) throw new ArgumentNullException("storeName", "storeName cannot be null.");


            string clusterXml = BootstrapMetadataWithRetries(MetadataStore.CLUSTER_KEY_BYTES, ValidateUrls(this.ClientConfig.BootstrapUrls));

            Cluster cluster = Cluster.LoadXml(clusterXml);

            IDictionary<int, Store> clusterMap = new Dictionary<int, Store>();
            IDictionary<int, Node> nodeMap = cluster.NodeMap;
            foreach (Node node in nodeMap.Values)
            {
                Store store = getStore(storeName, node.Host, node.SocketPort, this.ClientConfig.RequestFormatType, true);
                clusterMap.Add(node.ID, store);
            }

            RoutingStrategy routingStrategy = new RoundRobinRoutingStrategy(this.ClientConfig, cluster);
            Store routedStore = new RoutedStore(storeName,
                                                  this.ClientConfig,
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


        public StoreClient GetStoreClient(string storeName)
        {
            return GetStoreClient(storeName, null);
        }

        public StoreClient GetStoreClient(string storeName, InconsistencyResolver resolver)
        {
            if (string.IsNullOrEmpty(storeName)) throw new ArgumentNullException("storeName", "storeName cannot be null.");

            Store store = GetRawStore(storeName, resolver);

            return new DefaultStoreClient(store, resolver, this.ClientConfig, this);
        }

        public StoreClient<Key, Value> GetStoreClient<Key, Value>(string storeName, Serializers.Serializer<Key> KeySerializer, Serializers.Serializer<Value> ValueSerializer)
        {
            return GetStoreClient<Key, Value>(storeName, KeySerializer, ValueSerializer, null);
        }
        public StoreClient<Key, Value> GetStoreClient<Key, Value>(string storeName, Serializers.Serializer<Key> KeySerializer, Serializers.Serializer<Value> ValueSerializer, InconsistencyResolver resolver)
        {
            if (string.IsNullOrEmpty(storeName)) throw new ArgumentNullException("storeName", "storeName cannot be null.");
            Store store = GetRawStore(storeName, resolver);
            DefaultStoreClient<Key, Value> client = new DefaultStoreClient<Key, Value>(store, resolver, this.ClientConfig, this);
            client.KeySerializer = KeySerializer;
            client.ValueSerializer = ValueSerializer;
            return client;
        }

        //public StoreClient<Key, Value> GetStoreClient<Key, Value>(string storeName, Voldemort.Serializers.Serializer<Key> KeySerializer, Voldemort.Serializers.Serializer<Value> ValueSerializer)
        //{
        //    throw new NotImplementedException();
        //}

        //public StoreClient<Key, Value> GetStoreClient<Key, Value>(string storeName, Voldemort.Serializers.Serializer<Key> KeySerializer, Voldemort.Serializers.Serializer<Value> ValueSerializer, InconsistencyResolver resolver)
        //{
        //    throw new NotImplementedException();
        //}

        
    }
}
