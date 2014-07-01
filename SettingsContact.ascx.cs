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
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;

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
    [DNNtc.PackageProperties("BBStore Contact")]
    [DNNtc.ModuleProperties("BBStore Contact")]
    [DNNtc.ModuleControlProperties("Settings", "BBStore Contact Settings", DNNtc.ControlType.Edit, "", true, false)]
    partial class SettingsContact : ModuleSettingsBase
    {
        protected override void OnLoad(EventArgs e)
        {
            tplTemplate.CaptionText = LocalizeString("lblTemplate.Text");
            tplTemplate.CaptionHelp = LocalizeString("lblTemplate.Help");

            tplProduct.CaptionText = LocalizeString("lblProductTemplate.Text");
            tplProduct.CaptionHelp = LocalizeString("lblProductTemplate.Help");
            base.OnLoad(e);
        }
        
        #region "Base Method Implementations"
        public override void LoadSettings()
        {
            try
            {
				chkShowCompany.Checked = true;
				if (ModuleSettings["ShowCompany"] != null)
					chkShowCompany.Checked = Convert.ToBoolean(ModuleSettings["ShowCompany"]);
				chkMandCompany.Checked = false;
				if (ModuleSettings["MandCompany"] != null)
					chkMandCompany.Checked = Convert.ToBoolean(ModuleSettings["MandCompany"]);

				chkShowPrefix.Checked = true;
				if (ModuleSettings["ShowPrefix"] != null)
					chkShowPrefix.Checked = Convert.ToBoolean(ModuleSettings["ShowPrefix"]);
				chkMandPrefix.Checked = false;
				if (ModuleSettings["MandPrefix"] != null)
					chkMandPrefix.Checked = Convert.ToBoolean(ModuleSettings["MandPrefix"]);

				chkShowFirstname.Checked = true;
				if (ModuleSettings["ShowFirstname"] != null)
					chkShowFirstname.Checked = Convert.ToBoolean(ModuleSettings["ShowFirstname"]);
				chkMandFirstname.Checked = false;
				if (ModuleSettings["MandFirstname"] != null)
					chkMandFirstname.Checked = Convert.ToBoolean(ModuleSettings["MandFirstname"]);

				chkShowLastname.Checked = true;
				if (ModuleSettings["ShowLastname"] != null)
					chkShowLastname.Checked = Convert.ToBoolean(ModuleSettings["ShowLastname"]);
				chkMandLastname.Checked = false;
				if (ModuleSettings["MandLastname"] != null)
					chkMandLastname.Checked = Convert.ToBoolean(ModuleSettings["MandLastname"]);

				chkShowStreet.Checked = true;
				if (ModuleSettings["ShowStreet"] != null)
					chkShowStreet.Checked = Convert.ToBoolean(ModuleSettings["ShowStreet"]);
				chkMandStreet.Checked = false;
				if (ModuleSettings["MandStreet"] != null)
					chkMandStreet.Checked = Convert.ToBoolean(ModuleSettings["MandStreet"]);

				chkShowRegion.Checked = true;
				if (ModuleSettings["ShowRegion"] != null)
					chkShowRegion.Checked = Convert.ToBoolean(ModuleSettings["ShowRegion"]);
				chkMandRegion.Checked = false;
				if (ModuleSettings["MandRegion"] != null)
					chkMandRegion.Checked = Convert.ToBoolean(ModuleSettings["MandRegion"]);

				chkShowCity.Checked = true;
				if (ModuleSettings["ShowCity"] != null)
					chkShowCity.Checked = Convert.ToBoolean(ModuleSettings["ShowCity"]);
				chkMandCity.Checked = false;
				if (ModuleSettings["MandCity"] != null)
					chkMandCity.Checked = Convert.ToBoolean(ModuleSettings["MandCity"]);

				chkShowCountry.Checked = true;
				if (ModuleSettings["ShowCountry"] != null)
					chkShowCountry.Checked = Convert.ToBoolean(ModuleSettings["ShowCountry"]);
				chkMandCountry.Checked = false;
				if (ModuleSettings["MandCountry"] != null)
					chkMandCountry.Checked = Convert.ToBoolean(ModuleSettings["MandCountry"]);

				chkShowPhone.Checked = true;
				if (ModuleSettings["ShowPhone"] != null)
					chkShowPhone.Checked = Convert.ToBoolean(ModuleSettings["ShowPhone"]);
				chkMandPhone.Checked = false;
				if (ModuleSettings["MandPhone"] != null)
					chkMandPhone.Checked = Convert.ToBoolean(ModuleSettings["MandPhone"]);

				chkShowCell.Checked = true;
				if (ModuleSettings["ShowCell"] != null)
					chkShowCell.Checked = Convert.ToBoolean(ModuleSettings["ShowCell"]);
				chkMandCell.Checked = false;
				if (ModuleSettings["MandCell"] != null)
					chkMandCell.Checked = Convert.ToBoolean(ModuleSettings["MandCell"]);

				chkShowFax.Checked = true;
				if (ModuleSettings["ShowFax"] != null)
					chkShowFax.Checked = Convert.ToBoolean(ModuleSettings["ShowFax"]);
				chkMandFax.Checked = false;
				if (ModuleSettings["MandFax"] != null)
					chkMandFax.Checked = Convert.ToBoolean(ModuleSettings["MandFax"]);

				chkShowEmail.Checked = true;
				if (ModuleSettings["ShowEmail"] != null)
					chkShowEmail.Checked = Convert.ToBoolean(ModuleSettings["ShowEmail"]);
				chkMandEmail.Checked = false;
				if (ModuleSettings["MandEmail"] != null)
					chkMandEmail.Checked = Convert.ToBoolean(ModuleSettings["MandEmail"]);

				if (ModuleSettings["ShopHome"] != null)
					urlShopHome.Url = (string)ModuleSettings["ShopHome"];

				if (ModuleSettings["EmailSubject"] != null)
					txtEmailSubject.Text = (string)ModuleSettings["EmailSubject"];

                if (ModuleSettings["Template"] != null) 
                    tplTemplate.Value = (string)ModuleSettings["Template"];

                if (ModuleSettings["ProductTemplate"] != null)
                    tplProduct.Value = (string)ModuleSettings["ProductTemplate"];
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
				objModules.UpdateModuleSetting(ModuleId, "ShowCompany", chkShowCompany.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "MandCompany", chkMandCompany.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "ShowPrefix", chkShowPrefix.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "MandPrefix", chkMandPrefix.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "ShowFirstname", chkShowFirstname.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "MandFirstname", chkMandFirstname.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "ShowLastname", chkShowLastname.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "MandLastname", chkMandLastname.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "ShowStreet", chkShowStreet.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "MandStreet", chkMandStreet.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "ShowRegion", chkShowRegion.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "MandRegion", chkMandRegion.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "ShowCity", chkShowCity.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "MandCity", chkMandCity.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "ShowCountry", chkShowCountry.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "MandCountry", chkMandCountry.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "ShowPhone", chkShowPhone.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "MandPhone", chkMandPhone.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "ShowCell", chkShowCell.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "MandCell", chkMandCell.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "ShowFax", chkShowFax.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "MandFax", chkMandFax.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "ShowEmail", chkShowEmail.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "MandEmail", chkMandEmail.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "ShopHome", urlShopHome.Url);
				objModules.UpdateModuleSetting(ModuleId, "EmailSubject", txtEmailSubject.Text);
                objModules.UpdateModuleSetting(ModuleId, "Template", tplTemplate.Value);
                objModules.UpdateModuleSetting(ModuleId, "ProductTemplate", tplProduct.Value);
            }
            catch (Exception exc)
            {
                //Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        #endregion
     }
}

