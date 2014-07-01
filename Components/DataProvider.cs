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
using System.Collections.Generic;
using System.Data;
using DotNetNuke;

namespace Bitboxx.DNNModules.BBStore
{

    /// ----------------------------------------------------------------------------- 
    /// <summary> 
    /// An abstract class for the data access layer 
    /// </summary> 
    /// <remarks> 
    /// </remarks> 
    /// <history> 
    /// </history> 
    /// ----------------------------------------------------------------------------- 
    public abstract class DataProvider
    {

        #region "Shared/Static Methods"

        /// <summary>
        /// singleton reference to the instantiated object 
        /// </summary>
        private static DataProvider objProvider = null;

        /// <summary>
        /// constructor
        /// </summary>
        static DataProvider()
        {
            CreateProvider();
        }

        /// <summary>
        /// dynamically create provider 
        /// </summary>
        private static void CreateProvider()
        {
            objProvider = (DataProvider)DotNetNuke.Framework.Reflection.CreateObject("data", "Bitboxx.DNNModules.BBStore", "");
        }

        /// <summary>
        /// return the provider 
        /// </summary>
        /// <returns></returns>
        public static DataProvider Instance()
        {
            return objProvider;
        }

        #endregion

        #region Abstract methods

        // SimpleProduct methods
		public abstract IDataReader GetSimpleProducts(int PortalId);
		public abstract IDataReader GetSimpleProducts(int PortalId, string Language);
        public abstract IDataReader GetSimpleProducts(int PortalId, string Language, string Sort);
        public abstract IDataReader GetSimpleProducts(int PortalId, string Language, string Sort, string Where);
		public abstract IDataReader GetSimpleProducts(int PortalId, string Language, string Sort, string Where, int Top);
		public abstract IDataReader GetSimpleProductByProductId(int PortalId, int ProductId);
        public abstract IDataReader GetSimpleProductByProductId(int PortalId, int ProductId, string Language);
        public abstract IDataReader GetSimpleProductByModuleId(int PortalId, int ModuleId);
        public abstract IDataReader GetSimpleProductByModuleId(int PortalId, int ModuleId, string Language);
        public abstract int NewSimpleProduct(SimpleProductInfo SimpleProduct);
        public abstract void UpdateSimpleProduct(SimpleProductInfo SimpleProduct);
        public abstract void DeleteSimpleProduct(int SimpleProductId);
		public abstract void DeleteSimpleProducts(int PortalId);

        // ModuleProduct methods
        [Obsolete]
        public abstract IDataReader GetModuleProduct(int PortalId, int ModuleId);
        [Obsolete]
        public abstract int GetModuleProductId(int PortalId, int ModuleId);
        [Obsolete]
        public abstract int GetModuleProductModuleId(int PortalId, int ProductId);
        [Obsolete]
        public abstract void SetModuleProductId(int PortalId, int ModuleId, int ProductId);
        [Obsolete]
        public abstract string GetModuleProductTemplate(int PortalId, int ModuleId);
        [Obsolete]
        public abstract void SetModuleProductTemplate(int PortalId, int ModuleId, int ProductTemplateId, string Template);
        [Obsolete]
        public abstract void NewModuleProduct(ModuleProductInfo ModuleProduct);
        [Obsolete]
        public abstract void UpdateModuleProduct(ModuleProductInfo ModuleProduct);
        public abstract IDataReader GetModuleProducts(int portalId);
        public abstract void DeleteModuleProduct(int portalId, int moduleid);

        // SimpleProductLang methods
        public abstract IDataReader GetSimpleProductLang(int SimpleProductId, string Language);
        public abstract IDataReader GetSimpleProductLangs(int SimpleProductId);
        public abstract IDataReader GetSimpleProductLangsByPortal(int portalId);
        public abstract void NewSimpleProductLang(SimpleProductLangInfo SimpleProductLang);
        public abstract void UpdateSimpleProductLang(SimpleProductLangInfo SimpleProductLang);
        public abstract void DeleteSimpleProductLangs(int SimpleProductId);
        public abstract void DeleteSimpleProductLang(int SimpleProductId, string Language);

