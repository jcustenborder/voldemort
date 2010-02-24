using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ProtoBuf;
namespace Voldemort
{
    class ProtocolBuffersRequestFormat : RequestFormat
    {
        //enum OpCodes: byte
        //{
        //    GET_OP_CODE = 1,
        //    PUT_OP_CODE = 2,
        //    DELETE_OP_CODE = 3,
        //    GET_ALL_OP_CODE = 4,
        //    GET_PARTITION_AS_STREAM_OP_CODE = 4,
        //    PUT_ENTRIES_AS_STREAM_OP_CODE = 5,
        //    UPDATE_METADATA_OP_CODE = 6,
        //    SERVER_STATE_CHANGE_OP_CODE = 8,
        //    REDIRECT_GET_OP_CODE = 9,
        //}

        public override string getNegotiationString()
        {
            return "pb0";
        }

        public override void writeGetRequest(Stream iostr, string storeName, byte[] key, bool shouldReroute)
        {
            VoldemortRequest request = new VoldemortRequest();
            request.type = RequestType.GET;
            request.store = storeName;
            request.should_route = shouldReroute;
            request.get = new GetRequest() { key = key };
            Serializer.SerializeWithLengthPrefix(iostr, request, PrefixStyle.Fixed32BigEndian);
        }

        public override IList<Versioned> readGetResponse(Stream iostr)
        {
            GetResponse response = Serializer.DeserializeWithLengthPrefix<GetResponse>(iostr, PrefixStyle.Fixed32BigEndian);
            checkThrowError(response.error);
            return response.versioned;
        }

        private void checkThrowError(Error error)
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

        public override void readPutResponse(Stream iostr)
        {
            PutResponse response = Serializer.DeserializeWithLengthPrefix<PutResponse>(iostr, PrefixStyle.Fixed32BigEndian);
            checkThrowError(response.error);
        }

        public override void writePutRequest(Stream iostr, string storeName, byte[] key, byte[] value, VectorClock version, bool shouldReroute)
        {
            VoldemortRequest request = new VoldemortRequest();
            request.type = RequestType.PUT;
            request.store = storeName;
            request.should_route = shouldReroute;
            request.put = new PutRequest() { key = key, versioned = new Versioned() { value = value, version = version } };
            Serializer.SerializeWithLengthPrefix(iostr, request, PrefixStyle.Fixed32BigEndian);
        }

        public override void writeDeleteRequest(Stream iostr, string storeName, byte[] key, VectorClock version, bool shouldReroute)
        {
            VoldemortRequest request = new VoldemortRequest();
            request.type = RequestType.DELETE;
            request.store = storeName;
            request.should_route = shouldReroute;
            request.delete = new DeleteRequest() { key = key, version = version };
            Serializer.SerializeWithLengthPrefix(iostr, request, PrefixStyle.Fixed32BigEndian);
        }

        public override bool readDeleteResponse(Stream iostr)
        {
            DeleteResponse response = Serializer.DeserializeWithLengthPrefix<DeleteResponse>(iostr, PrefixStyle.Fixed32BigEndian);
            checkThrowError(response.error);
            return response.success;
        }

        public override void writeGetAllRequest(Stream iostr, string storeName, IEnumerable<byte[]> keys, bool shouldReroute)
        {
            VoldemortRequest request = new VoldemortRequest();
            request.type = RequestType.GET_ALL;
            request.store = storeName;
            request.should_route = shouldReroute;
            request.getAll = new GetAllRequest();
            request.getAll.keys.AddRange(keys);
            Serializer.SerializeWithLengthPrefix(iostr, request, PrefixStyle.Fixed32BigEndian);
        }

        public override List<KeyedVersions> readGetAllResponse(Stream iostr)
        {
            GetAllResponse response = Serializer.DeserializeWithLengthPrefix<GetAllResponse>(iostr, PrefixStyle.Fixed32BigEndian);
            checkThrowError(response.error);
            return response.values;
        }
    }
}
