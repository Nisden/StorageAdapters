namespace StorageAdapters
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IStorageService<TConfiguration> : IDisposable
        where TConfiguration : IStorageConfiguration
    {
        TConfiguration Configuration { get; set; }

        Task<IVirtualFileInfo> GetFileAsync(string path, CancellationToken cancellationToken);

        Task<bool> FileExistAsync(string path, CancellationToken cancellationToken);

        Task<IEnumerable<IVirtualFileInfo>> GetFilesAsync(string path, CancellationToken cancellationToken);

        Task<Stream> ReadFileAsync(string path, CancellationToken cancellationToken);

        Task SaveFileAsync(string path, Stream stream, CancellationToken cancellationToken);

        Task DeleteFileAsync(string path, CancellationToken cancellationToken);

        Task CreateDirectoryAsync(string path, CancellationToken cancellationToken);

        Task<bool> DirectoryExistAsync(string path, CancellationToken cancellationToken);

        Task<IEnumerable<IVirtualDirectory>> GetDirectoriesAsync(string path, CancellationToken cancellationToken);

        Task DeleteDirectoryAsync(string path, CancellationToken cancellationToken);
    }
}
