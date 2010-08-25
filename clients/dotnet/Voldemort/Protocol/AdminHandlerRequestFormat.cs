using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ProtoBuf;

namespace Voldemort.Protocol
{
    class AdminHandlerRequestFormat : RequestFormat
    {
        public override string getNegotiationString()
        {
            return "pb0";
        }

        public void writeGetMetadataRequest(Stream iostr, byte[] key)
        {
            VoldemortAdminRequest request = new VoldemortAdminRequest();
            request.type = AdminRequestType.GET_METADATA;
            request.get_metadata = new GetMetadataRequest();
            request.get_metadata.key = key;
            Serializer.SerializeWithLengthPrefix(iostr, request, PrefixStyle.Fixed32BigEndian);
        }

        public Versioned readGetMetadataResponse(Stream iostr)
        {
            GetMetadataResponse response = Serializer.DeserializeWithLengthPrefix<GetMetadataResponse>(iostr, PrefixStyle.Fixed32BigEndian);
            checkThrowError(response.error);
            return response.version;
        }

        public void writeUpdatePartitionEntries(Stream iostr, VoldemortFilter filter, PartitionEntry entry, string store) 
        {
            VoldemortAdminRequest request = new VoldemortAdminRequest();
            request.type = AdminRequestType.UPDATE_PARTITION_ENTRIES;
            request.update_partition_entries = new UpdatePartitionEntriesRequest();
            request.update_partition_entries.filter = filter;
            request.update_partition_entries.partition_entry = entry;
            request.update_partition_entries.store = store;
            Serializer.SerializeWithLengthPrefix(iostr, request, PrefixStyle.Fixed32BigEndian);
        }
        public void readUpdatePartitionEntries(Stream iostr)
        {
            UpdatePartitionEntriesResponse response = Serializer.DeserializeWithLengthPrefix<UpdatePartitionEntriesResponse>(iostr, PrefixStyle.Fixed32BigEndian);
            checkThrowError(response.error);            
        }

        public void writeFetchPartitionEntries(Stream iostr, bool fetch_master_entries, bool fetch_values, VoldemortFilter filter, List<int> partitions, string store) 
        {
            VoldemortAdminRequest request = new VoldemortAdminRequest();
            request.type = AdminRequestType.FETCH_PARTITION_ENTRIES;
            request.fetch_partition_entries = new FetchPartitionEntriesRequest();
            request.fetch_partition_entries.fetch_master_entries = fetch_master_entries;
            request.fetch_partition_entries.fetch_values = fetch_values;
            request.fetch_partition_entries.filter = filter;
            request.fetch_partition_entries.partitions.AddRange(partitions);
            request.fetch_partition_entries.store = store;
            Serializer.SerializeWithLengthPrefix(iostr, request, PrefixStyle.Fixed32BigEndian);
        }
        public FetchPartitionEntriesResponse readFetchPartitionEntries(Stream iostr)
        {
            FetchPartitionEntriesResponse response = Serializer.DeserializeWithLengthPrefix<FetchPartitionEntriesResponse>(iostr, PrefixStyle.Fixed32BigEndian);
            checkThrowError(response.error);
            return response;
        }
        public void writeDeletePartitionEntries(Stream iostr, VoldemortFilter filter, List<int> partitions, string store) 
        {
            VoldemortAdminRequest request = new VoldemortAdminRequest();
            request.type = AdminRequestType.DELETE_PARTITION_ENTRIES;
            request.delete_partition_entries.filter = filter;
            request.delete_partition_entries.partitions.AddRange(partitions);
            request.delete_partition_entries.store = store;
            Serializer.SerializeWithLengthPrefix(iostr, request, PrefixStyle.Fixed32BigEndian);
        }
        public int readDeletePartitionEntries(Stream iostr)
        {
            DeletePartitionEntriesResponse response = Serializer.DeserializeWithLengthPrefix<DeletePartitionEntriesResponse>(iostr, PrefixStyle.Fixed32BigEndian);
            checkThrowError(response.error);
            return response.count;
        }

        public void writeInitiateFetchAndUpdate(Stream iostr, VoldemortFilter filter, int node_id, List<int> partitions, string store) 
        {
            VoldemortAdminRequest request = new VoldemortAdminRequest();
            request.type = AdminRequestType.INITIATE_FETCH_AND_UPDATE;
            request.initiate_fetch_and_update.filter = filter;
            request.initiate_fetch_and_update.node_id = node_id;
            request.initiate_fetch_and_update.partitions.AddRange(partitions);
            request.initiate_fetch_and_update.store = store;
            Serializer.SerializeWithLengthPrefix(iostr, request, PrefixStyle.Fixed32BigEndian);
        }
        public int readInitiateFetchAndUpdate(Stream iostr)
        {
            AsyncOperationStatusResponse response = readAsyncOperationStatus(iostr);
            return response.request_id;
        }

