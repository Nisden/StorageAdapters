namespace StorageAdapters.FTP.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class FTPResponse
    {
        public int StatusCode { get; set; }

        public string Text { get; set; }

        public override string ToString()
        {
            return $"{StatusCode} - {Text}";
        }
    }
}
