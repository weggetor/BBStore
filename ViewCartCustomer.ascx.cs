using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;

namespace Bitboxx.DNNModules.BBStore
{
	public partial class ViewCartCustomer : PortalModuleBase
	{
		#region Private Members

		private BBStoreController _controller;
		private bool _inEditMode = false;
		private int selectedCustomer = -1;

		#endregion

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
		public bool InEditMode
		{
			get { return _inEditMode; }
			set
			{
				_inEditMode = value;
				pnlEdit.Visible = _inEditMode;
				pnlSearch.Visible = !_inEditMode;
				cmdNew.Visible = !_inEditMode;
				cmdSave.Visible = _inEditMode;
				cmdCancel.Visible = _inEditMode;
			}
		}
		public ViewCart MainControl { get; set; }
		#endregion

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			string fileName = System.IO.Path.GetFileNameWithoutExtension(this.AppRelativeVirtualPath);
			if (this.ID != null)
				//this will fix it when its placed as a ChildUserControl 
				this.LocalResourceFile = this.LocalResourceFile.Replace(this.ID, fileName);
			else
				// this will fix it when its dynamically loaded using LoadControl method 
				this.LocalResourceFile = this.LocalResourceFile + fileName + ".ascx.resx";
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Request.IsAuthenticated)
			{
				// Attention ! returnUrl must be relative path (cross-site-scripting denying)
				string returnUrl = HttpContext.Current.Request.RawUrl;
				returnUrl = HttpUtility.UrlEncode(returnUrl);

				Response.Redirect(Globals.NavigateURL(TabId, "", "action=login", "returnurl=" + returnUrl));
			}
			if (!IsPostBack)
			{
				Localization.LocalizeDataGrid(ref grdCustomers, this.LocalResourceFile);
				CartInfo cart = Controller.GetCart(PortalId, this.MainControl.CartId);
				selectedCustomer = this.MainControl.CustomerId;
				hidCustomerId.Value = selectedCustomer.ToString();
				BindData();
			}
		}
		
		protected void grdCustomers_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			int customerId = Convert.ToInt32(e.CommandArgument);
			
			switch (e.CommandName)
			{
				case "Edit":
					txtCustomerName.Text = Controller.GetCustomerById(customerId).CustomerName;
					hidCustomerId.Value = customerId.ToString();
					InEditMode = true;
					BindData();
					break;
				case "Delete":
					Controller.DeleteCustomer(customerId);
					InEditMode = false;
					BindData();
					break;
				case "Select":
					Controller.UpdateCartCustomerId(this.MainControl.CartId,customerId);
					Response.Redirect(Globals.NavigateURL(TabId,"","action=checkout"));
					break;
			}
		}

		protected void cmdNew_Click(object sender, EventArgs e)
		{
			txtCustomerName.Text = "";
			hidCustomerId.Value = "-1";
			InEditMode = true;
			BindData();
		}

		protected void cmdSave_Click(object sender, EventArgs e)
		{
			CustomerInfo customer = new CustomerInfo();
			customer.UserId = UserId;
			customer.PortalId = PortalId;
			customer.CustomerName = txtCustomerName.Text.Trim();
			customer.CustomerId = Convert.ToInt32(hidCustomerId.Value);
			Controller.SaveCustomer(customer);
			BindData();
			InEditMode = false;
			//EditCategoryFeeds ctrl = (EditCategoryFeeds)((Edit) MainControl).SubModules["EditCategoryFeeds"];
			//ctrl.FillDdlCategories();
		}

		protected void cmdCustomer_Click(object sender, EventArgs e)
		{

			int customerId = Convert.ToInt32(hidCustomerId.Value);
			if (customerId > -1)
				Controller.UpdateCartCustomerId(this.MainControl.CartId,customerId);
			Response.Redirect(Globals.NavigateURL(TabId,"","action=checkout"));
		}

		protected void cmdCancel_Click(object sender, EventArgs e)
		{
			InEditMode = false;
			BindData();
		}

		private void BindData()
		{
			List<CustomerInfo> allcustomers = Controller.GetCustomersByUserId(PortalId, UserId);
			grdCustomers.DataSource = allcustomers;
			grdCustomers.DataBind();
			for (int i = 0; i < allcustomers.Count; i++)
			{
				if (allcustomers[i].CustomerId == selectedCustomer)
				{
					grdCustomers.SelectedIndex = i;
					break;
				}
			}
		}

		protected void grdCustomers_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				ImageButton btn = e.Item.FindControl("cmdSelect") as ImageButton;
				if (((CustomerInfo)e.Item.DataItem).CustomerId == selectedCustomer)
					btn.ImageUrl = "images/checked.png";
				else
					btn.ImageUrl = "images/unchecked.png";
			}
		}
	}
}