namespace StorageAdapters.FTP.Client.WindowsStore
{
    using StorageAdapters.FTP.Client.Platform;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Windows.Networking.Sockets;

    public static class NetworkUtility
    {
        public static Exception HandleException(Exception ex)
        {
            SocketErrorStatus socketError = SocketError.GetStatus(ex.HResult);
            if (socketError != SocketErrorStatus.Unknown)
            {
                return new NetworkException("An network error occured", ex);
            }
            else
            {
                throw new Exception("An unhandled exception happend in the ftp client", ex);
            }
        }
    }
}
