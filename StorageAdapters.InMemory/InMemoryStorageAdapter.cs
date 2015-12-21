namespace StorageAdapters.InMemory
{
    public class InMemoryStorageAdapter : StorageAdapterBase<InMemoryStorageService, InMemoryConfiguration>
    {
        public InMemoryStorageAdapter(InMemoryConfiguration configuration) : base(configuration)
        { }
    }
}
