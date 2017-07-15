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

using Bitboxx.License;
using DotNetNuke.Application;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Definitions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Installer.Packages;
using DotNetNuke.Services.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;
using DotNetNuke.Services.Log.EventLog;

namespace Bitboxx.DNNModules.BBStore
{

    /// ----------------------------------------------------------------------------- 
    /// <summary> 
    /// The Controller class for BBStore 
    /// </summary> 
    /// <remarks> 
    /// </remarks> 
    /// <history> 
    /// </history> 
    /// ----------------------------------------------------------------------------- 
    [DNNtc.BusinessControllerClass()]
    public class BBStoreController 
    {
        #region Properties

        private static Guid _storeGuid;
        public static Guid StoreGuid 
        {
            get
            {
                return PortalSettings.Current.GUID;
                if (_storeGuid == Guid.Empty)
                {
                    int portalId = PortalSettings.Current.PortalId;
                    ModuleController mc = new ModuleController();
                    ModuleInfo adminModule = mc.GetModuleByDefinition(portalId, "BBStore Admin");
                    Hashtable storeSettings = mc.GetModuleSettings(adminModule.ModuleID);
                    
                    if (storeSettings["StoreId"] != null)
                    {
                        _storeGuid = new Guid((string)storeSettings["StoreId"]);
                    }
                    else
                    {
                        _storeGuid = Guid.NewGuid();
                        mc.UpdateModuleSetting(adminModule.ModuleID,"StoreId",_storeGuid.ToString());
                    }
                }
                return _storeGuid;
            }
            //set
            //{
            //    _storeGuid = value;
            //    int portalId = PortalSettings.Current.PortalId;
            //    ModuleController mc = new ModuleController();
            //    ModuleInfo adminModule = mc.GetModuleByDefinition(portalId, "BBStore Admin");
            //    mc.UpdateModuleSetting(adminModule.ModuleID, "StoreId", _storeGuid.ToString());
            //}
        }

        public static string StoreName
        {
            get
            {
                int portalId = PortalSettings.Current.PortalId;
                ModuleController mc = new ModuleController();
                ModuleInfo adminModule = mc.GetModuleByDefinition(portalId, "BBStore Admin");
                Hashtable storeSettings = mc.GetModuleSettings(adminModule.ModuleID);

                if (storeSettings["VendorName"] != null)
                    return (string) storeSettings["VendorName"];
                else
                    return "";
            }
        }

        // TODO: Wo kommt die PortalId her ?
        //public static Guid FilterSessionId
        //{
        //    get
        //    {
        //        string _filterSessionId;
                
        //        if (Request.Cookies["BBStoreFilterSessionId_" + PortalId.ToString()] != null)
        //            _filterSessionId = (string)(Request.Cookies["BBStoreFilterSessionId_" + PortalId.ToString()].Value);
        //        else
        //        {
        //            _filterSessionId = Guid.NewGuid().ToString();
        //            HttpCookie keks = new HttpCookie("BBStoreFilterSessionId_" + PortalId.ToString());
        //            keks.Value = _filterSessionId;
        //            keks.Expires = DateTime.Now.AddDays(1);
        //            Response.AppendCookie(keks);
        //        }
        //        return new Guid(_filterSessionId);
        //    }
        //}

        #endregion

        public BBStoreController()
        {
            CheckCss("999999", "000084", "EEEEEE", "DCDCDC", "A0AFEB");
        }

        #region Public Methods

        #region SimpleProduct methods
        public List<SimpleProductInfo> GetSimpleProducts(int PortalId)
        {
            return CBO.FillCollection<SimpleProductInfo>(DataProvider.Instance().GetSimpleProducts(PortalId));
        }

        public List<SimpleProductInfo> GetSimpleProductsStandardPrice(int PortalId, string Language, string Sort, string Where)
        {
            return CBO.FillCollection<SimpleProductInfo>(DataProvider.Instance().GetSimpleProductsStandardPrice(PortalId, Language, Sort, Where));
        }
        public List<SimpleProductInfo> GetSimpleProducts(int PortalId, string Language, string Sort, string Where, int Top, int userId, bool extendedPrice)
        {
            return CBO.FillCollection<SimpleProductInfo>(DataProvider.Instance().GetSimpleProducts(PortalId, Language, Sort, Where, Top, userId, extendedPrice));
        }
        public SimpleProductInfo GetSimpleProductByProductId(int PortalId, int ProductId)
        {
            return (SimpleProductInfo)CBO.FillObject(DataProvider.Instance().GetSimpleProductByProductId(PortalId, ProductId), typeof(SimpleProductInfo));
        }
        public SimpleProductInfo GetSimpleProductByProductId(int PortalId, int ProductId, string Language, int userId, bool extendedPrice)
        {
            return (SimpleProductInfo)CBO.FillObject(DataProvider.Instance().GetSimpleProductByProductId(PortalId, ProductId, Language, userId, extendedPrice), typeof(SimpleProductInfo));
        }
        public SimpleProductInfo GetSimpleProductByModuleId(int PortalId, int ModuleId)
        {
            return (SimpleProductInfo)CBO.FillObject(DataProvider.Instance().GetSimpleProductByModuleId(PortalId, ModuleId), typeof(SimpleProductInfo));
        }
        public SimpleProductInfo GetSimpleProductByModuleId(int PortalId, int ModuleId, string Language)
        {
            return (SimpleProductInfo)CBO.FillObject(DataProvider.Instance().GetSimpleProductByModuleId(PortalId, ModuleId, Language), typeof(SimpleProductInfo));
        }
        public int NewSimpleProduct(SimpleProductInfo SimpleProduct)
        {
            return DataProvider.Instance().NewSimpleProduct(SimpleProduct);
        }
        public void UpdateSimpleProduct(SimpleProductInfo SimpleProduct)
        {
            DataProvider.Instance().UpdateSimpleProduct(SimpleProduct);
        }
        public void DeleteSimpleProduct(int SimpleProductId)
        {
            DataProvider.Instance().DeleteSimpleProduct(SimpleProductId);
        }
        public void DeleteSimpleProducts(int PortalId)
        {
            DataProvider.Instance().DeleteSimpleProducts(PortalId);
        }

        #endregion

        #region ModuleProduct methods

        [Obsolete("GetModuleProduct is deprecated", false)]
        public ModuleProductInfo GetModuleProduct(int PortalId, int ModuleId)
        {
            return (ModuleProductInfo)CBO.FillObject(DataProvider.Instance().GetModuleProduct(PortalId, ModuleId), typeof(ModuleProductInfo));
        }
        [Obsolete("SetModuleProductId is deprecated", false)]
        public void SetModuleProductId(int PortalId, int ModuleId, int ProductId)
        {
            DataProvider.Instance().SetModuleProductId(PortalId, ModuleId, ProductId);
        }
        [Obsolete("GetModuleProductId is deprecated", false)]
        public int GetModuleProductId(int PortalId, int ModuleId)
        {
            return DataProvider.Instance().GetModuleProductId(PortalId, ModuleId);
        }
        [Obsolete("GetModuleProductModuleId is deprecated", false)]
        public int GetModuleProductModuleId(int PortalId, int ProductId)
        {
            return DataProvider.Instance().GetModuleProductModuleId(PortalId, ProductId);
        }
        [Obsolete("GetModuleProductTemplate is deprecated", false)]
        public string GetModuleProductTemplate(int PortalId, int ModuleId)
        {
            return DataProvider.Instance().GetModuleProductTemplate(PortalId, ModuleId);
        }
        [Obsolete("SetModuleProductTemplate is deprecated", false)]
        public void SetModuleProductTemplate(int PortalId, int ModuleId, int ProductTemplateId, string Template)
        {
            DataProvider.Instance().SetModuleProductTemplate(PortalId, ModuleId, ProductTemplateId, Template);
        }
        [Obsolete("NewModuleProduct is deprecated", false)]
        public void NewModuleProduct(ModuleProductInfo ModuleProduct)
        {
            DataProvider.Instance().NewModuleProduct(ModuleProduct);
        }
        [Obsolete("UpdateModuleProduct is deprecated", false)]
        public void UpdateModuleProduct(ModuleProductInfo ModuleProduct)
        {
            DataProvider.Instance().UpdateModuleProduct(ModuleProduct);
        }
        public List<ModuleProductInfo> GetModuleProducts(int portalId)
        {
            return CBO.FillCollection<ModuleProductInfo>(DataProvider.Instance().GetModuleProducts(portalId));
        }
        public void DeleteModuleProduct(int portalid, int moduleId)
        {
            DataProvider.Instance().DeleteModuleProduct(portalid, moduleId);
        }
        #endregion

        #region SimpleProductLang methods
        public List<SimpleProductLangInfo> GetSimpleProductLangs(int SimpleProductId)
        {
            return CBO.FillCollection<SimpleProductLangInfo>(DataProvider.Instance().GetSimpleProductLangs(SimpleProductId)); 
        }
        public List<SimpleProductLangInfo> GetSimpleProductLangsByPortal(int portalId)
        {
            return CBO.FillCollection<SimpleProductLangInfo>(DataProvider.Instance().GetSimpleProductLangsByPortal(portalId));
        }
        public SimpleProductLangInfo GetSimpleProductLang(int SimpleProductId,string Language)
        {
            return (SimpleProductLangInfo)CBO.FillObject(DataProvider.Instance().GetSimpleProductLang(SimpleProductId,Language),typeof(SimpleProductLangInfo));
        }
        public void NewSimpleProductLang(SimpleProductLangInfo SimpleProductLang)
        {
            DataProvider.Instance().NewSimpleProductLang(SimpleProductLang);
        }
        public void UpdateSimpleProductLang(SimpleProductLangInfo SimpleProductLang)
        {
            DataProvider.Instance().UpdateSimpleProductLang(SimpleProductLang);
        }
        public void DeleteSimpleProductLangs(int SimpleProductId)
        {
            DataProvider.Instance().DeleteSimpleProductLangs(SimpleProductId);
        }
        public void DeleteSimpleProductLang(int SimpleProductId, string Language)
        {
            DataProvider.Instance().DeleteSimpleProductLang(SimpleProductId, Language);
        }
        #endregion

        #region ProductPrice methods

        public ProductPriceInfo GetProductPriceById(int ProductPriceId)
        {
            return (ProductPriceInfo)CBO.FillObject(DataProvider.Instance().GetProductPriceById(ProductPriceId), typeof(ProductPriceInfo));
        }
        public List<ProductPriceInfo> GetProductPrices(int PortalId)
        {
            return CBO.FillCollection<ProductPriceInfo>(DataProvider.Instance().GetProductPrices(PortalId));
        }
        public List<ProductPriceInfo> GetProductPricesByProductId(int productId)
        {
            return CBO.FillCollection<ProductPriceInfo>(DataProvider.Instance().GetProductPricesByProductId(productId));
        }
        public int NewProductPrice(ProductPriceInfo ProductPrice)
        {
            return DataProvider.Instance().NewProductPrice(ProductPrice);
        }
        public void UpdateProductPrice(ProductPriceInfo ProductPrice)
        {
            DataProvider.Instance().UpdateProductPrice(ProductPrice);
        }
        public void DeleteProductPrice(int ProductPriceId)
        {
            DataProvider.Instance().DeleteProductPrice(ProductPriceId);
        }
        #endregion

        #region Customer methods

        public List<CustomerInfo> GetCustomersByUserId(int portalId, int userId)
        {
            return CBO.FillCollection<CustomerInfo>(DataProvider.Instance().GetCustomersByUserId(portalId,userId));
        }
        public List<CustomerInfo> GetCustomersByPortal(int portalId)
        {
            return CBO.FillCollection<CustomerInfo>(DataProvider.Instance().GetCustomersByPortal(portalId));
        }
        public List<CustomerInfo> GetCustomersByPortalAndUser(int portalId, int userId)
        {
            return CBO.FillCollection<CustomerInfo>(DataProvider.Instance().GetCustomersByPortalAndUser(portalId, userId));
        }
        public CustomerInfo GetCustomerById(int CustomerId)
        {
            return (CustomerInfo)CBO.FillObject(DataProvider.Instance().GetCustomerById(CustomerId), typeof(CustomerInfo));
        }
        public int NewCustomer(CustomerInfo Customer)
        {
            return DataProvider.Instance().NewCustomer(Customer);
        }
        public void UpdateCustomer(CustomerInfo Customer)
        {
            DataProvider.Instance().UpdateCustomer(Customer);
        }
        public int SaveCustomer(CustomerInfo Customer)
        {
            return DataProvider.Instance().SaveCustomer(Customer);
        }
        public void DeleteCustomer(int CustomerId)
        {
            DataProvider.Instance().DeleteCustomer(CustomerId);
        }
        #endregion

        #region CustomerAddress methods
        public CustomerAddressInfo GetCustomerAddress(int CustomerAddressId)
        {
            return (CustomerAddressInfo)CBO.FillObject(DataProvider.Instance().GetCustomerAddress(CustomerAddressId), typeof(CustomerAddressInfo));
        }
        public List<CustomerAddressInfo> GetCustomerAddresses(int CustomerId)
        {
            return CBO.FillCollection<CustomerAddressInfo>(DataProvider.Instance().GetCustomerAddresses(CustomerId));
        }
		public List<CustomerAddressInfo> GetCustomerAddressesByCart(Guid cartid, string language)
		{
			return CBO.FillCollection<CustomerAddressInfo>(DataProvider.Instance().GetCustomerAddressesByCart(cartid, language));
		}
        public CustomerAddressInfo GetCustomerMainAddress(int CustomerId)
        {
            return (CustomerAddressInfo)CBO.FillObject(DataProvider.Instance().GetCustomerMainAddress(CustomerId), typeof(CustomerAddressInfo));
        }
        public List<CustomerAddressInfo> GetCustomerAdditionalAddresses(int CustomerId)
        {
            return CBO.FillCollection<CustomerAddressInfo>(DataProvider.Instance().GetCustomerAdditionalAddresses(CustomerId));
        }
        public int NewCustomerAddress(CustomerAddressInfo CustomerAddress)
        {
            return DataProvider.Instance().NewCustomerAddress(CustomerAddress);
        }
        public void UpdateCustomerAddress(CustomerAddressInfo CustomerAddress)
        {
            DataProvider.Instance().UpdateCustomerAddress(CustomerAddress);
        }
        public void DeleteCustomerAddress(int CustomerAdressId)
        {
            DataProvider.Instance().DeleteCustomerAddress(CustomerAdressId);
        }
        #endregion

        #region Cart methods
        public CartInfo GetCart(int PortalId, Guid CartId, bool isTaxFree = false)
        {
            IDataReader dr = DataProvider.Instance().GetCart(CartId, isTaxFree);
            CartInfo Cart = (CartInfo)CBO.FillObject(dr, typeof(CartInfo));
            if (Cart != null)
                Cart.Total = Cart.AdditionalTotal + Cart.OrderTotal;
            return Cart;
        }
        public void NewCart(int PortalId, CartInfo Cart)
        {
            DataProvider.Instance().NewCart(PortalId, Cart);
        }
        public void UpdateCart(int PortalId, CartInfo Cart)
        {
            DataProvider.Instance().UpdateCart(PortalId, Cart);
        }
        public void DeleteCart(Guid CartId)
        {
            DataProvider.Instance().DeleteCart(CartId);
        }
        public void PurgeCarts(int portalId, int days)
        {
            DataProvider.Instance().PurgeCarts(portalId, days);
        }
        public List<TaxInfo> GetCartTax(int PortalId, Guid CartId)
        {
            return CBO.FillCollection<TaxInfo>(DataProvider.Instance().GetCartTax(PortalId, CartId));
        }
        public void UpdateCartCustomerId(Guid CartId, int CustomerId)
        {
            DataProvider.Instance().UpdateCartCustomerId(CartId, CustomerId);
        }
        public void UpdateCartCustomerPaymentProviderId(Guid CartId, int CustomerPaymentProviderId)
        {
            DataProvider.Instance().UpdateCartCustomerPaymentProviderId(CartId, CustomerPaymentProviderId);
        }
        public void UpdateCartCouponId(Guid cartId, int couponId)
        {
            DataProvider.Instance().UpdateCartCouponId(cartId, couponId);
        }
        public void UpdateCartTaxId(Guid cartId, string taxId)
        {
            DataProvider.Instance().UpdateCartTaxId(cartId, taxId);
        }
        public string SerializeCart(Guid cartId)
        {
            return DataProvider.Instance().SerializeCart(cartId);
        }
        public CartInfo DeserializeCart(int portalId, int userId, Guid cartId, string cartXml, bool extendedPrice)
        {
            return DataProvider.Instance().DeserializeCart(portalId, userId, cartId, cartXml, extendedPrice);
        }

        #endregion

