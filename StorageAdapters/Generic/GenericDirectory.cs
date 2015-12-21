namespace StorageAdapters.Generic
{
    public class GenericDirectory : IVirtualDirectory
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public override string ToString()
        {
            return Path;
        }
    }
}
