using System;
using System.Web.UI;
using BusWeb.DAO;

namespace BusWeb
{
    public partial class BusWebFront : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (null == Session["PASS"] || !(bool)Session["PASS"])
            {
                Response.Redirect("~/BusLogin.aspx");
                Response.End();
            }
        }

        protected void lbtnLogout_Click(object sender, EventArgs e)
        {
            Session.Remove("PASS");
        }

        protected void btnRemoveEmptyLines_Click(object sender, EventArgs e)
        {
            var db = BusWebDataService.GetServiceInstance();
                db.RemoveEmptyLines();
            Response.Redirect("~/DataManager.aspx");
            Response.End();
        }

        protected void btnMerge_Click(object sender, EventArgs e)
        {
            var db = BusWebDataService.GetServiceInstance();
                //db.MergeLines();
            Response.Redirect("~/DataManager.aspx");
            Response.End();
        }

        protected void btnManualMerge_Click(object sender, EventArgs e)
        {
            bool success = false;
            var db = BusWebDataService.GetServiceInstance();
                //success = db.MergeLines(int.Parse(txtLineId1.Text), int.Parse(txtLineId2.Text));

            if (success)
            {
                Response.Redirect("~/DataManager.aspx");
                Response.End();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "mergeError", "alert('路線合併失敗!');", true);
            }
        }
    }
}
