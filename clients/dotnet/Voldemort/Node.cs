using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Voldemort
{
    public class Node
    {
        public Node()
        {
            this._NodeDown = false;
        }
        private static readonly Logger log = new Logger();
        [XmlElement("id")]
        public int ID { get; set; }
        [XmlElement("host")]
        public string Host { get; set; }
        [XmlElement("http-port")]
        public int HttpPort { get; set; }
        [XmlElement("socket-port")]
        public int SocketPort { get; set; }
        
        [XmlElement("admin-port")]
        public int AdminPort { get; set; }

        [XmlElement("partitions")]
        public string PartitionsValue { get; set; }

        private bool _NodeDown = false;
        [XmlIgnore]
        public bool IsAvailable
        {
            get
            {
                if (!_NodeDown)
                    return true;

                TimeSpan span = DateTime.UtcNow - NodeDownTime;

                if (span > NodeRetyInterval)
                {
                    _NodeDown = false;
                }

                return !_NodeDown;
            }
        }

        private static TimeSpan NodeRetyInterval = new TimeSpan(0, 5, 0);
        private DateTime NodeDownTime = DateTime.MinValue;

        [XmlIgnore]
        internal long Requests { get; set; }

        internal void SetDown()
        {
            NodeDownTime = DateTime.UtcNow;
            _NodeDown = true;
        }
        internal void SetAvailable()
        {
            NodeDownTime = DateTime.MinValue;
            _NodeDown = false;
        }


        public int[] GetPartitions()
        {
            const string PREFIX = "GetPartitions() - ";
            if (string.IsNullOrEmpty(this.PartitionsValue))
                return new int[0];
            string[] parts = this.PartitionsValue.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            List<int> partitions = new List<int>();
            for (int i = 0; i < parts.Length; i++)
            {
                string parValue = parts[i].Trim();
                int value = 0;
                if (!int.TryParse(parValue, out value))
                {
                    if (log.IsWarnEnabled) log.WarnFormat(PREFIX + "Error parsing partition value for host \"{0}\" value = \"{1}\"", this.Host, parValue);
                    continue;
                }
                partitions.Add(value);
            }

            return partitions.ToArray();
        }
        public void SetPartitions(IEnumerable<int> partitions)
        {
            if (null == partitions) throw new ArgumentNullException("partitions", "partitions cannot be null.");

            List<string> values = new List<string>();
            foreach (int partition in partitions)
                values.Add(partition.ToString());

            this.PartitionsValue = string.Join(", ", values.ToArray());
        }

        private static Random Random = new Random();

        public static IList<Node> GetAvailableNodes(IList<Node> nodes)
        {
            if (null == nodes) throw new ArgumentNullException("nodes", "nodes cannot be null.");

            List<Node> availableNodes = new List<Node>();
            foreach (Node node in nodes)
            {
                if (node.IsAvailable)
                {
                    availableNodes.Add(node);
                }
            }

            return availableNodes;
        }

        internal Uri GetAdminUri()
        {
            UriBuilder builder = new UriBuilder();
            builder.Scheme = "tcp";
            builder.Host = this.Host;
            builder.Port = this.AdminPort;
            return builder.Uri;

        }
    }
}
