namespace StorageAdapters.Generic
{
    using System;

    public class GenericFileInfo : IVirtualFileInfo
    {
        public virtual string Name { get; set; }

        public virtual string Path { get; set; }
        
        public virtual long Size { get; set; }

        public virtual DateTimeOffset LastModified { get; set; }

        public override string ToString()
        {
            return Path;
        }
    }
}
