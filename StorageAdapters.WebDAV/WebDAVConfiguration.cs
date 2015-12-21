namespace StorageAdapters.WebDAV
{
    using System;
    using System.ComponentModel;

    public class WebDAVConfiguration : HTTPStorageConfiguration
    {
        private Uri server;
        public Uri Server
        {
            get { return server; }
            set { server = value; OnPropertyChanged(nameof(Server)); }
        }

        private string username;
        public string UserName
        {
            get { return username; }
            set { username = value; OnPropertyChanged(nameof(UserName)); }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set { password = value; OnPropertyChanged(nameof(Password)); }
        }
    }
}
