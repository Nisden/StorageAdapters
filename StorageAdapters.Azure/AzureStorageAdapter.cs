namespace StorageAdapters.Azure
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class AzureStorageAdapter : StorageAdapterBase<AzureStorageService, AzureConfiguration>
    {
        public AzureStorageAdapter(AzureConfiguration configuration) : base(configuration)
        { }

        public override async Task DeleteDirectoryAsync(string path, bool recursive, CancellationToken cancellationToken)
        {
            if (recursive)
            {
                await Service.DeleteDirectoryAsync(path, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                // TODO: We need to manualy check if the "thing" contains anything as azure will always delete everything in a container.
                throw new NotImplementedException();
            }
        }
    }
}
