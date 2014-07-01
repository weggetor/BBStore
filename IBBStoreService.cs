using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Bitboxx.DNNModules.BBStore
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IBBStoreService" in both code and config file together.
	[ServiceContract]
	public interface IBBStoreService
	{
		[OperationContract]
		[WebGet(ResponseFormat = WebMessageFormat.Xml, UriTemplate = "login/{portalId}/{userId}/{password}")]
		string Login(string portalId, string userId, string password);
		
		[SecurityTokenValidator("Administrators")]
		[OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Xml)]
        List<ProductGroupInfo> GetProductGroups(string portalId, string strStoreGuid, string token);

		[SecurityTokenValidator("Administrators")]
		[OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Xml)]
        List<SimpleProductInfo> GetSimpleProducts(string portalId, string strStoreGuid, string token);

		[SecurityTokenValidator("Administrators")]
		[OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Xml)]
        void DeleteProducts(string portalId, string strStoreGuid, string token);

		[SecurityTokenValidator("Administrators")]
		[OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Xml)]
        void DeleteProduct(string portalId, string strStoreGuid, string simpleProductId, string token);

		[SecurityTokenValidator("Administrators")]
		[OperationContract]
		[WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Xml)]
        int SaveProduct(string PortalId, string strStoreGuid, SimpleProductInfo Product, string Token);

		[SecurityTokenValidator("Administrators")]
		[OperationContract]
		[WebInvoke(BodyStyle=WebMessageBodyStyle.WrappedRequest, ResponseFormat= WebMessageFormat.Xml)]
        void SaveProductLang(string PortalId, string strStoreGuid, SimpleProductLangInfo ProductLang, string Token);

		[SecurityTokenValidator("Administrators")]
		[OperationContract]
		[WebInvoke(BodyStyle=WebMessageBodyStyle.WrappedRequest, ResponseFormat= WebMessageFormat.Xml)]
        void SaveProductInGroup(string PortalId, string strStoreGuid, string SimpleProductId, string ProductGroupId, string Token);

		[SecurityTokenValidator("Administrators")]
		[OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Xml)]
        ProductGroupInfo GetProductGroupByName(string PortalId, string strStoreGuid, string Language, string ProductGroupName, string Token);

		[SecurityTokenValidator("Administrators")]
		[OperationContract]
		[WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Xml)]
        void DeleteProductGroups(string PortalId, string strStoreGuid, string Token);

		[SecurityTokenValidator("Administrators")]
		[OperationContract]
		[WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Xml)]
        void SaveProductGroup(string PortalId, string strStoreGuid, ProductGroupInfo ProductGroup, string Token);

		[SecurityTokenValidator("Administrators")]
		[OperationContract]
		[WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Xml)]
        void SaveProductGroupLang(string PortalId, string strStoreGuid, ProductGroupLangInfo ProductGroupLang, string Token);

		[SecurityTokenValidator("Administrators")]
		[OperationContract]
		[WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Xml)]
        void DeleteFeatureGroups(string PortalId, string strStoreGuid, string Token);

		[SecurityTokenValidator("Administrators")]
		[OperationContract]
		[WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Xml)]
        void SaveFeatureGroup(string PortalId, string strStoreGuid, FeatureGroupInfo FeatureGroup, string Token);

		[SecurityTokenValidator("Administrators")]
		[OperationContract]
		[WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Xml)]
        void SaveFeatureGroupLang(string PortalId, string strStoreGuid, FeatureGroupLangInfo FeatureGroupLang, string Token);

		[SecurityTokenValidator("Administrators")]
		[OperationContract]
		[WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Xml)]
        void DeleteFeatureLists(string PortalId, string strStoreGuid, string Token);
		
		[SecurityTokenValidator("Administrators")]
		[OperationContract]
		[WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Xml)]
        void SaveFeatureList(string PortalId, string strStoreGuid, FeatureListInfo FeatureList, string Token);

		[SecurityTokenValidator("Administrators")]
		[OperationContract]
		[WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Xml)]
        void SaveFeatureListLang(string PortalId, string strStoreGuid, FeatureListLangInfo FeatureListLang, string Token);

		[SecurityTokenValidator("Administrators")]
		[OperationContract]
		[WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Xml)]
        void SaveFeatureListItem(string PortalId, string strStoreGuid, FeatureListItemInfo FeatureListItem, string Token);

		[SecurityTokenValidator("Administrators")]
		[OperationContract]
		[WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Xml)]
        void SaveFeatureListItemLang(string PortalId, string strStoreGuid, FeatureListItemLangInfo FeatureListItemLang, string Token);

		[SecurityTokenValidator("Administrators")]
		[OperationContract]
		[WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Xml)]
        void SaveFeature(string PortalId, string strStoreGuid, FeatureInfo Feature, string Token);

		[SecurityTokenValidator("Administrators")]
		[OperationContract]
		[WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Xml)]
        void DeleteFeatures(string PortalId, string strStoreGuid, string Token);
		
		[SecurityTokenValidator("Administrators")]
		[OperationContract]
		[WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Xml)]
        void SaveFeatureLang(string PortalId, string strStoreGuid, FeatureLangInfo FeatureLang, string Token);

		[SecurityTokenValidator("Administrators")]
		[OperationContract]
		[WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Xml)]
        void SaveProductGroupFeature(string PortalId, string strStoreGuid, string FeatureId, string ProductGroupId, string Token);

		[SecurityTokenValidator("Administrators")]
		[OperationContract]
		[WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Xml)]
        void SaveFeatureValue(string PortalId, string strStoreGuid, FeatureValueInfo FeatureValue, string Token);

		[SecurityTokenValidator("Administrators")]
		[OperationContract]
		[WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Xml)]
        void SaveProductGroupListItem(string PortalId, string strStoreGuid, string ProductGroupId, string FeatureListItemId, string Token);

		[SecurityTokenValidator("Administrators")]
		[OperationContract]
		[WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Xml)]
        void DeleteProductGroupListItems(string PortalId, string strStoreGuid, string Token);

		[SecurityTokenValidator("Administrators")]
		[OperationContract]
		[WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Xml)]
        void SaveImage(string PortalId, string strStoreGuid, int SimpleProductId, byte[] pictureData, string fileName, string Token);

		[SecurityTokenValidator("Administrators")]
		[OperationContract]
		[WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Xml)]
        void DeleteProductImages(string PortalId, string strStoreGuid, int SimpleProductId, string Token);

		[SecurityTokenValidator("Administrators")]
		[OperationContract]
		[WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Xml)]
        void DeleteProductImagesAll(string PortalId, string strStoreGuid, string Token);

		[SecurityTokenValidator("Administrators")]
		[OperationContract]
		[WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Xml)]
        void DeleteEmptyImageDirectories(string PortalId, string Token);

        [SecurityTokenValidator("Administrators")]
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Xml)]
        void ImportStore(string PortalId, BBStoreInfo bbstore, string Token);

        [SecurityTokenValidator("Administrators")]
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Xml)]
        BBStoreInfo ExportStore(string PortalId, string strStoreGuid, string Token);

        [SecurityTokenValidator("Administrators")]
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Xml)]
        void ResetStore(string PortalId, string strStoreGuid, bool skipImages, string Token);
	}
}
