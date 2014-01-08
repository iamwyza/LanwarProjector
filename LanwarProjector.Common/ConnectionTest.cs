using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.IO;
using System.Net;
using LanwarProjector.Common.Domain;
using log4net;

namespace LanwarProjector.Common
{
    public static class ConnectionTest
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ConnectionTest));

        public static Tuple<int,int> Test(ConnectionInfo conninfo)
        {
            NetworkCredential creds;

            if (conninfo.Username.Contains("\\"))
            {
                var temp = conninfo.Username.Split('\\');
                creds = new NetworkCredential(temp[1], System.Text.Encoding.UTF8.GetString(Encryption.AESGCM.SimpleDecrypt(conninfo.Password, Global.EncryptionKey)), temp[0]);
            }
            else
            {
                creds = new NetworkCredential(conninfo.Username, System.Text.Encoding.UTF8.GetString(Encryption.AESGCM.SimpleDecrypt(conninfo.Password, Global.EncryptionKey)));
            }

            using (new Utilities.NetworkConnection(conninfo.Path, creds))
            {
                var files = Directory.GetFiles(conninfo.Path).Count();
                var directores = Directory.GetDirectories(conninfo.Path).Count();
                return new Tuple<int, int>(files, directores);
            }
        }
    }
}