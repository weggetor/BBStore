// 
// DotNetNuke® - http://www.dotnetnuke.com 
// Copyright (c) 2002-2010 
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
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;

namespace Bitboxx.DNNModules.BBStore
{

    /// ----------------------------------------------------------------------------- 
    /// <summary> 
    /// The EditSampleModule class is used to manage content 
    /// </summary> 
    /// <remarks> 
    /// </remarks> 
    /// <history> 
    /// </history> 
    /// ----------------------------------------------------------------------------- 
    partial class EditUnit : PortalModuleBase
    {

        #region "Private Members"

        private BBStoreController Controller;

        #endregion

        #region "Public Properties"

        public int UnitId
        {
            get
            {
                if (ViewState["UnitId"] != null)
                    return (int) ViewState["UnitId"];
                else
                    return -1;
            }
            set { ViewState["UnitId"] = value; }
        }

        protected string CurrentLanguage
        {
            get { return System.Threading.Thread.CurrentThread.CurrentCulture.Name; }
        }

        protected string DefaultLanguage
        {
            get { return this.PortalSettings.DefaultLanguage; }
        }

        #endregion

        #region "Event Handlers"

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            string FileName = System.IO.Path.GetFileNameWithoutExtension(this.AppRelativeVirtualPath);
            if (this.ID != null)
                //this will fix it when its placed as a ChildUserControl 
                this.LocalResourceFile = this.LocalResourceFile.Replace(this.ID, FileName);
            else
                // this will fix it when its dynamically loaded using LoadControl method 
                this.LocalResourceFile = this.LocalResourceFile + FileName + ".ascx.resx";
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                Controller = new BBStoreController();

                LocaleController lc = new LocaleController();
                Dictionary<string, Locale> loc = lc.GetLocales(PortalId);

                ModuleController objModules = new ModuleController();

                // If this is the first visit to the page 
                if (Page.IsPostBack == false)
                {

                    UnitInfo unit = null;

                    if (Request["unitid"] != null)
                        UnitId = Convert.ToInt32(Request["unitid"]);

                    // if unit exists
                    if (UnitId > 0)
                        unit = Controller.GetUnit(UnitId);

                    List<ILanguageEditorInfo> dbLangs = new List<ILanguageEditorInfo>();
                    if (unit == null)
                    {
                        txtDecimals.Text = "0";
                        foreach (KeyValuePair<string, Locale> keyValuePair in loc)
                        {
                            UnitLangInfo unitLang = new UnitLangInfo();
                            unitLang.Language = keyValuePair.Key;
                            dbLangs.Add(unitLang);
                        }
                    }
                    else
                    {
                        txtDecimals.Text = unit.Decimals.ToString();
                        foreach (UnitLangInfo unitLang in Controller.GetUnitLangs(UnitId))
                        {
                            dbLangs.Add(unitLang);
                        }
                    }
                    lngUnits.Langs = dbLangs;
                }
            }

            catch (Exception exc)
            {
                //Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdCancel_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> addParams = new List<string>();
                if (!String.IsNullOrEmpty(Request.QueryString["adminmode"]))
                    addParams.Add("adminmode=unitlist");

                if (!String.IsNullOrEmpty(Request.QueryString["unitid"]))
                    addParams.Add("unitid=" + Request.QueryString["unitid"]);

                if (addParams.Count > 0)
                    Response.Redirect(Globals.NavigateURL(TabId, "", addParams.ToArray()), true);
                else
                    Response.Redirect(Globals.NavigateURL(), true);
            }
            catch (Exception exc)
            {
                //Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                // First lets save the product
                UnitInfo unit = null;
                bool isNew = false;

                if (UnitId >= 0)
                    unit = Controller.GetUnit(UnitId);
                else
                    isNew = true;

                if (unit != null)
                {
                    unit.Decimals = Convert.ToInt32(txtDecimals.Text);
                    Controller.UpdateUnit(unit);
                }
                else
                {
                    unit = new UnitInfo();
                    unit.PortalId = PortalId;
                    unit.Decimals = Convert.ToInt32(txtDecimals.Text);
                    UnitId = Controller.NewUnit(unit);
                }

                // Now lets update Language information
                lngUnits.UpdateLangs();
                Controller.DeleteUnitLangs(UnitId);
                foreach (UnitLangInfo ul in lngUnits.Langs)
                {
                    ul.UnitId = UnitId;
                    Controller.NewUnitLang(ul);
                }

                List<string> addParams = new List<string>();

                if (Request["adminmode"] != null)
                    addParams.Add("adminmode=unitlist");
                addParams.Add("unitId=" + UnitId.ToString());

                Response.Redirect(Globals.NavigateURL(TabId, "", addParams.ToArray()), true);

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
