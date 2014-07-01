using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Bitboxx.License;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;


namespace Bitboxx.DNNModules.BBStore
{
    [DNNtc.PackageProperties("BBStore MiniCart", 9, "BBStore MiniCart", "BBStore MiniCart", "BBStore.png", "Torsten Weggen", "bitboxx solutions", "http://www.bitboxx.net", "info@bitboxx.net", false)]
    [DNNtc.ModuleProperties("BBStore MiniCart", "BBStore MiniCart", 0)]
    [DNNtc.ModuleControlProperties("", "BBStore MiniCart", DNNtc.ControlType.View, "", true, false)]
	public partial class ViewMiniCart : PortalModuleBase
	{
        #region Private Members
      
        private BBStoreController _controller;

        #endregion

        #region Public Properties
       
        public Guid CartId
        {
            get
            {
                string _cartId;
                if (Request.Cookies["BBStoreCartId_" + PortalSettings.PortalId.ToString()] != null)
                    _cartId = (string)(Request.Cookies["BBStoreCartId_" + PortalSettings.PortalId.ToString()].Value);
                else
                {
                    _cartId = Guid.NewGuid().ToString();
                }
                return new Guid(_cartId);
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

        public ModuleKindEnum ModuleKind
        {
            get { return ModuleKindEnum.MiniCart; }
        }
        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                CartInfo myCart = Controller.GetCart(PortalSettings.PortalId, CartId);
                if (myCart != null)
                {
                    List<CartProductInfo> myProducts = _controller.GetCartProducts(CartId);

                    TemplateControl tp = LoadControl("controls/TemplateControl.ascx") as TemplateControl;
                    tp.Key = "MiniCart";
                    string template = tp.GetTemplate((string)Settings["Template"]);

                    ModuleController objModules = new ModuleController();
                    ModuleInfo cartModule = objModules.GetModuleByDefinition(PortalSettings.PortalId, "BBStore Cart");

                    Hashtable storeSettings = _controller.GetStoreSettings(PortalSettings.PortalId);
                    bool showNetPrice = (storeSettings["ShowNetpriceInCart"].ToString() == "0");
                    decimal total = myCart.OrderTotal + myCart.AdditionalTotal;

                    if (showNetPrice == false)
                        total += myCart.OrderTax + myCart.AdditionalTax;

                    decimal productCount = 0;
                    foreach (CartProductInfo cp in myProducts)
                    {
                        productCount += cp.Quantity;
                    }
                    template = template.Replace("[PRODUCTS]", productCount.ToString("f0"));
                    template = template.Replace("[TOTAL]", total.ToString("f2"));
                    template = template.Replace("[CURRENCY]", myCart.Currency);
                    template = template.Replace("[CARTLINK]", (cartModule == null ? "" : Globals.NavigateURL(cartModule.TabID)));
                    template = template.Replace("[CHECKOUTLINK]", (cartModule == null ? "" : Globals.NavigateURL(cartModule.TabID, "", "action=checkout")));

                    ltrMiniCart.Text = template;
                }
            }
            catch (Exception exc)
			{
				//Module failed to load 
				Exceptions.ProcessModuleLoadException(this, exc);
			}
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {
            // Check licensing
            LicenseDataInfo license = Controller.GetLicense(PortalId, false);
            Controller.CheckLicense(license, this, ModuleKind);
        }
	}
}