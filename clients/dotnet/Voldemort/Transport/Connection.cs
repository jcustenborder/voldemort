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
        public bool Errored { get; set; }

        public ClientConfig Config { get; private set; }
        public Connection(Uri uri, ClientConfig config)
        {
            if (null == uri) throw new ArgumentNullException("uri", "uri cannot be null.");
            
            if (null == config) throw new ArgumentNullException("config", "config cannot be null.");
            this.Config = config;

            this.Socket= new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.Socket.NoDelay = true;
            this.Socket.SendTimeout = Config.SocketTimeoutMs;
            this.Socket.ReceiveTimeout = Config.SocketTimeoutMs;

            this.Uri = uri;

            UriBuilder builder=new UriBuilder(uri);
            this.Host = builder.Host;
            this.Port = builder.Port;
            this.NegotiationString = builder.Path.Trim('/');
        }

        public Socket Socket { private set; get; }
        public Stream Stream { private set; get; }

        public void Connect()
        {
            IAsyncResult result = this.Socket.BeginConnect(this.Host, this.Port, null, null);

            if (!result.AsyncWaitHandle.WaitOne(this.Config.ConnectionTimeoutMs, false))
            {
                throw new TimeoutException();
            }

            this.Socket.EndConnect(result);

            write(this.NegotiationString);
            byte[] buffer = new byte[2];
            read(buffer, buffer.Length);

            if (buffer[0] != (byte)'o' && buffer[1] != (byte)'k')
            {
                throw new UnreachableStoreException("Failed to negotiate protocol with server");
            }

            this.Stream = new NetworkStream(this.Socket, FileAccess.ReadWrite);
        }

        public void Close()
        {
            const string PREFIX = "Close() - ";
            if (!this.Errored)
            {
                Pool pool = Pool.GetPool(this.Uri, this.Config);
                pool.CheckIn(this);
            }
            else
            {
                try
                {
                    if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "Error encountered on connection. Removing connection {0} to {1}", this.Socket.LocalEndPoint, this.Socket.RemoteEndPoint);
                    this.Stream.Dispose();
                    this.Socket.Close();
                }
                catch (Exception ex)
                {
                    if (log.IsDebugEnabled) log.Debug(PREFIX + "Exception thrown while cleaning up connection", ex);
                }
            }
        }

        private void read(byte[] buffer, int length)
        {
            int read = 0;
            int offset = 0;
            int size = length;
            while (read < length)
            {
                int len = this.Socket.Receive(buffer, offset, size, SocketFlags.None);
                read += len;
                size -= len;
                offset += len;
            }
        }

        private int read_some(byte[] buffer, int length)
        {
            throw new NotImplementedException();
        }

        private int write(byte[] buffer, int length)
        {
            int written = 0;
            int offset=0;
            int size = length;
            while (written < length)
            {
                int len = this.Socket.Send(buffer, offset, size, SocketFlags.None);
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
            Close();
        }


    }
}
