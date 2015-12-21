namespace StorageAdapters.FTP.Client.Platform
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SocketException : Exception
    {
        public SocketException() { }
        public SocketException(string message) : base(message) { }
        public SocketException(string message, Exception inner) : base(message, inner) { }
    }
}
