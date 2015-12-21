namespace StorageAdapters.Azure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public enum AzureBlobPublicBlobAccess
    {
        /// <summary>
        /// Specifies container data is private to the account owner
        /// </summary>
        @private,

        /// <summary>
        /// Specifies full public read access for container and blob data. Clients can enumerate blobs within the container via anonymous request, but cannot enumerate containers within the storage account.
        /// </summary>
        container,

        /// <summary>
        /// Specifies public read access for blobs. Blob data within this container can be read via anonymous request, but container data is not available. Clients cannot enumerate blobs within the container via anonymous request.
        /// </summary>
        blob
    }
}
