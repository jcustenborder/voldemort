using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ProtoBuf;

namespace Voldemort.Protocol
{
    class ProtocolBuffersRequestFormat : RequestFormat
    {
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