        // Customer methods
		public abstract IDataReader GetCustomersByUserId(int PortalId, int UserId);
		public abstract IDataReader GetCustomerById(int CustomerId);
        public abstract int NewCustomer(CustomerInfo Customer);
		public abstract void UpdateCustomer(CustomerInfo Customer);
	    public abstract int SaveCustomer(CustomerInfo Customer);
        public abstract void DeleteCustomer(int CustomerId);

        // CustomerAddress methods
        public abstract IDataReader GetCustomerAddress(int CustomerAddressId);
        public abstract IDataReader GetCustomerAddresses(int CustomerId);
		public abstract IDataReader GetCustomerAddressesByCart(Guid cartid, string language);
	    public abstract IDataReader GetCustomerMainAddress(int CustomerId);
		public abstract IDataReader GetCustomerAdditionalAddresses(int CustomerId);
        public abstract int NewCustomerAddress(CustomerAddressInfo CustomerAddress);
        public abstract void UpdateCustomerAddress(CustomerAddressInfo CustomerAddress);
        public abstract void DeleteCustomerAddress(int CustomerAdressId);

        // Cart methods
        public abstract IDataReader GetCart(Guid CartId);
        public abstract void NewCart(int PortalId, CartInfo Cart);
        public abstract void UpdateCart(int PortalId, CartInfo Cart);
        public abstract void DeleteCart(Guid CartId);
        public abstract IDataReader GetCartTax(int PortalId, Guid CartId);
        public abstract void UpdateCartCustomerId(Guid CartId, int CustomerId);
	    public abstract void UpdateCartCustomerPaymentProviderId(Guid CartId, int CustomerPaymentProviderId);
    	public abstract string SerializeCart(Guid cartId);
		public abstract CartInfo DeserializeCart(int portalId, int userId, Guid cartId, string cartXml);

        // CartAddress methods
		public abstract int GetCartAddressId(Guid cartid, string kzAddressType);
		public abstract IDataReader GetCartAddressByTypeId(Guid cartid, int subscriberAddressTypeId);
	    public abstract IDataReader GetCartAddressesByAddressId(Guid cartid, int customerAddressId);
	    public abstract void UpdateCartAddressType(Guid cartId, int portalId, int subscriberId, int customerAddressId, string kzAdressType, bool set);
		public abstract void UpdateCartAddressType(Guid cartId, int portalId, int subscriberId, int customerAddressId, int subscriberAddressId, bool set);
	    public abstract void NewCartAddress(CartAddressInfo cartAddress);
	    public abstract bool CheckCartAddresses(Guid CartId, int portalId, int subscriberId);
        
		// CartAdditionalCost methods
        public abstract IDataReader GetCartAdditionalCost(int CartAdditionalCostId);
        public abstract IDataReader GetCartAdditionalCosts(Guid CartId);
        public abstract int NewCartAdditionalCost(CartAdditionalCostInfo CartAdditionalCost);
        public abstract void UpdateCartAdditionalCost(CartAdditionalCostInfo CartAdditionalCost);
        public abstract void DeleteCartAdditionalCost(int CartAdditionalCostId);
        public abstract void DeleteCartAdditionalCost(Guid CartId, string Name);

        // CartProduct methods
        public abstract IDataReader GetCartProduct(int CartProductId);
        public abstract IDataReader GetCartProductByProductId(Guid CartId, int ProductId);
        public abstract IDataReader GetCartProductByProductIdAndSelectedOptions(Guid CartId, int ProductId, System.Collections.Generic.List<OptionListInfo> SelectedOptions);
        public abstract IDataReader GetCartProducts(Guid CartId);
        public abstract int NewCartProduct(Guid CartId, CartProductInfo CartProduct);
        public abstract void UpdateCartProduct(CartProductInfo CartProduct);
        public abstract void UpdateCartProductQuantity(int cartProductId,decimal quantity);
        public abstract void DeleteCartProduct(int CartProductId);

