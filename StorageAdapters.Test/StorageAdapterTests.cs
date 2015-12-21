namespace StorageAdapters.Test
{
    using StorageAdapters;
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public abstract partial class StorageAdapterTests<TFixture, TStorageAdapter> : IClassFixture<TFixture>
        where TStorageAdapter : IStorageAdapter
        where TFixture : StorageAdapterFixture<TStorageAdapter>, new()
    {
        private readonly TFixture fixture;

        protected TStorageAdapter StorageAdapter
        {
            get
            {
                return fixture.Adapter;
            }
        }

        protected string TestPath
        {
            get
            {
                return fixture.TestPath;
            }
        }

        public StorageAdapterTests(TFixture fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [InlineData("CreateDirectory_Simple")]
        [InlineData("CreateDirectory_øæå")]
        [InlineData("CreateDirectory_+-")]
        [InlineData("CreateDirectory_1234567")]
        public async Task CreateDirectory(string directoryName)
        {
            string directoryPath = StorageAdapter.PathCombine(TestPath, directoryName);
            await StorageAdapter.CreateDirectoryAsync(directoryPath);

            Assert.True(await StorageAdapter.DirectoryExistAsync(directoryPath));
        }

        [Theory]
        [InlineData("CreateDirectoryWithSubFolders", new string[] { "abc", "+-", "øæå" })]
        public async Task CreateDirectoryWithSubFolders(string directoryName, string[] subFolders)
        {
            await StorageAdapter.CreateDirectoryAsync(StorageAdapter.PathCombine(TestPath, directoryName));

            foreach (var subFolder in subFolders)
            {
                await StorageAdapter.CreateDirectoryAsync(StorageAdapter.PathCombine(TestPath, directoryName, subFolder));
            }

            Assert.Equal(subFolders.Length, (await StorageAdapter.GetDirectoriesAsync(StorageAdapter.PathCombine(TestPath, directoryName))).Count());
        }

        [Theory]
        [InlineData("DeleteDirectory_ælp")]
        [InlineData("DeleteDirectory_xyz")]
        public async Task DeleteDirectory(string directoryName)
        {
            await StorageAdapter.CreateDirectoryAsync(StorageAdapter.PathCombine(TestPath, directoryName));
            await StorageAdapter.DeleteDirectoryAsync(StorageAdapter.PathCombine(TestPath, directoryName));
        }

        [Theory]
        [InlineData("DeleteDirectoryThrowsNotFound")]
        public async Task DeleteDirectoryThrowsNotFound(string directoryName)
        {
            await Assert.ThrowsAsync(typeof(NotFoundException), () => StorageAdapter.DeleteDirectoryAsync(StorageAdapter.PathCombine(TestPath, directoryName)));
        }

        [Theory]
        [InlineData("CreateDirectoryWithFiles_1", new string[] { "test test.txt", "test+.pdf", "øæå.docx", "xyz.jpeg"})]
        [InlineData("CreateDirectoryWithFiles Test", new string[] { "test test.txt", "test+.pdf", "øæå.docx", "xyz.jpeg" })]
        public async Task CreateDirectoryWithFiles(string directoryName, string[] fileNames)
        {
            var directoryPath = StorageAdapter.PathCombine(TestPath, directoryName);
            await StorageAdapter.CreateDirectoryAsync(directoryPath);

            // Save empty files
            using (MemoryStream ms = new MemoryStream(new byte[0]))
            {
                foreach (var fileName in fileNames)
                {
                    await StorageAdapter.SaveFileAsync(StorageAdapter.PathCombine(directoryPath, fileName), ms);
                }
            }

            // Ensure that files exist
            var filesInDirectory = await StorageAdapter.GetFilesAsync(directoryPath);
            Assert.Equal(fileNames.Length, filesInDirectory.Count());

            // Ensure that they have the correct names
            for (int i = 0; i < fileNames.Length; i++)
            {
                Assert.Contains(filesInDirectory.ElementAt(i).Name, fileNames);
            }
        }

        [Theory]
        [InlineData("CreateAndDeleteFile_22.txt")]
        [InlineData("CreateAndDeleteFile_22 test.pdf")]
        [InlineData("CreateAndDeleteFile_22 +-.pdf")]
        public async Task CreateAndDeleteFile(string fileName)
        {
            // Create empty file
            using (MemoryStream ms = new MemoryStream(new byte[0]))
            {
                await StorageAdapter.SaveFileAsync(StorageAdapter.PathCombine(TestPath, fileName), ms);
            }

            await StorageAdapter.DeleteFileAsync(StorageAdapter.PathCombine(TestPath, fileName));
        }

        [Fact]
        public async Task DeleteFileThrowsNotFound()
        {
            await Assert.ThrowsAsync<NotFoundException>(() => StorageAdapter.DeleteFileAsync(StorageAdapter.PathCombine(TestPath, "DeleteFileThrowsNotFound")));
        }

        [Theory]
        [InlineData("FileExists_22.txt")]
        [InlineData("FileExists_22")]
        [InlineData("FileExists_22 test.pdf")]
        [InlineData("FileExists_22 +-.pdf")]
        public async virtual Task FileExists(string fileName)
        {
            // Create empty file
            using (MemoryStream ms = new MemoryStream(new byte[0]))
            {
                await StorageAdapter.SaveFileAsync(StorageAdapter.PathCombine(TestPath, fileName), ms);
            }

            Assert.True(await StorageAdapter.FileExistAsync(StorageAdapter.PathCombine(TestPath, fileName)));
        }

        [Theory]
        [InlineData("FileNotExists_22.txt")]
        [InlineData("FileNotExists_22 test.pdf")]
        [InlineData("FileNotExists_22 +-.pdf")]
        public async Task FileNotExists(string fileName)
        {
            Assert.False(await StorageAdapter.FileExistAsync(StorageAdapter.PathCombine(TestPath, fileName)));
        }

        [Theory]
        [InlineData("ContentIsNotModified.pdf")]
        [InlineData("ContentIsNotModified.txt")]
        [InlineData("ContentIsNotModified.docx")]
        [InlineData("ContentIsNotModified")]
        public async virtual Task ContentIsNotModified(string fileName)
        {
            // Generate random data
            var random = new Random();
            byte[] buffer = new byte[20001];
            random.NextBytes(buffer);

            // Write file
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                await StorageAdapter.SaveFileAsync(StorageAdapter.PathCombine(TestPath, fileName), ms);
            }

            // Read back file
            using (MemoryStream ms = new MemoryStream())
            {
                using (var stream = await StorageAdapter.ReadFileAsync(StorageAdapter.PathCombine(TestPath, fileName)))
                {
                    await stream.CopyToAsync(ms);
                }

                Assert.True(ms.ToArray().SequenceEqual(buffer));
            }
        }

        [Theory]
        [InlineData("GetFileInfo")]
        [InlineData("GetFileInfo.txt")]
        [InlineData("GetFileInfo+-.txt")]
        [InlineData("GetFileInfo øæå.txt")]
        public async virtual Task GetFileInfo(string fileName)
        {
            // Write file
            using (MemoryStream ms = new MemoryStream(new byte[10]))
            {
                await StorageAdapter.SaveFileAsync(StorageAdapter.PathCombine(TestPath, fileName), ms);
            }

            var fileInfo = await StorageAdapter.GetFileAsync(StorageAdapter.PathCombine(TestPath, fileName));
            Assert.NotNull(fileInfo.Name);
            Assert.NotNull(fileInfo.Path);
            Assert.EndsWith(fileInfo.Name, fileInfo.Path);
            Assert.True(fileInfo.LastModified > DateTimeOffset.Now.AddDays(-1));
            Assert.Equal(10, fileInfo.Size);
        }

        [Fact]
        public async Task GetFileInfoThrowsException()
        {
            await Assert.ThrowsAsync<NotFoundException>(() => StorageAdapter.GetFileAsync("GetFileInfoThrowsException.txt"));
        }
    }

    public abstract class StorageAdapterFixture<TStorageAdapter> : IDisposable
        where TStorageAdapter : IStorageAdapter
    {
        private readonly TStorageAdapter adapter;
        public virtual TStorageAdapter Adapter
        {
            get
            {
                return adapter;
            }
        }

        private readonly string testPath;
        public string TestPath
        {
            get
            {
                return testPath;
            }
        }

        public StorageAdapterFixture()
        {
            adapter = CreateAdapter();
            testPath = CreateTestPath();
        }

        protected abstract TStorageAdapter CreateAdapter();

        protected abstract string CreateTestPath();

        protected abstract void CleanupTestPath(string testPath);

        public void Dispose()
        {
            CleanupTestPath(TestPath);
        }
    }
}
