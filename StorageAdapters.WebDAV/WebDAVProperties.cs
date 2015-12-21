namespace StorageAdapters.WebDAV
{
    /// <summary>
    /// DAV Properties, <see cref="https://tools.ietf.org/html/rfc4918#section-15"/>
    /// </summary>
    internal static class WebDAVProperties
    {
        /// <summary>
        /// Records the time and date the resource was created.
        /// </summary>
        public const string CreationDate = "creationdate";

        /// <summary>
        /// Provides a name for the resource that is suitable for presentation to a user.
        /// </summary>
        public const string DisplayName = "displayname";

        /// <summary>
        /// Contains the Content-Language header value (from Section 14.12 of[RFC2616]) as it would be returned by a GET without accept headers.
        /// </summary>
        public const string GetContentLanguage = "getcontentlanguage";

        /// <summary>
        /// Contains the Content-Length header returned by a GET without accept headers.
        /// </summary>
        public const string GetContentLength = "getcontentlength";

        /// <summary>
        /// Contains the Content-Type header value (from Section 14.17 of[RFC2616]) as it would be returned by a GET without accept headers.
        /// </summary>
        public const string GetContentType = "getcontenttype";

        /// <summary>
        ///  Contains the ETag header value (from Section 14.19 of [RFC2616]) as it would be returned by a GET without accept headers.
        /// </summary>
        public const string GetETag = "getetag";

        /// <summary>
        /// Contains the Last-Modified header value (from Section 14.29 of[RFC2616]) as it would be returned by a GET method without accept headers.
        /// </summary>
        public const string GetLastModified = "getlastmodified";

        /// <summary>
        /// Describes the active locks on a resource
        /// </summary>
        public const string LockDiscovery = "lockdiscovery";

        /// <summary>
        /// Specifies the nature of the resource.
        /// </summary>
        public const string ResourceType = "resourcetype";

        /// <summary>
        /// To provide a listing of the lock capabilities supported by the resource.
        /// </summary>
        public const string SupportedLock = "supportedlock";
    }
}
