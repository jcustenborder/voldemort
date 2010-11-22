using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Voldemort.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("bootstrap urls must be passed.");
                
                return;
            }

            const string TESTSTORE = "wsm.deal";

            ClientConfig config = new ClientConfig();
            config.BootstrapUrls.AddRange(args);
            config.ConnectionTimeoutMs = 30 * 1000;
            config.SocketTimeoutMs = 30 * 1000;


            Console.WriteLine("BootstrapUrls:");
            foreach (string bootstrapUrl in config.BootstrapUrls)
            {
                Console.Write("\t");
                Console.WriteLine(bootstrapUrl);
            }

            AbstractStoreClientFactory factory = new SocketStoreClientFactory(config);
            
            Console.WriteLine("Testing with Store{0}\t{1}", Environment.NewLine, TESTSTORE);
            

            StoreClient client = factory.GetStoreClient(TESTSTORE);

            Random random = new Random();

            byte[] key = Encoding.UTF8.GetBytes("test");
            byte[] newValue = new byte[100 * 1024];

            random.NextBytes(newValue);

            client.Put(key, newValue);
            
            int[] RequestSizes = new int[] { 1024, 1024 * 10, 1024 * 100, 1024 * 500};

            Console.WriteLine("Get Tests");
            const int REQUESTS = 1000;


            foreach (int RequestSize in RequestSizes)
            {
                byte[] buffer = new byte[RequestSize];
                random.NextBytes(buffer);

                client.Put(key, buffer);

               
                long totalTicks = 0;
                

                int Top = Console.CursorTop;
                
                for (int i = 0; i < REQUESTS; i++)
                {
                    //break;
                    // byte[] newValue = Encoding.UTF8.GetBytes(

                    // do some random pointless operations
                    Stopwatch watch = Stopwatch.StartNew();
                    Versioned value = client.Get(key);
                    watch.Stop();
                    totalTicks += watch.ElapsedTicks;
                }
                
                TimeSpan getRequestSpan = new TimeSpan(totalTicks);

                Console.WriteLine("\tBody Size {0:###,###,###,###,##0}k {1:###,###,###,##0}k requests in {2} {3:###,##0} Requests per sec.", RequestSize / 1000, REQUESTS / 1000, getRequestSpan, 
                    getRequestsPerSecond(REQUESTS, getRequestSpan)
                    );
            }

            RequestSizes = new int[] { 1024, 1024 * 10, 1024 * 100 };
            Console.WriteLine("GetAll Tests");

            foreach (int RequestSize in RequestSizes)
            {
                const int CLIENTKEYS = 50;
                List<byte[]> getAllTestKeys = new List<byte[]>();
                for (int i = 0; i < CLIENTKEYS; i++)
                {
                    string test = string.Format("key{0}", i);
                    byte[] testKey = Encoding.UTF8.GetBytes(test);
                    getAllTestKeys.Add(testKey);
                        byte[] buffer = new byte[RequestSize];
                        random.NextBytes(buffer);
                        client.Put(testKey, buffer);
                }

                long totalTicks = 0;
                for (int i = 0; i < REQUESTS; i++)
                {
                    Stopwatch watch = Stopwatch.StartNew();
                    IList<KeyedVersions> values = client.GetAll(getAllTestKeys);
                    watch.Stop();
                    totalTicks += watch.ElapsedTicks;
                }
                TimeSpan getAllRequestSpan = new TimeSpan(totalTicks);

                Console.WriteLine("\tBody Size {0:###,###,###,###,##0}k {1:###,###,###,##0}k requests ({4:###,##0} objects) in {2} {3:###,##0} Requests per sec.", RequestSize / 1000, REQUESTS / 1000, getAllRequestSpan,
                        getRequestsPerSecond(REQUESTS, getAllRequestSpan),
                        REQUESTS * CLIENTKEYS
                    );
            }
            


            //const string FORMAT = "Performed {0:###,###,###,##0} {1} requests in {2}";

            //Console.WriteLine(FORMAT, REQUESTS,  "get", getRequestSpan);
            //Console.WriteLine("Package length {0} bytes", newValue.Length);
            //double requestsPerSecond = ((double)REQUESTS / getRequestSpan.TotalSeconds);
            //Console.WriteLine("{0} requests per second", requestsPerSecond);

            //Console.WriteLine(FORMAT, REQUESTS, "getAll", getAllRequestSpan);
            //Console.WriteLine("Package length {0} bytes", 1024);
            //requestsPerSecond = ((double)REQUESTS / getAllRequestSpan.TotalSeconds);
            //Console.WriteLine("{0} requests per second", requestsPerSecond);
        }

        private static double getRequestsPerSecond(int REQUESTS, TimeSpan getRequestSpan)
        {
            return (double)REQUESTS / getRequestSpan.TotalSeconds;
        }
    }
}
