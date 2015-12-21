namespace StorageAdapters
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class StorageAdapterBase<TService, TConfiguration> : IStorageAdapter
        where TService : IStorageService<TConfiguration>, new()
        where TConfiguration : IStorageConfiguration
    {
        private readonly TService service;
        public TService Service
        {
            get
            {
                return service;
            }
        }

        public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromMinutes(1);

        public StorageAdapterBase(TConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            service = new TService();
            service.Configuration = configuration;
        }

        public Task<IVirtualFileInfo> GetFileAsync(string path)
        {
            using (var tokenSource = CreateDefaultTimeoutToken())
            {
                return this.GetFileAsync(PathClean(path, true), tokenSource.Token);
            }
        }

        public virtual Task<IVirtualFileInfo> GetFileAsync(string path, CancellationToken cancellationToken)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return service.GetFileAsync(path, cancellationToken);
        }

        public async Task<IEnumerable<IVirtualFileInfo>> GetFilesRecursiveAsync(string path)
        {
            using (var tokenSource = CreateDefaultTimeoutToken())
            {
                return await this.GetFilesRecursiveAsync(PathClean(path, false), tokenSource.Token).ConfigureAwait(false);
            }
        }

        public virtual async Task<IEnumerable<IVirtualFileInfo>> GetFilesRecursiveAsync(string path, CancellationToken cancellationToken)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            List<IVirtualFileInfo> result = new List<IVirtualFileInfo>();

            var subDirectories = await GetDirectoriesRecursiveAsync(PathClean(path, false), cancellationToken);

            // Get Files
            result.AddRange(await GetFilesAsync(PathClean(path, false), cancellationToken));
            foreach (var directory in subDirectories)
            {
                cancellationToken.ThrowIfCancellationRequested();
                result.AddRange(await GetFilesAsync(directory.Path, cancellationToken));
            }

            return result;
        }

        public async Task<bool> FileExistAsync(string path)
        {
            using (var tokenSource = CreateDefaultTimeoutToken())
            {
                return await this.FileExistAsync(path, tokenSource.Token).ConfigureAwait(false);
            }
        }

        public virtual Task<bool> FileExistAsync(string path, CancellationToken cancellationToken)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return service.FileExistAsync(PathClean(path, true), cancellationToken);
        }

        public async Task<IEnumerable<IVirtualFileInfo>> GetFilesAsync(string path)
        {
            using (var tokenSource = CreateDefaultTimeoutToken())
            {
                return await this.GetFilesAsync(path, tokenSource.Token).ConfigureAwait(false);
            }
        }

        public virtual Task<IEnumerable<IVirtualFileInfo>> GetFilesAsync(string path, CancellationToken cancellationToken)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return service.GetFilesAsync(PathClean(path, false), cancellationToken);
        }

        public Task<Stream> ReadFileAsync(string path)
        {
            using (var tokenSource = CreateDefaultTimeoutToken())
            {
                return this.ReadFileAsync(path, tokenSource.Token);
            }
        }

        public virtual Task<Stream> ReadFileAsync(string path, CancellationToken cancellationToken)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return service.ReadFileAsync(PathClean(path, true), cancellationToken);
        }

        public Task SaveFileAsync(string path, Stream stream)
        {
            using (var tokenSource = CreateDefaultTimeoutToken())
            {
                return this.SaveFileAsync(path, stream, tokenSource.Token);
            }
        }

        public virtual Task SaveFileAsync(string path, Stream stream, CancellationToken cancellationToken)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            return service.SaveFileAsync(PathClean(path, true), stream, cancellationToken);
        }

        public async Task DeleteFileAsync(string path)
        {
            using (var tokenSource = CreateDefaultTimeoutToken())
            {
                await this.DeleteFileAsync(path, tokenSource.Token).ConfigureAwait(false);
            }
        }

        public virtual Task DeleteFileAsync(string path, CancellationToken cancellationToken)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return service.DeleteFileAsync(path, cancellationToken);
        }

        public Task<bool> DirectoryExistAsync(string path)
        {
            using (var tokenSource = CreateDefaultTimeoutToken())
            {
                return this.DirectoryExistAsync(path, tokenSource.Token);
            }
        }

        public virtual Task<bool> DirectoryExistAsync(string path, CancellationToken cancellationToken)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return service.DirectoryExistAsync(PathClean(path, false), cancellationToken);
        }

        public async Task<IEnumerable<IVirtualDirectory>> GetDirectoriesAsync(string path)
        {
            using (var tokenSource = CreateDefaultTimeoutToken())
            {
                return await this.GetDirectoriesAsync(path, tokenSource.Token).ConfigureAwait(false);
            }
        }

        public Task<IEnumerable<IVirtualDirectory>> GetDirectoriesAsync(string path, CancellationToken cancellationToken)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return service.GetDirectoriesAsync(PathClean(path, false), cancellationToken);
        }

        public async virtual Task<IEnumerable<IVirtualDirectory>> GetDirectoriesRecursiveAsync(string path)
        {
            using (var tokenSource = CreateDefaultTimeoutToken())
            {
                return await this.GetDirectoriesRecursiveAsync(path, tokenSource.Token).ConfigureAwait(false);
            }
        }

        public virtual async Task<IEnumerable<IVirtualDirectory>> GetDirectoriesRecursiveAsync(string path, CancellationToken cancellationToken)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            List<IVirtualDirectory> result = new List<IVirtualDirectory>();

            foreach (var directory in await service.GetDirectoriesAsync(PathClean(path, false), cancellationToken).ConfigureAwait(false))
            {
                cancellationToken.ThrowIfCancellationRequested();
                result.AddRange(await service.GetDirectoriesAsync(directory.Path, cancellationToken));
            }

            return result;
        }

        public async Task DeleteDirectoryAsync(string path)
        {
            using (var tokenSource = CreateDefaultTimeoutToken())
            {
                await this.DeleteDirectoryAsync(path, true, tokenSource.Token).ConfigureAwait(false);
            }
        }

        public virtual async Task DeleteDirectoryAsync(string path, bool recursive, CancellationToken cancellationToken)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (recursive)
            {
                var subDirectories = await GetDirectoriesRecursiveAsync(PathClean(path, false), cancellationToken);
                var files = await GetFilesRecursiveAsync(PathClean(path, false), cancellationToken);

                // Delete all files
                Task.WaitAll(files.Select(file => DeleteFileAsync(file.Path, cancellationToken)).ToArray());

                // Delete all directories
                Task.WaitAll(subDirectories.Select(directory => service.DeleteDirectoryAsync(directory.Path, cancellationToken)).ToArray());
            }

            // None recursive delete / Or delete after deleting all content
            await service.DeleteDirectoryAsync(path, cancellationToken).ConfigureAwait(false);
        }

        public async Task CreateDirectoryAsync(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            using (var tokenSource = CreateDefaultTimeoutToken())
            {
                await this.CreateDirectoryAsync(path, true, tokenSource.Token).ConfigureAwait(false);
            }
        }

        public virtual async Task CreateDirectoryAsync(string path, bool recursive, CancellationToken cancellationToken)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (recursive && PathClean(path, false).IndexOf(Service.Configuration.DirectorySeperator) > -1)
            {
                if (await service.DirectoryExistAsync(PathClean(Path.GetDirectoryName(path), false), cancellationToken).ConfigureAwait(false))
                {
                    // If the parent directory already exists create the new directory
                    await service.CreateDirectoryAsync(path, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    // Split path in parts
                    string[] pathParts = path.Split(Service.Configuration.DirectorySeperator);
                    for (int i = 0; i < pathParts.Length; i++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        // Create each missing part, one by one.
                        string currentPath = string.Join(Service.Configuration.DirectorySeperator.ToString(), pathParts.Take(i + 1));
                        if (!await service.DirectoryExistAsync(currentPath, cancellationToken).ConfigureAwait(false))
                        {
                            await service.CreateDirectoryAsync(currentPath, cancellationToken).ConfigureAwait(false);
                        }
                    }
                }
            }
            else
            {
                // Either the path is in the root (No parents to create) or we have disabled recursive directory creation
                await service.CreateDirectoryAsync(path, cancellationToken).ConfigureAwait(false);
            }
        }

        protected virtual string PathClean(string path, bool file)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return PathUtility.Clean(Service.Configuration.DirectorySeperator, path);
        }

        public virtual string PathCombine(params string[] paths)
        {
            if (paths == null)
                throw new ArgumentNullException(nameof(paths));

            return PathUtility.Combine(Service.Configuration.DirectorySeperator, paths);
        }

        protected CancellationTokenSource CreateDefaultTimeoutToken()
        {
            return new CancellationTokenSource(DefaultTimeout);
        }

        public void Dispose()
        {
            Service.Dispose();
        }
    }
}
