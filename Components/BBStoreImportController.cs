using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.FileSystem;
using FileInfo = System.IO.FileInfo;

namespace Bitboxx.DNNModules.BBStore
{
    public class BBStoreImportController
    {
        #region Fields and Properties
        
        private BBStoreController _controller;
        public BBStoreController Controller
        {
            get
            {
                if (_controller == null)
                    _controller = new BBStoreController();
                return _controller;
            }
        }

        #endregion

        #region Maintenance  methods
        public BBStoreInfo ExportStore(int portalId, Guid storeGuid)
        {
            // TODO: Der Export auf die eigene StoreID exportiert ALLES, nicht nur die Daten die im Store selber angelegt wurden. 
            BBStoreInfo bbStore = new BBStoreInfo();

            bbStore.StoreGuid = storeGuid;
            bbStore.StoreName = (storeGuid == BBStoreController.StoreGuid ? BBStoreController.StoreName : GetImportStoreName(storeGuid));
            bbStore.ProductGroup = GetProductGroups(portalId,storeGuid);
            bbStore.ProductGroupLang = GetProductGroupsLangs(portalId, storeGuid);
            bbStore.Product = GetSimpleProducts(portalId,storeGuid);
            bbStore.ProductLang = GetSimpleProductsLangs(portalId,storeGuid);
            bbStore.ProductInGroup = GetProductsInGroups(portalId, storeGuid);
            bbStore.FeatureGroup = GetFeatureGroups(portalId,storeGuid);
            bbStore.FeatureGroupLang = GetFeatureGroupsLangs(portalId,storeGuid);
            bbStore.FeatureList = GetFeatureLists(portalId, storeGuid);
            bbStore.FeatureListLang = GetFeatureListsLangs(portalId, storeGuid);
            bbStore.FeatureListItem = GetFeatureListItems(portalId,storeGuid);
            bbStore.FeatureListItemLang = GetFeatureListItemsLangs(portalId,storeGuid);
            bbStore.Feature = GetFeatures(portalId,storeGuid);
            bbStore.FeatureLang = GetFeaturesLangs(portalId,storeGuid);
            bbStore.ProductGroupFeature = GetProductGroupFeatures(portalId, storeGuid);
            bbStore.FeatureValue = GetFeatureValues(portalId,storeGuid);
            bbStore.ProductGroupListItem = GetProductGroupListItems(portalId, storeGuid);
            bbStore.Unit = GetUnits(portalId, storeGuid);
            bbStore.UnitLang = GetUnitLangs(portalId, storeGuid);

            return bbStore;
        }

        public void ImportStore(int portalId, BBStoreInfo bbStore)
        {
            Guid storeGuid = bbStore.StoreGuid;

            SaveImportStore(storeGuid, bbStore.StoreName);

            foreach (ProductGroupInfo productGroup in bbStore.ProductGroup)
            {
                SaveProductGroup(portalId, productGroup, storeGuid);
            }
            foreach (ProductGroupLangInfo productGroupLang in bbStore.ProductGroupLang)
            {
                SaveProductGroupLang(portalId, productGroupLang, storeGuid);
            }
            foreach (SimpleProductInfo simpleProduct in bbStore.Product)
            {
                SaveProduct(portalId, simpleProduct, storeGuid);
            }
            foreach (SimpleProductLangInfo simpleProductLang in bbStore.ProductLang)
            {
                SaveProductLang(portalId, simpleProductLang, storeGuid);
            }
            foreach (ProductInGroupInfo productInGroup in bbStore.ProductInGroup)
            {
                SaveProductInGroup(portalId, productInGroup.SimpleProductId, productInGroup.ProductGroupId, storeGuid);
            }
            foreach (FeatureGroupInfo featureGroup in bbStore.FeatureGroup)
            {
                SaveFeatureGroup(portalId, featureGroup, storeGuid);
            }
            foreach (FeatureGroupLangInfo featureGroupLang in bbStore.FeatureGroupLang)
            {
                SaveFeatureGroupLang(portalId, featureGroupLang, storeGuid);
            }
            foreach (FeatureListInfo featureList in bbStore.FeatureList)
            {
                SaveFeatureList(portalId, featureList, storeGuid);
            }
            foreach (FeatureListLangInfo featureListLang in bbStore.FeatureListLang)
            {
                SaveFeatureListLang(portalId, featureListLang, storeGuid);
            }
            foreach (FeatureListItemInfo featureListItem in bbStore.FeatureListItem)
            {
                SaveFeatureListItem(portalId, featureListItem, storeGuid);
            }
            foreach (FeatureListItemLangInfo featureListItemLang in bbStore.FeatureListItemLang)
            {
                SaveFeatureListItemLang(portalId, featureListItemLang, storeGuid);
            }
            foreach (FeatureInfo feature in bbStore.Feature)
            {
                SaveFeature(portalId, feature, storeGuid);
            }
            foreach (FeatureLangInfo featureLang in bbStore.FeatureLang)
            {
                SaveFeatureLang(portalId, featureLang, storeGuid);
            }
            if (bbStore.ProductGroupFeature.Count > 0)
            {
                Controller.DeleteProductGroupFeaturesByPortal(portalId);
                foreach (ProductGroupFeatureInfo productGroupFeature in bbStore.ProductGroupFeature)
                {
                    SaveProductGroupFeature(portalId, productGroupFeature.FeatureId, productGroupFeature.ProductGroupId, storeGuid);
                }
            }
            if (bbStore.FeatureValue.Count > 0)
            {
                Controller.DeleteFeatureValuesByPortal(portalId);
                foreach (FeatureValueInfo featureValue in bbStore.FeatureValue)
                {
                    SaveFeatureValue(portalId, featureValue, storeGuid);
                }
            }
            DeleteProductGroupListItems(portalId,storeGuid);
            foreach (ProductGroupListItemInfo productGroupListItem in bbStore.ProductGroupListItem)
            {
                SaveProductGroupListItem(portalId, productGroupListItem.ProductGroupId, productGroupListItem.FeatureListItemId, storeGuid);
            }
            foreach (UnitInfo unit in bbStore.Unit)
            {
                SaveUnit(portalId, unit, storeGuid);
            }
            foreach (UnitLangInfo unitLang in bbStore.UnitLang)
            {
                SaveUnitLang(portalId, unitLang, storeGuid);
            }
            // Deletion of Products no more used
            // bbstore.Product is ID-converted because of the routines running before
            List<int> allProducts = GetImportRelationForeignIdsByTable(portalId, "PRODUCT", storeGuid);
            List<int> newProducts = (from p in bbStore.Product select p.SimpleProductId).ToList<int>();

            List<int> deleteProducts = allProducts.Except(newProducts).ToList();
            foreach (int deleteProduct in deleteProducts)
            {
                //int foreignId = GetImportRelationForeignId(portalId, "PRODUCT", deleteProduct, storeGuid);
                DeleteProductImages(portalId, deleteProduct, storeGuid);
                DeleteProduct(portalId,deleteProduct,storeGuid);
            }
            DeleteEmptyImageDirectories(portalId);
        }

