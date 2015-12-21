namespace StorageAdapters.WebDAV
{
    using Generic;
    using Streams;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public sealed class WebDAVStorageService : HTTPStorageServiceBase<WebDAVConfiguration>
    {
        public override async Task CreateDirectoryAsync(string path, CancellationToken cancellationToken)
        {
            if (Configuration == null)
                throw new InvalidOperationException(Exceptions.ConfigurationMustBeSet);

            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var response = await HttpClient.SendAsync(new HttpRequestMessage(new HttpMethod(WebDAVMethods.MakeCollection), EncodePath(path)), cancellationToken).ConfigureAwait(false);
            HandleErrorStatus(response, path, false);
        }

        public override async Task DeleteDirectoryAsync(string path, CancellationToken cancellationToken)
        {
            if (Configuration == null)
                throw new InvalidOperationException(Exceptions.ConfigurationMustBeSet);

            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var response = await HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, EncodePath(path)), cancellationToken).ConfigureAwait(false);
            HandleErrorStatus(response, path, null); // Used by both Directory and File
        }

        public override Task DeleteFileAsync(string path, CancellationToken cancellationToken)
        {
            if (Configuration == null)
                throw new InvalidOperationException(Exceptions.ConfigurationMustBeSet);

            // Delete Directory and Delete File is the same for WebDAV
            return DeleteDirectoryAsync(path, cancellationToken);
        }

        public override async Task<bool> DirectoryExistAsync(string path, CancellationToken cancellationToken)
        {
            if (Configuration == null)
                throw new InvalidOperationException(Exceptions.ConfigurationMustBeSet);

            if (path == null)
                throw new ArgumentNullException(nameof(path));

            XNamespace davNamespace = @"DAV:";
            var xml = new XDocument(
                new XElement(davNamespace + "propfind",
                    new XElement(davNamespace + "prop",
                        new XElement(davNamespace + WebDAVProperties.ResourceType)
                    )
                )
            );

            try
            {
                await SendRequest(path, WebDAVMethods.PropertyFind, xml, 0);
            }
            catch (NotFoundException)
            {
                return false;
            }

            return true;
        }

        public override async Task<bool> FileExistAsync(string path, CancellationToken cancellationToken)
        {
            if (Configuration == null)
                throw new InvalidOperationException(Exceptions.ConfigurationMustBeSet);

            var response = await HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, EncodePath(path)), cancellationToken).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }
            else if (response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                // This should always throw an exception before returning.
                HandleErrorStatus(response, path, true);
                return false;
            }
        }

        public override async Task<IEnumerable<IVirtualDirectory>> GetDirectoriesAsync(string path, CancellationToken cancellationToken)
        {
            if (Configuration == null)
                throw new InvalidOperationException(Exceptions.ConfigurationMustBeSet);

            // Create request
            XNamespace davNamespace = @"DAV:";
            var xml = new XDocument(
                new XElement(davNamespace + "propfind",
                    new XElement(davNamespace + "prop",
                        new XElement(davNamespace + WebDAVProperties.ResourceType)
                    )
                )
            );

            // Send
            var response = await SendRequest(EncodePath(path), WebDAVMethods.PropertyFind, xml, 1).ConfigureAwait(false);

            // Parse response
            var directories = from dir in response.Descendants(davNamespace + "response")
                              where dir.Descendants(davNamespace + "collection").Any()
                              let href = dir.Element(davNamespace + "href").Value
                              let name = Uri.UnescapeDataString(href.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Last())
                              select new GenericDirectory
                              {
                                  Name = name,
                                  Path = PathUtility.Combine(Configuration.DirectorySeperator, path, name)
                              };

            return directories.Skip(1).ToArray();
        }

        public override async Task<IVirtualFileInfo> GetFileAsync(string path, CancellationToken cancellationToken)
        {
            if (Configuration == null)
                throw new InvalidOperationException(Exceptions.ConfigurationMustBeSet);

            var response = await HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, EncodePath(path)), cancellationToken).ConfigureAwait(false);
            HandleErrorStatus(response, path, true);

            return new GenericFileInfo()
            {
                Name = PathUtility.GetFileName(Configuration.DirectorySeperator, path),
                Path = path,
                Size = response.Content.Headers.ContentLength.GetValueOrDefault(),
                LastModified = response.Content.Headers.LastModified.GetValueOrDefault()
            };
        }

        public override async Task<IEnumerable<IVirtualFileInfo>> GetFilesAsync(string path, CancellationToken cancellationToken)
        {
            if (Configuration == null)
                throw new InvalidOperationException(Exceptions.ConfigurationMustBeSet);

            // Create request
            XNamespace davNamespace = @"DAV:";
            var xml = new XDocument(
                new XElement(davNamespace + "propfind",
                    new XElement(davNamespace + "prop",
                        new XElement(davNamespace + WebDAVProperties.ResourceType),
                        new XElement(davNamespace + WebDAVProperties.GetContentLength),
                        new XElement(davNamespace + WebDAVProperties.GetLastModified)
                    )
                )
            );

            // Send and get response
            var response = await SendRequest(EncodePath(path), WebDAVMethods.PropertyFind, xml, 1).ConfigureAwait(false);

            // Parse response
            var files = from file in response.Descendants(davNamespace + "response")
                        where !file.Descendants(davNamespace + "collection").Any() // Filter out directories
                        let href = file.Element(davNamespace + "href").Value
                        let name = Uri.UnescapeDataString(href.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Last())
                        let properties = file.Descendants(davNamespace + "prop").First()
                        let lastModified = DateTimeOffset.ParseExact(properties.Element(davNamespace + WebDAVProperties.GetLastModified).Value, "r", CultureInfo.InvariantCulture)
                        let size = int.Parse(properties.Element(davNamespace + WebDAVProperties.GetContentLength).Value)
                        select new GenericFileInfo
                        {
                            Name = name,
                            Path = PathUtility.Combine(Configuration.DirectorySeperator, path, name),
                            Size = size,
                            LastModified = lastModified
                        };

            return files.ToArray();
        }

        public override async Task<Stream> ReadFileAsync(string path, CancellationToken cancellationToken)
        {
            if (Configuration == null)
                throw new InvalidOperationException(Exceptions.ConfigurationMustBeSet);

            var response = await HttpClient.GetAsync(EncodePath(path), cancellationToken).ConfigureAwait(false);
            HandleErrorStatus(response, path, true);

            return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        }

        public override async Task SaveFileAsync(string path, Stream stream, CancellationToken cancellationToken)
        {
            if (Configuration == null)
                throw new InvalidOperationException(Exceptions.ConfigurationMustBeSet);

            var response = await HttpClient.PutAsync(EncodePath(path), new StreamContent(new UncloseableStream(stream)), cancellationToken);
            HandleErrorStatus(response, path, true);
        }

        protected override void ConfigureHttpClientHandler(HttpClientHandler clientHandler)
        {
            // Create credentials
            if (Configuration.UserName != null)
            {
                clientHandler.Credentials = new NetworkCredential(Configuration.UserName, Configuration.Password);
            }

            base.ConfigureHttpClientHandler(clientHandler);
        }

        protected override void ConfigureHttpClient(HttpClient client)
        {
            client.BaseAddress = Configuration.Server;

            base.ConfigureHttpClient(client);
        }

        /// <summary>
        /// Sends a "WebDAV" request.
        /// </summary>
        /// <param name="path">Target path</param>
        /// <param name="method">WebDAV Method, see <see cref="WebDAVMethods"/></param>
        /// <param name="body">XML body of the request</param>
        /// <param name="depth">Depth of WebDAV request, 0 = Target, 1 = Target and sub</param>
        /// <returns></returns>
        private async Task<XDocument> SendRequest(string path, string method, XDocument body, int depth = 1)
        {
            if (Configuration == null)
                throw new InvalidOperationException(Exceptions.ConfigurationMustBeSet);

            // Create request
            var message = new HttpRequestMessage(new HttpMethod(method), path);
            message.Content = new StringContent(body.ToString(SaveOptions.None | SaveOptions.OmitDuplicateNamespaces));
            message.Headers.Add("Depth", depth.ToString());

            // Send request and get response
            var response = await HttpClient.SendAsync(message);

            // Check for response errors
            HandleErrorStatus(response, path, false);

            // Read response body
            return XDocument.Parse(await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Throws the correct exception based on the response status code.
        /// </summary>
        /// <param name="response">HTTP Response.</param>
        /// <param name="path">Request path.</param>
        /// <param name="isFile">If its a file, directory or both.</param>
        private void HandleErrorStatus(HttpResponseMessage response, string path, bool? isFile)
        {
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    if (string.IsNullOrEmpty(Configuration.UserName))
                    {
                        throw new UnauthorizedException(Exceptions.UsernameAndPasswordRequired);
                    }
                    else
                    {
                        throw new UnauthorizedException(Exceptions.InvalidUsernameAndPassword);
                    }
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    if (isFile == true)
                    {
                        throw new NotFoundException(string.Format(Exceptions.FileNotFound, path));
                    }
                    else if (isFile == false)
                    {
                        throw new NotFoundException(string.Format(Exceptions.DirectoryNotFound, path));
                    }
                    else
                    {
                        throw new NotFoundException(string.Format(Exceptions.FileOrDirectoryNotFound, path));
                    }
                }
                else
                {
                    throw new StorageAdapterException(Exceptions.UnknownException + Environment.NewLine + response.ToString());
                }
            }
        }
    }
}
