namespace StorageAdapters.FTP.Client.WindowsStore
{
    using StorageAdapters.FTP.Client.Platform;
    using System;

    public class PlatformFactory : IPlatformFactory
    {
        public ITCPSocket CreateSocket()
        {
            return new TCPSocket();
        }
    }
}