        #region CartAddress methods
        public int GetCartAddressId(Guid cartid, string kzAddressType)
        {
            return DataProvider.Instance().GetCartAddressId(cartid, kzAddressType);
        }
		public List<CartAddressInfo> GetCartAddressByTypeId(Guid cartid, int subscriberAddressTypeId)
		{
			return CBO.FillCollection<CartAddressInfo>(DataProvider.Instance().GetCartAddressByTypeId(cartid, subscriberAddressTypeId));
		}
		public List<CartAddressInfo> GetCartAddressesByAddressId(Guid cartid, int customerAddressId)
		{
			return CBO.FillCollection<CartAddressInfo>(DataProvider.Instance().GetCartAddressesByAddressId(cartid, customerAddressId));
		}
        public  void UpdateCartAddressType(Guid cartId, int portalId, int subscriberId, int customerAddressId, string kzAddressType, bool set)
        {
            DataProvider.Instance().UpdateCartAddressType(cartId, portalId,subscriberId,customerAddressId,kzAddressType,set);
        }
		public void UpdateCartAddressType(Guid cartId, int portalId, int subscriberId, int customerAddressId, int subscriberAddressId, bool set)
		{
			DataProvider.Instance().UpdateCartAddressType(cartId, portalId, subscriberId, customerAddressId, subscriberAddressId, set);
		}
        public void NewCartAddress(CartAddressInfo cartAddress)
        {
            DataProvider.Instance().NewCartAddress(cartAddress);
        }
		public bool CheckCartAddresses(Guid CartId, int portalId, int subscriberId)
		{
			return DataProvider.Instance().CheckCartAddresses(CartId, portalId, subscriberId);
		}
        public int GetInvalidValidCartAddress(Guid CartId, int portalId)
        {
            Hashtable cartSettings = GetCartSettings(portalId);
            List<CartAddressInfo> adresses = GetCartAddresses(CartId);
            foreach (CartAddressInfo adr in adresses)
            {
                CustomerAddressInfo customerAddress = GetCustomerAddress(adr.CustomerAddressId);
                if (Convert.ToBoolean(cartSettings["MandCompany"]) && String.IsNullOrEmpty(customerAddress.Company))
                    return  adr.CustomerAddressId;
                if (Convert.ToBoolean(cartSettings["MandPrefix"]) && String.IsNullOrEmpty(customerAddress.Prefix))
                    return adr.CustomerAddressId;
                if (Convert.ToBoolean(cartSettings["MandFirstname"]) && String.IsNullOrEmpty(customerAddress.Firstname))
                    return adr.CustomerAddressId;
                if (Convert.ToBoolean(cartSettings["MandMiddlename"]) && String.IsNullOrEmpty(customerAddress.Middlename))
                    return adr.CustomerAddressId;
                if (Convert.ToBoolean(cartSettings["MandLastname"]) && String.IsNullOrEmpty(customerAddress.Lastname))
                    return adr.CustomerAddressId;
                if (Convert.ToBoolean(cartSettings["MandSuffix"]) && String.IsNullOrEmpty(customerAddress.Suffix))
                    return adr.CustomerAddressId;
                if (Convert.ToBoolean(cartSettings["MandStreet"]) && String.IsNullOrEmpty(customerAddress.Street))
                    return adr.CustomerAddressId;
                if (Convert.ToBoolean(cartSettings["MandUnit"]) && String.IsNullOrEmpty(customerAddress.Unit))
                    return adr.CustomerAddressId;
                if (Convert.ToBoolean(cartSettings["MandRegion"]) && String.IsNullOrEmpty(customerAddress.Region))
                    return adr.CustomerAddressId;
                if (Convert.ToBoolean(cartSettings["MandPostalCode"]) && String.IsNullOrEmpty(customerAddress.PostalCode))
                    return adr.CustomerAddressId;
                if (Convert.ToBoolean(cartSettings["MandCity"]) && String.IsNullOrEmpty(customerAddress.City))
                    return adr.CustomerAddressId;
                if (Convert.ToBoolean(cartSettings["MandSuburb"]) && String.IsNullOrEmpty(customerAddress.Suburb))
                    return adr.CustomerAddressId;
                if (Convert.ToBoolean(cartSettings["MandCountry"]) && String.IsNullOrEmpty(customerAddress.Country))
                    return adr.CustomerAddressId;
                if (Convert.ToBoolean(cartSettings["MandPhone"]) && String.IsNullOrEmpty(customerAddress.Telephone))
                    return adr.CustomerAddressId;
                if (Convert.ToBoolean(cartSettings["MandCell"]) && String.IsNullOrEmpty(customerAddress.Cell))
                    return adr.CustomerAddressId;
                if (Convert.ToBoolean(cartSettings["MandFax"]) && String.IsNullOrEmpty(customerAddress.Fax))
                    return adr.CustomerAddressId;
                if (Convert.ToBoolean(cartSettings["MandEmail"]) && String.IsNullOrEmpty(customerAddress.Email))
                    return adr.CustomerAddressId;
            }
            return -1;
        }

        public List<CartAddressInfo> GetCartAddresses(Guid CartId)
        {
            return CBO.FillCollection<CartAddressInfo>(DataProvider.Instance().GetCartAddresses(CartId));
        }

        #endregion

        #region CartAdditionalCost methods
        public CartAdditionalCostInfo GetCartAdditionalCost(int CartAdditionalCostId)
        {
            return (CartAdditionalCostInfo)CBO.FillObject(DataProvider.Instance().GetCartAdditionalCost(CartAdditionalCostId), typeof(CartAdditionalCostInfo));
        }
        public List<CartAdditionalCostInfo> GetCartAdditionalCosts(Guid CartId, bool isTaxfree = false)
        {
            return CBO.FillCollection<CartAdditionalCostInfo>(DataProvider.Instance().GetCartAdditionalCosts(CartId, isTaxfree));
        }
        public int NewCartAdditionalCost(CartAdditionalCostInfo CartAdditionalCost)
        {
            return DataProvider.Instance().NewCartAdditionalCost(CartAdditionalCost);
        }
        public void UpdateCartAdditionalCost(CartAdditionalCostInfo CartAdditionalCost)
        {
            DataProvider.Instance().UpdateCartAdditionalCost(CartAdditionalCost);
        }
        public void DeleteCartAdditionalCost(int CartAdditionalCostId)
        {
            DataProvider.Instance().DeleteCartAdditionalCost(CartAdditionalCostId);
        }
        public void DeleteCartAdditionalCost(Guid CartId, string Name)
        {
            DataProvider.Instance().DeleteCartAdditionalCost(CartId, Name);
        }
        #endregion

        #region CartProduct methods
        public CartProductInfo GetCartProduct(int CartProductId)
        {
            return (CartProductInfo)CBO.FillObject(DataProvider.Instance().GetCartProduct(CartProductId), typeof(CartProductInfo));
        }
        public CartProductInfo GetCartProductByProductId(Guid CartId, int ProductId)
        {
            return (CartProductInfo)CBO.FillObject(DataProvider.Instance().GetCartProductByProductId(CartId,ProductId), typeof(CartProductInfo));
        }
        public CartProductInfo GetCartProductByProductIdAndSelectedOptions(Guid CartId, int ProductId, System.Collections.Generic.List<OptionListInfo> SelectedOptions)
        {
            return (CartProductInfo)CBO.FillObject(DataProvider.Instance().GetCartProductByProductIdAndSelectedOptions(CartId,ProductId,SelectedOptions), typeof(CartProductInfo));
        }
        public List<CartProductInfo> GetCartProducts(Guid CartId, bool isTaxFree = false)
        {
            return CBO.FillCollection<CartProductInfo>(DataProvider.Instance().GetCartProducts(CartId, isTaxFree));
        }
        public int NewCartProduct(Guid CartId, CartProductInfo CartProduct)
        {
            return DataProvider.Instance().NewCartProduct(CartId, CartProduct);
        }
        public void UpdateCartProduct(CartProductInfo CartProduct)
        {
            DataProvider.Instance().UpdateCartProduct(CartProduct);
        }
        public void UpdateCartProductQuantity(int CartProductId, decimal Quantity)
        {
            DataProvider.Instance().UpdateCartProductQuantity(CartProductId,Quantity);
        }
        public void DeleteCartProduct(int CartProductId)
        {
            DataProvider.Instance().DeleteCartProduct(CartProductId);
        }
        #endregion

        #region CartProductOption methods
        public CartProductOptionInfo GetCartProductOption(int CartProductOptionId)
        {
            return (CartProductOptionInfo)CBO.FillObject(DataProvider.Instance().GetCartProductOption(CartProductOptionId), typeof(CartProductOptionInfo));
        }
        public List<CartProductOptionInfo> GetCartProductOptions(int CartProductId)
        {
            return CBO.FillCollection<CartProductOptionInfo>(DataProvider.Instance().GetCartProductOptions(CartProductId));
        }
        public int NewCartProductOption(int CartProductId, CartProductOptionInfo CartProductOption)
        {
            return DataProvider.Instance().NewCartProductOption(CartProductId, CartProductOption);
        }
        public void UpdateCartProductOption(CartProductOptionInfo CartProductOption)
        {
            DataProvider.Instance().UpdateCartProductOption(CartProductOption);
        }
        public void DeleteCartProductOption(int CartProductOptionId)
        {
            DataProvider.Instance().DeleteCartProductOption(CartProductOptionId);
        }
        public void DeleteCartProductOptions(int CartProductId)
        {
            DataProvider.Instance().DeleteCartProductOptions(CartProductId);
        }
        #endregion

        #region ProductTemplate methods
        [Obsolete("GetProductTemplate is deprecated", false)]
        public ProductTemplateInfo GetProductTemplate(int ProductTemplateid)
        {
            return (ProductTemplateInfo)CBO.FillObject(DataProvider.Instance().GetProductTemplate(ProductTemplateid), typeof(ProductTemplateInfo));
        }
        [Obsolete("GetProductTemplate is deprecated", false)]
        public ProductTemplateInfo GetProductTemplate(int PortalId,int SubscriberId,string TemplateName,string TemplateSource)
        {
            return (ProductTemplateInfo)CBO.FillObject(DataProvider.Instance().GetProductTemplate(PortalId, SubscriberId, TemplateName, TemplateSource), typeof(ProductTemplateInfo));
        }
        [Obsolete("GetProductTemplates is deprecated", false)]
        public List<ProductTemplateInfo> GetProductTemplates(int PortalId, int SubscriberId,string TemplateSource)
        {
            return CBO.FillCollection<ProductTemplateInfo>(DataProvider.Instance().GetProductTemplates(PortalId, SubscriberId, TemplateSource));
        }
        [Obsolete("NewProductTemplate is deprecated", false)]
        public int NewProductTemplate(ProductTemplateInfo ProductTemplate)
        {
            return DataProvider.Instance().NewProductTemplate(ProductTemplate);
        }
        [Obsolete("UpdateProductTemplate is deprecated", false)]
        public void UpdateProductTemplate(ProductTemplateInfo ProductTemplate)
        {
            DataProvider.Instance().UpdateProductTemplate(ProductTemplate);
        }
        public List<ProductTemplateInfo> GetProductTemplates(int PortalId)
        {
            return CBO.FillCollection<ProductTemplateInfo>(DataProvider.Instance().GetProductTemplates(PortalId));
        }
        public void DeleteProductTemplate(int ProductTemplateId)
        {
            DataProvider.Instance().DeleteProductTemplate(ProductTemplateId);
        }
        #endregion

        #region PaymentProvider methods
        public PaymentProviderInfo GetPaymentProvider(int PaymentProviderId, string Language)
        {
            return (PaymentProviderInfo)CBO.FillObject(DataProvider.Instance().GetPaymentProvider(PaymentProviderId,Language), typeof(PaymentProviderInfo));
        }
        public List<PaymentProviderInfo> GetPaymentProviders(string Language)
        {
            return CBO.FillCollection<PaymentProviderInfo>(DataProvider.Instance().GetPaymentProviders(Language));
        }
        #endregion

        #region PaymentProviderLang methods
        public List<PaymentProviderLangInfo> GetPaymentProviderLangs(int paymentProviderId)
        {
            return CBO.FillCollection<PaymentProviderLangInfo>(DataProvider.Instance().GetPaymentProviderLangs(paymentProviderId));
        }
        public void NewPaymentProviderLang(PaymentProviderLangInfo paymentProviderLang)
        {
            DataProvider.Instance().NewPaymentProviderLang(paymentProviderLang);
        }
        #endregion

        #region SubscriberPaymentProvider methods
        public List<SubscriberPaymentProviderInfo> GetSubscriberPaymentProviders(int PortalId,int SubscriberId)
        {
            return CBO.FillCollection<SubscriberPaymentProviderInfo>(DataProvider.Instance().GetSubscriberPaymentProviders(PortalId,SubscriberId));
        }
        public SubscriberPaymentProviderInfo GetSubscriberPaymentProvider(int PortalId,int SubscriberId, int PaymentProviderId)
        {
            return (SubscriberPaymentProviderInfo)CBO.FillObject(DataProvider.Instance().GetSubscriberPaymentProvider(PortalId,SubscriberId,PaymentProviderId), typeof(SubscriberPaymentProviderInfo));
        }

        public SubscriberPaymentProviderInfo GetSubscriberPaymentProviderByCPP(int customerPaymentProviderId)
        {
            return (SubscriberPaymentProviderInfo)CBO.FillObject(DataProvider.Instance().GetSubscriberPaymentProviderByCPP(customerPaymentProviderId), typeof(SubscriberPaymentProviderInfo));
        }

        public int NewSubscriberPaymentProvider(SubscriberPaymentProviderInfo SubscriberPaymentProvider)
        {
            return DataProvider.Instance().NewSubscriberPaymentProvider(SubscriberPaymentProvider);
        }
        public void UpdateSubscriberPaymentProvider(SubscriberPaymentProviderInfo SubscriberPaymentProvider)
        {
            DataProvider.Instance().UpdateSubscriberPaymentProvider(SubscriberPaymentProvider);
        }
        public void DeleteSubscriberPaymentProvider(int SubscriberPaymentProviderId)
        {
            DataProvider.Instance().DeleteSubscriberPaymentProvider(SubscriberPaymentProviderId);
        }
        #endregion

        #region CustomerPaymentProvider methods
        public List<CustomerPaymentProviderInfo> GetCustomerPaymentProviders(int CustomerId)
        {
            return CBO.FillCollection<CustomerPaymentProviderInfo>(DataProvider.Instance().GetCustomerPaymentProviders(CustomerId));
        }
        public CustomerPaymentProviderInfo GetCustomerPaymentProvider(int CustomerId, int PaymentProviderId)
        {
            return (CustomerPaymentProviderInfo)CBO.FillObject(DataProvider.Instance().GetCustomerPaymentProvider(CustomerId,PaymentProviderId), typeof(CustomerPaymentProviderInfo));    
        }
        public CustomerPaymentProviderInfo GetCustomerPaymentProvider(int CustomerPaymentProviderId)
        {
            return (CustomerPaymentProviderInfo)CBO.FillObject(DataProvider.Instance().GetCustomerPaymentProvider(CustomerPaymentProviderId), typeof(CustomerPaymentProviderInfo));    
        }
        public int NewCustomerPaymentProvider(CustomerPaymentProviderInfo CustomerPaymentProvider)
        {
            return DataProvider.Instance().NewCustomerPaymentProvider(CustomerPaymentProvider);
        }
        public void UpdateCustomerPaymentProvider(CustomerPaymentProviderInfo CustomerPaymentProvider)
        {
            DataProvider.Instance().UpdateCustomerPaymentProvider(CustomerPaymentProvider);
        }
        public void DeleteCustomerPaymentProvider(int CustomerPaymentProviderId)
        {
            DataProvider.Instance().DeleteCustomerPaymentProvider(CustomerPaymentProviderId);
        }
        #endregion

        #region SubscriberAddressType methods
        public List<SubscriberAddressTypeInfo> GetSubscriberAddressTypesByPortal(int portalId)
        {
            return CBO.FillCollection<SubscriberAddressTypeInfo>(DataProvider.Instance().GetSubscriberAddressTypesByPortal(portalId));
        }
        public List<SubscriberAddressTypeInfo> GetSubscriberAddressTypes(int portalId, int subscriberId, string language)
        {
            return CBO.FillCollection<SubscriberAddressTypeInfo>(DataProvider.Instance().GetSubscriberAddressTypes(portalId, subscriberId,language));
        }
        public SubscriberAddressTypeInfo GetSubscriberAddressType(int portalId, int subscriberId, string kzAddressType, string language)
        {
            return (SubscriberAddressTypeInfo) CBO.FillObject(DataProvider.Instance().GetSubscriberAddressType(portalId, subscriberId, kzAddressType,language),typeof (SubscriberAddressTypeInfo));
        }
        public SubscriberAddressTypeInfo GetSubscriberAddressType(int portalId, int subscriberId, int subscriberAddressTypeId, string language)
        {
            return (SubscriberAddressTypeInfo)CBO.FillObject(DataProvider.Instance().GetSubscriberAddressType(subscriberAddressTypeId,language), typeof(SubscriberAddressTypeInfo));
        }
        public int NewSubscriberAddressType(SubscriberAddressTypeInfo subscriberAddressType)
        {
            return DataProvider.Instance().NewSubscriberAddressType(subscriberAddressType);
        }
        public void UpdateSubscriberAddressType(SubscriberAddressTypeInfo subscriberAddressType)
        {
            DataProvider.Instance().UpdateSubscriberAddressType(subscriberAddressType);
        }
        public void DeleteSubscriberAddressType(int SubscriberAddressTypeId)
        {
            DataProvider.Instance().DeleteSubscriberAddressType(SubscriberAddressTypeId);
        }
        #endregion