        public void ResetStore(int portalId, bool skipImages, Guid storeGuid)
        {
            DeleteFeatures(portalId,storeGuid);
            DeleteFeatureGroups(portalId, storeGuid);
            if (!skipImages)
                DeleteProductImagesAll(portalId, storeGuid);
            DeleteProducts(portalId, storeGuid);
            DeleteFeatureLists(portalId,storeGuid);
            DeleteProductGroups(portalId,storeGuid);
            DeleteUnits(portalId, storeGuid);
            DeleteImportRelationByStore(portalId,storeGuid);
            Controller.ReseedTables();
            DeleteEmptyImageDirectories(portalId);
        }

        public void ResetStore(int portalId, bool skipImages, Guid filterSessionId, Guid storeGuid)
        {
            ResetStore(portalId,skipImages, storeGuid);
            Controller.DeleteProductFilters(portalId, filterSessionId);
        }

        public List<Guid> GetStoreGuids(int portalId)
        {
            List<Guid> retVal = new List<Guid>();
            retVal.Add(BBStoreController.StoreGuid);
            IDataReader dr = DataProvider.Instance().GetStoreGuids(portalId);
            
            while (dr.Read())
            {
                if (dr[0] is Guid)
                    retVal.Add((Guid)dr[0]);
            }
            dr.Close();
            return retVal;
        }

        public void SaveImportStore(Guid storeGuid, string storeName)
        {
            DataProvider.Instance().SaveImportStore(storeGuid,storeName);
        }

        public string GetImportStoreName(Guid storeGuid)
        {
            return DataProvider.Instance().GetImportStoreName(storeGuid);
        }

        public void DeleteImportStore(Guid storeGuid)
        {
            DataProvider.Instance().DeleteImportStore(storeGuid);
        }
        #endregion

        #region Worker methods

        public void DeleteProduct(int portalId, int productId, Guid storeGuid)
        {
            int ownId = GetImportRelationOwnId(portalId, "PRODUCT", productId, storeGuid);
            Controller.DeleteSimpleProduct(ownId);
            DeleteImportRelationByOwnId(portalId, "PRODUCT", ownId);
        }

        public void DeleteProducts(int portalId, Guid storeGuid)
        {
            List<int> ownIds = GetImportRelationOwnIdsByTable(portalId, "PRODUCT", storeGuid);
            foreach (int ownId in ownIds)
            {
                Controller.DeleteSimpleProduct(ownId);
            }
            DeleteImportRelationByTable(portalId, "PRODUCT", storeGuid);
        }

        public void DeleteFeatureGroups(int portalId, Guid storeGuid)
        {
            List<int> ownIds = GetImportRelationOwnIdsByTable(portalId, "FEATUREGROUP", storeGuid);
            foreach (int ownId in ownIds)
            {
                Controller.DeleteFeatureGroup(ownId);
            }
            DeleteImportRelationByTable(portalId, "FEATUREGROUP", storeGuid);
        }

        public void DeleteProductGroups(int portalId, Guid storeGuid)
        {
            List<int> ownIds = GetImportRelationOwnIdsByTable(portalId, "PRODUCTGROUP", storeGuid);
            foreach (int ownId in ownIds)
            {
                Controller.DeleteProductGroup(ownId);
            }
            DeleteImportRelationByTable(portalId, "PRODUCTGROUP", storeGuid);
        }

        public void DeleteFeatureLists(int portalId, Guid storeGuid)
        {
            List<int> ownIds = GetImportRelationOwnIdsByTable(portalId, "FEATURELIST", storeGuid);
            foreach (int ownId in ownIds)
            {
                Controller.DeleteFeatureList(ownId);
            }
            DeleteImportRelationByTable(portalId, "FEATURELIST", storeGuid);
            DeleteImportRelationByTable(portalId, "FEATURELISTITEM", storeGuid);
        }

        public void DeleteFeatures(int portalId, Guid storeGuid)
        {
            List<int> ownIds = GetImportRelationOwnIdsByTable(portalId, "FEATURE", storeGuid);
            foreach (int ownId in ownIds)
            {
                Controller.DeleteFeature(ownId);
            }
            DeleteImportRelationByTable(portalId, "FEATURE", storeGuid);
        }

        public void DeleteProductGroupListItems(int portalId, Guid storeGuid)
        {
            List<int> ownIds = GetImportRelationOwnIdsByTable(portalId, "PRODUCTGROUP", storeGuid);
            foreach (int ownId in ownIds)
            {
                Controller.DeleteProductGroupListItemsByProductGroup(ownId);
            }
        }

        public void DeleteCustomerAddress(int portalId, int customerAddressId)
        {
            Controller.DeleteCustomerAddress(customerAddressId);
            DeleteImportRelationByOwnId(portalId, "CUSTOMERADDRESS", customerAddressId);
        }

        public void DeleteUnits(int portalId, Guid storeGuid)
        {
            List<int> ownIds = GetImportRelationOwnIdsByTable(portalId, "UNIT", storeGuid);
            foreach (int ownId in ownIds)
            {
                Controller.DeleteUnit(ownId);
            }
            DeleteImportRelationByTable(portalId, "UNIT", storeGuid);
        }

