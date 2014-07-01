using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using System;
using System.Collections.Generic;

namespace Bitboxx.DNNModules.BBStore
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "BBStoreService" in code, svc and config file together.
	public class BBStoreService : IBBStoreService
	{
        private BBStoreImportController _importController;
	    
        public BBStoreImportController ImportController 
        { 
            get
            {
                if  (_importController == null)
                    _importController = new BBStoreImportController();
                return _importController;
            } 
        }
        
        #region IBBStoreService Members
		
        public string Login(string portalId, string userId, string password)
		{
			UserLoginStatus loginStatus = new UserLoginStatus();
			UserInfo user = UserController.ValidateUser(Convert.ToInt32(portalId), userId, password,
				"","","0.0.0.0",ref loginStatus);
			if (user == null)
				throw new Exception("Access Request Denied. Invalid UserId and Password");
			Guid tokenId = Guid.NewGuid();
			BBStoreSecurityToken token = new BBStoreSecurityToken()
			{
				UserId = user.UserID,
				PortalId = Convert.ToInt32(portalId),
				UserName = user.Username
			};
			DataCache.SetCache("BBStoreSecurityToken_" + tokenId.ToString(), token, new TimeSpan(0, 5, 0));
			return tokenId.ToString();
		}

        public List<ProductGroupInfo> GetProductGroups(string PortalId, string strStoreGuid, string Token)
		{
			int portalId = -1;
		    Int32.TryParse(PortalId, out portalId);
		    if (portalId < 0)
		        throw new Exception("PortalId must be zero or greater");

            Guid storeGuid = new Guid(strStoreGuid);
            if (storeGuid == Guid.Empty)
                throw new Exception("StoreGuid must be valid!");

			return  ImportController.GetProductGroups(portalId, storeGuid);
		}

        public ProductGroupInfo GetProductGroupByName(string PortalId, string strStoreGuid, string Language, string ProductGroupName, string Token)
		{
			int portalId = -1;
		    Int32.TryParse(PortalId, out portalId);
		    if (portalId < 0)
		        throw new Exception("PortalId must be zero or greater");

            Guid storeGuid = new Guid(strStoreGuid);
            if (storeGuid == Guid.Empty)
                throw new Exception("StoreGuid must be valid!");

			return ImportController.GetProductGroupByName(portalId, Language, ProductGroupName, storeGuid);
		}
        
        public List<SimpleProductInfo> GetSimpleProducts(string PortalId, string strStoreGuid, string Token)
		{
			int portalId = -1;
		    Int32.TryParse(PortalId, out portalId);
		    if (portalId < 0)
		        throw new Exception("PortalId must be zero or greater");

            Guid storeGuid = new Guid(strStoreGuid);
            if (storeGuid == Guid.Empty)
                throw new Exception("StoreGuid must be valid!");

			return ImportController.GetSimpleProducts(portalId, storeGuid);
		}
		
        public void DeleteProducts(string PortalId, string strStoreGuid, string Token)
		{
			int portalId = -1;
		    Int32.TryParse(PortalId, out portalId);
		    if (portalId < 0)
		        throw new Exception("PortalId must be zero or greater");

            Guid storeGuid = new Guid(strStoreGuid);
            if (storeGuid == Guid.Empty)
                throw new Exception("StoreGuid must be valid!");

			ImportController.DeleteProducts(portalId, storeGuid);
		}

		public void DeleteProduct(string PortalId, string strStoreGuid, string simpleProductId, string Token)
		{
			int portalId = -1;
		    Int32.TryParse(PortalId, out portalId);
		    if (portalId < 0)
		        throw new Exception("PortalId must be zero or greater");
		    
            int productId = -1;
		    Int32.TryParse(simpleProductId, out productId);
		    if (productId < 0)
		        throw new Exception("ProductId must be zero or greater");

            Guid storeGuid = new Guid(strStoreGuid);
            if (storeGuid == Guid.Empty)
                throw new Exception("StoreGuid must be valid!");

            ImportController.DeleteProduct(portalId, productId, storeGuid);
        }

	    public int SaveProduct(string PortalId, string strStoreGuid, SimpleProductInfo product, string Token)
	    {
	        int portalId = -1;
	        Int32.TryParse(PortalId, out portalId);
	        if (portalId < 0)
	            throw new Exception("PortalId must be zero or greater");

            if (product == null)
                throw new Exception("Product must not be null");

            Guid storeGuid = new Guid(strStoreGuid);
            if (storeGuid == Guid.Empty)
                throw new Exception("StoreGuid must be valid!");

            return ImportController.SaveProduct(portalId, product, storeGuid);
	    }

	    public void SaveProductLang(string PortalId, string strStoreGuid, SimpleProductLangInfo productLang, string Token)
	    {
	        int portalId = -1;
	        Int32.TryParse(PortalId, out portalId);
	        if (portalId < 0)
	            throw new Exception("PortalId must be zero or greater");

            if (productLang == null)
                throw new Exception("ProductLang must not be null");

            Guid storeGuid = new Guid(strStoreGuid);
            if (storeGuid == Guid.Empty)
                throw new Exception("StoreGuid must be valid!");

            ImportController.SaveProductLang(portalId, productLang, storeGuid);
	    }

        public void SaveProductInGroup(string PortalId, string strStoreGuid, string ProductId, string ProductGroupId, string Token)
		{
	        int portalId = -1;
	        Int32.TryParse(PortalId, out portalId);
	        if (portalId < 0)
	            throw new Exception("PortalId must be zero or greater");

            int productId = -1;
	        Int32.TryParse(ProductId, out productId);
	        if (productId < 0)
	            throw new Exception("ProductId must be zero or greater");

            int productGroupId = -1;
            Int32.TryParse(ProductGroupId, out productGroupId);
            if (productId < 0)
                throw new Exception("ProductGroupId must be zero or greater");

            Guid storeGuid = new Guid(strStoreGuid);
            if (storeGuid == Guid.Empty)
                throw new Exception("StoreGuid must be valid!");

            ImportController.SaveProductInGroup(portalId, productId, productGroupId, storeGuid);
		}

	    public void DeleteProductGroups(string PortalId, string strStoreGuid, string Token)
	    {
	        int portalId = -1;
	        Int32.TryParse(PortalId, out portalId);
	        if (portalId < 0)
	            throw new Exception("PortalId must be zero or greater");

            Guid storeGuid = new Guid(strStoreGuid);
            if (storeGuid == Guid.Empty)
                throw new Exception("StoreGuid must be valid!");

	        ImportController.DeleteProductGroups(portalId, storeGuid);
	    }

	    public void SaveProductGroup(string PortalId, string strStoreGuid, ProductGroupInfo productGroup, string Token)
	    {
	        int portalId = -1;
	        Int32.TryParse(PortalId, out portalId);
	        if (portalId < 0)
	            throw new Exception("PortalId must be zero or greater");

            if (productGroup == null)
                throw new Exception("ProductGroup must not be null");

            Guid storeGuid = new Guid(strStoreGuid);
            if (storeGuid == Guid.Empty)
                throw new Exception("StoreGuid must be valid!");

            ImportController.SaveProductGroup(portalId, productGroup, storeGuid);
	    }

        public void SaveProductGroupLang(string PortalId, string strStoreGuid, ProductGroupLangInfo productGroupLang, string Token)
		{
            int portalId = -1;
            Int32.TryParse(PortalId, out portalId);
            if (portalId < 0)
                throw new Exception("PortalId must be zero or greater");

            if (productGroupLang == null)
                throw new Exception("ProductGroupLang must not be null");

            Guid storeGuid = new Guid(strStoreGuid);
            if (storeGuid == Guid.Empty)
                throw new Exception("StoreGuid must be valid!");

            ImportController.SaveProductGroupLang(portalId, productGroupLang, storeGuid);
		}

		public void DeleteFeatureGroups(string PortalId, string strStoreGuid, string Token)
		{
            int portalId = -1;
            Int32.TryParse(PortalId, out portalId);
            if (portalId < 0)
                throw new Exception("PortalId must be zero or greater");

            Guid storeGuid = new Guid(strStoreGuid);
            if (storeGuid == Guid.Empty)
                throw new Exception("StoreGuid must be valid!");

            ImportController.DeleteFeatureGroups(portalId,storeGuid);
        }

		public void SaveFeatureGroup(string PortalId, string strStoreGuid, FeatureGroupInfo featureGroup, string Token)
		{
            int portalId = -1;
            Int32.TryParse(PortalId, out portalId);
            if (portalId < 0)
                throw new Exception("PortalId must be zero or greater");

            if (featureGroup == null)
                throw new Exception("FeatureGroup must not be null");

            Guid storeGuid = new Guid(strStoreGuid);
            if (storeGuid == Guid.Empty)
                throw new Exception("StoreGuid must be valid!");

            ImportController.SaveFeatureGroup(portalId, featureGroup, storeGuid);
		}

        public void SaveFeatureGroupLang(string PortalId, string strStoreGuid, FeatureGroupLangInfo featureGroupLang, string Token)
		{
            int portalId = -1;
            Int32.TryParse(PortalId, out portalId);
            if (portalId < 0)
                throw new Exception("PortalId must be zero or greater");

            if (featureGroupLang == null)
                throw new Exception("FeatureGroupLang must not be null");

            Guid storeGuid = new Guid(strStoreGuid);
            if (storeGuid == Guid.Empty)
                throw new Exception("StoreGuid must be valid!");

            ImportController.SaveFeatureGroupLang(portalId, featureGroupLang, storeGuid);
        }

        public void DeleteFeatureLists(string PortalId, string strStoreGuid, string Token)
		{
            int portalId = -1;
            Int32.TryParse(PortalId, out portalId);
            if (portalId < 0)
                throw new Exception("PortalId must be zero or greater");

            Guid storeGuid = new Guid(strStoreGuid);
            if (storeGuid == Guid.Empty)
                throw new Exception("StoreGuid must be valid!");

            ImportController.DeleteFeatureLists(portalId, storeGuid);
        }

        public void SaveFeatureList(string PortalId, string strStoreGuid, FeatureListInfo featureList, string Token)
		{
            int portalId = -1;
            Int32.TryParse(PortalId, out portalId);
            if (portalId < 0)
                throw new Exception("PortalId must be zero or greater");

            if (featureList == null)
                throw new Exception("FeatureList must not be null");

            Guid storeGuid = new Guid(strStoreGuid);
            if (storeGuid == Guid.Empty)
                throw new Exception("StoreGuid must be valid!");

            ImportController.SaveFeatureList(portalId, featureList, storeGuid);
        }

        public void SaveFeatureListLang(string PortalId, string strStoreGuid, FeatureListLangInfo featureListLang, string Token)
		{
            int portalId = -1;
            Int32.TryParse(PortalId, out portalId);
            if (portalId < 0)
                throw new Exception("PortalId must be zero or greater");

            if (featureListLang == null)
                throw new Exception("FeatureListLang must not be null");

            Guid storeGuid = new Guid(strStoreGuid);
            if (storeGuid == Guid.Empty)
                throw new Exception("StoreGuid must be valid!");

            ImportController.SaveFeatureListLang(portalId, featureListLang, storeGuid);
        }

        public void SaveFeatureListItem(string PortalId, string strStoreGuid, FeatureListItemInfo featureListItem, string Token)
		{
            int portalId = -1;
            Int32.TryParse(PortalId, out portalId);
            if (portalId < 0)
                throw new Exception("PortalId must be zero or greater");

            if (featureListItem == null)
                throw new Exception("FeatureListItem must not be null");

            Guid storeGuid = new Guid(strStoreGuid);
            if (storeGuid == Guid.Empty)
                throw new Exception("StoreGuid must be valid!");

            ImportController.SaveFeatureListItem(portalId, featureListItem, storeGuid);
        }

        public void SaveFeatureListItemLang(string PortalId, string strStoreGuid, FeatureListItemLangInfo featureListItemLang, string Token)
		{
            int portalId = -1;
            Int32.TryParse(PortalId, out portalId);
            if (portalId < 0)
                throw new Exception("PortalId must be zero or greater");

            if (featureListItemLang == null)
                throw new Exception("FeatureListItemLang must not be null");

            Guid storeGuid = new Guid(strStoreGuid);
            if (storeGuid == Guid.Empty)
                throw new Exception("StoreGuid must be valid!");

            ImportController.SaveFeatureListItemLang(portalId, featureListItemLang, storeGuid);
        }

        public void SaveFeature(string PortalId, string strStoreGuid, FeatureInfo feature, string Token)
		{
            int portalId = -1;
            Int32.TryParse(PortalId, out portalId);
            if (portalId < 0)
                throw new Exception("PortalId must be zero or greater");

            if (feature == null)
                throw new Exception("Feature must not be null");

            Guid storeGuid = new Guid(strStoreGuid);
            if (storeGuid == Guid.Empty)
                throw new Exception("StoreGuid must be valid!");

            ImportController.SaveFeature(portalId, feature, storeGuid);
        }

        public void SaveFeatureLang(string PortalId, string strStoreGuid, FeatureLangInfo featureLang, string Token)
		{
            int portalId = -1;
            Int32.TryParse(PortalId, out portalId);
            if (portalId < 0)
                throw new Exception("PortalId must be zero or greater");

            if (featureLang == null)
                throw new Exception("FeatureLang must not be null");

            Guid storeGuid = new Guid(strStoreGuid);
            if (storeGuid == Guid.Empty)
                throw new Exception("StoreGuid must be valid!");

            ImportController.SaveFeatureLang(portalId, featureLang, storeGuid);
        }

        public void DeleteFeatures(string PortalId, string strStoreGuid, string Token)
        {
            int portalId = -1;
            Int32.TryParse(PortalId, out portalId);
            if (portalId < 0)
                throw new Exception("PortalId must be zero or greater");

            Guid storeGuid = new Guid(strStoreGuid);
            if (storeGuid == Guid.Empty)
                throw new Exception("StoreGuid must be valid!");

            ImportController.DeleteFeatures(portalId, storeGuid);
        }

        public void SaveProductGroupFeature(string PortalId, string strStoreGuid, string FeatureId, string ProductGroupId, string Token)
		{
            int portalId = -1;
            Int32.TryParse(PortalId, out portalId);
            if (portalId < 0)
                throw new Exception("PortalId must be zero or greater");

            int featureId = -1;
            Int32.TryParse(FeatureId, out featureId);
            if (featureId < 0)
                throw new Exception("FeatureId must be zero or greater");

            int productGroupId = -1;
            Int32.TryParse(ProductGroupId, out productGroupId);
            if (productGroupId < 0)
                throw new Exception("ProductGroupId must be zero or greater");

            Guid storeGuid = new Guid(strStoreGuid);
            if (storeGuid == Guid.Empty)
                throw new Exception("StoreGuid must be valid!");

            ImportController.SaveProductGroupFeature(portalId, featureId, productGroupId, storeGuid);
		}

        public void SaveFeatureValue(string PortalId, string strStoreGuid, FeatureValueInfo featureValue, string Token)
		{
            int portalId = -1;
            Int32.TryParse(PortalId, out portalId);
            if (portalId < 0)
                throw new Exception("PortalId must be zero or greater");

            if (featureValue == null)
                throw new Exception("FeatureValue must not be null");
            
            Guid storeGuid = new Guid(strStoreGuid);
            if (storeGuid == Guid.Empty)
                throw new Exception("StoreGuid must be valid!");

            ImportController.SaveFeatureValue(portalId, featureValue, storeGuid);
        }

        public void SaveProductGroupListItem(string PortalId, string strStoreGuid, string ProductGroupId, string FeatureListItemId, string Token)
		{
            int portalId = -1;
            Int32.TryParse(PortalId, out portalId);
            if (portalId < 0)
                throw new Exception("PortalId must be zero or greater");

            int productGroupId = -1;
            Int32.TryParse(ProductGroupId, out productGroupId);
            if (productGroupId < 0)
                throw new Exception("ProductGroupId must be zero or greater");

            int featureListItemId = -1;
            Int32.TryParse(FeatureListItemId, out featureListItemId);
            if (featureListItemId < 0)
                throw new Exception("FeatureListItemId must be zero or greater");

            Guid storeGuid = new Guid(strStoreGuid);
            if (storeGuid == Guid.Empty)
                throw new Exception("StoreGuid must be valid!");

            ImportController.SaveProductGroupListItem(portalId, productGroupId, featureListItemId, storeGuid);
		}

        public void DeleteProductGroupListItems(string PortalId, string strStoreGuid, string Token)
		{
            int portalId = -1;
            Int32.TryParse(PortalId, out portalId);
            if (portalId < 0)
                throw new Exception("PortalId must be zero or greater");

            Guid storeGuid = new Guid(strStoreGuid);
            if (storeGuid == Guid.Empty)
                throw new Exception("StoreGuid must be valid!");

            ImportController.DeleteProductGroupListItems(portalId, storeGuid);
        }

        public void SaveImage(string PortalId, string strStoreGuid, int productId, byte[] pictureData, string fileName, string Token)
		{
            int portalId = -1;
            Int32.TryParse(PortalId, out portalId);
            if (portalId < 0)
                throw new Exception("PortalId must be zero or greater");

            if (productId < 0)
                throw new Exception("ProductId must be zero or greater");

            Guid storeGuid = new Guid(strStoreGuid);
            if (storeGuid == Guid.Empty)
                throw new Exception("StoreGuid must be valid!");

            ImportController.SaveImage(portalId, productId, pictureData, fileName, storeGuid);
		}
        
        public void DeleteProductImages(string PortalId, string strStoreGuid, int productId, string Token)
		{
            int portalId = -1;
            Int32.TryParse(PortalId, out portalId);
            if (portalId < 0)
                throw new Exception("PortalId must be zero or greater");

            if (productId < 0)
                throw new Exception("ProductId must be zero or greater");

            Guid storeGuid = new Guid(strStoreGuid);
            if (storeGuid == Guid.Empty)
                throw new Exception("StoreGuid must be valid!");

            ImportController.DeleteProductImages(portalId, productId, storeGuid);
		}
        
        public void DeleteEmptyImageDirectories(string PortalId, string Token)
        {
            // Deletion of folders initiates App Restart -> Token expires !
            int portalId = -1;
            Int32.TryParse(PortalId, out portalId);
            if (portalId < 0)
                throw new Exception("PortalId must be zero or greater");

            ImportController.DeleteEmptyImageDirectories(portalId);
        }
        
        public void DeleteProductImagesAll(string PortalId, string strStoreGuid, string Token)
		{
            int portalId = -1;
            Int32.TryParse(PortalId, out portalId);
            if (portalId < 0)
                throw new Exception("PortalId must be zero or greater");
            
            Guid storeGuid = new Guid(strStoreGuid);
            if (storeGuid == Guid.Empty)
                throw new Exception("StoreGuid must be valid!");

            ImportController.DeleteProductImagesAll(portalId, storeGuid);
        }

        public void ImportStore(string PortalId, BBStoreInfo bbStore, string Token)
        {
            int portalId = -1;
            Int32.TryParse(PortalId, out portalId);
            if (portalId < 0)
                throw new Exception("PortalId must be zero or greater");
            ImportController.ImportStore(portalId, bbStore);
        }

        public BBStoreInfo ExportStore(string PortalId, string strStoreGuid, string Token)
        {
            int portalId = -1;
            Int32.TryParse(PortalId, out portalId);
            if (portalId < 0)
                throw new Exception("PortalId must be zero or greater");

            Guid storeGuid = new Guid(strStoreGuid);
            if (storeGuid == Guid.Empty)
                throw new Exception("StoreGuid must be valid!");

            return ImportController.ExportStore(portalId, storeGuid);
        }

        public void ResetStore(string PortalId, string strStoreGuid, bool skipImages, string Token)
        {
            int portalId = -1;
            Int32.TryParse(PortalId, out portalId);
            if (portalId < 0)
                throw new Exception("PortalId must be zero or greater");

            Guid storeGuid = new Guid(strStoreGuid);
            if (storeGuid == Guid.Empty)
                throw new Exception("StoreGuid must be valid!");

            ImportController.ResetStore(portalId, skipImages, storeGuid);
        }
		
        #endregion
	}
}