        #region SubscriberAddressTypeLangs methods
        public List<SubscriberAddressTypeLangInfo> GetSubscriberAddressTypeLangs(int subscriberAddressTypeId)
        {
            return CBO.FillCollection<SubscriberAddressTypeLangInfo>(DataProvider.Instance().GetSubscriberAddressTypeLangs(subscriberAddressTypeId));
        }
        public List<SubscriberAddressTypeLangInfo> GetSubscriberAddressTypeLangsByPortal(int portalId)
        {
            return CBO.FillCollection<SubscriberAddressTypeLangInfo>(DataProvider.Instance().GetSubscriberAddressTypeLangsByPortal(portalId));
        }
        public void NewSubscriberAddressTypeLang(SubscriberAddressTypeLangInfo subscriberAddressTypeLang)
        {
            DataProvider.Instance().NewSubscriberAddressTypeLang(subscriberAddressTypeLang);
        }
        public void DeleteSubscriberAddressTypeLang(int subscriberAddressTypeId, string language)
        {
            DataProvider.Instance().DeleteSubscriberAddressTypeLang(subscriberAddressTypeId,language);
        }
        #endregion

        #region Order methods
        public int SaveOrder(Guid CartId, int PortalId, string numberMask, bool isTaxfree = false)
        {
            return DataProvider.Instance().SaveOrder(CartId, PortalId, numberMask, isTaxfree);
        }
        public OrderInfo GetOrder(int OrderId)
        {
            return (OrderInfo)CBO.FillObject<OrderInfo>(DataProvider.Instance().GetOrder(OrderId));
        }
        public List<OrderInfo> GetOrdersByPortal(int PortalId)
        {
            return CBO.FillCollection<OrderInfo>(DataProvider.Instance().GetOrdersByPortal(PortalId));
        }
        public List<OrderInfo> GetOrdersByPortalAndUser(int portalId, int userId)
        {
            return CBO.FillCollection<OrderInfo>(DataProvider.Instance().GetOrdersByPortalAndUser(portalId, userId));
        }
        public List<OrderListInfo> GetOrders(int PortalId, string Language, string Sort, string Filter)
        {
            return CBO.FillCollection<OrderListInfo>(DataProvider.Instance().GetOrders(PortalId, Language, Sort, Filter));
        }
        public int NewOrder(OrderInfo Order)
        {
            return DataProvider.Instance().NewOrder(Order);
        }
        public void UpdateOrder(OrderInfo Order)
        {
            DataProvider.Instance().UpdateOrder(Order);
        }
        public void DeleteOrder(int OrderId)
        {
            DataProvider.Instance().DeleteOrder(OrderId);
        }

        #endregion

        #region OrderProduct methods
        public List<OrderProductInfo> GetOrderProducts(int OrderId)
        {
            return CBO.FillCollection<OrderProductInfo>(DataProvider.Instance().GetOrderProducts(OrderId));
        }
        public List<OrderProductInfo> GetOrderProductsByPortal(int portalId)
        {
            return CBO.FillCollection<OrderProductInfo>(DataProvider.Instance().GetOrderProductsByPortal(portalId));
        }
        public List<OrderProductInfo> GetOrderProductsByPortalAndUser(int portalId, int userId)
        {
            return CBO.FillCollection<OrderProductInfo>(DataProvider.Instance().GetOrderProductsByPortalAndUser(portalId, userId));
        }
        public int NewOrderProduct(OrderProductInfo OrderProduct)
        {
            return DataProvider.Instance().NewOrderProduct(OrderProduct);
        }
        public void UpdateOrderProduct(OrderProductInfo OrderProduct)
        {
            DataProvider.Instance().UpdateOrderProduct(OrderProduct);
        }
        public void DeleteOrderProduct(int OrderProductId)
        {
            DataProvider.Instance().DeleteOrderProduct(OrderProductId);
        }
        #endregion

        #region OrderProductOption methods

        public List<OrderProductOptionInfo> GetOrderProductOptions(int OrderProductId)
        {
            return CBO.FillCollection<OrderProductOptionInfo>(DataProvider.Instance().GetOrderProductOptions(OrderProductId));
        }
        public List<OrderProductOptionInfo> GetOrderProductOptionsByPortal(int portalId)
        {
            return CBO.FillCollection<OrderProductOptionInfo>(DataProvider.Instance().GetOrderProductOptionsByPortal(portalId));
        }
        public List<OrderProductOptionInfo> GetOrderProductOptionsByPortalAndUser(int portalId, int userId)
        {
            return CBO.FillCollection<OrderProductOptionInfo>(DataProvider.Instance().GetOrderProductOptionsByPortalAndUser(portalId, userId));
        }
        public int NewOrderProductOption(OrderProductOptionInfo OrderProductOption)
        {
            return DataProvider.Instance().NewOrderProductOption(OrderProductOption);
        }
        public void UpdateOrderProductOption(OrderProductOptionInfo OrderProductOption)
        {
            DataProvider.Instance().UpdateOrderProductOption(OrderProductOption);
        }
        public void DeleteOrderProductOption(int OrderProductOptionId)
        {
            DataProvider.Instance().DeleteOrderProductOption(OrderProductOptionId);
        }

        #endregion

        #region OrderAdditionalCosts
        public List<OrderAdditionalCostInfo> GetOrderAdditionalCosts(int OrderId)
        {
            return CBO.FillCollection<OrderAdditionalCostInfo>(DataProvider.Instance().GetOrderAdditionalCosts(OrderId));
        }
        public List<OrderAdditionalCostInfo> GetOrderAdditionalCostsByPortal(int portalId)
        {
            return CBO.FillCollection<OrderAdditionalCostInfo>(DataProvider.Instance().GetOrderAdditionalCostsByPortal(portalId));
        }
        public List<OrderAdditionalCostInfo> GetOrderAdditionalCostsByPortalAndUser(int portalId, int userId)
        {
            return CBO.FillCollection<OrderAdditionalCostInfo>(DataProvider.Instance().GetOrderAdditionalCostsByPortalAndUser(portalId, userId));
        }

        public int NewOrderAdditionalCost(OrderAdditionalCostInfo OrderAdditionalCost)
        {
            return DataProvider.Instance().NewOrderAdditionalCost(OrderAdditionalCost);
        }
        public void UpdateOrderAdditionalCost(OrderAdditionalCostInfo OrderAdditionalCost)
        {
            DataProvider.Instance().UpdateOrderAdditionalCost(OrderAdditionalCost);
        }
        public void DeleteOrderAdditionalCost(int OrderAdditionalCostId)
        {
            DataProvider.Instance().DeleteOrderAdditionalCost(OrderAdditionalCostId);
        }


        #endregion

        #region OrderAddress

        public List<OrderAddressInfo> GetOrderAddresses(int orderId, string language)
        {
			return CBO.FillCollection<OrderAddressInfo>(DataProvider.Instance().GetOrderAddresses(orderId, language));
        }
        public List<OrderAddressInfo> GetOrderAddressesByPortal(int portalId)
        {
            return CBO.FillCollection<OrderAddressInfo>(DataProvider.Instance().GetOrderAddressesByPortal(portalId));
        }
        public List<OrderAddressInfo> GetOrderAddressesByPortalAndUser(int portalId, int userId)
        {
            return CBO.FillCollection<OrderAddressInfo>(DataProvider.Instance().GetOrderAddressesByPortalAndUser(portalId, userId));
        }
        public int NewOrderAddress(OrderAddressInfo OrderAddress)
        {
            return DataProvider.Instance().NewOrderAddress(OrderAddress);
        }
        public void UpdateOrderAddress(OrderAddressInfo OrderAddress)
        {
            DataProvider.Instance().UpdateOrderAddress(OrderAddress);
        }
        public bool HasOrderAddress(int customerAddressId)
        {
            return DataProvider.Instance().HasOrderAddress(customerAddressId);
        }
        public List<OrderStatsInfo> GetOrderStats(DateTime startDate, DateTime endDate)
        {
            return CBO.FillCollection<OrderStatsInfo>(DataProvider.Instance().GetOrderStats(startDate, endDate));
        }
        public void DeleteOrderAddress(int OrderAddressId)
        {
            DataProvider.Instance().DeleteOrderAddress(OrderAddressId);
        }
        #endregion

        #region OrderStates methods
        public List<OrderStateInfo> GetOrderStates(int portalId)
        {
            return CBO.FillCollection<OrderStateInfo>(DataProvider.Instance().GetOrderStates(portalId));
        }
        public List<OrderStateInfo> GetOrderStates(int portalId, string language)
        {
            return CBO.FillCollection<OrderStateInfo>(DataProvider.Instance().GetOrderStates(portalId,language));
        }

        public void SetOrderState(int orderId, int orderStateId)
        {
            DataProvider.Instance().SetOrderState(orderId,orderStateId);
        }
        public int NewOrderState(OrderStateInfo orderState)
        {
            return DataProvider.Instance().NewOrderState(orderState);
        }
        #endregion

        #region OrderStateLangs methods
        public OrderStateLangInfo GetOrderStateLang(int orderStateId, string language)
        {
            return (OrderStateLangInfo)CBO.FillObject(DataProvider.Instance().GetOrderStateLang(orderStateId, language), typeof(OrderStateLangInfo));
        }
        public List<OrderStateLangInfo> GetOrderStateLangs(int orderStateId)
        {
            return CBO.FillCollection<OrderStateLangInfo>(DataProvider.Instance().GetOrderStateLangs(orderStateId));
        }
        public void NewOrderStateLang(OrderStateLangInfo orderStateLang)
        {
            DataProvider.Instance().NewOrderStateLang(orderStateLang);
        }
        public void UpdateOrderStateLang(OrderStateLangInfo orderStateLang)
        {
            DataProvider.Instance().UpdateOrderStateLang(orderStateLang);
        }
        public void DeleteOrderStateLangs(int orderStateId)
        {
            DataProvider.Instance().DeleteOrderStateLangs(orderStateId);
        }
        public void DeleteOrderStateLang(int orderStateId, string language)
        {
            DataProvider.Instance().DeleteOrderStateLang(orderStateId, language);
        }

        #endregion

        #region ProductGroup methods
        public List<ProductGroupInfo> GetProductGroups(int portalId)
        {
            return CBO.FillCollection<ProductGroupInfo>(DataProvider.Instance().GetProductGroups(portalId));
        }
		public List<ProductGroupInfo> GetProductGroups(int portalId, string language, bool includeDisabled)
        {
			return CBO.FillCollection<ProductGroupInfo>(DataProvider.Instance().GetProductGroups(portalId, language, includeDisabled));
        }
        public List<ProductGroupInfo> GetProductSubGroupsByNode(int PortalId, string Language, int NodeId, bool IncludeCount, bool IncludeSubDirsInCount,bool IncludeDisabled)
        {
            return CBO.FillCollection<ProductGroupInfo>(DataProvider.Instance().GetProductSubGroupsByNode(PortalId, Language, NodeId, IncludeCount, IncludeSubDirsInCount,IncludeDisabled));
        }
        public ProductGroupInfo GetProductGroupByName(int PortalId, string Language, string ProductGroupName)
        {
            return (ProductGroupInfo)CBO.FillObject(DataProvider.Instance().GetProductGroupByName(PortalId, Language, ProductGroupName), typeof(ProductGroupInfo));
        }
        public ProductGroupInfo GetProductGroup(int PortalId, string Language, int ProductGroupId)
        {
            return (ProductGroupInfo)CBO.FillObject(DataProvider.Instance().GetProductGroup(PortalId, Language, ProductGroupId), typeof(ProductGroupInfo));
        }
        public ProductGroupInfo GetProductGroup(int PortalId, int ProductGroupId)
        {
            return (ProductGroupInfo)CBO.FillObject(DataProvider.Instance().GetProductGroup(PortalId, ProductGroupId), typeof(ProductGroupInfo));
        }
        public string GetProductGroupPath(int portalId, int productGroupId, string language, bool returnId, string delimiter, string linkTemplate, string rootText)
        {
            return DataProvider.Instance().GetProductGroupPath(portalId, productGroupId, language, returnId, delimiter, linkTemplate, rootText);
        }
        public string GetProductGroupPath(int PortalId, int ProductGroupId)
        {
            return DataProvider.Instance().GetProductGroupPath(PortalId, ProductGroupId);
        }
        public int NewProductGroup(ProductGroupInfo ProductGroup)
        {
            return DataProvider.Instance().NewProductGroup(ProductGroup);
        }
        public void UpdateProductGroup(ProductGroupInfo ProductGroup)
        {
            DataProvider.Instance().UpdateProductGroup(ProductGroup);
        }
        public void DeleteProductGroup(int ProductGroupId)
        {
            DataProvider.Instance().DeleteProductGroup(ProductGroupId);
        }
        public void DeleteProductGroups(int PortalId)
        {
            DataProvider.Instance().DeleteProductGroups(PortalId);
        }
        #endregion

        #region ProductGroupLang methods
        public List<ProductGroupLangInfo> GetProductGroupLangs(int ProductGroupId)
        {
            return CBO.FillCollection<ProductGroupLangInfo>(DataProvider.Instance().GetProductGroupLangs(ProductGroupId));
        }
        public List<ProductGroupLangInfo> GetProductGroupLangsByPortal(int portalId)
        {
            return CBO.FillCollection<ProductGroupLangInfo>(DataProvider.Instance().GetProductGroupLangsByPortal(portalId));
        }
        public ProductGroupLangInfo GetProductGroupLang(int ProductGroupId, string Language)
        {
            return (ProductGroupLangInfo)CBO.FillObject(DataProvider.Instance().GetProductGroupLang(ProductGroupId, Language), typeof(ProductGroupLangInfo));
        }
        public void NewProductGroupLang(ProductGroupLangInfo ProductGroupLang)
        {
            DataProvider.Instance().NewProductGroupLang(ProductGroupLang);
        }
        public void UpdateProductGroupLang(ProductGroupLangInfo ProductGroupLang)
        {
            DataProvider.Instance().UpdateProductGroupLang(ProductGroupLang);
        }
        public void DeleteProductGroupLangs(int ProductGroupId)
        {
            DataProvider.Instance().DeleteProductGroupLangs(ProductGroupId);
        }
        public void DeleteProductGroupLang(int ProductGroupId, string Language)
        {
            DataProvider.Instance().DeleteProductGroupLang(ProductGroupId, Language);
        }
        #endregion

        #region ProductInGroup methods
        public DataTable GetProductsInGroupByProduct(int SimpleProductId)
        {
            return DataProvider.Instance().GetProductsInGroupByProduct(SimpleProductId);
        }
        public List<ProductInGroupInfo> GetProductsInGroupByPortal(int portalId)
        {
            return CBO.FillCollection<ProductInGroupInfo>(DataProvider.Instance().GetProductsInGroupByPortal(portalId));
        }

        public void NewProductInGroup(int SimpleProductId, int ProductGroupId)
        {
            DataProvider.Instance().NewProductInGroup(SimpleProductId, ProductGroupId);

        }
        public void DeleteProductInGroups(int SimpleProductId)
        {
            DataProvider.Instance().DeleteProductInGroups(SimpleProductId);
        }
        public void DeleteProductInGroup(int SimpleProductId, int ProductGroupId)
        {
            DataProvider.Instance().DeleteProductInGroup(SimpleProductId, ProductGroupId);
        }
        #endregion

        #region  ProductFilter methods
        public List<ProductFilterInfo> GetProductFilters(int PortalId, Guid FilterSessionId)
        {
            return CBO.FillCollection<ProductFilterInfo>(DataProvider.Instance().GetProductFilters(PortalId,FilterSessionId));
        }
        public List<ProductFilterInfo> GetProductFilter(int PortalId, Guid FilterSessionId, string FilterSource)
        {
            return CBO.FillCollection<ProductFilterInfo>(DataProvider.Instance().GetProductFilter(PortalId, FilterSessionId,FilterSource));
        }
        public void NewProductFilter(ProductFilterInfo ProductFilter)
        {
            DataProvider.Instance().NewProductFilter(ProductFilter);
        }
        public void UpdateProductFilter(ProductFilterInfo ProductFilter)
        {
            DataProvider.Instance().UpdateProductFilter(ProductFilter);
        }
        public void DeleteProductFilters(int PortalId, Guid FilterSessionId)
        {
            DataProvider.Instance().DeleteProductFilters(PortalId, FilterSessionId);
        }
        public void DeleteProductFilter(int PortalId, Guid FilterSessionId, string FilterSource)
        {
            DataProvider.Instance().DeleteProductFilter(PortalId, FilterSessionId, FilterSource);
        }
        public void DeleteProductFilter(int PortalId, Guid FilterSessionId, string FilterSource, string FirstFilterValue)
        {
            DataProvider.Instance().DeleteProductFilter(PortalId, FilterSessionId, FilterSource, FirstFilterValue);
        }
        #endregion

