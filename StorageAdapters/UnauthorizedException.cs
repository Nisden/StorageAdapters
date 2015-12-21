namespace StorageAdapters
{
    using System;

    /// <summary>
    /// Thrown when the storage adapter is denied access, can be invalid credentials.
    /// </summary>
    public class UnauthorizedException : StorageAdapterException
    {
        public UnauthorizedException() { }
        public UnauthorizedException(string message) : base(message) { }
        public UnauthorizedException(string message, Exception inner) : base(message, inner) { }
    }
}
