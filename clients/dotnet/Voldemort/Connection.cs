using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;


namespace Voldemort
{
    class Connection:IDisposable
    {
        private static readonly Logger log = new Logger();
        public Uri Uri { get; private set; }
        public string Host { get; private set; }
        public string NegotiationString { get; private set; }
        public int Port { get; private set; }



        public ClientConfig Config { get; private set; }
        public Connection(Uri uri, ClientConfig config)
        {
            if (null == uri) throw new ArgumentNullException("uri", "uri cannot be null.");
            
            if (null == config) throw new ArgumentNullException("config", "config cannot be null.");
            this.Config = config;

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.NoDelay = true;
            socket.SendTimeout = Config.SocketTimeoutMs;
            socket.ReceiveTimeout = Config.SocketTimeoutMs;

            this.Uri = uri;

            UriBuilder builder=new UriBuilder(uri);
            this.Host = builder.Host;
            this.Port = builder.Port;
            this.NegotiationString = builder.Path.Trim('/');
            

        }

        private Socket socket;

        public void connect()
        {
            IAsyncResult result = socket.BeginConnect(this.Host, this.Port, null, null);

            if (!result.AsyncWaitHandle.WaitOne(this.Config.ConnectionTimeoutMs, false))
            {
                throw new TimeoutException();
            }
            
            socket.EndConnect(result);

            write(this.NegotiationString);
            byte[] buffer = new byte[2];
            read(buffer, buffer.Length);

            if (buffer[0] != (byte)'o' && buffer[1] != (byte)'k')
            {
                throw new UnreachableStoreException("Failed to negotiate protocol with server");
            }

            _Stream = new NetworkStream(this.socket, FileAccess.ReadWrite);
        }

        private NetworkStream _Stream;

        private void read(byte[] buffer, int length)
        {
            int read = 0;
            int offset = 0;
            int size = length;
            while (read < length)
            {
                int len=socket.Receive(buffer, offset, size, SocketFlags.None);
                read += len;
                size -= len;
                offset += len;
            }
        }

        public void close()
        {
            Pool pool = Pool.GetPool(this.Uri, this.Config);
            pool.CheckIn(this);
        }

        /// <summary>
        /// Returns a network stream for the socket. 
        /// </summary>
        /// <returns></returns>
        public Stream get_io_stream()
        {
            return _Stream;
        }

        public int read_some(byte[] buffer, int length)
        {
            throw new NotImplementedException();
        }

        public int write(byte[] buffer, int length)
        {
            int written = 0;
            int offset=0;
            int size = length;
            while (written < length)
            {
                int len = socket.Send(buffer, offset, size, SocketFlags.None);
                written += len;
                size -= len;
                offset += len;
            }
            return length;
        }

        private void write(string s)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(s);
            write(buffer, buffer.Length);
        }

        public void Dispose()
        {
            close();
        }


    }
}