        #region FeatureGrid methods
        public List<FeatureGridValueInfo> GetFeatureGridValues(int PortalId, int ProductId, string Language,int RoleId, int FeatureGroupId, bool showAll)
        {
            return CBO.FillCollection<FeatureGridValueInfo>(DataProvider.Instance().GetFeatureGridValues(PortalId, ProductId, Language, RoleId, FeatureGroupId, showAll));
        }
        public FeatureGridValueInfo GetFeatureGridValueByProductAndToken(int PortalId, int ProductId, string Language, string FeatureToken)
        {
            return (FeatureGridValueInfo)CBO.FillObject(DataProvider.Instance().GetFeatureGridValueByProductAndToken(PortalId, ProductId, Language, FeatureToken), typeof(FeatureGridValueInfo));    
        }
        public List<FeatureGridFeatureInfo> GetFeatureGridFeaturesByProduct(int PortalId, int ProductId, string Language, int RoleId, int FeatureGroupId)
        {
            return CBO.FillCollection<FeatureGridFeatureInfo>(DataProvider.Instance().GetFeatureGridFeaturesByProduct(PortalId, ProductId, Language, RoleId, FeatureGroupId));
        }
        public List<FeatureGridFeatureInfo> GetFeatureGridFeaturesByProductGroup(int PortalId, int ProductGroupId, string Language, int RoleId, int FeatureGroupId,bool OnlyShowInSearch)
        {
            return CBO.FillCollection<FeatureGridFeatureInfo>(DataProvider.Instance().GetFeatureGridFeaturesByProductGroup(PortalId, ProductGroupId, Language, RoleId, FeatureGroupId, OnlyShowInSearch));
        }
        #endregion

        #region FeatureValue methods
        public int GetFeatureValueId(int productId,int featureId)
        {
            return DataProvider.Instance().GetFeatureValueId(productId, featureId);
        }
        public List<FeatureValueInfo> GetFeatureValuesByPortal(int portalId)
        {
            return CBO.FillCollection<FeatureValueInfo>(DataProvider.Instance().GetFeatureValuesByPortal(portalId));
        }
        public void DeleteFeatureValuesByProductId(int ProductId, int FeatureGroupId)
        {
            DataProvider.Instance().DeleteFeatureValuesByProductId(ProductId, FeatureGroupId);
        }
        public int NewFeatureValue(FeatureValueInfo FeatureValue)
        {
            return DataProvider.Instance().NewFeatureValue(FeatureValue);
        }
        public void UpdateFeatureValue(FeatureValueInfo FeatureValue)
        {
            DataProvider.Instance().UpdateFeatureValue(FeatureValue);
        }
        public void DeleteFeatureValue(int FeatureValueId)
        {
            DataProvider.Instance().DeleteFeatureValue(FeatureValueId);
        }
        public void DeleteFeatureValuesByPortal(int portalId)
        {
            DataProvider.Instance().DeleteFeatureValuesByPortal(portalId);
        }
        #endregion

        #region FeatureGroup methods
        public FeatureGroupInfo GetFeatureGroupById(int FeatureGroupId)
        {
            return (FeatureGroupInfo)CBO.FillObject(DataProvider.Instance().GetFeatureGroupById(FeatureGroupId), typeof(FeatureGroupInfo));
        }
        public List<FeatureGroupInfo> GetFeatureGroups(int PortalId)
        {
            return CBO.FillCollection<FeatureGroupInfo>(DataProvider.Instance().GetFeatureGroups(PortalId));
        }
        public FeatureGroupInfo GetFeatureGroupById(int FeatureGroupId, string Language)
        {
            return (FeatureGroupInfo)CBO.FillObject(DataProvider.Instance().GetFeatureGroupById(FeatureGroupId, Language), typeof(FeatureGroupInfo));
        }
        public List<FeatureGroupInfo> GetFeatureGroups(int FeatureGroupId, string Language)
        {
            return CBO.FillCollection<FeatureGroupInfo>(DataProvider.Instance().GetFeatureGroups(FeatureGroupId, Language));
        }
        public int NewFeatureGroup(FeatureGroupInfo FeatureGroup)
        {
            return DataProvider.Instance().NewFeatureGroup(FeatureGroup);
        }
        public void UpdateFeatureGroup(FeatureGroupInfo FeatureGroup)
        {
            DataProvider.Instance().UpdateFeatureGroup(FeatureGroup);
        }
        public void DeleteFeatureGroup(int FeatureGroupId)
        {
            DataProvider.Instance().DeleteFeatureGroup(FeatureGroupId);
        }
        public void DeleteFeatureGroups(int PortalId)
        {
            DataProvider.Instance().DeleteFeatureGroups(PortalId);
        }
        #endregion

        #region FeatureGroupLang methods
        public FeatureGroupLangInfo GetFeatureGroupLang(int FeatureGroupId,string Language)
        {
            return (FeatureGroupLangInfo)CBO.FillObject(DataProvider.Instance().GetFeatureGroupLang(FeatureGroupId,Language), typeof(FeatureGroupLangInfo));
        }
        public List<FeatureGroupLangInfo> GetFeatureGroupLangs(int FeatureGroupId)
        {
            return CBO.FillCollection<FeatureGroupLangInfo>(DataProvider.Instance().GetFeatureGroupLangs(FeatureGroupId));
        }
        public List<FeatureGroupLangInfo> GetFeatureGroupLangsByPortal(int portalId)
        {
            return CBO.FillCollection<FeatureGroupLangInfo>(DataProvider.Instance().GetFeatureGroupLangsByPortal(portalId));
        }
        public void NewFeatureGroupLang(FeatureGroupLangInfo FeatureGroupLang)
        {
            DataProvider.Instance().NewFeatureGroupLang(FeatureGroupLang);
        }
        public void UpdateFeatureGroupLang(FeatureGroupLangInfo FeatureGroupLang)
        {
            DataProvider.Instance().UpdateFeatureGroupLang(FeatureGroupLang);
        }
        public void DeleteFeatureGroupLangs(int FeatureGroupId)
        {
            DataProvider.Instance().DeleteFeatureGroupLangs(FeatureGroupId);
        }
        public void DeleteFeatureGroupLang(int FeatureGroupId,string Language)
        {
            DataProvider.Instance().DeleteFeatureGroupLang(FeatureGroupId,Language);
        }
        #endregion

        #region Feature methods
        public List<FeatureInfo> GetFeatures(int PortalId)
        {
            return CBO.FillCollection<FeatureInfo>(DataProvider.Instance().GetFeatures(PortalId));
        }
        public List<FeatureInfo> GetFeatures(int FeatureId, string Language)
        {
            return CBO.FillCollection<FeatureInfo>(DataProvider.Instance().GetFeatures(FeatureId, Language));
        }
        public FeatureInfo GetFeatureById(int FeatureId)
        {
            return (FeatureInfo)CBO.FillObject(DataProvider.Instance().GetFeatureById(FeatureId), typeof(FeatureInfo));
        }
        public FeatureInfo GetFeatureById(int FeatureId, string Language)
        {
            return (FeatureInfo)CBO.FillObject(DataProvider.Instance().GetFeatureById(FeatureId, Language), typeof(FeatureInfo));
        }
        public int NewFeature(FeatureInfo Feature)
        {
            return DataProvider.Instance().NewFeature(Feature);
        }
        public void UpdateFeature(FeatureInfo Feature)
        {
            DataProvider.Instance().UpdateFeature(Feature);
        }
        public void DeleteFeature(int FeatureId)
        {
            DataProvider.Instance().DeleteFeature(FeatureId);
        }
        public void DeleteFeatures(int Portalid)
        {
            DataProvider.Instance().DeleteFeatures(Portalid);
        }
        #endregion

        #region FeatureLang methods
        public List<FeatureLangInfo> GetFeatureLangs(int FeatureId)
        {
            return CBO.FillCollection<FeatureLangInfo>(DataProvider.Instance().GetFeatureLangs(FeatureId));
        }
        public List<FeatureLangInfo> GetFeatureLangsByPortal(int portalId)
        {
            return CBO.FillCollection<FeatureLangInfo>(DataProvider.Instance().GetFeatureLangsByPortal(portalId));
        }
        public FeatureLangInfo GetFeatureLang(int FeatureId, string Language)
        {
            return (FeatureLangInfo)CBO.FillObject(DataProvider.Instance().GetFeatureLang(FeatureId, Language), typeof(FeatureLangInfo));
        }
        public void NewFeatureLang(FeatureLangInfo FeatureLang)
        {
            DataProvider.Instance().NewFeatureLang(FeatureLang);
        }
        public void UpdateFeatureLang(FeatureLangInfo FeatureLang)
        {
            DataProvider.Instance().UpdateFeatureLang(FeatureLang);
        }
        public void DeleteFeatureLangs(int FeatureId)
        {
            DataProvider.Instance().DeleteFeatureLangs(FeatureId);
        }
        public void DeleteFeatureLang(int FeatureId, string Language)
        {
            DataProvider.Instance().DeleteFeatureLang(FeatureId, Language);
        }
        #endregion

        #region FeatureList methods
        public List<FeatureListInfo> GetFeatureLists(int PortalId)
        {
            return CBO.FillCollection<FeatureListInfo>(DataProvider.Instance().GetFeatureLists(PortalId));
        }
        public List<FeatureListInfo> GetFeatureLists(int portalId, string Language)
        {
            return CBO.FillCollection<FeatureListInfo>(DataProvider.Instance().GetFeatureLists(portalId, Language));
        }
        public FeatureListInfo GetFeatureListById(int FeatureListId)
        {
            return (FeatureListInfo)CBO.FillObject(DataProvider.Instance().GetFeatureListById(FeatureListId), typeof(FeatureListInfo));
        }
        public FeatureListInfo GetFeatureListById(int FeatureListId, string Language)
        {
            return (FeatureListInfo)CBO.FillObject(DataProvider.Instance().GetFeatureListById(FeatureListId, Language), typeof(FeatureListInfo));
        }
        public int NewFeatureList(FeatureListInfo FeatureList)
        {
            return DataProvider.Instance().NewFeatureList(FeatureList);
        }
        public void UpdateFeatureList(FeatureListInfo FeatureList)
        {
            DataProvider.Instance().UpdateFeatureList(FeatureList);
        }
        public void DeleteFeatureList(int FeatureListId)
        {
            DataProvider.Instance().DeleteFeatureList(FeatureListId);
        }
        public void DeleteFeatureLists(int PortalId)
        {
            DataProvider.Instance().DeleteFeatureLists(PortalId);
        }
        #endregion

        #region FeatureListLang methods
        public List<FeatureListLangInfo> GetFeatureListLangs(int FeatureListId)
        {
            return CBO.FillCollection<FeatureListLangInfo>(DataProvider.Instance().GetFeatureListLangs(FeatureListId));
        }
        public List<FeatureListLangInfo> GetFeatureListLangsByPortal(int portalId)
        {
            return CBO.FillCollection<FeatureListLangInfo>(DataProvider.Instance().GetFeatureListLangsByPortal(portalId));
        }
        public FeatureListLangInfo GetFeatureListLang(int FeatureListId, string Language)
        {
            return (FeatureListLangInfo)CBO.FillObject(DataProvider.Instance().GetFeatureListLang(FeatureListId, Language), typeof(FeatureListLangInfo));
        }
        public void NewFeatureListLang(FeatureListLangInfo FeatureListLang)
        {
            DataProvider.Instance().NewFeatureListLang(FeatureListLang);
        }
        public void UpdateFeatureListLang(FeatureListLangInfo FeatureListLang)
        {
            DataProvider.Instance().UpdateFeatureListLang(FeatureListLang);
        }
        public void DeleteFeatureListLangs(int FeatureListId)
        {
            DataProvider.Instance().DeleteFeatureListLangs(FeatureListId);
        }
        public void DeleteFeatureListLang(int FeatureListId, string Language)
        {
            DataProvider.Instance().DeleteFeatureListLang(FeatureListId, Language);
        }
        #endregion

        #region FeatureListItem methods
        public FeatureListItemInfo GetFeatureListItemById(int FeatureListItemId)
        {
            return (FeatureListItemInfo)CBO.FillObject(DataProvider.Instance().GetFeatureListItemById(FeatureListItemId), typeof(FeatureListItemInfo));
        }
        public FeatureListItemInfo GetFeatureListItemById(int FeatureListItemId, string Language)
        {
            return (FeatureListItemInfo)CBO.FillObject(DataProvider.Instance().GetFeatureListItemById(FeatureListItemId, Language), typeof(FeatureListItemInfo));
        }
        public List<FeatureListItemInfo> GetFeatureListItems(int portalId)
        {
            return CBO.FillCollection<FeatureListItemInfo>(DataProvider.Instance().GetFeatureListItems(portalId));
        }
        public List<FeatureListItemInfo> GetFeatureListItemsByListId(int FeatureListId)
        {
            return CBO.FillCollection<FeatureListItemInfo>(DataProvider.Instance().GetFeatureListItemsByListId(FeatureListId));
        }
        public List<FeatureListItemInfo> GetFeatureListItemsByListId(int FeatureListId, string Language, bool onlyWithImage)
        {
            return CBO.FillCollection<FeatureListItemInfo>(DataProvider.Instance().GetFeatureListItemsByListId(FeatureListId, Language, onlyWithImage));
        }
        public List<FeatureListItemInfo> GetFeatureListItemsByListAndProduct(int FeatureListId, int ProductId, string Language)
        {
            return CBO.FillCollection<FeatureListItemInfo>(DataProvider.Instance().GetFeatureListItemsByListAndProduct(FeatureListId, ProductId, Language));
        }
        public List<FeatureListItemInfo> GetFeatureListItemsByListAndProductGroup(int FeatureListId, int ProductGroupId, string Language)
        {
            return CBO.FillCollection<FeatureListItemInfo>(DataProvider.Instance().GetFeatureListItemsByListAndProductGroup(FeatureListId, ProductGroupId, Language));
        }
        public int NewFeatureListItem(FeatureListItemInfo FeatureListItem)
        {
            return DataProvider.Instance().NewFeatureListItem(FeatureListItem);
        }
        public void UpdateFeatureListItem(FeatureListItemInfo FeatureListItem)
        {
            DataProvider.Instance().UpdateFeatureListItem(FeatureListItem);
        }
        public void DeleteFeatureListItem(int FeatureListItemId)
        {
            DataProvider.Instance().DeleteFeatureListItem(FeatureListItemId);
        }
        public void DeleteFeatureListItems(int FeatureListId)
        {
            DataProvider.Instance().DeleteFeatureListItems(FeatureListId);
        }
        #endregion

        #region FeatureListItemLang methods
        public List<FeatureListItemLangInfo> GetFeatureListItemLangs(int FeatureListItemId)
        {
            return CBO.FillCollection<FeatureListItemLangInfo>(DataProvider.Instance().GetFeatureListItemLangs(FeatureListItemId));			
        }
        public List<FeatureListItemLangInfo> GetFeatureListItemLangsByPortal(int portalId)
        {
            return CBO.FillCollection<FeatureListItemLangInfo>(DataProvider.Instance().GetFeatureListItemLangsByPortal(portalId));
        }
        public FeatureListItemLangInfo GetFeatureListItemLang(int FeatureListItemId, string Language)
        {
            return (FeatureListItemLangInfo)CBO.FillObject(DataProvider.Instance().GetFeatureListItemLang(FeatureListItemId,Language), typeof(FeatureListItemLangInfo));

        }
        public void NewFeatureListItemLang(FeatureListItemLangInfo FeatureListItemLang)
        {
            DataProvider.Instance().NewFeatureListItemLang(FeatureListItemLang);
        }
        public void UpdateFeatureListItemLang(FeatureListItemLangInfo FeatureListItemLang)
        {
            DataProvider.Instance().UpdateFeatureListItemLang(FeatureListItemLang);
        }
        public void DeleteFeatureListItemLangs(int FeatureListItemId)
        {
            DataProvider.Instance().DeleteFeatureListItemLangs(FeatureListItemId);
        }
        public void DeleteFeatureListItemLang(int FeatureListItemId, string Language)
        {
            DataProvider.Instance().DeleteFeatureListItemLang(FeatureListItemId, Language);
        }
        #endregion

