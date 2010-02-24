using System;
using System.Collections.Generic;
using System.Text;

namespace Voldemort
{
    class InconsistencyResolvingStore:Store
    {
        private static readonly Logger log = new Logger();
        private Store _substore;
        public InconsistencyResolvingStore(Store substore)
        {
            if (null == substore) throw new ArgumentNullException("substore", "substore cannot be null.");
            _substore = substore;
        }

        

        public IList<Versioned> get(byte[] key)
        {
            IList<Versioned> resultList = _substore.get(key);
            if (resultList != null && resultList.Count > 1)
            {
                try
                {
                    foreach (InconsistencyResolver resolver in _Resolvers)
                    {
                        resolver.resolveConflicts(resultList);
                    }
                }
                catch (Exception ex)
                {
                    if (log.IsErrorEnabled) log.Error("Exception thrown while resolving conflicts", ex);
                    throw;
                }
            }

            return resultList;
        }

        public void put(byte[] key, Versioned value)
        {
            _substore.put(key, value);
        }

        public bool deleteKey(byte[] key, Versioned version)
        {
            return _substore.deleteKey(key, version);
        }

        public string Name
        {
            get { return _substore.Name; }
        }

        public void close()
        {
            _substore.close();
        }

        

        public void addResolver(InconsistencyResolver resolver)
        {
            if (null == resolver) throw new ArgumentNullException("resolver", "resolver cannot be null.");
            _Resolvers.Add(resolver);
        }

        private IList<InconsistencyResolver> _Resolvers = new List<InconsistencyResolver>();



        public IList<KeyedVersions> getAll(IEnumerable<byte[]> keys)
        {
            IList<KeyedVersions> values = _substore.getAll(keys);

            foreach (KeyedVersions kv in values)
            {
                try
                {
                    foreach (InconsistencyResolver resolver in _Resolvers)
                    {
                        resolver.resolveConflicts(kv.versions);
                    }
                }
                catch (Exception ex)
                {
                    if (log.IsErrorEnabled) log.Error("Exception thrown while resolving conflicts", ex);
                    throw;
                }
            }

            return values;
        }

    }
}
