using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Voldemort
{
    class VoldemortNativeRequestFormat : RequestFormat
    {
        enum OpCodes : byte
        {
             GET_OP_CODE = 1,
             PUT_OP_CODE = 2,
             DELETE_OP_CODE = 3,
             GET_ALL_OP_CODE = 4,
             GET_PARTITION_AS_STREAM_OP_CODE = 4,
             PUT_ENTRIES_AS_STREAM_OP_CODE = 5,
             UPDATE_METADATA_OP_CODE = 6,
             SERVER_STATE_CHANGE_OP_CODE = 8,
             REDIRECT_GET_OP_CODE = 9
        }
        public override string getNegotiationString()
        {
            return "vp0";
        }


        public override void writeGetRequest(Stream iostr, string storeName, byte[] key, bool shouldReroute)
        {
            //byte[] keyBuffer = Encoding.UTF8.GetBytes(key);
            //byte[] storeNameBuffer = Encoding.UTF8.GetBytes(storeName);
            UInt16 storeNameLen = (UInt16)Encoding.UTF8.GetByteCount(storeName);

            BinaryWriter writer = new BinaryWriter(iostr);
            writer.Write((byte)OpCodes.GET_OP_CODE);
            writer.Write(storeNameLen);
            writer.Write(storeName);
            writer.Write(shouldReroute);
            writer.Write(key.Length);
            writer.Write(key);
            writer.Flush();
        }

        public override IList<Versioned> readGetResponse(Stream iostr)
        {
            checkException(iostr);

            throw new NotImplementedException();
        }

        private void checkException(Stream iostr)
        {
            BinaryReader reader = new BinaryReader(iostr);
            short errorcode = reader.ReadInt16();

        }

        public override void readPutResponse(Stream iostr)
        {
            throw new NotImplementedException();
        }

        public override void writePutRequest(Stream iostr, string storeName, byte[] key, byte[] value, VectorClock version, bool shouldReroute)
        {
            throw new NotImplementedException();
        }

        public override void writeDeleteRequest(Stream iostr, string storeName, byte[] key, VectorClock version, bool shouldReroute)
        {
            throw new NotImplementedException();
        }

        public override bool readDeleteResponse(Stream iostr)
        {
            throw new NotImplementedException();
        }

        public override void writeGetAllRequest(Stream iostr, string storeName, IEnumerable<byte[]> keys, bool shouldReroute)
        {
            throw new NotImplementedException();
        }

        public override List<KeyedVersions> readGetAllResponse(Stream iostr)
        {
            throw new NotImplementedException();
        }
    }
}
