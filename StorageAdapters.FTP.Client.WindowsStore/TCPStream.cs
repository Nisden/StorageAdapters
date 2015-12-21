namespace StorageAdapters.FTP.Client.WindowsStore
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Windows.Storage.Streams;

    public sealed class TCPStream : System.IO.Stream
    {
        private readonly Stream inputStream;
        private readonly Stream outputStream;

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override long Length
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public TCPStream(IInputStream inputStream, IOutputStream outputStream)
        {
            if (inputStream == null)
                throw new ArgumentNullException(nameof(inputStream));

            if (outputStream == null)
                throw new ArgumentNullException(nameof(outputStream));

            this.inputStream = inputStream.AsStreamForRead(0);
            this.outputStream = outputStream.AsStreamForWrite(0);
        }

        #region Unimplemented Sync Methods

        public override void Flush()
        {
            try
            {
                outputStream.Flush();
            }
            catch (Exception ex)
            {
                throw NetworkUtility.HandleException(ex);
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            try
            {
                return inputStream.Read(buffer, offset, count);
            }
            catch (Exception ex)
            {
                throw NetworkUtility.HandleException(ex);
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            try
            {
                outputStream.Write(buffer, offset, count);
            }
            catch (Exception ex)
            {
                throw NetworkUtility.HandleException(ex);
            }
        }

        #endregion

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            try
            {
                return inputStream.ReadAsync(buffer, offset, count, cancellationToken);
            }
            catch (Exception ex)
            {
                throw NetworkUtility.HandleException(ex);
            }
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            try
            {
                return outputStream.WriteAsync(buffer, offset, count, cancellationToken);
            }
            catch (Exception ex)
            {
                throw NetworkUtility.HandleException(ex);
            }
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            try
            {
                return outputStream.FlushAsync();
            }
            catch (Exception ex)
            {
                throw NetworkUtility.HandleException(ex);
            }
        }

        protected override void Dispose(bool disposing)
        {
            outputStream.Dispose();
            inputStream.Dispose();
        }
    }
}
