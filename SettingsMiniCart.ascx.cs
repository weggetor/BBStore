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
using System.Text;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
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
    [DNNtc.PackageProperties("BBStore MiniCart")]
    [DNNtc.ModuleProperties("BBStore MiniCart")]
    [DNNtc.ModuleControlProperties("Settings", "BBStore MiniCart Settings", DNNtc.ControlType.Edit, "", true, false)]
    partial class SettingsMiniCart : ModuleSettingsBase
    {
        protected override void OnLoad(EventArgs e)
        {
            tplTemplate.CaptionText = LocalizeString("lblTemplate.Text");
            tplTemplate.CaptionHelp = LocalizeString("lblTemplate.Help");
            tplTemplate.CreateImageCallback = CreateThumbHtml;
            base.OnLoad(e);
        }

        #region "Base Method Implementations"
        public override void LoadSettings()
        {
            try
            {
                if (ModuleSettings["Template"] != null) 
                    tplTemplate.Value = (string)ModuleSettings["Template"];
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
                objModules.UpdateModuleSetting(ModuleId, "Template", tplTemplate.Value);
            }
            catch (Exception exc)
            {
                //Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        #endregion

        #region Helper methods

        private string CreateThumbHtml(string template)
        {
            StringBuilder sb = new StringBuilder(template);

            ModuleController objModules = new ModuleController();
            ModuleInfo cartModule = objModules.GetModuleByDefinition(PortalSettings.PortalId, "BBStore Cart");

            sb.Replace("[PRODUCTS]", 3.00m.ToString("f0"));
            sb.Replace("[TOTAL]", 13.5438m.ToString("f2"));
            sb.Replace("[CURRENCY]", "USD");
            sb.Replace("[CARTLINK]", (cartModule == null ? "" : Globals.NavigateURL(cartModule.TabID)));
            sb.Replace("[CHECKOUTLINK]", (cartModule == null ? "" : Globals.NavigateURL(cartModule.TabID, "", "action=checkout")));
            return sb.ToString();
        }

        #endregion
    }
}

