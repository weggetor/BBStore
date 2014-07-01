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
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Bitboxx.License;
using Bitboxx.Web.GeneratedImage;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;

using Telerik.Web.UI;


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
    [DNNtc.PackageProperties("BBStore FeatureList", 8, "BBStore FeatureList", "BBStore FeatureList", "BBStore.png", "Torsten Weggen", "bitboxx solutions", "http://www.bitboxx.net", "info@bitboxx.net", false)]
    [DNNtc.ModuleProperties("BBStore FeatureList", "BBStore FeatureList", 0)]
    [DNNtc.ModuleControlProperties("", "BBStore FeatureList", DNNtc.ControlType.View, "", true, false)]
    partial class ViewFeatureList : PortalModuleBase, IActionable
    {
		#region Fields
        private const string Currency = "EUR";
        BBStoreController _controller;
        private string _template = "";
		#endregion

		#region Properties
        public Guid FilterSessionId
        {
            get
            {
                string _filterSessionId;
                if (Request.Cookies["BBStoreFilterSessionId_"+ PortalId.ToString()] != null)
                    _filterSessionId = (string)(Request.Cookies["BBStoreFilterSessionId_" + PortalId.ToString()].Value);
                else
                {
                    _filterSessionId = Guid.NewGuid().ToString();
                    HttpCookie keks = new HttpCookie("BBStoreFilterSessionId_" + PortalId.ToString());
                    keks.Value = _filterSessionId;
                    keks.Expires = DateTime.Now.AddDays(1);
                    Response.AppendCookie(keks);
                }
                return new Guid(_filterSessionId);
            }
        }
		public BBStoreController Controller
        {
            get
            {
                if (_controller == null)
                    _controller = new BBStoreController();
                return _controller;
            }
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
		protected bool IsAdmin
		{
			get
			{
				UserInfo user = UserController.GetCurrentUserInfo();
				return (user.IsInRole("Administrators") && IsEditable);
			}
		}
		protected bool IsConfigured
		{
			get
			{
				return (Settings["FeatureListId"] != null && FeatureListId > -1);
			}
		}
		public int FeaturesInRow
		{
			get
			{
				if (Settings["FeaturesInRow"] != null)
					return Convert.ToInt32(Settings["FeaturesInRow"]);
				else
					return 1;
			}
		}
		public int FeatureListId
		{
			get
			{
				if (Settings["FeatureListId"] != null)
					return Convert.ToInt32(Settings["FeatureListId"]);
				else
					return -1;
			}
		}
    	public int ViewMode
    	{
    		get
    		{
				if (Settings["ViewMode"] != null)
					return Convert.ToInt32(Settings["ViewMode"]);
    			return 0;
    		}
    	}

		public int RotatorHeight
		{
			get
			{
				if (Settings["RotatorHeight"] != null)
					return Convert.ToInt32(Settings["RotatorHeight"]);
				return 100;
			}
		}
    	public bool OnlyWithImage
    	{
			get
			{
				if (Settings["OnlyWithImage"] != null)
					return Convert.ToBoolean(Settings["OnlyWithImage"]);
				return false;
			}
    	}
        public ModuleKindEnum ModuleKind
        {
            get { return ModuleKindEnum.FeatureList; }
        }
		protected string Template
		{
			get
			{
			    if (_template == String.Empty && Settings["Template"] != null)
			    {
			        TemplateControl tp = LoadControl("controls/TemplateControl.ascx") as TemplateControl;
			        tp.Key = "FeatureList";
			        _template = tp.GetTemplate((string) Settings["Template"]);
			    }
			    return _template;
			}
		}

    	protected List<FeatureListItemInfo>Features
    	{
    		get
    		{
				if (ViewState["Features"] != null)
					return (List<FeatureListItemInfo>) ViewState["Features"];
				return new List<FeatureListItemInfo>();
    		}
			set { ViewState["Features"] = value; }

    	}
		#endregion

        #region "Optional Interfaces"

        /// ----------------------------------------------------------------------------- 
        /// <summary> 
        /// Registers the module actions required for interfacing with the portal framework 
        /// </summary> 
        /// <value></value> 
        /// <returns></returns> 
        /// <remarks></remarks> 
        /// <history> 
        /// </history> 
        /// ----------------------------------------------------------------------------- 
        public ModuleActionCollection ModuleActions
        {
            get
            {
                ModuleActionCollection actions = new ModuleActionCollection();
                
                ModuleController moduleController = new ModuleController();
                ModuleInfo adminModule = moduleController.GetModuleByDefinition(PortalId, "BBStore Admin");
                if (adminModule != null)
                {
                    string url = Globals.NavigateURL(adminModule.TabID, "", "adminmode=featurelist","featurelist=-1");
                    actions.Add(GetNextActionID(), Localization.GetString("cmdNew.Text", this.LocalResourceFile), ModuleActionType.AddContent, "", "/images/icon_unknown_16px.gif", url, false, DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);
                    url = Globals.NavigateURL(adminModule.TabID, "", "adminmode=featurelist", "featurelist=" + FeatureListId.ToString());
                    actions.Add(GetNextActionID(), Localization.GetString("cmdEdit.Text", this.LocalResourceFile), ModuleActionType.EditContent, "", "edit.gif", url, false, DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);
                }
               
                return actions;
            }
        }
        #endregion

		#region Event Handlers
        protected void Page_Init(object sender, System.EventArgs e)
        {
            if (IsConfigured)
            {
				lstFeatures.GroupItemCount = FeaturesInRow;
            }
            else
            {
                string message = Localization.GetString("Configure.Message", this.LocalResourceFile);
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, message, ModuleMessage.ModuleMessageType.YellowWarning);
            }
        }
        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {

                if (IsConfigured)
				{
					Features = Controller.GetFeatureListItemsByListId(FeatureListId, CurrentLanguage, OnlyWithImage);
					
					if (ViewMode == 0)
					{
						lstFeatures.DataSource = Features;
						lstFeatures.DataBind();
						pnlList.Visible = true;
						pnlRotator.Visible = false;
					}
					else
					{
						rotFeatures.DataSource = Features;
						rotFeatures.DataBind();
						pnlList.Visible = false;
						pnlRotator.Visible = true;
					}
					if (Settings["HeaderText"] != null)
						ltrHead.Text = (string)Settings["HeaderText"];
					if (Settings["FooterText"] != null)
						ltrFoot.Text = (string)Settings["FooterText"];

					rotFeatures.Height = new Unit(RotatorHeight);
				}
            }
            catch (Exception exc)
            {
                //Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {
            // Check licensing
            LicenseDataInfo license = Controller.GetLicense(PortalId, false);
            Controller.CheckLicense(license, this, ModuleKind);
        }

		protected void lstFeatures_ItemCreated(object sender, ListViewItemEventArgs e)
		{

			if (IsConfigured)
			{
				string template;
				int imageWidth = 200;
				ListView lv = sender as ListView;
				ListViewDataItem item = e.Item as ListViewDataItem;
				FeatureListItemInfo FeatureListItem = item.DataItem as FeatureListItemInfo;
				if (FeatureListItem != null)
				{
					PlaceHolder ph = e.Item.FindControl("featurePlaceHolder") as PlaceHolder;

					template = Template;
					template = template.Replace("[TITLE]", "<asp:Label ID=\"lblTitle\" runat=\"server\" />");
					if (template.IndexOf("[IMAGE") > -1)
					{
						if (template.IndexOf("[IMAGE:") > -1)
						{
							string width = template.Substring(template.IndexOf("[IMAGE:") + 7);
							width = width.Substring(0, width.IndexOf("]"));
							if (Int32.TryParse(width, out imageWidth) == false)
								imageWidth = 200;
							template = template.Replace("[IMAGE:" + width + "]", "<asp:PlaceHolder ID=\"phimgFeature\" runat=\"server\" />");
						}
						else
							template = template.Replace("[IMAGE]", "<asp:PlaceHolder ID=\"phimgFeature\" runat=\"server\" />");
					}
					Control ctrl = ParseControl(template);
					Label lblTitle = FindControlRecursive(ctrl, "lblTitle") as Label;
					if (lblTitle != null)
						lblTitle.Text = FeatureListItem.FeatureListItem;

					PlaceHolder phimgFeature = FindControlRecursive(ctrl, "phimgFeature") as PlaceHolder;
					if (phimgFeature != null && FeatureListItem.Image != null)
					{

						string fileName =
							PortalSettings.HomeDirectoryMapPath.Replace(HttpContext.Current.Request.PhysicalApplicationPath, "") +
							FeatureListItem.Image.Replace('/', '\\');

						GeneratedImage imgFeature = new GeneratedImage();
						imgFeature.ImageHandlerUrl = "~/BBImageHandler.ashx";
						if (imageWidth > 0)
							imgFeature.Parameters.Add(new ImageParameter() { Name = "Width", Value = imageWidth.ToString() });
						imgFeature.Parameters.Add(new ImageParameter() { Name = "File", Value = fileName });
						// TODO: Watermark
						//if (false)
						//{
						//    imgProduct.Parameters.Add(new ImageParameter() { Name = "WatermarkText", Value = Localization.GetString("Sold.Text", this.LocalResourceFile) });
						//    imgProduct.Parameters.Add(new ImageParameter() { Name = "WatermarkFontFamily", Value = "Verdana" });
						//    imgProduct.Parameters.Add(new ImageParameter() { Name = "WatermarkFontColor", Value = "Red" });
						//    imgProduct.Parameters.Add(new ImageParameter() { Name = "WatermarkFontSize", Value = "20" });
						//}
						phimgFeature.Controls.Add(imgFeature);
					}

					ph.Controls.Add(ctrl);
				}
			}
			else
			{
				lstFeatures.Visible = false;
			}
		}
		protected void lstFeatures_SelectedIndexChanging(object sender, ListViewSelectEventArgs e)
		{
			int TabId = Convert.ToInt32(Settings["ProductListModulePage"]);
			int featureListItemId = (int)lstFeatures.DataKeys[e.NewSelectedIndex].Value;
			Controller.DeleteProductFilter(PortalId, FilterSessionId, "FeatureList");

			ProductFilterInfo pf = new ProductFilterInfo();
			pf.FilterSessionId = FilterSessionId;
			pf.FilterSource = "FeatureList";
			pf.FilterValue = FeatureListId.ToString() + "|" + featureListItemId.ToString();
			pf.PortalId = PortalId;
			Controller.NewProductFilter(pf);

			Response.Redirect(Globals.NavigateURL(TabId));
		}

		protected void rotFeatures_DataBound(object sender, RadRotatorEventArgs e)
		{
			if (IsConfigured)
			{
				string template;
				int imageWidth = 200;
				RadRotator rot = sender as RadRotator;
				RadRotatorItem item = e.Item as RadRotatorItem;
				FeatureListItemInfo featureListItem = item.DataItem as FeatureListItemInfo;
				if (featureListItem != null)
				{
					PlaceHolder ph = e.Item.FindControl("featurePlaceHolder") as PlaceHolder;

					template = Template;
					template = template.Replace("[TITLE]", "<asp:Label ID=\"lblTitle\" runat=\"server\" />");
					if (template.IndexOf("[IMAGE") > -1)
					{
						if (template.IndexOf("[IMAGE:") > -1)
						{
							string width = template.Substring(template.IndexOf("[IMAGE:") + 7);
							width = width.Substring(0, width.IndexOf("]"));
							if (Int32.TryParse(width, out imageWidth) == false)
								imageWidth = 200;
							template = template.Replace("[IMAGE:" + width + "]", "<asp:PlaceHolder ID=\"phimgFeature\" runat=\"server\" />");
						}
						else
							template = template.Replace("[IMAGE]", "<asp:PlaceHolder ID=\"phimgFeature\" runat=\"server\" />");
					}
					Control ctrl = ParseControl(template);
					Label lblTitle = FindControlRecursive(ctrl, "lblTitle") as Label;
					if (lblTitle != null)
						lblTitle.Text = featureListItem.FeatureListItem;

					PlaceHolder phimgFeature = FindControlRecursive(ctrl, "phimgFeature") as PlaceHolder;
					if (phimgFeature != null && featureListItem.Image != null)
					{

						string fileName =
							PortalSettings.HomeDirectoryMapPath.Replace(HttpContext.Current.Request.PhysicalApplicationPath, "") +
							featureListItem.Image.Replace('/', '\\');

						GeneratedImage imgFeature = new GeneratedImage();
						imgFeature.ImageHandlerUrl = "~/BBImageHandler.ashx";
						if (imageWidth > 0)
							imgFeature.Parameters.Add(new ImageParameter() { Name = "Width", Value = imageWidth.ToString() });
						imgFeature.Parameters.Add(new ImageParameter() { Name = "File", Value = fileName });
						// TODO: Watermark
						//if (false)
						//{
						//    imgProduct.Parameters.Add(new ImageParameter() { Name = "WatermarkText", Value = Localization.GetString("Sold.Text", this.LocalResourceFile) });
						//    imgProduct.Parameters.Add(new ImageParameter() { Name = "WatermarkFontFamily", Value = "Verdana" });
						//    imgProduct.Parameters.Add(new ImageParameter() { Name = "WatermarkFontColor", Value = "Red" });
						//    imgProduct.Parameters.Add(new ImageParameter() { Name = "WatermarkFontSize", Value = "20" });
						//}
						phimgFeature.Controls.Add(imgFeature);
					}

					ph.Controls.Add(ctrl);
				}
			}
			else
			{
				rotFeatures.Visible = false;
			}
		}

		public void rotFeatures_ItemClick(object sender, RadRotatorEventArgs e)
		{
			FeatureListItemInfo feature = Features[e.Item.Index];

			int TabId = Convert.ToInt32(Settings["ProductListModulePage"]);
			int featureListItemId = feature.FeatureListItemId;
			Controller.DeleteProductFilter(PortalId, FilterSessionId, "FeatureList");

			ProductFilterInfo pf = new ProductFilterInfo();
			pf.FilterSessionId = FilterSessionId;
			pf.FilterSource = "FeatureList";
			pf.FilterValue = FeatureListId.ToString() + "|" + featureListItemId.ToString();
			pf.PortalId = PortalId;
			Controller.NewProductFilter(pf);

			Response.Redirect(Globals.NavigateURL(TabId));
		}
    	#endregion

        #region Helper Methods
        private Control FindControlRecursive(Control rootControl, string controlID)
        {
            if (rootControl.ID == controlID)
                return rootControl;

            foreach (Control controlToSearch in rootControl.Controls)
            {
                Control controlToReturn = FindControlRecursive(controlToSearch, controlID);
                if (controlToReturn != null)
                    return controlToReturn;
            }
            return null;
        }
		protected string GetWidth()
		{
			return (100 / lstFeatures.GroupItemCount).ToString();
		}
        #endregion


     }
}