        // CartProductOption methods
        public abstract IDataReader GetCartProductOption(int CartProductOptionId);
        public abstract IDataReader GetCartProductOptions(int CartProductId);
        public abstract int NewCartProductOption(int CartProductId, CartProductOptionInfo CartProductOption);
        public abstract void UpdateCartProductOption(CartProductOptionInfo cartProductOption);
        public abstract void DeleteCartProductOption(int CartProductOptionId);
        public abstract void DeleteCartProductOptions(int CartProductId);

        // ProductTemplate methods
        public abstract IDataReader GetProductTemplate(int ProductTemplateId);
        public abstract IDataReader GetProductTemplate(int PortalId, int SubscriberId, string TemplateName, string TemplateSource);
        public abstract IDataReader GetProductTemplates(int PortalId, int SubscriberId, string TemplateSource);
        public abstract IDataReader GetProductTemplates(int PortalId);
        public abstract int NewProductTemplate(ProductTemplateInfo ProductTemplate);
        public abstract void UpdateProductTemplate(ProductTemplateInfo ProductTemplate);
        public abstract void DeleteProductTemplate(int ProductTemplateId);

        // PaymentProvider methods
        public abstract IDataReader GetPaymentProvider(int PaymentProviderId,string Language);
        public abstract IDataReader GetPaymentProviders(string Language);

        // PaymentProviderLang methods
        public abstract IDataReader GetPaymentProviderLangs(int paymentProviderId);
        public abstract void NewPaymentProviderLang(PaymentProviderLangInfo paymentProviderLang);

        // SubscriberPaymentProvider methods
        public abstract IDataReader GetSubscriberPaymentProviders(int PortalId, int SubscriberId);
		public abstract IDataReader GetSubscriberPaymentProvider(int PortalId, int SubscriberId, int PaymentProviderId);
        public abstract IDataReader GetSubscriberPaymentProviderByCPP(int customerPaymentProviderId);
        public abstract int NewSubscriberPaymentProvider(SubscriberPaymentProviderInfo SubscriberPaymentProvider);
        public abstract void UpdateSubscriberPaymentProvider(SubscriberPaymentProviderInfo SubscriberPaymentProvider);
        public abstract void DeleteSubscriberPaymentProvider(int SubscriberPaymentProviderId);

        // CustomerPaymentProvider methods
        public abstract IDataReader GetCustomerPaymentProviders(int CustomerId);
        public abstract IDataReader GetCustomerPaymentProvider(int CustomerId, int PaymentProviderId);
        public abstract IDataReader GetCustomerPaymentProvider(int CustomerPaymentProviderId);
        public abstract int NewCustomerPaymentProvider(CustomerPaymentProviderInfo CustomerPaymentProvider);
        public abstract void UpdateCustomerPaymentProvider(CustomerPaymentProviderInfo CustomerPaymentProvider);
        public abstract void DeleteCustomerPaymentProvider(int CustomerPaymentProviderId);

		// SubscriberAddressType methods
        public abstract IDataReader GetSubscriberAddressTypes(int portalId);
		public abstract IDataReader GetSubscriberAddressTypes(int portalId, int subscriberId, string language);
		public abstract IDataReader GetSubscriberAddressType(int portalId, int subscriberId, string kzAddressType, string language);
		public abstract IDataReader GetSubscriberAddressType(int subscriberAddressTypeId, string language);
        public abstract int NewSubscriberAddressType(SubscriberAddressTypeInfo subscriberAddressType);
        public abstract void UpdateSubscriberAddressType(SubscriberAddressTypeInfo subscriberAddressType);

        // SubscriberAddressTypeLangs methods
        public abstract IDataReader GetSubscriberAddressTypeLangs(int subscriberAddressTypeId);
        public abstract void NewSubscriberAddressTypeLang(SubscriberAddressTypeLangInfo subscriberAddressTypelang);
		
