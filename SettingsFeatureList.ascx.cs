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
    [DNNtc.PackageProperties("BBStore FeatureList")]
    [DNNtc.ModuleProperties("BBStore FeatureList")]
    [DNNtc.ModuleControlProperties("Settings", "BBStore FeatureList Settings", DNNtc.ControlType.Edit, "", true, false)]
    partial class SettingsFeatureList : ModuleSettingsBase
    {

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
                // We fill the Combo
				FillFeatureListCombo();
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
					if (ModuleSettings["ViewMode"] != null)
						rblViewMode.SelectedValue = (string)ModuleSettings["ViewMode"];
					else
						rblViewMode.SelectedValue = "0";

					rblViewMode_SelectedIndexChanged(this,new EventArgs());

					if (ModuleSettings["RotatorHeight"] != null)
						txtRotatorHeight.Text = (string)ModuleSettings["RotatorHeight"];
					else
						txtRotatorHeight.Text = "90";
                    
					if (ModuleSettings["FeaturesInRow"] != null)
						txtFeaturesInRow.Text = (string)ModuleSettings["FeaturesInRow"];
                    else
						txtFeaturesInRow.Text = "1";

					if (ModuleSettings["OnlyWithImage"] != null)
						chkOnlyWithImage.Checked = Convert.ToBoolean(ModuleSettings["OnlyWithImage"]);
					else
						chkOnlyWithImage.Checked = false;

					if (ModuleSettings["FeatureListId"] != null)
					{
						cboFeatureList.SelectedValue = (string)ModuleSettings["FeatureListId"];
					}

					if (ModuleSettings["ProductListModulePage"] != null)
						urlProductListModulePage.Url = (string)ModuleSettings["ProductListModulePage"];

					if (ModuleSettings["HeaderText"] != null)
						txtHeaderText.Text = (string)ModuleSettings["HeaderText"];

					if (ModuleSettings["FooterText"] != null)
						txtFooterText.Text = (string)ModuleSettings["FooterText"];

                    if (ModuleSettings["Template"] != null)
                        tplTemplate.Value = (string)ModuleSettings["Template"];

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

				objModules.UpdateModuleSetting(ModuleId, "FeatureListId", cboFeatureList.SelectedValue);
				objModules.UpdateModuleSetting(ModuleId, "OnlyWithImage", chkOnlyWithImage.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "ProductListModulePage", urlProductListModulePage.Url);
				objModules.UpdateModuleSetting(ModuleId, "ViewMode", rblViewMode.SelectedValue);
				objModules.UpdateModuleSetting(ModuleId, "FeaturesInRow", txtFeaturesInRow.Text.Trim());
				objModules.UpdateModuleSetting(ModuleId, "RotatorHeight", txtRotatorHeight.Text);
				objModules.UpdateModuleSetting(ModuleId, "HeaderText", txtHeaderText.Text);
				objModules.UpdateModuleSetting(ModuleId, "FooterText", txtFooterText.Text);
                objModules.UpdateModuleSetting(ModuleId, "Template", tplTemplate.Value);
            }
            catch (Exception exc)
            {
                //Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        #endregion

		protected void rblViewMode_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (rblViewMode.SelectedValue == "0")
			{
				pnlTable.Visible = true;
				pnlRotator.Visible = false;
			}
			else
			{
				pnlTable.Visible = false;
				pnlRotator.Visible = true;
			}
		}

        private string CreateThumbHtml(string template)
        {
            StringBuilder sb = new StringBuilder(template);
            string imageUrl = Request.Url.Scheme + "://" + Request.Url.Host + "/bbimagehandler.ashx?placeholder=1&nocache=1";
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
            sb.Replace("[TITLE]", "Product Title");
            return sb.ToString();
        }
        
		private void FillFeatureListCombo()
		{
			List<FeatureListInfo> lst = Controller.GetFeatureLists(PortalId,CurrentLanguage);
			ListItemCollection colListItemCollection = new ListItemCollection();
			colListItemCollection.Add(new ListItem(Localization.GetString("Edit.Text", this.LocalResourceFile), "-1"));
			foreach (FeatureListInfo fli in lst)
			{
				colListItemCollection.Add(new ListItem(fli.FeatureList,fli.FeatureListId.ToString()));
			}
			cboFeatureList.DataSource = colListItemCollection;
			cboFeatureList.DataTextField = "text";
			cboFeatureList.DataValueField = "value";
			cboFeatureList.DataBind();
		}
    }
}

