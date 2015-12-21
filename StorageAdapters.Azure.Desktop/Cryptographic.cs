namespace StorageAdapters.Azure.Desktop
{
    using StorageAdapters.Azure.Platform;
    using System.Security.Cryptography;

    public class Cryptographic : ICryptographic
    {
        public byte[] HMACSHA256(byte[] key, byte[] buffer)
        {
            using (var hmacsha = new HMACSHA256(key))
            {
                return hmacsha.ComputeHash(buffer);
            }
        }
    }
}
