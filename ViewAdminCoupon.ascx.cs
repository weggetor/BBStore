// 
// DotNetNuke® - http://www.dotnetnuke.com 
// Copyright (c) 2002-2009 
// by DotNetNuke Corporation 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions: 
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software. 
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE. 
// 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

namespace Bitboxx.DNNModules.BBStore
{

	/// ----------------------------------------------------------------------------- 
	/// <summary> 
	/// The ViewBBStore class displays the content 
	/// </summary> 
	/// <remarks> 
	/// </remarks> 
	/// <history> 
	/// </history> 
	/// ----------------------------------------------------------------------------- 
	partial class ViewAdminCoupon : PortalModuleBase
	{
		#region Private Members
		private const string Currency = "EUR";
		BBStoreController _controller;
		private int _pageIndex;
		private int _rowCount;
		private string _redirection = "";
		#endregion

		#region Public Properties
		public BBStoreController Controller
		{
			get
			{
				if (_controller == null)
					_controller = new BBStoreController();
				return _controller;
			}
		}

		public string CouponSort
		{
			get
			{
				if (ViewState["CouponSort"] != null)
                    return ViewState["CouponSort"].ToString();
				else
					return "CouponId";
			}
            set { ViewState["CouponSort"] = value; }
		}
		
		public List<CouponInfo> Coupons
		{
			get { return Controller.GetCoupons(PortalId, CouponSort); }
		}
		protected string CurrentLanguage
		{
		    get { return System.Threading.Thread.CurrentThread.CurrentCulture.Name; }
		}
		protected string DefaultLanguage
		{
		    get { return this.PortalSettings.DefaultLanguage; }
		}

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
                    Hashtable storeSettings = Controller.GetStoreSettings(PortalId);
                    string[] sizes = ((string)(storeSettings["ItemsPerPage"] ?? "10,!25,50,100")).Split(',');
                    string defaultValue = sizes[0];
                    for (int i = 0; i < sizes.Length; i++)
                    {
                        string size = sizes[i];
                        if (size.StartsWith("!"))
                        {
                            defaultValue = size.Substring(1);
                            sizes[i] = defaultValue;
                            break;
                        }
                    }
                    ddlCouponPageSize.DataSource = sizes;
                    ddlCouponPageSize.DataBind();
                    ddlCouponPageSize.SelectedValue = defaultValue;
                    
                    Localization.LocalizeGridView(ref grdCoupon, this.LocalResourceFile);
                    grdCoupon.PageSize = Int16.Parse(ddlCouponPageSize.SelectedValue);

