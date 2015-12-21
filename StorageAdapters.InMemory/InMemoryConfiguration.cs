namespace StorageAdapters.InMemory
{
    public class InMemoryConfiguration : IStorageConfiguration
    {
        public char DirectorySeperator
        {
            get
            {
                return '/';
            }
        }
    }
}
