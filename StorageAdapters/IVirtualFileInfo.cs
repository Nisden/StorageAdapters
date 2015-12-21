namespace StorageAdapters
{
    using System;

    public interface IVirtualFileInfo : IVirtualEntry
    {
        long Size { get; set; }

        DateTimeOffset LastModified { get; set; }
    }
}
