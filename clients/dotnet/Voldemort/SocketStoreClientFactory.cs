using System;
using System.Collections.Generic;
using System.Text;
using Voldemort.Protocol;

namespace Voldemort
{
    public class SocketStoreClientFactory : AbstractStoreClientFactory
    {
        
        private const string STORES_KEY = "stores.xml";
        private const string ROLLBACK_CLUSTER_KEY = "rollback.cluster.xml";
        public const string URL_SCHEME = "tcp";
        

        private static readonly Logger log = new Logger();
        private ConnectionPool _ConnPool;
        

        public SocketStoreClientFactory(ClientConfig config):base(config)
        {
            _ConnPool = new ConnectionPool(this.ClientConfig);
        }





        

        protected override Store getStore(string storeName, string host, int port, RequestFormatType type, bool shouldReRoute)
        {
            return new SocketStore(storeName,
                       host,
                       port,
                       this.ClientConfig,
                       _ConnPool,
                       type,
                       shouldReRoute);
        }


        //private Versioned bootstrapMetadata(byte[] key)
        //{

        //}

        protected override Store<byte[], byte[]> GetStore(string storeName, string host, int port, RequestFormatType type)
        {
            throw new NotImplementedException();
        }

        protected override int GetPort(Node node)
        {
            throw new NotImplementedException();
        }

        protected override void ValidateUrl(Uri url)
        {
            if (null == url) throw new ArgumentNullException("url", "url cannot be null.");

            if (!string.Equals(url.Scheme, URL_SCHEME))
                throw new ArgumentException("Illegal scheme in bootstrap URL for SocketStoreClientFactory:"
                                               + " expected '"
                                               + URL_SCHEME
                                               + "' but found '"
                                               + url.Scheme + "'.");
        }




    }
}
