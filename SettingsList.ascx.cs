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
    [DNNtc.PackageProperties("BBStore Product List")]
    [DNNtc.ModuleProperties("BBStore Product List")]
    [DNNtc.ModuleControlProperties("Settings", "BBStore Product List Settings", DNNtc.ControlType.Edit, "", true, false)]
    partial class SettingsList : ModuleSettingsBase
    {

        private BBStoreController Controller;
		protected string CurrentLanguage
		{
			get
			{
				return System.Threading.Thread.CurrentThread.CurrentCulture.Name;
			}
		}

        public int ProductListHeaderId
        {
            get
            {
                if (ViewState["ProductListHeaderId"] != null)
                    return Convert.ToInt32(ViewState["ProductListHeaderId"]);
                return -1;
            }
            set { ViewState["ProductListHeaderId"] = value; }
        }

        public int ProductListFooterId
        {
            get
            {
                if (ViewState["ProductListFooterId"] != null)
                    return Convert.ToInt32(ViewState["ProductListFooterId"]);
                return -1;
            }
            set { ViewState["ProductListFooterId"] = value; }
        }
        public int ProductListEmptyId
        {
            get
            {
                if (ViewState["ProductListEmptyId"] != null)
                    return Convert.ToInt32(ViewState["ProductListEmptyId"]);
                return -1;
            }
            set { ViewState["ProductListEmptyId"] = value; }
        }
        #region "Base Method Implementations"
        protected override void OnInit(EventArgs e)
        {
            Controller = new BBStoreController();
            if (!IsPostBack)
            {
				cmdDeleteStaticFilter.Attributes.Add("onClick", "javascript:return confirm('" + Localization.GetString("DeleteProductTemplate.Text", this.LocalResourceFile) + "');");
				FillStaticFilterCombo();

            	String allLinkPos = Localization.GetString("AllLinkPos.Text", this.LocalResourceFile);
				if (!String.IsNullOrEmpty(allLinkPos))
				{
					string[] allPos = allLinkPos.Split(',');
					foreach (string pos in allPos)
					{
						cboShowAllLink.Items.Add(new ListItem(pos));
					}
				}
				else
				{
					cboShowAllLink.Items.Add(new ListItem("None"));
				}
            }
            
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                LocalResourceInfo resource = Controller.GetLocalResource(PortalId, "PRODUCTLISTHEADER");
                if (resource == null)
                {
                    resource = new LocalResourceInfo() { LocalResourceToken = "PRODUCTLISTHEADER", PortalId = PortalId };
                    ProductListHeaderId = Controller.NewLocalResource(resource);
                }
                else
                {
                    ProductListHeaderId = resource.LocalResourceId;
                }
                List<ILanguageEditorInfo> dbLangs = new List<ILanguageEditorInfo>();
                foreach (LocalResourceLangInfo lang in Controller.GetLocalResourceLangs(ProductListHeaderId))
                {
                    dbLangs.Add(lang);
                }
                lngHeaderText.Langs = dbLangs;

                resource = Controller.GetLocalResource(PortalId, "PRODUCTLISTFOOTER");
                if (resource == null)
                {
                    resource = new LocalResourceInfo() { LocalResourceToken = "PRODUCTLISTFOOTER", PortalId = PortalId };
                    ProductListFooterId = Controller.NewLocalResource(resource);
                }
                else
                {
                    ProductListFooterId = resource.LocalResourceId;
                }
                dbLangs = new List<ILanguageEditorInfo>();
                foreach (LocalResourceLangInfo lang in Controller.GetLocalResourceLangs(ProductListFooterId))
                {
                    dbLangs.Add(lang);
                }
                lngFooterText.Langs = dbLangs;

                resource = Controller.GetLocalResource(PortalId, "PRODUCTLISTEMPTY");
                if (resource == null)
                {
                    resource = new LocalResourceInfo() { LocalResourceToken = "PRODUCTLISTEMPTY", PortalId = PortalId };
                    ProductListEmptyId = Controller.NewLocalResource(resource);
                }
                else
                {
                    ProductListEmptyId = resource.LocalResourceId;
                }
                dbLangs = new List<ILanguageEditorInfo>();
                foreach (LocalResourceLangInfo lang in Controller.GetLocalResourceLangs(ProductListEmptyId))
                {
                    dbLangs.Add(lang);
                }
                lngEmptyList.Langs = dbLangs;
            }
            base.OnLoad(e);
            tplTemplate.CreateImageCallback = CreateThumbHtml;
        }
        public override void LoadSettings()
        {
            try
            {
                if (!IsPostBack)
                {

                    if (ModuleSettings["ProductsInRow"] != null)
                        txtProductsInRow.Text = (string)ModuleSettings["ProductsInRow"];
                    else
                        txtProductsInRow.Text = "3";

					if (ModuleSettings["ProductsPerPage"] != null)
						txtProductsPerPage.Text = (string)ModuleSettings["ProductsPerPage"];
					else
						txtProductsPerPage.Text = "25";

                    if (ModuleSettings["SetTitle"] != null)
                        chkSetTitle.Checked = Convert.ToBoolean(ModuleSettings["SetTitle"]);

					if (ModuleSettings["TitleBreadcrumb"] != null)
						chkTitleBreadcrumb.Checked = Convert.ToBoolean(ModuleSettings["TitleBreadcrumb"]);

					if (ModuleSettings["ShowAllLinkPos"] != null)
						cboShowAllLink.SelectedIndex = Convert.ToInt32(ModuleSettings["ShowAllLinkPos"]);

					if (ModuleSettings["ShowListHead"] != null)
						chkShowListHead.Checked = Convert.ToBoolean(ModuleSettings["ShowListHead"]);
					else
						chkShowListHead.Checked = true;

					if (ModuleSettings["ShowPaging"] != null)
						chkShowPaging.Checked = Convert.ToBoolean(ModuleSettings["ShowPaging"]);
					else
						chkShowPaging.Checked = true;

					if (ModuleSettings["RandomSort"] != null)
						chkRandomSort.Checked = Convert.ToBoolean(ModuleSettings["RandomSort"]);
					else
						chkRandomSort.Checked = false;

                    if (ModuleSettings["Template"] != null)
                        tplTemplate.Value = (string)ModuleSettings["Template"];

					if (ModuleSettings["ProductModulePage"] != null)
						urlProductModulePage.Url = (string)ModuleSettings["ProductModulePage"];

					if (ModuleSettings["ProductListModulePage"] != null)
						urlProductListModulePage.Url = (string)ModuleSettings["ProductListModulePage"];

					rblSelection.Items.Add(new ListItem(Localization.GetString("rblSelectionDynamic.Text", this.LocalResourceFile), "0"));
					rblSelection.Items.Add(new ListItem(Localization.GetString("rblSelectionStatic.Text", this.LocalResourceFile), "1"));

					if (ModuleSettings["Selection"] != null)
						rblSelection.SelectedValue = (string)ModuleSettings["Selection"];
					else
						rblSelection.SelectedIndex = 0;
					rblSelection_SelectedIndexChanged(this, new EventArgs());

					if (ModuleSettings["StaticFilterId"] != null)
					{
						int staticFilterId = Convert.ToInt32((string)ModuleSettings["StaticFilterId"]);
						cboStaticFilter.SelectedValue = staticFilterId.ToString();

						if (staticFilterId == -1)
						{
							cboStaticFilter.SelectedIndex = 0;
							cmdSaveStaticFilter.Visible = true;
							cmdDeleteStaticFilter.Visible = false;
							txtStaticFilterToken.Visible = true;
							lblStaticFilterToken.Visible = true;
							txtStaticFilter.Enabled = true;
							txtStaticFilter.Text = "";
						}
						else
						{
							StaticFilterInfo fi = Controller.GetStaticFilterById(staticFilterId);
							cboStaticFilter.SelectedValue = fi.StaticFilterId.ToString();
							cmdSaveStaticFilter.Visible = false;
							cmdDeleteStaticFilter.Visible = true;
							txtStaticFilterToken.Visible = false;
							txtStaticFilterToken.Text = cboStaticFilter.SelectedItem.Text;
							lblStaticFilterToken.Visible = false;
							txtStaticFilter.Enabled = false;
							txtStaticFilter.Text = fi.FilterCondition;
						}
					}
					else
					{
						cboStaticFilter.SelectedIndex = 0;
						cmdSaveStaticFilter.Visible = true;
						cmdDeleteStaticFilter.Visible = false;
						txtStaticFilterToken.Visible = true;
						lblStaticFilterToken.Visible = true;
						txtStaticFilter.Enabled = true;
						txtStaticFilter.Text = "";
					}

					if (ModuleSettings["TopN"] != null)
						txtTopN.Text = (string)ModuleSettings["TopN"];
					else
						txtTopN.Text = "0";

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

                objModules.UpdateModuleSetting(ModuleId, "ProductsInRow", txtProductsInRow.Text.Trim());
				objModules.UpdateModuleSetting(ModuleId, "ProductsPerPage", txtProductsPerPage.Text.Trim());
				objModules.UpdateModuleSetting(ModuleId, "TopN", txtTopN.Text.Trim());
                objModules.UpdateModuleSetting(ModuleId, "SetTitle", chkSetTitle.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "TitleBreadcrumb", chkTitleBreadcrumb.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "ShowAllLinkPos", cboShowAllLink.SelectedIndex.ToString());
				objModules.UpdateModuleSetting(ModuleId, "ShowListHead", chkShowListHead.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "ShowPaging", chkShowPaging.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "RandomSort", chkRandomSort.Checked.ToString());
                objModules.UpdateModuleSetting(ModuleId, "Template", tplTemplate.Value);
                objModules.UpdateModuleSetting(ModuleId, "ProductModulePage", urlProductModulePage.Url);
				objModules.UpdateModuleSetting(ModuleId, "ProductListModulePage", urlProductListModulePage.Url);
				objModules.UpdateModuleSetting(ModuleId, "Selection", rblSelection.SelectedValue);
				if (cboStaticFilter.SelectedIndex == 0)
					objModules.UpdateModuleSetting(ModuleId, "StaticFilterId", "-1");
				else
					objModules.UpdateModuleSetting(ModuleId, "StaticFilterId", cboStaticFilter.SelectedValue);

                lngHeaderText.UpdateLangs();
                Controller.DeleteLocalResourceLangs(ProductListHeaderId);
                foreach (LocalResourceLangInfo lang in lngHeaderText.Langs)
                {
                    lang.LocalResourceId = ProductListHeaderId;
                    Controller.NewLocalResourceLang(lang);
                }

                lngFooterText.UpdateLangs();
                Controller.DeleteLocalResourceLangs(ProductListFooterId);
                foreach (LocalResourceLangInfo lang in lngFooterText.Langs)
                {
                    lang.LocalResourceId = ProductListFooterId;
                    Controller.NewLocalResourceLang(lang);
                }
                lngEmptyList.UpdateLangs();
                Controller.DeleteLocalResourceLangs(ProductListEmptyId);
                foreach (LocalResourceLangInfo lang in lngEmptyList.Langs)
                {
                    lang.LocalResourceId = ProductListEmptyId;
                    Controller.NewLocalResourceLang(lang);
                }
            }

            catch (Exception exc)
            {
                //Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        #endregion

        

		protected void cboStaticFilter_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cboStaticFilter.SelectedIndex == 0)
			{
				txtStaticFilter.Enabled = true;
				cmdSaveStaticFilter.Visible = true;
				cmdDeleteStaticFilter.Visible = false;
				txtStaticFilterToken.Visible = true;
				lblStaticFilterToken.Visible = true;
			}
			else
			{
				//BBStoreController Controller = new BBStoreController();
				StaticFilterInfo StaticFilter = Controller.GetStaticFilterById(Convert.ToInt32(cboStaticFilter.SelectedValue));
				txtStaticFilter.Text = StaticFilter.FilterCondition;
				txtStaticFilter.Enabled = false;
				cmdSaveStaticFilter.Visible = false;
				cmdDeleteStaticFilter.Visible = true;
				txtStaticFilterToken.Text = cboStaticFilter.SelectedItem.Text;
				txtStaticFilterToken.Visible = false;
				lblStaticFilterToken.Visible = false;
			}
		}
		protected void cmdSaveStaticFilter_Click(object sender, ImageClickEventArgs e)
		{

			int staticFilterId = -1;
			StaticFilterInfo StaticFilter = Controller.GetStaticFilter(PortalId, txtStaticFilterToken.Text.Trim());
			if (StaticFilter != null)
			{
				StaticFilter.FilterCondition = txtStaticFilter.Text;
				Controller.UpdateStaticFilter(StaticFilter);
				staticFilterId = StaticFilter.StaticFilterId;
			}
			else
			{
				StaticFilter = new StaticFilterInfo();
				StaticFilter.Token = txtStaticFilterToken.Text.Trim();
				StaticFilter.FilterCondition = txtStaticFilter.Text;
				StaticFilter.PortalId = PortalId;
				staticFilterId = Controller.NewStaticFilter(StaticFilter);
			}

			FillStaticFilterCombo();

			cboStaticFilter.SelectedValue = staticFilterId.ToString();
			cmdSaveStaticFilter.Visible = false;
			cmdDeleteStaticFilter.Visible = true;
			txtStaticFilterToken.Visible = false;
			lblStaticFilterToken.Visible = false;
		}
		protected void cmdDeleteStaticFilter_Click(object sender, ImageClickEventArgs e)
		{
			int staticFilterId = Convert.ToInt32(cboStaticFilter.SelectedValue);
			StaticFilterInfo fi = Controller.GetStaticFilterById(staticFilterId);
			Controller.DeleteStaticFilterById(staticFilterId);

			FillStaticFilterCombo();
			cboStaticFilter.SelectedIndex = 0;
			txtStaticFilter.Enabled = true;
			cmdSaveStaticFilter.Visible = true;
			cmdDeleteStaticFilter.Visible = false;
			txtStaticFilterToken.Visible = true;
			lblStaticFilterToken.Visible = true;
		}

		protected void rblSelection_SelectedIndexChanged(object sender, EventArgs e)
		{
			trStaticText.Visible = rblSelection.SelectedValue == "1";
			trStaticSelection.Visible = rblSelection.SelectedValue == "1";
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
            sb.Replace("[PRODUCTDESCRIPTION]", "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum .");
            sb.Replace("[PRODUCTSHORTDESCRIPTION]", "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor.");
            sb.Replace("[PRODUCTOPTIONS]", "<span>ProductOption</span>&nbsp;<select name=\"Select1\"><option>Option1</option></select>");
            sb.Replace("[MANDATORYERROR]", "Error Message");
            sb.Replace("[PRICE]", "123.44");
            sb.Replace("[CURRENCY]", PortalSettings.Currency);
            sb.Replace("[ADDCARTIMAGE]", "<img src=\"file:///" + Server.MapPath("~/images/cart.gif") + "\" />");
            sb.Replace("[ADDCARTLINK]", "Add to cart");
            sb.Replace("[TAX]", "includes tax (19%)");
            sb.Replace("[UNIT]", "pcs.");
            sb.Replace("[AMOUNT]", "<input name=\"Text1\" type=\"text\" size=\"3\" value=\"1\"/>");

            return sb.ToString();
        }
        
		private void FillStaticFilterCombo()
		{
			List<StaticFilterInfo> lst = Controller.GetStaticFilters(PortalId);
			ListItemCollection colListItemCollection = new ListItemCollection();
			colListItemCollection.Add(new ListItem(Localization.GetString("Edit.Text", this.LocalResourceFile), "-1"));
			foreach (StaticFilterInfo fi in lst)
			{
				colListItemCollection.Add(new ListItem(fi.Token,fi.StaticFilterId.ToString()));
			}
			cboStaticFilter.DataSource = colListItemCollection;
			cboStaticFilter.DataTextField = "text";
			cboStaticFilter.DataValueField = "value";
			cboStaticFilter.DataBind();
		}
    }
}

