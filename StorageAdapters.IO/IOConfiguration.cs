namespace StorageAdapters.IO
{
    using System.IO;

    public class IOConfiguration : IStorageConfiguration
    {
        public string BaseDirectory { get; set; } = "%TEMP%";

        public bool ExpandEnvironmentVariables { get; set; } = true;

        public bool AllowBaseDirectoryEscape { get; set; } = false;

        public char DirectorySeperator
        {
            get
            {
                return Path.DirectorySeparatorChar;
            }
        }
    }
}