		// Order methods
        public abstract int SaveOrder(Guid CartId, int PortalId, string numberMask);
        public abstract IDataReader GetOrder(int OrderId);
    	public abstract IDataReader GetOrders(int PortalId, string Language, string Sort, string Filter);
        public abstract IDataReader GetOrderProducts(int OrderId);
        public abstract IDataReader GetOrderProductOptions(int OrderProductId);
        public abstract IDataReader GetOrderAdditionalCosts(int OrderId);
        public abstract IDataReader GetOrderAddresses(int orderId, string language);
        public abstract bool HasOrderAddress(int customerAddressId);

        // OrderState methods
        public abstract IDataReader GetOrderStates(int portalId);
        public abstract IDataReader GetOrderStates(int portalId,string Language);
    	public abstract void SetOrderState(int orderId, int orderStateId);
        public abstract int NewOrderState(OrderStateInfo orderState);

        // OrderStateLangs methods
        public abstract IDataReader GetOrderStateLang(int orderStateId, string language);
        public abstract IDataReader GetOrderStateLangs(int orderStateId);
        public abstract void NewOrderStateLang(OrderStateLangInfo orderStateLang);
        public abstract void UpdateOrderStateLang(OrderStateLangInfo orderStateLang);
        public abstract void DeleteOrderStateLangs(int orderStateId);
        public abstract void DeleteOrderStateLang(int orderStateId, string language);

        // ProductGroup methods
        public abstract IDataReader GetProductGroups(int portalId);
		public abstract IDataReader GetProductGroups(int portalId, string language, bool includeDisabled);
		public abstract IDataReader GetProductSubGroupsByNode(int PortalId, string Language, int NodeId, bool IncludeCount, bool IncludeSubDirsInCount, bool IncludeDisabled);
		public abstract IDataReader GetProductGroupByName(int PortalId, string Language, string ProductGroupName);
        public abstract IDataReader GetProductGroup(int PortalId, string Language, int ProductGroupId);
		public abstract IDataReader GetProductGroup(int PortalId, int ProductGroupId);
        public abstract string GetProductGroupPath(int PortalId, int ProductGroupId);
        public abstract string GetProductGroupPath(int portalId, int productGroupId, string language, bool returnId, string delimiter, string linkTemplate, string rootText);
		public abstract int NewProductGroup(ProductGroupInfo ProductGroup);
		public abstract void UpdateProductGroup(ProductGroupInfo ProductGroup);
		public abstract void DeleteProductGroup(int ProductGroupId);
		public abstract void DeleteProductGroups(int PortalId);


		// ProductGroupLang methods
		public abstract IDataReader GetProductGroupLang(int ProductGroupId, string Language);
		public abstract IDataReader GetProductGroupLangs(int ProductGroupId);
        public abstract IDataReader GetProductGroupLangsByPortal(int portalId);
		public abstract void NewProductGroupLang(ProductGroupLangInfo ProductGroupLang);
		public abstract void UpdateProductGroupLang(ProductGroupLangInfo ProductGroupLang);
		public abstract void DeleteProductGroupLangs(int ProductGroupId);
		public abstract void DeleteProductGroupLang(int ProductGroupId, string Language);
		
		// ProductInGroup methods
        public abstract DataTable GetProductsInGroupByProduct(int SimpleProductId);
        public abstract IDataReader GetProductsInGroupByPortal(int portalId);
        public abstract void NewProductInGroup(int SimpleProductId, int ProductGroupId);
        public abstract void DeleteProductInGroups(int SimpleProductId);
		public abstract void DeleteProductInGroup(int SimpleProductId, int ProductGroupId);

