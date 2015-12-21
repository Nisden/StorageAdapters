using System.Threading;
using System.Threading.Tasks;

namespace StorageAdapters.WebDAV
{
    public sealed class WebDAVStorageAdapter : StorageAdapterBase<WebDAVStorageService, WebDAVConfiguration>
    {
        public WebDAVStorageAdapter(WebDAVConfiguration configuration) : base(configuration)
        { }

        public override Task DeleteDirectoryAsync(string path, bool recursive, CancellationToken cancellationToken)
        {
            // Delete Directory on WebDAV is always recursive
            // https://tools.ietf.org/html/rfc4918#section-9.6.1
            return Service.DeleteDirectoryAsync(path, cancellationToken);
        }

        protected override string PathClean(string path, bool file)
        {
            if (file && string.IsNullOrEmpty(PathUtility.GetFileExtension(path)))
            {
                throw new InvalidPathException("Filenames on WebDAV must have an extension");
            }

            return base.PathClean(path, file);
        }
    }
}
