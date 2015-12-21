namespace StorageAdapters.Azure.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class AzureStorageAdapterTest : StorageAdapters.Test.StorageAdapterTests<AzureStorageAdapterFixture, AzureStorageAdapter>
    {
        public AzureStorageAdapterTest(AzureStorageAdapterFixture fixture) : base(fixture)
        { }
    }

    public class AzureStorageAdapterFixture : StorageAdapters.Test.StorageAdapterFixture<AzureStorageAdapter>
    {
        protected override void CleanupTestPath(string testPath)
        {
            Adapter.DeleteDirectoryAsync("test").Wait();
        }

        protected override AzureStorageAdapter CreateAdapter()
        {
            return new AzureStorageAdapter(new AzureConfiguration()
            {
                APIAddress = AzureConfiguration.DeveloperAPIAddress,
                AccountName = AzureConfiguration.DeveloperAccountName,
                AccountKey = AzureConfiguration.DeveloperAccountKey,
                UseHTTPS = false
            });
        }

        protected override string CreateTestPath()
        {
            Adapter.CreateDirectoryAsync("test").Wait();
            return "test";
        }
    }
}
