namespace StorageAdapters.IO.Test
{
    using StorageAdapters.Test;
    using System;

    public class IOStorageAdapterTest : StorageAdapterTests<IOStorageAdapterFixture, IOStorageAdapter>
    {
        public IOStorageAdapterTest(IOStorageAdapterFixture fixture) : base(fixture)
        { }
    }

    public class IOStorageAdapterFixture : StorageAdapterFixture<IOStorageAdapter>
    {
        protected override IOStorageAdapter CreateAdapter()
        {
            return new IOStorageAdapter(new IOConfiguration());
        }

        protected override string CreateTestPath()
        {
            var path = Guid.NewGuid().ToString();
            Adapter.CreateDirectoryAsync(path).Wait();

            return path;
        }

        protected override void CleanupTestPath(string testPath)
        {
            Adapter.DeleteDirectoryAsync(TestPath).Wait();
        }
    }
}
