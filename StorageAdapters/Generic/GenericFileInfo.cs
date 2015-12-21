namespace StorageAdapters.Generic
{
    using System;

    public class GenericFileInfo : IVirtualFileInfo
    {
        public string Name { get; set; }

        public string Path { get; set; }
        
        public long Size { get; set; }

        public DateTimeOffset LastModified { get; set; }

        public override string ToString()
        {
            return Path;
        }
    }
}
