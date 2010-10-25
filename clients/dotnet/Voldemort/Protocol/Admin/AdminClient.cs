using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

namespace Voldemort.Protocol.Admin
{
    public class AdminClient
    {
        private static readonly Logger log = new Logger();
        public string BootStrapUrl { get; private set; }
        public ClientConfig ClientConfig { get; private set; }
        public Cluster Cluster { get; private set; }
        private ConnectionPool _Pool;
        
        public AdminClient(string bootstrapURL, ClientConfig clientConfig)
        {
            if (string.IsNullOrEmpty(bootstrapURL)) throw new ArgumentNullException("bootstrapURL", "bootstrapURL cannot be null.");
            if (null == clientConfig) throw new ArgumentNullException("adminClientConfig", "adminClientConfig cannot be null.");
            if (RequestFormatType.ADMIN_HANDLER != clientConfig.RequestFormatType)
                throw new ArgumentException("RequestFormatType must be ADMIN_HANDLER.", "clientConfig.RequestFormatType");
            this.BootStrapUrl = bootstrapURL;
            this.ClientConfig = clientConfig;
            this.Cluster = getClusterFromBootstrapURL(bootstrapURL);
            this._Pool = new ConnectionPool(clientConfig);
        }

        private Cluster getClusterFromBootstrapURL(String bootstrapURL)
        {            
            ClientConfig config = new ClientConfig();
            // try to bootstrap metadata from bootstrapUrl
            config.BootstrapUrls.Add(bootstrapURL);
            SocketStoreClientFactory factory = new SocketStoreClientFactory(config);
            
            // get Cluster from bootStrapUrl
            string clusterXml = factory.BootstrapMetadataWithRetries(MetadataStore.CLUSTER_KEY_BYTES, factory.ValidateUrls(config.BootstrapUrls));
            // release all threads/sockets hold by the factory.
            factory.Close();

            return Cluster.LoadXml(clusterXml);// clusterMapper.readCluster(new StringReader(clusterXml), false);
        }

        private void initiateFetchRequest(Stream outputStream,
                                  String storeName,
                                  IEnumerable<int> partitionList,
                                  VoldemortFilter filter,
                                  bool fetchValues,
                                  bool fetchMasterEntries)
        {
            FetchPartitionEntriesRequest fetchRequest = new FetchPartitionEntriesRequest();
            if (string.IsNullOrEmpty(storeName)) throw new ArgumentNullException("storeName", "storeName cannot be null.");
            if (null == partitionList) throw new ArgumentNullException("partitionList", "partitionList cannot be null.");
            
            fetchRequest.store = storeName;
            fetchRequest.partitions.AddRange(partitionList);
            if (null != filter)
                fetchRequest.filter = filter;
            fetchRequest.fetch_values = fetchValues;
            fetchRequest.fetch_master_entries = fetchMasterEntries;

            VoldemortAdminRequest request = new VoldemortAdminRequest();
            request.type = AdminRequestType.FETCH_PARTITION_ENTRIES;
            request.fetch_partition_entries = fetchRequest;

            ProtoBuf.Serializer.SerializeWithLengthPrefix(outputStream, request, ProtoBuf.PrefixStyle.Fixed32BigEndian);
            outputStream.Flush();
        }

        public IEnumerable<KeyValuePair<byte[], Versioned>> FetchEntries(int nodeId, String storeName, IEnumerable<int> partitionList, VoldemortFilter filter, bool fetchMasterEntries)
        {
            Node node = Cluster[nodeId];



            using (Connection connection = _Pool.Checkout(node.Host, node.AdminPort, this.ClientConfig.RequestFormatType))
            {
                try
                {
                    initiateFetchRequest(connection.Stream, storeName, partitionList, filter, true, fetchMasterEntries);
                }
                catch
                {
                    connection.Errored = true;
                    throw;

                }

                byte[] buffer = new byte[connection.Socket.ReceiveBufferSize];

                while (true)
                {
                    KeyValuePair<byte[], Versioned> value;
                    try
                    {
                        int length = readLength(connection);
                        if (length ==-1)
                            break;//endof stream

                        using (MemoryStream iostr = new MemoryStream(length))
                        {
                            int read = 0;

                            while (read < length)
                            {
                                int count = Math.Min(buffer.Length, (length - read));
                                int thispass = connection.Stream.Read(buffer, 0, count);
                                iostr.Write(buffer, 0, thispass);
                                read += thispass;
                            }
                            iostr.Position = 0;

                            FetchPartitionEntriesResponse response = ProtoBuf.Serializer.Deserialize<FetchPartitionEntriesResponse>(iostr);

                            value = new KeyValuePair<byte[], Versioned>(response.partition_entry.key, response.partition_entry.versioned);

                        }
                    }
                    catch
                    {
                        connection.Errored = true;
                        throw;
                    }

                    yield return value;
                }

                yield break;
            }
        }

        public IEnumerable<T> FetchKeys<T>(int nodeId, String storeName, IEnumerable<int> partitionList, VoldemortFilter filter, bool fetchMasterEntries, Serializers.Serializer<T> serializer)
        {
            if (null == serializer) throw new ArgumentNullException("serializer", "serializer cannot be null.");
            
            foreach(byte[] b in FetchKeys(nodeId, storeName, partitionList, filter, fetchMasterEntries))
            {
                T key = serializer.Deserialize(b);
                yield return key;
            }
        }

        public IEnumerable<byte[]> FetchKeys(int nodeId, String storeName, IEnumerable<int> partitionList, VoldemortFilter filter, bool fetchMasterEntries)
        {
            Node node = Cluster[nodeId];



            using (Connection connection = _Pool.Checkout(node.Host, node.AdminPort, this.ClientConfig.RequestFormatType))
            {
                try
                {
                    initiateFetchRequest(connection.Stream, storeName, partitionList, filter, false, fetchMasterEntries);
                }
                catch
                {
                    connection.Errored = true;
                    throw;

                }

                byte[] buffer = new byte[connection.Socket.ReceiveBufferSize];

                while (true)
                {
                    byte[] value;

                    try
                    {
                        
                        int length = readLength(connection);
                        if (length ==-1)
                            break;//End of stream
                        
                        using (MemoryStream iostr = new MemoryStream(length))
                        {
                            int read = 0;

                            while (read < length)
                            {
                                int count = Math.Min(buffer.Length, (length - read));
                                int thispass = connection.Stream.Read(buffer, 0, count);
                                iostr.Write(buffer, 0, thispass);
                                read += thispass;
                            }
                            iostr.Position = 0;

                            FetchPartitionEntriesResponse response = ProtoBuf.Serializer.Deserialize<FetchPartitionEntriesResponse>(iostr);

                            value = response.key;
                        }
                    }
                    catch
                    {
                        connection.Errored = true;
                        throw;
                    }

                    yield return value;
                }

                yield break;
            }
        }
        private int readLength(Connection connection)
        {
            byte[] buffer = new byte[4];
            connection.Read(buffer, 0, 4);
            int value = BitConverter.ToInt32(buffer, 0);
            if (BitConverter.IsLittleEndian)
                value = System.Net.IPAddress.NetworkToHostOrder(value);
            return value;

        }
    }

}
