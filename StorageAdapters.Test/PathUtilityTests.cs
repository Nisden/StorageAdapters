namespace StorageAdapters.Test
{
    using Xunit;

    public class PathUtilityTests
    {
        [Theory]
        [InlineData('\\', new string[] { "Test", "Test2", "Test3" }, "Test\\Test2\\Test3")]
        [InlineData('/', new string[] { "Test", "Test2", "Test3" }, "Test/Test2/Test3")]
        [InlineData('/', new string[] { "Test", "Test2", "", "Test3" }, "Test/Test2/Test3")]
        [InlineData('/', new string[] { "Test", "Test2", "", "Test3", "" }, "Test/Test2/Test3")]
        public void PathCombine(char seperator, string[] paths, string result)
        {
            Assert.Equal(result, PathUtility.Combine(seperator, paths));
        }

        [Theory]
        [InlineData('\\', "Test/Test/Test", "Test\\Test\\Test")]
        [InlineData('\\', "Test\\Test\\Test", "Test\\Test\\Test")]
        [InlineData('/', "Test/Test\\Test", "Test/Test/Test")]
        public void CleanPath(char seperator, string path, string result)
        {
            Assert.Equal(result, PathUtility.Clean(seperator, path));
        }
    }
}
