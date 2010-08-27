using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Voldemort.Protocol
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



    public abstract class RequestFormat
    {


        protected void checkThrowError(Error error)
        {
            if (null == error) return;

            switch (error.error_code)
            {
                case 2:
                    throw new InsufficientOperationalNodesException(error.error_message);
                case 3:
                    throw new StoreOperationFailureException(error.error_message);
                case 4:
                    throw new ObsoleteVersionException(error.error_message);
                case 7:
                    throw new UnreachableStoreException(error.error_message);
                case 8:
                    throw new InconsistentDataException(error.error_message);
                case 9:
                    throw new InvalidMetadataException(error.error_message);
                case 10:
                    throw new PersistenceFailureException(error.error_message);
                case 1:
                case 5:
                case 6:
                default:
                    throw new VoldemortException(error.error_message);
            }

            throw new NotImplementedException("Implement the rest of the checkThrowError method");
        }


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
                case RequestFormatType.ADMIN_HANDLER:
                    format = new AdminHandlerRequestFormat();
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
