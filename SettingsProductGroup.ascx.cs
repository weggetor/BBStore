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
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using System.Threading;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Text;
using System.Web.UI;
using System.Web;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;

namespace Bitboxx.DNNModules.BBStore
{

    /// ----------------------------------------------------------------------------- 
    /// <summary> 
    /// The Settings class manages Module Settings 
    /// </summary> 
    /// <remarks> 
    /// </remarks> 
    /// <history> 
    /// </history> 
    /// ----------------------------------------------------------------------------- 
    [DNNtc.PackageProperties("BBStore Product Groups")]
    [DNNtc.ModuleProperties("BBStore Product Groups")]
    [DNNtc.ModuleControlProperties("Settings", "BBStore Product Group Settings", DNNtc.ControlType.Edit, "", true, false)]
    partial class SettingsProductGroup : ModuleSettingsBase
    {
        protected global::DotNetNuke.UI.UserControls.UrlControl AllGroupsImageSelector;

        private BBStoreController Controller;
		protected string CurrentLanguage
		{
			get
			{
				return System.Threading.Thread.CurrentThread.CurrentCulture.Name;
			}
		}

        #region "Base Method Implementations"
        protected override void OnInit(EventArgs e)
        {
            Controller = new BBStoreController();
            if (!IsPostBack)
            {
                // TODO: Sort Combobox
				List<ProductGroupInfo> pgl = Controller.GetProductGroups(PortalId, CurrentLanguage, true);
				cboRootLevel.Items.Add(new ListItem(Localization.GetString("SelectOption.Text", this.LocalResourceFile),"-1"));
				cboRootLevel.AppendDataBoundItems = true;
				cboRootLevel.DataSource = pgl;
				cboRootLevel.DataTextField = "ProductGroupName";
				cboRootLevel.DataValueField = "ProductGroupId";
				cboRootLevel.DataBind();

                cboDefaultProductGroup.Items.Add(new ListItem(Localization.GetString("SelectOption.Text", this.LocalResourceFile), "-1"));
                cboDefaultProductGroup.AppendDataBoundItems = true;
                cboDefaultProductGroup.DataSource = pgl;
                cboDefaultProductGroup.DataTextField = "ProductGroupName";
                cboDefaultProductGroup.DataValueField = "ProductGroupId";
                cboDefaultProductGroup.DataBind();

            }
            tplTemplate.CreateImageCallback = CreateThumbHtml;
            base.OnLoad(e);
        }
        public override void LoadSettings()
        {
            try
            {
				if (!IsPostBack)
                {

                    if (ModuleSettings["ProductGroupsInRow"] != null)
                        txtProductGroupsInRow.Text = (string)ModuleSettings["ProductGroupsInRow"];
                    else
                        txtProductGroupsInRow.Text = "3";

                   if (ModuleSettings["Template"] != null ) 
                        tplTemplate.Value = (string)ModuleSettings["Template"];

                    if (ModuleSettings["DynamicPage"] != null)
                        urlSelectDynamicPage.Url = (string)ModuleSettings["DynamicPage"];
                    else
                        urlSelectDynamicPage.Url = TabId.ToString();

                    if (ModuleSettings["ViewMode"] != null)
                        rblViewMode.SelectedValue = (string)ModuleSettings["ViewMode"];
                    else
                        rblViewMode.SelectedValue = "0";
                    rblViewMode_SelectedIndexChanged(this, new EventArgs());

                    if (ModuleSettings["IncludeChilds"] != null)
                        chkIncludeChilds.Checked = Convert.ToBoolean(ModuleSettings["IncludeChilds"]);
					
					if (ModuleSettings["ShowProductCount"] != null)
						chkShowProductCount.Checked = Convert.ToBoolean(ModuleSettings["ShowProductCount"]);
                    if (ModuleSettings["SetTitle"] != null)
                        chkSetTitle.Checked = Convert.ToBoolean(ModuleSettings["SetTitle"]);
                    if (ModuleSettings["ShowBreadcrumb"] != null)
                        chkShowBreadcrumb.Checked = Convert.ToBoolean(ModuleSettings["ShowBreadcrumb"]);
                    if (ModuleSettings["ShowExpandCollapse"] != null)
                        chkShowExpandCollapse.Checked = Convert.ToBoolean(ModuleSettings["ShowExpandCollapse"]);
					if (ModuleSettings["WrapNode"] != null)
						chkWrapNode.Checked = Convert.ToBoolean(ModuleSettings["WrapNode"]);
                    if (ModuleSettings["ShowIcons"] != null)
                        chkShowIcons.Checked = Convert.ToBoolean(ModuleSettings["ShowIcons"]);

					if (ModuleSettings["RootLevel"] != null)
						cboRootLevel.SelectedValue = (string)ModuleSettings["RootLevel"];

					if (ModuleSettings["RootLevelFixed"] != null)
						chkRootLevelFixed.Checked = Convert.ToBoolean(Settings["RootLevelFixed"]);

                    if (ModuleSettings["DefaultProductGroup"] != null)
                        cboDefaultProductGroup.SelectedValue = (string)ModuleSettings["DefaultProductGroup"];

                    if (ModuleSettings["ShowLevels"] != null)
						txtShowLevels.Text = (string)ModuleSettings["ShowLevels"];
					else
						txtShowLevels.Text = "0";

					if (ModuleSettings["AllGroupsImage"] != null)
						AllGroupsImageSelector.Url = (string)ModuleSettings["AllGroupsImage"];
					else
						AllGroupsImageSelector.Url = "";

					if (ModuleSettings["ShowUpNavigation"] != null)
						chkShowUpNavigation.Checked = Convert.ToBoolean(ModuleSettings["ShowUpNavigation"]);
					else
						chkShowUpNavigation.Checked = false;

                    if (ModuleSettings["ShowThisNode"] != null)
                        chkShowThisNode.Checked = Convert.ToBoolean(ModuleSettings["ShowThisNode"]);
                    else
                        chkShowThisNode.Checked = false;

                    if (ModuleSettings["ShowSubNodes"] != null)
                        chkShowSubNodes.Checked = Convert.ToBoolean(ModuleSettings["ShowSubNodes"]);
                    else
                        chkShowSubNodes.Checked = true;
                }
            }
            catch (Exception exc)
            {
                //Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        public override void UpdateSettings()
        {
            try
            {
                ModuleController objModules = new ModuleController();

                objModules.UpdateModuleSetting(ModuleId, "ProductGroupsInRow", txtProductGroupsInRow.Text.Trim());
                objModules.UpdateModuleSetting(ModuleId, "Template", tplTemplate.Value);
                objModules.UpdateModuleSetting(ModuleId, "DynamicPage", urlSelectDynamicPage.Url);
                objModules.UpdateModuleSetting(ModuleId, "IncludeChilds", chkIncludeChilds.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "ShowProductCount", chkShowProductCount.Checked.ToString());
                objModules.UpdateModuleSetting(ModuleId, "SetTitle", chkSetTitle.Checked.ToString());
                objModules.UpdateModuleSetting(ModuleId, "ShowBreadcrumb", chkShowBreadcrumb.Checked.ToString());
                objModules.UpdateModuleSetting(ModuleId, "ShowExpandCollapse", chkShowExpandCollapse.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "WrapNode", chkWrapNode.Checked.ToString());
                objModules.UpdateModuleSetting(ModuleId, "ViewMode", rblViewMode.SelectedValue);
                objModules.UpdateModuleSetting(ModuleId, "ShowIcons", chkShowIcons.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "RootLevel", cboRootLevel.SelectedValue);
				objModules.UpdateModuleSetting(ModuleId, "RootLevelFixed", chkRootLevelFixed.Checked.ToString());
                objModules.UpdateModuleSetting(ModuleId, "DefaultProductGroup", cboDefaultProductGroup.SelectedValue);
                objModules.UpdateModuleSetting(ModuleId, "ShowLevels", txtShowLevels.Text.Trim());
				objModules.UpdateModuleSetting(ModuleId, "AllGroupsImage", BBStoreHelper.GetRelativeFilePath(AllGroupsImageSelector.Url));
				objModules.UpdateModuleSetting(ModuleId, "ShowUpNavigation", chkShowUpNavigation.Checked.ToString());
                objModules.UpdateModuleSetting(ModuleId, "ShowThisNode", chkShowThisNode.Checked.ToString());
                objModules.UpdateModuleSetting(ModuleId, "ShowSubNodes", chkShowSubNodes.Checked.ToString());
            }

            catch (Exception exc)
            {
                //Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        #endregion

        private string CreateThumbHtml(string template)
        {
            StringBuilder sb = new StringBuilder(template);
            string imageUrl = Request.Url.Scheme + "://" + Request.Url.Host + "/dnnimagehandler.ashx?mode=placeholder&nocache=1";
            if (template.IndexOf("[IMAGE:") > -1)
            {
                string imageDimText = VfpInterop.StrExtract(sb.ToString(), "[IMAGE:", "]", 1, 1);
                if (imageDimText != String.Empty)
                {

                    int imageDim = 0;
                    if (Int32.TryParse(imageDimText, out imageDim))
                        imageUrl += string.Format("&width={0}&height={1}&text={0}", imageDim, (int) (imageDim*2/3));
                    
                    sb.Replace("[IMAGE:" + imageDimText + "]", "<img src=\"" + imageUrl + "\" />");
                }
            }
            else if (template.IndexOf("[IMAGE]") > -1)
            {
                imageUrl += "&width=200&height=150&text=Unresized+Image";
                sb.Replace("[IMAGE]", "<img src=\"" + imageUrl + "\" />");
            }
            sb.Replace("[PRODUCTGROUPNAME]", "ProductGroup");
            sb.Replace("[PRODUCTCOUNT]", "42");
            sb.Replace("[ICON]", "<img src=\"file:///" + Server.MapPath("~/Images/icon_solutions_16px.gif") + "\" />");
            return sb.ToString();
        }
        

        protected void rblViewMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            int viewmode = Convert.ToInt32(rblViewMode.SelectedValue);
            MultiView1.ActiveViewIndex = (viewmode == 3  ? 0 : viewmode );
			pnlShowLevels.Visible = (MultiView1.ActiveViewIndex > 0);
        }
    }
}

