namespace StorageAdapters.Azure.WindowsStore
{
    using StorageAdapters.Azure.Platform;
    using System;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using Windows.Security.Cryptography.Core;

    public class Cryptographic : ICryptographic
    {
        private static MacAlgorithmProvider hmacsha = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacSha256);

        public byte[] HMACSHA256(byte[] key, byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            var hash = hmacsha.CreateHash(key.AsBuffer());
            hash.Append(buffer.AsBuffer());
            return hash.GetValueAndReset().ToArray();
        }
    }
}
