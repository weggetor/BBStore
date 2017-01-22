
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Bitboxx.License;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using System.Text;
using System.IO;
using DotNetNuke.Common.Lists;
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
    [DNNtc.PackageProperties("BBStore Admin")]
    [DNNtc.ModuleProperties("BBStore Admin")]
    [DNNtc.ModuleControlProperties("Settings", "BBStore Admin Settings", DNNtc.ControlType.Edit, "", true, false)]
    partial class SettingsAdmin : ModuleSettingsBase
	{

		#region Properties
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
                return PortalSettings.DefaultLanguage;
            }
        }
		#endregion

        #region Fields

        string _startString = "/* BBSTORE Styles BEGIN */";
        string _endString = "/* BBSTORE Styles END */";

        #endregion

        #region "Base Method Implementations"
        protected override void OnInit(EventArgs e)
		{
            base.OnInit(e);
			cboSupplierRole.DataSource = GetRoles();
			cboSupplierRole.DataBind();
            rblSMTPSettings.Items.Add(new ListItem(LocalizeString("SMTPSettingsHost"), "0"));
            rblSMTPSettings.Items.Add(new ListItem(LocalizeString("SMTPSettingsManual"), "1"));
		}
       
		protected override void OnLoad(EventArgs e)
        {

            if (!IsPostBack)
            {
                ListController listController = new ListController();
                ListEntryInfoCollection countries = listController.GetListEntryInfoCollection("Country");
                ddlVendorCountry.DataSource = countries;
                ddlVendorCountry.DataTextField = "Text";
                ddlVendorCountry.DataValueField = "Value";
                try
                {
                    ddlVendorCountry.DataBind();
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    ddlVendorCountry.SelectedValue = ddlVendorCountry.Items[0].Value;
                    ddlVendorCountry.DataBind();
                }
            }

            base.OnLoad(e);
        }

        public override void LoadSettings()
        {
            try
            {
				if (ModuleSettings["StoreEmail"] == null)
				{
					ModuleController objModules = new ModuleController();
					ModuleInfo cartModule = objModules.GetModuleByDefinition(PortalSettings.PortalId, "BBStore Cart");
					if (cartModule != null)
					{
						Hashtable storeSettings = objModules.GetModuleSettings(cartModule.ModuleID);
						ModuleSettings["StoreEmail"] = storeSettings["StoreEmail"];
						ModuleSettings["StoreName"] = storeSettings["StoreName"];
						ModuleSettings["StoreReplyTo"] = storeSettings["StoreReplyTo"];
						ModuleSettings["StoreAdmin"] = storeSettings["StoreAdmin"];
						ModuleSettings["StoreSubject"] = storeSettings["StoreSubject"];
						ModuleSettings["VendorName"] = storeSettings["VendorName"];
						ModuleSettings["VendorStreet1"] = storeSettings["VendorStreet1"];
						ModuleSettings["VendorStreet2"] = storeSettings["VendorStreet2"];
						ModuleSettings["VendorZip"] = storeSettings["VendorZip"];
						ModuleSettings["VendorCity"] = storeSettings["VendorCity"];
						ModuleSettings["VendorCountry"] = storeSettings["VendorCountry"];
						ModuleSettings["OrderMask"] = storeSettings["OrderMask"];
						ModuleSettings["ShowNetpriceInCart"] = storeSettings["ShowNetpriceInCart"];
						ModuleSettings["ShowNetpriceInCart"] = storeSettings["ShowNetpriceInCart"];
						ModuleSettings["TermsUrl"] = storeSettings["TermsUrl"];
						ModuleSettings["StartPage"] = storeSettings["StartPage"];
					}
				}

            	if (!IsPostBack)
                {
                    if (ModuleSettings["InitialKey"] != null)
                        txtInitialKey.Text = (string)ModuleSettings["InitialKey"];

                    if (ModuleSettings["StoreEmail"] != null)
                        txtStoreEmail.Text = (string)ModuleSettings["StoreEmail"];
                    if (ModuleSettings["StoreName"] != null)
                        txtStoreName.Text = (string)ModuleSettings["StoreName"];
                    if (ModuleSettings["StoreReplyTo"] != null)
                        txtStoreReplyTo.Text = (string)ModuleSettings["StoreReplyTo"];
                    if (ModuleSettings["StoreAdmin"] != null)
                        txtStoreAdmin.Text = (string)ModuleSettings["StoreAdmin"];
                    if (ModuleSettings["StoreSubject"] != null)
                        txtStoreSubject.Text = (string)ModuleSettings["StoreSubject"];

                    int smtpSettings = Convert.ToInt32(ModuleSettings["SMTPSettings"] ?? "0");
                    pnlSMTP.Visible = (smtpSettings == 1);
                    rblSMTPSettings.SelectedIndex = smtpSettings;

                    txtSMTPServer.Text = (string)ModuleSettings["SMTPServer"] ?? "";
                    txtSMTPUser.Text = (string)ModuleSettings["SMTPUser"] ?? "";
                    txtSMTPPassword.Text = (string)ModuleSettings["SMTPPassword"] ?? "";

                    if (ModuleSettings["VendorName"] != null)
                        txtVendorName.Text = (string)ModuleSettings["VendorName"];
                    if (ModuleSettings["VendorStreet1"] != null)
                        txtVendorStreet1.Text = (string)ModuleSettings["VendorStreet1"];
                    if (ModuleSettings["VendorStreet2"] != null)
                        txtVendorStreet2.Text = (string)ModuleSettings["VendorStreet2"];
                    if (ModuleSettings["VendorZip"] != null)
                        txtVendorZip.Text = (string)ModuleSettings["VendorZip"];
                    if (ModuleSettings["VendorCity"] != null)
                        txtVendorCity.Text = (string)ModuleSettings["VendorCity"];
                    if (ModuleSettings["VendorCountry"] != null)
                        ddlVendorCountry.SelectedValue = (string)ModuleSettings["VendorCountry"];

					if (ModuleSettings["SupplierRole"] != null)
						cboSupplierRole.SelectedValue = (string) ModuleSettings["SupplierRole"];
					
					if (ModuleSettings["OrderMask"] != null)
                        txtOrderMask.Text = (string)ModuleSettings["OrderMask"];
                    else
                        txtOrderMask.Text = @"%N4%";

                    if (ModuleSettings["ShowNetpriceInCart"] != null)
                    {
                        foreach (System.Web.UI.WebControls.ListItem item in opgShowNetpriceInCart.Items)
                        {
                            item.Selected = false;
                            if (item.Value == (string)ModuleSettings["ShowNetpriceInCart"])
                            {
                                item.Selected = true;
                                break;
                            }
                        }
                    }
                    else
                        opgShowNetpriceInCart.SelectedIndex = 1;

                    if (ModuleSettings["ExtendedPrice"] != null)
                        chkExtendedPrice.Checked = Convert.ToBoolean(ModuleSettings["ExtendedPrice"]);

                    if (ModuleSettings["TermsUrl"] != null)
                        urlSelectTerms.Url = (string)ModuleSettings["TermsUrl"];

					if (ModuleSettings["TermsMandatory"] != null)
						chkTermsMandatory.Checked = Convert.ToBoolean(ModuleSettings["TermsMandatory"]);

                    if (ModuleSettings["CancelTermsMandatory"] != null)
                        chkCancelTermsMandatory.Checked = Convert.ToBoolean(ModuleSettings["CancelTermsMandatory"]);

                    if (ModuleSettings["CancelTerms"] != null)
                        txtCancelTerms.Text = (string)ModuleSettings["CancelTerms"];

                    if (ModuleSettings["CouponsEnabled"] != null)
                        chkCouponsEnabled.Checked = Convert.ToBoolean(ModuleSettings["CouponsEnabled"]);

                    if (ModuleSettings["StartPage"] != null)
                        urlSelectStartPage.Url = (string)ModuleSettings["StartPage"];

					if (ModuleSettings["ProductImageDir"] != null)
						cboProductImageDir.Text = (string)ModuleSettings["ProductImageDir"];
					if (ModuleSettings["ProductGroupImageDir"] != null)
						cboProductGroupImageDir.Text = (string)ModuleSettings["ProductGroupImageDir"];
					if (ModuleSettings["ProductGroupIconDir"] != null)
						cboProductGroupIconDir.Text = (string)ModuleSettings["ProductGroupIconDir"];

					if (ModuleSettings["ItemsPerPage"] != null)
						txtItemsPerPage.Text = (string)ModuleSettings["ItemsPerPage"];
					else
						txtItemsPerPage.Text = "5,!10,25,50,100";

                    RefreshLicense(false);
                }
                ReadCss();
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

                objModules.UpdateModuleSetting(ModuleId, "InitialKey", txtInitialKey.Text.Trim());
                objModules.UpdateModuleSetting(ModuleId, "ShowNetpriceInCart", opgShowNetpriceInCart.SelectedValue);
                objModules.UpdateModuleSetting(ModuleId, "ExtendedPrice", chkExtendedPrice.Checked.ToString());
                objModules.UpdateModuleSetting(ModuleId, "TermsUrl", urlSelectTerms.Url);
				objModules.UpdateModuleSetting(ModuleId, "TermsMandatory", chkTermsMandatory.Checked.ToString());
                objModules.UpdateModuleSetting(ModuleId, "CancelTermsMandatory", chkCancelTermsMandatory.Checked.ToString());
                objModules.UpdateModuleSetting(ModuleId, "CancelTerms", txtCancelTerms.Text.Trim());
                objModules.UpdateModuleSetting(ModuleId, "CouponsEnabled", chkCouponsEnabled.Checked.ToString());
                objModules.UpdateModuleSetting(ModuleId, "StartPage", urlSelectStartPage.Url);
				objModules.UpdateModuleSetting(ModuleId, "ProductImageDir", cboProductImageDir.Text.Trim());
				objModules.UpdateModuleSetting(ModuleId, "ProductGroupImageDir", cboProductGroupImageDir.Text.Trim());
				objModules.UpdateModuleSetting(ModuleId, "ProductGroupIconDir", cboProductGroupIconDir.Text.Trim());
                objModules.UpdateModuleSetting(ModuleId, "StoreEmail", txtStoreEmail.Text.Trim());
                objModules.UpdateModuleSetting(ModuleId, "StoreName", txtStoreName.Text.Trim());
                objModules.UpdateModuleSetting(ModuleId, "StoreReplyTo", txtStoreReplyTo.Text.Trim());
                objModules.UpdateModuleSetting(ModuleId, "StoreAdmin", txtStoreAdmin.Text.Trim());
                objModules.UpdateModuleSetting(ModuleId, "StoreSubject", txtStoreSubject.Text.Trim());
                objModules.UpdateModuleSetting(ModuleId, "SMTPSettings", rblSMTPSettings.SelectedValue);
                objModules.UpdateModuleSetting(ModuleId, "SMTPServer", txtSMTPServer.Text.Trim());
                objModules.UpdateModuleSetting(ModuleId, "SMTPUser", txtSMTPUser.Text.Trim());
                objModules.UpdateModuleSetting(ModuleId, "SMTPPassword", txtSMTPPassword.Text.Trim());
                objModules.UpdateModuleSetting(ModuleId, "VendorName", txtVendorName.Text.Trim());
                objModules.UpdateModuleSetting(ModuleId, "VendorStreet1", txtVendorStreet1.Text.Trim());
                objModules.UpdateModuleSetting(ModuleId, "VendorStreet2", txtVendorStreet2.Text.Trim());
                objModules.UpdateModuleSetting(ModuleId, "VendorZip", txtVendorZip.Text.Trim());
                objModules.UpdateModuleSetting(ModuleId, "VendorCity", txtVendorCity.Text.Trim());
                objModules.UpdateModuleSetting(ModuleId, "VendorCountry", ddlVendorCountry.SelectedValue.Trim());
                objModules.UpdateModuleSetting(ModuleId, "OrderMask", txtOrderMask.Text.Trim());
				objModules.UpdateModuleSetting(ModuleId, "ItemsPerPage", txtItemsPerPage.Text.Trim());
				objModules.UpdateModuleSetting(ModuleId, "SupplierRole", cboSupplierRole.SelectedValue);

                UpdateCss();
            }

            catch (Exception exc)
            {
                //Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion

		#region Eventhandlers

        protected void cmdRestoreCss_Click(object sender, EventArgs e)
        {
            ResetCss();
            ReadCss();
        }

        protected void txtInitialKey_OnTextChanged(object sender, EventArgs e)
        {
            ModuleController objModules = new ModuleController();
            objModules.UpdateModuleSetting(ModuleId, "InitialKey", txtInitialKey.Text.Trim());
            RefreshLicense(true);
        }

        protected void rblSMTPSettings_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            pnlSMTP.Visible = (rblSMTPSettings.SelectedValue == "1");
        }

        protected void cmdRefreshLicense_OnClick(object sender, EventArgs e)
        {
            RefreshLicense(true);
        }

        #endregion

		#region Helper methods
        protected void ReadCss()
        {
            string[] colors = new string[5] { "", "", "", "", "" };

            string moduleCssFile = MapPath(ControlPath + "module.css");
            if (File.Exists(moduleCssFile))
            {
                string cssContent = File.ReadAllText(moduleCssFile);
                if (cssContent.Contains("COLORBORDER :"))
                {
                    colors[0] = VfpInterop.StrExtract(cssContent, "COLORBORDER : #", ";",1,1).Trim();
                    colors[1] = VfpInterop.StrExtract(cssContent, "COLORROW : #", ";", 1, 1).Trim();
                    colors[2] = VfpInterop.StrExtract(cssContent, "COLORALT : #", ";", 1, 1).Trim();
                    colors[3] = VfpInterop.StrExtract(cssContent, "COLORSUM : #", ";", 1, 1).Trim();
                    colors[4] = VfpInterop.StrExtract(cssContent, "COLORHEAD : #", ";", 1, 1).Trim();
                }
                else
                {
                    string tmp = VfpInterop.StrExtract(cssContent, ".bbstore-grid,", "}", 1, 1);
                    colors[0] = VfpInterop.StrExtract(tmp, "border-color:#", ";", 1, 1);
                    tmp = VfpInterop.StrExtract(cssContent, ".bbstore-grid-row,", "}", 1, 1);
                    colors[1] = VfpInterop.StrExtract(tmp, "background-color:#", ";", 1, 1);
                    tmp = VfpInterop.StrExtract(cssContent, ".bbstore-grid-alternaterow,", "}", 1, 1);
                    colors[2] = VfpInterop.StrExtract(tmp, "background-color:#", ";", 1, 1);
                    tmp = VfpInterop.StrExtract(cssContent, ".bbstore-grid-sumrow,", "}", 1, 1);
                    colors[3] = VfpInterop.StrExtract(tmp, "background-color:#", ";", 1, 1);
                    tmp = VfpInterop.StrExtract(cssContent, ".bbstore-grid-summary,", "}", 1, 1);
                    colors[4] = VfpInterop.StrExtract(tmp, "background-color:#", ";", 1, 1);

                }

            }
            string portalCssFile = this.PortalSettings.HomeDirectoryMapPath + "portal.css";
            if (File.Exists(portalCssFile))
            {
                string cssContent = File.ReadAllText(portalCssFile);
                if (cssContent.Contains("COLORBORDER :"))
                {
                    colors[0] = VfpInterop.StrExtract(cssContent, "COLORBORDER : #", ";", 1, 1).Trim();
                    colors[1] = VfpInterop.StrExtract(cssContent, "COLORROW : #", ";", 1, 1).Trim();
                    colors[2] = VfpInterop.StrExtract(cssContent, "COLORALT : #", ";", 1, 1).Trim();
                    colors[3] = VfpInterop.StrExtract(cssContent, "COLORSUM : #", ";", 1, 1).Trim();
                    colors[4] = VfpInterop.StrExtract(cssContent, "COLORHEAD : #", ";", 1, 1).Trim();
                }
                else if (cssContent.IndexOf(".bbstore-grid,") > -1)
                {
                    string tmp = VfpInterop.StrExtract(cssContent, ".bbstore-grid,", "}", 1, 1);
                    colors[0] = VfpInterop.StrExtract(tmp, "border-color:#", ";", 1, 1);
                    tmp = VfpInterop.StrExtract(cssContent, ".bbstore-grid-row,", "}", 1, 1);
                    colors[1] = VfpInterop.StrExtract(tmp, "background-color:#", ";", 1, 1);
                    tmp = VfpInterop.StrExtract(cssContent, ".bbstore-grid-alternaterow,", "}", 1, 1);
                    colors[2] = VfpInterop.StrExtract(tmp, "background-color:#", ";", 1, 1);
                    tmp = VfpInterop.StrExtract(cssContent, ".bbstore-grid-sumrow,", "}", 1, 1);
                    colors[3] = VfpInterop.StrExtract(tmp, "background-color:#", ";", 1, 1);
                    tmp = VfpInterop.StrExtract(cssContent, ".bbstore-grid-summary,", "}", 1, 1);
                    colors[4] = VfpInterop.StrExtract(tmp, "background-color:#", ";", 1, 1);
                }
                else if (cssContent.IndexOf(".grdCart,") > -1)
                {
                    string tmp = VfpInterop.StrExtract(cssContent, ".grdCart,", "}", 1, 1);
                    colors[0] = VfpInterop.StrExtract(tmp, "border-color:#", ";", 1, 1);
                    tmp = VfpInterop.StrExtract(cssContent, ".grdCartRow,", "}", 1, 1);
                    colors[1] = VfpInterop.StrExtract(tmp, "background-color:#", ";", 1, 1);
                    tmp = VfpInterop.StrExtract(cssContent, ".grdCartAlternatingRow,", "}", 1, 1);
                    colors[2] = VfpInterop.StrExtract(tmp, "background-color:#", ";", 1, 1);
                    tmp = VfpInterop.StrExtract(cssContent, ".grdCartSumRow,", "}", 1, 1);
                    colors[3] = VfpInterop.StrExtract(tmp, "background-color:#", ";", 1, 1);
                    tmp = VfpInterop.StrExtract(cssContent, ".grdCartSummary,", "}", 1, 1);
                    colors[4] = VfpInterop.StrExtract(tmp, "background-color:#", ";", 1, 1);
                }
            }
            txtColorBorder.Text = colors[0];
            txtColorRow.Text = colors[1];
            txtColorAlt.Text = colors[2];
            txtColorSum.Text = colors[3];
            txtColorHead.Text = colors[4];
		}

        private void UpdateCss()
        {
            string portalCssFile = this.PortalSettings.HomeDirectoryMapPath + "portal.css";
            string templateCssFile = PathUtils.Instance.MapPath("~/DesktopModules/BBStore/Templates/module.css.template");
            string cssContent = "";

            if (File.Exists(portalCssFile))
            {
                // we need to cut off the old CSS
                cssContent = File.ReadAllText(portalCssFile);
                int Start = cssContent.IndexOf(_startString);
                int End = cssContent.IndexOf(_endString);
                if (Start > -1 && End > -1 && Start < End)
                {
                    cssContent = cssContent.Substring(0, Start) + "[BBSTORECSS]\r\n" + cssContent.Substring(End + _endString.Length);
                }
                else
                {
                    cssContent = cssContent + "\r\n" + "[BBSTORECSS]";
                }
            }
            string cssTemplate = File.ReadAllText(templateCssFile);
            string[] lines = cssTemplate.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            StringBuilder bbStoreCss = new StringBuilder();
            bbStoreCss.AppendLine(_startString);
            foreach (string line in lines)
            {
                if ((line.StartsWith("/*")  && !line.Contains("*/")) ||
                    (!line.Contains("/*")  && line.Contains("*/")) ||
                    (line.Contains("[") && line.Contains("]")))
                {
                    string newline = line.Replace("[COLORBORDER]", "#" + txtColorBorder.Text.Trim());
                    newline = newline.Replace("[COLORROW]", "#" + txtColorRow.Text.Trim());
                    newline = newline.Replace("[COLORALT]", "#" + txtColorAlt.Text.Trim());
                    newline = newline.Replace("[COLORSUM]", "#" + txtColorSum.Text.Trim());
                    newline = newline.Replace("[COLORHEAD]", "#" + txtColorHead.Text.Trim());
                    newline = newline.Replace("<IMAGEPATH>", "/DesktopModules/BBStore/Controls/Images/");

                    bbStoreCss.AppendLine(newline);
                }
            }
            bbStoreCss.AppendLine(_endString);

            // Fill in the new CSS
            cssContent = cssContent.Replace("[BBSTORECSS]", bbStoreCss.ToString());
            File.WriteAllText(portalCssFile, cssContent);
        }

        private void ResetCss()
        {
            string portalCssFile = PortalSettings.HomeDirectoryMapPath + "portal.css";
            string cssContent = "";
            if (File.Exists(portalCssFile))
            {
                // we need to cut off the old CSS
                cssContent = File.ReadAllText(portalCssFile);
                int Start = cssContent.IndexOf(_startString);
                int End = cssContent.IndexOf(_endString);
                if (Start > 0 && End > 0 && Start < End)
                {
                    cssContent = cssContent.Substring(0, Start) + cssContent.Substring(End + _endString.Length);
                    File.WriteAllText(portalCssFile, cssContent);
                }
            }
        }

		private ListItemCollection GetRoles()
		{
			// Lets populate thbe Usergroups into Collection
			RoleController roleController = new RoleController();
			ArrayList colArrayList = roleController.GetPortalRoles(PortalId);

			// Create a ListItemCollection to hold the Roles 
			ListItemCollection colRoles = new ListItemCollection();

			// Add all defined Roles to the List 
			foreach (RoleInfo Role in colArrayList)
			{
				ListItem roleListItem = new ListItem();
				roleListItem.Text = Role.RoleName;
				roleListItem.Value = Role.RoleID.ToString();
				colRoles.Add(roleListItem);
			}
			ListItem selectRoleListItem = new ListItem();
			selectRoleListItem.Text = Localization.GetString("SelectRole.Text", this.LocalResourceFile);
			selectRoleListItem.Value = "-1";
			colRoles.Insert(0, selectRoleListItem);
			return colRoles;
		}

        private void RefreshLicense(bool forceUpdate)
        {
            LicenseDataInfo license = new BBStoreController().GetLicense(PortalId, forceUpdate);
            if (license != null)
            {
                string modules = "<ul style=\"line-height:16px;padding:0;margin-left:0;font-weight:normal;\">";
                ModuleKindEnum licenseModuleKindEnum = (ModuleKindEnum) license.Modules;
                if ((licenseModuleKindEnum & ModuleKindEnum.Admin) == ModuleKindEnum.Admin)
                    modules += "<li style=\"list-style-type:none;\">Admin</li>";
                if ((licenseModuleKindEnum & ModuleKindEnum.Cart) == ModuleKindEnum.Cart)
                    modules += "<li style=\"list-style-type:none;\">Cart</li>";
                if ((licenseModuleKindEnum & ModuleKindEnum.Contact) == ModuleKindEnum.Contact)
                    modules += "<li style=\"list-style-type:none;\">Contact</li>";
                if ((licenseModuleKindEnum & ModuleKindEnum.FeatureList) == ModuleKindEnum.FeatureList)
                    modules += "<li style=\"list-style-type:none;\">FeatureList</li>";
                if ((licenseModuleKindEnum & ModuleKindEnum.MiniCart) == ModuleKindEnum.MiniCart)
                    modules += "<li style=\"list-style-type:none;\">MiniCart</li>";
                if ((licenseModuleKindEnum & ModuleKindEnum.Product) == ModuleKindEnum.Product)
                    modules += "<li style=\"list-style-type:none;\">Product</li>";
                if ((licenseModuleKindEnum & ModuleKindEnum.ProductGroup) == ModuleKindEnum.ProductGroup)
                    modules += "<li style=\"list-style-type:none;\">ProductGroup</li>";
                if ((licenseModuleKindEnum & ModuleKindEnum.ProductList) == ModuleKindEnum.ProductList)
                    modules += "<li style=\"list-style-type:none;\">ProductList</li>";
                if ((licenseModuleKindEnum & ModuleKindEnum.Search) == ModuleKindEnum.Search)
                    modules += "<li style=\"list-style-type:none;\">Search</li>";
                modules += "</ul>";

                ltrLicense.Text = String.Format("<table class=\"dnnGrid\" cellpadding=\"2\" cellspacing=\"0\" border=\"0\">"+
                    "<tr class=\"dnnGridItem\"><td style=\"vertical-align:top;font-weight:bold\">" + LocalizeString("lblTag.Header") + "</td><td>{0}</td></tr>" +
                    "<tr class=\"dnnGridAltItem\"><td style=\"vertical-align:top;font-weight:bold\">" + LocalizeString("lblVersion.Header") + "</td><td>{1}</td></tr>" +
                    "<tr class=\"dnnGridItem\"><td style=\"vertical-align:top;font-weight:bold\">" + LocalizeString("lblModules.Header") + "</td><td>{2}</td></tr>" +
                    "<tr class=\"dnnGridAltItem\"><td style=\"vertical-align:top;font-weight:bold\">" + LocalizeString("lblValid.Header") + "</td><td>{3}</td></tr>" +
                    "<tr class=\"dnnGridItem\"><td style=\"vertical-align:top;font-weight:bold\">" + LocalizeString("lblLastCheck.Header") + "</td><td>{4}</td></tr>" +
                    "</table>", license.Tag, license.Version.ToString() + ".x", modules,
                    (license.ValidUntil == null ? "-" : ((DateTime)license.ValidUntil).ToShortDateString()),
                    (ModuleSettings["LastLicenseRead"] ?? "-") + " (" + (ModuleSettings["LicenseReadRetries"] ?? "0") + ")");
            }
            else
            {
                ltrLicense.Text = "";
            }
        }
        
		#endregion
	}
}

