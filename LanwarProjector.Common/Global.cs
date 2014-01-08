using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace LanwarProjector.Common
{
    public static class Global
    {
        public static byte[] EncryptionKey = ConfigurationManager.AppSettings["EncyrptionKey"].Split(',').Cast<Byte>().ToArray();
    }
}
