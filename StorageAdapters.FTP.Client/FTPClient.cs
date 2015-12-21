namespace StorageAdapters.FTP.Client
{
    using Platform;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// A basic FTP Client implementation.
    /// </summary>
    /// <remarks>
    /// The FTPClient is not thread safe on any instance methods.
    /// </remarks>
    public sealed class FTPClient : IDisposable
    {
        private const string NewLine = "\r\n";

        private readonly static string[] PlatformFactoryTypes = {
            "StorageAdapters.FTP.Client.WindowsStore.PlatformFactory, StorageAdapters.FTP.Client.WindowsStore"
        };

        private readonly IPlatformFactory platformFactory;
        private IPlatformFactory PlatformFactory
        {
            get
            {
                return platformFactory;
            }
        }

        /// <summary>
        /// Response send by the server when connecting, known as the banner.
        /// </summary>
        public string ServerBanner { get; private set; }

        public FTPResponse LastResponse { get; private set; }

        private ITCPSocket socket;
        public ITCPSocket Socket
        {
            get
            {
                return socket;
            }
        }

        private Stream networkStream;
        private StreamReader reader;
        private StreamWriter writer;

        public bool Connected { get; private set; }

        public FTPClient()
        {
            foreach (var platformFactoryType in PlatformFactoryTypes)
            {
                Type factoryType = Type.GetType(platformFactoryType);
                if (factoryType != null)
                {
                    platformFactory = (IPlatformFactory)Activator.CreateInstance(factoryType);
                }
            }

            if (PlatformFactory == null)
            {
                throw new PlatformNotSupportedException("No platform factory found, the following types are supported:" + Environment.NewLine + string.Join(Environment.NewLine, PlatformFactoryTypes));
            }
        }

        /// <summary>
        /// Connects to the target host on the specefied port.
        /// </summary>
        /// <param name="host">Target host, can be a IP Address or DNS name.</param>
        /// <param name="port">Target port, default should be 21</param>
        /// <exception cref="NetworkException">When an error occures in the network stack.</exception>
        /// <exception cref="ProtocolViolationException">When a voilation of the FTP protocol happens.</exception>
        public async Task ConnectAsync(string host, int port)
        {
            // Create the underlying socket that we will use.
            socket = platformFactory.CreateSocket();

            try
            {
                await socket.ConnectAsync(host, port).ConfigureAwait(false);
                Connected = true;

                // Create network stream from socket
                networkStream = await Socket.GetStreamAsync();

                // Create reader and writer
                reader = new StreamReader(networkStream, Encoding.UTF8);
                writer = new StreamWriter(networkStream, new UTF8Encoding(false));
                writer.NewLine = NewLine;

                // Read banner
                ServerBanner = (await ReadFTPResponse()).Text;
            }
            catch (Exception ex)
            {
                Close(); // Cleans up

                throw new NetworkException("Unable to connect", ex);
            }
        }

        public async Task<bool> Login(string username, string password)
        {
            var userResponse = await SendFTPCommand($"USER {username}");
            if (userResponse.StatusCode == FTPStatusCode.LoggedIn)
            {
                // Apprently we don't need a password
                return true;
            }
            else if (userResponse.StatusCode == FTPStatusCode.NeedPassword)
            {
                // Attempt to send password as requested.
                if ((await SendFTPCommand($"PASS {password}")).StatusCode != FTPStatusCode.LoggedIn)
                    return false;
            }
            else
            {
                // Some other response we don't know
                return false;
            }

            return true;
        }

        private async Task<FTPResponse> SendFTPCommand(string command)
        {
            await writer.WriteLineAsync(command);
            await writer.FlushAsync();
            return await ReadFTPResponse();
        }

        private async Task<FTPResponse> ReadFTPResponse()
        {
            try
            {
                FTPResponse response = new FTPResponse();

                // Read the first line of response
                bool readMore = true;
                bool firstLine = true;
                string currentLine;
                while (readMore && !string.IsNullOrEmpty(currentLine = await reader.ReadLineAsync()))
                {
                    // Read status code from the current line
                    int statusCode = 0;
                    if (currentLine.Length >= 3 && int.TryParse(currentLine.Substring(0, 3), out statusCode))
                    {
                        response.StatusCode = statusCode;
                        currentLine = currentLine.Substring(3); // Remove status code
                    }

                    // Ensure that the first line we read have an status code
                    if (statusCode == 0 && firstLine)
                        throw new ProtocolViolationException("First line of response did not contain an status code:" + Environment.NewLine + currentLine);
                    firstLine = false;

                    // Detect if we should keep reading the response or if its finished
                    readMore = (statusCode > 0 && currentLine.StartsWith("-"));

                    if (readMore)
                    {
                        currentLine = currentLine.Substring(1);
                    }

                    if (!string.IsNullOrEmpty(response.Text))
                        response.Text += Environment.NewLine;

                    response.Text += currentLine;
                }

                // Return response
                LastResponse = response;
                return response;
            }
            catch
            {
                Close(); // Close the connection
                throw;
            }
        }

        public void Close()
        {
            Connected = false;

            if (writer != null)
            {
                writer.Dispose();
                writer = null;
            }

            if (reader != null)
            {
                reader.Dispose();
                reader = null;
            }

            if (socket != null)
            {
                socket.Dispose();
                socket = null;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    Close();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~FtpClient() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion


    }
}
