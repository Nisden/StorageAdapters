namespace StorageAdapters.Streams
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// This is a stream wrapper that prevent components from closing the underlying stream.
    /// Ensure that you always dispose the <see cref="sourceStream"/> on your own.
    /// </summary>
    public sealed class UncloseableStream : Stream
    {
        public override bool CanRead
        {
            get { return sourceStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return sourceStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return sourceStream.CanWrite; }
        }

        public override long Length
        {
            get { return sourceStream.Length; }
        }

        public override long Position
        {
            get { return sourceStream.Position; }
            set { sourceStream.Position = value; }
        }

        private readonly Stream sourceStream;

        public UncloseableStream(Stream sourceStream)
        {
            this.sourceStream = sourceStream;
        }

        public override void Flush()
        {
            sourceStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return sourceStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return sourceStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            sourceStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            sourceStream.Write(buffer, offset, count);
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            return sourceStream.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return sourceStream.FlushAsync(cancellationToken);
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return sourceStream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override int ReadByte()
        {
            return sourceStream.ReadByte();
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return sourceStream.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override void WriteByte(byte value)
        {
            sourceStream.WriteByte(value);
        }
    }
}
