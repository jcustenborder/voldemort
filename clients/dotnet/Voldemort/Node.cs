using System;
using System.Collections.Generic;
using System.Text;

namespace Voldemort
{
    /// <summary>
    /// Represent a Voldemort node configuration
    /// </summary>
    public class Node
    {
        /// <summary>
        /// Construct a new Node object
        /// </summary>
        /// <param name="id">the node ID</param>
        /// <param name="host">the hostname for the node</param>
        /// <param name="httpPort">the HTTP port for the node</param>
        /// <param name="socketPort">the socket port for the node</param>
        /// <param name="adminPort">the admin port for the node</param>
        /// <param name="partitions">the list of partitions hosted on the node</param>
        public Node(int id,
         string host,
         int httpPort,
         int socketPort,
         int adminPort,
         List<int> partitions):this()
        {
            if (string.IsNullOrEmpty(host)) throw new ArgumentNullException("host", "host cannot be null.");
            if (null == partitions) throw new ArgumentNullException("partitions", "partitions cannot be null.");

            this.Id = id;
            this.Host = host;
            this.HttpPort = httpPort;
            this.SocketPort = socketPort;
            this.AdminPort = adminPort;
            this.Partitions = partitions;
        }
        public Node()
        {

        }



        /// <summary>
        /// Node ID for this node
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// Host name for this node
        /// </summary>
        public string Host { get; private set; }
        /// <summary>
        /// HTTP port for this node
        /// </summary>
        public int HttpPort { get; private set; }
        /// <summary>
        /// socket port for this node
        /// </summary>
        public int SocketPort { get; private set; }
        /// <summary>
        /// admin port for this node
        /// </summary>
        public int AdminPort { get; private set; }
        /// <summary>
        /// node is current available in the node list.
        /// </summary>
        public bool IsAvailable { get; set; }
        /// <summary>
        ///Get the system time in milliseconds when this node was last checked.
        /// </summary>
        public int LastChecked { get; private set; }
        /// <summary>
        /// Get the number of milliseconds since the last time this node was checked for availability.
        /// </summary>
        public int MsSinceLastChecked { get; private set; }
        /// <summary>
        /// Partitions this node contains.
        /// </summary>
        public List<int> Partitions { get; private set; }

        internal void setAvailable(bool p)
        {
            throw new NotImplementedException();
        }

        public static IList<Node> GetAvailableNodes(IList<Node> nodes)
        {
            List<Node> availableNodes = new List<Node>();

            foreach (Node node in nodes)
                if (node.IsAvailable)
                    availableNodes.Add(node);

            return availableNodes;
        }
    }

    public class NodeComparer : IEqualityComparer<Node>, IComparer<Node>
    {

        public bool Equals(Node x, Node y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(Node obj)
        {
            return obj.Id.GetHashCode();
        }

        
        public int Compare(Node x, Node y)
        {
            return x.Id.CompareTo(y.Id);
        }

        public static readonly NodeComparer Instance = new NodeComparer();
    }
}
