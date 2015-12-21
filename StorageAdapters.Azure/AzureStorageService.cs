namespace StorageAdapters.Azure
{
    using Platform;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class AzureStorageService : HTTPStorageServiceBase<AzureConfiguration>
    {
        private static string[] PlatformFactoryTypes = {
            "StorageAdapters.Azure.WindowsStore.PlatformFactory, StorageAdapters.Azure.WindowsStore",
            "StorageAdapters.Azure.Desktop.PlatformFactory, StorageAdapters.Azure.Desktop"
        };

        private readonly IPlatformFactory platformFactory;
        public IPlatformFactory PlatformFactory
        {
            get
            {
                return platformFactory;
            }
        }

        private readonly ICryptographic cryptographic;

        public AzureStorageService() : base()
        {
            foreach (var platformFactoryType in PlatformFactoryTypes)
            {
                Type factoryType = Type.GetType(platformFactoryType);
                if (factoryType != null)
                {
                    platformFactory = (IPlatformFactory)Activator.CreateInstance(factoryType);
                }
            }

            if (platformFactory == null)
            {
                throw new PlatformNotSupportedException("No platform factory found, the following factories are supported:" + Environment.NewLine + string.Join(Environment.NewLine, PlatformFactoryTypes));
            }

            cryptographic = platformFactory.CreateCryptograhicModule();
        }

        #region HTTPStorageServiceBase

        public override async Task CreateDirectoryAsync(string path, CancellationToken cancellationToken)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            string[] pathParts = path.Split(Configuration.DirectorySeperator);

            // Create container if it does not exist
            if (!await ContainerExistAsync(pathParts[0], cancellationToken).ConfigureAwait(false))
            {
                await CreateContainerAsync(pathParts[0], cancellationToken).ConfigureAwait(false);
            }
        }

        public override async Task DeleteDirectoryAsync(string path, CancellationToken cancellationToken)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            string[] pathParts = path.Split(Configuration.DirectorySeperator);

            if (pathParts.Length == 1)
            {
                await DeleteContainerAsync(pathParts[0], cancellationToken).ConfigureAwait(false);
            }
            else
            {
                // Delete all blobls with matching prefix
                throw new NotImplementedException();
            }
        }

        public override Task DeleteFileAsync(string path, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> DirectoryExistAsync(string path, CancellationToken cancellationToken)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            string[] pathParts = path.Split(Configuration.DirectorySeperator);

            // The root always exists
            if (pathParts.Length == 0)
            {
                return true;
            }
            else
            {
                // Only check if the container exists, all other directories always exists
                try
                {
                    await GetContainerProperties(pathParts[0], cancellationToken);
                    return true;
                }
                catch (NotFoundException)
                {
                    return false;
                }
            }
            
        }

        public override Task<bool> FileExistAsync(string path, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override async Task<IEnumerable<IVirtualDirectory>> GetDirectoriesAsync(string path, CancellationToken cancellationToken)
        {
            if (PathUtility.IsRootPath(path))
            {
                return (await GetContainersAsync(cancellationToken).ConfigureAwait(false)).Select(containerName => new AzureDirectory()
                {
                    Name = containerName,
                    Path = containerName
                });
            }
            else
            {
                var pathParts = path.Split(Configuration.DirectorySeperator);

                // TODO: How the hell will i solve this one? The delimiter and prefix stuff stops from going deeper.
                throw new NotImplementedException();
            }
        }

        public override Task<IVirtualFileInfo> GetFileAsync(string path, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override async Task<IEnumerable<IVirtualFileInfo>> GetFilesAsync(string path, CancellationToken cancellationToken)
        {
            List<AzureFileInfo> files = new List<AzureFileInfo>();
            string nextMarker = string.Empty;
            do
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"?comp=list&marker={Uri.EscapeDataString(nextMarker)}&prefix={Uri.EscapeDataString(path)}&delimiter={Configuration.DirectorySeperator}");
                var response = await SendRequest(request, cancellationToken);
                var responseDocument = System.Xml.Linq.XDocument.Parse(await response.Content.ReadAsStringAsync());

                files.AddRange(from file in responseDocument.Element("Blobs").Elements("Blob")
                               select new AzureFileInfo()
                               {
                                   Path = file.Element("Name").Value,
                                   Name = PathUtility.GetFileName(Configuration.DirectorySeperator, file.Element("Name").Value),
                                   Size = long.Parse(file.Element("Content-Length").Value),
                                   LastModified = DateTime.ParseExact(file.Element("Last-Modified").Value, "R", System.Globalization.CultureInfo.InvariantCulture)
                               });
                
                // Get the next marker from the current response
                nextMarker = responseDocument.Element("NextMarker").Value;
            }
            while (!string.IsNullOrEmpty(nextMarker));

            return files;
        }

        public override Task<Stream> ReadFileAsync(string path, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task SaveFileAsync(string path, Stream stream, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region AzureStorageService

        public async Task CreateContainerAsync(string containerName, CancellationToken cancellationToken)
        {
            await CreateContainerAsync(containerName, Configuration.DefaultContainerAccess, cancellationToken).ConfigureAwait(false);
        }

        public async Task CreateContainerAsync(string containerName, AzureBlobPublicBlobAccess access, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, EncodePath(containerName) + "?restype=container");
            if (access != AzureBlobPublicBlobAccess.@private)
            {
                request.Headers.Add("x-ms-blob-public-access", access.ToString());
            }

            await SendRequest(request, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> ContainerExistAsync(string containerName, CancellationToken cancellationToken)
        {
            try
            {
                await GetContainerProperties(containerName, cancellationToken);
                return true;
            }
            catch (NotFoundException)
            {
                return false;
            }
        }

        public async Task<Dictionary<string, string>> GetContainerProperties(string containerName, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Head, EncodePath(containerName) + "?restype=container");
            var response = await SendRequest(request, cancellationToken);

            return response.Headers.Where(x => x.Key.StartsWith("x-ms-")).ToDictionary(x => x.Key, x => string.Join(", ", x.Value));
        }

        public async Task<List<string>> GetContainersAsync(CancellationToken cancellationToken)
        {
            List<string> containers = new List<string>();
            string nextMarker = string.Empty;
            do
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "?comp=list&marker=" + Uri.EscapeDataString(nextMarker));
                var response = await SendRequest(request, cancellationToken);
                var responseDocument = System.Xml.Linq.XDocument.Parse(await response.Content.ReadAsStringAsync());

                containers.AddRange(responseDocument.Elements("Container").Select(x => x.Element("Name").Value));

                // Get the next marker from the current response
                nextMarker = responseDocument.Element("NextMarker").Value;
            }
            while (!string.IsNullOrEmpty(nextMarker));

            return containers;
        } 

        public async Task DeleteContainerAsync(string containerName, CancellationToken cancellationToken)
        {
            await SendRequest(new HttpRequestMessage(HttpMethod.Delete, EncodePath(containerName) + "?restype=container"), cancellationToken).ConfigureAwait(false);
        }

        #endregion

        protected override void ConfigureHttpClient(HttpClient client)
        {
            if (string.IsNullOrWhiteSpace(Configuration.AzureVersion))
                throw new StorageAdapterException("Invalid configuration, AzureVersion must be set");

            // Set general headers
            client.DefaultRequestHeaders.Add("x-ms-version", Configuration.AzureVersion);

            if (!string.IsNullOrWhiteSpace(Configuration.ClientRequestId))
                client.DefaultRequestHeaders.Add("x-ms-client-request-id", Configuration.ClientRequestId);

            base.ConfigureHttpClient(client);
        }

        private async Task<HttpResponseMessage> SendRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Create correct URI
            if (!request.RequestUri.IsAbsoluteUri)
            {
                UriBuilder uriBuilder = new UriBuilder(CombineUrl(string.Format(Configuration.APIAddress, Configuration.AccountName), request.RequestUri.OriginalString));
                uriBuilder.Scheme = Configuration.UseHTTPS ? "https" : "http";
                request.RequestUri = uriBuilder.Uri;
            }

            // Date header is always required.
            request.Headers.Add("x-ms-date", DateTime.UtcNow.ToString("r"));

            // Create that darm Azure signature!
            string stringToSign = request.Method.Method.ToUpper() + "\n" +
               GetHeaderValue("Content-Encoding", request) + "\n" +
               GetHeaderValue("Content-Language", request) + "\n" +
               GetHeaderValue("Content-Length", request) + "\n" +
               GetHeaderValue("Content-MD5", request) + "\n" +
               GetHeaderValue("Content-Type", request) + "\n" +
               GetHeaderValue("Date", request) + "\n" +
               GetHeaderValue("If-Modified-Since", request) + "\n" +
               GetHeaderValue("If-Match", request) + "\n" +
               GetHeaderValue("If-None-Match", request) + "\n" +
               GetHeaderValue("If-Unmodified-Since", request) + "\n" +
               GetHeaderValue("Range", request) + "\n" +
               CanonicalizedHeaders(request) + "\n" +
               CanonicalizedResources(request);

            // Sign signature and add as header
            string signature = Convert.ToBase64String(cryptographic.HMACSHA256(Convert.FromBase64String(Configuration.AccountKey), Encoding.UTF8.GetBytes(stringToSign)));
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("SharedKey", Configuration.AccountName.Trim() + ":" + signature);

            var response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new NotFoundException(response.ReasonPhrase);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedException(response.ReasonPhrase);
                }
                else
                {
                    throw new StorageAdapterException(Exceptions.UnknownException + Environment.NewLine + response.StatusCode + ": " + response.ReasonPhrase);
                }
            }

            return response;
        }

        private string GetHeaderValue(string header, HttpRequestMessage request)
        {
            if (header == null)
                throw new ArgumentNullException(nameof(header));

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (header.StartsWith("Content", StringComparison.OrdinalIgnoreCase))
            {
                if (request.Content == null)
                    return "";

                IEnumerable<string> values;
                if (request.Content.Headers.TryGetValues(header, out values))
                {
                    return values.First();
                }
                else
                {
                    return "";
                }
            }
            else
            {
                IEnumerable<string> values;
                if (HttpClient.DefaultRequestHeaders.TryGetValues(header, out values))
                {
                    return values.First();
                }
                else if (request.Headers.TryGetValues(header, out values))
                {
                    return values.First();
                }
                else
                {
                    return "";
                }
            }
        }

        private string CanonicalizedHeaders(HttpRequestMessage request)
        {
            List<string> headers = HttpClient.DefaultRequestHeaders.Concat(request.Headers).Select(x => x.Key.ToLower()).Distinct().ToList();
            headers.RemoveAll(x => !x.StartsWith("x-ms-")); // Remove all none x-ms- headers
            headers.Sort(); // This is suppose to be lexicographically 

            return string.Join("\n", headers.Select(header => (header + ":" + GetHeaderValue(header, request).Replace('\n', ' ')).Trim()));
        }

        private string CanonicalizedResources(HttpRequestMessage request)
        {
            List<string> queryParameters = request.RequestUri.Query.TrimStart('?').Split('&').Select(x => Uri.UnescapeDataString(x.ToLower())).ToList();
            queryParameters.Sort();

            string resourcePath = "/" + Configuration.AccountName.Trim() + request.RequestUri.AbsolutePath + "\n"; 

            if (queryParameters.Any())
            {
                var queryParameterDict = queryParameters.ToLookup(x => x.Split('=')[0], x => x.Split('=')[1]);
                string query = string.Join("\n", queryParameterDict.Select(x => x.Key + ":" + string.Join(",", x)));

                resourcePath += query;
            }

            return resourcePath;
        }
    }
}
 