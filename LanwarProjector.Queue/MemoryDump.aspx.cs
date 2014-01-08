using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using System.IO;

namespace LanwarProjector.Queue
{
    public partial class MemoryDump : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Context.Request.QueryString["admin"] == Global.AdminKey)
            {
                var serializer = new XmlSerializer(typeof(Config));
                using (var streamWriter = new MemoryStream())
                {
                    //serializer.Serialize(streamWriter, Global.Config);
                    streamWriter.Flush();
                    streamWriter.Position = 0;
                    using (var streamReader = new StreamReader(streamWriter))
                    {
                        memory.InnerText = streamReader.ReadToEnd();
                    }
                }

            }
        }
    }
}