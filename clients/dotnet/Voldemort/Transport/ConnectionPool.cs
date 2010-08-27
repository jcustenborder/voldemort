using System;
using System.Collections.Generic;
using System.Threading;
using Hashtable = System.Collections.Hashtable;
using RequestFormatType = Voldemort.Protocol.RequestFormatType;
namespace Voldemort
{
    class Pool
    {
        private Uri _Uri;
        private ClientConfig _Config;

        private Pool(Uri uri, ClientConfig config)
        {
            _Uri = uri;
            _Config = config;
        }

        Queue<Connection> _ConnectionQueue = new Queue<Connection>();

        public Connection Checkout()
        {
            lock (_ConnectionQueue)
            {
                if (_ConnectionQueue.Count > 0)
                    return _ConnectionQueue.Dequeue();
            }

            //TODO: Come back and put support to ensure that we do not create a fuckton of connections. 
            Connection connection = new Connection(_Uri, _Config);
            connection.Connect();
            return connection;
        }

        public void CheckIn(Connection value)
        {
            lock (_ConnectionQueue)
            {
                _ConnectionQueue.Enqueue(value);
            }
        }

        private static Hashtable Pools = new Hashtable();

        public static Pool GetPool(Uri uri, ClientConfig config)
        {
            if (null == uri) throw new ArgumentNullException("uri", "uri cannot be null.");

            Pool pool = Pools[uri] as Pool;

            if (null == pool)
            {
                lock (Pools)
                {
                    pool = Pools[uri] as Pool;

                    if (null == pool)
                    {
                        pool = new Pool(uri, config);
                        Pools.Add(uri, pool);
                    }
                }
            }

            return pool;
        }
    }

    sealed class ConnectionPool
    {
        static readonly Logger log = new Logger();
        static IDictionary<string, int> Ready_Count = new Dictionary<string, int>();
        static object lockObject = new object();
        //int totalConnections;
        public ClientConfig Config { get; private set; }

        public ConnectionPool(ClientConfig config)
        {
            if (null == config) throw new ArgumentNullException("config", "config cannot be null.");
            this.Config = config;
        }

        static Hashtable Pools = new Hashtable();

        static ConnectionPool GetPool(ClientConfig config, Uri uri)
        {
            ConnectionPool pool = Pools[uri] as ConnectionPool;

            if (null == pool)
            {
                pool = new ConnectionPool(config);

                lock (Pools)
                {
                    Pools.Add(uri, pool);
                }
            }

            return pool;
        }

        public void Checkin(Connection conn)
        {
            Pool pool = Pool.GetPool(conn.Uri, this.Config);
            pool.CheckIn(conn);
        }

        public Connection Checkout(string host, int port, RequestFormatType requestFormatType)
        {
            string negString = getNegString(requestFormatType);
            return Checkout(host, port, negString);
        }

        private string getNegString(RequestFormatType requestFormatType)
        {

            /*
    VOLDEMORT_V0("vp0", "voldemort-native-v0"),
    VOLDEMORT_V1("vp1", "voldemort-native-v1"),
    VOLDEMORT_V2("vp2", "voldemort-native-v2"),
    PROTOCOL_BUFFERS("pb0", "protocol-buffers-v0"),
    ADMIN_PROTOCOL_BUFFERS("ad1", "admin-v1");
             * */

            string value = null;

            switch (requestFormatType)
            {
                case RequestFormatType.ADMIN_HANDLER:
                    value = "ad1";
                    break;
                case RequestFormatType.PROTOCOL_BUFFERS:
                    value = "pb0";
                    break;
                default:
                    throw new NotSupportedException("RequestFormatType of \"" + requestFormatType + "\" is not supported.");
            }



            return value;
        }

        public Connection Checkout(string host, int port, string negString)
        {
            UriBuilder builder = new UriBuilder();
            builder.Scheme = "tcp";
            builder.Host = host;
            builder.Port = port;
            builder.Path = negString;

            Pool pool = Pool.GetPool(builder.Uri, this.Config);
            return pool.Checkout();
        }
    }
}
