namespace StorageAdapters.InMemory.Test
{
    using System;

    public class InMemoryStorageAdapterTests : StorageAdapters.Test.StorageAdapterTests<InMemoryStorageAdapterFixture, InMemoryStorageAdapter>
    {
        public InMemoryStorageAdapterTests(InMemoryStorageAdapterFixture fixture) : base(fixture)
        { }
    }

    public class InMemoryStorageAdapterFixture : StorageAdapters.Test.StorageAdapterFixture<InMemoryStorageAdapter>
    {
        protected override void CleanupTestPath(string testPath)
        {
            
        }

        protected override InMemoryStorageAdapter CreateAdapter()
        {
            return new InMemoryStorageAdapter(new InMemoryConfiguration());
        }

        protected override string CreateTestPath()
        {
            string testPath = Guid.NewGuid().ToString();
            Adapter.CreateDirectoryAsync(testPath).Wait();

            return testPath;
        }
    }
}
