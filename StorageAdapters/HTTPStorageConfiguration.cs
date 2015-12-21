namespace StorageAdapters
{
    using System;
    using System.ComponentModel;

    public abstract class HTTPStorageConfiguration : IStorageConfiguration, INotifyPropertyChanged
    {
        public event EventHandler ConfigurationChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public char DirectorySeperator { get { return '/'; } }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

            if (ConfigurationChanged != null)
                ConfigurationChanged(this, EventArgs.Empty);
        }
    }
}
