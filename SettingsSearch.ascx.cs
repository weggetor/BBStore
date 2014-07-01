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
    [DNNtc.PackageProperties("BBStore Product Search")]
    [DNNtc.ModuleProperties("BBStore Product Search")]
    [DNNtc.ModuleControlProperties("Settings", "BBStore Search Settings", DNNtc.ControlType.Edit, "", true, false)]
    partial class SettingsSearch : ModuleSettingsBase
    {
        #region "Base Method Implementations"
        public override void LoadSettings()
        {
            try
            {
				if (ModuleSettings["ResetSearchEnabled"] != null)
					chkResetSearchEnabled.Checked = Convert.ToBoolean(ModuleSettings["ResetSearchEnabled"]);
				else
					chkResetSearchEnabled.Checked = true;

				if (ModuleSettings["ResetSearchPGEnabled"] != null)
					chkResetSearchPGEnabled.Checked = Convert.ToBoolean(ModuleSettings["ResetSearchPGEnabled"]);
				else
					chkResetSearchPGEnabled.Checked = true;

				if (ModuleSettings["ProductGroupSearchEnabled"] != null)
					chkProductGroupSearchEnabled.Checked = Convert.ToBoolean(ModuleSettings["ProductGroupSearchEnabled"]);
				else
					chkProductGroupSearchEnabled.Checked = true;

				if (ModuleSettings["TextSearchEnabled"] != null)
					chkTextSearchEnabled.Checked = Convert.ToBoolean(ModuleSettings["TextSearchEnabled"]);
				else
					chkTextSearchEnabled.Checked = true;

				if (ModuleSettings["StaticSearchEnabled"] != null)
					chkStaticSearchEnabled.Checked = Convert.ToBoolean(ModuleSettings["StaticSearchEnabled"]);
				else
					chkStaticSearchEnabled.Checked = true;

				if (ModuleSettings["PriceSearchEnabled"] != null)
					chkPriceSearchEnabled.Checked = Convert.ToBoolean(ModuleSettings["PriceSearchEnabled"]);
				else
					chkPriceSearchEnabled.Checked = true;

				if (ModuleSettings["FeatureSearchEnabled"] != null)
					chkFeatureSearchEnabled.Checked = Convert.ToBoolean(ModuleSettings["FeatureSearchEnabled"]);
				else
					chkFeatureSearchEnabled.Checked = true;

				if (ModuleSettings["DynamicPage"] != null)
					urlSelectDynamicPage.Url = (string)ModuleSettings["DynamicPage"];
				else
					urlSelectDynamicPage.Url = TabId.ToString();

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
				objModules.UpdateModuleSetting(ModuleId, "ResetSearchEnabled", chkResetSearchEnabled.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "ResetSearchPGEnabled", chkResetSearchPGEnabled.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "ProductGroupSearchEnabled", chkProductGroupSearchEnabled.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "TextSearchEnabled", chkTextSearchEnabled.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "StaticSearchEnabled", chkStaticSearchEnabled.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "PriceSearchEnabled", chkPriceSearchEnabled.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "FeatureSearchEnabled", chkFeatureSearchEnabled.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "DynamicPage", urlSelectDynamicPage.Url);
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

