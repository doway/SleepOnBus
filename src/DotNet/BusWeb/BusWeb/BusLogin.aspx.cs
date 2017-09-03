using System;
using System.Web.UI;

namespace BusWeb
{
    public partial class BusLogin : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (txtPwd.Text == "35171124")
            {
                Session["PASS"] = true;
                Response.Redirect("~/DataManager.aspx");
                Response.End();
            }
            else
                Session["PASS"] = false;
        }
    }
}
