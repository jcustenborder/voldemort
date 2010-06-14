using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Voldemort.Protocol;

namespace Voldemort
{
    class SocketStore: Store
    {
        private static readonly Logger log = new Logger();

        public string Name { get; private set; }
        public string Host { get; private set; }
        public int Port { get; private set; }
        public ClientConfig Config { get; private set; }
        public ConnectionPool Pool { get; private set; }
        public RequestFormat.RequestFormatType Type { get; private set; }
        public bool ShouldReroute { get; private set; }

        private RequestFormat request;


        public SocketStore(string storeName,
                       string host,
                       int port,
                       ClientConfig config,
                       ConnectionPool connPool,
                       RequestFormat.RequestFormatType type,
                       bool shouldReroute)
        {
            if (string.IsNullOrEmpty(storeName)) throw new ArgumentNullException("storeName", "storeName cannot be null.");
            if (string.IsNullOrEmpty(host)) throw new ArgumentNullException("host", "host cannot be null.");
            if (null == config) throw new ArgumentNullException("config", "config cannot be null.");
            if (null == connPool) throw new ArgumentNullException("connPool", "connPool cannot be null.");
            
            this.Name = storeName;
            this.Host = host;
            this.Port = port;
            this.Config = config;
            this.Pool = connPool;
            this.Type = type;
            this.ShouldReroute = shouldReroute;

            this.request = RequestFormat.newRequestFormat(type);
        }

        public IList<Versioned> get(byte[] key)
        {
            try
            {
                using (Connection conn = Pool.Checkout(this.Host, this.Port, request.getNegotiationString()))
                {
                    request.writeGetRequest(conn.Stream, this.Name, key, this.ShouldReroute);
                    conn.Stream.Flush();

                    return request.readGetResponse(conn.Stream);
                }
            }
            catch (UnreachableStoreException ex)
            {
                if (log.IsErrorEnabled) log.Error("Failure to get " + Host, ex);
                throw new UnreachableStoreException("Failure to get " + Host);
            }
        }

        public void put(byte[] key, Versioned value)
        {
            try
            {
                using (Connection conn = Pool.Checkout(this.Host, this.Port, request.getNegotiationString()))
                {
                    request.writePutRequest(conn.Stream, 
                        this.Name, 
                        key, 
                        value.value, 
                        (VectorClock)value.version, 
                        this.ShouldReroute);
                    conn.Stream.Flush();

                    request.readPutResponse(conn.Stream);
                }
            }
            catch (UnreachableStoreException ex)
            {
                if (log.IsErrorEnabled) log.Error("Failure to get " + Host, ex);
                throw new UnreachableStoreException("Failure to get " + Host);
            }
        }

        public bool deleteKey(byte[] key, Versioned version)
        {
            try
            {
                using (Connection conn = Pool.Checkout(this.Host, this.Port, request.getNegotiationString()))
                {
                    request.writeDeleteRequest(conn.Stream,
                        this.Name,
                        key,
                        version.version,
                        this.ShouldReroute);
                    conn.Stream.Flush();

                    return request.readDeleteResponse(conn.Stream);
                }
            }
            catch (UnreachableStoreException ex)
            {
                if (log.IsErrorEnabled) log.Error("Failure to get " + Host, ex);
                throw new UnreachableStoreException("Failure to get " + Host);
            }
        }



        public void close()
        {
            
        }

        public IList<KeyedVersions> getAll(IEnumerable<byte[]> keys)
        {
            try
            {
                using (Connection conn = Pool.Checkout(this.Host, this.Port, request.getNegotiationString()))
                {
                    request.writeGetAllRequest(conn.Stream,
                        this.Name,
                        keys,
                        this.ShouldReroute);
                    conn.Stream.Flush();

                    return request.readGetAllResponse(conn.Stream);
                }
            }
            catch (UnreachableStoreException ex)
            {
                if (log.IsErrorEnabled) log.Error("Failure to get " + Host, ex);
                throw new UnreachableStoreException("Failure to get " + Host);
            }
        }
    }
}
