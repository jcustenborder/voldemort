using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Voldemort
{
    /// <summary>
    /// Represent a Voldemort cluster configuration
    /// </summary>
    class Cluster
    {
        private IDictionary<int, Node> nodesById;
        /// <summary>
        /// Construct a new Cluster object from an XML string
        /// </summary>
        /// <param name="clusterXml">the Xml string</param>
        public Cluster(string clusterXml)
        {
            if (string.IsNullOrEmpty(clusterXml)) throw new ArgumentNullException("clusterXml", "clusterXml cannot be null.");
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(clusterXml);

            Dictionary<int, Node> nodeLookup = new Dictionary<int, Node>();
            

            foreach (XmlElement serverElement in xmldoc.SelectNodes("//server"))
            {
                int id = 0, httpPort = 0, socketPort = 0, adminPort = 0;
                string host = getChildElement(serverElement, "host");
                if (!int.TryParse(getChildElement(serverElement, "id"), out id))
                {

                }
                if (!int.TryParse(getChildElement(serverElement, "http-port"), out httpPort))
                {

                }
                if (!int.TryParse(getChildElement(serverElement, "socket-port"), out socketPort))
                {

                }
                if (!int.TryParse(getChildElement(serverElement, "admin-port"), out adminPort))
                {

                }

                string[] parts = getChildElement(serverElement, "partitions").Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                List<int> partitions = new List<int>();
                foreach (string part in parts)
                {
                    int partition = 0;
                    if (!int.TryParse(part, out partition))
                    {

                    }
                    partitions.Add(partition);
                }

                Node node = new Node(id, host, httpPort, socketPort, adminPort, partitions);
                nodeLookup.Add(node.Id, node);
            }

            this.nodesById = nodeLookup;
        }

        static string getChildElement(XmlElement element, string childElementName)
        {
            if (null == element) throw new ArgumentNullException("element", "element cannot be null.");
            if (string.IsNullOrEmpty(childElementName)) throw new ArgumentNullException("childElementName", "childElementName cannot be null.");

            XmlElement childElement = element.SelectSingleNode(childElementName) as XmlElement;
            return childElement.InnerText;
        }

        public Node GetNodeById(int nodeId)
        {
            Node node = null;
            if (nodesById.TryGetValue(nodeId, out node))
            {
                return node;
            }

            return null;
        }

        public int NodeCount
        {
            get
            {
                return nodesById.Count;
            }
        }

        public IDictionary<int, Node> NodeMap
        {
            get { return nodesById; }
        }
    }
}
