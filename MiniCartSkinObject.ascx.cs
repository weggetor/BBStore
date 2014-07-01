using System;
using System.Collections.Generic;
using DotNetNuke.UI.Skins;
using DotNetNuke.Services.Localization;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Common;
using System.Collections;

namespace Bitboxx.DNNModules.BBStore
{
    public partial class MiniCartSkinObject : SkinObjectBase
    {
        #region Private Members
		string _itemTemplate = "[PRODUCTS]<br />[TOTAL]<br />[CARTLINK]&nbsp;[CHECKOUTLINK]";
    	private BBStoreController _controller;

    	#endregion

        #region Public Properties
        public string ItemTemplate
        {
			get { return _itemTemplate; }
			set { _itemTemplate = value; }
        }
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
        #endregion

		public override void RenderControl(System.Web.UI.HtmlTextWriter writer)
		{
			ShowMiniCart();
			base.RenderControl(writer);
		}
		#region Helper methods
		
		private void ShowMiniCart()
        {
            _controller = new BBStoreController();

			CartInfo myCart = _controller.GetCart(PortalSettings.PortalId,CartId);
			if (myCart != null)
			{
				List<CartProductInfo> myProducts = _controller.GetCartProducts(CartId);

				string template = _itemTemplate;

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
		#endregion

	}
}