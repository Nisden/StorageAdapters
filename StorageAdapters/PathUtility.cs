namespace StorageAdapters
{
    using System;
    using System.Linq;

    public static class PathUtility
    {
        public static string Combine(char seperator, params string[] paths)
        {
            return string.Join(seperator.ToString(), paths.Where(pathPart => !string.IsNullOrWhiteSpace(pathPart)));
        }

        public static string Clean(char seperator, string path)
        {
            return path.Trim('\\', '/').Replace('/', seperator).Replace('\\', seperator);
        }

        public static string GetDirectoryName(char seperator, string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return string.Join(seperator.ToString(), Clean(seperator, path).Split(seperator).Reverse().Skip(1).Reverse());
        }

        public static string GetFileName(char seperator, string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return Clean(seperator, path).Split(seperator).Last();
        }

        public static string GetFileExtension(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var fileName = path.Split('\\', '/').Last();
            var fileNameSplitWithExtension = fileName.Split('.');

            if (fileNameSplitWithExtension.Length == 1)
                return "";
            else
                return fileNameSplitWithExtension.Last();
        }

        public static bool IsRootPath(string path)
        {
            if (path == null)
            {
                return true;
            }
            else if (string.IsNullOrWhiteSpace(path.Trim()))
            {
                return true;
            }
            else if (new string[] { "/", "\\" }.Contains(path.Trim()))
            {
                return true;
            }

            return false;
        }
    }
}
