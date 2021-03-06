﻿using System;
using System.Collections.Generic;
using System.Text;


namespace Voldemort
{
    class RoutedStore:Store
    {
        private static readonly Logger log = new Logger();
        private string _storeName;
        private ClientConfig _config;
        private Cluster _cluster;
        private IDictionary<int, Store> _clusterMap;
        private RoutingStrategy _routingStrategy;


        public RoutedStore(string storeName,
                    ClientConfig config,
                    Cluster cluster,
                    IDictionary<int, Store> clusterMap,
                    RoutingStrategy routingStrategy)
        {
            if (string.IsNullOrEmpty(storeName)) throw new ArgumentNullException("storeName", "storeName cannot be null.");
            if (null == config) throw new ArgumentNullException("config", "config cannot be null.");
            if (null == cluster) throw new ArgumentNullException("cluster", "cluster cannot be null.");
            if (null == clusterMap) throw new ArgumentNullException("clusterMap", "clusterMap cannot be null.");
            if (null == routingStrategy) throw new ArgumentNullException("routingStrategy", "routingStrategy cannot be null.");


            _storeName = storeName;
            _config = config;
            _cluster = cluster;
            _clusterMap = clusterMap;
            _routingStrategy = routingStrategy;
        }
        #region Store Members

        static bool doGetFromStore(byte[] key,
                           Node node,
                           Store store,
            out IList<Versioned> result)
        {
            result = null;
            try
            {
                result = store.get(key);
                node.SetAvailable(); //TODO: Check the cpp source for node and what's it's doing.
                return true;
            }
            catch (UnreachableStoreException ex)
            {
                if (log.IsErrorEnabled) log.Error("Exception while calling store.get on " + node, ex);
                node.SetDown();
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                if (log.IsErrorEnabled) log.Error("Exception while calling store.get on " + node, ex);
                node.SetDown();
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled) log.Error("Exception while calling store.get on " + node, ex);
                node.SetDown();
            }
            finally
            {
                node.Requests++;
            }
            return false;
        }
        static bool doGetAllFromStore(IEnumerable<byte[]> key,
                                   Node node,
                                   Store store,
                    out IList<KeyedVersions> result)
        {
            result = null;
            try
            {
                result = store.getAll(key);
                node.SetAvailable(); //TODO: Check the cpp source for node and what's it's doing.
                return true;
            }
            catch (UnreachableStoreException ex)
            {
                if (log.IsErrorEnabled) log.Error("Exception while calling store.get on " + node, ex);
                node.SetDown();
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                if (log.IsErrorEnabled) log.Error("Exception while calling store.get on " + node, ex);
                node.SetDown();
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled) log.Error("Exception while calling store.get on " + node, ex);
                node.SetDown();
            }
            finally
            {
                node.Requests++;
            }
            return false;
        }
        static bool doPutFromStore(byte[] key, 
                           Versioned value,
                           Node node,
                           Store store) 
        {
            try 
            {
                store.put(key, value);
                node.SetAvailable(); //TODO: Check the cpp source for node and what's it's doing.
                return true;
            }
            catch (UnreachableStoreException ex)
            {
                if (log.IsErrorEnabled) log.Error("Exception while calling store.get on " + node, ex);
                node.SetDown();
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                if (log.IsErrorEnabled) log.Error("Exception while calling store.get on " + node, ex);
                node.SetDown();
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled) log.Error("Exception while calling store.get on " + node, ex);
                node.SetDown();
            }
            finally
            {
                node.Requests++;
            }
            return false;
        }

        static bool doDeleteFromStore(byte[] key,
                              Versioned version,
                              Node node,
                              Store store,
                              out bool result)
        {
            try
            {
                result = store.deleteKey(key, version);
                node.SetAvailable(); //TODO: Check the cpp source for node and what's it's doing.
                return true;
            }
            catch (UnreachableStoreException ex)
            {
                if (log.IsErrorEnabled) log.Error("Exception while calling store.get on " + node, ex);
                node.SetDown();
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                if (log.IsErrorEnabled) log.Error("Exception while calling store.get on " + node, ex);
                node.SetDown();
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled) log.Error("Exception while calling store.get on " + node, ex);
                node.SetDown();
            }
            finally
            {
                node.Requests++;
            }
            result = false;
            return false;
        }

        public IList<Versioned> get(byte[] key)
        {
            bool status = false;
            IList<Versioned> result = null;

            IList<Node> prefList = _routingStrategy.routeRequest(key);
            IList<Node> availableNodes = Node.GetAvailableNodes(prefList);

            foreach (Node node in availableNodes)
            {
                status = doGetFromStore(key, node, _clusterMap[node.ID], out result);

                if (status)
                    return result;
            }

            foreach (Node node in _cluster.NodeMap.Values)
            {
                status = doGetFromStore(key, node, _clusterMap[node.ID], out result);

                if (status)
                    return result;
            }

            throw new InsufficientOperationalNodesException("Could not reach any node for get operation");
        }

        public void put(byte[] key, Versioned value)
        {
            bool status = false;
            
            IList<Node> prefList = _routingStrategy.routeRequest(key);
            IList<Node> availableNodes = Node.GetAvailableNodes(prefList);
            foreach (Node node in availableNodes)
            {
                status = doPutFromStore(key, value, node, _clusterMap[node.ID]);

                if (status)
                    return;
            }

            foreach (Node node in _cluster.NodeMap.Values)
            {
                status = doPutFromStore(key, value, node, _clusterMap[node.ID]);

                if (status)
                    return;
            }

            throw new InsufficientOperationalNodesException("Could not reach any node for get operation");
        }

        public bool deleteKey(byte[] key, Versioned version)
        {
            bool status = false;
            bool result = false;
            IList<Node> prefList = _routingStrategy.routeRequest(key);
            IList<Node> availableNodes = Node.GetAvailableNodes(prefList);
            foreach (Node node in availableNodes)
            {
                status = doDeleteFromStore(key, version, node, _clusterMap[node.ID], out result);

                if (status)
                    return result;
            }

            foreach (Node node in _cluster.NodeMap.Values)
            {
                status = doDeleteFromStore(key, version, node, _clusterMap[node.ID], out result);

                if (status)
                    return result;
            }

            throw new InsufficientOperationalNodesException("Could not reach any node for get operation");
        }

        public string Name
        {
            get { return _storeName; }
        }

        public void close()
        {
            foreach (Store store in _clusterMap.Values)
            {
                store.close();
            }
        }

        #endregion

        


        public IList<KeyedVersions> getAll(IEnumerable<byte[]> keys)
        {
            bool status = false;
            IList<Node> availableNodes = Node.GetAvailableNodes(_cluster.Servers);
            IList<KeyedVersions> result = null;
            foreach (Node node in availableNodes)
            {
                status = doGetAllFromStore(keys, node, _clusterMap[node.ID], out result);

                if (status)
                    return result;
            }

            throw new InsufficientOperationalNodesException("Could not reach any node for get operation");
        }

    

    
    }

}
