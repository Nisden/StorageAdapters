namespace StorageAdapters.IO
{
    using Generic;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class IOStorageService : IStorageService<IOConfiguration>
    {
        public IOConfiguration Configuration { get; set; }

        public Task DeleteDirectoryAsync(string path, CancellationToken cancellationToken)
        {
            return DeleteDirectoryAsync(path, false, cancellationToken);
        }

        public Task DeleteDirectoryAsync(string path, bool recursive, CancellationToken cancellationToken)
        {
            if (Configuration == null)
                throw new InvalidOperationException(Exceptions.ConfigurationMustBeSet);

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    Directory.Delete(GetFullPath(path), recursive);
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw new UnauthorizedException(ex.Message, ex);
                }
                catch (DirectoryNotFoundException ex)
                {
                    throw new NotFoundException(string.Format(Exceptions.DirectoryNotFound, path), ex);
                }
            }, cancellationToken);
        }

        public Task DeleteFileAsync(string path, CancellationToken cancellationToken)
        {
            if (Configuration == null)
                throw new InvalidOperationException(Exceptions.ConfigurationMustBeSet);

            return Task.Factory.StartNew(() =>
            {
                if (!File.Exists(GetFullPath(path)))
                    throw new NotFoundException(string.Format(Exceptions.FileNotFound, path));

                try
                {
                    File.Delete(GetFullPath(path));
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw new UnauthorizedException(ex.Message, ex);
                }
                catch (DirectoryNotFoundException ex)
                {
                    throw new NotFoundException(string.Format(Exceptions.FileNotFound, path), ex);
                }
            }, cancellationToken);
        }

        public async Task<IEnumerable<IVirtualDirectory>> GetDirectoriesAsync(string path, CancellationToken cancellationToken)
        {
            if (Configuration == null)
                throw new InvalidOperationException(Exceptions.ConfigurationMustBeSet);

            // Starts a new thread, where we get all the paths for all the files, and use PLinq to retrive multiple FileInfo at a time.
            // End up with returning an list of GenericFileInfo
            return await Task.Factory.StartNew(() =>
            {
                try
                {
                    return (from directoryPath in Directory.EnumerateDirectories(GetFullPath(path)).AsParallel().WithCancellation(cancellationToken)
                            let directoryInfo = new DirectoryInfo(directoryPath)
                            select new GenericDirectory()
                            {
                                Name = directoryInfo.Name,
                                Path = Path.Combine(path, directoryInfo.Name)
                            }).ToList();
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw new UnauthorizedException(ex.Message, ex);
                }
                catch (DirectoryNotFoundException ex)
                {
                    throw new NotFoundException(string.Format(Exceptions.DirectoryNotFound, path), ex);
                }
            }).ConfigureAwait(false);
        }

        public Task CreateDirectoryAsync(string path, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    Directory.CreateDirectory(GetFullPath(path));
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw new UnauthorizedException(ex.Message, ex);
                }
                catch (DirectoryNotFoundException ex)
                {
                    throw new NotFoundException(string.Format(Exceptions.DirectoryNotFound, path), ex);
                }
            }, cancellationToken);
        }

        public async Task<bool> DirectoryExistAsync(string path, CancellationToken cancellationToken)
        {
            if (Configuration == null)
                throw new InvalidOperationException(Exceptions.ConfigurationMustBeSet);

            try
            {
                return await Task.FromResult(Directory.Exists(GetFullPath(path))).ConfigureAwait(false);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedException(ex.Message, ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new NotFoundException(string.Format(Exceptions.DirectoryNotFound, path), ex);
            }
        }

        public async Task<IVirtualFileInfo> GetFileAsync(string path, CancellationToken cancellationToken)
        {
            if (Configuration == null)
                throw new InvalidOperationException(Exceptions.ConfigurationMustBeSet);

            try
            {
                var fileInfo = new FileInfo(GetFullPath(path));
                if (!fileInfo.Exists)
                    throw new NotFoundException(string.Format(Exceptions.FileNotFound, path));

                return await Task.FromResult<IVirtualFileInfo>(new GenericFileInfo()
                {
                    Name = fileInfo.Name,
                    Path = Path.Combine(path, fileInfo.Name),
                    LastModified = new DateTimeOffset(fileInfo.LastWriteTimeUtc),
                    Size = fileInfo.Length
                }).ConfigureAwait(false);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedException(ex.Message, ex);
            }
        }

        public async Task<IEnumerable<IVirtualFileInfo>> GetFilesAsync(string path, CancellationToken cancellationToken)
        {
            if (Configuration == null)
                throw new InvalidOperationException(Exceptions.ConfigurationMustBeSet);

            // Starts a new thread, where we get all the paths for all the files, and use PLinq to retrive multiple FileInfo at a time.
            // End up with returning an list of GenericFileInfo
            return await Task.Factory.StartNew(() =>
            {
                try
                {
                    return (from filePath in Directory.EnumerateFiles(GetFullPath(path)).AsParallel().WithCancellation(cancellationToken)
                            let fileInfo = new FileInfo(filePath)
                            select new GenericFileInfo()
                            {
                                Name = fileInfo.Name,
                                Path = Path.Combine(path, fileInfo.Name),
                                Size = fileInfo.Length,
                                LastModified = new DateTimeOffset(fileInfo.LastWriteTimeUtc)
                            }).ToList();
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw new UnauthorizedException(ex.Message, ex);
                }
                catch (DirectoryNotFoundException ex)
                {
                    throw new NotFoundException(string.Format(Exceptions.DirectoryNotFound, path), ex);
                }
            }).ConfigureAwait(false);
        }

        public async Task<Stream> ReadFileAsync(string path, CancellationToken cancellationToken)
        {
            if (Configuration == null)
                throw new InvalidOperationException(Exceptions.ConfigurationMustBeSet);

            try
            {
                return await Task.FromResult<Stream>(File.OpenRead(GetFullPath(path)));
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedException(ex.Message, ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new NotFoundException(string.Format(Exceptions.FileNotFound, path), ex);
            }
        }

        public async Task SaveFileAsync(string path, Stream stream, CancellationToken cancellationToken)
        {
            if (Configuration == null)
                throw new InvalidOperationException(Exceptions.ConfigurationMustBeSet);

            try
            {
                using (var fileStream = File.OpenWrite(GetFullPath(path)))
                {
                    await stream.CopyToAsync(fileStream).ConfigureAwait(false);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedException(ex.Message, ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new NotFoundException(string.Format(Exceptions.FileNotFound, path), ex);
            }
        }

        public async Task AppendFileAsync(string path, byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (Configuration == null)
                throw new InvalidOperationException(Exceptions.ConfigurationMustBeSet);

            try
            {
                using (var fileStream = File.Open(GetFullPath(path), FileMode.Append, FileAccess.Write, FileShare.Read))
                {
                    await fileStream.WriteAsync(buffer, offset, count);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedException(ex.Message, ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new NotFoundException(string.Format(Exceptions.FileNotFound, path), ex);
            }
        }

        public Task<bool> FileExistAsync(string path, CancellationToken cancellationToken)
        {
            if (Configuration == null)
                throw new InvalidOperationException(Exceptions.ConfigurationMustBeSet);

            try
            {
                return Task.FromResult(File.Exists(GetFullPath(path)));
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedException(ex.Message, ex);
            }
        }

        internal string GetFullPath(string path)
        {
            if (Configuration == null)
                throw new InvalidOperationException(Exceptions.ConfigurationMustBeSet);

            // Escape the base directory, we do this individualy if we need to check on the escape later
            var baseDirectory = Configuration.ExpandEnvironmentVariables && !Configuration.AllowBaseDirectoryEscape ? Environment.ExpandEnvironmentVariables(Configuration.BaseDirectory) : Configuration.BaseDirectory;

            var result = Path.GetFullPath(Path.Combine(baseDirectory, path));
            if (Configuration.ExpandEnvironmentVariables) // Expand variables again, this time including the user defined path.
                result = Environment.ExpandEnvironmentVariables(result);

            // Ensure that we havent escaped the BaseDirectory
            if (!Configuration.AllowBaseDirectoryEscape)
            {
                if (!result.StartsWith(baseDirectory, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidPathException();
            }

            return result;
        }

        public void Dispose()
        {
        }
    }
}
