using System;
using System.Web.UI;
using BusWeb.DAO;

namespace BusWeb
{
    public partial class AddNew : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                refreshLineList();
            }
        }

        private void refreshLineList()
        {
            var dao = BusWebDataService.GetServiceInstance();
            {
                var dt = dao.GetLinesList();
                foreach (var dr in dt) dr.LineName = string.Format("{0}({1})", dr.LineName, dr.LineID);
                ddlLine.DataSource = dt;
                ddlLine.DataBind();
            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            var gp = parseGMapUrl(txtGMapURL.Text);
            var dao = BusWebDataService.GetServiceInstance();
            {
                int lineID = 0;
                if (!string.IsNullOrEmpty(txtNewLineName.Text))
                {
                    lineID = dao.InsertNewLine(txtNewLineName.Text, "");
                }
                else
                {
                    lineID = int.Parse(ddlLine.SelectedValue);
                }

                int StopID = dao.InsertNewStop(txtNewStopName.Text, gp.longitude, gp.latitude, 0d, 0d, "SYSTEM", "");
                dao.InsertNewLine2StopRelation(lineID, StopID);
            }
            if (!string.IsNullOrEmpty(txtNewLineName.Text)) refreshLineList();
            txtNewLineName.Text = string.Empty;
            ClientScript.RegisterClientScriptBlock(typeof(string), "alert", "<script lang=jscript>alert('站點新增成功');</script>");
        }

        private GeoPoint parseGMapUrl(string url)
        {
            // sample:
            // http://maps.google.com.tw/maps?hl=zh-TW&ie=UTF8&ll=25.030924,121.558569&spn=0.01184,0.01929&hnear=110%E5%8F%B0%E5%8C%97%E5%B8%82%E4%BF%A1%E7%BE%A9%E5%8D%80%E5%9F%BA%E9%9A%86%E8%B7%AF%E4%BA%8C%E6%AE%B553%E8%99%9F&t=m&z=16&vpsrc=0&brcurrent=3,0x3442abadec7543e7:0x5dbcdd8252aeabe7,1,0x3442ac6b61dbbd9d:0xc0c243da98cba64b
            int llIdx = url.IndexOf("ll") + 3;
            string location = url.Substring(llIdx, url.IndexOf("&", llIdx) - llIdx);
            string[] ll=location.Split(',');
            GeoPoint gp = new GeoPoint() { latitude = double.Parse(ll[0]), longitude = double.Parse(ll[1]) };
            return gp;            
        }

        struct GeoPoint
        {
            public double latitude;
            public double longitude;
        }
    }
}
