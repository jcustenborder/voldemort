using System;
using System.Collections.Generic;
using System.Text;

namespace Voldemort
{

    public interface AdminClient
    {
        Versioned GetMetadata(byte[] key);
        void UpdateMetadata(byte[] key, Voldemort.Versioned versioned);
        void UpdatePartitionEntries(string store, Voldemort.PartitionEntry partition_entry, Voldemort.VoldemortFilter filter);
        void FetchPartitionEntries(System.Collections.Generic.List<int> partitions, string store, Voldemort.VoldemortFilter filter, bool fetch_values, bool fetch_master_entries);
        void DeletePartitionEntries(string store, System.Collections.Generic.List<int> partitions, Voldemort.VoldemortFilter filter);
        void InitiateFetchAndUpdate(int node_id, System.Collections.Generic.List<int> partitions, string store, Voldemort.VoldemortFilter filter);
        void AsyncOperationStatus(int request_id);
        void InitiateRebalanceNode(int stealer_id, int donor_id, System.Collections.Generic.List<int> partitions, int attempt, System.Collections.Generic.List<int> deletePartitions, System.Collections.Generic.List<string> unbalanced_store);
        void AsyncOperationStop(int request_id);
        void AsyncOperationList(int request_id, bool show_complete);
        void TruncateEntries(string store);
        void AddStore(string storeDefinition);
        void DeleteStore(string storeName);
        void FetchStore(string store_name, string store_dir);
        void SwapStore(string store_name, string store_dir);
        void RollbackStore(string store_name);
    }




    public class VoldemortAdminClient : AdminClient
    {

        

        public Versioned GetMetadata(byte[] key)
        {
            GetMetadataRequest request = new GetMetadataRequest();
            request.key = key;

            GetMetadataResponse response = new GetMetadataResponse();
            


            throw new NotImplementedException();
        }

        public void UpdateMetadata(byte[] key, Versioned versioned)
        {
            throw new NotImplementedException();
        }

        public void UpdatePartitionEntries(string store, PartitionEntry partition_entry, VoldemortFilter filter)
        {
            throw new NotImplementedException();
        }

        public void FetchPartitionEntries(List<int> partitions, string store, VoldemortFilter filter, bool fetch_values, bool fetch_master_entries)
        {
            throw new NotImplementedException();
        }

        public void DeletePartitionEntries(string store, List<int> partitions, VoldemortFilter filter)
        {
            throw new NotImplementedException();
        }

        public void InitiateFetchAndUpdate(int node_id, List<int> partitions, string store, VoldemortFilter filter)
        {
            throw new NotImplementedException();
        }

        public void AsyncOperationStatus(int request_id)
        {
            throw new NotImplementedException();
        }

        public void InitiateRebalanceNode(int stealer_id, int donor_id, List<int> partitions, int attempt, List<int> deletePartitions, List<string> unbalanced_store)
        {
            throw new NotImplementedException();
        }

        public void AsyncOperationStop(int request_id)
        {
            throw new NotImplementedException();
        }

        public void AsyncOperationList(int request_id, bool show_complete)
        {
            throw new NotImplementedException();
        }

        public void TruncateEntries(string store)
        {
            throw new NotImplementedException();
        }

        public void AddStore(string storeDefinition)
        {
            throw new NotImplementedException();
        }

        public void DeleteStore(string storeName)
        {
            throw new NotImplementedException();
        }

        public void FetchStore(string store_name, string store_dir)
        {
            throw new NotImplementedException();
        }

        public void SwapStore(string store_name, string store_dir)
        {
            throw new NotImplementedException();
        }

        public void RollbackStore(string store_name)
        {
            throw new NotImplementedException();
        }

        
    }
}
