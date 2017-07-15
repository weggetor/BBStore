using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    [Serializable]
    [DataContract()]
    public class BBStoreInfo
    {
        public BBStoreInfo()
        {
            StoreGuid = BBStoreController.StoreGuid;
            StoreName = "";
            ProductGroup = new List<ProductGroupInfo>();
            ProductGroupLang = new List<ProductGroupLangInfo>();
            Product = new List<SimpleProductInfo>();
            ProductLang = new List<SimpleProductLangInfo>();
            ProductInGroup = new List<ProductInGroupInfo>();
            FeatureGroup = new List<FeatureGroupInfo>();
            FeatureGroupLang = new List<FeatureGroupLangInfo>();
            FeatureList = new List<FeatureListInfo>();
            FeatureListLang = new List<FeatureListLangInfo>();
            FeatureListItem = new List<FeatureListItemInfo>();
            FeatureListItemLang = new List<FeatureListItemLangInfo>();
            Feature = new List<FeatureInfo>();
            FeatureLang = new List<FeatureLangInfo>();
            ProductGroupFeature = new List<ProductGroupFeatureInfo>();
            FeatureValue = new List<FeatureValueInfo>();
            ProductGroupListItem = new List<ProductGroupListItemInfo>();
            Unit= new List<UnitInfo>();
            UnitLang = new List<UnitLangInfo>();
            Order = new List<OrderInfo>();
            OrderProduct = new List<OrderProductInfo>();
            OrderProductOption = new List<OrderProductOptionInfo>();
            OrderAdditionalCost = new List<OrderAdditionalCostInfo>();
            OrderAddress = new List<OrderAddressInfo>();
            Customer = new List<CustomerInfo>();
            SubscriberAddressType = new List<SubscriberAddressTypeInfo>();
            SubscriberAddressTypeLang = new List<SubscriberAddressTypeLangInfo>();
        }

    
        [DataMember()]
        public Guid StoreGuid { get; set; }
        [DataMember()]
        public string StoreName { get; set; }
        [DataMember()]
        public List<ProductGroupInfo> ProductGroup { get; set; }
        [DataMember()]
        public List<ProductGroupLangInfo> ProductGroupLang { get; set; }
        [DataMember()]
        public List<SimpleProductInfo> Product { get; set; }
        [DataMember()]
        public List<SimpleProductLangInfo> ProductLang { get; set; }
        [DataMember()]
        public List<ProductInGroupInfo> ProductInGroup { get; set; }
        [DataMember()]
        public List<FeatureGroupInfo> FeatureGroup { get; set; }
        [DataMember()]
        public List<FeatureGroupLangInfo> FeatureGroupLang { get; set; }
        [DataMember()]
        public List<FeatureListInfo> FeatureList { get; set; }
        [DataMember()]
        public List<FeatureListLangInfo> FeatureListLang { get; set; }
        [DataMember()]
        public List<FeatureListItemInfo> FeatureListItem { get; set; }
        [DataMember()]
        public List<FeatureListItemLangInfo> FeatureListItemLang { get; set; }
        [DataMember()]
        public List<FeatureInfo> Feature { get; set; }
        [DataMember()]
        public List<FeatureLangInfo> FeatureLang { get; set; }
        [DataMember()]
        public List<ProductGroupFeatureInfo> ProductGroupFeature { get; set; }
        [DataMember()]
        public List<FeatureValueInfo> FeatureValue { get; set; }
        [DataMember()]
        public List<ProductGroupListItemInfo> ProductGroupListItem { get; set; }
        [DataMember()]
        public List<UnitInfo> Unit { get; set; }
        [DataMember()]
        public List<UnitLangInfo> UnitLang { get; set; }
        [DataMember()]
        public List<OrderInfo> Order { get; set; }
        [DataMember()]
        public List<OrderProductInfo> OrderProduct { get; set; }
        [DataMember()]
        public List<OrderProductOptionInfo> OrderProductOption { get; set; }
        [DataMember()]
        public List<OrderAdditionalCostInfo> OrderAdditionalCost { get; set; }
        [DataMember()]
        public List<OrderAddressInfo> OrderAddress { get; set; }
        [DataMember()]
        public List<CustomerInfo> Customer { get; set; }
        [DataMember()]
        public List<SubscriberAddressTypeInfo> SubscriberAddressType { get; set; }
        [DataMember()]
        public List<SubscriberAddressTypeLangInfo> SubscriberAddressTypeLang { get; set; }


    }
}