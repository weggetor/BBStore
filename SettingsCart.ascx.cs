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

using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using System.Web.UI.WebControls;

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
    [DNNtc.PackageProperties("BBStore Cart")]
    [DNNtc.ModuleProperties("BBStore Cart")]
    [DNNtc.ModuleControlProperties("Settings", "BBStore Cart Settings", DNNtc.ControlType.Edit, "", false, false)]
    partial class SettingsCart : ModuleSettingsBase
    {

        private BBStoreController _controller;
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

    	public BBStoreController Controller
		{
			get
			{
				if (_controller == null)
					 _controller = new BBStoreController();
				return _controller;
			}
		}

        public int EmptyCartResourceId
        {
            get
            {
                if (ViewState["EmptyCartResourceId"] != null)
                    return Convert.ToInt32(ViewState["EmptyCartResourceId"]);
                return -1;
            }
            set { ViewState["EmptyCartResourceId"] = value; }
        }

        public int ConfirmCartResourceId
        {
            get
            {
                if (ViewState["ConfirmCartResourceId"] != null)
                    return Convert.ToInt32(ViewState["ConfirmCartResourceId"]);
                return -1;
            }
            set { ViewState["ConfirmCartResourceId"] = value; }
        }
        #region "Base Method Implementations"

        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                // Read the texts for empty cart display
                LocalResourceInfo emptyCartResource = Controller.GetLocalResource(PortalId, "EMPTYCART");
                if (emptyCartResource == null)
                {
                    emptyCartResource = new LocalResourceInfo() {LocalResourceToken = "EMPTYCART", PortalId = PortalId};
                    EmptyCartResourceId = Controller.NewLocalResource(emptyCartResource);
                }
                else
                {
                    EmptyCartResourceId = emptyCartResource.LocalResourceId;
                }

                List<ILanguageEditorInfo> dbLangs = new List<ILanguageEditorInfo>();
                foreach (LocalResourceLangInfo lang in Controller.GetLocalResourceLangs(EmptyCartResourceId))
                {
                    dbLangs.Add(lang);
                }
                lngEmptyCart.Langs = dbLangs;

                // Read the texts for confirmed order display
                LocalResourceInfo confirmCartResource = Controller.GetLocalResource(PortalId, "CONFIRMCART");
                if (confirmCartResource == null)
                {
                    confirmCartResource = new LocalResourceInfo() { LocalResourceToken = "CONFIRMCART", PortalId = PortalId };
                    ConfirmCartResourceId = Controller.NewLocalResource(confirmCartResource);
                }
                else
                {
                    ConfirmCartResourceId = confirmCartResource.LocalResourceId;
                }
                
                dbLangs = new List<ILanguageEditorInfo>();
                foreach (LocalResourceLangInfo lang in Controller.GetLocalResourceLangs(ConfirmCartResourceId))
                {
                    dbLangs.Add(lang);
                }
                lngConfirmCart.Langs = dbLangs;

                // Cart Navigation Styles
                CartNavigationControl nav = LoadControl("Controls/CartNavigationControl.ascx") as CartNavigationControl;
                ddlCartNavigationStyle.DataSource = nav.ImageStyles;
                ddlCartNavigationStyle.DataBind();

            }
            tplTemplate.CaptionText = LocalizeString("lblTemplate.Text");
            tplTemplate.CaptionHelp = LocalizeString("lblTemplate.Help");
            base.OnLoad(e);
        }

        public override void LoadSettings()
        {
            try
            {
                if (!IsPostBack)
                {
                    if (ModuleSettings["ColVisibleImage"] != null)
                        chkColVisibleImage.Checked = Convert.ToBoolean(ModuleSettings["ColVisibleImage"]);
                    else
                        chkColVisibleImage.Checked = true;

                    if (ModuleSettings["ColWidthImage"] != null)
                        txtColWidthImage.Text = (string)ModuleSettings["ColWidthImage"];
                    else
                        txtColWidthImage.Text = "80";

                    if (ModuleSettings["ColWidthQuantity"] != null)
                        txtColWidthQuantity.Text = (string)ModuleSettings["ColWidthQuantity"];
                    else
                        txtColWidthQuantity.Text = "50";

                    if (ModuleSettings["ColWidthUnit"] != null)
                        txtColWidthUnit.Text = (string)ModuleSettings["ColWidthUnit"];
                    else
                        txtColWidthUnit.Text = "20";

                    if (ModuleSettings["ColVisibleUnit"] != null)
                        chkColVisibleUnit.Checked = Convert.ToBoolean(ModuleSettings["ColVisibleUnit"]);
                    else
                        chkColVisibleUnit.Checked = false;

                    if (ModuleSettings["ColVisibleItemNo"] != null)
                        chkColVisibleItemNo.Checked = Convert.ToBoolean(ModuleSettings["ColVisibleItemNo"]);
                    else
                        chkColVisibleItemNo.Checked = false;

                    if (ModuleSettings["ColWidthItemNo"] != null)
                        txtColWidthItemNo.Text = (string)ModuleSettings["ColWidthItemNo"];
                    else
                        txtColWidthItemNo.Text = "70";

                    if (ModuleSettings["ColVisibleTaxPercent"] != null)
                        chkColVisibleTaxPercent.Checked = Convert.ToBoolean(ModuleSettings["ColVisibleTaxPercent"]);
                    else
                        chkColVisibleTaxPercent.Checked = true;

                    if (ModuleSettings["ColWidthPercent"] != null)
                        txtColWidthPercent.Text = (string)ModuleSettings["ColWidthPercent"];
                    else
                        txtColWidthPercent.Text = "50";

                    if (ModuleSettings["ColVisibleUnitCost"] != null)
                        chkColVisibleUnitCost.Checked = Convert.ToBoolean(ModuleSettings["ColVisibleUnitCost"]);
                    else
                        chkColVisibleUnitCost.Checked = true;

                    if (ModuleSettings["ColVisibleNetTotal"] != null)
                        chkColVisibleNetTotal.Checked = Convert.ToBoolean(ModuleSettings["ColVisibleNetTotal"]);
                    else
                        chkColVisibleNetTotal.Checked = true;
 
                    if (ModuleSettings["ColVisibleTaxTotal"] != null)
                        chkColVisibleTaxTotal.Checked = Convert.ToBoolean(ModuleSettings["ColVisibleTaxTotal"]);
                    else
                        chkColVisibleTaxTotal.Checked = true;

                    if (ModuleSettings["ColVisibleSubTotal"] != null)
                        chkColVisibleSubTotal.Checked = Convert.ToBoolean(ModuleSettings["ColVisibleSubTotal"]);
                    else
                        chkColVisibleSubTotal.Checked = true;

                    if (ModuleSettings["ColWidthAmount"] != null)
                        txtColWidthAmount.Text = (string)ModuleSettings["ColWidthAmount"];
                    else
                        txtColWidthAmount.Text = "60";

                    if (ModuleSettings["ShowSummary"] != null)
                        chkShowSummary.Checked = Convert.ToBoolean(ModuleSettings["ShowSummary"]);
                    else
                        chkShowSummary.Checked = true;

					if (ModuleSettings["ShoppingTarget"] != null)
						rblShoppingTarget.SelectedValue = (string)ModuleSettings["ShoppingTarget"];
					else
                		rblShoppingTarget.SelectedValue = "1";

					if (ModuleSettings["EnableCartUpload"] != null)
						chkEnableCartUpload.Checked = Convert.ToBoolean(ModuleSettings["EnableCartUpload"]);
					else
						chkEnableCartUpload.Checked = false;

					if (ModuleSettings["EnableCartDownload"] != null)
						chkEnableCartDownload.Checked = Convert.ToBoolean(ModuleSettings["EnableCartDownload"]);
					else
						chkEnableCartDownload.Checked = false;

					if (ModuleSettings["MultipleCustomers"] != null)
						chkMultipleCustomers.Checked = Convert.ToBoolean(ModuleSettings["MultipleCustomers"]);
					else
						chkMultipleCustomers.Checked = false;

                    if (ModuleSettings["CartNavigationStyle"] != null)
                        ddlCartNavigationStyle.SelectedValue = (string) ModuleSettings["CartNavigationStyle"];
                    else
                        ddlCartNavigationStyle.SelectedIndex = 0;
                    
					Hashtable storeSettings = Controller.GetStoreSettings(PortalId);
                	bool showNetPrice = ((string) storeSettings["ShowNetpriceInCart"] == "0");


					// Address Settings
					chkMandCompany.Checked = false;
					if (ModuleSettings["MandCompany"] != null)
						chkMandCompany.Checked = Convert.ToBoolean(ModuleSettings["MandCompany"]);

					chkMandPrefix.Checked = false;
					if (ModuleSettings["MandPrefix"] != null)
						chkMandPrefix.Checked = Convert.ToBoolean(ModuleSettings["MandPrefix"]);

					chkMandFirstname.Checked = false;
					if (ModuleSettings["MandFirstname"] != null)
						chkMandFirstname.Checked = Convert.ToBoolean(ModuleSettings["MandFirstname"]);

					chkMandMiddlename.Checked = false;
					if (ModuleSettings["MandMiddlename"] != null)
						chkMandMiddlename.Checked = Convert.ToBoolean(ModuleSettings["MandMiddlename"]);

					chkMandLastname.Checked = false;
					if (ModuleSettings["MandLastname"] != null)
						chkMandLastname.Checked = Convert.ToBoolean(ModuleSettings["MandLastname"]);

					chkMandSuffix.Checked = false;
					if (ModuleSettings["MandSuffix"] != null)
						chkMandSuffix.Checked = Convert.ToBoolean(ModuleSettings["MandSuffix"]);

					chkMandStreet.Checked = false;
					if (ModuleSettings["MandStreet"] != null)
						chkMandStreet.Checked = Convert.ToBoolean(ModuleSettings["MandStreet"]);

					chkMandUnit.Checked = false;
					if (ModuleSettings["MandUnit"] != null)
						chkMandUnit.Checked = Convert.ToBoolean(ModuleSettings["MandUnit"]);

					chkMandPostalCode.Checked = false;
					if (ModuleSettings["MandPostalCode"] != null)
						chkMandPostalCode.Checked = Convert.ToBoolean(ModuleSettings["MandPostalCode"]);

					chkMandCity.Checked = false;
					if (ModuleSettings["MandCity"] != null)
						chkMandCity.Checked = Convert.ToBoolean(ModuleSettings["MandCity"]);

					chkMandSuburb.Checked = false;
					if (ModuleSettings["MandSuburb"] != null)
						chkMandSuburb.Checked = Convert.ToBoolean(ModuleSettings["MandSuburb"]);

					chkMandRegion.Checked = false;
					if (ModuleSettings["MandRegion"] != null)
						chkMandRegion.Checked = Convert.ToBoolean(ModuleSettings["MandRegion"]);

					chkMandCountry.Checked = false;
					if (ModuleSettings["MandCountry"] != null)
						chkMandCountry.Checked = Convert.ToBoolean(ModuleSettings["MandCountry"]);

					chkMandPhone.Checked = false;
					if (ModuleSettings["MandPhone"] != null)
						chkMandPhone.Checked = Convert.ToBoolean(ModuleSettings["MandPhone"]);

					chkMandCell.Checked = false;
					if (ModuleSettings["MandCell"] != null)
						chkMandCell.Checked = Convert.ToBoolean(ModuleSettings["MandCell"]);

					chkMandFax.Checked = false;
					if (ModuleSettings["MandFax"] != null)
						chkMandFax.Checked = Convert.ToBoolean(ModuleSettings["MandFax"]);

					chkMandEmail.Checked = false;
					if (ModuleSettings["MandEmail"] != null)
						chkMandEmail.Checked = Convert.ToBoolean(ModuleSettings["MandEmail"]);

                    if (ModuleSettings["Template"] != null) 
                        tplTemplate.Value = (string)ModuleSettings["Template"];

                    txtAddressTemplate.Text = Localization.GetString("AddressTemplate.Text", this.LocalResourceFile.Replace("SettingsCart", "ViewCartAddressEdit"));


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

                objModules.UpdateModuleSetting(ModuleId, "ColWidthQuantity", txtColWidthQuantity.Text.Trim());
                objModules.UpdateModuleSetting(ModuleId, "ColVisibleImage", chkColVisibleImage.Checked.ToString());
                objModules.UpdateModuleSetting(ModuleId, "ColWidthImage", txtColWidthImage.Text.Trim());
                objModules.UpdateModuleSetting(ModuleId, "ColVisibleItemNo", chkColVisibleItemNo.Checked.ToString());
                objModules.UpdateModuleSetting(ModuleId, "ColWidthItemNo", txtColWidthItemNo.Text.Trim());
                objModules.UpdateModuleSetting(ModuleId, "ColWidthUnit", txtColWidthUnit.Text.Trim());
                objModules.UpdateModuleSetting(ModuleId, "ColVisibleTaxPercent", chkColVisibleTaxPercent.Checked.ToString());
                objModules.UpdateModuleSetting(ModuleId, "ColWidthPercent", txtColWidthPercent.Text.Trim());
                objModules.UpdateModuleSetting(ModuleId, "ColVisibleUnitCost", chkColVisibleUnitCost.Checked.ToString());
                objModules.UpdateModuleSetting(ModuleId, "ColVisibleNetTotal", chkColVisibleNetTotal.Checked.ToString());
                objModules.UpdateModuleSetting(ModuleId, "ColVisibleTaxTotal", chkColVisibleTaxTotal.Checked.ToString());
                objModules.UpdateModuleSetting(ModuleId, "ColVisibleSubTotal", chkColVisibleSubTotal.Checked.ToString());
                objModules.UpdateModuleSetting(ModuleId, "ColVisibleUnit", chkColVisibleUnit.Checked.ToString());
                objModules.UpdateModuleSetting(ModuleId, "ColWidthAmount", txtColWidthAmount.Text.Trim());
                objModules.UpdateModuleSetting(ModuleId, "ShowSummary", chkShowSummary.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "ShoppingTarget", rblShoppingTarget.SelectedValue);
				objModules.UpdateModuleSetting(ModuleId, "EnableCartUpload", chkEnableCartUpload.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "EnableCartDownload", chkEnableCartDownload.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "MultipleCustomers", chkMultipleCustomers.Checked.ToString());
                objModules.UpdateModuleSetting(ModuleId, "CartNavigationStyle", ddlCartNavigationStyle.SelectedValue);

				objModules.UpdateModuleSetting(ModuleId, "MandCompany", chkMandCompany.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "MandPrefix", chkMandPrefix.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "MandFirstname", chkMandFirstname.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "MandMiddlename", chkMandMiddlename.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "MandLastname", chkMandLastname.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "MandSuffix", chkMandSuffix.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "MandStreet", chkMandStreet.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "MandUnit", chkMandUnit.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "MandRegion", chkMandRegion.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "MandPostalCode", chkMandPostalCode.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "MandCity", chkMandCity.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "MandSuburb", chkMandSuburb.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "MandCountry", chkMandCountry.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "MandPhone", chkMandPhone.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "MandCell", chkMandCell.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "MandFax", chkMandFax.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "MandEmail", chkMandEmail.Checked.ToString());
                objModules.UpdateModuleSetting(ModuleId, "Template", tplTemplate.Value);

				string resFile = this.LocalResourceFile.Replace("SettingsCart", "ViewCartAddressEdit") + ".ascx" + (CurrentLanguage != "en-US" ? "." + CurrentLanguage : "") +
	                             ".Portal-" + PortalSettings.PortalId.ToString() + ".resx";

				Dictionary<string, string> entries = new Dictionary<string, string>();
				if (File.Exists(this.MapPath(resFile)))
	            {
					using (ResXResourceReader resxReader = new ResXResourceReader(this.MapPath(resFile)))
		            {
			            foreach (DictionaryEntry entry in resxReader)
			            {
				            entries.Add((string)entry.Key,(string)entry.Value);
			            }
		            }
	            }
	            ResXResourceWriter resourceWriter = new ResXResourceWriter(this.MapPath(resFile));
				resourceWriter.AddResource("AddressTemplate.Text", txtAddressTemplate.Text);
	            foreach (KeyValuePair<string, string> entry in entries)
	            {
					if (entry.Key != "AddressTemplate.Text")
                        resourceWriter.AddResource(entry.Key,entry.Value);
	            }
				resourceWriter.Close();

                lngEmptyCart.UpdateLangs();
                Controller.DeleteLocalResourceLangs(EmptyCartResourceId);
                foreach (LocalResourceLangInfo lang in lngEmptyCart.Langs)
                {
                    lang.LocalResourceId = EmptyCartResourceId;
                    Controller.NewLocalResourceLang(lang);
                }

                lngConfirmCart.UpdateLangs();
                Controller.DeleteLocalResourceLangs(ConfirmCartResourceId);
                foreach (LocalResourceLangInfo lang in lngConfirmCart.Langs)
                {
                    lang.LocalResourceId = ConfirmCartResourceId;
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

		protected void valAddressTemplate_ServerValidate(object sender, ServerValidateEventArgs e)
		{
			e.IsValid = !(chkMandCompany.Checked && txtAddressTemplate.Text.IndexOf("[COMPANY") == -1 ||
			        chkMandPrefix.Checked && txtAddressTemplate.Text.IndexOf("[PREFIX") == -1 ||
			        chkMandFirstname.Checked && txtAddressTemplate.Text.IndexOf("[FIRSTNAME") == -1 ||
			        chkMandMiddlename.Checked && txtAddressTemplate.Text.IndexOf("[MIDDLENAME") == -1 ||
			        chkMandLastname.Checked && txtAddressTemplate.Text.IndexOf("[LASTNAME") == -1 ||
			        chkMandSuffix.Checked && txtAddressTemplate.Text.IndexOf("[SUFFIX") == -1 ||
			        chkMandStreet.Checked && txtAddressTemplate.Text.IndexOf("[STREET") == -1 ||
			        chkMandUnit.Checked && txtAddressTemplate.Text.IndexOf("[UNIT") == -1 ||
			        chkMandRegion.Checked && txtAddressTemplate.Text.IndexOf("[REGION") == -1 ||
			        chkMandPostalCode.Checked && txtAddressTemplate.Text.IndexOf("[POSTALCODE") == -1 ||
			        chkMandCity.Checked && txtAddressTemplate.Text.IndexOf("[CITY") == -1 ||
			        chkMandSuburb.Checked && txtAddressTemplate.Text.IndexOf("[SUBURB") == -1 ||
			        chkMandCountry.Checked && txtAddressTemplate.Text.IndexOf("[COUNTRY") == -1 ||
			        chkMandPhone.Checked && txtAddressTemplate.Text.IndexOf("[PHONE") == -1 ||
			        chkMandCell.Checked && txtAddressTemplate.Text.IndexOf("[CELL") == -1 ||
			        chkMandFax.Checked && txtAddressTemplate.Text.IndexOf("[FAX") == -1 ||
			        chkMandEmail.Checked && txtAddressTemplate.Text.IndexOf("[EMAIL") == -1);

		}

        

    }
}

