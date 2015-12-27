namespace StorageAdapters.Azure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AzureFileInfo : Generic.GenericFileInfo
    {
        public string BlobType { get; set; }
        public string MD5 { get; set; }
    }
}
