using System;
using System.Data;
using System.Web.UI.WebControls;
using BusWeb.DAO;
using BusWeb.DAO.DataSet;
using DOWILL.Web;

namespace BusWeb
{
    public partial class DataManager : VersatilePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PreRender += new EventHandler(DataManager_PreRender);
        }

        void DataManager_PreRender(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblServerRootPath.Text = "Server Path=" + Server.MapPath("~/");
                bgnDatePicker.SelectedDate = DateTime.Today.AddMonths(-3);
                endDatePicker.SelectedDate = DateTime.Today.AddDays(1);
                btnQry_Click(sender, e);
            }
        }

        private void Grid1Bind()
        {
            using (var dao = BusWebDataService.GetServiceInstance())
            {
                DataTable dt = dao.GetLinesList(bgnDatePicker.SelectedDate, endDatePicker.SelectedDate);
                if (!string.IsNullOrEmpty(Grid1SortExp)) dt.DefaultView.Sort = Grid1SortExp + " " + ((Grid1SortDir == SortDirection.Ascending) ? "ASC" : "DESC");
                GridView1.DataSource = dt.DefaultView;
                GridView1.DataBind();
            }
        }

        private void Grid2Bind()
        {
            using (var dao = BusWebDataService.GetServiceInstance())
            {
                DataTable dt = dao.GetStopsByLineID(LineID);
                if (!string.IsNullOrEmpty(Grid2SortExp)) dt.DefaultView.Sort = Grid2SortExp + " " + ((Grid2SortDir == SortDirection.Ascending) ? "ASC" : "DESC");
                GridView2.DataSource = dt.DefaultView;
                GridView2.DataBind();
            }
        }

        private int LineID
        {
            get { return Convert.ToInt32(ViewState["LineID"]); }
            set { ViewState["LineID"] = value; }
        }

        private string LineName
        {
            get { return Convert.ToString(ViewState["LineName"]); }
            set { ViewState["LineName"] = value; }
        }

        private SortDirection Grid1SortDir
        {
            set { ViewState["G1Sort"] = value; }
            get
            {
                if (null == ViewState["G1Sort"]) ViewState["G1Sort"] = SortDirection.Ascending;
                return (SortDirection)ViewState["G1Sort"];
            }
        }

        private string Grid1SortExp
        {
            set { ViewState["G1SortExp"] = value; }
            get
            {
                if (null == ViewState["G1SortExp"]) ViewState["G1SortExp"] = string.Empty;
                return (string)ViewState["G1SortExp"];
            }
        }

        private SortDirection Grid2SortDir
        {
            set { ViewState["G2Sort"] = value; }
            get
            {
                if (null == ViewState["G2Sort"]) ViewState["G2Sort"] = SortDirection.Ascending;
                return (SortDirection)ViewState["G2Sort"];
            }
        }

        private string Grid2SortExp
        {
            set { ViewState["G2SortExp"] = value; }
            get
            {
                if (null == ViewState["G2SortExp"]) ViewState["G2SortExp"] = string.Empty;
                return (string)ViewState["G2SortExp"];
            }
        }

        #region GridView1 event handler
        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Button btnID = (Button)GridView1.Rows[e.RowIndex].Cells[0].Controls[0];
            LineID = int.Parse(btnID.Text);
            using (var dao = BusWebDataService.GetServiceInstance())
            {
                dao.DeleteLineByID(LineID);
                GridView1.DataSource = dao.GetLinesList();
                GridView1.DataBind();
            }
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if ("ID" == e.CommandName)
            {
                Button btnID = (Button)GridView1.Rows[int.Parse(e.CommandArgument.ToString())].Cells[0].Controls[0];
                LineID = int.Parse(btnID.Text);
                LineName = GridView1.Rows[int.Parse(e.CommandArgument.ToString())].Cells[1].Text;
                lblParent.Text = string.Format("這是[ID={0}][{1}]路線的站點清單：", LineID, LineName);
                using (var dao = BusWebDataService.GetServiceInstance())
                {
                    GridView2.DataSource = dao.GetStopsByLineID(LineID);
                    GridView2.DataBind();
                }
            }
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            Grid1Bind();
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            Grid1Bind();
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if ((e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate) && e.Row.RowIndex >= 0)
                ((Button)e.Row.Cells[5].Controls[0]).OnClientClick = "if (confirm('確定要刪除嗎?') == false) return false;";
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            using (DsBusWeb ds = new DsBusWeb())
            {
                var dr = ds.Lines.NewLinesRow();
                dr.LineID = int.Parse(((Button)GridView1.Rows[e.RowIndex].Cells[0].Controls[0]).Text);
                dr.LineName = ((TextBox)GridView1.Rows[e.RowIndex].Cells[1].Controls[0]).Text;
                using (var dao = BusWebDataService.GetServiceInstance())
                {
                    dao.UpdateLine(dr);
                }
            }
            GridView1.EditIndex = -1;
            Grid1Bind();
        }
        #endregion

        #region GridView2 event handler
        protected void GridView2_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            using (var dao = BusWebDataService.GetServiceInstance())
            {
                dao.DeleteStopByID(int.Parse(GridView2.Rows[e.RowIndex].Cells[0].Text));
                GridView2.DataSource = dao.GetStopsByLineID(LineID);
                GridView2.DataBind();
            }
        }

        protected void GridView2_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Longitude" || e.CommandName == "Latitude")
            {
                double latitude = double.Parse(((LinkButton)GridView2.Rows[int.Parse(e.CommandArgument.ToString())].Cells[2].Controls[0]).Text);
                double longitude = double.Parse(((LinkButton)GridView2.Rows[int.Parse(e.CommandArgument.ToString())].Cells[3].Controls[0]).Text);
                litMapPreview.Text = string.Format("<iframe width=\"425\" height=\"350\" frameborder=\"0\" scrolling=\"no\" marginheight=\"0\" marginwidth=\"0\" src=\"http://maps.google.com.tw/maps?hl=zh-TW&amp;ie=UTF8&amp;hq=&amp;hnear=110%E5%8F%B0%E5%8C%97%E5%B8%82%E4%BF%A1%E7%BE%A9%E5%8D%80%E5%9F%BA%E9%9A%86%E8%B7%AF%E4%BA%8C%E6%AE%B553%E8%99%9F&amp;t=m&amp;vpsrc=6&amp;brcurrent=3,0x3442abadec7543e7:0x5dbcdd8252aeabe7,1,0x3442ac6b61dbbd9d:0xc0c243da98cba64b&amp;ll={0},{1}&amp;spn=0.006805,0.00912&amp;z=16&amp;output=embed\"></iframe><br /><small><a href=\"http://maps.google.com.tw/maps?hl=zh-TW&amp;ie=UTF8&amp;hq=&amp;hnear=110%E5%8F%B0%E5%8C%97%E5%B8%82%E4%BF%A1%E7%BE%A9%E5%8D%80%E5%9F%BA%E9%9A%86%E8%B7%AF%E4%BA%8C%E6%AE%B553%E8%99%9F&amp;t=m&amp;vpsrc=6&amp;brcurrent=3,0x3442abadec7543e7:0x5dbcdd8252aeabe7,1,0x3442ac6b61dbbd9d:0xc0c243da98cba64b&amp;ll{0},{1}&amp;spn=0.006805,0.00912&amp;z=16&amp;source=embed\" style=\"color:#0000FF;text-align:left\">檢視較大的地圖</a></small>", latitude, longitude);
            }
            if (e.CommandName == "CreatorLongitude" || e.CommandName == "CreatorLatitude")
            {
                double latitude = double.Parse(((LinkButton)GridView2.Rows[int.Parse(e.CommandArgument.ToString())].Cells[4].Controls[0]).Text);
                double longitude = double.Parse(((LinkButton)GridView2.Rows[int.Parse(e.CommandArgument.ToString())].Cells[5].Controls[0]).Text);
                litCreatorMapPreview.Text = string.Format("<iframe width=\"425\" height=\"350\" frameborder=\"0\" scrolling=\"no\" marginheight=\"0\" marginwidth=\"0\" src=\"http://maps.google.com.tw/maps?hl=zh-TW&amp;ie=UTF8&amp;hq=&amp;hnear=110%E5%8F%B0%E5%8C%97%E5%B8%82%E4%BF%A1%E7%BE%A9%E5%8D%80%E5%9F%BA%E9%9A%86%E8%B7%AF%E4%BA%8C%E6%AE%B553%E8%99%9F&amp;t=m&amp;vpsrc=6&amp;brcurrent=3,0x3442abadec7543e7:0x5dbcdd8252aeabe7,1,0x3442ac6b61dbbd9d:0xc0c243da98cba64b&amp;ll={0},{1}&amp;spn=0.006805,0.00912&amp;z=16&amp;output=embed\"></iframe><br /><small><a href=\"http://maps.google.com.tw/maps?hl=zh-TW&amp;ie=UTF8&amp;hq=&amp;hnear=110%E5%8F%B0%E5%8C%97%E5%B8%82%E4%BF%A1%E7%BE%A9%E5%8D%80%E5%9F%BA%E9%9A%86%E8%B7%AF%E4%BA%8C%E6%AE%B553%E8%99%9F&amp;t=m&amp;vpsrc=6&amp;brcurrent=3,0x3442abadec7543e7:0x5dbcdd8252aeabe7,1,0x3442ac6b61dbbd9d:0xc0c243da98cba64b&amp;ll{0},{1}&amp;spn=0.006805,0.00912&amp;z=16&amp;source=embed\" style=\"color:#0000FF;text-align:left\">檢視較大的地圖</a></small>", latitude, longitude);
            }
        }

        protected void GridView2_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            using (DsBusWeb ds = new DsBusWeb())
            {
                var dr = ds.Stops.NewStopsRow();
                dr.StopID = int.Parse(((TextBox)GridView2.Rows[e.RowIndex].Cells[0].Controls[0]).Text);
                dr.StopName = ((TextBox)GridView2.Rows[e.RowIndex].Cells[1].Controls[0]).Text;
                using (var dao = BusWebDataService.GetServiceInstance())
                {
                    dao.UpdateStop(dr);
                }
            }
            GridView2.EditIndex = -1;
            Grid2Bind();
        }

        protected void GridView2_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView2.EditIndex = e.NewEditIndex;
            Grid2Bind();
        }

        protected void GridView2_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if ((e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate) && e.Row.RowIndex >= 0)
                ((Button)e.Row.Cells[11].Controls[0]).OnClientClick = "if (confirm('確定要刪除嗎?') == false) return false;";
        }

        protected void GridView2_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView2.EditIndex = -1;
            Grid2Bind();
        }
        #endregion

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            Grid1Bind();
        }

        protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        {
            Grid1SortDir = (SortDirection.Ascending == Grid1SortDir) ? SortDirection.Descending : SortDirection.Ascending;
            Grid1SortExp = e.SortExpression;
            Grid1Bind();
        }

        protected void GridView2_Sorting(object sender, GridViewSortEventArgs e)
        {
            Grid2SortDir = (SortDirection.Ascending == Grid2SortDir) ? SortDirection.Descending : SortDirection.Ascending;
            Grid2SortExp = e.SortExpression;
            Grid2Bind();
        }

        protected void btnQry_Click(object sender, EventArgs e)
        {
            GridView1.PageSize = int.Parse(txtPageSize.Text);
            GridView1.PageIndex = 0;
            Grid1Bind();
        }
    }
}
