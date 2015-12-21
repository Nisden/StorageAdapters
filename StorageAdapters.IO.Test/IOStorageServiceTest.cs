namespace StorageAdapters.IO.Test
{
    using Xunit;

    public class IOStorageServiceTest
    {
        [Theory]
        [InlineData("\\..\\Test")]
        [InlineData("//..//Test")]
        [InlineData("..\\..")]
        [InlineData("..\\..\\Test")]
        [InlineData("C:tempdir\\tmp.txt")]
        public void TestRelativeEscape(string path)
        {
            var service = new IOStorageService();
            service.Configuration = new IOConfiguration();

            Assert.Throws<InvalidPathException>(() => service.GetFullPath(path));
        }
    }
}
