using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Serialization;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;


namespace Bitboxx.DNNModules.BBStore
{
    public partial class ViewAdminStats : PortalModuleBase
    {
        #region Properties
        public BBStoreController Controller
        {
            get
            {
                if (_controller == null)
                    _controller = new BBStoreController();
                return _controller;
            }
        }

        private BBStoreController _controller;

        #endregion

        #region Event Handlers
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            string FileName = System.IO.Path.GetFileNameWithoutExtension(this.AppRelativeVirtualPath);
            if (this.ID != null)
                //this will fix it when its placed as a ChildUserControl 
                this.LocalResourceFile = this.LocalResourceFile.Replace(this.ID, FileName);
            else
                // this will fix it when its dynamically loaded using LoadControl method 
                this.LocalResourceFile = this.LocalResourceFile + FileName + ".ascx.resx";
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {

                if (!IsPostBack)
                {
                    DateTime minDate = new DateTime(1900, 01, 01);
                    DateTime maxDate = new DateTime(2100, 12, 31);
                    txtStartDate.Value = minDate.ToString("yyyy-MM-dd");
                    txtEndDate.Value = maxDate.ToString("yyyy-MM-dd");
                    lblStartDate2Value.Text = minDate.ToShortDateString();
                    lblEndDate2Value.Text = maxDate.ToShortDateString();
                    Localization.LocalizeGridView(ref grdStats, this.LocalResourceFile);
                    List<OrderStatsInfo> stats = Controller.GetOrderStats(minDate, maxDate);
                    grdStats.DataSource = stats;
                    grdStats.DataBind();
                }
            }
            catch (Exception exc)
            {
                //Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }


        #endregion

        protected void cmdRefresh_OnClick(object sender, EventArgs e)
        {
            DateTime minDate = DateTime.ParseExact(txtStartDate.Value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime maxDate = DateTime.ParseExact(txtEndDate.Value, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddHours(23).AddMinutes(59);
            List<OrderStatsInfo> stats = Controller.GetOrderStats(minDate, maxDate);
            grdStats.DataSource = stats;
            grdStats.DataBind();
        }

        protected void ddlTimeframe_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            DateTime minDate, maxDate;
            pnlShowDates.Visible = true;
            pnlSelectDates.Visible = false;
            switch (ddlTimeframe.SelectedValue)
            {
                case "0":
                    minDate = new DateTime(1900, 01, 01);
                    maxDate = new DateTime(2100, 12, 31);
                    break;
                case "1": // Today
                    minDate = DateTime.Today;
                    maxDate = minDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                    break;
                case "2": // Yesterday
                    minDate = DateTime.Today.AddDays(-1);
                    maxDate = minDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                    break;
                case "3": // This week
                    minDate = DateTime.Today.AddDays((-1) * (int)DateTime.Today.DayOfWeek);
                    maxDate = minDate.AddDays(6).AddHours(23).AddMinutes(59).AddSeconds(59);
                    break;
                case "4": // Last week
                    minDate = DateTime.Today.AddDays((-1) * (int)DateTime.Today.DayOfWeek - 7);
                    maxDate = minDate.AddDays(6).AddHours(23).AddMinutes(59).AddSeconds(59);
                    break;
                case "5": // This month
                    minDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    maxDate = minDate.AddMonths(1).AddSeconds(-1);
                    break;
                case "6": // Last month
                    minDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
                    maxDate = minDate.AddMonths(1).AddSeconds(-1);
                    break;
                case "7": // this year
                    minDate = new DateTime(DateTime.Today.Year, 1, 1);
                    maxDate = minDate.AddYears(1).AddSeconds(-1);
                    break;
                case "8": // Last year
                    minDate = new DateTime(DateTime.Today.Year-1, 1, 1);
                    maxDate = minDate.AddYears(1).AddSeconds(-1);
                    break;
                case "9": // See Textboxes
                    minDate = DateTime.ParseExact(txtStartDate.Value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    maxDate = DateTime.ParseExact(txtEndDate.Value, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddHours(23).AddMinutes(59);
                    pnlShowDates.Visible = false;
                    pnlSelectDates.Visible = true;
                    break;
                default:
                    minDate = new DateTime(1900, 01, 01);
                    maxDate = new DateTime(2100, 12, 31);
                    break;
            }
            txtStartDate.Value = minDate.ToString("yyyy-MM-dd");
            txtEndDate.Value = maxDate.ToString("yyyy-MM-dd");
            lblStartDate2Value.Text = minDate.ToShortDateString();
            lblEndDate2Value.Text = maxDate.ToShortDateString();
            List<OrderStatsInfo> stats = Controller.GetOrderStats(minDate, maxDate);
            grdStats.DataSource = stats;
            grdStats.DataBind();
        }
    }
}