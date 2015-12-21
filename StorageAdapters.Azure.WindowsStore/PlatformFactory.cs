namespace StorageAdapters.Azure.WindowsStore
{
    using Platform;

    public class PlatformFactory : IPlatformFactory
    {
        public ICryptographic CreateCryptograhicModule()
        {
            return new Cryptographic();
        }
    }
}
