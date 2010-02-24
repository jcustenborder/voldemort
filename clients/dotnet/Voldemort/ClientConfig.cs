using System;
using System.Collections.Generic;
using System.Text;

namespace Voldemort
{
    public class ClientConfig
    {
        public ClientConfig()
        {
            this.BootstrapUrls = new List<string>();
        }

        public ClientConfig(ClientConfig cc):this()
        {

        }

        public List<string> BootstrapUrls { get; set; }
        public int MaxConnectionsPerNode { get; set; }
        public int MaxTotalConnections { get; set; }
        public int ConnectionTimeoutMs { get; set; }
        public int SocketTimeoutMs { get; set; }
        public int NodeBannageMs { get; set; }

    }
}
