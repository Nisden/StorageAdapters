namespace StorageAdapters.FTP.Client.Platform
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public interface ITCPSocket : IDisposable
    {
        Task ConnectAsync(string host, int port);

        Task UpgradeToSSL();

        Task<Stream> GetStreamAsync();
    }
}