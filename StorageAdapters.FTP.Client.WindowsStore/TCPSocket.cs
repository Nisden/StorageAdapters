namespace StorageAdapters.FTP.Client.WindowsStore
{
    using StorageAdapters.FTP.Client.Platform;
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Windows.Networking;
    using Windows.Networking.Sockets;

    public sealed class TCPSocket : ITCPSocket
    {
        private readonly StreamSocket socket;

        public string Host { get; private set; }

        public TCPSocket()
        {
            socket = new StreamSocket();
        }

        public async Task ConnectAsync(string host, int port)
        {
            try
            {
                await socket.ConnectAsync(new HostName(host), port.ToString());
                this.Host = host;
            }
            catch (Exception ex)
            {
                throw NetworkUtility.HandleException(ex);
            }
        }

        public async Task UpgradeToSSL()
        {
            await socket.UpgradeToSslAsync(SocketProtectionLevel.Tls11, new HostName(Host));
        }

        public Task<Stream> GetStreamAsync()
        {
            return Task.FromResult<Stream>(new TCPStream(socket.InputStream, socket.OutputStream));
        }

        public void Dispose()
        {
            socket.Dispose();
        }
    }
}