        #region ProductGroupFeature methods
        public DataTable GetProductGroupFeatures(int FeatureId)
        {
            return DataProvider.Instance().GetProductGroupFeatures(FeatureId);
        }
        public List<ProductGroupFeatureInfo> GetProductGroupFeaturesByPortal(int portalId)
        {
            return CBO.FillCollection<ProductGroupFeatureInfo>(DataProvider.Instance().GetProductGroupFeaturesByPortal(portalId));
        }
        public bool IsFeatureInProductGroup(int productGroupId, int featureId)
        {
            return DataProvider.Instance().IsFeatureInProductGroup(productGroupId, featureId);
        }
        public void NewProductGroupFeature(int FeatureId, int ProductGroupId)
        {
            DataProvider.Instance().NewProductGroupFeature(FeatureId, ProductGroupId);
        }
        public void DeleteProductGroupFeatures(int FeatureId)
        {
            DataProvider.Instance().DeleteProductGroupFeatures(FeatureId);
        }
        public void DeleteProductGroupFeature(int FeatureId, int ProductGroupId)
        {
            DataProvider.Instance().DeleteProductGroupFeature(FeatureId, ProductGroupId);
        }
        public void DeleteProductGroupFeaturesByPortal(int portalId)
        {
            DataProvider.Instance().DeleteProductGroupFeaturesByPortal(portalId);
        }
        #endregion

        #region ProductGroupListItems methods
        public List<ProductGroupListItemInfo> GetProductGroupListItemsByPortal(int portalId)
        {
            return CBO.FillCollection<ProductGroupListItemInfo>(DataProvider.Instance().GetProductGroupListItemsByPortal(portalId));
        }
        public bool IsFeatureListItemInProductGroup(int productGroupId, int featureListItemid)
        {
            return DataProvider.Instance().IsFeatureListItemInProductGroup(productGroupId, featureListItemid);
        }
        public void NewProductGroupListItem(int productGroupId, int featureListItemid)
        {
            DataProvider.Instance().NewProductGroupListItem(productGroupId, featureListItemid);
        }
        public void DeleteProductGroupListItem(int productGroupId, int featureListItemid)
        {
            DataProvider.Instance().DeleteProductGroupListItem(productGroupId, featureListItemid);
        }
        public void DeleteProductGroupListItemsByPortal(int portalId)
        {
            DataProvider.Instance().DeleteProductGroupListItemsByPortal(portalId);
        }
        public void DeleteProductGroupListItemsByProductGroup(int productGroupId)
        {
            DataProvider.Instance().DeleteProductGroupListItemsByProductGroup(productGroupId);
        }
        public void DeleteProductGroupListItemsByProductGroupAndFeatureList(int productgroupId, int featureListId)
		{
			DataProvider.Instance().DeleteProductGroupListItemsByProductGroupAndFeatureList(productgroupId, featureListId);
        }
		public void AddProductGroupListItemsByProductGroupAndFeatureList(int productgroupId, int featureListId)
		{
			DataProvider.Instance().AddProductGroupListItemsByProductGroupAndFeatureList(productgroupId, featureListId);
		}
		public List<FeatureListInfo> GetSelectedFeatureListsByProductGroup(int productGroupId, string language)
		{
			return CBO.FillCollection<FeatureListInfo>(DataProvider.Instance().GetSelectedFeatureListsByProductGroup(productGroupId, language));
		}
        #endregion

        #region StaticFilter methods
        public List<StaticFilterInfo> GetStaticFilters(int PortalId)
        {
            return CBO.FillCollection<StaticFilterInfo>(DataProvider.Instance().GetStaticFilters(PortalId));
        }
        public StaticFilterInfo GetStaticFilter(int PortalId, string Token)
        {
            return (StaticFilterInfo)CBO.FillObject(DataProvider.Instance().GetStaticFilter(PortalId, Token), typeof(StaticFilterInfo));
        }
        public StaticFilterInfo GetStaticFilterById(int StaticFilterId)
        {
            return (StaticFilterInfo)CBO.FillObject(DataProvider.Instance().GetStaticFilterById(StaticFilterId), typeof(StaticFilterInfo));
        }
        public int NewStaticFilter(StaticFilterInfo StaticFilter)
        {
            return DataProvider.Instance().NewStaticFilter(StaticFilter);
        }
        public void UpdateStaticFilter(StaticFilterInfo StaticFilter)
        {
            DataProvider.Instance().UpdateStaticFilter(StaticFilter);
        }
        public void DeleteStaticFilter(int PortalId, string Token)
        {
            DataProvider.Instance().DeleteStaticFilter(PortalId,Token);
        }
        public void DeleteStaticFilterById(int StaticFilterId)
        {
            DataProvider.Instance().DeleteStaticFilterById(StaticFilterId);
        }
        #endregion

        #region LocalResources methods
        public LocalResourceInfo GetLocalResource(int portalId, string token)
        {
            return (LocalResourceInfo)CBO.FillObject(DataProvider.Instance().GetLocalResource(portalId, token), typeof(LocalResourceInfo));
        }
        public int NewLocalResource(LocalResourceInfo resourceInfo)
        {
            return DataProvider.Instance().NewLocalResource(resourceInfo);
        }
        public void UpdateLocalResource(LocalResourceInfo resourceInfo)
        {
            DataProvider.Instance().UpdateLocalResource(resourceInfo);
        }
        public void DeleteLocalResource(int resourceId)
        {
            DataProvider.Instance().DeleteLocalResource(resourceId);
        }
        #endregion

        #region  LocalResourceLang methods
        public LocalResourceLangInfo GetLocalResourceLang(int resourceId, string language)
        {
            return (LocalResourceLangInfo)CBO.FillObject(DataProvider.Instance().GetLocalResourceLang(resourceId, language), typeof(LocalResourceLangInfo));
        }
        public LocalResourceLangInfo GetLocalResourceLang(int portalId, string token, string language)
        {
            return (LocalResourceLangInfo)CBO.FillObject(DataProvider.Instance().GetLocalResourceLang(portalId,token, language), typeof(LocalResourceLangInfo));
        }
        public List<LocalResourceLangInfo> GetLocalResourceLangs(int resourceId)
        {
            return CBO.FillCollection<LocalResourceLangInfo>(DataProvider.Instance().GetLocalResourceLangs(resourceId));
        }
        public List<LocalResourceLangInfo> GetLocalResourceLangs(int portalId, string token)
        {
            return CBO.FillCollection<LocalResourceLangInfo>(DataProvider.Instance().GetLocalResourceLangs(portalId,token));
        }
        public void NewLocalResourceLang(LocalResourceLangInfo resourceLangInfo)
        {
            DataProvider.Instance().NewLocalResourceLang(resourceLangInfo);
        }
        public void UpdateLocalResourceLang(LocalResourceLangInfo resourceLangInfo)
        {
            DataProvider.Instance().UpdateLocalResourceLang(resourceLangInfo);
        }
        public void DeleteLocalResourceLang(LocalResourceLangInfo resourceLangInfo)
        {
            DataProvider.Instance().DeleteLocalResourceLang(resourceLangInfo);
        }
        public void DeleteLocalResourceLangs(int resourceId)
        {
            DataProvider.Instance().DeleteLocalResourceLangs(resourceId);
        }
        #endregion

        #region ContactAddress methods
        public ContactAddressInfo GetContactAddresses(DateTime? StartDate)
        {
            return (ContactAddressInfo)CBO.FillObject(DataProvider.Instance().GetContactAddresses(StartDate), typeof(ContactAddressInfo));
        }
        public int NewContactAddress(ContactAddressInfo ContactAddress)
        {
            return DataProvider.Instance().NewContactAddress(ContactAddress);
        }
        public void UpdateContactAddress(ContactAddressInfo ContactAddress)
        {
            DataProvider.Instance().UpdateContactAddress(ContactAddress);
        }
        public void DeleteContactAddress(int ContactAddressId)
        {
            DataProvider.Instance().DeleteContactAddress(ContactAddressId);
        }
        #endregion

        #region ContactProduct methods
        public List<SimpleProductInfo> GetContactProductsByCartId(int PortalId, Guid CartId, string Language)
        {
            return CBO.FillCollection<SimpleProductInfo>(DataProvider.Instance().GetContactProductsByCartId(PortalId, CartId, Language));
        }
        public List<SimpleProductInfo> GetContactProductsByAddressId(int PortalId, int ContactAddressId, string Language)
        {
            return CBO.FillCollection<SimpleProductInfo>(DataProvider.Instance().GetContactProductsByAddressId(PortalId, ContactAddressId, Language));
        }
        public void NewContactProduct(Guid CartId, int ProductId, int ContactAddressId, string selectedAttributes)
        {
            DataProvider.Instance().NewContactProduct(CartId, ProductId, ContactAddressId, selectedAttributes);
        }
        public void UpdateContactProduct(Guid CartId, int ProductId, int ContactAddressId)
        {
            DataProvider.Instance().UpdateContactProduct(CartId, ProductId, ContactAddressId);
        }
        public void DeleteContactProduct(Guid CartId, int ProductId)
        {
            DataProvider.Instance().DeleteContactProduct(CartId, ProductId);
        }
        #endregion

        #region ContactReason methods
        public List<ContactReasonInfo> GetContactReasons(int ContactAddressId)
        {
            return CBO.FillCollection<ContactReasonInfo>(DataProvider.Instance().GetContactReasons(ContactAddressId));
        }
        public ContactReasonInfo GetContactReasonByToken(int ContactAddressId, string Token)
        {
            return (ContactReasonInfo)CBO.FillObject(DataProvider.Instance().GetContactReasonByToken(ContactAddressId, Token), typeof(ContactReasonInfo));
        }
        public void NewContactReason(ContactReasonInfo ContactReason)
        {
            DataProvider.Instance().NewContactReason(ContactReason);
        }
        public void UpdateContactReason(ContactReasonInfo ContactReason)
        {
            DataProvider.Instance().UpdateContactReason(ContactReason);
        }
        public void DeleteContactReasons(int ContactAddressId)
        {
            DataProvider.Instance().DeleteContactReasons(ContactAddressId);
        }
        public void DeleteContactReason(int ContactAddressId, string Token)
        {
            DataProvider.Instance().DeleteContactReason(ContactAddressId, Token);
        }
        #endregion

        #region Unit methods
        public UnitInfo GetUnit(int UnitId)
        {
            return (UnitInfo)CBO.FillObject(DataProvider.Instance().GetUnit(UnitId), typeof(UnitInfo));
        }
        public UnitInfo GetUnit(int UnitId, string Language)
        {
            return (UnitInfo)CBO.FillObject(DataProvider.Instance().GetUnit(UnitId, Language), typeof(UnitInfo));
        }
        public List<UnitInfo> GetUnits(int portalId)
        {
            return CBO.FillCollection<UnitInfo>(DataProvider.Instance().GetUnits(portalId));
        }
        public List<UnitInfo> GetUnits(int portalId, string language, string sortByField)
        {
            return CBO.FillCollection<UnitInfo>(DataProvider.Instance().GetUnits(portalId, language, sortByField));
        }
       
        public int NewUnit(UnitInfo Unit)
        {
            return DataProvider.Instance().NewUnit(Unit);
        }
        public void UpdateUnit(UnitInfo Unit)
        {
            DataProvider.Instance().UpdateUnit(Unit);
        }
        public void DeleteUnit(int UnitId)
        {
            DataProvider.Instance().DeleteUnit(UnitId);
        }
        #endregion

        #region UnitLang methods
        public UnitLangInfo GetUnitLang(int unitId, string language)
        {
            return (UnitLangInfo)CBO.FillObject(DataProvider.Instance().GetUnitLang(unitId, language), typeof(UnitLangInfo));
        }
        public List<UnitLangInfo> GetUnitLangs(int unitId)
        {
            return CBO.FillCollection<UnitLangInfo>(DataProvider.Instance().GetUnitLangs(unitId));
        }
        public List<UnitLangInfo> GetUnitLangsByPortal(int portalId)
        {
            return CBO.FillCollection<UnitLangInfo>(DataProvider.Instance().GetUnitLangsByPortal(portalId));
        }
        public List<UnitLangInfo> GetPortalUnitLangs(int portalId)
        {
            return CBO.FillCollection<UnitLangInfo>(DataProvider.Instance().GetPortalUnitLangs(portalId));
        }
        public void NewUnitLang(UnitLangInfo unitLang)
        {
            DataProvider.Instance().NewUnitLang(unitLang);
        }
        public void DeleteUnitLang(int unitId, string language)
        {
            DataProvider.Instance().DeleteUnitLang(unitId,language);
        }
        public void DeleteUnitLangs(int unitId)
        {
            DataProvider.Instance().DeleteUnitLangs(unitId);
        }
        #endregion

        #region ShippingModel
        public ShippingModelInfo GetShippingModel(int shippingModelId)
        {
            return (ShippingModelInfo)CBO.FillObject(DataProvider.Instance().GetShippingModel(shippingModelId),typeof(ShippingModelInfo));
        }
        public List<ShippingModelInfo> GetShippingModels(int portalId)
        {
            return CBO.FillCollection<ShippingModelInfo>(DataProvider.Instance().GetShippingModels(portalId));
        }
        public int NewShippingModel(ShippingModelInfo shippingModel)
        {
            return DataProvider.Instance().NewShippingModel(shippingModel);
        }
        public void UpdateShippingModel(ShippingModelInfo shippingModel)
        {
            DataProvider.Instance().UpdateShippingModel(shippingModel);
        }
        public void DeleteShippingModel(int shippingModelId)
        {
            DataProvider.Instance().DeleteShippingModel(shippingModelId);
        }
        #endregion

        #region ProductShippingModel
        public List<ProductShippingModelInfo> GetProductShippingModelsByProduct(int productId)
        {
            return CBO.FillCollection<ProductShippingModelInfo>(DataProvider.Instance().GetProductShippingModelsByProduct(productId));
        }

        public void DeleteProductShippingModelByProduct(int productId)
        {
            DataProvider.Instance().DeleteProductShippingModelByProduct(productId);
        }

        public void InsertProductShippingModel(ProductShippingModelInfo productShippingModel)
        {
            DataProvider.Instance().InsertProductShippingModel(productShippingModel);
        }
        #endregion

        #region ShippingCost methods
        public List<ShippingCostInfo> GetShippingCosts(int PortalId)
        {
            return CBO.FillCollection<ShippingCostInfo>(DataProvider.Instance().GetShippingCosts(PortalId));
        }
        public List<ShippingCostInfo> GetShippingCostsByModelId(int shippingModelId)
        {
            return CBO.FillCollection<ShippingCostInfo>(DataProvider.Instance().GetShippingCostsByModelId(shippingModelId));
        }
        public ShippingCostInfo GetShippingCostById(int ShippingCostId)
        {
            return (ShippingCostInfo)CBO.FillObject(DataProvider.Instance().GetShippingCostById(ShippingCostId), typeof(ShippingCostInfo));
        }
        public int NewShippingCost(ShippingCostInfo ShippingCost)
        {
            return DataProvider.Instance().NewShippingCost(ShippingCost);
        }
        public void UpdateShippingCost(ShippingCostInfo ShippingCost)
        {
            DataProvider.Instance().UpdateShippingCost(ShippingCost);
        }
        public void DeleteShippingCost(int ShippingCostId)
        {
            DataProvider.Instance().DeleteShippingCost(ShippingCostId);
        }
        #endregion

        #region Coupon methods
        public CouponInfo GetCouponById(int CouponId)
        {
            return (CouponInfo)CBO.FillObject(DataProvider.Instance().GetCouponById(CouponId), typeof(CouponInfo));
        }
        public CouponInfo GetCouponByCode(string code)
        {
            return (CouponInfo)CBO.FillObject(DataProvider.Instance().GetCouponByCode(code), typeof(CouponInfo));
        }
        public List<CouponInfo> GetCoupons(int PortalId)
        {
            return CBO.FillCollection<CouponInfo>(DataProvider.Instance().GetCoupons(PortalId));
        }
        public List<CouponInfo> GetCoupons(int portalId, string sortByField)
        {
            return CBO.FillCollection<CouponInfo>(DataProvider.Instance().GetCoupons(portalId, sortByField));
        }
        public int NewCoupon(CouponInfo Coupon)
        {
            return DataProvider.Instance().NewCoupon(Coupon);
        }
        public void UpdateCoupon(CouponInfo Coupon)
        {
            DataProvider.Instance().UpdateCoupon(Coupon);
        }
        public void UpdateCouponCount(int couponId, int count)
        {
            DataProvider.Instance().UpdateCouponCount(couponId,count);
        }
        public void DeleteCoupon(int CouponId)
        {
            DataProvider.Instance().DeleteCoupon(CouponId);
        }
        #endregion

