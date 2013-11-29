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

using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using System;
using System.Collections;

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
    partial class ViewAdminShipping : PortalModuleBase
    {
        #region Private Members

        private const string Currency = "EUR";
        private BBStoreController _controller;

        #endregion

        #region Public Properties

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
            get { return System.Threading.Thread.CurrentThread.CurrentCulture.Name; }
        }

        protected string DefaultLanguage
        {
            get { return this.PortalSettings.DefaultLanguage; }
        }

        #endregion

        #region Event Handlers

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
                if (!IsPostBack)
                {
                    LoadSettings();
                }
            }
            catch (Exception exc)
            {
                //Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdUpdate_Click(object sender, EventArgs e)
        {
            UpdateSettings();
            Response.Redirect(Globals.NavigateURL(), true);
        }

        protected void cmdCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(Globals.NavigateURL(), true);
        }

        #endregion

        #region Helper Methods
        public void LoadSettings()
        {
            try
            {
                if (!IsPostBack)
                {
                    Hashtable settings = Controller.GetStoreSettings(PortalId);
                    bool showNetPrice = ((string)settings["ShowNetpriceInCart"] == "0");

                    // Update 1.09 -> 1.0.10 , Settings from cart -> admin
                    if (settings["ShippingType"] == null)
                    {
                        ModuleController moduleController = new ModuleController();
                        ModuleInfo cartModule = moduleController.GetModuleByDefinition(PortalId, "BBStore Cart");
                        if (cartModule != null)
                            settings = moduleController.GetModuleSettings(cartModule.ModuleID);
                    }

                    if (settings["ShippingTax"] != null)
                        txtShippingTax.Text = (string)settings["ShippingTax"];
                    else
                        txtShippingTax.Text = 0.0m.ToString((System.Globalization.CultureInfo.InvariantCulture));
                    decimal shippingTax = 0.0m;
                    Decimal.TryParse(txtShippingTax.Text.Replace(",", "."), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out shippingTax);

                    // National Shipping
                    txtShippingCost.PercentControl = txtShippingTax;
                    txtShippingCost.Mode = (showNetPrice ? "net" : "gross");
                    txtShippingFree.PercentControl = txtShippingTax;
                    txtShippingFree.Mode = (showNetPrice ? "net" : "gross");

                    if (settings["ShippingCost"] != null)
                    {
                        string strShippingCost = ((string)settings["ShippingCost"]).Replace(",", ".");
                        decimal shippingCost = 0.0000m;
                        Decimal.TryParse(strShippingCost, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out shippingCost);
                        txtShippingCost.Value = shippingCost;
                    }
                    else
                        txtShippingCost.Value = 0.0000m;

                    if (settings["ShippingFree"] != null)
                    {
                        string strShippingFree = ((string)settings["ShippingFree"]).Replace(",", ".");
                        decimal shippingFree = 0.0000m;
                        Decimal.TryParse(strShippingFree, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out shippingFree);
                        txtShippingFree.Value = shippingFree;
                    }
                    else
                        txtShippingFree.Value = 0.0000m;

                    if (settings["ShippingType"] != null)
                        txtShippingType.Text = (string)settings["ShippingType"];

                    // International

                    txtShippingCostInt.PercentControl = txtShippingTax;
                    txtShippingCostInt.Mode = (showNetPrice ? "net" : "gross");
                    txtShippingFreeInt.PercentControl = txtShippingTax;
                    txtShippingFreeInt.Mode = (showNetPrice ? "net" : "gross");

                    if (settings["ShippingCostInt"] != null)
                    {
                        string strShippingCostInt = ((string)settings["ShippingCostInt"]).Replace(",", ".");
                        decimal shippingCostInt = 0.0000m;
                        Decimal.TryParse(strShippingCostInt, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out shippingCostInt);
                        txtShippingCostInt.Value = shippingCostInt;
                    }
                    else
                        txtShippingCostInt.Value = 0.0000m;

                    if (settings["ShippingFreeInt"] != null)
                    {
                        string strShippingFreeInt = ((string)settings["ShippingFreeInt"]).Replace(",", ".");
                        decimal shippingFreeInt = 0.0000m;
                        Decimal.TryParse(strShippingFreeInt, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out shippingFreeInt);
                        txtShippingFreeInt.Value = shippingFreeInt;
                    }
                    else
                        txtShippingFreeInt.Value = 0.0000m;

                    if (settings["ShippingTypeInt"] != null)
                        txtShippingTypeInt.Text = (string)settings["ShippingTypeInt"];

                }
            }
            catch (Exception exc)
            {
                //Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        public void UpdateSettings()
        {
            try
            {
                ModuleController objModules = new ModuleController();

                objModules.UpdateModuleSetting(ModuleId, "ShippingTax", txtShippingTax.Text.Trim());
                objModules.UpdateModuleSetting(ModuleId, "ShippingCost", txtShippingCost.NetPrice.ToString());
                objModules.UpdateModuleSetting(ModuleId, "ShippingFree", txtShippingFree.NetPrice.ToString());
                objModules.UpdateModuleSetting(ModuleId, "ShippingType", txtShippingType.Text.Trim());
                objModules.UpdateModuleSetting(ModuleId, "ShippingCostInt", txtShippingCostInt.NetPrice.ToString());
                objModules.UpdateModuleSetting(ModuleId, "ShippingFreeInt", txtShippingFreeInt.NetPrice.ToString());
                objModules.UpdateModuleSetting(ModuleId, "ShippingTypeInt", txtShippingTypeInt.Text.Trim());
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