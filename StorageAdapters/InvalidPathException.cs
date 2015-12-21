namespace StorageAdapters
{
    using System;

    /// <summary>
    /// Throwen when the path is invalid or incorrect.
    /// </summary>
    public class InvalidPathException : StorageAdapterException
    {
        public InvalidPathException() { }
        public InvalidPathException(string message) : base(message) { }
        public InvalidPathException(string message, Exception inner) : base(message, inner) { }
    }
}