        #region ShippingZone methods
        public int GetShippingZoneIdByAddress(int modelId, string countryCodeISO2, int postalCode)
        {
            return DataProvider.Instance().GetShippingZoneIdByAddress(modelId, countryCodeISO2, postalCode);
        }
        public ShippingZoneDisplayInfo GetShippingZoneById(int shippingZoneId, string language)
        {
            return (ShippingZoneDisplayInfo)CBO.FillObject(DataProvider.Instance().GetShippingZoneById(shippingZoneId, language), typeof(ShippingZoneDisplayInfo));
        }

        #endregion

        #region SearchFilters

        public string GetProductGroupFilter(int PortalId, int ProductGroupId, bool IncludeChilds)
        {
            return DataProvider.Instance().GetProductGroupFilter(PortalId, ProductGroupId, IncludeChilds);
        }
        public string GetSearchTextFilter(int PortalId, string SearchText, string Language)
        {
            return DataProvider.Instance().GetSearchTextFilter(PortalId, SearchText, Language);
        }
        public string GetSearchStaticFilter(int PortalId, string Token, string Language)
        {
            return DataProvider.Instance().GetSearchStaticFilter(PortalId, Token, Language);
        }
        public string GetSearchStaticFilter(int StaticFilterId, string Language)
        {
            return DataProvider.Instance().GetSearchStaticFilter(StaticFilterId, Language);
        }
        public string GetSearchPriceFilter(int PortalId, decimal StartPrice, decimal EndPrice, bool IncludeTax)
        {
            return DataProvider.Instance().GetSearchPriceFilter(PortalId, StartPrice, EndPrice,IncludeTax);
        }
        public string GetSearchFeatureFilter(string DataType, string Value)
        {
            return DataProvider.Instance().GetSearchFeatureFilter(DataType, Value);
        }
        public string GetSearchFeatureListFilter(int FeatureListId, int FeatureListItemId)
        {
            return DataProvider.Instance().GetSearchFeatureListFilter(FeatureListId, FeatureListItemId);
        }

        #endregion

        #endregion

        #region Helper methods