        public AsyncOperationStatusResponse readAsyncOperationStatus(Stream iostr) 
        {
            AsyncOperationStatusResponse response = Serializer.DeserializeWithLengthPrefix<AsyncOperationStatusResponse>(iostr, PrefixStyle.Fixed32BigEndian);
            checkThrowError(response.error);
            return response;
        }
        public void writeInitiateRebalanceNode(Stream iostr, int attempt, List<int> deletePartitions, int donor_id, List<int> partitions, int stealer_id, List<string> unbalanced_store) 
        {
            VoldemortAdminRequest request = new VoldemortAdminRequest();
            request.type = AdminRequestType.INITIATE_REBALANCE_NODE;
            request.initiate_rebalance_node = new InitiateRebalanceNodeRequest();
            request.initiate_rebalance_node.attempt = attempt;
            request.initiate_rebalance_node.deletePartitions.AddRange(deletePartitions);
            request.initiate_rebalance_node.donor_id = donor_id;
            request.initiate_rebalance_node.partitions.AddRange(partitions);
            request.initiate_rebalance_node.stealer_id = stealer_id;
            request.initiate_rebalance_node.unbalanced_store.AddRange(unbalanced_store);
            Serializer.SerializeWithLengthPrefix(iostr, request, PrefixStyle.Fixed32BigEndian);
        }
        public int readInitiateRebalanceNode(Stream iostr)
        {
            AsyncOperationStatusResponse response = readAsyncOperationStatus(iostr);
            return response.request_id;
        }

        public void writeAsyncOperationStop(Stream iostr, int request_id) 
        {
            VoldemortAdminRequest request = new VoldemortAdminRequest();
            request.type = AdminRequestType.ASYNC_OPERATION_STOP;
            request.async_operation_stop = new AsyncOperationStopRequest();
            request.async_operation_stop.request_id = request_id;
            Serializer.SerializeWithLengthPrefix(iostr, request, PrefixStyle.Fixed32BigEndian);            
        }
        public void readAsyncOperationStop(Stream iostr)
        {
            AsyncOperationStopResponse response = Serializer.DeserializeWithLengthPrefix<AsyncOperationStopResponse>(iostr, PrefixStyle.Fixed32BigEndian);
            checkThrowError(response.error);
        }

        public void writeAsyncOperationList(Stream iostr, int request_id, bool show_complete) 
        {
            VoldemortAdminRequest request = new VoldemortAdminRequest();
            request.type = AdminRequestType.ASYNC_OPERATION_LIST;
            request.async_operation_list = new AsyncOperationListRequest();
            request.async_operation_list.request_id = request_id;
            request.async_operation_list.show_complete = show_complete;
            Serializer.SerializeWithLengthPrefix(iostr, request, PrefixStyle.Fixed32BigEndian);            
        }
        public List<int> readAsyncOperationList(Stream iostr)
        {
            AsyncOperationListResponse response = Serializer.DeserializeWithLengthPrefix<AsyncOperationListResponse>(iostr, PrefixStyle.Fixed32BigEndian);
            checkThrowError(response.error);
            return response.request_ids;
        }

        public void writeTruncateEntries(Stream iostr, string store) 
        {
            VoldemortAdminRequest request = new VoldemortAdminRequest();
            request.type = AdminRequestType.TRUNCATE_ENTRIES;
            request.truncate_entries = new TruncateEntriesRequest();
            request.truncate_entries.store = store;
            Serializer.SerializeWithLengthPrefix(iostr, request, PrefixStyle.Fixed32BigEndian);            
        }
        public void readTruncateEntries(Stream iostr)
        {
            TruncateEntriesResponse response = Serializer.DeserializeWithLengthPrefix<TruncateEntriesResponse>(iostr, PrefixStyle.Fixed32BigEndian);
            checkThrowError(response.error);
        }

        public void writeAddStore(Stream iostr, string storeDefinition) 
        {
            VoldemortAdminRequest request = new VoldemortAdminRequest();
            request.type = AdminRequestType.ADD_STORE;
            request.add_store = new AddStoreRequest();
            request.add_store.storeDefinition = storeDefinition;
            Serializer.SerializeWithLengthPrefix(iostr, request, PrefixStyle.Fixed32BigEndian);            

        }
        public void readAddStore(Stream iostr)
        {
            AddStoreResponse response = Serializer.DeserializeWithLengthPrefix<AddStoreResponse>(iostr, PrefixStyle.Fixed32BigEndian);
            checkThrowError(response.error);
        }

        public void writeDeleteStore(Stream iostr, string storeName) 
        {
            VoldemortAdminRequest request = new VoldemortAdminRequest();
            request.type = AdminRequestType.DELETE_STORE;
            request.delete_store = new DeleteStoreRequest();
            request.delete_store.storeName = storeName;
            Serializer.SerializeWithLengthPrefix(iostr, request, PrefixStyle.Fixed32BigEndian);            
        }
        public void readDeleteStore(Stream iostr) 
        {
            DeleteStoreResponse response = Serializer.DeserializeWithLengthPrefix<DeleteStoreResponse>(iostr, PrefixStyle.Fixed32BigEndian);
            checkThrowError(response.error);
        }