        //ProductFilter methods
        public abstract IDataReader GetProductFilters(int PortalId,Guid FilterSessionId);
        public abstract IDataReader GetProductFilter(int PortalId,Guid FilterSessionId, string FilterSource);
        public abstract void NewProductFilter(ProductFilterInfo ProductFilter);
        public abstract void UpdateProductFilter(ProductFilterInfo ProductFilter);
        public abstract void DeleteProductFilters(int PortalId,Guid FilterSessionId);
        public abstract void DeleteProductFilter(int PortalId,Guid FilterSessionId, string FilterSource);
		public abstract void DeleteProductFilter(int PortalId, Guid FilterSessionId, string FilterSource, string FirstFilterValue);

		//FeatureGrid methods
		public abstract IDataReader GetFeatureGridValues(int PortalId, int ProductId, string Language, int RoleId, int FeatureGroupId);
		public abstract IDataReader GetFeatureGridValueByProductAndToken(int PortalId, int ProductId, string Language, string FeatureToken);
		public abstract IDataReader GetFeatureGridFeaturesByProduct(int PortalId, int ProductId, string Language, int RoleId, int FeatureGroupId );
		public abstract IDataReader GetFeatureGridFeaturesByProductGroup(int PortalId, int ProductGroupId, string Language, int RoleId, int FeatureGroupId,bool OnlyShowInSearch);

		// FeatureValues methods
    	public abstract int GetFeatureValueId(int productId, int featureId);
        public abstract IDataReader GetFeatureValuesByPortal(int portalId);
		public abstract void DeleteFeatureValuesByProductId(int ProductId, int FeatureGroupId);
		public abstract int NewFeatureValue(FeatureValueInfo FeatureValue);
		public abstract void UpdateFeatureValue(FeatureValueInfo FeatureValue);
		public abstract void DeleteFeatureValue(int FeatureValueId);

		// FeatureGroup methods
		public abstract IDataReader GetFeatureGroupById(int FeatureGroupId);
		public abstract IDataReader GetFeatureGroups(int PortalId);
		public abstract IDataReader GetFeatureGroupById(int FeatureGroupId, string Language);
		public abstract IDataReader GetFeatureGroups(int PortalId, string Language);
		public abstract int NewFeatureGroup(FeatureGroupInfo FeatureGroup);
		public abstract void UpdateFeatureGroup(FeatureGroupInfo FeatureGroup);
		public abstract void DeleteFeatureGroup(int FeatureGroupId);
		public abstract void DeleteFeatureGroups(int PortalId);

		// FeatureGroupLang methods
		public abstract IDataReader GetFeatureGroupLang(int FeatureGroupId, string Language);
		public abstract IDataReader GetFeatureGroupLangs(int FeatureGroupId);
        public abstract IDataReader GetFeatureGroupLangsByPortal(int portalId);
		public abstract void NewFeatureGroupLang(FeatureGroupLangInfo FeatureGroupLang);
		public abstract void UpdateFeatureGroupLang(FeatureGroupLangInfo FeatureGroupLang);
		public abstract void DeleteFeatureGroupLangs(int FeatureGroupLangId);
		public abstract void DeleteFeatureGroupLang(int FeatureGroupLangId, string Language);

		// Feature methods
		public abstract IDataReader GetFeatures(int PortalId);
		public abstract IDataReader GetFeatures(int PortalId, string Language);
		public abstract IDataReader GetFeatureById(int FeatureId);
		public abstract IDataReader GetFeatureById(int FeatureId, string Language);
		public abstract int NewFeature(FeatureInfo Feature);
		public abstract void UpdateFeature(FeatureInfo Feature);
		public abstract void DeleteFeature(int FeatureId);
		public abstract void DeleteFeatures(int PortalId);

		// FeatureLang methods
		public abstract IDataReader GetFeatureLang(int FeatureId, string Language);
		public abstract IDataReader GetFeatureLangs(int FeatureId);
        public abstract IDataReader GetFeatureLangsByPortal(int portalId);
		public abstract void NewFeatureLang(FeatureLangInfo FeatureLang);
		public abstract void UpdateFeatureLang(FeatureLangInfo FeatureLang);
		public abstract void DeleteFeatureLangs(int FeatureLangId);
		public abstract void DeleteFeatureLang(int FeatureLangId, string Language);