        public void DeleteEmptyImageDirectories(int portalId)
        {
            try
            {
                // We have to translate the imagePath !
               
                PortalController pc = new PortalController();
                PortalInfo pi = pc.GetPortal(portalId);
                BBStoreController ctrl = new BBStoreController();
                Hashtable storeSettings = ctrl.GetStoreSettings(portalId);
                string homeDir = Path.Combine(pi.HomeDirectoryMapPath, "Images\\Produkt\\");
                if (storeSettings["ProductImageDir"] != null)
                {
                    // HomeDir is physical portal path + root for productimages from storesettings
                    homeDir = Path.Combine(pi.HomeDirectoryMapPath, ((string)storeSettings["ProductImageDir"])).Replace("/","\\");
                }

                var folders = FolderManager.Instance.GetFolders(portalId);
                foreach (var folder in folders)
                {
                    if (folder.PhysicalPath.Contains(homeDir) && folder.PhysicalPath != homeDir)
                    {
                        List<IFileInfo> files = FolderManager.Instance.GetFiles(folder).ToList();
                        if (files.Count == 0)
                            FolderManager.Instance.DeleteFolder(folder.FolderID);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new IOException(string.Format("DeleteEmptyImageDirectories: Error when deleting Files or Folder: {0}", ex.ToString()));
            }
        }

        public void DeleteProductImages(int portalId, int productId, Guid storeGuid)
        {
            int ownId = GetImportRelationOwnId(portalId, "PRODUCT", productId, storeGuid);
            if (ownId < 0)
                throw new KeyNotFoundException(String.Format("DeleteProductImages: ProductId {0} not found in ImportRelation",productId));


            // Retrieve product
            SimpleProductInfo product = Controller.GetSimpleProductByProductId(portalId, ownId);

            if (!String.IsNullOrEmpty(product.Image))
            {
                PortalController pc = new PortalController();
                PortalInfo pi = pc.GetPortal(portalId);
                FileInfo fi = new FileInfo(pi.HomeDirectoryMapPath + product.Image.Replace("/", "\\"));
                string folderPath = product.Image.Replace("\\", "/").Replace(fi.Name, "");

                try
                {
                    IFolderInfo folder = FolderManager.Instance.GetFolder(portalId, folderPath);
                    if (folder != null)
                    {
                        List<IFileInfo> files = FolderManager.Instance.GetFiles(folder).ToList();
                        foreach (IFileInfo file in files)
                        {
                            FileManager.Instance.DeleteFile(file);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new IOException(string.Format("DeleteProductImages: Error when deleting Files or Folder: {0}", ex.ToString()));
                }
            }
        }

        public void DeleteProductImagesAll(int portalId)
        {
            // Retrieve Image Path
            PortalController pc = new PortalController();
            PortalInfo pi = pc.GetPortal(portalId);
            string imagePath = Path.Combine(pi.HomeDirectoryMapPath, "Images/Produkt");

            BBStoreController ctrl = new BBStoreController();
            Hashtable storeSettings = ctrl.GetStoreSettings(portalId);

            if (storeSettings["ProductImageDir"] != null)
            {
                imagePath = Path.Combine(pi.HomeDirectoryMapPath, (string) storeSettings["ProductImageDir"]);
            }

            if (Directory.Exists(imagePath))
                Directory.Delete(imagePath, true);

        }

        public void DeleteProductImagesAll(int portalId, Guid storeGuid)
        {
            List<int> foreignIds = GetImportRelationForeignIdsByTable(portalId, "PRODUCT", storeGuid);
            foreach (int foreignId in foreignIds)
            {
                DeleteProductImages(portalId,foreignId,storeGuid);
            }
        }

        public ProductGroupInfo GetProductGroupByName(int portalId, string language, string productGroupName, Guid storeGuid)
        {
            ProductGroupInfo productGroup = Controller.GetProductGroupByName(portalId, language, productGroupName);
            if (storeGuid != BBStoreController.StoreGuid)
            {
                productGroup.ProductGroupId = GetImportRelationForeignId(portalId, "PRODUCTGROUP", productGroup.ProductGroupId, storeGuid);
                productGroup.ParentId = GetImportRelationForeignId(portalId, "PRODUCTGROUP", productGroup.ParentId, storeGuid);
            }
            return productGroup;
        }

        public List<ProductGroupInfo> GetProductGroups(int portalId, Guid storeGuid)
        {
            List<ProductGroupInfo> productGroups = Controller.GetProductGroups(portalId);
            if (storeGuid == BBStoreController.StoreGuid)
                return productGroups;

            List<ProductGroupInfo> result = new List<ProductGroupInfo>();
            
            foreach (ProductGroupInfo productGroup in productGroups)
            {
                int productGroupId = GetImportRelationForeignId(portalId, "PRODUCTGROUP", productGroup.ProductGroupId, storeGuid);
                if (productGroupId > -1)
                {
                    productGroup.ProductGroupId = productGroupId;
                    productGroup.ParentId = GetImportRelationForeignId(portalId, "PRODUCTGROUP", productGroup.ParentId, storeGuid);
                    result.Add(productGroup);
                }
            }
            return result;
        }

        public List<ProductGroupLangInfo> GetProductGroupsLangs(int portalId, Guid storeGuid)
        {
            List<ProductGroupLangInfo> productGroupsLangs = Controller.GetProductGroupLangsByPortal(portalId);
            if (storeGuid == BBStoreController.StoreGuid)
                return productGroupsLangs;

            List<ProductGroupLangInfo> result = new List<ProductGroupLangInfo>();

            foreach (ProductGroupLangInfo productGroupLang in productGroupsLangs)
            {
                int productGroupId = GetImportRelationForeignId(portalId, "PRODUCTGROUP", productGroupLang.ProductGroupId, storeGuid);
                if (productGroupId > -1)
                {
                    productGroupLang.ProductGroupId = productGroupId;
                    result.Add(productGroupLang);
                }
            }
            return result;
        }

        public List<SimpleProductInfo> GetSimpleProducts(int portalId, Guid storeGuid)
        {
            List<SimpleProductInfo> products = Controller.GetSimpleProducts(portalId);
            if (storeGuid == BBStoreController.StoreGuid)
                return products;

            List<SimpleProductInfo> result = new List<SimpleProductInfo>();

            foreach (SimpleProductInfo product in products)
            {
                int simpleProductId = GetImportRelationForeignId(portalId, "PRODUCT", product.SimpleProductId, storeGuid);
                if (simpleProductId > -1 )
                {
                    if (product.UnitId > 0)
                    {
                        int unitId = GetImportRelationForeignId(portalId, "UNIT", product.UnitId, storeGuid);
                        product.UnitId = unitId;
                    }
                    product.SimpleProductId = simpleProductId;
                    result.Add(product);
                }
            }
            return result;
        }

        public List<SimpleProductLangInfo> GetSimpleProductsLangs(int portalId, Guid storeGuid)
        {
            List<SimpleProductLangInfo> productsLangs = Controller.GetSimpleProductLangsByPortal(portalId);
            if (storeGuid == BBStoreController.StoreGuid)
                return productsLangs;

            List<SimpleProductLangInfo> result = new List<SimpleProductLangInfo>();

            foreach (SimpleProductLangInfo productLang in productsLangs)
            {
                int simpleProductId = GetImportRelationForeignId(portalId, "PRODUCT", productLang.SimpleProductId, storeGuid);
                if (simpleProductId > -1)
                {
                    productLang.SimpleProductId = simpleProductId;
                    result.Add(productLang);
                }
            }
            return result;
        }

        public List<ProductInGroupInfo> GetProductsInGroups(int portalId, Guid storeGuid)
        {
            List<ProductInGroupInfo> productInGroups = Controller.GetProductsInGroupByPortal(portalId);
            if (storeGuid == BBStoreController.StoreGuid)
                return productInGroups;

            List<ProductInGroupInfo> result = new List<ProductInGroupInfo>();

            foreach (ProductInGroupInfo productInGroup in productInGroups)
            {
                int productGroupId = GetImportRelationForeignId(portalId, "PRODUCTGROUP", productInGroup.ProductGroupId, storeGuid);
                int simpleProductId = GetImportRelationForeignId(portalId, "PRODUCT", productInGroup.SimpleProductId, storeGuid);
                if (simpleProductId > -1 && productGroupId > -1)
                {
                    productInGroup.ProductGroupId = productGroupId;
                    productInGroup.SimpleProductId = simpleProductId;
                    result.Add(productInGroup);
                }
            }
            return result;
        }

        public List<FeatureGroupInfo> GetFeatureGroups(int portalId, Guid storeGuid)
        {
            List<FeatureGroupInfo> featureGroups = Controller.GetFeatureGroups(portalId);
            if (storeGuid == BBStoreController.StoreGuid)
                return featureGroups;

            List<FeatureGroupInfo> result = new List<FeatureGroupInfo>();

            foreach (FeatureGroupInfo featureGroup in featureGroups)
            {
                int featureGroupId = GetImportRelationForeignId(portalId, "FEATUREGROUP", featureGroup.FeatureGroupId, storeGuid);
                if (featureGroupId > -1)
                {
                    featureGroup.FeatureGroupId = featureGroupId;
                    result.Add(featureGroup);
                }
            }
            return result;
        }

        public List<FeatureGroupLangInfo> GetFeatureGroupsLangs(int portalId, Guid storeGuid)
        {
            List<FeatureGroupLangInfo> featureGroupsLangs = Controller.GetFeatureGroupLangsByPortal(portalId);
            if (storeGuid == BBStoreController.StoreGuid)
                return featureGroupsLangs;

            List<FeatureGroupLangInfo> result = new List<FeatureGroupLangInfo>();

            foreach (FeatureGroupLangInfo featureGroupLang in featureGroupsLangs)
            {
                int featureGroupId = GetImportRelationForeignId(portalId, "FEATUREGROUP", featureGroupLang.FeatureGroupId, storeGuid);
                if (featureGroupId > -1)
                {
                    featureGroupLang.FeatureGroupId = featureGroupId;
                    result.Add(featureGroupLang);
                }
            }
            return result;
        }

        public List<FeatureListInfo> GetFeatureLists(int portalId, Guid storeGuid)
        {
            List<FeatureListInfo> featureLists = Controller.GetFeatureLists(portalId);
            if (storeGuid == BBStoreController.StoreGuid)
                return featureLists;

            List<FeatureListInfo> result = new List<FeatureListInfo>();
            
            foreach (FeatureListInfo featureList in featureLists)
            {
                int featureListId = GetImportRelationForeignId(portalId, "FEATURELIST", featureList.FeatureListId, storeGuid);
                if (featureListId > -1)
                {
                    featureList.FeatureListId = featureListId;
                    result.Add(featureList);
                }
            }
            return result;
        }

        public List<FeatureListLangInfo> GetFeatureListsLangs(int portalId, Guid storeGuid)
        {
            List<FeatureListLangInfo> featureListsLangs = Controller.GetFeatureListLangs(portalId);
            if (storeGuid == BBStoreController.StoreGuid)
                return featureListsLangs;

            List<FeatureListLangInfo> result = new List<FeatureListLangInfo>();
            
            foreach (FeatureListLangInfo featureListLang in featureListsLangs)
            {
                int featureListId = GetImportRelationForeignId(portalId, "FEATURELIST", featureListLang.FeatureListId, storeGuid);
                if (featureListId > -1)
                {
                    featureListLang.FeatureListId = featureListId;
                    result.Add(featureListLang);
                }
            }
            return result;
        }

        public List<FeatureListItemInfo> GetFeatureListItems(int portalId, Guid storeGuid)
        {
            List<FeatureListItemInfo> featureListItems = Controller.GetFeatureListItems(portalId);
            if (storeGuid == BBStoreController.StoreGuid)
                return featureListItems;

            List<FeatureListItemInfo> result = new List<FeatureListItemInfo>();

            foreach (FeatureListItemInfo featureListItem in featureListItems)
            {
                int featureListId = GetImportRelationForeignId(portalId, "FEATURELIST", featureListItem.FeatureListId, storeGuid);
                int featureListItemId = GetImportRelationForeignId(portalId, "FEATURELISTITEM", featureListItem.FeatureListItemId, storeGuid);
                if (featureListId > -1 && featureListItemId > -1)
                {
                    featureListItem.FeatureListId = featureListId;
                    featureListItem.FeatureListItemId = featureListItemId;
                    result.Add(featureListItem);
                }
            }
            return result;
        }

        public List<FeatureListItemLangInfo> GetFeatureListItemsLangs(int portalId, Guid storeGuid)
        {
            List<FeatureListItemLangInfo> featureListItemsLangs = Controller.GetFeatureListItemLangs(portalId);
            if (storeGuid == BBStoreController.StoreGuid)
                return featureListItemsLangs;

            List<FeatureListItemLangInfo> result = new List<FeatureListItemLangInfo>();

            foreach (FeatureListItemLangInfo featureListItemLang in featureListItemsLangs)
            {
                int featureListItemId = GetImportRelationForeignId(portalId, "FEATURELISTITEM", featureListItemLang.FeatureListItemId, storeGuid);
                if (featureListItemId > -1)
                {
                    featureListItemLang.FeatureListItemId = featureListItemId;
                    result.Add(featureListItemLang);
                }
            }
            return result;
        }

        public List<FeatureInfo> GetFeatures(int portalId, Guid storeGuid)
        {
            List<FeatureInfo> features = Controller.GetFeatures(portalId);
            if (storeGuid == BBStoreController.StoreGuid)
                return features;

            List<FeatureInfo> result = new List<FeatureInfo>();

            foreach (FeatureInfo feature in features)
            {
                int featureId = GetImportRelationForeignId(portalId, "FEATURE", feature.FeatureId, storeGuid);
                int featureListId = GetImportRelationForeignId(portalId, "FEATURELIST", feature.FeatureListId, storeGuid);
                int featureGroupId = GetImportRelationForeignId(portalId, "FEATUREGROUP", feature.FeatureGroupId, storeGuid);
                if (featureId > -1 && featureListId > -1 && featureGroupId > -1)
                {
                    feature.FeatureId = featureId;
                    feature.FeatureListId = featureListId;
                    feature.FeatureGroupId = featureGroupId;
                    result.Add(feature);
                }
            }
            return result;
        }

        public List<FeatureLangInfo> GetFeaturesLangs(int portalId, Guid storeGuid)
        {
            List<FeatureLangInfo> featuresLangs = Controller.GetFeatureLangsByPortal(portalId);
            if (storeGuid == BBStoreController.StoreGuid)
                return featuresLangs;

            List<FeatureLangInfo> result = new List<FeatureLangInfo>();

            foreach (FeatureLangInfo featureLang in featuresLangs)
            {
                int featureId = GetImportRelationForeignId(portalId, "FEATURE", featureLang.FeatureId, storeGuid);
                if (featureId > -1)
                {
                    featureLang.FeatureId = featureId;
                    result.Add(featureLang);
                }
            }
            return result;
        }

        public List<ProductGroupFeatureInfo> GetProductGroupFeatures(int portalId, Guid storeGuid)
        {
            List<ProductGroupFeatureInfo> productGroupFeatures = Controller.GetProductGroupFeaturesByPortal(portalId);
            if (storeGuid == BBStoreController.StoreGuid)
                return productGroupFeatures;

            List<ProductGroupFeatureInfo> result = new List<ProductGroupFeatureInfo>();

            foreach (ProductGroupFeatureInfo productGroupFeature in productGroupFeatures)
            {
                int productGroupId = GetImportRelationForeignId(portalId, "PRODUCTGROUP", productGroupFeature.ProductGroupId, storeGuid);
                int featureId = GetImportRelationForeignId(portalId, "FEATURE", productGroupFeature.FeatureId, storeGuid);
                if (productGroupId > -1 && featureId > -1)
                {
                    productGroupFeature.ProductGroupId = productGroupId;
                    productGroupFeature.FeatureId = featureId;
                    result.Add(productGroupFeature);
                }
            }
            return result;
        }

        public List<FeatureValueInfo> GetFeatureValues(int portalId, Guid storeGuid)
        {
            List<FeatureValueInfo> featureValues = Controller.GetFeatureValuesByPortal(portalId);
            if (storeGuid == BBStoreController.StoreGuid)
                return featureValues;

            List<FeatureValueInfo> result = new List<FeatureValueInfo>();

            foreach (FeatureValueInfo featureValue in featureValues)
            {
                int featureId = GetImportRelationForeignId(portalId, "FEATURE", featureValue.FeatureId, storeGuid);
                int featureValueId = GetImportRelationForeignId(portalId, "FEATUREVALUE", featureValue.FeatureValueId, storeGuid);
                int productId = GetImportRelationForeignId(portalId, "PRODUCT", featureValue.ProductId, storeGuid);
                
                if (featureId > -1 && featureValueId > -1 && productId > -1)
                {
                    int? featureListItemId = featureValue.FeatureListItemId;
                    if (featureValue.FeatureListItemId != null && featureValue.FeatureListItemId > -1)
                    {
                        featureListItemId = GetImportRelationForeignId(portalId, "FEATURELISTITEM", (int)featureValue.FeatureListItemId, storeGuid);
                    }
                        
                    featureValue.FeatureId = featureId;
                    featureValue.FeatureValueId = featureValueId;
                    featureValue.FeatureListItemId = featureListItemId;
                    featureValue.ProductId = productId;
                    result.Add(featureValue);
                }
            }
            return result;
        }

        public List<ProductGroupListItemInfo> GetProductGroupListItems(int portalId, Guid storeGuid)
        {
            List<ProductGroupListItemInfo> productGroupListItems = Controller.GetProductGroupListItemsByPortal(portalId);
            if (storeGuid == BBStoreController.StoreGuid)
                return productGroupListItems;

            List<ProductGroupListItemInfo> result = new List<ProductGroupListItemInfo>();

            foreach (ProductGroupListItemInfo productGroupListItem in productGroupListItems)
            {
                int productGroupId = GetImportRelationForeignId(portalId, "PRODUCTGROUP", productGroupListItem.ProductGroupId, storeGuid);
                int featureListItemId = GetImportRelationForeignId(portalId, "FEATURELISTITEM", productGroupListItem.FeatureListItemId, storeGuid);
                if (productGroupId > -1 && featureListItemId > -1)
                {
                    productGroupListItem.FeatureListItemId = featureListItemId;
                    productGroupListItem.ProductGroupId = productGroupId;
                    result.Add(productGroupListItem);
                }
            }
            return result;
        }

        private List<UnitInfo> GetUnits(int portalId, Guid storeGuid)
        {
            List<UnitInfo> units = Controller.GetUnits(portalId);
            if (storeGuid == BBStoreController.StoreGuid)
                return units;
            
            List<UnitInfo> result = new List<UnitInfo>();
            foreach (UnitInfo unit in units)
            {
                int unitId = GetImportRelationForeignId(portalId, "UNIT", unit.UnitId, storeGuid);
                if (unitId > -1)
                {
                    unit.UnitId = unitId;
                    result.Add(unit);
                }
            }
            return result;
        }

        private List<UnitLangInfo> GetUnitLangs(int portalId, Guid storeGuid)
        {
            List<UnitLangInfo> unitLangs = Controller.GetUnitLangsByPortal(portalId);
            if (storeGuid == BBStoreController.StoreGuid)
                return unitLangs;

            List<UnitLangInfo> result = new List<UnitLangInfo>();
            foreach (UnitLangInfo unitLang in unitLangs)
            {
                int unitId = GetImportRelationForeignId(portalId, "UNIT", unitLang.UnitId, storeGuid);
                if (unitId > -1)
                {
                    unitLang.UnitId = unitId;
                    result.Add(unitLang);
                }
            }
            return result;
        }

        public void SaveProductGroupListItem(int portalId, int productGroupId, int featureListItemId, Guid storeGuid)
        {
            int ownFeatureListItemId = GetImportRelationOwnId(portalId, "FEATURELISTITEM", featureListItemId, storeGuid);
            if (ownFeatureListItemId < 0)
                throw new KeyNotFoundException(String.Format("SaveProductGroupListItem: FeatureListItemId {0} not found in Importrelation",featureListItemId));

            int ownProductGroupId = GetImportRelationOwnId(portalId, "PRODUCTGROUP", productGroupId, storeGuid);
            if (ownProductGroupId < 0)
                throw new KeyNotFoundException(String.Format("SaveProductGroupListItem: ProductGroupId {0} not found in ImportRelation",productGroupId));

            Controller.DeleteProductGroupListItem(ownProductGroupId, ownFeatureListItemId);
            Controller.NewProductGroupListItem(ownProductGroupId, ownFeatureListItemId);
        }

        public void SaveImage(int portalId, int productId, byte[] pictureData, string fileName, Guid storeGuid)
        {
            int ownId = GetImportRelationOwnId(portalId, "PRODUCT", productId, storeGuid);
            if (ownId < 0)
                throw new KeyNotFoundException(String.Format("SaveImage: ProductId {0} not found in ImportRelation",productId));

            // We have to translate the imagePath !
            BBStoreController ctrl = new BBStoreController();
            Hashtable storeSettings = ctrl.GetStoreSettings(portalId);
            if (storeSettings["ProductImageDir"] != null)
            {
                if (fileName.StartsWith("Images/Produkt/"))
                    fileName = fileName.Replace("Images/Produkt/", (string) storeSettings["ProductImageDir"]);
                else
                    fileName = (string) storeSettings["ProductImageDir"] + fileName;
            }

            PortalController pc = new PortalController();
            PortalInfo pi = pc.GetPortal(portalId);
            string imagePath = Path.Combine(pi.HomeDirectoryMapPath,fileName);
            FileInfo fi = new FileInfo(imagePath);

            //FolderMappingInfo folderMapping = FolderMappingController.Instance.GetDefaultFolderMapping(portalId);
            
            string folderPath = fileName.Replace("\\", "/").Replace(fi.Name, "");
            IFolderInfo folder = FolderManager.Instance.GetFolder(portalId, folderPath);;
            
            if (folder == null)
            {
                folder = FolderManager.Instance.AddFolder(portalId,folderPath);
            }
            
            Stream pictureStream = new MemoryStream(pictureData);
            FileManager.Instance.AddFile(folder, fi.Name, pictureStream, true);
        }

        public void SaveFeatureGroup(int portalId, FeatureGroupInfo featureGroup, Guid storeGuid)
        {
            featureGroup.PortalID = portalId;
            int ownId = GetImportRelationOwnId(portalId, "FEATUREGROUP", featureGroup.FeatureGroupId, storeGuid);
            if (ownId > -1)
            {
                featureGroup.FeatureGroupId = ownId;
                featureGroup.PortalID = portalId;
                Controller.UpdateFeatureGroup(featureGroup);
            }
            else
            {
                featureGroup.PortalID = portalId;
                ownId = Controller.NewFeatureGroup(featureGroup);
                NewImportRelation(portalId, "FEATUREGROUP", ownId, featureGroup.FeatureGroupId,storeGuid);
            }
        }

        public int SaveProduct(int portalId, SimpleProductInfo product, Guid storeGuid)
        {
            product.PortalId = portalId;
            
            // We have to translate the image !
            BBStoreController ctrl = new BBStoreController();
            Hashtable storeSettings = ctrl.GetStoreSettings(portalId);
            if (storeSettings["ProductImageDir"] != null)
            {
                if (product.Image.StartsWith("Images/Produkt/"))
                    product.Image = product.Image.Replace("Images/Produkt/", (string)storeSettings["ProductImageDir"]);
                else
                    product.Image = (string)storeSettings["ProductImageDir"] + product.Image;
            }

            int ownId = GetImportRelationOwnId(product.PortalId, "PRODUCT", product.SimpleProductId, storeGuid);
            if (ownId > -1)
            {
                int foreignId = product.SimpleProductId;
                product.SimpleProductId = ownId;
                Controller.UpdateSimpleProduct(product);
                product.SimpleProductId = foreignId;
            }
            else
            {
                ownId = Controller.NewSimpleProduct(product);
                NewImportRelation(product.PortalId, "PRODUCT", ownId, product.SimpleProductId, storeGuid);
            }
            return ownId;
        }

        public void SaveFeatureGroupLang(int portalId, FeatureGroupLangInfo featureGroupLang, Guid storeGuid)
        {
            int ownId = GetImportRelationOwnId(portalId, "FEATUREGROUP", featureGroupLang.FeatureGroupId, storeGuid);
            if (ownId < 0)
                throw new KeyNotFoundException(String.Format("SaveFeatureGroupLang: FeatureGroupId {0} not found in Importrelation",featureGroupLang.FeatureGroupId));

            Controller.DeleteFeatureGroupLang(ownId, featureGroupLang.Language);
            featureGroupLang.FeatureGroupId = ownId;
            Controller.NewFeatureGroupLang(featureGroupLang);
        }

        public void SaveProductLang(int portalId, SimpleProductLangInfo productLang, Guid storeGuid)
        {
            int ownId = GetImportRelationOwnId(portalId, "PRODUCT", productLang.SimpleProductId, storeGuid);
            if (ownId == -1)
                throw new KeyNotFoundException(String.Format("SaveProductLang: ProductId {0} not found in Importrelation",productLang.SimpleProductId));

            Controller.DeleteSimpleProductLang(ownId, productLang.Language);
            productLang.SimpleProductId = ownId;
            Controller.NewSimpleProductLang(productLang);
        }

        public void SaveProductInGroup(int portalId, int productId, int productGroupId, Guid storeGuid)
        {
            int ownProductId = GetImportRelationOwnId(portalId, "PRODUCT", productId, storeGuid);
            if (ownProductId < 0)
                throw new KeyNotFoundException(String.Format("SaveProductInGroup: ProductId {0} not found in Importrelation",productId));
            
            int ownProductGroupId = GetImportRelationOwnId(portalId, "PRODUCTGROUP", productGroupId, storeGuid);
            if (ownProductGroupId < 0)
                throw new KeyNotFoundException(String.Format("SaveProductInGroup: ProductGroupId {0} not found in Importrelation",productGroupId));
            
            Controller.DeleteProductInGroup(ownProductId, ownProductGroupId);
            Controller.NewProductInGroup(ownProductId, ownProductGroupId);
        }

        public void SaveProductGroup(int portalId, ProductGroupInfo productGroup, Guid storeGuid)
        {
            productGroup.PortalId = portalId;
            int ownProductGroupId = GetImportRelationOwnId(portalId, "PRODUCTGROUP", productGroup.ProductGroupId, storeGuid);
            int ownParentId = productGroup.ParentId;
            if (productGroup.ParentId != -1)
            {
                ownParentId = GetImportRelationOwnId(portalId, "PRODUCTGROUP", productGroup.ParentId, storeGuid);
                if (ownParentId < 0)
                    throw new KeyNotFoundException(String.Format("SaveProductGroup: ParentId of ProductGroup {0} could not be found in ImportRelation",productGroup.ParentId));
            }

            if (ownProductGroupId > 0)
            {
                productGroup.ProductGroupId = ownProductGroupId;
                productGroup.ParentId = ownParentId;
                Controller.UpdateProductGroup(productGroup);
            }
            else
            {
                productGroup.ParentId = ownParentId;
                ownProductGroupId = Controller.NewProductGroup(productGroup);
                NewImportRelation(portalId, "PRODUCTGROUP", ownProductGroupId, productGroup.ProductGroupId,storeGuid);
            }
        }

        public void SaveProductGroupLang(int portalId, ProductGroupLangInfo productGroupLang, Guid storeGuid)
        {
            int ownId = GetImportRelationOwnId(portalId, "PRODUCTGROUP", productGroupLang.ProductGroupId, storeGuid);
            if (ownId < 0)
                throw new KeyNotFoundException(String.Format("SaveProductGroupLang: ProductGroupId {0} not found in Importrelation",productGroupLang.ProductGroupId));

            Controller.DeleteProductGroupLang(ownId, productGroupLang.Language);
            productGroupLang.ProductGroupId = ownId;
            Controller.NewProductGroupLang(productGroupLang);
        }

        public void SaveFeatureList(int portalId, FeatureListInfo featureList, Guid storeGuid)
        {
            featureList.PortalID = portalId;
            int ownId = GetImportRelationOwnId(portalId, "FEATURELIST", featureList.FeatureListId, storeGuid);
            if (ownId > -1)
            {
                featureList.FeatureListId = ownId;
                featureList.PortalID = portalId;
                Controller.UpdateFeatureList(featureList);
            }
            else
            {
                featureList.PortalID = portalId;
                ownId = Controller.NewFeatureList(featureList);
                NewImportRelation(portalId, "FEATURELIST", ownId, featureList.FeatureListId, storeGuid);
            }
        }

        public void SaveFeatureListLang(int portalId, FeatureListLangInfo featureListLang, Guid storeGuid)
        {
            int ownId = GetImportRelationOwnId(portalId, "FEATURELIST", featureListLang.FeatureListId, storeGuid);
            if (ownId < -0)
                throw new KeyNotFoundException(String.Format("SaveFeatureListLang: FeatureListId {0} not found in Importrelation",featureListLang.FeatureListId));

            featureListLang.FeatureListId = ownId;
            Controller.DeleteFeatureListLang(ownId, featureListLang.Language);
            Controller.NewFeatureListLang(featureListLang);
        }

        public void SaveFeatureListItem(int portalId, FeatureListItemInfo featureListItem, Guid storeGuid)
        {
            int ownId = GetImportRelationOwnId(portalId, "FEATURELISTITEM", featureListItem.FeatureListItemId, storeGuid);
            int ownFeatureListId = featureListItem.FeatureListId;
            if (featureListItem.FeatureListId != -1)
            {
                ownFeatureListId = GetImportRelationOwnId(portalId, "FEATURELIST", featureListItem.FeatureListId, storeGuid);
                if (ownFeatureListId < 0)
                    throw new KeyNotFoundException(String.Format("SaveFeatureListItem: FeatureListId {0} not found in ImportRelation",featureListItem.FeatureListId));
            }

            if (ownId > 0)
            {
                featureListItem.FeatureListItemId = ownId;
                featureListItem.FeatureListId = ownFeatureListId;
                Controller.UpdateFeatureListItem(featureListItem);
            }
            else
            {
                featureListItem.FeatureListId = ownFeatureListId;
                ownId = Controller.NewFeatureListItem(featureListItem);
                NewImportRelation(portalId, "FEATURELISTITEM", ownId, featureListItem.FeatureListItemId,storeGuid);
            }
        }

        public void SaveFeatureListItemLang(int portalId, FeatureListItemLangInfo featureListItemLang, Guid storeGuid)
        {
            BBStoreController Controller = new BBStoreController();
            int ownId = GetImportRelationOwnId(portalId, "FEATURELISTITEM", featureListItemLang.FeatureListItemId, storeGuid);
            if (ownId < 0)
                throw new KeyNotFoundException(String.Format("SaveFeatureListItemLang: FeatureListItemId {0} not found in Importrelation",featureListItemLang.FeatureListItemId));

            Controller.DeleteFeatureListItemLang(ownId, featureListItemLang.Language);
            featureListItemLang.FeatureListItemId = ownId;
            Controller.NewFeatureListItemLang(featureListItemLang);
        }

        public void SaveFeature(int portalId, FeatureInfo feature, Guid storeGuid)
        {
            feature.PortalID = portalId;
            int ownId = GetImportRelationOwnId(portalId, "FEATURE", feature.FeatureId, storeGuid);
            int ownFeatureGroupId = feature.FeatureGroupId;
            if (feature.FeatureGroupId != -1)
            {
                ownFeatureGroupId = GetImportRelationOwnId(portalId, "FEATUREGROUP", feature.FeatureGroupId, storeGuid);
                if (ownFeatureGroupId < 0)
                    throw new KeyNotFoundException(String.Format("SaveFeature: FeatureGroupId {0} not found in ImportRelation",feature.FeatureGroupId));
            }
            int ownFeatureListId = feature.FeatureListId;
            if (feature.FeatureListId != -1)
            {
                ownFeatureListId = GetImportRelationOwnId(portalId, "FEATURELIST", feature.FeatureListId, storeGuid);
                if (ownFeatureListId < 0)
                    throw new KeyNotFoundException(String.Format("SaveFeature: FeatureListId {0} could not be found in ImportRelation", feature.FeatureListId));
            }

            if (ownId > -1)
            {
                feature.FeatureId = ownId;
                feature.FeatureGroupId = ownFeatureGroupId;
                feature.FeatureListId = ownFeatureListId;
                feature.PortalID = portalId;
                Controller.UpdateFeature(feature);
            }
            else
            {
                feature.FeatureGroupId = ownFeatureGroupId;
                feature.FeatureListId = ownFeatureListId;
                feature.PortalID = portalId;
                ownId = Controller.NewFeature(feature);
                NewImportRelation(portalId, "FEATURE", ownId, feature.FeatureId, storeGuid);
            }
        }

        public void SaveFeatureLang(int portalId, FeatureLangInfo featureLang, Guid storeGuid)
        {
            int ownId = GetImportRelationOwnId(portalId, "FEATURE", featureLang.FeatureId, storeGuid);
            if (ownId < 0)
                throw new KeyNotFoundException(String.Format("SaveFeatureLang: FeatureId {0} not found in Importrelation",featureLang.FeatureId));

            Controller.DeleteFeatureLang(ownId, featureLang.Language);
            featureLang.FeatureId = ownId;
            Controller.NewFeatureLang(featureLang);
        }

        public void SaveFeatureValue(int portalId, FeatureValueInfo featureValue, Guid storeGuid)
        {
            int ownFeatureId = GetImportRelationOwnId(portalId, "FEATURE", featureValue.FeatureId, storeGuid);
            if (ownFeatureId < 0)
                throw new KeyNotFoundException(String.Format("SaveFeatureValue: FeatureId {0} not found in Importrelation",featureValue.FeatureId));

            int ownProductId = GetImportRelationOwnId(portalId, "PRODUCT", featureValue.ProductId, storeGuid);
            if (ownProductId < 0)
                throw new KeyNotFoundException(String.Format("SaveFeatureValue: ProductId {0} not found in Importrelation", featureValue.ProductId));

            if (featureValue.FeatureListItemId != null && featureValue.FeatureListItemId > -1)
            {
                int ownFeatureListItemId = GetImportRelationOwnId(portalId, "FEATURELISTITEM", (int)featureValue.FeatureListItemId, storeGuid);
                if (ownFeatureListItemId < 0)
                    throw new KeyNotFoundException(String.Format("SaveFeatureValue: FeatureListItemId {0} not found in ImportRelation",featureValue.FeatureListItemId));
                featureValue.FeatureListItemId = ownFeatureListItemId;
            }
            else
            {
                featureValue.FeatureListItemId = null;
            }

            int ownId = Controller.GetFeatureValueId(ownProductId, ownFeatureId);
            if (ownId > 0)
            {
                featureValue.FeatureValueId = ownId;
                featureValue.FeatureId = ownFeatureId;
                featureValue.ProductId = ownProductId;
                Controller.UpdateFeatureValue(featureValue);
            }
            else
            {
                featureValue.FeatureId = ownFeatureId;
                featureValue.ProductId = ownProductId;
                ownId = Controller.NewFeatureValue(featureValue);
                NewImportRelation(portalId, "FEATUREVALUE", ownId, featureValue.FeatureValueId, storeGuid);
            }
        }

        public void SaveProductGroupFeature(int portalId, int featureId, int productGroupId, Guid storeGuid)
        {
            int ownFeatureId = GetImportRelationOwnId(portalId, "FEATURE", featureId, storeGuid);
            if (ownFeatureId < 0)
                throw new KeyNotFoundException(String.Format("SaveProductGroupFeature: FeatureId {0} not found in Importrelation",featureId));
            
            int ownProductGroupId = GetImportRelationOwnId(portalId, "PRODUCTGROUP", productGroupId, storeGuid);
            if (ownProductGroupId < 0)
                throw new KeyNotFoundException(String.Format("SaveProductGroupFeature: ProductGroupId {0} not found in Importrelation",productGroupId));

            Controller.DeleteProductGroupFeature(ownFeatureId, ownProductGroupId);
            Controller.NewProductGroupFeature(ownFeatureId, ownProductGroupId);
        }

        private void SaveUnit(int portalId, UnitInfo unit, Guid storeGuid)
        {
            unit.PortalId = portalId;
            int ownId = GetImportRelationOwnId(portalId, "UNIT", unit.UnitId, storeGuid);
            if (ownId > -1)
            {
                unit.UnitId = ownId;
                Controller.UpdateUnit(unit);
            }
            else
            {
                ownId = Controller.NewUnit(unit);
                NewImportRelation(portalId, "UNIT", ownId, unit.UnitId, storeGuid);
            }
        }

        private void SaveUnitLang(int portalId, UnitLangInfo unitLang, Guid storeGuid)
        {
            int ownId = GetImportRelationOwnId(portalId, "UNIT", unitLang.UnitId, storeGuid);
            if (ownId < 0)
                throw new KeyNotFoundException(String.Format("SaveUnitLang: UnitId {0} not found in Importrelation",unitLang.UnitId));

            Controller.DeleteUnitLang(ownId, unitLang.Language);
            unitLang.UnitId = ownId;
            Controller.NewUnitLang(unitLang);
        }

        #endregion

        #region ImportRelation Helper methods

        private int GetImportRelationOwnId(int PortalId, string Tablename, int ForeignId, Guid storeGuid)
        {
            return DataProvider.Instance().GetImportRelationOwnId(PortalId, Tablename, ForeignId, storeGuid);
        }

        private int GetImportRelationForeignId(int PortalId, string Tablename, int OwnId, Guid storeGuid)
        {
            return DataProvider.Instance().GetImportRelationForeignId(PortalId, Tablename, OwnId, storeGuid);
        }
        
        private List<int> GetImportRelationOwnIdsByTable(int portalId, string tableName, Guid storeGuid)
        {
            List<int> retVal = new List<int>();
            IDataReader dr = DataProvider.Instance().GetImportRelationOwnIdsByTable(portalId, tableName, storeGuid);
            while (dr.Read())
            {
                retVal.Add((int)dr[0]);
            }
            return retVal;
        }

        private List<int> GetImportRelationForeignIdsByTable(int portalId, string tableName, Guid storeGuid)
        {
            List<int> retVal = new List<int>();
            IDataReader dr = DataProvider.Instance().GetImportRelationForeignIdsByTable(portalId, tableName, storeGuid);
            while (dr.Read())
            {
                retVal.Add((int)dr[0]);
            }
            return retVal;
        }

        private void NewImportRelation(int PortalId, string Tablename, int OwnId, int ForeignId, Guid storeGuid)
        {
            DataProvider.Instance().NewImportRelation(PortalId, Tablename, OwnId, ForeignId, storeGuid);
        }
        
        private void DeleteImportRelationByOwnId(int PortalId, string Tablename, int OwnId)
        {
            DataProvider.Instance().DeleteImportRelationByOwnId(PortalId, Tablename, OwnId);
        }
        
        private void DeleteImportRelationByForeignId(int PortalId, string Tablename, int ForeignId ,Guid storeGuid)
        {
            DataProvider.Instance().DeleteImportRelationByForeignId(PortalId, Tablename, ForeignId, storeGuid);
        }
        
        private void DeleteImportRelationByTable(int PortalId, string Tablename, Guid storeGuid)
        {
            DataProvider.Instance().DeleteImportRelationByTable(PortalId, Tablename, storeGuid);
        }
        
        private void DeleteImportRelationByPortal(int PortalId)
        {
            DataProvider.Instance().DeleteImportRelationByPortal(PortalId);
        }

        private void DeleteImportRelationByStore(int PortalId, Guid storeId)
        {
            DataProvider.Instance().DeleteImportRelationByStore(PortalId, storeId);
        }

        
        #endregion

    }
}