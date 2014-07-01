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
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Serialization;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;

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
    [DNNtc.PackageProperties("BBStore Admin",6, "BBStore Admin", "BBStore Admin module", "BBStore.png", "Torsten Weggen", "bitboxx solutions", "http://www.bitboxx.net", "info@bitboxx.net",false)]
    [DNNtc.ModuleProperties("BBStore Admin", "BBStore Admin", 0)]
    [DNNtc.ModuleControlProperties("", "BBStore Admin", DNNtc.ControlType.View, "", false, false)]
	partial class ViewAdmin : PortalModuleBase, IActionable
	{
		#region Private Members

		private const string Currency = "EUR";
		BBStoreController _controller;

        private class BBStoreAdminButtonType
        {
            public string Name { get; set; }
            public string ImageUrl { get; set; }
            public ImageClickEventHandler ImgClickFunc { get; set; }
            public EventHandler LnkClickFunc { get; set; }
            public bool Enabled { get; set; }
        }
		
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
        public ModuleKindEnum ModuleKind
        {
            get { return ModuleKindEnum.Admin; }
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

        #region Interface IActionable
        public ModuleActionCollection ModuleActions
        {
            get
            {
                ModuleActionCollection actions = new ModuleActionCollection();

                actions.Add(GetNextActionID(), Localization.GetString("cmdSettings.Text", this.LocalResourceFile), ModuleActionType.AddContent, "", ControlPath +"Images/admin_settings_small.png", Globals.NavigateURL("Module", "ModuleId", ModuleId.ToString()), false, DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);
                actions.Add(GetNextActionID(), Localization.GetString("cmdMaintenance.Text", this.LocalResourceFile), ModuleActionType.AddContent, "", ControlPath + "Images/admin_maintenance_small.png", Globals.NavigateURL(TabId, "", "adminmode=maintenance"), false, DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);
                actions.Add(GetNextActionID(), Localization.GetString("cmdProduct.Text", this.LocalResourceFile), ModuleActionType.AddContent, "", ControlPath + "Images/admin_product_small.png", Globals.NavigateURL(TabId, "", "adminmode=productlist"), false, DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);
                actions.Add(GetNextActionID(), Localization.GetString("cmdShipping.Text", this.LocalResourceFile), ModuleActionType.AddContent, "", ControlPath + "Images/admin_shipping_small.png", Globals.NavigateURL(TabId, "", "adminmode=shipping"), false, DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);
                actions.Add(GetNextActionID(), Localization.GetString("cmdPayment.Text", this.LocalResourceFile), ModuleActionType.AddContent, "", ControlPath + "Images/admin_payment_small.png", Globals.NavigateURL(TabId, "", "adminmode=payment"), false, DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);
                actions.Add(GetNextActionID(), Localization.GetString("cmdProductGroup.Text", this.LocalResourceFile), ModuleActionType.AddContent, "", ControlPath + "Images/admin_productgroup_small.png", Globals.NavigateURL(TabId, "", "adminmode=productgroup"), false, DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);
                actions.Add(GetNextActionID(), Localization.GetString("cmdFeatureList.Text", this.LocalResourceFile), ModuleActionType.AddContent, "", ControlPath + "Images/admin_featurelist_small.png", Globals.NavigateURL(TabId, "", "adminmode=featurelist"), false, DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);
                actions.Add(GetNextActionID(), Localization.GetString("cmdUnits.Text", this.LocalResourceFile), ModuleActionType.AddContent, "", ControlPath + "Images/admin_units_small.png", Globals.NavigateURL(TabId, "", "adminmode=unitlist"), false, DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);
                actions.Add(GetNextActionID(), Localization.GetString("cmdOrder.Text", this.LocalResourceFile), ModuleActionType.AddContent, "", ControlPath + "Images/admin_order_small.png", Globals.NavigateURL(TabId, "", "adminmode=orderlist"), false, DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);

                return actions;
            }
        }
        #endregion

        #region Event Handlers
        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
			{
                string mode = String.IsNullOrEmpty(Request.QueryString["adminmode"]) ? "menu" : Request.QueryString["adminmode"];
				switch (mode)
				{
                    case "maintenance":
                        pnlMain.Visible = false;
						pnlPlaceholder.Visible = true;
						pnlBackLink.Visible = true;
						ViewAdminMaintenance adminMaintenanceControl = LoadControl(@"~\DesktopModules\BBStore\ViewAdminMaintenance.ascx") as ViewAdminMaintenance;
                        phContent.Controls.Add(adminMaintenanceControl);
						lblTitle.Text = Localization.GetString("TitleMaintenance.Text", this.LocalResourceFile);
						break;
                    
                    case "check":
                        pnlMain.Visible = false;
                        pnlPlaceholder.Visible = true;
                        pnlBackLink.Visible = true;

                        bool checkonly = (Convert.ToInt32(String.IsNullOrEmpty(Request.QueryString["checkonly"]) ? "1" : Request.QueryString["checkonly"]) == 1);
                        Literal ltr = new Literal();
                        ltr.Text = Controller.CheckStore(PortalId,checkonly).Replace("\r\n","<br/>");
                        phContent.Controls.Add(ltr);
				        break;
                    
                    case "menu":
                        GenMainMenu();
						pnlMain.Visible = true;
						pnlPlaceholder.Visible = false;
						pnlBackLink.Visible = false;
						lblTitle.Text = Localization.GetString("TitleMenu.Text", this.LocalResourceFile);
						break;

					case "productlist":
						pnlMain.Visible = false;
						pnlPlaceholder.Visible = true;
						pnlBackLink.Visible = true;
						ViewAdminProductList adminProductListControl = LoadControl(@"~\DesktopModules\BBStore\ViewAdminProductList.ascx") as ViewAdminProductList;
						phContent.Controls.Add(adminProductListControl);
						lblTitle.Text = Localization.GetString("TitleProductList.Text", this.LocalResourceFile);
						break;

					case "editproduct":
						pnlMain.Visible = false;
						pnlPlaceholder.Visible = true;
						pnlBackLink.Visible = false;
						EditProduct editProductControl = LoadControl(@"~\DesktopModules\BBStore\EditProduct.ascx") as EditProduct;
						phContent.Controls.Add(editProductControl);
						lblTitle.Text = Localization.GetString("TitleEditProduct.Text", this.LocalResourceFile);
						break;

                    case "unitlist":
                        pnlMain.Visible = false;
                        pnlPlaceholder.Visible = true;
                        pnlBackLink.Visible = true;
                        ViewAdminUnitList adminUnitListControl = LoadControl(@"~\DesktopModules\BBStore\ViewAdminUnitList.ascx") as ViewAdminUnitList;
                        phContent.Controls.Add(adminUnitListControl);
                        lblTitle.Text = Localization.GetString("TitleUnitList.Text", this.LocalResourceFile);
                        break;

                    case "editunit":
                        pnlMain.Visible = false;
                        pnlPlaceholder.Visible = true;
                        pnlBackLink.Visible = true;
                        EditUnit editUnitControl = LoadControl(@"~\DesktopModules\BBStore\EditUnit.ascx") as EditUnit;
                        phContent.Controls.Add(editUnitControl);
                        lblTitle.Text = Localization.GetString("TitleEditUnit.Text", this.LocalResourceFile);
                        break;

					case "productgroup":
						pnlMain.Visible = false;
						pnlPlaceholder.Visible = true;
						pnlBackLink.Visible = true;
						EditProductGroup adminProductGroupControl = LoadControl(@"~\DesktopModules\BBStore\EditProductGroup.ascx") as EditProductGroup;
						phContent.Controls.Add(adminProductGroupControl);
						lblTitle.Text = Localization.GetString("TitleProductGroup.Text", this.LocalResourceFile);
						break;

                    case "shipping":
                        pnlMain.Visible = false;
						pnlPlaceholder.Visible = true;
						pnlBackLink.Visible = true;
						ViewAdminShipping adminShippingControl = LoadControl(@"~\DesktopModules\BBStore\ViewAdminShipping.ascx") as ViewAdminShipping;
                        adminShippingControl.ModuleConfiguration = this.ModuleConfiguration;
                        phContent.Controls.Add(adminShippingControl);
						lblTitle.Text = Localization.GetString("TitleAdminShipping.Text", this.LocalResourceFile);
						break;

                    case "payment":
                        pnlMain.Visible = false;
                        pnlPlaceholder.Visible = true;
                        pnlBackLink.Visible = true;
                        ViewAdminPayment adminPaymentControl = LoadControl(@"~\DesktopModules\BBStore\ViewAdminPayment.ascx") as ViewAdminPayment;
                        adminPaymentControl.ModuleConfiguration = this.ModuleConfiguration;
                        phContent.Controls.Add(adminPaymentControl);
                        lblTitle.Text = Localization.GetString("TitleAdminPayment.Text", this.LocalResourceFile);
                        break;

					case "featurelist": 
						pnlMain.Visible = false;
						pnlPlaceholder.Visible = true;
						pnlBackLink.Visible = true;
						EditFeatureList adminFeatureListControl = LoadControl(@"~\DesktopModules\BBStore\EditFeatureList.ascx") as EditFeatureList;
						phContent.Controls.Add(adminFeatureListControl);
						lblTitle.Text = Localization.GetString("TitleFeatureList.Text", this.LocalResourceFile);
						break;
					case "featurelistitem":
						pnlMain.Visible = false;
						pnlPlaceholder.Visible = true;
						pnlBackLink.Visible = true;
						EditFeatureListItem adminFeatureListItemControl = LoadControl(@"~\DesktopModules\BBStore\EditFeatureListItem.ascx") as EditFeatureListItem;
						phContent.Controls.Add(adminFeatureListItemControl);
						lblTitle.Text = Localization.GetString("TitleFeatureListItem.Text", this.LocalResourceFile);
						break;
					case "orderlist":
						pnlMain.Visible = false;
						pnlPlaceholder.Visible = true;
						pnlBackLink.Visible = true;
						ViewAdminOrderList adminOrderListControl = LoadControl(@"~\DesktopModules\BBStore\ViewAdminOrderList.ascx") as ViewAdminOrderList;
						adminOrderListControl.ModuleConfiguration = this.ModuleConfiguration;
						phContent.Controls.Add(adminOrderListControl);
						lblTitle.Text = Localization.GetString("TitleOrderList.Text", this.LocalResourceFile);
						break;
					case "vieworder":
						pnlMain.Visible = false;
						pnlPlaceholder.Visible = true;
						pnlBackLink.Visible = false;
						ViewCart adminCartControl = LoadControl(@"~\DesktopModules\BBStore\ViewCart.ascx") as ViewCart;
						
						// Settings
						ModuleController objModules = new ModuleController();
						ModuleInfo cartModule = objModules.GetModuleByDefinition(PortalId, "BBStore Cart");
						adminCartControl.ModuleConfiguration = cartModule;

						// ResourceFile
						string FileName = System.IO.Path.GetFileNameWithoutExtension(adminCartControl.AppRelativeVirtualPath);
						adminCartControl.LocalResourceFile = adminCartControl.LocalResourceFile + FileName + ".ascx.resx";

						int orderId = Convert.ToInt32(Request.QueryString["orderid"]);
						OrderInfo order = Controller.GetOrder(orderId);
						
						List<OrderStateInfo> orderStates = Controller.GetOrderStates(PortalId,CurrentLanguage);
						DropDownList ddlOrderStates = new DropDownList();
						ddlOrderStates.DataSource = orderStates;
						ddlOrderStates.DataTextField = "OrderState";
						ddlOrderStates.DataValueField = "OrderStateId";
						ddlOrderStates.DataBind();
						ddlOrderStates.SelectedValue = order.OrderStateId.ToString();
						ddlOrderStates.SelectedIndexChanged += new EventHandler(ddlOrderStates_SelectedIndexChanged);
						ddlOrderStates.AutoPostBack = true;
						
						string orderHtml = adminCartControl.GenOrderHtml(order,false);
				        orderHtml = orderHtml.Replace("cid:Logo", PortalSettings.HomeDirectory+ PortalSettings.LogoFile);
				        orderHtml = "<div style=\"float:right\"><asp:Button id=\"cmdSendAgain\" runat=\"server\" /></div><div style=\"clear:right\"></div>" + orderHtml;
						orderHtml = orderHtml.Replace("[BBSTORE-ORDERSTATE]", "<asp:PlaceHolder id=\"phOrderState\" runat=\"server\" />");
						Control ctrl = ParseControl(orderHtml);
						PlaceHolder phOrderState = FindControlRecursive(ctrl, "phOrderState") as PlaceHolder;
						if (phOrderState != null)
							phOrderState.Controls.Add(ddlOrderStates);

				        Button cmdSendAgain = FindControlRecursive(ctrl,"cmdSendAgain") as Button;
				        if (cmdSendAgain != null)
				        {
				            cmdSendAgain.Text = LocalizeString("cmdSendAgain.Text");
                            cmdSendAgain.Click += cmdSendAgain_Click;
				        }
				        phContent.Controls.Add(ctrl);

						lblTitle.Text = Localization.GetString("TitleViewOrder.Text", this.LocalResourceFile);
						break;
				}
			}
			catch (Exception exc)
			{
				//Module failed to load 
				Exceptions.ProcessModuleLoadException(this, exc);
			}
        }

        void cmdSendAgain_Click(object sender, EventArgs e)
        {
            int orderId = Convert.ToInt32(Request.QueryString["orderid"]);
            ViewCart viewCartControl = LoadControl(@"~\DesktopModules\BBStore\ViewCart.ascx") as ViewCart;

            // Settings
            ModuleController oModules = new ModuleController();
            ModuleInfo mailCartModule = oModules.GetModuleByDefinition(PortalId, "BBStore Cart");
            viewCartControl.ModuleConfiguration = mailCartModule;

            // ResourceFile
            string resourceFile = System.IO.Path.GetFileNameWithoutExtension(viewCartControl.AppRelativeVirtualPath);
            viewCartControl.LocalResourceFile = viewCartControl.LocalResourceFile + resourceFile + ".ascx.resx";

            orderId = Convert.ToInt32(Request.QueryString["orderid"]);
            viewCartControl.MailOrder(orderId);

            string message = LocalizeString("SendAgain.Message");
            DotNetNuke.UI.Skins.Skin.AddModuleMessage(this,message, ModuleMessage.ModuleMessageType.BlueInfo);
        }

	    void ddlOrderStates_SelectedIndexChanged(object sender, EventArgs e)
		{
			DropDownList ddl = sender as DropDownList;
			int orderId = Convert.ToInt32(Request.QueryString["orderid"]);
			int orderStateId = Convert.ToInt32(ddl.SelectedValue);
			Controller.SetOrderState(orderId,orderStateId);
			Response.Redirect(Globals.NavigateURL(TabId, "", "adminmode=orderlist"));
		}

        protected void cmdSettings_Click(object sender, EventArgs e)
        {
            if (PortalSettings.UserMode != PortalSettings.Mode.Edit)
                DotNetNuke.Services.Personalization.Personalization.SetProfile("Usability", "UserMode" + (object)this.PortalSettings.PortalId, (object)"EDIT");
            
            Response.Redirect(Globals.NavigateURL(TabId, "", "ctl=Module", "ModuleId="+ ModuleId.ToString()));
        }
        protected void cmdMaintenance_Click(object sender, EventArgs e)
        {
            Response.Redirect(Globals.NavigateURL(TabId, "", "adminmode=maintenance"));
        }
        protected void cmdProduct_Click(object sender, EventArgs e)
        {
            Response.Redirect(Globals.NavigateURL(TabId, "", "adminmode=productlist"));
        }
        protected void cmdProductGroup_Click(object sender, EventArgs e)
        {
            Response.Redirect(Globals.NavigateURL(TabId, "", "adminmode=productgroup"));
        }
        protected void cmdShipping_Click(object sender, EventArgs e)
        {
            Response.Redirect(Globals.NavigateURL(TabId, "", "adminmode=shipping"));
        }
        protected void cmdPayment_Click(object sender, EventArgs e)
        {
            Response.Redirect(Globals.NavigateURL(TabId, "", "adminmode=payment"));
        }
        protected void cmdFeatureList_Click(object sender, EventArgs e)
        {
            Response.Redirect(Globals.NavigateURL(TabId, "", "adminmode=featurelist"));
        }
        protected void cmdOrder_Click(object sender, EventArgs e)
        {
            Response.Redirect(Globals.NavigateURL(TabId, "", "adminmode=orderlist"));
        }
        protected void cmdUnits_Click(object sender, EventArgs e)
        {
            Response.Redirect(Globals.NavigateURL(TabId, "", "adminmode=unitlist"));
        }
        protected void cmdMainMenu_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["adminmode"] != null && Request.QueryString["adminmode"] == "featurelistitem")
                Response.Redirect(Globals.NavigateURL(TabId, "", "adminmode=featurelist"));
            else
                Response.Redirect(Globals.NavigateURL(TabId));
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

	    private void GenMainMenu()
	    {
	        ModuleController moduleController = new ModuleController();
            ModuleInfo cartModule = moduleController.GetModuleByDefinition(PortalId, "BBStore Cart");
            
            List<BBStoreAdminButtonType> buttons = new List<BBStoreAdminButtonType>();
	        buttons.Add(new BBStoreAdminButtonType() { Name = "Settings", ImageUrl = "Images/admin_settings.png", ImgClickFunc = cmdSettings_Click, LnkClickFunc = cmdSettings_Click, Enabled = true});
	        buttons.Add(new BBStoreAdminButtonType() { Name = "Maintenance", ImageUrl = "Images/admin_maintenance.png", ImgClickFunc = cmdMaintenance_Click, LnkClickFunc = cmdMaintenance_Click, Enabled = true});
            buttons.Add(new BBStoreAdminButtonType() { Name = "Product", ImageUrl = "Images/admin_product.png", ImgClickFunc = cmdProduct_Click, LnkClickFunc = cmdProduct_Click, Enabled = true });
            buttons.Add(new BBStoreAdminButtonType() { Name = "Shipping", ImageUrl = "Images/admin_shipping.png", ImgClickFunc = cmdShipping_Click, LnkClickFunc = cmdShipping_Click, Enabled = true });
            buttons.Add(new BBStoreAdminButtonType() { Name = "Payment", ImageUrl = "Images/admin_payment.png", ImgClickFunc = cmdPayment_Click, LnkClickFunc = cmdPayment_Click, Enabled = true });
            buttons.Add(new BBStoreAdminButtonType() { Name = "ProductGroup", ImageUrl = "Images/admin_productgroup.png", ImgClickFunc = cmdProductGroup_Click, LnkClickFunc = cmdProductGroup_Click, Enabled = true });
            buttons.Add(new BBStoreAdminButtonType() { Name = "FeatureList", ImageUrl = "Images/admin_featurelist.png", ImgClickFunc = cmdFeatureList_Click, LnkClickFunc = cmdFeatureList_Click, Enabled = true });
            buttons.Add(new BBStoreAdminButtonType() { Name = "Units", ImageUrl = "Images/admin_units.png", ImgClickFunc = cmdUnits_Click, LnkClickFunc = cmdUnits_Click, Enabled = true });
            buttons.Add(new BBStoreAdminButtonType() { Name = "Order", ImageUrl = "Images/admin_order.png", ImgClickFunc = cmdOrder_Click, LnkClickFunc = cmdOrder_Click, Enabled = (cartModule != null)});

	        int i = 0;

	        HtmlGenericControl divRow = new HtmlGenericControl("div");
	        divRow.Style.Add("display", "table-row");

	        int rowLength = 4;

	        foreach (var button in buttons)
	        {
	            HtmlGenericControl divTopic = new HtmlGenericControl("div");
	            divTopic.Attributes["class"] = "bbTopic";

	            ImageButton img = new ImageButton();
	            img.ID = "img" + button.Name;
	            img.ImageUrl = button.ImageUrl;
	            img.Click += button.ImgClickFunc;
	            img.Enabled = button.Enabled;

	            LinkButton lnk = new LinkButton();
	            lnk.ID = "cmd" + button.Name;
	            lnk.Attributes.Add("ResourceKey", "cmd" + button.Name + ".Text");
	            lnk.Click += button.LnkClickFunc;
	            lnk.Enabled = button.Enabled;

	            divTopic.Controls.Add(img);
                divTopic.Controls.Add(new HtmlGenericControl("br"));
	            divTopic.Controls.Add(lnk);

	            divRow.Controls.Add(divTopic);
	            if (i%rowLength == rowLength-1 || i == buttons.Count - 1)
	            {
	                phButtons.Controls.Add(divRow);
	                divRow = new HtmlGenericControl("div");
	                divRow.Style.Add("display", "table-row");
	            }
	            i++;
	        }
	    }

	    #endregion
	}

}