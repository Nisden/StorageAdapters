namespace StorageAdapters.WebDAV.Test
{
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class WebDAVStorageAdapterTests : StorageAdapters.Test.StorageAdapterTests<WebDAVStorageAdapterFixture, WebDAVStorageAdapter>
    {
        public WebDAVStorageAdapterTests(WebDAVStorageAdapterFixture fixture) : base(fixture)
        { }

        public async override Task ContentIsNotModified(string fileName)
        {
            if (string.IsNullOrEmpty(System.IO.Path.GetExtension(fileName)))
            {
                return;
            }

            await base.ContentIsNotModified(fileName);
        }

        public async override Task FileExists(string fileName)
        {
            if (string.IsNullOrEmpty(System.IO.Path.GetExtension(fileName)))
            {
                return;
            }

            await base.FileExists(fileName);
        }

        public async override Task GetFileInfo(string fileName)
        {
            if (string.IsNullOrEmpty(System.IO.Path.GetExtension(fileName)))
            {
                return;
            }

            await base.GetFileInfo(fileName);
        }
    }

    public class WebDAVStorageAdapterFixture : StorageAdapters.Test.StorageAdapterFixture<WebDAVStorageAdapter>
    {
        protected override WebDAVStorageAdapter CreateAdapter()
        {
            return new WebDAVStorageAdapter(new WebDAVConfiguration()
            {
                Server = new Uri("http://localhost/WebDAV/", UriKind.Absolute),
                UserName = "Test",
                Password = "Test"
            });
        }

        protected override string CreateTestPath()
        {
            var testPath = Guid.NewGuid().ToString();
            Adapter.CreateDirectoryAsync(testPath).Wait();

            return testPath;
        }

        protected override void CleanupTestPath(string testPath)
        {
            Adapter.DeleteDirectoryAsync(testPath).Wait();
        }
    }
}