        public void writeFetchStore(Stream iostr, string store_dir, string store_name) 
        {
            VoldemortAdminRequest request = new VoldemortAdminRequest();
            request.type = AdminRequestType.FETCH_STORE;
            request.fetch_store = new FetchStoreRequest();
            request.fetch_store.store_dir = store_dir;
            request.fetch_store.store_name = store_name;
            Serializer.SerializeWithLengthPrefix(iostr, request, PrefixStyle.Fixed32BigEndian);   
        }
        public int readFetchStore(Stream iostr) 
        {
            return readAsyncOperationStatus(iostr).request_id;
        }
        public void writeSwapStore(Stream iostr, string store_dir, string store_name) 
        {
            VoldemortAdminRequest request = new VoldemortAdminRequest();
            request.type = AdminRequestType.SWAP_STORE;
            request.swap_store = new SwapStoreRequest();
            request.swap_store.store_dir = store_dir;
            request.swap_store.store_name = store_name;
            Serializer.SerializeWithLengthPrefix(iostr, request, PrefixStyle.Fixed32BigEndian);
        }
        public void readSwapStore(Stream iostr) 
        {
            SwapStoreResponse response = Serializer.DeserializeWithLengthPrefix<SwapStoreResponse>(iostr, PrefixStyle.Fixed32BigEndian);
            checkThrowError(response.error);
        }
        public void writeRollbackStore(Stream iostr, string store_name) 
        {
            VoldemortAdminRequest request = new VoldemortAdminRequest();
            request.type = AdminRequestType.ROLLBACK_STORE;
            request.rollback_store = new RollbackStoreRequest();
            request.rollback_store.store_name = store_name;
            Serializer.SerializeWithLengthPrefix(iostr, request, PrefixStyle.Fixed32BigEndian);
        }
        public void readRollbackStore(Stream iostr) 
        {
            RollbackStoreResponse response = Serializer.DeserializeWithLengthPrefix<RollbackStoreResponse>(iostr, PrefixStyle.Fixed32BigEndian);
            checkThrowError(response.error);
        }

            private const long INITIAL_DELAY = 250; // Initial delay
            private const long MAX_DELAY = 1000 * 60;

        //public String waitForCompletion(int nodeId, int requestId, long maxWait)
        //{
        //    long delay = INITIAL_DELAY;
        //    TimeSpan span = TimeSpan.FromMilliseconds((double)maxWait);
        //    DateTime waitUntil = DateTime.UtcNow + span;

        //    String description = null;
        //    while (DateTime.UtcNow < waitUntil)
        //    {
        //        try
        //        {
        //            AsyncOperationStatus status = getAsyncRequestStatus(nodeId, requestId);
        //            logger.info("Status for async task " + requestId + " at node " + nodeId + " is "
        //                        + status);
        //            description = status.getDescription();
        //            if (status.hasException())
        //                throw status.getException();

        //            if (status.isComplete())
        //                return status.getStatus();

        //            if (delay < MAX_DELAY)
        //                delay <<= 1;

        //            try
        //            {
        //                Thread.sleep(delay);
        //            }
        //            catch (InterruptedException e)
        //            {
        //                Thread.currentThread().interrupt();
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            throw new VoldemortException("Failed while waiting for async task " + description
        //                                         + " at node " + nodeId + " to finish", e);
        //        }
        //    }
        //    throw new VoldemortException("Failed to finish task requestId: " + requestId
        //                                 + " in maxWait " + maxWait + " " + timeUnit.toString());
        //}



        public override void writeGetRequest(Stream iostr, string storeName, byte[] key, bool shouldReroute)
        {
            throw new NotSupportedException();
        }

        public override IList<Versioned> readGetResponse(Stream iostr)
        {
            throw new NotSupportedException();
        }

        

        public override void readPutResponse(Stream iostr)
        {
            throw new NotSupportedException();
        }

        public override void writePutRequest(Stream iostr, string storeName, byte[] key, byte[] value, VectorClock version, bool shouldReroute)
        {
            throw new NotSupportedException();
        }

        public override void writeDeleteRequest(Stream iostr, string storeName, byte[] key, VectorClock version, bool shouldReroute)
        {
            throw new NotSupportedException();
        }

        public override bool readDeleteResponse(Stream iostr)
        {
            throw new NotSupportedException();
        }

        public override void writeGetAllRequest(Stream iostr, string storeName, IEnumerable<byte[]> keys, bool shouldReroute)
        {
            throw new NotSupportedException();
        }

        public override List<KeyedVersions> readGetAllResponse(Stream iostr)
        {
            throw new NotSupportedException();
        }
    }
}
