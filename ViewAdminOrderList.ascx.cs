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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
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
	partial class ViewAdminOrderList : PortalModuleBase, IActionable
	{
		#region Private Members
		private const string Currency = "EUR";
		BBStoreController _controller;
		private int _pageIndex;
		private int _rowCount;
		private List<OrderStateInfo> _orderStates;
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

		public string OrderSort
		{
			get
			{
				if (ViewState["OrderSort"] != null)
					return ViewState["OrderSort"].ToString();
				else
					return "OrderId";
			}
			set
			{
				ViewState["OrderSort"] = value;
			}
		}
		public string OrderFilter
		{
			get
			{
				string filter = "";

				if (txtOrder.Text != string.Empty)
					filter = "(Orders.OrderNo LIKE '%" + txtOrder.Text.Trim() + "%'" +
					         " OR OrderAddress.LastName LIKE '%" + txtOrder.Text.Trim() + "%'" +
					         " OR OrderAddress.FirstName LIKE '%" + txtOrder.Text.Trim() + "%'" +
					         " OR OrderAddress.Company LIKE '%" + txtOrder.Text.Trim() + "%'" +
					         " OR OrderAddress.City LIKE '%" + txtOrder.Text.Trim() + "%'" +
					         " OR OrderAddress.PostalCode LIKE '%" + txtOrder.Text.Trim() + "%'" +
					         " OR OrderAddress.CountryCode LIKE '%" + txtOrder.Text.Trim() + "%'" +
					         " OR OrderStateLang.OrderState LIKE '%" + txtOrder.Text.Trim() + "%')";

				return filter;
			}
		}
		public List<OrderListInfo> Orders
		{
			get { return Controller.GetOrders(PortalId, CurrentLanguage, OrderSort, OrderFilter); }
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
		public List<OrderStateInfo> OrderStates
		{
			get
			{
				if (_orderStates == null)
					_orderStates = Controller.GetOrderStates(PortalId,CurrentLanguage);
				return _orderStates;
			}
		}
		public int ItemsPerPage
		{
			get
			{
                if (Request.Cookies["ItemsPerPage_" + TabModuleId.ToString()] != null)
                    return Convert.ToInt32(Request.Cookies["ItemsPerPage_" + TabModuleId.ToString()].Value);
				else
				{
					if (Settings["ItemsPerPage"] != null)
					{
						string[] ppp = ((string)Settings["ItemsPerPage"]).Split(',');
						foreach (string s in ppp)
						{
							if (s.IndexOf('!') == 0)
								return Int32.Parse(s.Substring(1));
						}
						return Int32.Parse(ppp[0]);
					}
					else
						return 25;
				}
			}
			set
			{
                if (Request.Cookies["ItemsPerPage_" + TabModuleId.ToString()] != null)
                    Request.Cookies["ItemsPerPage_" + TabModuleId.ToString()].Value = value.ToString();
                else
                {
                    HttpCookie cookie = new HttpCookie("ItemsPerPage_" + TabModuleId.ToString());
                    cookie.Value = value.ToString();
                    cookie.Expires = DateTime.Now.AddDays(1);
                    Response.AppendCookie(cookie);
                }
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
                    ddlOrderPageSize.DataSource = sizes;
                    ddlOrderPageSize.DataBind();
                    ddlOrderPageSize.SelectedValue = defaultValue;

                    grdOrder.PageSize = Int16.Parse(ddlOrderPageSize.SelectedValue);
					Localization.LocalizeGridView(ref grdOrder, this.LocalResourceFile);

					_pageIndex = 0;
					grdOrder.PageIndex = _pageIndex;
					grdOrder.DataSource = Orders;
					grdOrder.DataBind();
				}
			}
			catch (Exception exc)
			{
				//Module failed to load 
				Exceptions.ProcessModuleLoadException(this, exc);
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (grdOrder.Rows.Count > 0)
			{
				foreach (GridViewRow row in grdOrder.Rows)
				{
					if (row.RowType == DataControlRowType.DataRow)
					{
						row.Attributes.Add("onclick",
						                   Page.ClientScript.GetPostBackEventReference(grdOrder, "Select$" + row.RowIndex, true));
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

		protected void grdOrder_PageIndexChanging(object sender, GridViewPageEventArgs e)
		{
			_pageIndex = (e.NewPageIndex < 0 ? 0 : e.NewPageIndex);
			grdOrder.PageIndex = _pageIndex;
			grdOrder.DataSource = Orders;
			grdOrder.DataBind();
		}

		protected void grdOrder_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			try
			{
				switch (e.CommandName)
				{
					case "Select":
						int index = Convert.ToInt32(e.CommandArgument);
						int orderId = (int)grdOrder.DataKeys[index].Value;
						Response.Redirect(Globals.NavigateURL(TabId, "", "adminmode=vieworder", "orderid=" + orderId.ToString()));
						break;

					//case "Insert":
					//    Response.Redirect(Globals.NavigateURL(TabId, "", "adminmode=editorder"));
					//    break;

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

		protected void grdOrder_Sorting(object sender, GridViewSortEventArgs e)
		{
			OrderSort = e.SortExpression;
			_pageIndex = 0;
			grdOrder.PageIndex = _pageIndex;
			grdOrder.DataSource = Orders;
			grdOrder.DataBind();
		}

		protected void grdOrder_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';";
				//e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(grdOrder, "Select$" + e.Row.RowIndex);
				
			}
		}

		protected void grdOrder_DataBound(object sender, EventArgs e)
		{
			GridViewRow gvrPager = grdOrder.BottomPagerRow;

			if (gvrPager == null)
				return;

			// get your controls from the gridview
			DropDownList ddlPages = (DropDownList)gvrPager.Cells[0].FindControl("ddlOrderPages");
			if (ddlPages != null)
			{
				// populate pager
				for (int i = 0; i < grdOrder.PageCount; i++)
				{
					int intPageNumber = i + 1;
					ListItem lstItem = new ListItem(intPageNumber.ToString());

					if (i == grdOrder.PageIndex)
						lstItem.Selected = true;

					ddlPages.Items.Add(lstItem);
				}

			}
			// populate page count
			Label lblPageCount = (Label)gvrPager.Cells[0].FindControl("lblPageCount");
			if (lblPageCount != null)
				lblPageCount.Text = grdOrder.PageCount.ToString();

			// populate item count
			Label lblItemCount = (Label)gvrPager.Cells[0].FindControl("lblItemCount");
			if (lblItemCount != null)
				lblItemCount.Text = String.Format("{0} items on {1} pages", _rowCount, grdOrder.PageCount);

		}

		protected void grdOrder_RowCreated(object sender, GridViewRowEventArgs e)
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

		protected void grdOrder_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{
			int orderId = (int)grdOrder.DataKeys[e.RowIndex].Value;
			//Controller.DeleteOrder(orderId);
			grdOrder.DataSource = Orders;
			grdOrder.DataBind();
		}

		protected void cmdFilter_Click(object sender, EventArgs e)
		{
			grdOrder.DataSource = Orders;
			grdOrder.DataBind();
		}

		protected void ddlOrderPageSize_SelectedIndexChanged(object sender, EventArgs e)
		{
			grdOrder.PageSize = Int16.Parse(ddlOrderPageSize.SelectedValue);
			grdOrder.DataSource = Orders;
			grdOrder.DataBind();
		}

		protected void ddlOrderPages_SelectedIndexChanged(object sender, EventArgs e)
		{
			GridViewRow gvrPager = grdOrder.BottomPagerRow;
			DropDownList ddlPages = (DropDownList)gvrPager.Cells[0].FindControl("ddlOrderPages");

			grdOrder.PageIndex = ddlPages.SelectedIndex;
			grdOrder.DataSource = Orders;
			grdOrder.DataBind();
		}
		public static string FormatAddress(object dataItem)
		{
			string company = (string) DataBinder.Eval(dataItem, "Company");
			string name = (string)DataBinder.Eval(dataItem, "FirstName") + " " + (string)DataBinder.Eval(dataItem, "LastName");
			string street = (string)DataBinder.Eval(dataItem, "Street");
			string countryCode = (string) DataBinder.Eval(dataItem, "CountryCode");
			string city = (string)DataBinder.Eval(dataItem, "PostalCode") + " " + (string)DataBinder.Eval(dataItem, "City");

			string result = company.Trim() + (string.IsNullOrEmpty(company) ? "" : " • ") + name.Trim() + "<br />" +
							street.Trim() + "<br />" +
			                countryCode.Trim() + (string.IsNullOrEmpty(countryCode) ? "" : " - ") + city.Trim();

			return result;
		}
	}
}