namespace StorageAdapters.Azure.Desktop
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
