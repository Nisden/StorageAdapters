namespace StorageAdapters.FTP.Client.WindowsStore.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using System.Threading.Tasks;
    using Platform;

    [TestClass]
    public class FtpClientTests
    {
        [TestMethod]
        public async Task Connect_InvalidAddress()
        {
            try
            {
                FTPClient ftp = new FTPClient();
                await ftp.ConnectAsync("somethingthatdontexist.somethingthatreallysuckssssss.somethig", 21);

                Assert.IsTrue(false);
            }
            catch (NetworkException)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public async Task Connect_GetsBanner()
        {
            using (FTPClient ftp = new FTPClient())
            {
                await ftp.ConnectAsync("localhost", 21);
                Assert.IsNotNull(ftp.ServerBanner);
            }
        }

        [TestMethod]
        public async Task AttemptLogin()
        {
            using (FTPClient ftp = new FTPClient())
            {
                await ftp.ConnectAsync("localhost", 21);
                Assert.IsTrue(await ftp.Login("Test", "Test"));
            }
        }
    }
}
