using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

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
                    Stream sstream = conn.get_io_stream();
                    request.writeGetRequest(sstream, this.Name, key, this.ShouldReroute);
                    sstream.Flush();

                    return request.readGetResponse(sstream);
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
                    Stream sstream = conn.get_io_stream();
                    request.writePutRequest(sstream, 
                        this.Name, 
                        key, 
                        value.value, 
                        (VectorClock)value.version, 
                        this.ShouldReroute);
                    sstream.Flush();

                    request.readPutResponse(sstream);
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
                    Stream sstream = conn.get_io_stream();
                    request.writeDeleteRequest(sstream,
                        this.Name,
                        key,
                        version.version,
                        this.ShouldReroute);
                    sstream.Flush();

                    return request.readDeleteResponse(sstream);
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

        #region Store Members


        public IList<KeyedVersions> getAll(IEnumerable<byte[]> keys)
        {
            try
            {
                using (Connection conn = Pool.Checkout(this.Host, this.Port, request.getNegotiationString()))
                {
                    Stream sstream = conn.get_io_stream();
                    request.writeGetAllRequest(sstream,
                        this.Name,
                        keys,
                        this.ShouldReroute);
                    sstream.Flush();

                    return request.readGetAllResponse(sstream);
                }
            }
            catch (UnreachableStoreException ex)
            {
                if (log.IsErrorEnabled) log.Error("Failure to get " + Host, ex);
                throw new UnreachableStoreException("Failure to get " + Host);
            }
        }

        #endregion
    }
}
