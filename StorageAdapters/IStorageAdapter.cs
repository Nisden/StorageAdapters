namespace StorageAdapters
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IStorageAdapter : IDisposable
    {
        /// <summary>
        /// Gets the information about a file from the storage adapter, with the default timeout.
        /// </summary>
        /// <param name="path">Relative path for the file</param>
        /// <exception cref="ArgumentNullException">If the path is null</exception>
        Task<IVirtualFileInfo> GetFileAsync(string path);

        /// <summary>
        /// Gets the information about a file from the storage adapter with a cancellationToken to abort the retrival.
        /// </summary>
        /// <param name="path">Relative path for the file.</param>
        /// <param name="cancellationToken">CancellationToken used to abort the operation.</param>
        /// <exception cref="ArgumentNullException">If the path is null</exception>
        Task<IVirtualFileInfo> GetFileAsync(string path, CancellationToken cancellationToken);

        Task<bool> FileExistAsync(string path);
        Task<bool> FileExistAsync(string path, CancellationToken cancellationToken);

        Task<IEnumerable<IVirtualFileInfo>> GetFilesAsync(string path);
        Task<IEnumerable<IVirtualFileInfo>> GetFilesAsync(string path, CancellationToken cancellationToken);

        Task<IEnumerable<IVirtualFileInfo>> GetFilesRecursiveAsync(string path);
        Task<IEnumerable<IVirtualFileInfo>> GetFilesRecursiveAsync(string path, CancellationToken cancellationToken);

        Task<Stream> ReadFileAsync(string path);
        Task<Stream> ReadFileAsync(string path, CancellationToken cancellationToken);

        Task SaveFileAsync(string path, Stream stream);
        Task SaveFileAsync(string path, Stream stream, CancellationToken cancellationToken);

        Task DeleteFileAsync(string path);
        Task DeleteFileAsync(string path, CancellationToken cancellationToken);

        Task CreateDirectoryAsync(string path);
        Task CreateDirectoryAsync(string path, bool recursive, CancellationToken cancellationToken);

        Task<bool> DirectoryExistAsync(string path);
        Task<bool> DirectoryExistAsync(string path, CancellationToken cancellationToken);

        Task<IEnumerable<IVirtualDirectory>> GetDirectoriesAsync(string path);
        Task<IEnumerable<IVirtualDirectory>> GetDirectoriesAsync(string path, CancellationToken cancellationToken);

        Task<IEnumerable<IVirtualDirectory>> GetDirectoriesRecursiveAsync(string path);
        Task<IEnumerable<IVirtualDirectory>> GetDirectoriesRecursiveAsync(string path, CancellationToken cancellationToken);

        Task DeleteDirectoryAsync(string path);
        Task DeleteDirectoryAsync(string path, bool recursive, CancellationToken cancellationToken);

        string PathCombine(params string[] paths);
    }
}
