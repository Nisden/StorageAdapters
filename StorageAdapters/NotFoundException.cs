namespace StorageAdapters
{
    using System;

    /// <summary>
    /// Throwen when a file or directory can not be found.
    /// </summary>
    public class NotFoundException : StorageAdapterException
    {
        public NotFoundException() { }
        public NotFoundException(string message) : base(message) { }
        public NotFoundException(string message, Exception inner) : base(message, inner) { }
    }
}
