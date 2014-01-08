using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LanwarProjector.Queue
{
    public partial class Manager : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["admin"] != Global.AdminKey)
            {
                form1.Visible = false;
                deny.Visible = true;
            }
            else
            {
                deny.Visible = false;
            }
        }
    }
}