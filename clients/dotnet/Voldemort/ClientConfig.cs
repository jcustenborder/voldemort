using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Voldemort.Protocol;

namespace Voldemort
{
    public class ClientConfig
    {
        public ClientConfig()
        {
            this.BootstrapUrls = new List<string>();
            this.MaxConnectionsPerNode = 6;
            this.MaxTotalConnections = 500;
            this.ConnectionTimeoutMs = 5000;
            this.SocketTimeoutMs = 5000;
            this.RequestFormatType = RequestFormatType.PROTOCOL_BUFFERS;
        }

        

        [XmlArray("BootstrapUrls")]
        [XmlArrayItem("BootstrapUrl", typeof(string))]
        public List<string> BootstrapUrls { get; set; }
        [XmlAttribute]
        public int MaxConnectionsPerNode { get; set; }
        [XmlAttribute]
        public int MaxTotalConnections { get; set; }
        [XmlAttribute]
        public int ConnectionTimeoutMs { get; set; }
        [XmlAttribute]
        public int SocketTimeoutMs { get; set; }
        [XmlAttribute]
        public RequestFormatType RequestFormatType { get; set; }

        public ClientConfig Clone()
        {
            ClientConfig config = new ClientConfig();
            config.BootstrapUrls.AddRange(this.BootstrapUrls);
            config.MaxConnectionsPerNode = this.MaxConnectionsPerNode;
            config.MaxTotalConnections = this.MaxTotalConnections;
            config.RequestFormatType = this.RequestFormatType;
            config.SocketTimeoutMs = this.SocketTimeoutMs;
            return config;
        }
    }
}
