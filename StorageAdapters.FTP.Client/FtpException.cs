namespace StorageAdapters.FTP.Client
{
    using System;

    public class FTPException : Exception
    {
        public FTPException() { }
        public FTPException(string message) : base(message) { }
        public FTPException(string message, Exception inner) : base(message, inner) { }
    }
}
