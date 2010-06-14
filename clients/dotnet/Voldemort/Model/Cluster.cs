using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Voldemort.Model
{
    /*   
<cluster>
        <name>mycluster</name>
        <server>
                <id>0</id>
                <host>localhost</host>
                <http-port>8081</http-port>
                <socket-port>6666</socket-port>
                <partitions>0, 1</partitions>
        </server>
</cluster>
     * */


    [XmlRoot("cluster")]
    public class Cluster
    {
        private static readonly Logger log = new Logger();
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(Cluster));
        public Cluster()
        {
            this.Servers = new List<Node>();
        }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("server")]
        public List<Node> Servers { get; set; }


        public static Cluster Load(byte[] buffer)
        {
            if (null == buffer || buffer.Length == 0) throw new ArgumentNullException("buffer", "buffer cannot be null or empty.");
            using (System.IO.MemoryStream iostr = new System.IO.MemoryStream(buffer))
            {
                return Load(iostr);
            }
        }

        public static Cluster Load(System.IO.Stream stream)
        {
            if (null == stream) throw new ArgumentNullException("stream", "stream cannot be null.");
            
            Cluster cluster = Serializer.Deserialize(stream) as Cluster;
            cluster.afterDeserialize();
            return cluster;
        }

        public Node this[int ID]
        {
            get
            {
                Node node = null;
                if (_NodeLookup.TryGetValue(ID, out node))
                {
                    return node;
                }

                return null;


            }
        }

        private IDictionary<int, Node> _NodeLookup;

        public int NodeCount
        {
            get
            {
                return _NodeLookup.Count;
            }
        }

        public IDictionary<int, Node> NodeMap
        {
            get { return _NodeLookup; }
        }


        private void afterDeserialize()
        {
            Dictionary<int, Node> nodeLookup = new Dictionary<int, Node>();

            foreach (Node node in this.Servers)
            {
                nodeLookup.Add(node.ID, node);
            }

            _NodeLookup = nodeLookup;
        }
    }
}
