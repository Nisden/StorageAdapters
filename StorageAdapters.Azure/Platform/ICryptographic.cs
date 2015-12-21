namespace StorageAdapters.Azure.Platform
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ICryptographic
    {
        byte[] HMACSHA256(byte[] key, byte[] buffer);
    }
}
