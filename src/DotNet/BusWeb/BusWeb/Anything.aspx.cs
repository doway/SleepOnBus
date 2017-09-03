using System;
using BusWeb.DAO;

namespace BusWeb
{
    public partial class Anything : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnRun_Click(object sender, EventArgs e)
        {
            using (var dao = BusWebDataService.GetServiceInstance())
            {
                int effectRow = dao.ExecuteAnything(txtCmd.Text);
                lblRowCount.Text = effectRow.ToString();
            }
        }

        protected void btnQry_Click(object sender, EventArgs e)
        {
            using (var dao = BusWebDataService.GetServiceInstance())
            {
                gvData.DataSource = dao.QueryAnything(txtCmd.Text);
                gvData.DataBind();
            }
        }
    }
}
