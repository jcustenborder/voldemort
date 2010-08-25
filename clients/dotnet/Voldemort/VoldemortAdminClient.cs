using System;
using System.Collections.Generic;
using System.Text;

namespace Voldemort
{

    public class VoldemortAdminClient
    {
        public void GetMetadata(byte[] key)
        {
            GetMetadataRequest request = new GetMetadataRequest();
            request.key = key;
            GetMetadataResponse response = new GetMetadataResponse();
            
        }
        public void UpdateMetadata()
        {

        }
        public void UpdatePartitionEntries() { }
        public void FetchPartitionEntries() { }
        public void DeletePartitionEntries() { }
        public void InitiateFetchAndUpdate() { }
        public void AsyncOperationStatus() { }
        public void InitiateRebalanceNode() { }
        public void AsyncOperationStop() { }
        public void AsyncOperationList() { }
        public void TruncateEntries() { }
        public void AddStore() { }
        public void DeleteStore() { }
        public void FetchStore() { }
        public void SwapStore() { }
        public void RollbackStore() { }
    }
}