        public decimal GetDiscount(string DiscountLine, decimal Amount, decimal UnitCost, decimal TaxPercent)
        {
            // Discountline: 
            // 5[-10.000%],15[-20.000%] -> 5 or more 10 % off, 15 or more 20 % off
            // 5[-2.23N],10[-4.19N] -> 5 or more 2.23 net off, 10 or more 4.19 net off

            decimal retVal = UnitCost;
            if (DiscountLine != String.Empty)
            {
                // We need to find out the modificator which is relevant for our amount
                string[] discountValues = DiscountLine.Split(',');
                string modificator = "";
                foreach (string level in discountValues)
                {
                    int opgValAmount = 1;
                    if (level.IndexOf('[') > 0 && level.IndexOf(']') > 0)
                    {
                        string opgVal = level.Substring(0, level.IndexOf('['));
                        Int32.TryParse(opgVal, out opgValAmount);
                        if (Amount >= opgValAmount)
                        {
                            modificator = level.Substring(level.IndexOf('[') + 1);
                            continue;
                        }
                    }
                }

                // Now we have the correct discount and price contains the actual discount modificator
                if (modificator != String.Empty)
                {
                    // Strip closing bracket
                    modificator = modificator.Trim().Substring(0, modificator.Trim().Length - 1);

                    // Get kind of discount
                    string lastchar = modificator.Substring(modificator.Length - 1).ToUpper();
                    if (lastchar == "T" || lastchar == "%" || lastchar == "N" || lastchar == "B" || lastchar == "G")
                        modificator = modificator.Substring(0, modificator.Length - 1);
                    else
                        lastchar = " ";

                    decimal modificatorValue;
                    if (decimal.TryParse(modificator, NumberStyles.Number, CultureInfo.InvariantCulture, out modificatorValue))
                    {
                        switch (lastchar)
                        {
                            case "T":
                            case "B":
                            case "G":
                                retVal = UnitCost + modificatorValue / (1 + TaxPercent / 100);
                                break;
                            case "%":
                                retVal = UnitCost + (UnitCost * modificatorValue / 100);
                                break;
                            case "N":
                            case " ":
                                retVal = UnitCost + modificatorValue;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            return retVal;
        }
        public string GetDefaultTemplate(string TemplateSource)
        {
            StringBuilder sb = new StringBuilder();
            switch (TemplateSource)
            {
                case "SimpleProduct":
                    sb.AppendLine("<table cellpadding=\"0\" cellspacing=\"5\" border=\"0\">");
                    sb.AppendLine("  <tr>");
                    sb.AppendLine("    <td>[IMAGE:200]</td>");
                    sb.AppendLine("    <td><h5>[TITLE]</h5>[PRODUCTDESCRIPTION]</td>");
                    sb.AppendLine("  </tr>");
                    sb.AppendLine("</table>");
                    sb.AppendLine("<div align=\"right\">");
                    sb.AppendLine("  <table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"bbstore-product-cartbox\">");
                    sb.AppendLine("     <tr>");
                    sb.AppendLine("        <td colspan=\"2\">[PRODUCTOPTIONS]</td>");
                    sb.AppendLine("     </tr>");
                    sb.AppendLine("     <tr>");
                    sb.AppendLine("        <td colspan=\"2\" style=\"color:red\">[MANDATORYERROR]</td>");
                    sb.AppendLine("     </tr>");
                    sb.AppendLine("     <tr>");
                    sb.AppendLine("        <td style=\"font-size:x-large;white-space:nowrap\">[PRICE] [CURRENCY]</td>");
                    sb.AppendLine("        <td rowspan=\"2\" style=\"vertical-align:middle; text-align:center; padding-left:5px;white-space:nowrap\">[AMOUNT]&nbsp;[ADDCARTIMAGE]<br />[ADDCARTLINK]</td>");
                    sb.AppendLine("     </tr>");
                    sb.AppendLine("     <tr>");
                    sb.AppendLine("        <td>[TAX]</td>");
                    sb.AppendLine("     </tr>");
                    sb.AppendLine("  </table>");
                    sb.AppendLine("</div>");
                    break;

                case "ProductList":
                    sb.AppendLine("[IMAGE:80]<br /><span>[TITLE] [PRICE] [CURRENCY]</span>");
                    break;
                case "ProductGroup":
                    sb.AppendLine("[IMAGE]");
                    break;
                case "FeatureList":
                    sb.AppendLine("[IMAGE]<br />[TITLE]");
                    break;
                default:
                    break;
            }
            

            return sb.ToString();
        }
        public Hashtable GetStoreSettings(int portalId)
        {
            ModuleController moduleController = new ModuleController();
            ModuleInfo adminModule = moduleController.GetModuleByDefinition(portalId, "BBStore Admin");
            if (adminModule != null)
                return moduleController.GetModuleSettings(adminModule.ModuleID);
            throw new Exception("No admin module found in portal");
        }

        public Hashtable GetCartSettings(int portalId)
        {
            ModuleController moduleController = new ModuleController();
            ModuleInfo cartModule = moduleController.GetModuleByDefinition(portalId, "BBStore Cart");
            if (cartModule != null)
                return moduleController.GetModuleSettings(cartModule.ModuleID);
            throw new Exception("No cart module found in portal");
        }

        private void CheckCss(string colorBorder, string colorHead, string colorRow, string colorAlt, string colorSum)
        {
            string cssFile = PathUtils.Instance.MapPath("~/DesktopModules/BBStore/module.css");
            if (!File.Exists(cssFile))
            {
                string cssTemplate = File.ReadAllText(PathUtils.Instance.MapPath("~/DesktopModules/BBStore/Templates/module.css.template"));
                cssTemplate = cssTemplate.Replace("[COLORBORDER]", "#" + colorBorder);
                cssTemplate = cssTemplate.Replace("[COLORHEAD]", "#" + colorHead);
                cssTemplate = cssTemplate.Replace("[COLORROW]", "#" + colorRow);
                cssTemplate = cssTemplate.Replace("[COLORALT]", "#" + colorAlt);
                cssTemplate = cssTemplate.Replace("[COLORSUM]", "#" + colorSum);
                cssTemplate = cssTemplate.Replace("<IMAGEPATH>", "Controls/Images/");
                File.WriteAllText(cssFile,cssTemplate);
            }
        }

        public string CheckStore(int portalId, bool checkOnly)
        {
            // First lets check the language
            StringBuilder result = new StringBuilder();
            LocaleController lc = new LocaleController();
            Dictionary<string, Locale> loc = lc.GetLocales(portalId);
            string defaultLanguage = PortalSettings.Current.DefaultLanguage;
            string verb = (checkOnly ? "missing" : "added");

            try
            {
                #region Check OrderState
                List<OrderStateInfo> orderStates = GetOrderStates(portalId,"en-US");
                
                int orderStateId;
                OrderStateLangInfo orderStateLang;
                OrderStateInfo newOrderState = new OrderStateInfo() { PortalId = portalId };

                if (!(from o in orderStates where o.OrderState == "created" select o).Any())
                {
                    
                    orderStateId = NewOrderState(newOrderState);
                    orderStateLang = new OrderStateLangInfo
                                     {
                                         OrderStateId = orderStateId,
                                         Language = "en-US",
                                         OrderState = "created"
                                     };
                    if (!checkOnly) NewOrderStateLang(orderStateLang);
                    result.AppendLine($"OrderState: OrderState '{orderStateLang.OrderState}' for language '{orderStateLang.Language}' {verb}");
                }

                if (!(from o in orderStates where o.OrderState == "confirmed" select o).Any())
                {
                    orderStateId = NewOrderState(newOrderState);
                    orderStateLang = new OrderStateLangInfo
                                     {
                                         OrderStateId = orderStateId,
                                         Language = "en-US",
                                         OrderState = "confirmed"
                                     };
                    if (!checkOnly) NewOrderStateLang(orderStateLang);
                    result.AppendLine($"OrderState: OrderState '{orderStateLang.OrderState}' for language '{orderStateLang.Language}' {verb}");
                }

                if (!(from o in orderStates where o.OrderState == "payed" select o).Any())
                {
                    orderStateId = NewOrderState(newOrderState);
                    orderStateLang = new OrderStateLangInfo
                                     {
                                         OrderStateId = orderStateId,
                                         Language = "en-US",
                                         OrderState = "payed"
                                     };
                    if (!checkOnly) NewOrderStateLang(orderStateLang);
                    result.AppendLine($"OrderState: OrderState '{orderStateLang.OrderState}' for language '{orderStateLang.Language}' {verb}");
                }
                if (!(from o in orderStates where o.OrderState == "cancelled" select o).Any())
                {
                    orderStateId = NewOrderState(newOrderState);
                    orderStateLang = new OrderStateLangInfo
                                     {
                                         OrderStateId = orderStateId,
                                         Language = "en-US",
                                         OrderState = "cancelled"
                                     };
                    if (!checkOnly) NewOrderStateLang(orderStateLang);
                    result.AppendLine($"OrderState: OrderState '{orderStateLang.OrderState}' for language '{orderStateLang.Language}' {verb}");
                }
                if (!(from o in orderStates where o.OrderState == "shipped" select o).Any())
                {
                    orderStateId = NewOrderState(newOrderState);
                    orderStateLang = new OrderStateLangInfo
                                     {
                                         OrderStateId = orderStateId,
                                         Language = "en-US",
                                         OrderState = "shipped"
                                     };
                    if (!checkOnly) NewOrderStateLang(orderStateLang);
                    result.AppendLine($"OrderState: OrderState '{orderStateLang.OrderState}' for language '{orderStateLang.Language}' {verb}");
                }

                #endregion

                #region Check SubscriberAddressType

                List<SubscriberAddressTypeInfo> subscriberAddressTypes = GetSubscriberAddressTypesByPortal(portalId);
                bool orderAddressExists = (from s in subscriberAddressTypes where s.IsOrderAddress select s).Any();
                if (!orderAddressExists)
                {
                    SubscriberAddressTypeInfo billAddressType =
                        (from s in subscriberAddressTypes where s.KzAddressType.ToLower() == "billing" select s)
                            .FirstOrDefault();

                    if (billAddressType == null)
                    {
                        billAddressType = new SubscriberAddressTypeInfo(){IsOrderAddress = true, KzAddressType = "Billing", Mandatory = true,PortalId = portalId};
                        if (!checkOnly) NewSubscriberAddressType(billAddressType);
                    }
                    else
                    {
                        billAddressType.IsOrderAddress = true;
                        if (!checkOnly) UpdateSubscriberAddressType(billAddressType);
                    }
                }

                #endregion

                #region Check Unit

                List<UnitInfo> units = GetUnits(portalId,"en-US","unitid");
                if (!(from u in units where u.Unit == "Piece" select u).Any())
                {
                    UnitInfo unit = new UnitInfo
                                    {
                                        Decimals = 0,
                                        PortalId = portalId
                                    };
                    int unitId = NewUnit(unit);
                    UnitLangInfo unitLang = new UnitLangInfo();
                    unitLang.UnitId = unitId;
                    unitLang.Unit = "Piece";
                    unitLang.Symbol = "pcs.";
                    unitLang.Language = "en-US";
                    
                    if (!checkOnly) NewUnitLang(unitLang);
                    result.AppendLine($"Unit: Unit '{unitLang.Unit}' for language '{unitLang.Language}' {verb}");
                }
                #endregion

                #region Check OrderStateLang

                orderStates = GetOrderStates(portalId);
                foreach (OrderStateInfo orderState in orderStates)
                {
                    List<OrderStateLangInfo> orderStateLangs = GetOrderStateLangs(orderState.OrderStateId);
                    foreach (KeyValuePair<string, Locale> keyValuePair in loc)
                    {
                        string language = keyValuePair.Key;
                        bool exists = (from l in orderStateLangs where l.Language == language select l).Any();
                        if (!exists)
                        {
                            OrderStateLangInfo def = (from l in orderStateLangs where l.Language == defaultLanguage select l).FirstOrDefault();
                            OrderStateLangInfo newOrderStateLang = new OrderStateLangInfo();
                            if (def == null)
                            {
                                newOrderStateLang.OrderState = "### " + language + ": " + orderState.OrderStateId.ToString();
                            }
                            else
                            {
                                newOrderStateLang.OrderState = "### " + language + ": " + def.OrderState;
                            }
                            newOrderStateLang.Language = language;
                            newOrderStateLang.OrderStateId = orderState.OrderStateId;
                            if (!checkOnly) NewOrderStateLang(newOrderStateLang);
                            result.AppendLine($"OrderStateLang: OrderStateId({orderState.OrderStateId}) language({language}) {verb}");
                        }
                    }
                }
                #endregion

                #region check PaymentProviderLang
                List<PaymentProviderInfo> paymentProviders = GetPaymentProviders("en-US");
                foreach (PaymentProviderInfo paymentProvider in paymentProviders)
                {
                    List<PaymentProviderLangInfo> paymentProviderLangs = GetPaymentProviderLangs(paymentProvider.PaymentProviderId);
                    foreach (KeyValuePair<string, Locale> keyValuePair in loc)
                    {
                        string language = keyValuePair.Key;
                        bool exists = (from l in paymentProviderLangs where l.Language == language select l).Any();
                        if (!exists)
                        {
                            PaymentProviderLangInfo def = (from l in paymentProviderLangs where l.Language == defaultLanguage select l).FirstOrDefault();
                            PaymentProviderLangInfo newPaymentProviderLang = new PaymentProviderLangInfo();
                            if (def == null)
                            {
                                newPaymentProviderLang.ProviderName = "### " + language + ": " + paymentProvider.PaymentProviderId.ToString();
                            }
                            else
                            {
                                newPaymentProviderLang.ProviderName = "### " + language + ": " + def.ProviderName;
                            }
                            newPaymentProviderLang.Language = language;
                            newPaymentProviderLang.PaymentProviderId = paymentProvider.PaymentProviderId;
                            if (!checkOnly) NewPaymentProviderLang(newPaymentProviderLang);
                            result.AppendLine($"PaymentProviderLang: PaymentProviderId({paymentProvider.PaymentProviderId}) language({language}) {verb}");
                        }
                    }
                }
                #endregion

                #region Check FeatureGroupLang
                List<FeatureGroupInfo> featureGroups = GetFeatureGroups(portalId);
                foreach (FeatureGroupInfo featureGroup in featureGroups)
                {
                    List<FeatureGroupLangInfo> groupLangs = GetFeatureGroupLangs(featureGroup.FeatureGroupId);
                    foreach (KeyValuePair<string, Locale> keyValuePair in loc)
                    {
                        string language = keyValuePair.Key;
                        bool exists = (from l in groupLangs where l.Language == language select l).Any();
                        if (!exists)
                        {
                            FeatureGroupLangInfo def = (from l in groupLangs where l.Language == defaultLanguage select l).FirstOrDefault();
                            FeatureGroupLangInfo newFeatureGroupLang = new FeatureGroupLangInfo();
                            if (def == null)
                            {
                                newFeatureGroupLang.FeatureGroup = "### " + language + ": " + featureGroup.FeatureGroupId.ToString();
                            }
                            else
                            {
                                newFeatureGroupLang.FeatureGroup = "### " + language + ": " + def.FeatureGroup;
                            }
                            newFeatureGroupLang.Language = language;
                            newFeatureGroupLang.FeatureGroupId = featureGroup.FeatureGroupId;
                            if (!checkOnly) NewFeatureGroupLang(newFeatureGroupLang);
                            result.AppendLine($"FeatureGroupLang: FeatureGroupId({featureGroup.FeatureGroupId}) language({language}) {verb}");
                        }
                    }
                }
                #endregion

                #region Check FeatureLang
                List<FeatureInfo> features = GetFeatures(portalId);
                foreach (FeatureInfo feature in features)
                {
                    List<FeatureLangInfo> langs = GetFeatureLangs(feature.FeatureId);
                    foreach (KeyValuePair<string, Locale> keyValuePair in loc)
                    {
                        string language = keyValuePair.Key;
                        bool exists = (from l in langs where l.Language == language select l).Any();
                        if (!exists)
                        {
                            FeatureLangInfo def = (from l in langs where l.Language == defaultLanguage select l).FirstOrDefault();
                            FeatureLangInfo newFeatureLang = new FeatureLangInfo();
                            if (def == null)
                            {
                                newFeatureLang.Feature = $"### {language}: {feature.FeatureToken} ({feature.FeatureId.ToString()})";
                                newFeatureLang.Unit = "###";
                            }
                            else
                            {
                                newFeatureLang.Feature = "### " + language + ": " + def.Feature;
                                newFeatureLang.Unit = "### " + def.Unit;
                            }
                            if (newFeatureLang.Feature.Length > 40)
                                newFeatureLang.Feature = newFeatureLang.Feature.Substring(0, 40);
                            if (newFeatureLang.Unit.Length > 20)
                                newFeatureLang.Unit = newFeatureLang.Unit.Substring(0, 20);
                            newFeatureLang.Language = language;
                            newFeatureLang.FeatureId = feature.FeatureId;
                            if (!checkOnly) NewFeatureLang(newFeatureLang);
                            result.AppendLine($"FeatureLang: FeatureId({feature.FeatureId}) language({language}) {verb}");
                        }
                    }
                }
                #endregion

                #region Check FeatureListLang
                List<FeatureListInfo> featureLists = GetFeatureLists(portalId);
                foreach (FeatureListInfo featureList in featureLists)
                {
                    List<FeatureListLangInfo> listLangs = GetFeatureListLangs(featureList.FeatureListId);
                    foreach (KeyValuePair<string, Locale> keyValuePair in loc)
                    {
                        string language = keyValuePair.Key;
                        bool exists = (from l in listLangs where l.Language == language select l).Any();
                        if (!exists)
                        {
                            FeatureListLangInfo def = (from l in listLangs where l.Language == defaultLanguage select l).FirstOrDefault();
                            FeatureListLangInfo newFeatureListLang = new FeatureListLangInfo();
                            if (def == null)
                            {
                                newFeatureListLang.FeatureList = "### " + language + ": " + featureList.FeatureListId.ToString();
                            }
                            else
                            {
                                newFeatureListLang.FeatureList = "### " + language + ": " + def.FeatureList;
                            }
                            if (newFeatureListLang.FeatureList.Length > 40)
                                newFeatureListLang.FeatureList = newFeatureListLang.FeatureList.Substring(0, 40);
                            newFeatureListLang.Language = language;
                            newFeatureListLang.FeatureListId = featureList.FeatureListId;
                            if (!checkOnly) NewFeatureListLang(newFeatureListLang);
                            result.AppendLine($"FeatureListLang: FeatureListId({featureList.FeatureListId}) language({language}) {verb}");
                        }
                    }
                }
                #endregion

                #region Check FeatureListItemLang
                List<FeatureListItemInfo> featureListItems = GetFeatureListItems(portalId);
                foreach (FeatureListItemInfo featureListItem in featureListItems)
                {
                    List<FeatureListItemLangInfo> listItemLangs = GetFeatureListItemLangs(featureListItem.FeatureListItemId);
                    foreach (KeyValuePair<string, Locale> keyValuePair in loc)
                    {
                        string language = keyValuePair.Key;
                        bool exists = (from l in listItemLangs where l.Language == language select l).Any();
                        if (!exists)
                        {
                            FeatureListItemLangInfo def = (from l in listItemLangs where l.Language == defaultLanguage select l).FirstOrDefault();
                            FeatureListItemLangInfo newFeatureListItemLang = new FeatureListItemLangInfo();
                            if (def == null)
                            {
                                newFeatureListItemLang.FeatureListItem = "### " + language + ": " + featureListItem.FeatureListItemId.ToString();
                            }
                            else
                            {
                                newFeatureListItemLang.FeatureListItem = "### " + language + ": " + def.FeatureListItem;
                            }
                            if (newFeatureListItemLang.FeatureListItem.Length > 60)
                                newFeatureListItemLang.FeatureListItem = newFeatureListItemLang.FeatureListItem.Substring(0, 60);
                            newFeatureListItemLang.Language = language;
                            newFeatureListItemLang.FeatureListItemId = featureListItem.FeatureListItemId;
                            if (!checkOnly) NewFeatureListItemLang(newFeatureListItemLang);
                            result.AppendLine($"FeatureListItemLang: FeatureListItemId({featureListItem.FeatureListItemId}) language({language}) {verb}");
                        }
                    }
                }
                #endregion

                #region Check ProductGroupLang
                List<ProductGroupInfo> productGroups = GetProductGroups(portalId);
                foreach (ProductGroupInfo productGroup in productGroups)
                {
                    List<ProductGroupLangInfo> productGroupLangs = GetProductGroupLangs(productGroup.ProductGroupId);
                    foreach (KeyValuePair<string, Locale> keyValuePair in loc)
                    {
                        string language = keyValuePair.Key;
                        bool exists = (from l in productGroupLangs where l.Language == language select l).Any();
                        if (!exists)
                        {
                            ProductGroupLangInfo def = (from l in productGroupLangs where l.Language == defaultLanguage select l).FirstOrDefault();
                            ProductGroupLangInfo newProductGroupLang = new ProductGroupLangInfo();
                            if (def == null)
                            {
                                newProductGroupLang.ProductGroupName = "### " + language + ": " + productGroup.ProductGroupId.ToString();
                                newProductGroupLang.ProductGroupDescription = "### " + language + ": " + productGroup.ProductGroupId.ToString();
                                newProductGroupLang.ProductGroupShortDescription = "### " + language + ": " + productGroup.ProductGroupId.ToString();
                            }
                            else
                            {
                                newProductGroupLang.ProductGroupName = "### " + language + ": " + def.ProductGroupName;
                                newProductGroupLang.ProductGroupDescription = "### " + language + ": " + def.ProductGroupDescription;
                                newProductGroupLang.ProductGroupShortDescription = "### " + language + ": " + def.ProductGroupShortDescription;
                            }
                            if (newProductGroupLang.ProductGroupName.Length > 120)
                                newProductGroupLang.ProductGroupName = newProductGroupLang.ProductGroupName.Substring(0, 120);
                            if (newProductGroupLang.ProductGroupShortDescription.Length > 500)
                                newProductGroupLang.ProductGroupShortDescription = newProductGroupLang.ProductGroupShortDescription.Substring(0, 120);

                            newProductGroupLang.Language = language;
                            newProductGroupLang.ProductGroupId = productGroup.ProductGroupId;
                            if (!checkOnly) NewProductGroupLang(newProductGroupLang);
                            result.AppendLine($"ProductGroupLang: ProductGroupId({productGroup.ProductGroupId}) language({language}) {verb}");
                        }
                    }
                }

                #endregion

                #region Check SimpleProductLang
                List<SimpleProductInfo> products = GetSimpleProducts(portalId);
                foreach (SimpleProductInfo product in products)
                {
                    List<SimpleProductLangInfo> productLangs = GetSimpleProductLangs(product.SimpleProductId);
                    foreach (KeyValuePair<string, Locale> keyValuePair in loc)
                    {
                        string language = keyValuePair.Key;
                        bool exists = (from l in productLangs where l.Language == language select l).Any();
                        if (!exists)
                        {
                            SimpleProductLangInfo def = (from l in productLangs where l.Language == defaultLanguage select l).FirstOrDefault();
                            SimpleProductLangInfo newProductLang = new SimpleProductLangInfo();
                            if (def == null)
                            {
                                newProductLang.Name = "### " + language + ": " + product.SimpleProductId.ToString();
                                newProductLang.ShortDescription = "### " + language + ": " + product.SimpleProductId.ToString();
                                newProductLang.ProductDescription = "### " + language + ": " + product.SimpleProductId.ToString();
                                newProductLang.Attributes = "";
                            }
                            else
                            {
                                newProductLang.Name = "### " + language + ": " + def.Name;
                                newProductLang.ShortDescription = "### " + language + ": " + def.ShortDescription;
                                newProductLang.ProductDescription = "### " + language + ": " + def.ProductDescription;
                                if (def.Attributes != String.Empty)
                                {
                                    string[] attributes = def.Attributes.Split(new string[] {"\r\n","\r","\n"},
                                                                               StringSplitOptions.None);
                                    StringBuilder sb = new StringBuilder();
                                    foreach (string attribute in attributes)
                                    {
                                        string[] props = attribute.Split(':');
                                        if (props[0].StartsWith("^") || props[0].StartsWith("!"))
                                        {
                                            string prefix = props[0].Substring(0, 1);
                                            string name = props[0].Substring(1);
                                            string proplist = attribute.Substring(name.Length + 1);
                                            sb.AppendLine(prefix + "### " + language + "_" + name + proplist);
                                        }
                                        else
                                        {
                                            string name = props[0];
                                            string proplist = attribute.Substring(name.Length);
                                            sb.AppendLine("### " + language + "_" + name + proplist);
                                        }
                                        newProductLang.Attributes = sb.ToString();
                                    }
                                }
                            }
                            newProductLang.Language = language;
                            newProductLang.SimpleProductId = product.SimpleProductId;
                            if (!checkOnly) NewSimpleProductLang(newProductLang);
                            result.AppendLine($"SimpleProductLang: SimpleProductId({product.SimpleProductId}) language({language}) {verb}");
                        }
                    }
                }

                #endregion

                #region Check SubscriberAddressTypeLang
                List<SubscriberAddressTypeInfo> subScriberAddressTypes = GetSubscriberAddressTypesByPortal(portalId);
                foreach (SubscriberAddressTypeInfo subscriberAddressType in subScriberAddressTypes)
                {
                    List<SubscriberAddressTypeLangInfo> listLangs = GetSubscriberAddressTypeLangs(subscriberAddressType.SubscriberAddressTypeId);
                    foreach (KeyValuePair<string, Locale> keyValuePair in loc)
                    {
                        string language = keyValuePair.Key;
                        bool exists = (from l in listLangs where l.Language == language select l).Any();
                        if (!exists)
                        {
                            SubscriberAddressTypeLangInfo def = (from l in listLangs where l.Language == defaultLanguage select l).FirstOrDefault();
                            SubscriberAddressTypeLangInfo newSubscriberAddressTypeLang = new SubscriberAddressTypeLangInfo();
                            if (def == null)
                            {
                                newSubscriberAddressTypeLang.AddressType = "### " + language + ": " + subscriberAddressType.SubscriberAddressTypeId.ToString();
                            }
                            else
                            {
                                newSubscriberAddressTypeLang.AddressType = "### " + language + ": " + def.AddressType;
                            }
                            newSubscriberAddressTypeLang.Language = language;
                            newSubscriberAddressTypeLang.SubscriberAddressTypeId = subscriberAddressType.SubscriberAddressTypeId;
                            if (!checkOnly) NewSubscriberAddressTypeLang(newSubscriberAddressTypeLang);
                            result.AppendLine($"SubscriberAddressTypeLang: SubscriberAddresstypeid({subscriberAddressType.SubscriberAddressTypeId}) language({language}) {verb}");
                        }
                    }
                }
                #endregion

                #region Check UnitLang

                units = GetUnits(portalId);
                foreach (UnitInfo unit in units)
                {
                    List<UnitLangInfo> langs = GetUnitLangs(unit.UnitId);
                    foreach (KeyValuePair<string, Locale> keyValuePair in loc)
                    {
                        string language = keyValuePair.Key;
                        bool exists = (from l in langs where l.Language == language select l).Any();
                        if (!exists)
                        {
                            UnitLangInfo def = (from l in langs where l.Language == defaultLanguage select l).FirstOrDefault();
                            UnitLangInfo newUnitLang = new UnitLangInfo();
                            if (def == null)
                            {
                                newUnitLang.Unit = "### " + language + ": Unit (" + unit.UnitId.ToString()+")";
                                newUnitLang.Symbol = "### " + language + ": Symbol (" + unit.UnitId.ToString() + ")";
                            }
                            else
                            {
                                newUnitLang.Unit = "### " + language + ": " + def.Unit;
                                newUnitLang.Symbol = "### " + def.Symbol;
                            }
                            if (newUnitLang.Unit.Length > 20)
                                newUnitLang.Unit = newUnitLang.Unit.Substring(0, 20);
                            if (newUnitLang.Symbol.Length > 10)
                                newUnitLang.Symbol = newUnitLang.Symbol.Substring(0, 10);
                            newUnitLang.Language = language;
                            newUnitLang.UnitId = unit.UnitId;
                            if (!checkOnly) NewUnitLang(newUnitLang);
                            result.AppendLine($"UnitLang: unitId({unit.UnitId}) language({language}) {verb}");
                        }
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                result.AppendLine(ex.ToString());
            }
            return result.ToString();
        }

        public void ReseedTables()
        {
            DataProvider.Instance().ReseedTables();
        }

        public LicenseDataInfo GetLicense(int portalId, bool forceUpdate)
        {
            return new LicenseDataInfo("bitboxx.net", "BB", -1, 255, 2, null);

            //EventLogController objEventLog = new EventLogController();

            //string message = "";
            //LicenseDataInfo license;
            //string password = "geheim";
            //string cacheKey = "bbstore_license_" + portalId.ToString();

            //if (!forceUpdate)
            //{
            //    license = (LicenseDataInfo)DataCache.GetCache(cacheKey);
            //    if (license != null)
            //    {
            //        objEventLog.AddLog("BBStore_License", "Fetching BBStore License from Cache", PortalSettings.Current, -1, EventLogController.EventLogType.PORTAL_SETTING_UPDATED);
            //        return license;
            //    }
            //}

            //IPAddress hostIp = Dns.GetHostAddresses(PortalSettings.Current.PortalAlias.HTTPAlias)[0];
            //IPAddress localHost = IPAddress.Parse("127.0.0.1");
            //if (hostIp.Equals(localHost))
            //{
            //    objEventLog.AddLog("BBStore_License", "HostIP equals localhost", PortalSettings.Current, -1, EventLogController.EventLogType.PORTAL_SETTING_UPDATED);
            //    return new LicenseDataInfo("127.0.0.1", "00", -1, 255, 0, null);
            //}

            //ModuleController objModules = new ModuleController();
            //ModuleInfo adminModule = objModules.GetModuleByDefinition(portalId, "BBStore Admin");
            //if (adminModule != null)
            //{
            //    Hashtable adminSettings = objModules.GetModuleSettings(adminModule.ModuleID);

            //    string initialKey = (string)adminSettings["InitialKey"];

            //    objEventLog.AddLog("BBStore_License", String.Format("InitialKey is {0}", initialKey), PortalSettings.Current, -1, EventLogController.EventLogType.PORTAL_SETTING_UPDATED);
            //    if (String.IsNullOrEmpty(initialKey))
            //        return null;

            //    string customerTag = hostIp.ToString();
            //    objEventLog.AddLog("BBStore_License", String.Format("CustomerTag (1) is {0}", customerTag), PortalSettings.Current, -1, EventLogController.EventLogType.PORTAL_SETTING_UPDATED);

            //    int productId, customerId;
            //    if (!LicenseClient.CheckInitialKey(initialKey, customerTag, out productId, out customerId))
            //    {
            //        customerTag = PortalSettings.Current.PortalAlias.HTTPAlias;

            //        if (VfpInterop.Occurs(".", customerTag) > 1)
            //            customerTag = customerTag.Substring(customerTag.IndexOf('.') + 1);

            //        objEventLog.AddLog("BBStore_License", String.Format("CustomerTag (2) is {0}", customerTag), PortalSettings.Current, -1, EventLogController.EventLogType.PORTAL_SETTING_UPDATED);

            //        if (!LicenseClient.CheckInitialKey(initialKey, customerTag, out productId, out customerId))
            //        {
            //            objEventLog.AddLog("BBStore_License", "CheckInitialKey failed (2)", PortalSettings.Current, -1, EventLogController.EventLogType.PORTAL_SETTING_UPDATED);
            //            return null;
            //        }
            //    }
            //    else
            //    {
            //        objEventLog.AddLog("BBStore_License", "CheckInitialKey failed (1)", PortalSettings.Current, -1, EventLogController.EventLogType.PORTAL_SETTING_UPDATED);
            //    }

            //    DateTime lastLicenseRead = DateTime.Parse((string)adminSettings["LastLicenseRead"] ?? "1970/01/01", CultureInfo.InvariantCulture);
            //    int licenseReadRetries = Int32.Parse((string)adminSettings["LicenseReadRetries"] ?? "0", CultureInfo.InvariantCulture);

            //    // We only contact license server once a day
            //    string licenseKey = (string)adminSettings["LicenseKey"] ?? "";
            //    if (lastLicenseRead < DateTime.Now - new TimeSpan(24, 0, 0) || forceUpdate)
            //    {
            //        try
            //        {
            //            licenseKey = LicenseClient.GetLicenseKey(productId, customerId, customerTag);
            //            objEventLog.AddLog("BBStore_License", "Fetching Successfull", PortalSettings.Current, -1, EventLogController.EventLogType.PORTAL_SETTING_UPDATED);
            //            objModules.UpdateModuleSetting(adminModule.ModuleID, "LicenseKey", licenseKey);
            //            objModules.UpdateModuleSetting(adminModule.ModuleID, "LicenseReadRetries", "0");
            //            objModules.UpdateModuleSetting(adminModule.ModuleID, "LastLicenseRead", DateTime.Now.ToString(CultureInfo.InvariantCulture));
            //        }
            //        catch (Exception)
            //        {
            //            licenseReadRetries++;
            //            objEventLog.AddLog("BBStore_License", "Fetching Failed (" + licenseReadRetries.ToString() + " try of 30)", PortalSettings.Current, -1, EventLogController.EventLogType.PORTAL_SETTING_UPDATED);
            //            if (!forceUpdate)
            //                objModules.UpdateModuleSetting(adminModule.ModuleID, "LicenseReadRetries", licenseReadRetries.ToString());
            //            objModules.UpdateModuleSetting(adminModule.ModuleID, "LastLicenseRead", (DateTime.Now - new TimeSpan(12, 0, 0)).ToString(CultureInfo.InvariantCulture));
            //            if (licenseReadRetries >= 30)
            //                return null;
            //        }
            //    }
            //    if (String.IsNullOrEmpty(licenseKey))
            //        return null;

            //    license = LicenseClient.CheckLicenseKey(customerTag, licenseKey, password);

            //    int timeOut = Convert.ToInt32(Host.PerformanceSetting);
            //    if (timeOut > 0 & license != null)
            //    {
            //        DataCache.SetCache(cacheKey, license, TimeSpan.FromMinutes(timeOut));
            //    }

            //    return license;
            //}
            //else
            //{
            //    objEventLog.AddLog("BBStore_License", "No admin module", PortalSettings.Current, -1, EventLogController.EventLogType.HOST_SETTING_UPDATED);
            //}
            //return null;

        }

        public void CheckLicense(LicenseDataInfo license, PortalModuleBase callingControl, ModuleKindEnum moduleKind)
        {
            // Get Module version number
            Version moduleVersion = new Version(0,0);
            DesktopModuleInfo deskModInfo = DesktopModuleController.GetDesktopModule(callingControl.ModuleConfiguration.DesktopModuleID, callingControl.PortalId);
            PackageInfo pkgInfo = PackageController.GetPackage(deskModInfo.PackageID);

            string message = "";

            if (pkgInfo != null)
                moduleVersion = pkgInfo.Version;
            
            if (license == null)
            {
                message = "No valid license!";
                Exceptions.ProcessModuleLoadException(message, callingControl, new Exception(message));
            } 
            else if (license.Tag == "127.0.0.1")
            {
                string titleLabelName = DotNetNukeContext.Current.Application.Version.Major < 6 ? "lblTitle" : "titleLabel";
                Label lbl = Globals.FindControlRecursiveDown(callingControl.ContainerControl, titleLabelName) as Label;
                if (lbl != null)
                {
                    lbl.Text = "BBSTORE-DEMO: " + lbl.Text;
                }
            }
            else if (license.Version < moduleVersion.Major )
            {
                message = "No valid license for version " + moduleVersion.ToString() + "!";
                Exceptions.ProcessModuleLoadException(message, callingControl, new Exception(message));
            }
            else if (license != null && license.ValidUntil < DateTime.Now)
            {
                message = $"License expired on {((DateTime) license.ValidUntil).Date}";
                Exceptions.ProcessModuleLoadException(message, callingControl, new Exception(message));
            }
            else
            {
                ModuleKindEnum licenseModuleKindEnum = (ModuleKindEnum) license.Modules;
                if ((licenseModuleKindEnum & moduleKind) != moduleKind)
                {
                    message = $"No valid license for module {licenseModuleKindEnum.ToString("F")} !";
                    Exceptions.ProcessModuleLoadException(message, callingControl, new Exception(message));
                }
            }
        }

        #endregion

        #region Optional Interfaces

        /// ----------------------------------------------------------------------------- 
        /// <summary> 
        /// GetSearchItems implements the ISearchable Interface 
        /// </summary> 
        /// <remarks> 
        /// </remarks> 
        /// <param name="ModInfo">The ModuleInfo for the module to be Indexed</param> 
        /// <history> 
        /// </history> 
        /// ----------------------------------------------------------------------------- 
        //public DotNetNuke.Services.Search.SearchItemInfoCollection GetSearchItems(ModuleInfo ModInfo)
        //{

        //    SearchItemInfoCollection SearchItemCollection = new SearchItemInfoCollection();

        //    // TODO Die Defaultsprache des Portals als Suchsprache verwenden
        //    int ProductId = GetModuleProductId(ModInfo.PortalID, ModInfo.ModuleID);
        //    if (ProductId > 0)
        //    {
        //        SimpleProductInfo Product = GetSimpleProductByProductId(ModInfo.PortalID, ProductId, System.Threading.Thread.CurrentThread.CurrentCulture.Name);
        //        SearchItemInfo SearchItem = new SearchItemInfo(ModInfo.ModuleTitle, Product.ProductDescription, ModInfo.CreatedByUserID, ModInfo.CreatedOnDate, ModInfo.ModuleID, "BBStore", Product.ProductDescription);
        //        SearchItemCollection.Add(SearchItem);
        //    }
        //    return SearchItemCollection;

        //}

        /// ----------------------------------------------------------------------------- 
        /// <summary> 
        /// ExportModule implements the IPortable ExportModule Interface 
        /// </summary> 
        /// <remarks> 
        /// </remarks> 
        /// <param name="ModuleID">The Id of the module to be exported</param> 
        /// <history> 
        /// </history> 
        /// ----------------------------------------------------------------------------- 
        //public string ExportModule(int ModuleID)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    ModuleController objModules = new ModuleController();
        //    ModuleInfo mod = objModules.GetModule(ModuleID);

        //    List<SimpleProductInfo> products = GetSimpleProducts(mod.PortalID);

        //    sb.Append("<BBStore>");
        //    sb.Append("<Products>");
        //    foreach (SimpleProductInfo product in products)
        //    {
        //        sb.Append("<Product>");
        //        sb.Append("<SimpleProductId>" + product.SimpleProductId.ToString() + "</SimpleProductId>");
        //        sb.Append("<SubscriberId>" + product.SubscriberId.ToString() + "</SubscriberId>");
        //        sb.Append("<Image>" + XmlUtils.XMLEncode(product.Image) + "</Image>");
        //        sb.Append("<ItemNo>" + XmlUtils.XMLEncode(product.ItemNo) + "</ItemNo>");
        //        sb.Append("<ItemNo>" + product.UnitCost.ToString(CultureInfo.InvariantCulture) + "</ItemNo>");
        //        sb.Append("<OriginalUnitCost>" + product.OriginalUnitCost.ToString(CultureInfo.InvariantCulture) + "</OriginalUnitCost>");
        //        sb.Append("<HideCost>" + product.HideCost.ToString() + "</HideCost>");
        //        sb.Append("<TaxPercent>" + product.TaxPercent.ToString(CultureInfo.InvariantCulture) + "</TaxPercent>");
        //        sb.Append("<Disabled>" + product.Disabled.ToString() + "</Disabled>");
        //        sb.Append("<NoCart>" + product.NoCart.ToString() + "</NoCart>");
                
        //        List<SimpleProductLangInfo> langs = GetSimpleProductLangs(product.SimpleProductId);
        //        sb.Append("<ProductLangs>");
        //        foreach (SimpleProductLangInfo lang in langs)
        //        {
        //            sb.Append("<ProductLang>");
        //            sb.Append("<Language>" + lang.Language + "</Language>");
        //            sb.Append("<Name>" + XmlUtils.XMLEncode(lang.Name) + "</Name>");
        //            sb.Append("<ShortDescription>" + XmlUtils.XMLEncode(lang.ShortDescription) + "</ShortDescription>");
        //            sb.Append("<ProductDescription>" + XmlUtils.XMLEncode(lang.ProductDescription) + "</ProductDescription>");
        //            sb.Append("<Attributes>" + XmlUtils.XMLEncode(lang.Attributes) + "</Attributes>");
        //            sb.Append("</ProductLang>");
        //        }
        //        sb.Append("</ProductLangs>");
        //        sb.Append("</Product>");
        //    }
        //    sb.Append("</Products>");

        //    List<FeatureGroupInfo> groups = GetFeatureGroups(mod.PortalID);
        //    sb.Append("<FeatureGroups>");
        //    foreach (FeatureGroupInfo group in groups)
        //    {
        //        sb.Append("<FeatureGroup>");
        //        sb.Append("<FeatureGroupId>" + group.FeatureGroupId.ToString() + "</FeatureGroupId>");
        //        List<FeatureGroupLangInfo> langs = GetFeatureGroupLangs(group.FeatureGroupId);
        //        sb.Append("<FeatureGroupLangs>");
        //        foreach (FeatureGroupLangInfo lang in langs)
        //        {
        //            sb.Append("<FeatureGroupLang>");
        //            sb.Append("<Language>" + lang.Language + "</Language>");
        //            sb.Append("<FeatureGroupName>" + XmlUtils.XMLEncode(lang.FeatureGroup) + "</FeatureGroupName>");
        //            sb.Append("</FeatureGroupLang>");
        //        }
        //        sb.Append("</FeatureGroupLangs>");
        //        sb.Append("</FeatureGroup>");
        //    }
        //    sb.Append("</FeatureGroups>");

        //    List<FeatureInfo> features = GetFeatures(mod.PortalID);
        //    sb.Append("<Features>");
        //    foreach (FeatureInfo feature in features)
        //    {
        //        sb.Append("<Feature>");
        //        sb.Append("<FeatureId>" + feature.FeatureId.ToString() + "</FeatureId>");
        //        sb.Append("<FeatureGroupId>" + feature.FeatureGroupId.ToString() + "</FeatureGroupId>");
        //        sb.Append("<FeatureListId>" + feature.FeatureListId.ToString() + "</FeatureListId>");
        //        sb.Append("<Datatype>" + feature.Datatype.Trim() + "</Datatype>");
        //        sb.Append("<Multiselect>" + feature.Multiselect.ToString() + "</Multiselect>");
        //        sb.Append("<Control>" + feature.Control.Trim() + "</Control>");
        //        sb.Append("<Dimension>" + feature.Dimension.ToString() + "</Dimension>");
        //        sb.Append("<Required>" + feature.Required.ToString() + "</Required>");
        //        sb.Append("<MinValue>" + feature.MinValue.ToString(CultureInfo.InvariantCulture) + "</MinValue>");
        //        sb.Append("<MaxValue>" + feature.MaxValue.ToString(CultureInfo.InvariantCulture) + "</MaxValue>");
        //        sb.Append("<RegEx>" + XmlUtils.XMLEncode(feature.RegEx) + "</RegEx>");
        //        sb.Append("<RoleID>" + feature.RoleID.ToString() + "</RoleID>");
        //        sb.Append("<ShowInSearch>" + feature.ShowInSearch.ToString() + "</ShowInSearch>");
        //        sb.Append("<SearchGroups>" + XmlUtils.XMLEncode(feature.SearchGroups) + "</SearchGroups>");
        //        sb.Append("<FeatureToken>" + XmlUtils.XMLEncode(feature.FeatureToken) + "</FeatureToken>");
        //        sb.Append("<ViewOrder>" + feature.ViewOrder.ToString() + "</ViewOrder>");
        //        List<FeatureLangInfo> langs = GetFeatureLangs(feature.FeatureId);
        //        sb.Append("<FeatureLangs>");
        //        foreach (FeatureLangInfo lang in langs)
        //        {
        //            sb.Append("<FeatureLang>");
        //            sb.Append("<Language>" + lang.Language + "</Language>");
        //            sb.Append("<FeatureName>" + XmlUtils.XMLEncode(lang.Feature) + "</FeatureName>");
        //            sb.Append("<Unit>" + XmlUtils.XMLEncode(lang.Unit) + "</Unit>");
        //            sb.Append("</FeatureLang>");
        //        }
        //        sb.Append("</FeatureLangs>");
        //        sb.Append("</Feature>");
        //    }
        //    sb.Append("</Features>");

        //    List<FeatureListInfo> featureLists = GetFeatureLists(mod.PortalID);
        //    sb.Append("<FeatureLists>");
        //    foreach (FeatureListInfo featureList in featureLists)
        //    {
        //        sb.Append("<FeatureList>");
        //        sb.Append("<FeatureListId>" + featureList.FeatureListId.ToString() + "</FeatureListId>");
        //        sb.Append("<FeatureListLangs>");
        //        List<FeatureListLangInfo> langs = GetFeatureListLangs(featureList.FeatureListId);
        //        foreach (FeatureListLangInfo lang in langs)
        //        {
        //            sb.Append("<FeatureListLang>");
        //            sb.Append("<Language>" + lang.Language + "</Language>");
        //            sb.Append("<FeatureListName>" + XmlUtils.XMLEncode(lang.FeatureList) + "</FeatureListName>");
        //            sb.Append("</FeatureListLang>");
        //        }
        //        sb.Append("</FeatureListLangs>");
        //        sb.Append("<FeatureListItems>");
        //        List<FeatureListItemInfo> featureListItems = GetFeatureListItemsByListId(featureList.FeatureListId);
        //        foreach (FeatureListItemInfo featureListItem in featureListItems)
        //        {
        //            sb.Append("<FeatureListItem>");
        //            sb.Append("<FeatureListItemId>" + featureListItem.FeatureListItemId.ToString() + "</FeatureListItemId>");
        //            sb.Append("<Image>" + XmlUtils.XMLEncode(featureListItem.Image) + "</Image>");
        //            sb.Append("<ViewOrder>" + featureListItem.ViewOrder.ToString() + "</ViewOrder>");
        //            sb.Append("<FeatureListLangs>");
        //            List<FeatureListItemLangInfo> ilangs = GetFeatureListItemLangs(featureListItem.FeatureListItemId);
        //            foreach (FeatureListItemLangInfo lang in ilangs)
        //            {
        //                sb.Append("<FeatureListItemLang>");
        //                sb.Append("<Language>" + lang.Language + "</Language>");
        //                sb.Append("<FeatureListItemName>" + XmlUtils.XMLEncode(lang.FeatureListItem) + "</FeatureListItemName>");
        //                sb.Append("</FeatureListItemLang>");
        //            }
        //            sb.Append("</FeatureListLangs>");
        //            sb.Append("</FeatureListItem>");
        //        }
        //        sb.Append("</FeatureListItems>");
        //        sb.Append("</FeatureList>");
        //    }
        //    sb.Append("</FeatureLists>");
            
        //    sb.Append("</BBStore>");
        //    return sb.ToString();
        //}

        ///// ----------------------------------------------------------------------------- 
        ///// <summary> 
        ///// ImportModule implements the IPortable ImportModule Interface 
        ///// </summary> 
        ///// <remarks> 
        ///// </remarks> 
        ///// <param name="ModuleID">The Id of the module to be imported</param> 
        ///// <param name="Content">The content to be imported</param> 
        ///// <param name="Version">The version of the module to be imported</param> 
        ///// <param name="UserId">The Id of the user performing the import</param> 
        ///// <history> 
        ///// </history> 
        ///// ----------------------------------------------------------------------------- 
        //public void ImportModule(int ModuleID, string Content, string Version, int UserId)
        //{

        //    //XmlNode xmlBBStores = Globals.GetContent(Content, "BBStores");
        //    //foreach (XmlNode xmlBBStore in xmlBBStores.SelectNodes("BBStore"))
        //    //{
        //    //    BBStoreInfo objBBStore = new BBStoreInfo();
        //    //    objBBStore.ModuleId = ModuleID;
        //    //    objBBStore.Content = xmlBBStore.SelectSingleNode("content").InnerText;
        //    //    objBBStore.CreatedByUser = UserId;
        //    //    AddBBStore(objBBStore);
        //    //}

        //}

        #endregion

    }
}