		// FeatureList methods
		public abstract IDataReader GetFeatureLists(int PortalId);
		public abstract IDataReader GetFeatureLists(int PortalId, string Language);
		public abstract IDataReader GetFeatureListById(int FeatureListId);
		public abstract IDataReader GetFeatureListById(int FeatureListId, string Language);
		public abstract int NewFeatureList(FeatureListInfo FeatureList);
		public abstract void UpdateFeatureList(FeatureListInfo FeatureList);
		public abstract void DeleteFeatureList(int FeatureListId);
		public abstract void DeleteFeatureLists(int PortalId);

		// FeatureListLang methods
		public abstract IDataReader GetFeatureListLang(int FeatureListId, string Language);
		public abstract IDataReader GetFeatureListLangs(int FeatureListId);
        public abstract IDataReader GetFeatureListLangsByPortal(int portalId);
		public abstract void NewFeatureListLang(FeatureListLangInfo FeatureListLang);
		public abstract void UpdateFeatureListLang(FeatureListLangInfo FeatureListLang);
		public abstract void DeleteFeatureListLangs(int FeatureListLangId);
		public abstract void DeleteFeatureListLang(int FeatureListLangId, string Language);

		// FeatureListItem methods
		public abstract IDataReader GetFeatureListItemById(int FeatureListItemId);
		public abstract IDataReader GetFeatureListItemById(int FeatureListItemId, string Language);
        public abstract IDataReader GetFeatureListItems(int portalId);
		public abstract IDataReader GetFeatureListItemsByListId(int FeatureListId);
		public abstract IDataReader GetFeatureListItemsByListId(int FeatureListId, string Language, bool onlyWithImage);
		public abstract IDataReader GetFeatureListItemsByListAndProduct(int FeatureListId, int ProductId, string Language);
		public abstract IDataReader GetFeatureListItemsByListAndProductGroup(int FeatureListId, int ProductGroupId, string Language);
		public abstract int NewFeatureListItem(FeatureListItemInfo FeatureListItem);
		public abstract void UpdateFeatureListItem(FeatureListItemInfo FeatureListItem);
		public abstract void DeleteFeatureListItem(int FeatureListItemId);
		public abstract void DeleteFeatureListItems(int FeatureListId);

		// FeatureListItemLang methods
		public abstract IDataReader GetFeatureListItemLang(int FeatureListItemId, string Language);
		public abstract IDataReader GetFeatureListItemLangs(int FeatureListItemId);
        public abstract IDataReader GetFeatureListItemLangsByPortal(int portalId);
		public abstract void NewFeatureListItemLang(FeatureListItemLangInfo FeatureListItemLang);
		public abstract void UpdateFeatureListItemLang(FeatureListItemLangInfo FeatureListItemLang);
		public abstract void DeleteFeatureListItemLangs(int FeatureListItemId);
		public abstract void DeleteFeatureListItemLang(int FeatureListItemId, string Language);

		// ProductGroupFeature methods
		public abstract DataTable GetProductGroupFeatures(int FeatureId);
        public abstract IDataReader GetProductGroupFeaturesByPortal(int portalid);
	    public abstract bool IsFeatureInProductGroup(int productGroupId, int featureId);
		public abstract void NewProductGroupFeature(int FeatureId, int ProductGroupId);
		public abstract void DeleteProductGroupFeatures(int FeatureId);
		public abstract void DeleteProductGroupFeature(int FeatureId, int ProductGroupId);

		// ProductGroupListItems methods
        public abstract IDataReader GetProductGroupListItemsByPortal(int portalId);
    	public abstract void NewProductGroupListItem(int productGroupId, int featureListItemid);
	    public abstract bool IsFeatureListItemInProductGroup(int productGroupId, int featureListItemid);
		public abstract void DeleteProductGroupListItem(int productGroupId, int featureListItemid);
    	public abstract void DeleteProductGroupListItemsByPortal(int portalId);
        public abstract void DeleteProductGroupListItemsByProductGroup(int productGroupId);
		public abstract void DeleteProductGroupListItemsByProductGroupAndFeatureList(int productgroupId, int featureListId);
		public abstract void AddProductGroupListItemsByProductGroupAndFeatureList(int productgroupId, int featureListId);
	    public abstract IDataReader GetSelectedFeatureListsByProductGroup(int productGroupId, string language);
		
