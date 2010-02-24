using System;
using System.Collections.Generic;
using System.Text;

namespace Voldemort
{
    public interface StoreClientFactory
    {
        StoreClient getStoreClient(string storeName);
        StoreClient getStoreClient(string storeName, InconsistencyResolver resolver);
        Store getRawStore(string storeName, InconsistencyResolver resolver);
    }

    public interface StoreClientFactory<Key, Value>
    {
        StoreClient<Key, Value> getStoreClient(string storeName);
        StoreClient<Key, Value> getStoreClient(string storeName, InconsistencyResolver resolver);
        Store<Key, Value> getRawStore(string storeName, InconsistencyResolver resolver);
    }
}
