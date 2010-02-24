using System;
using System.Collections.Generic;
using System.Threading;
using Hashtable = System.Collections.Hashtable;

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
            connection.connect();
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
                pool = new Pool(uri, config);

                lock (Pools)
                {
                    Pools.Add(uri, pool);
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

        public Connection Checkout(string host, int port, string negString)
        {
            UriBuilder builder = new UriBuilder();
            builder.Scheme = "tcp";
            builder.Host = host;
            builder.Port = port;
            builder.Path = negString;

            Pool pool = Pool.GetPool(builder.Uri, this.Config);
            return pool.Checkout();


            //Connection connRet = new Connection(builder.Uri, this.Config);

            //try
            //{
            //    connRet.connect();
            //}
            //catch (Exception ex)
            //{
            //    if (log.IsErrorEnabled) log.Error("Exception while connecting", ex);
            //    throw;
            //}


            //return connRet;
        }

        public void Checkin(Connection conn)
        {
            Pool pool = Pool.GetPool(conn.Uri, this.Config);
            pool.CheckIn(conn);

        }
    }
}
