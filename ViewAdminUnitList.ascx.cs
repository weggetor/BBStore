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
	partial class ViewAdminUnitList : PortalModuleBase
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

		public string UnitSort
		{
			get
			{
				if (ViewState["UnitSort"] != null)
					return ViewState["UnitSort"].ToString();
				else
					return "UnitId";
			}
		    set { ViewState["UnitSort"] = value; }
		}
		
		public List<UnitInfo> Units
		{
			get { return Controller.GetUnits(PortalId, CurrentLanguage, UnitSort); }
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
                    ddlUnitPageSize.DataSource = sizes;
                    ddlUnitPageSize.DataBind();
                    ddlUnitPageSize.SelectedValue = defaultValue;
                    
                    Localization.LocalizeGridView(ref grdUnit, this.LocalResourceFile);
					grdUnit.PageSize = Int16.Parse(ddlUnitPageSize.SelectedValue);

					_pageIndex = 0;
					grdUnit.PageIndex = _pageIndex;
					grdUnit.DataSource = Units;
					grdUnit.DataBind();
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
			if (grdUnit.Rows.Count > 0)
			{
				foreach (GridViewRow row in grdUnit.Rows)
				{
					if (row.RowType == DataControlRowType.DataRow)
					{
						row.Attributes.Add("onclick",
						                   Page.ClientScript.GetPostBackEventReference(grdUnit, "Select$" + row.RowIndex, true));
					}
				}
			}
			base.Render(writer);
		}
		#endregion


		protected void grdUnit_PageIndexChanging(object sender, GridViewPageEventArgs e)
		{
			_pageIndex = (e.NewPageIndex < 0 ? 0 : e.NewPageIndex);
			grdUnit.PageIndex = _pageIndex;
            grdUnit.DataSource = Units;
            grdUnit.DataBind();
		}

        protected void grdUnit_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			try
			{
				switch (e.CommandName)
				{
					case "Select":
						int index = Convert.ToInt32(e.CommandArgument);
						int unitId = (int)grdUnit.DataKeys[index].Value;
						_redirection = Globals.NavigateURL(TabId, Null.NullString, "adminmode=editunit", "unitid=" + unitId.ToString());
						break;

					case "Insert":
						_redirection = Globals.NavigateURL(TabId, Null.NullString, "adminmode=editunit");
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

        protected void grdUnit_Sorting(object sender, GridViewSortEventArgs e)
		{
			UnitSort = e.SortExpression;
			_pageIndex = 0;
			grdUnit.PageIndex = _pageIndex;
			grdUnit.DataSource = Units;
			grdUnit.DataBind();
		}

        protected void grdUnit_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';";
				//e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(grdUnit, "Select$" + e.Row.RowIndex);
				
			}
		}

        protected void grdUnit_DataBound(object sender, EventArgs e)
		{
			GridViewRow gvrPager = grdUnit.BottomPagerRow;

			if (gvrPager == null)
				return;

			// get your controls from the gridview
			DropDownList ddlPages = (DropDownList)gvrPager.Cells[0].FindControl("ddlProductPages");
			if (ddlPages != null)
			{
				// populate pager
				for (int i = 0; i < grdUnit.PageCount; i++)
				{
					int intPageNumber = i + 1;
					ListItem lstItem = new ListItem(intPageNumber.ToString());

					if (i == grdUnit.PageIndex)
						lstItem.Selected = true;

					ddlPages.Items.Add(lstItem);
				}

			}
			// populate page count
			Label lblPageCount = (Label)gvrPager.Cells[0].FindControl("lblPageCount");
			if (lblPageCount != null)
				lblPageCount.Text = grdUnit.PageCount.ToString();

			// populate item count
			Label lblItemCount = (Label)gvrPager.Cells[0].FindControl("lblItemCount");
			if (lblItemCount != null)
				lblItemCount.Text = String.Format("{0} items on {1} pages", _rowCount, grdUnit.PageCount);

		}

        protected void grdUnit_RowCreated(object sender, GridViewRowEventArgs e)
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

		protected void grdUnit_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{
			int simpleProductId = (int)grdUnit.DataKeys[e.RowIndex].Value;
			Controller.DeleteSimpleProduct(simpleProductId);
			grdUnit.DataSource = Units;
			grdUnit.DataBind();
		}

		protected void cmdFilter_Click(object sender, EventArgs e)
		{
			grdUnit.DataSource = Units;
			grdUnit.DataBind();
		}

		protected void ddlUnitPageSize_SelectedIndexChanged(object sender, EventArgs e)
		{
			grdUnit.PageSize = Int16.Parse(ddlUnitPageSize.SelectedValue);
			grdUnit.DataSource = Units;
			grdUnit.DataBind();
		}

		protected void ddlUnitPages_SelectedIndexChanged(object sender, EventArgs e)
		{
			GridViewRow gvrPager = grdUnit.BottomPagerRow;
			DropDownList ddlPages = (DropDownList)gvrPager.Cells[0].FindControl("ddlUnitPages");

			grdUnit.PageIndex = ddlPages.SelectedIndex;
			grdUnit.DataSource = Units;
			grdUnit.DataBind();
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