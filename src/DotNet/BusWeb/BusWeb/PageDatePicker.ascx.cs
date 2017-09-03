using System;
using System.Web.UI;

namespace BusWeb
{
    public partial class PageDatePicker : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                for (int yr = DateTime.Today.Year - 5; yr < DateTime.Today.Year + 1; yr++) ddlYear.Items.Add(yr.ToString());
                for (int mn = 1; mn < 13; mn++) ddlMonth.Items.Add(mn.ToString());
                for (int dd = 1; dd < 31 + 1; dd++) ddlDay.Items.Add(dd.ToString());
            }
        }

        public DateTime SelectedDate
        {
            get { return new DateTime(int.Parse(ddlYear.SelectedValue), int.Parse(ddlMonth.SelectedValue), int.Parse(ddlDay.SelectedValue)); }
            set { ddlYear.SelectedValue = value.Year.ToString(); ddlMonth.SelectedValue = value.Month.ToString(); ddlDay.SelectedValue = value.Day.ToString(); }
        }
    }
}