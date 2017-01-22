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
using DotNetNuke.Entities.Modules.Actions;
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
	partial class ViewAdminProductList : PortalModuleBase, IActionable
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
		public ModuleActionCollection ModuleActions
		{
			get
			{
				ModuleActionCollection Actions = new ModuleActionCollection();
				//Actions.Add(GetNextActionID(), Localization.GetString(ModuleActionType.AddContent, this.LocalResourceFile),
				//   ModuleActionType.AddContent, "", "add.gif", EditUrl(), false, DotNetNuke.Security.SecurityAccessLevel.Edit,
				//    true, false);
				return Actions;
			}
		}

		public string ProductSort
		{
			get
			{
				if (ViewState["ProductSort"] != null)
					return ViewState["ProductSort"].ToString();
				else
					return "SimpleproductId";
			}
			set
			{
				ViewState["ProductSort"] = value;
			}
		}
		public string ProductFilter
		{
			get
			{
				string filter = "";

				if (txtProduct.Text != string.Empty)
                    filter = String.Format("(Name LIKE '%{0}%' OR ItemNo LIKE '%{0}%')", txtProduct.Text.Trim());
				return filter;
			}
		}
		public List<SimpleProductInfo> Products
		{
            //TODO: Hier unabhängig von User + extendedPrice Daten ermitteln !
            get { return Controller.GetSimpleProductsStandardPrice(PortalId, CurrentLanguage, ProductSort, ProductFilter); }
		}
		protected string CurrentLanguage
		{
			get
			{
				return System.Threading.Thread.CurrentThread.CurrentCulture.Name;
			}
		}
		protected string DefaultLanguage
		{
			get
			{
				return this.PortalSettings.DefaultLanguage;
			}
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
				    ddlProductPageSize.DataSource = sizes;
                    ddlProductPageSize.DataBind();
				    ddlProductPageSize.SelectedValue = defaultValue;

                    Localization.LocalizeGridView(ref grdProduct, this.LocalResourceFile);
					grdProduct.PageSize = Int16.Parse(ddlProductPageSize.SelectedValue);

					_pageIndex = 0;
					grdProduct.PageIndex = _pageIndex;
					grdProduct.DataSource = Products;
					grdProduct.DataBind();
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
			if (grdProduct.Rows.Count > 0)
			{
				foreach (GridViewRow row in grdProduct.Rows)
				{
					if (row.RowType == DataControlRowType.DataRow)
					{
						row.Attributes.Add("onclick",
						                   Page.ClientScript.GetPostBackEventReference(grdProduct, "Select$" + row.RowIndex, true));
					}
				}
			}
			base.Render(writer);
		}
		#endregion

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

		protected void grdProduct_PageIndexChanging(object sender, GridViewPageEventArgs e)
		{
			_pageIndex = (e.NewPageIndex < 0 ? 0 : e.NewPageIndex);
			grdProduct.PageIndex = _pageIndex;
			grdProduct.DataSource = Products;
			grdProduct.DataBind();
		}

		protected void grdProduct_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			try
			{
				switch (e.CommandName)
				{
					case "Select":
						int index = Convert.ToInt32(e.CommandArgument);
						int simpleProductId = (int)grdProduct.DataKeys[index].Value;
						_redirection = Globals.NavigateURL(TabId, Null.NullString, "adminmode=editproduct",
						                                   "productid=" + simpleProductId.ToString());
						//Response.Redirect(Globals.NavigateURL(TabId, Null.NullString, "adminmode=editproduct", "productid=" + simpleProductId.ToString()));
						break;

					case "Insert":
						_redirection = Globals.NavigateURL(TabId, Null.NullString, "adminmode=editproduct");
						//Response.Redirect(Globals.NavigateURL(TabId, Null.NullString, "adminmode=editproduct"));
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

		protected void grdProduct_Sorting(object sender, GridViewSortEventArgs e)
		{
			ProductSort = e.SortExpression;
			_pageIndex = 0;
			grdProduct.PageIndex = _pageIndex;
			grdProduct.DataSource = Products;
			grdProduct.DataBind();
		}

		protected void grdProduct_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';";
				//e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(grdProduct, "Select$" + e.Row.RowIndex);
				
			}
		}

		protected void grdProduct_DataBound(object sender, EventArgs e)
		{
			GridViewRow gvrPager = grdProduct.BottomPagerRow;

			if (gvrPager == null)
				return;

			// get your controls from the gridview
			DropDownList ddlPages = (DropDownList)gvrPager.Cells[0].FindControl("ddlProductPages");
			if (ddlPages != null)
			{
				// populate pager
				for (int i = 0; i < grdProduct.PageCount; i++)
				{
					int intPageNumber = i + 1;
					ListItem lstItem = new ListItem(intPageNumber.ToString());

					if (i == grdProduct.PageIndex)
						lstItem.Selected = true;

					ddlPages.Items.Add(lstItem);
				}

			}
			// populate page count
			Label lblPageCount = (Label)gvrPager.Cells[0].FindControl("lblPageCount");
			if (lblPageCount != null)
				lblPageCount.Text = grdProduct.PageCount.ToString();

			// populate item count
			Label lblItemCount = (Label)gvrPager.Cells[0].FindControl("lblItemCount");
			if (lblItemCount != null)
				lblItemCount.Text = String.Format("{0} items on {1} pages", _rowCount, grdProduct.PageCount);

		}

		protected void grdProduct_RowCreated(object sender, GridViewRowEventArgs e)
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

		protected void grdProduct_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{
			int simpleProductId = (int)grdProduct.DataKeys[e.RowIndex].Value;
			Controller.DeleteSimpleProduct(simpleProductId);
			grdProduct.DataSource = Products;
			grdProduct.DataBind();
		}

		protected void cmdFilter_Click(object sender, EventArgs e)
		{
			grdProduct.DataSource = Products;
			grdProduct.DataBind();
		}

		protected void ddlProductPageSize_SelectedIndexChanged(object sender, EventArgs e)
		{
			grdProduct.PageSize = Int16.Parse(ddlProductPageSize.SelectedValue);
			grdProduct.DataSource = Products;
			grdProduct.DataBind();
		}

		protected void ddlProductPages_SelectedIndexChanged(object sender, EventArgs e)
		{
			GridViewRow gvrPager = grdProduct.BottomPagerRow;
			DropDownList ddlPages = (DropDownList)gvrPager.Cells[0].FindControl("ddlProductPages");

			grdProduct.PageIndex = ddlPages.SelectedIndex;
			grdProduct.DataSource = Products;
			grdProduct.DataBind();
		}
	}
}