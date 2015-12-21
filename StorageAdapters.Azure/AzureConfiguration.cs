namespace StorageAdapters.Azure
{
    public sealed class AzureConfiguration : HTTPStorageConfiguration
    {
        public const string PublicAPIAddress = "{0}.blob.core.windows.net";

        public const string DeveloperAPIAddress = "http://127.0.0.1:10000/{0}";
        public const string DeveloperAccountName = "devstoreaccount1";
        public const string DeveloperAccountKey = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";

        public string AzureVersion { get; set; } = "2015-04-05";

        public string ClientRequestId { get; set; } = "Azure StorageAdapter";

        public string APIAddress { get; set; } = "{0}.blob.core.windows.net";

        public bool UseHTTPS { get; set; } = true;

        public string AccountName { get; set; }

        public string AccountKey { get; set; }

        public AzureBlobPublicBlobAccess DefaultContainerAccess { get; set; } = AzureBlobPublicBlobAccess.@private;
    }
}