		// StaticFilter methods
		public abstract IDataReader GetStaticFilters(int PortalId);
		public abstract IDataReader GetStaticFilter(int PortalId, string Token);
		public abstract IDataReader GetStaticFilterById(int StaticFilterId);
		public abstract int NewStaticFilter(StaticFilterInfo StaticFilter);
		public abstract void UpdateStaticFilter(StaticFilterInfo StaticFilter);
		public abstract void DeleteStaticFilter(int PortalId, string Token);
		public abstract void DeleteStaticFilterById(int StaticFilterId);

        // LocalResources methods
        public abstract IDataReader GetLocalResource(int portalId, string token);
        public abstract int NewLocalResource(LocalResourceInfo resourceInfo);
        public abstract void UpdateLocalResource(LocalResourceInfo resourceInfo);
        public abstract void DeleteLocalResource(int resourceId);

        //LocalResourceLang methods
        public abstract IDataReader GetLocalResourceLang(int resourceId, string language);
        public abstract IDataReader GetLocalResourceLang(int portalId, string token, string language);
        public abstract IDataReader GetLocalResourceLangs(int resourceId);
        public abstract IDataReader GetLocalResourceLangs(int portalId,string token);
        public abstract void NewLocalResourceLang(LocalResourceLangInfo resourceLangInfo);
        public abstract void UpdateLocalResourceLang(LocalResourceLangInfo resourceLangInfo);
        public abstract void DeleteLocalResourceLang(LocalResourceLangInfo resourceLangInfo);
        public abstract void DeleteLocalResourceLangs(int resourceId);
  
		// ContactAddress methods
		public abstract IDataReader GetContactAddresses(DateTime? StartDate);
		public abstract int NewContactAddress(ContactAddressInfo ContactAddress);
		public abstract void UpdateContactAddress(ContactAddressInfo ContactAddress);
		public abstract void DeleteContactAddress(int ContactAddressId);

		// ContactProduct methods
		public abstract IDataReader GetContactProductsByCartId(int PortalId, Guid CartId, string Language);
		public abstract IDataReader GetContactProductsByAddressId(int PortalId, int ContactAddressId, string Language);
		public abstract void NewContactProduct(Guid CartId, int ProductId, int ContactAddressId, string selectedAttributes);
		public abstract void UpdateContactProduct(Guid CartId, int ProductId, int ContactAddressId);
		public abstract void DeleteContactProduct(Guid CartId, int ProductId);
		public abstract void DeleteContactProduct(int ContactAddressId, int ProductId);

		// ContactReason methods
		public abstract IDataReader GetContactReasons(int ContactAddressId);
		public abstract IDataReader GetContactReasonByToken(int ContactAddressId, string Token);
		public abstract void NewContactReason(ContactReasonInfo ContactReason);
		public abstract void UpdateContactReason(ContactReasonInfo ContactReason);
		public abstract void DeleteContactReasons(int ContactAddressId);
		public abstract void DeleteContactReason(int ContactAddressId, string Token);

        // Unit methods
        public abstract IDataReader GetUnit(int unitId);
        public abstract IDataReader GetUnit(int unitId, string language);
        public abstract IDataReader GetUnits(int portalId);
        public abstract IDataReader GetUnits(int portalId, string language, string sortByField);
        public abstract int NewUnit(UnitInfo unit);
        public abstract void UpdateUnit(UnitInfo unit);
        public abstract void DeleteUnit(int unitId);

