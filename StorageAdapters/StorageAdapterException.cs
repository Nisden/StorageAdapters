namespace StorageAdapters
{
    using System;

    /// <summary>
    /// The base exception for all custom exceptions that the storage adapters may throw.
    /// </summary>
    public class StorageAdapterException : Exception
    {
        public StorageAdapterException() { }
        public StorageAdapterException(string message) : base(message) { }
        public StorageAdapterException(string message, Exception inner) : base(message, inner) { }
    }
}
