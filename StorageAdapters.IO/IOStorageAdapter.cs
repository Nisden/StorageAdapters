namespace StorageAdapters.IO
{
    using System.Threading;
    using System.Threading.Tasks;

    public class IOStorageAdapter : StorageAdapterBase<IOStorageService, IOConfiguration>
    {
        public IOStorageAdapter(IOConfiguration configuration) : base(configuration)
        {
        }

        public override Task DeleteDirectoryAsync(string path, bool recursive, CancellationToken cancellationToken)
        {
            // The IO Storage Service has a custom funtion for recursive directory deletes
            return Service.DeleteDirectoryAsync(path, recursive, cancellationToken);
        }
    }
}