        // UnitLang methods
        public abstract IDataReader GetUnitLang(int unitId, string language);
        public abstract IDataReader GetUnitLangs(int unitId);
        public abstract IDataReader GetUnitLangsByPortal(int portalId);
        public abstract IDataReader GetPortalUnitLangs(int portalId);
        public abstract void NewUnitLang(UnitLangInfo unitLang);
        public abstract void DeleteUnitLang(int unitId, string language);
        public abstract void DeleteUnitLangs(int unitId);

        // ShippingModel
        public abstract IDataReader GetShippingModel(int shippingModelId);
        public abstract IDataReader GetShippingModels(int portalId);
        public abstract int NewShippingModel(ShippingModelInfo shippingModel);
        public abstract void UpdateShippingModel(ShippingModelInfo shippingModel);
        public abstract void DeleteShippingModel(int shippingModelId);

        // ProductShippingModel
        public abstract IDataReader GetProductShippingModelsByProduct(int productId);
        public abstract void DeleteProductShippingModelByProduct(int productId);
        public abstract void InsertProductShippingModel(ProductShippingModelInfo productShippingModel);

       // ShippingCost methods
        public abstract IDataReader GetShippingCosts(int PortalId);
        public abstract IDataReader GetShippingCostsByModelId(int shippingModelId);
        public abstract IDataReader GetShippingCostById(int ShippingCostId);
        public abstract int NewShippingCost(ShippingCostInfo ShippingCost);
        public abstract void UpdateShippingCost(ShippingCostInfo ShippingCost);
        public abstract void DeleteShippingCost(int ShippingCostId);
        #endregion

        // ShippingZone methods
        public abstract int GetShippingZoneIdByAddress(int modelId, string countryCodeISO2, int postalCode);
        public abstract IDataReader GetShippingZoneById(int shippingZoneId, string language);

        #region Helper methods

        // Helper methods
        public abstract void ReseedTables();

        #endregion

        #region SearchFilters
        public abstract string GetProductGroupFilter(int PortalId, int ProductGroupId, bool IncludeChilds);
		public abstract string GetSearchTextFilter(int PortalId, string SearchText, string Language);
		public abstract string GetSearchStaticFilter(int PortalId, string Token, string Language);
		public abstract string GetSearchStaticFilter(int StaticFilterId, string Language);
		public abstract string GetSearchPriceFilter(int PortalId, decimal StartPrice, decimal EndPrice,bool IncludeTax);
		public abstract string GetSearchFeatureFilter(string DataType, string Value);
		public abstract string GetSearchFeatureListFilter(int FeatureListId, int FeatureListItemId);
		#endregion

		#region ImportRelation

		public abstract int GetImportRelationOwnId(int PortalId, string Tablename, int ForeignId, Guid storeGuid);
        public abstract int GetImportRelationForeignId(int PortalId, string Tablename, int OwnId, Guid storeGuid);
        public abstract IDataReader GetImportRelationOwnIdsByTable(int portalId, string tableName, Guid storeGuid);
        public abstract IDataReader GetImportRelationForeignIdsByTable(int portalId, string tableName, Guid storeGuid);
        public abstract void NewImportRelation(int PortalId, string Tablename, int OwnId, int ForeignId, Guid storeGuid);
        public abstract void DeleteImportRelationByOwnId(int PortalId, string Tablename, int OwnId);
        public abstract void DeleteImportRelationByForeignId(int PortalId, string Tablename, int ForeignId, Guid storeGuid);
        public abstract void DeleteImportRelationByTable(int PortalId, string Tablename, Guid storeGuid);
        public abstract void DeleteImportRelationByPortal(int PortalId);
        public abstract void DeleteImportRelationByStore(int PortalId, Guid storeGuid);

        #endregion

        #region ImportStore

        public abstract void SaveImportStore(Guid storeGuid, string storeName);
        public abstract string GetImportStoreName(Guid storeGuid);
        public abstract IDataReader GetStoreGuids(int portalId);
        public abstract void DeleteImportStore(Guid storeGuid);

        #endregion

    }
}