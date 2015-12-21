namespace StorageAdapters.InMemory
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class InMemoryStorageService : IStorageService<InMemoryConfiguration>
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, InMemoryFile>> storage = new ConcurrentDictionary<string, ConcurrentDictionary<string, InMemoryFile>>();

        public InMemoryConfiguration Configuration { get; set; }

        public Task CreateDirectoryAsync(string path, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                storage.AddOrUpdate(path, new ConcurrentDictionary<string, InMemoryFile>(), (key, oldValue) =>
                {
                    return oldValue;
                });
            });
        }

        public Task DeleteDirectoryAsync(string path, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                ConcurrentDictionary<string, InMemoryFile> deletedDirectory;
                if (!storage.TryRemove(path, out deletedDirectory))
                    throw new NotFoundException(string.Format(Exceptions.DirectoryNotFound, path));
            });
        }

        public Task DeleteFileAsync(string path, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                ConcurrentDictionary<string, InMemoryFile> currentDirectory;
                if (!storage.TryGetValue(PathUtility.GetDirectoryName(Configuration.DirectorySeperator, path), out currentDirectory))
                    throw new NotFoundException(string.Format(Exceptions.DirectoryNotFound, path));

                InMemoryFile currentFile;
                if (!currentDirectory.TryRemove(PathUtility.GetFileName(Configuration.DirectorySeperator, path), out currentFile))
                    throw new NotFoundException(string.Format(Exceptions.FileNotFound, path));
            });
        }

        public Task<bool> DirectoryExistAsync(string path, CancellationToken cancellationToken)
        {
            return Task.FromResult(storage.ContainsKey(path));
        }

        public Task<bool> FileExistAsync(string path, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                ConcurrentDictionary<string, InMemoryFile> currentDirectory;
                if (!storage.TryGetValue(PathUtility.GetDirectoryName(Configuration.DirectorySeperator, path), out currentDirectory))
                    throw new NotFoundException(string.Format(Exceptions.DirectoryNotFound, path));

                return currentDirectory.ContainsKey(PathUtility.GetFileName(Configuration.DirectorySeperator, path));
            });
        }

        public async Task<IEnumerable<IVirtualDirectory>> GetDirectoriesAsync(string path, CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(() =>
            {
                // Ensure that parent directory exists
                ConcurrentDictionary<string, InMemoryFile> currentDirectory;
                if (!storage.TryGetValue(path, out currentDirectory))
                    throw new NotFoundException(string.Format(Exceptions.DirectoryNotFound, path));

                return storage.Keys.Where(x => 
                x.StartsWith(path) && // Key should start with the path
                x.Length > path.Length && // And be longer then the current path
                x.IndexOf(Configuration.DirectorySeperator, path.Length + 1) == -1) // But not contain anymore Seperators
                .Select(x => new Generic.GenericDirectory()
                {
                    Name = PathUtility.GetDirectoryName(Configuration.DirectorySeperator, x),
                    Path = x
                });
                
            }).ConfigureAwait(false);
        }

        public async Task<IVirtualFileInfo> GetFileAsync(string path, CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(() =>
            {
                ConcurrentDictionary<string, InMemoryFile> currentDirectory;
                if (!storage.TryGetValue(PathUtility.GetDirectoryName(Configuration.DirectorySeperator, path), out currentDirectory))
                    throw new NotFoundException(string.Format(Exceptions.DirectoryNotFound, path));

                InMemoryFile currentFile;
                if (!currentDirectory.TryGetValue(PathUtility.GetFileName(Configuration.DirectorySeperator, path), out currentFile))
                    throw new NotFoundException(string.Format(Exceptions.FileNotFound, path));

                return currentFile;
            }).ConfigureAwait(false);
        }

        public async Task<IEnumerable<IVirtualFileInfo>> GetFilesAsync(string path, CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(() =>
            {
                ConcurrentDictionary<string, InMemoryFile> currentDirectory;
                if (!storage.TryGetValue(path, out currentDirectory))
                    throw new NotFoundException(string.Format(Exceptions.DirectoryNotFound, path));

                return currentDirectory.Values.ToArray();
            }).ConfigureAwait(false);
        }

        public async Task<Stream> ReadFileAsync(string path, CancellationToken cancellationToken)
        {
            return new MemoryStream(((InMemoryFile)(await GetFileAsync(path, cancellationToken).ConfigureAwait(false))).Data);
        }

        public async Task SaveFileAsync(string path, Stream stream, CancellationToken cancellationToken)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms).ConfigureAwait(false);

                ConcurrentDictionary<string, InMemoryFile> currentDirectory;
                if (!storage.TryGetValue(PathUtility.GetDirectoryName(Configuration.DirectorySeperator, path), out currentDirectory))
                    throw new NotFoundException(string.Format(Exceptions.DirectoryNotFound, path));

                string fileName = PathUtility.GetFileName(Configuration.DirectorySeperator, path);
                if (!currentDirectory.TryAdd(fileName, new InMemoryFile()
                {
                    Name = fileName,
                    Path = path,
                    LastModified = DateTimeOffset.Now,
                    Size = ms.Length,
                    Data = ms.ToArray()
                }))
                {
                    throw new InvalidOperationException("Unable to save the file, might have tried to save more then once to quickly");
                }
            }
        }

        public void Dispose()
        {
        }

        private class InMemoryFile : Generic.GenericFileInfo
        {
            public byte[] Data { get; set; }
        }
    }
}