					_pageIndex = 0;
                    grdCoupon.PageIndex = _pageIndex;
                    grdCoupon.DataSource = Coupons;
                    grdCoupon.DataBind();
				}
			}
			catch (Exception exc)
			{
				//Module failed to load 
				Exceptions.ProcessModuleLoadException(this, exc);
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (!String.IsNullOrEmpty(_redirection))
				Response.Redirect(_redirection);
			else
				base.OnPreRender(e);
		}
		
		protected override void Render(HtmlTextWriter writer)
		{
            if (grdCoupon.Rows.Count > 0)
			{
                foreach (GridViewRow row in grdCoupon.Rows)
				{
					if (row.RowType == DataControlRowType.DataRow)
					{
						row.Attributes.Add("onclick",
                                           Page.ClientScript.GetPostBackEventReference(grdCoupon, "Select$" + row.RowIndex, true));
					}
				}
			}
			base.Render(writer);
		}
		#endregion


        protected void grdCoupon_PageIndexChanging(object sender, GridViewPageEventArgs e)
		{
			_pageIndex = (e.NewPageIndex < 0 ? 0 : e.NewPageIndex);
			grdCoupon.PageIndex = _pageIndex;
            grdCoupon.DataSource = Coupons;
            grdCoupon.DataBind();
		}

        protected void grdCoupon_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			try
			{
				switch (e.CommandName)
				{
					case "Select":
						int index = Convert.ToInt32(e.CommandArgument);
                        int couponId = (int)grdCoupon.DataKeys[index].Value;
						_redirection = Globals.NavigateURL(TabId, Null.NullString, "adminmode=editcoupon", "couponid=" + couponId.ToString());
						break;

					case "Insert":
						_redirection = Globals.NavigateURL(TabId, Null.NullString, "adminmode=editcoupon");
						break;

					default:
						break;
				}
			}
			catch (Exception exc)
			{
				//Module failed to load 
				Exceptions.ProcessModuleLoadException(this, exc);
			}

		}

        protected void grdCoupon_Sorting(object sender, GridViewSortEventArgs e)
		{
			CouponSort = e.SortExpression;
			_pageIndex = 0;
            grdCoupon.PageIndex = _pageIndex;
            grdCoupon.DataSource = Coupons;
            grdCoupon.DataBind();
		}

        protected void grdCoupon_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';";
				//e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(grdUnit, "Select$" + e.Row.RowIndex);
				
			}
		}

        protected void grdCoupon_DataBound(object sender, EventArgs e)
		{
            GridViewRow gvrPager = grdCoupon.BottomPagerRow;

			if (gvrPager == null)
				return;

			// get your controls from the gridview
			DropDownList ddlPages = (DropDownList)gvrPager.Cells[0].FindControl("ddlCouponPages");
			if (ddlPages != null)
			{
				// populate pager
                for (int i = 0; i < grdCoupon.PageCount; i++)
				{
					int intPageNumber = i + 1;
					ListItem lstItem = new ListItem(intPageNumber.ToString());

                    if (i == grdCoupon.PageIndex)
						lstItem.Selected = true;

					ddlPages.Items.Add(lstItem);
				}

			}
			// populate page count
			Label lblPageCount = (Label)gvrPager.Cells[0].FindControl("lblPageCount");
			if (lblPageCount != null)
                lblPageCount.Text = grdCoupon.PageCount.ToString();

			// populate item count
			Label lblItemCount = (Label)gvrPager.Cells[0].FindControl("lblItemCount");
			if (lblItemCount != null)
                lblItemCount.Text = String.Format("{0} items on {1} pages", _rowCount, grdCoupon.PageCount);

		}

        protected void grdCoupon_RowCreated(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.Footer)
			{
				int m = e.Row.Cells.Count;
				for (int i = m - 1; i >= 1; i--)
				{
					e.Row.Cells.RemoveAt(i);
				}
				e.Row.Cells[0].ColumnSpan = m;
			}
		}

        protected void grdCoupon_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{
            int couponId = (int)grdCoupon.DataKeys[e.RowIndex].Value;
			Controller.DeleteCoupon(couponId);
            grdCoupon.DataSource = Coupons;
            grdCoupon.DataBind();
		}

		protected void cmdFilter_Click(object sender, EventArgs e)
		{
            grdCoupon.DataSource = Coupons;
            grdCoupon.DataBind();
		}

		protected void ddlCouponPageSize_SelectedIndexChanged(object sender, EventArgs e)
		{
            grdCoupon.PageSize = Int16.Parse(ddlCouponPageSize.SelectedValue);
            grdCoupon.DataSource = Coupons;
            grdCoupon.DataBind();
		}

		protected void ddlCouponPages_SelectedIndexChanged(object sender, EventArgs e)
		{
            GridViewRow gvrPager = grdCoupon.BottomPagerRow;
			DropDownList ddlPages = (DropDownList)gvrPager.Cells[0].FindControl("ddlCouponPages");

            grdCoupon.PageIndex = ddlPages.SelectedIndex;
            grdCoupon.DataSource = Coupons;
            grdCoupon.DataBind();
		}

        #region Helper Methods
        private Control FindControlRecursive(Control rootControl, string controlId)
        {
            if (rootControl.ID == controlId)
                return rootControl;

            foreach (Control controlToSearch in rootControl.Controls)
            {
                Control controlToReturn = FindControlRecursive(controlToSearch, controlId);
                if (controlToReturn != null)
                    return controlToReturn;
            }
            return null;
        }
        #endregion
	}
}