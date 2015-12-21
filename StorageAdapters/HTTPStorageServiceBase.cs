namespace StorageAdapters
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class HTTPStorageServiceBase<TConfiguration> : IStorageService<TConfiguration>
        where TConfiguration : HTTPStorageConfiguration
    {
        private TConfiguration configuration;
        public TConfiguration Configuration
        {
            get
            {
                return configuration;
            }
            set
            {
                // Unsub from existing
                if (configuration != null)
                    configuration.ConfigurationChanged -= Configuration_ConfigurationChanged;

                // Subscribe for when the configuration changes
                configuration = value;
                configuration.ConfigurationChanged += Configuration_ConfigurationChanged;

                // The configuration object has totally changed
                Configuration_ConfigurationChanged(null, null);
            }
        }

        private HttpClient httpClient;
        public HttpClient HttpClient
        {
            get
            {
                return httpClient;
            }
        }

        public abstract Task CreateDirectoryAsync(string path, CancellationToken cancellationToken);
        public abstract Task DeleteDirectoryAsync(string path, CancellationToken cancellationToken);
        public abstract Task DeleteFileAsync(string path, CancellationToken cancellationToken);
        public abstract Task<bool> DirectoryExistAsync(string path, CancellationToken cancellationToken);
        public abstract Task<bool> FileExistAsync(string path, CancellationToken cancellationToken);
        public abstract Task<IEnumerable<IVirtualDirectory>> GetDirectoriesAsync(string path, CancellationToken cancellationToken);
        public abstract Task<IVirtualFileInfo> GetFileAsync(string path, CancellationToken cancellationToken);
        public abstract Task<IEnumerable<IVirtualFileInfo>> GetFilesAsync(string path, CancellationToken cancellationToken);
        public abstract Task<Stream> ReadFileAsync(string path, CancellationToken cancellationToken);
        public abstract Task SaveFileAsync(string path, Stream stream, CancellationToken cancellationToken);

        protected virtual void ConfigureHttpClientHandler(HttpClientHandler clientHandler)
        { }

        protected virtual void ConfigureHttpClient(HttpClient client)
        { }

        protected string EncodePath(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var normalizedPath = path.Replace('\\', Configuration.DirectorySeperator).Replace('/', Configuration.DirectorySeperator);
            return string.Join(Configuration.DirectorySeperator.ToString(), normalizedPath.Split(Configuration.DirectorySeperator).Select(x => Uri.EscapeUriString(x)));
        }

        protected string CombineUrl(params string[] urls)
        {
            return string.Join(Configuration.DirectorySeperator.ToString(), urls.Select(x => x.TrimStart(Configuration.DirectorySeperator)));
        }

        private void Configuration_ConfigurationChanged(object sender, EventArgs e)
        {
            if (Configuration != null)
            {
                // Dispose the existing HTTP client, if there is one.
                if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }

                HttpClientHandler clientHandler = new HttpClientHandler();
                ConfigureHttpClientHandler(clientHandler);

                // Create new HTTP client
                httpClient = new HttpClient(clientHandler);
                ConfigureHttpClient(httpClient);
            }
        }

        public void Dispose()
        {
            ((IDisposable)HttpClient).Dispose();
        }
    }
}
