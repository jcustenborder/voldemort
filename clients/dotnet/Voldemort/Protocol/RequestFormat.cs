using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Voldemort.Protocol
{
    abstract class RequestFormat
    {
        public enum RequestFormatType
        {
            /** The Version 0 native protocol */
            VOLDEMORT,
            /** The Version 1 native protocol */
            VOLDEMORT_V1,
            /** Protocol buffers */
            PROTOCOL_BUFFERS,
            /** Admin request handler protocol */
            ADMIN_HANDLER
        };

        public static RequestFormat newRequestFormat(RequestFormatType type)
        {
            RequestFormat format = null;

            switch (type)
            {
                case RequestFormatType.VOLDEMORT:
                    format = new VoldemortNativeRequestFormat();
                    break;
                case RequestFormatType.PROTOCOL_BUFFERS:
                    format = new ProtocolBuffersRequestFormat();
                    break;
                default:
                    

                    break;

            }

            if(null==format)
                throw new VoldemortException("Request format type not implemented " + type);


            return format;
        }

        public abstract string getNegotiationString();
        public abstract void writeGetRequest(Stream iostr, string storeName, byte[] key, bool shouldReroute);
        public abstract IList<Versioned> readGetResponse(Stream iostr);
        public abstract void writeGetAllRequest(Stream iostr, string storeName, IEnumerable<byte[]> keys, bool shouldReroute);
        public abstract List<KeyedVersions> readGetAllResponse(Stream iostr);
        public abstract void readPutResponse(Stream iostr);
        public abstract void writePutRequest(Stream iostr, string storeName, byte[] key, byte[] value, VectorClock version, bool shouldReroute);
        public abstract void writeDeleteRequest(Stream iostr, string storeName, byte[] key, VectorClock version, bool shouldReroute);
        public abstract bool readDeleteResponse(Stream iostr);
    }
}
