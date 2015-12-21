namespace StorageAdapters.FTP.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class FTPStatusCode
    {
        #region 200 - The requested action has been successfully completed.

        public const int LoggedIn = 230;

        #endregion

        #region 300 - The command has been accepted, but the requested action is on hold, pending receipt of further information.

        public const int NeedPassword = 331;

        #endregion
    }
}
