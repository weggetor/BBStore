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
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Services.Mail;
using Newtonsoft.Json;
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

        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(BBStoreImportController));

        #endregion

        #region Maintenance  methods
        public BBStoreInfo ExportStore(int portalId, Guid storeGuid)
        {
            // TODO: Der Export auf die eigene StoreID exportiert zur Zeit noch ALLES, 
            // nicht nur die Daten die im Store selber angelegt wurden. 

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
            bbStore.Order = GetOrders(portalId, storeGuid);
            bbStore.OrderProduct = GetOrderProducts(portalId, storeGuid);
            bbStore.OrderProductOption = GetOrderProductOptions(portalId, storeGuid);
            bbStore.OrderAdditionalCost = GetOrderAdditionalCosts(portalId, storeGuid);
            bbStore.OrderAddress = GetOrderAddresses(portalId, storeGuid);
            bbStore.Customer = GetCustomers(portalId, storeGuid);
            bbStore.SubscriberAddressType = GetSubscriberAddressTypes(portalId, storeGuid);
            bbStore.SubscriberAddressTypeLang = GetSubscriberAddressTypeLangs(portalId, storeGuid);

            return bbStore;
        }

        public void ImportStore(int portalId, BBStoreInfo bbStore)
        {
            Guid storeGuid = bbStore.StoreGuid;

            SaveImportStore(storeGuid, bbStore.StoreName);

            foreach (UnitInfo unit in bbStore.Unit)
            {
                if (unit._status == 3)
                    DeleteUnit(portalId,unit.UnitId,storeGuid);
                else
                    SaveUnit(portalId, unit, storeGuid);
            }
            foreach (UnitLangInfo unitLang in bbStore.UnitLang)
            {
                SaveUnitLang(portalId, unitLang, storeGuid);
            }
            foreach (ProductGroupInfo productGroup in bbStore.ProductGroup)
            {
                if (productGroup._status == 3)
                    DeleteProductGroup(portalId,productGroup.ProductGroupId, storeGuid);
                else
                    SaveProductGroup(portalId, productGroup, storeGuid);
            }
            foreach (ProductGroupLangInfo productGroupLang in bbStore.ProductGroupLang)
            {
                SaveProductGroupLang(portalId, productGroupLang, storeGuid);
            }
            foreach (SimpleProductInfo simpleProduct in bbStore.Product)
            {
                if(simpleProduct._status == 3)
                    DeleteProduct(portalId,simpleProduct.SimpleProductId, storeGuid);
                else
                    SaveProduct(portalId, simpleProduct, storeGuid);
            }
            foreach (SimpleProductLangInfo simpleProductLang in bbStore.ProductLang)
            {
                SaveProductLang(portalId, simpleProductLang, storeGuid);
            }
            foreach (ProductInGroupInfo productInGroup in bbStore.ProductInGroup)
            {
                if (productInGroup._status == 3)
                    DeleteProductInGroup(portalId, productInGroup.SimpleProductId, productInGroup.ProductGroupId, storeGuid);
                else
                    SaveProductInGroup(portalId, productInGroup.SimpleProductId, productInGroup.ProductGroupId, storeGuid);
            }
            foreach (FeatureGroupInfo featureGroup in bbStore.FeatureGroup)
            {
                if (featureGroup._status == 3)
                    DeleteFeatureGroup(portalId,featureGroup.FeatureGroupId, storeGuid);
                else
                    SaveFeatureGroup(portalId, featureGroup, storeGuid);
            }
            foreach (FeatureGroupLangInfo featureGroupLang in bbStore.FeatureGroupLang)
            {
                SaveFeatureGroupLang(portalId, featureGroupLang, storeGuid);
            }
            foreach (FeatureListInfo featureList in bbStore.FeatureList)
            {
                if (featureList._status == 3)
                    DeleteFeatureList(portalId,featureList.FeatureListId, storeGuid);
                else
                    SaveFeatureList(portalId, featureList, storeGuid);
            }
            foreach (FeatureListLangInfo featureListLang in bbStore.FeatureListLang)
            {
                SaveFeatureListLang(portalId, featureListLang, storeGuid);
            }
            foreach (FeatureListItemInfo featureListItem in bbStore.FeatureListItem)
            {
                if (featureListItem._status == 3)
                    DeleteFeatureListItem(portalId, featureListItem.FeatureListItemId, storeGuid);
                else
                    SaveFeatureListItem(portalId, featureListItem, storeGuid);
            }
            foreach (FeatureListItemLangInfo featureListItemLang in bbStore.FeatureListItemLang)
            {
                SaveFeatureListItemLang(portalId, featureListItemLang, storeGuid);
            }
            foreach (FeatureInfo feature in bbStore.Feature)
            {
                if (feature._status == 3)
                    DeleteFeature(portalId,feature.FeatureId, storeGuid);
                else
                    SaveFeature(portalId, feature, storeGuid);
            }

            foreach (FeatureLangInfo featureLang in bbStore.FeatureLang)
            {
                SaveFeatureLang(portalId, featureLang, storeGuid);
            }

            //TODO: Nicht alle löschen und dann wieder neu anlegen. Funktioniert bei CATO nicht mehr
            if (bbStore.ProductGroupFeature.Count > 0)
            {
                Controller.DeleteProductGroupFeaturesByPortal(portalId);
                foreach (ProductGroupFeatureInfo productGroupFeature in bbStore.ProductGroupFeature)
                {
                    if (productGroupFeature._status != 3)
                        SaveProductGroupFeature(portalId, productGroupFeature.FeatureId, productGroupFeature.ProductGroupId, storeGuid);
                }
            }

            //TODO: Nicht alle löschen und dann wieder neu anlegen. Funktioniert bei CATO nicht mehr
            if (bbStore.FeatureValue.Count > 0)
            {
                Controller.DeleteFeatureValuesByPortal(portalId);
                foreach (FeatureValueInfo featureValue in bbStore.FeatureValue)
                {
                    if (featureValue._status != 3)
                        SaveFeatureValue(portalId, featureValue, storeGuid);
                }
            }

            //TODO: Nicht alle löschen und dann wieder neu anlegen. Funktioniert bei CATO nicht mehr
            if (bbStore.ProductGroupListItem.Count > 0)
            {
                DeleteProductGroupListItems(portalId, storeGuid);
                foreach (ProductGroupListItemInfo productGroupListItem in bbStore.ProductGroupListItem)
                {
                    SaveProductGroupListItem(portalId, productGroupListItem.ProductGroupId, productGroupListItem.FeatureListItemId, storeGuid);
                }
            }

            foreach (CustomerInfo customer in bbStore.Customer)
            {
                if (customer._status == 3)
                    DeleteCustomer(portalId, customer.CustomerId, storeGuid);
                else
                    SaveCustomer(portalId, customer, storeGuid);
            }

            foreach (OrderInfo order in bbStore.Order)
            {
                if (order._status == 3)
                    DeleteOrder(portalId, order.OrderID, storeGuid);
                else
                    SaveOrder(portalId, order, storeGuid);
            }

            foreach (OrderProductInfo orderProduct in bbStore.OrderProduct)
            {
                if (orderProduct._status == 3)
                    DeleteOrderProduct(portalId, orderProduct.OrderProductId, storeGuid);
                else
                    SaveOrderProduct(portalId, orderProduct, storeGuid);
            }
            foreach (OrderProductOptionInfo orderProductOption in bbStore.OrderProductOption)
            {
                if (orderProductOption._status == 3)
                    DeleteOrderProductOption(portalId, orderProductOption.OrderProductOptionId, storeGuid);
                else
                    SaveOrderProductOption(portalId, orderProductOption, storeGuid);
            }
            foreach (OrderAdditionalCostInfo orderAdditionalCost in bbStore.OrderAdditionalCost)
            {
                if(orderAdditionalCost._status ==3)
                    DeleteOrderAdditionalCost(portalId, orderAdditionalCost.OrderAdditionalCostId, storeGuid);
                else
                    SaveOrderAdditionalCost(portalId, orderAdditionalCost, storeGuid);
            }
            foreach (SubscriberAddressTypeInfo subscriberAddressType in bbStore.SubscriberAddressType)
            {
                if(subscriberAddressType._status == 3)
                    DeleteSubscriberAddressType(portalId,subscriberAddressType.SubscriberAddressTypeId, storeGuid);
                else
                    SaveSubscriberAddressType(portalId, subscriberAddressType, storeGuid);
            }
            foreach (SubscriberAddressTypeLangInfo subscriberAddressTypeLang in bbStore.SubscriberAddressTypeLang)
            {
                SaveSubscriberAddressTypeLang(portalId, subscriberAddressTypeLang, storeGuid);
            }
            foreach (OrderAddressInfo orderAddress in bbStore.OrderAddress)
            {
                if (orderAddress._status == 3)
                    DeleteOrderAddress(portalId, orderAddress.OrderAddressId, storeGuid);
                else
                    SaveOrderAddress(portalId, orderAddress, storeGuid);
            }

            DeleteEmptyImageDirectories(portalId);
            EnsureImportRelationIntegrity(portalId, storeGuid);
        }

        public BBStoreInfo GetAppOrders(int portalId, int status, Guid storeGuid)
        {
            Logger.Debug("Starte GetAppOrders");
            try
            {
                BBStoreController ctrl = new BBStoreController();

                BBStoreInfo bbStore = new BBStoreInfo();

                bbStore.StoreGuid = storeGuid;
                bbStore.StoreName = (storeGuid == BBStoreController.StoreGuid ? BBStoreController.StoreName : GetImportStoreName(storeGuid));
                Logger.Debug("Starte GetChangedOrders...");
                bbStore.Order = GetOrdersByStatus(portalId, status, storeGuid);
                Logger.DebugFormat("Anzahl Gelesene Order: {}", bbStore.Order.Count);
                return bbStore;
            }
            catch (Exception ex)
            {
                string message = ex.ToString();
                var eventlogCtrl = new EventLogController();
                eventlogCtrl.AddLog("BBStoreImportController.GetAppOrders", message,
                    PortalSettings.Current, PortalSettings.Current.UserId, EventLogController.EventLogType.ADMIN_ALERT);
                throw;
            }
        }

        public BBStoreInfo GetOrderDetails(int portalId, int orderId, Guid storeGuid)
        {
            Logger.Debug("Starte GetOrderDetails");
            try
            {
                BBStoreController ctrl = new BBStoreController();

                BBStoreInfo bbStore = new BBStoreInfo();

                bbStore.StoreGuid = storeGuid;
                bbStore.StoreName = (storeGuid == BBStoreController.StoreGuid ? BBStoreController.StoreName : GetImportStoreName(storeGuid));
                Logger.Debug("Starte GetOrder...");
                OrderInfo order = GetOrder(portalId, orderId, storeGuid);
                
                List<OrderProductInfo> orderproducts = GetForeignOrderProductsByOrderId(portalId, order.OrderID, storeGuid);
                Logger.DebugFormat("Gelesene OrderProducts für Order.OrderID {0}: Anzahl: {1}", order.OrderID, orderproducts.Count);
                bbStore.OrderProduct.AddRange(orderproducts);

                foreach (var orderProduct in bbStore.OrderProduct)
                {
                    List<OrderProductOptionInfo> orderproductOptions = GetForeignOrderProductOptionsByOrderProductId(portalId, orderProduct.OrderProductId, storeGuid);
                    Logger.DebugFormat("Gelesene OrderProductOptions für OrderProduct.OrderProductID {0}: Anzahl: {1}", orderProduct.OrderProductId, orderproductOptions.Count);
                    bbStore.OrderProductOption.AddRange(orderproductOptions);
                }

                List<OrderAdditionalCostInfo> orderAdditionalCosts = GetForeignOrderAdditionalCostsByOrderId(portalId, order.OrderID, storeGuid);
                Logger.DebugFormat("Gelesene OrderAdditionalCost für Order.OrderID {0}: Anzahl: {1}", order.OrderID, orderAdditionalCosts.Count);
                bbStore.OrderAdditionalCost.AddRange(orderAdditionalCosts);

                List<OrderAddressInfo> orderAddresses = GetForeignOrderAddressesByOrderId(portalId, order.OrderID, storeGuid);
                Logger.DebugFormat("Gelesene OrderAddresses für Order.OrderID {0}: Anzahl: {1}", order.OrderID, orderAddresses.Count);
                bbStore.OrderAddress.AddRange(orderAddresses);

                Logger.DebugFormat("Länge des Ergbnis-Strings: {0}", Newtonsoft.Json.JsonConvert.SerializeObject(bbStore).Length);
                return bbStore;
            }
            catch (Exception ex)
            {
                string message = ex.ToString();
                var eventlogCtrl = new EventLogController();
                eventlogCtrl.AddLog("BBStoreImportController.GetAppOrders", message,
                    PortalSettings.Current, PortalSettings.Current.UserId, EventLogController.EventLogType.ADMIN_ALERT);
                throw;
            }
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
            DeleteOrderAdditionalCosts(portalId, storeGuid);
            DeleteOrderProductOptions(portalId, storeGuid);
            DeleteOrderProducts(portalId, storeGuid);
            DeleteOrders(portalId, storeGuid);
            DeleteCustomers(portalId, storeGuid);
            DeleteOrderAddresses(portalId, storeGuid);
            DeleteSubscriberAddressTypes(portalId, storeGuid);
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

        public void DeleteFeatureGroup(int portalId, int featureGroupId, Guid storeGuid)
        {
            int ownId = GetImportRelationOwnId(portalId, "FEATUREGROUP", featureGroupId, storeGuid);
            Controller.DeleteFeatureGroup(ownId);
            DeleteImportRelationByOwnId(portalId, "FEATUREGROUP", ownId);
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

        public void DeleteProductGroup(int portalId, int productGroupId, Guid storeGuid)
        {
            int ownId = GetImportRelationOwnId(portalId, "PRODUCTGROUP", productGroupId, storeGuid);
            Controller.DeleteProductGroup(ownId);
            DeleteImportRelationByOwnId(portalId, "PRODUCTGROUP", ownId);
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

        public void DeleteFeatureList(int portalId, int featureListId, Guid storeGuid)
        {
            int ownId = GetImportRelationOwnId(portalId, "FEATURELIST", featureListId, storeGuid);
            Controller.DeleteFeatureList(ownId);
            DeleteImportRelationByOwnId(portalId, "FEATURELIST", ownId);
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

        public void DeleteFeature(int portalId, int featureId, Guid storeGuid)
        {
            int ownId = GetImportRelationOwnId(portalId, "FEATURE", featureId, storeGuid);
            Controller.DeleteFeature(ownId);
            DeleteImportRelationByOwnId(portalId, "FEATURE", ownId);
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

        public void DeleteFeatureListItem(int portalId, int featureListItemId, Guid storeGuid)
        {
            int ownId = GetImportRelationOwnId(portalId, "FEATURELISTITEM", featureListItemId, storeGuid);
            Controller.DeleteFeatureListItem(ownId);
            DeleteImportRelationByOwnId(portalId, "FEATURELISTITEM", ownId);
        }

        public void DeleteProductGroupListItem(int portalId, int productGroupId, int featureListItemId, Guid storeGuid)
        {
            int ownProductGroupId = GetImportRelationOwnId(portalId, "PRODUCTGROUP", productGroupId, storeGuid);
            int ownFeatureListItemId = GetImportRelationOwnId(portalId, "FEATURELISTITEM", featureListItemId, storeGuid);
            Controller.DeleteProductGroupListItem(ownProductGroupId,ownFeatureListItemId);
        }
        public void DeleteProductGroupListItems(int portalId, Guid storeGuid)
        {
            List<int> ownIds = GetImportRelationOwnIdsByTable(portalId, "PRODUCTGROUP", storeGuid);
            foreach (int ownId in ownIds)
            {
                Controller.DeleteProductGroupListItemsByProductGroup(ownId);
            }
        }

        public void DeleteProductInGroup(int portalId, int productId, int productGroupId, Guid storeGuid)
        {
            int ownProductId = GetImportRelationOwnId(portalId, "PRODUCT", productId, storeGuid);
            int ownProductGroupId = GetImportRelationOwnId(portalId, "PRODUCTGROUP", productGroupId, storeGuid);
            Controller.DeleteProductInGroup(ownProductId, ownProductGroupId);
        }

        public void DeleteCustomerAddress(int portalId, int customerAddressId)
        {
            Controller.DeleteCustomerAddress(customerAddressId);
            DeleteImportRelationByOwnId(portalId, "CUSTOMERADDRESS", customerAddressId);
        }

        public void DeleteUnit(int portalId, int unitId, Guid storeGuid)
        {
            int ownId = GetImportRelationOwnId(portalId, "UNIT", unitId, storeGuid);
            Controller.DeleteUnit(ownId);
            DeleteImportRelationByOwnId(portalId, "UNIT", ownId);
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
            try
            {
                int ownId = GetImportRelationOwnId(portalId, "PRODUCT", productId, storeGuid);
                if (ownId < 0)
                    throw new KeyNotFoundException(String.Format("DeleteProductImages: ProductId {0} not found in ImportRelation", productId));


                // Retrieve product
                SimpleProductInfo product = Controller.GetSimpleProductByProductId(portalId, ownId);

                if (!String.IsNullOrEmpty(product.Image))
                {
                    PortalController pc = new PortalController();
                    PortalInfo pi = pc.GetPortal(portalId);
                    FileInfo fi = new FileInfo(pi.HomeDirectoryMapPath + product.Image.Replace("/", "\\"));
                    if (String.IsNullOrEmpty(fi.Name))
                        return;

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
            catch (Exception)
            {
                throw;
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

        public void DeleteOrderAdditionalCost(int portalId, int orderAdditionalCostId, Guid storeGuid)
        {
            int ownId = GetImportRelationOwnId(portalId, "ORDERADDITIONALCOST", orderAdditionalCostId, storeGuid);
            Controller.DeleteOrderAdditionalCost(ownId);
            DeleteImportRelationByOwnId(portalId, "ORDERADDITIONALCOST", ownId);
        }
        public void DeleteOrderAdditionalCosts(int portalId, Guid storeGuid)
        {
            List<int> ownIds = GetImportRelationOwnIdsByTable(portalId, "ORDERADDITIONALCOST", storeGuid);
            foreach (int ownId in ownIds)
            {
                Controller.DeleteOrderAdditionalCost(ownId);
            }
            DeleteImportRelationByTable(portalId, "ORDERADDITIONALCOST", storeGuid);
        }

        public void DeleteOrderProductOption(int portalId, int orderProductOptionId, Guid storeGuid)
        {
            int ownId = GetImportRelationOwnId(portalId, "ORDERPRODUCTOPTION", orderProductOptionId, storeGuid);
            Controller.DeleteOrderProductOption(ownId);
            DeleteImportRelationByOwnId(portalId, "ORDERPRODUCTOPTION", ownId);
        }
        public void DeleteOrderProductOptions(int portalId, Guid storeGuid)
        {
            List<int> ownIds = GetImportRelationOwnIdsByTable(portalId, "ORDERPRODUCTOPTION", storeGuid);
            foreach (int ownId in ownIds)
            {
                Controller.DeleteOrderProductOption(ownId);
            }
            DeleteImportRelationByTable(portalId, "ORDERPRODUCTOPTION", storeGuid);
        }

        public void DeleteOrderProduct(int portalId, int orderProductId, Guid storeGuid)
        {
            int ownId = GetImportRelationOwnId(portalId, "ORDERPRODUCT", orderProductId, storeGuid);
            Controller.DeleteOrderProduct(ownId);
            DeleteImportRelationByOwnId(portalId, "ORDERPRODUCT", ownId);
        }
        public void DeleteOrderProducts(int portalId, Guid storeGuid)
        {
            List<int> ownIds = GetImportRelationOwnIdsByTable(portalId, "ORDERPRODUCT", storeGuid);
            foreach (int ownId in ownIds)
            {
                Controller.DeleteOrderProduct(ownId);
            }
            DeleteImportRelationByTable(portalId, "ORDERPRODUCT", storeGuid);
        }

        public void DeleteOrder(int portalId, int orderId, Guid storeGuid)
        {
            int ownId = GetImportRelationOwnId(portalId, "ORDER", orderId, storeGuid);
            Controller.DeleteOrder(ownId);
            DeleteImportRelationByOwnId(portalId, "ORDER", ownId);
        }
        public void DeleteOrders(int portalId, Guid storeGuid)
        {
            List<int> ownIds = GetImportRelationOwnIdsByTable(portalId, "ORDER", storeGuid);
            foreach (int ownId in ownIds)
            {
                Controller.DeleteOrder(ownId);
            }
            DeleteImportRelationByTable(portalId, "ORDER", storeGuid);
        }

        public void DeleteCustomer(int portalId, int customerId, Guid storeGuid)
        {
            int ownId = GetImportRelationOwnId(portalId, "CUSTOMER", customerId, storeGuid);
            Controller.DeleteCustomer(ownId);
            DeleteImportRelationByOwnId(portalId, "CUSTOMER", ownId);
        }
        public void DeleteCustomers(int portalId, Guid storeGuid)
        {
            List<int> ownIds = GetImportRelationOwnIdsByTable(portalId, "CUSTOMER", storeGuid);
            foreach (int ownId in ownIds)
            {
                Controller.DeleteCustomer(ownId);
            }
            DeleteImportRelationByTable(portalId, "CUSTOMER", storeGuid);
        }

        public void DeleteOrderAddress(int portalId, int orderAddressId, Guid storeGuid)
        {
            int ownId = GetImportRelationOwnId(portalId, "ORDERADDRESS", orderAddressId, storeGuid);
            Controller.DeleteOrderAddress(ownId);
            DeleteImportRelationByOwnId(portalId, "ORDERADDRESS", ownId);
        }
        public void DeleteOrderAddresses(int portalId, Guid storeGuid)
        {
            List<int> ownIds = GetImportRelationOwnIdsByTable(portalId, "ORDERADDRESS", storeGuid);
            foreach (int ownId in ownIds)
            {
                Controller.DeleteOrderAddress(ownId);
            }
            DeleteImportRelationByTable(portalId, "ORDERADDRESS", storeGuid);
        }

        public void DeleteSubscriberAddressType(int portalId, int subscriberAddressTypeId, Guid storeGuid)
        {
            int ownId = GetImportRelationOwnId(portalId, "SUBSCRIBERADDRESSTYP", subscriberAddressTypeId, storeGuid);
            Controller.DeleteSubscriberAddressType(ownId);
            DeleteImportRelationByOwnId(portalId, "SUBSCRIBERADDRESSTYP", ownId);
        }
        public void DeleteSubscriberAddressTypes(int portalId, Guid storeGuid)
        {
            List<int> ownIds = GetImportRelationOwnIdsByTable(portalId, "SUBSCRIBERADDRESSTYP", storeGuid);
            foreach (int ownId in ownIds)
            {
                Controller.DeleteSubscriberAddressType(ownId);
            }
            DeleteImportRelationByTable(portalId, "SUBSCRIBERADDRESSTYP", storeGuid);
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
            List<FeatureListLangInfo> featureListsLangs = Controller.GetFeatureListLangsByPortal(portalId);
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
            List<FeatureListItemLangInfo> featureListItemsLangs = Controller.GetFeatureListItemLangsByPortal(portalId);
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

        private List<OrderInfo> GetOrders(int portalId, Guid storeGuid)
        {
            List<OrderInfo> orders = Controller.GetOrdersByPortal(portalId);
            if (storeGuid == BBStoreController.StoreGuid)
                return orders;

            List<OrderInfo> result = new List<OrderInfo>();
            foreach (OrderInfo order in orders)
            {
                int orderId = GetImportRelationForeignId(portalId, "ORDER", order.OrderID, storeGuid);
                int customerId = GetImportRelationForeignId(portalId, "CUSTOMER", order.CustomerID, storeGuid);
                int subscriberID = GetImportRelationForeignId(portalId, "USER", order.SubscriberID, storeGuid);
                if (orderId > -1)
                {
                    order.OrderID = orderId;
                    order.CustomerID = customerId;
                    order.SubscriberID = subscriberID;
                    result.Add(order);
                }
            }
            return result;
        }

        private OrderInfo GetOrder(int portalId, int foreignOrderId, Guid storeGuid)
        {
            int ownOrderId = GetImportRelationOwnId(portalId, "ORDER", foreignOrderId, storeGuid);
            if (ownOrderId > -1) { 
                OrderInfo order = Controller.GetOrder(ownOrderId);
                int customerId = GetImportRelationForeignId(portalId, "CUSTOMER", order.CustomerID, storeGuid);
                order.OrderID = foreignOrderId;
                order.CustomerID = customerId;
                order.SubscriberID = GetImportRelationForeignId(portalId, "USER", order.SubscriberID, storeGuid);
                return order;
            }
            return null;
        }


        private List<OrderInfo> GetOrdersByStatus(int portalId, int status, Guid storeGuid)
        {
            try
            {
                List<OrderInfo> orders = Controller.GetOrdersByPortal(portalId);
                if (storeGuid == BBStoreController.StoreGuid)
                    return orders;

                List<OrderInfo> result = new List<OrderInfo>();
                foreach (OrderInfo order in orders.Where(o => o.OrderStateId == status))
                {
                    int orderId = GetImportRelationForeignId(portalId, "ORDER", order.OrderID, storeGuid);
                    if (orderId > -1)
                    {
                        order.OrderID = orderId;
                        order.CustomerID = GetImportRelationForeignId(portalId, "CUSTOMER", order.CustomerID, storeGuid);
                        order.SubscriberID = GetImportRelationForeignId(portalId, "USER", order.SubscriberID, storeGuid);
                        result.Add(order);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                string message = ex.ToString();
                var eventlogCtrl = new EventLogController();
                eventlogCtrl.AddLog("BBStoreImportController.GetChangedOrders", message,
                    PortalSettings.Current, PortalSettings.Current.UserId, EventLogController.EventLogType.ADMIN_ALERT);
                throw;
            }
        }

        private List<OrderProductInfo> GetOrderProducts(int portalId, Guid storeGuid)
        {
            try
            {
                List<OrderProductInfo> orderProducts = Controller.GetOrderProductsByPortal(portalId);
                if (storeGuid == BBStoreController.StoreGuid)
                    return orderProducts;

                List<OrderProductInfo> result = new List<OrderProductInfo>();
                foreach (OrderProductInfo orderProduct in orderProducts)
                {
                    int orderProductId = GetImportRelationForeignId(portalId, "ORDERPRODUCT", orderProduct.OrderProductId, storeGuid);
                    if (orderProductId == -1)
                        orderProductId = (-1) * orderProduct.OrderProductId;
                    int productId = GetImportRelationForeignId(portalId, "PRODUCT", orderProduct.ProductId, storeGuid);
                    int orderId = GetImportRelationForeignId(portalId, "ORDER", orderProduct.OrderId, storeGuid);
                    if (orderId > -1 && productId > -1)
                    {
                        orderProduct.OrderId = orderId;
                        orderProduct.ProductId = productId;
                        orderProduct.OrderProductId = orderProductId;
                        result.Add(orderProduct);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                string message = ex.ToString();
                var eventlogCtrl = new EventLogController();
                eventlogCtrl.AddLog("BBStoreImportController.GetOrderProducts", message,
                    PortalSettings.Current, PortalSettings.Current.UserId, EventLogController.EventLogType.ADMIN_ALERT);
                throw;
            }
        }

        private List<OrderProductInfo> GetForeignOrderProductsByOrderId(int portalId, int foreignOrderId, Guid storeGuid)
        {
            try
            {
                int ownOrderId = GetImportRelationOwnId(portalId, "ORDER", foreignOrderId, storeGuid);
                List<OrderProductInfo> orderProducts = Controller.GetOrderProducts(ownOrderId);
                if (storeGuid == BBStoreController.StoreGuid)
                    return orderProducts;

                List<OrderProductInfo> result = new List<OrderProductInfo>();
                foreach (OrderProductInfo orderProduct in orderProducts)
                {
                    int orderProductId = GetImportRelationForeignId(portalId, "ORDERPRODUCT", orderProduct.OrderProductId, storeGuid);
                    if (orderProductId == -1)
                        orderProductId = (-1) * orderProduct.OrderProductId;
                    int productId = GetImportRelationForeignId(portalId, "PRODUCT", orderProduct.ProductId, storeGuid);
                    int orderId = foreignOrderId;
                    if (orderId > -1 && productId > -1)
                    {
                        orderProduct.OrderId = orderId;
                        orderProduct.ProductId = productId;
                        orderProduct.OrderProductId = orderProductId;
                        result.Add(orderProduct);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                string message = ex.ToString();
                var eventlogCtrl = new EventLogController();
                eventlogCtrl.AddLog("BBStoreImportController.GetOrderProducts", message,
                    PortalSettings.Current, PortalSettings.Current.UserId, EventLogController.EventLogType.ADMIN_ALERT);
                throw;
            }
        }

        private List<OrderProductOptionInfo> GetOrderProductOptions(int portalId, Guid storeGuid)
        {
            try
            {
                List<OrderProductOptionInfo> orderProductOptions = Controller.GetOrderProductOptionsByPortal(portalId);
                if (storeGuid == BBStoreController.StoreGuid)
                    return orderProductOptions;

                List<OrderProductOptionInfo> result = new List<OrderProductOptionInfo>();
                foreach (OrderProductOptionInfo orderProductOption in orderProductOptions)
                {
                    int orderProductOptionId = GetImportRelationForeignId(portalId, "ORDERPRODUCTOPTION", orderProductOption.OrderProductOptionId, storeGuid);
                    int orderProductId = GetImportRelationForeignId(portalId, "ORDERPRODUCT", orderProductOption.OrderProductId, storeGuid);
                    if (orderProductId == -1)
                        orderProductId = (-1) * orderProductOption.OrderProductId;

                    orderProductOption._status = (orderProductOptionId == -1 ? 1 : 2);
                    orderProductOption.OrderProductOptionId = orderProductOptionId;
                    orderProductOption.OrderProductId = orderProductId;
                    result.Add(orderProductOption);
                }
                return result;
            }
            catch (Exception ex)
            {
                string message = ex.ToString();
                var eventlogCtrl = new EventLogController();
                eventlogCtrl.AddLog("BBStoreImportController.GetOrderProductOptions", message,
                    PortalSettings.Current, PortalSettings.Current.UserId, EventLogController.EventLogType.ADMIN_ALERT);
                throw;
            }
        }

        private List<OrderProductOptionInfo> GetForeignOrderProductOptionsByOrderProductId(int portalId, int foreignOrderProductId, Guid storeGuid)
        {
            try
            {
                int ownOrderProductId = GetImportRelationOwnId(portalId, "ORDERPRODUCT", foreignOrderProductId, storeGuid);
                List<OrderProductOptionInfo> orderProductOptions;
                int orderProductId;
                if (ownOrderProductId == -1)
                {
                    orderProductOptions = Controller.GetOrderProductOptions((-1) * foreignOrderProductId);
                    orderProductId = foreignOrderProductId;
                }
                else
                {
                    orderProductOptions = Controller.GetOrderProductOptions(ownOrderProductId);
                    orderProductId = foreignOrderProductId;
                }

                if (storeGuid == BBStoreController.StoreGuid)
                    return orderProductOptions;

                List<OrderProductOptionInfo> result = new List<OrderProductOptionInfo>();
                foreach (OrderProductOptionInfo orderProductOption in orderProductOptions)
                {
                    int orderProductOptionId = GetImportRelationForeignId(portalId, "ORDERPRODUCTOPTION", orderProductOption.OrderProductOptionId, storeGuid);
                    orderProductOption._status = (orderProductOptionId == -1 ? 1 : 2);
                    orderProductOption.OrderProductOptionId = orderProductOptionId;
                    orderProductOption.OrderProductId = orderProductId;
                    result.Add(orderProductOption);
                }
                return result;
            }
            catch (Exception ex)
            {
                string message = ex.ToString();
                var eventlogCtrl = new EventLogController();
                eventlogCtrl.AddLog("BBStoreImportController.GetForeignOrderProductOptionsByOrderProductId", message,
                    PortalSettings.Current, PortalSettings.Current.UserId, EventLogController.EventLogType.ADMIN_ALERT);
                throw;
            }
        }

        private List<OrderAdditionalCostInfo> GetOrderAdditionalCosts(int portalId, Guid storeGuid)
        {
            try
            {
                List<OrderAdditionalCostInfo> orderadditionalCosts = Controller.GetOrderAdditionalCostsByPortal(portalId);
                if (storeGuid == BBStoreController.StoreGuid)
                    return orderadditionalCosts;

                List<OrderAdditionalCostInfo> result = new List<OrderAdditionalCostInfo>();
                foreach (OrderAdditionalCostInfo orderadditionalCost in orderadditionalCosts)
                {
                    int orderadditionalCostId = GetImportRelationForeignId(portalId, "ORDERADDITIONALCOST", orderadditionalCost.OrderAdditionalCostId, storeGuid);
                    int orderId = GetImportRelationForeignId(portalId, "ORDER", orderadditionalCost.OrderId, storeGuid);
                    if (orderId > -1 && orderadditionalCostId > -1)
                    {
                        orderadditionalCost.OrderId = orderId;
                        orderadditionalCost.OrderAdditionalCostId = orderadditionalCostId;
                        result.Add(orderadditionalCost);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                string message = ex.ToString();
                var eventlogCtrl = new EventLogController();
                eventlogCtrl.AddLog("BBStoreImportController.GetOrderAdditionalCosts", message,
                    PortalSettings.Current, PortalSettings.Current.UserId, EventLogController.EventLogType.ADMIN_ALERT);
                throw;
            }

        }

        private List<OrderAdditionalCostInfo> GetForeignOrderAdditionalCostsByOrderId(int portalId,int foreignOrderId, Guid storeGuid)
        {
            try
            {
                int ownOrderId = GetImportRelationOwnId(portalId, "ORDER", foreignOrderId, storeGuid);
                List<OrderAdditionalCostInfo> orderadditionalCosts = Controller.GetOrderAdditionalCosts(ownOrderId);
                if (storeGuid == BBStoreController.StoreGuid)
                    return orderadditionalCosts;

                List<OrderAdditionalCostInfo> result = new List<OrderAdditionalCostInfo>();
                foreach (OrderAdditionalCostInfo orderadditionalCost in orderadditionalCosts)
                {
                    int orderadditionalCostId = GetImportRelationForeignId(portalId, "ORDERADDITIONALCOST", orderadditionalCost.OrderAdditionalCostId, storeGuid);
                    int orderId = foreignOrderId;
                    if (orderId > -1 && orderadditionalCostId > -1)
                    {
                        orderadditionalCost.OrderId = orderId;
                        orderadditionalCost.OrderAdditionalCostId = orderadditionalCostId;
                        result.Add(orderadditionalCost);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                string message = ex.ToString();
                var eventlogCtrl = new EventLogController();
                eventlogCtrl.AddLog("BBStoreImportController.GetForeignOrderAdditionalCostsByOrder", message,
                    PortalSettings.Current, PortalSettings.Current.UserId, EventLogController.EventLogType.ADMIN_ALERT);
                throw;
            }

        }

        private List<OrderAddressInfo> GetOrderAddresses(int portalId, Guid storeGuid)
        {
            try
            {
                List<OrderAddressInfo> orderAddresses = Controller.GetOrderAddressesByPortal(portalId);
                if (storeGuid == BBStoreController.StoreGuid)
                    return orderAddresses;

                List<OrderAddressInfo> result = new List<OrderAddressInfo>();
                foreach (OrderAddressInfo orderAddress in orderAddresses)
                {
                    int orderAddressId = GetImportRelationForeignId(portalId, "ORDERADDRESS", orderAddress.OrderAddressId, storeGuid);
                    int subscriberAddressTypeId = GetImportRelationForeignId(portalId, "SUBSCRIBERADDRESSTYP", orderAddress.SubscriberAddressTypeId, storeGuid);
                    int orderId = GetImportRelationForeignId(portalId, "ORDER", orderAddress.OrderId, storeGuid);
                    if (orderId > -1 && subscriberAddressTypeId > -1)
                    {
                        orderAddress._status = (orderAddressId == -1 ? 1 : 2);
                        orderAddress.OrderId = orderId;
                        orderAddress.OrderAddressId = orderAddressId;
                        orderAddress.SubscriberAddressTypeId = subscriberAddressTypeId;
                        result.Add(orderAddress);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                string message = ex.ToString();
                var eventlogCtrl = new EventLogController();
                eventlogCtrl.AddLog("BBStoreImportController.GetOrderAddresses", message,
                    PortalSettings.Current, PortalSettings.Current.UserId, EventLogController.EventLogType.ADMIN_ALERT);
                throw;
            }

        }

        private List<OrderAddressInfo> GetForeignOrderAddressesByOrderId(int portalId, int foreignOrderId, Guid storeGuid)
        {
            try
            {
                int ownOrderId = GetImportRelationOwnId(portalId, "ORDER", foreignOrderId, storeGuid);
                List<OrderAddressInfo> orderAddresses = Controller.GetOrderAddresses(ownOrderId);
                if (storeGuid == BBStoreController.StoreGuid)
                    return orderAddresses;

                List<OrderAddressInfo> result = new List<OrderAddressInfo>();
                foreach (OrderAddressInfo orderAddress in orderAddresses)
                {
                    int orderAddressId = GetImportRelationForeignId(portalId, "ORDERADDRESS", orderAddress.OrderAddressId, storeGuid);
                    int subscriberAddressTypeId = GetImportRelationForeignId(portalId, "SUBSCRIBERADDRESSTYP", orderAddress.SubscriberAddressTypeId, storeGuid);
                    int orderId = foreignOrderId;
                    if (orderId > -1 && subscriberAddressTypeId > -1)
                    {
                        orderAddress._status = (orderAddressId == -1 ? 1 : 2);
                        orderAddress.OrderId = orderId;
                        orderAddress.OrderAddressId = orderAddressId;
                        orderAddress.SubscriberAddressTypeId = subscriberAddressTypeId;
                        result.Add(orderAddress);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                string message = ex.ToString();
                var eventlogCtrl = new EventLogController();
                eventlogCtrl.AddLog("BBStoreImportController.GetOrderAddresses", message,
                    PortalSettings.Current, PortalSettings.Current.UserId, EventLogController.EventLogType.ADMIN_ALERT);
                throw;
            }

        }

        private List<CustomerInfo> GetCustomers(int portalId, Guid storeGuid)
        {
            List<CustomerInfo> customers = Controller.GetCustomersByPortal(portalId);
            if (storeGuid == BBStoreController.StoreGuid)
                return customers;

            List<CustomerInfo> result = new List<CustomerInfo>();
            foreach (CustomerInfo customer in customers)
            {
                int customerId = GetImportRelationForeignId(portalId, "CUSTOMER", customer.CustomerId, storeGuid);
                int userId = GetImportRelationForeignId(portalId, "USER", customer.UserId, storeGuid);
                if (userId > -1 && customerId > -1)
                {
                    customer.CustomerId = customerId;
                    customer.UserId = userId;
                    result.Add(customer);
                }
            }
            return result;
        }

        private List<SubscriberAddressTypeInfo> GetSubscriberAddressTypes(int portalId, Guid storeGuid)
        {
            List<SubscriberAddressTypeInfo> subscriberAddressTypes = Controller.GetSubscriberAddressTypesByPortal(portalId);
            if (storeGuid == BBStoreController.StoreGuid)
                return subscriberAddressTypes;

            List<SubscriberAddressTypeInfo> result = new List<SubscriberAddressTypeInfo>();
            foreach (SubscriberAddressTypeInfo subscriberAddressType in subscriberAddressTypes)
            {
                int subscriberAddressTypeId = GetImportRelationForeignId(portalId, "SUBSCRIBERADDRESSTYP", subscriberAddressType.SubscriberAddressTypeId, storeGuid);
                if (subscriberAddressTypeId > -1)
                {
                    subscriberAddressType.SubscriberAddressTypeId = subscriberAddressTypeId;
                    result.Add(subscriberAddressType);
                }
            }
            return result;
        }

        private List<SubscriberAddressTypeLangInfo> GetSubscriberAddressTypeLangs(int portalId, Guid storeGuid)
        {
            List<SubscriberAddressTypeLangInfo> subscriberAddressTypeLangs = Controller.GetSubscriberAddressTypeLangsByPortal(portalId);
            if (storeGuid == BBStoreController.StoreGuid)
                return subscriberAddressTypeLangs;

            List<SubscriberAddressTypeLangInfo> result = new List<SubscriberAddressTypeLangInfo>();
            foreach (SubscriberAddressTypeLangInfo subscriberAddressTypeLang in subscriberAddressTypeLangs)
            {
                int subscriberAddressTypeId = GetImportRelationForeignId(portalId, "SUBSCRIBERADDRESSTYP", subscriberAddressTypeLang.SubscriberAddressTypeId, storeGuid);
                if (subscriberAddressTypeId > -1)
                {
                    subscriberAddressTypeLang.SubscriberAddressTypeId = subscriberAddressTypeId;
                    result.Add(subscriberAddressTypeLang);
                }
            }
            return result;
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
            // TODO:try-Catch
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

        public void SaveFeatureGroupLang(int portalId, FeatureGroupLangInfo featureGroupLang, Guid storeGuid)
        {
            // TODO:try-Catch
            int ownId = GetImportRelationOwnId(portalId, "FEATUREGROUP", featureGroupLang.FeatureGroupId, storeGuid);
            if (ownId > 0)
            {
                Controller.DeleteFeatureGroupLang(ownId, featureGroupLang.Language);
                featureGroupLang.FeatureGroupId = ownId;
                Controller.NewFeatureGroupLang(featureGroupLang);
            }
        }

        public int SaveProduct(int portalId, SimpleProductInfo product, Guid storeGuid)
        {
            // TODO:try-Catch
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

            int ownUnitId = GetImportRelationOwnId(portalId, "UNIT", product.UnitId, storeGuid);
            product.UnitId = ownUnitId;

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

        public void SaveProductLang(int portalId, SimpleProductLangInfo productLang, Guid storeGuid)
        {
            // TODO:try-Catch
            int ownId = GetImportRelationOwnId(portalId, "PRODUCT", productLang.SimpleProductId, storeGuid);
            if (ownId > 0)
            {
                Controller.DeleteSimpleProductLang(ownId, productLang.Language);
                productLang.SimpleProductId = ownId;
                Controller.NewSimpleProductLang(productLang);
            }
        }

        public void SaveProductInGroup(int portalId, int productId, int productGroupId, Guid storeGuid)
        {
            // TODO:try-Catch
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
            // TODO:try-Catch
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
            // TODO:try-Catch
            int ownId = GetImportRelationOwnId(portalId, "PRODUCTGROUP", productGroupLang.ProductGroupId, storeGuid);
            if (ownId > 0)
            {
                Controller.DeleteProductGroupLang(ownId, productGroupLang.Language);
                productGroupLang.ProductGroupId = ownId;
                Controller.NewProductGroupLang(productGroupLang);
            }
        }

        public void SaveFeatureList(int portalId, FeatureListInfo featureList, Guid storeGuid)
        {
            // TODO:try-Catch
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
            // TODO:try-Catch
            int ownId = GetImportRelationOwnId(portalId, "FEATURELIST", featureListLang.FeatureListId, storeGuid);
            if (ownId > 0)
            {
                featureListLang.FeatureListId = ownId;
                Controller.DeleteFeatureListLang(ownId, featureListLang.Language);
                Controller.NewFeatureListLang(featureListLang);
            }
        }

        public void SaveFeatureListItem(int portalId, FeatureListItemInfo featureListItem, Guid storeGuid)
        {
            string originalFeatureListItem = JsonConvert.SerializeObject(featureListItem);
            int ownId = GetImportRelationOwnId(portalId, "FEATURELISTITEM", featureListItem.FeatureListItemId, storeGuid);
            int ownFeatureListId = featureListItem.FeatureListId;

            try
            {
                if (featureListItem.FeatureListId != -1)
                {
                    ownFeatureListId = GetImportRelationOwnId(portalId, "FEATURELIST", featureListItem.FeatureListId, storeGuid);
                    if (ownFeatureListId < 0)
                        throw new KeyNotFoundException(String.Format("SaveFeatureListItem: FeatureListId {0} not found in ImportRelation", featureListItem.FeatureListId));
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
                    NewImportRelation(portalId, "FEATURELISTITEM", ownId, featureListItem.FeatureListItemId, storeGuid);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SaveFeatureListItem: " + ex.Message + ": FeatureListItem=" + originalFeatureListItem + " ownFeatureListItemId=" + ownId.ToString() + " ownFeatureListId=" + ownFeatureListId.ToString());
            }
        }

        public void SaveFeatureListItemLang(int portalId, FeatureListItemLangInfo featureListItemLang, Guid storeGuid)
        {
            string originalFeatureListItemLang = JsonConvert.SerializeObject(featureListItemLang);
            int ownId = GetImportRelationOwnId(portalId, "FEATURELISTITEM", featureListItemLang.FeatureListItemId, storeGuid);

            try
            {
                if (ownId > 0)
                {
                    Controller.DeleteFeatureListItemLang(ownId, featureListItemLang.Language);
                    featureListItemLang.FeatureListItemId = ownId;
                    Controller.NewFeatureListItemLang(featureListItemLang);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SaveFeatureListItemLang: " + ex.Message + ": FeatureListItemLang=" + originalFeatureListItemLang + " ownFeatureListItemId=" + ownId.ToString());
            }
        }

        public void SaveFeature(int portalId, FeatureInfo feature, Guid storeGuid)
        {
            string originalFeature = JsonConvert.SerializeObject(feature);
            int ownId = GetImportRelationOwnId(portalId, "FEATURE", feature.FeatureId, storeGuid);
            int ownFeatureGroupId = feature.FeatureGroupId;
            int ownFeatureListId = feature.FeatureListId;

            try
            {
                feature.PortalID = portalId;

                if (feature.FeatureGroupId != -1)
                {
                    ownFeatureGroupId = GetImportRelationOwnId(portalId, "FEATUREGROUP", feature.FeatureGroupId, storeGuid);
                    if (ownFeatureGroupId < 0)
                        throw new KeyNotFoundException(String.Format("FeatureGroupId {0} not found in ImportRelation", feature.FeatureGroupId));
                }

                if (feature.FeatureListId != -1)
                {
                    ownFeatureListId = GetImportRelationOwnId(portalId, "FEATURELIST", feature.FeatureListId, storeGuid);
                    if (ownFeatureListId < 0)
                        throw new KeyNotFoundException(String.Format("FeatureListId {0} could not be found in ImportRelation", feature.FeatureListId));
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
            catch (Exception ex)
            {
                throw new Exception("SaveFeature: " + ex.Message + ": Feature=" + originalFeature + " ownFeatureId=" + ownId.ToString() + "ownFeatureListId=" + ownFeatureListId.ToString() + " ownFeatureGroupId=" + ownFeatureGroupId.ToString());
            }
        }

        public void SaveFeatureLang(int portalId, FeatureLangInfo featureLang, Guid storeGuid)
        {
            string originalFeatureLang = JsonConvert.SerializeObject(featureLang);
            int ownId = GetImportRelationOwnId(portalId, "FEATURE", featureLang.FeatureId, storeGuid);
            try
            {
                if (ownId > 0)
                {
                    Controller.DeleteFeatureLang(ownId, featureLang.Language);
                    featureLang.FeatureId = ownId;
                    Controller.NewFeatureLang(featureLang);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SaveFeatureLang: " + ex.Message + ": FeatureLang=" + originalFeatureLang + " ownFeatureId=" + ownId.ToString());
            }
        }

        public void SaveFeatureValue(int portalId, FeatureValueInfo featureValue, Guid storeGuid)
        {
            string originalFeatureValue = JsonConvert.SerializeObject(featureValue);
            int ownFeatureId = GetImportRelationOwnId(portalId, "FEATURE", featureValue.FeatureId, storeGuid);
            int ownProductId = GetImportRelationOwnId(portalId, "PRODUCT", featureValue.ProductId, storeGuid);
            int ownFeatureListItemId = -1;
            int ownId = -1;

            try
            {
                if (ownFeatureId < 0)
                    throw new KeyNotFoundException(String.Format("FeatureId {0} not found in Importrelation", featureValue.FeatureId));

                if (ownProductId < 0)
                    throw new KeyNotFoundException(String.Format("ProductId {0} not found in Importrelation", featureValue.ProductId));

                if (featureValue.FeatureListItemId != null && featureValue.FeatureListItemId > -1)
                {
                    ownFeatureListItemId = GetImportRelationOwnId(portalId, "FEATURELISTITEM", (int)featureValue.FeatureListItemId, storeGuid);
                    if (ownFeatureListItemId < 0)
                        throw new KeyNotFoundException(String.Format("FeatureListItemId {0} not found in ImportRelation", featureValue.FeatureListItemId));
                    featureValue.FeatureListItemId = ownFeatureListItemId;
                }
                else
                {
                    featureValue.FeatureListItemId = null;
                }

                ownId = Controller.GetFeatureValueId(ownProductId, ownFeatureId);
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
            catch (Exception ex)
            {
                throw new Exception("SaveFeatureValue: " + ex.Message + ": FeatureValue=" + originalFeatureValue + " ownFeatureValueId=" + ownId.ToString() + " ownProductId=" + ownProductId.ToString() + "ownFeatureId=" + ownFeatureId.ToString() + "ownFeatureListItemId=" + ownFeatureListItemId.ToString());
            }
        }

        public void SaveProductGroupFeature(int portalId, int featureId, int productGroupId, Guid storeGuid)
        {
            string originalProductGroupFeature = String.Format("FeatureId={0}, ProductGroupId={1}",featureId,productGroupId);
            int ownFeatureId = GetImportRelationOwnId(portalId, "FEATURE", featureId, storeGuid);
            int ownProductGroupId = GetImportRelationOwnId(portalId, "PRODUCTGROUP", productGroupId, storeGuid);

            try
            {
                if (ownFeatureId < 0)
                    throw new KeyNotFoundException(String.Format("FeatureId {0} not found in Importrelation", featureId));

                if (ownProductGroupId < 0)
                    throw new KeyNotFoundException(String.Format("ProductGroupId {0} not found in Importrelation", productGroupId));

                Controller.DeleteProductGroupFeature(ownFeatureId, ownProductGroupId);
                Controller.NewProductGroupFeature(ownFeatureId, ownProductGroupId);
            }
            catch (Exception ex)
            {
                throw new Exception("SaveProductGroupFeature: " + ex.Message + ": ProductGroupFeature=" + originalProductGroupFeature + " ownFeatureId=" + ownFeatureId.ToString() + " ownProductGroupId=" + ownProductGroupId.ToString());
            }

        }

        public void SaveProductGroupListItem(int portalId, int productGroupId, int featureListItemId, Guid storeGuid)
        {
            string originalProductGroupListItem = String.Format("FeatureListItemId={0}, ProductGroupId={1}", featureListItemId, productGroupId);
            int ownFeatureListItemId = GetImportRelationOwnId(portalId, "FEATURELISTITEM", featureListItemId, storeGuid);
            int ownProductGroupId = GetImportRelationOwnId(portalId, "PRODUCTGROUP", productGroupId, storeGuid);

            try
            {
                if (ownFeatureListItemId < 0)
                    throw new KeyNotFoundException(String.Format("SaveProductGroupListItem: FeatureListItemId {0} not found in Importrelation", featureListItemId));

                if (ownProductGroupId < 0)
                    throw new KeyNotFoundException(String.Format("SaveProductGroupListItem: ProductGroupId {0} not found in ImportRelation", productGroupId));

                Controller.DeleteProductGroupListItem(ownProductGroupId, ownFeatureListItemId);
                Controller.NewProductGroupListItem(ownProductGroupId, ownFeatureListItemId);
            }
            catch (Exception ex)
            {
                throw new Exception("SaveProductGroupListItem: " + ex.Message + ": ProductGroupListItem=" + originalProductGroupListItem + " ownFeatureListItemId=" + ownFeatureListItemId.ToString() + " ownProductGroupId=" + ownProductGroupId.ToString());
            }
        }

        private void SaveUnit(int portalId, UnitInfo unit, Guid storeGuid)
        {
            string originalUnit = JsonConvert.SerializeObject(unit);
            int ownId = GetImportRelationOwnId(portalId, "UNIT", unit.UnitId, storeGuid);

            try
            {
                unit.PortalId = portalId;
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
            catch (Exception ex)
            {
                throw new Exception("SaveUnit: " + ex.Message + ": Unit=" + originalUnit + " ownUnitId=" + ownId.ToString());
            }
        }

        private void SaveUnitLang(int portalId, UnitLangInfo unitLang, Guid storeGuid)
        {
            string originalUnitLang = JsonConvert.SerializeObject(unitLang);
            int ownId = GetImportRelationOwnId(portalId, "UNIT", unitLang.UnitId, storeGuid);

            try
            {
                if (ownId > 0)
                {
                    Controller.DeleteUnitLang(ownId, unitLang.Language);
                    unitLang.UnitId = ownId;
                    Controller.NewUnitLang(unitLang);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SaveUnitLang: " + ex.Message + ": UnitLang=" + originalUnitLang + " ownUnitId=" + ownId.ToString());
            }
            
        }

        private void SaveOrder(int portalId, OrderInfo order, Guid storeGuid)
        {
            string originalOrder = JsonConvert.SerializeObject(order);
            int ownId = GetImportRelationOwnId(portalId, "ORDER", order.OrderID, storeGuid);
            int ownCustomerId = GetImportRelationOwnId(portalId, "CUSTOMER", order.CustomerID, storeGuid);
            int ownSubscriberId = GetImportRelationOwnId(portalId, "USER", order.SubscriberID, storeGuid);
            try
            {
                order.PortalId = portalId;
                if (ownId > -1)
                {
                    order.OrderID = ownId;
                    order.CustomerID = ownCustomerId;
                    order.SubscriberID = ownSubscriberId;
                    Controller.UpdateOrder(order);
                }
                else
                {
                    order.CustomerID = ownCustomerId;
                    order.SubscriberID = ownSubscriberId;
                    ownId = Controller.NewOrder(order);
                    NewImportRelation(portalId, "ORDER", ownId, order.OrderID, storeGuid);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SaveOrder: " + ex.Message + ": Order=" + originalOrder + " ownOrderID=" + ownId.ToString() + " ownCustomerId=" + ownCustomerId.ToString() + " ownSubscriberId="+ ownSubscriberId.ToString());
            }
        }

        private void SaveOrderProduct(int portalId, OrderProductInfo orderProduct, Guid storeGuid)
        {
            string originalOrderProduct = JsonConvert.SerializeObject(orderProduct);
            int ownId = -1;
            int ownProductId = -1;
            int ownOrderId = -1;
            try
            {
                if (orderProduct.OrderProductId < -1)
                    ownId = (-1)*orderProduct.OrderProductId;
                else
                    ownId = GetImportRelationOwnId(portalId, "ORDERPRODUCT", orderProduct.OrderProductId, storeGuid);

                ownProductId = orderProduct.ProductId;
                ownOrderId = orderProduct.OrderId;
                if (orderProduct.ProductId != -1)
                {
                    ownProductId = GetImportRelationOwnId(portalId, "PRODUCT", orderProduct.ProductId, storeGuid);
                    if (ownProductId < 0)
                        throw new KeyNotFoundException($"ProductId {orderProduct.ProductId} not found in ImportRelation");
                }

                if (orderProduct.OrderId != -1)
                {
                    ownOrderId = GetImportRelationOwnId(portalId, "ORDER", orderProduct.OrderId, storeGuid);
                    if (ownOrderId < 0)
                        throw new KeyNotFoundException($"OrderId {orderProduct.OrderId} not found in ImportRelation");
                }

                if (ownId > -1)
                {
                    orderProduct.OrderProductId = ownId;
                    orderProduct.ProductId = ownProductId;
                    orderProduct.OrderId = ownOrderId;
                    Controller.UpdateOrderProduct(orderProduct);
                }
                else
                {
                    orderProduct.ProductId = ownProductId;
                    orderProduct.OrderId = ownOrderId;
                    ownId = Controller.NewOrderProduct(orderProduct);
                    NewImportRelation(portalId, "ORDERPRODUCT", ownId, orderProduct.OrderProductId, storeGuid);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SaveOrderProduct: " + ex.Message + ": OrderProduct=" + originalOrderProduct + " ownOrderProductId=" + ownId.ToString() + " ownProductId=" + ownProductId.ToString() + " ownOrderId=" + ownOrderId.ToString());
            }
 
        }

        private void SaveOrderProductOption(int portalId, OrderProductOptionInfo orderProductOption, Guid storeGuid)
        {
            string originalOrderProductOption = JsonConvert.SerializeObject(orderProductOption);
            int ownId = GetImportRelationOwnId(portalId, "ORDERPRODUCTOPTION", orderProductOption.OrderProductOptionId, storeGuid);
            int ownOrderProductId = -1;
            try
            {
                if (orderProductOption.OrderProductId < -1)
                    ownOrderProductId = (-1) * orderProductOption.OrderProductId;
                else
                    ownOrderProductId = GetImportRelationOwnId(portalId, "ORDERPRODUCT", orderProductOption.OrderProductId, storeGuid);

                if (orderProductOption.OrderProductId != -1)
                {
                    if (ownOrderProductId < 0)
                        throw new KeyNotFoundException(String.Format("OrderProductId {0} not found in ImportRelation", orderProductOption.OrderProductId));
                }

                if (ownId > -1)
                {
                    orderProductOption.OrderProductOptionId = ownId;
                    orderProductOption.OrderProductId = ownOrderProductId;
                    Controller.UpdateOrderProductOption(orderProductOption);
                }
                else
                {
                    orderProductOption.OrderProductId = ownOrderProductId;
                    ownId = Controller.NewOrderProductOption(orderProductOption);
                    NewImportRelation(portalId, "ORDERPRODUCTOPTION", ownId, orderProductOption.OrderProductOptionId, storeGuid);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SaveOrderProductOption: " + ex.Message + ": OrderProductOption=" + originalOrderProductOption + " ownOrderProductOptionId=" + ownId.ToString() + " ownOrderProductId=" + ownOrderProductId.ToString());
            }
        }

        private void SaveOrderAdditionalCost(int portalId, OrderAdditionalCostInfo orderAdditionalCost, Guid storeGuid)
        {
            string originalOrderAdditionalCost = JsonConvert.SerializeObject(orderAdditionalCost);
            int ownId = GetImportRelationOwnId(portalId, "ORDERADDITIONALCOST", orderAdditionalCost.OrderAdditionalCostId, storeGuid);
            int ownOrderId = orderAdditionalCost.OrderId;

            try
            {
                if (orderAdditionalCost.OrderId != -1)
                {
                    ownOrderId = GetImportRelationOwnId(portalId, "ORDER", orderAdditionalCost.OrderId, storeGuid);
                    if (ownOrderId < 0)
                        throw new KeyNotFoundException(String.Format("OrderId {0} not found in ImportRelation", orderAdditionalCost.OrderId));
                }

                if (ownId > -1)
                {
                    orderAdditionalCost.OrderAdditionalCostId = ownId;
                    orderAdditionalCost.OrderId = ownOrderId;
                    Controller.UpdateOrderAdditionalCost(orderAdditionalCost);
                }
                else
                {
                    orderAdditionalCost.OrderId = ownOrderId;
                    ownId = Controller.NewOrderAdditionalCost(orderAdditionalCost);
                    NewImportRelation(portalId, "ORDERADDITIONALCOST", ownId, orderAdditionalCost.OrderAdditionalCostId, storeGuid);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SaveOrderAdditionalCost: " + ex.Message + ": OrderAdditionalCost=" + originalOrderAdditionalCost + " ownOrderAdditionalCostId=" + ownId.ToString() + " ownOrderId=" + ownOrderId.ToString());
            }
        }

        private void SaveOrderAddress(int portalId, OrderAddressInfo orderAddress, Guid storeGuid)
        {
            string originalOrderAddress = JsonConvert.SerializeObject(orderAddress);
            int ownId = GetImportRelationOwnId(portalId, "ORDERADDRESS", orderAddress.OrderAddressId, storeGuid);
            int ownOrderId = orderAddress.OrderId;
            int ownCustomerAddressId = -1;
            int ownSubscriberAddressTypeId = -1;

            try
            {
                if (ownOrderId != -1)
                {
                    ownOrderId = GetImportRelationOwnId(portalId, "ORDER", orderAddress.OrderId, storeGuid);
                    if (ownOrderId < 0)
                        throw new KeyNotFoundException(String.Format("OrderId {0} not found in ImportRelation", orderAddress.OrderId));
                }

                ownCustomerAddressId = orderAddress.CustomerAddressId;
                if (ownCustomerAddressId != -1)
                {
                    ownCustomerAddressId = GetImportRelationOwnId(portalId, "CUSTOMERADDRESS", orderAddress.CustomerAddressId, storeGuid);
                    if (ownCustomerAddressId < 0)
                        throw new KeyNotFoundException(String.Format("CustomerAddressId {0} not found in ImportRelation", orderAddress.OrderId));
                }

                ownSubscriberAddressTypeId = orderAddress.SubscriberAddressTypeId;
                if (ownSubscriberAddressTypeId != -1)
                {
                    ownSubscriberAddressTypeId = GetImportRelationOwnId(portalId, "SUBSCRIBERADDRESSTYP", orderAddress.SubscriberAddressTypeId, storeGuid);
                    if (ownSubscriberAddressTypeId < 0)
                        throw new KeyNotFoundException(String.Format("SubscriberAddressTypeId {0} not found in ImportRelation", orderAddress.OrderId));
                }

                if (ownId > -1)
                {
                    orderAddress.OrderAddressId = ownId;
                    orderAddress.OrderId = ownOrderId;
                    orderAddress.CustomerAddressId = ownCustomerAddressId;
                    orderAddress.SubscriberAddressTypeId = ownSubscriberAddressTypeId;
                    Controller.UpdateOrderAddress(orderAddress);
                }
                else
                {
                    orderAddress.OrderId = ownOrderId;
                    orderAddress.CustomerAddressId = ownCustomerAddressId;
                    orderAddress.SubscriberAddressTypeId = ownSubscriberAddressTypeId;
                    ownId = Controller.NewOrderAddress(orderAddress);
                    NewImportRelation(portalId, "ORDERADDRESS", ownId, orderAddress.OrderAddressId, storeGuid);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SaveOrderAddress: " + ex.Message + ": OrderAddress=" + originalOrderAddress + " ownOrderAddressId=" + ownId.ToString() + " ownOrderId=" + ownOrderId.ToString() + " ownCustomerAddressId=" + ownCustomerAddressId.ToString() + " ownSubscriberAddressTypeId=" + ownSubscriberAddressTypeId.ToString());
            }

        }

        private void SaveCustomer(int portalId, CustomerInfo customer, Guid storeGuid)
        {
            string originalCustomer = JsonConvert.SerializeObject(customer);
            int ownId = GetImportRelationOwnId(portalId, "CUSTOMER", customer.CustomerId, storeGuid);
            int ownUserId = customer.UserId;

            try
            {
                if (customer.UserId != -1)
                {
                    ownUserId = GetImportRelationOwnId(portalId, "USER", customer.UserId, storeGuid);
                    if (ownUserId < 0)
                        throw new KeyNotFoundException(String.Format("UserId {0} not found in ImportRelation", customer.UserId));
                }

                if (ownId > -1)
                {
                    customer.CustomerId = ownId;
                    customer.UserId = ownUserId;
                    Controller.UpdateCustomer(customer);
                }
                else
                {
                    customer.UserId = ownUserId;
                    ownId = Controller.NewCustomer(customer);
                    NewImportRelation(portalId, "CUSTOMER", ownId, customer.CustomerId, storeGuid);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SaveCustomer: " + ex.Message + ": Customer=" + originalCustomer + " ownCustomerId=" + ownId.ToString() + " ownUserId=" + ownUserId.ToString());
            }
        }

        private void SaveSubscriberAddressType(int portalId, SubscriberAddressTypeInfo subscriberAddressType, Guid storeGuid)
        {
            string originalSubscriberAddressType = JsonConvert.SerializeObject(subscriberAddressType);
            int ownId = GetImportRelationOwnId(portalId, "SUBSCRIBERADDRESSTYP", subscriberAddressType.SubscriberAddressTypeId, storeGuid);
            int ownSubscriberId = subscriberAddressType.SubscriberId;

            try
            {
                if (ownSubscriberId != -1)
                {
                    ownSubscriberId = GetImportRelationOwnId(portalId, "SUBSCRIBER", subscriberAddressType.SubscriberId, storeGuid);
                    if (ownSubscriberId < 0)
                        throw new KeyNotFoundException(String.Format("SubscriberId {0} not found in ImportRelation", subscriberAddressType.SubscriberId));
                }
                if (ownId > -1)
                {
                    subscriberAddressType.SubscriberAddressTypeId = ownId;
                    subscriberAddressType.SubscriberId = ownSubscriberId;
                    Controller.UpdateSubscriberAddressType(subscriberAddressType);
                }
                else
                {
                    subscriberAddressType.SubscriberId = ownSubscriberId;
                    ownId = Controller.NewSubscriberAddressType(subscriberAddressType);
                    NewImportRelation(portalId, "SUBSCRIBERADDRESSTYP", ownId, subscriberAddressType.SubscriberAddressTypeId, storeGuid);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SaveSubscriberAddressType: " + ex.Message + ": SubscriberAddressType=" + originalSubscriberAddressType + " ownSubscriberAddressTypeId=" + ownId.ToString() + " ownSubscriberId=" + ownSubscriberId.ToString());
            }
        }

        public void SaveSubscriberAddressTypeLang(int portalId, SubscriberAddressTypeLangInfo subscriberAddressTypeLang, Guid storeGuid)
        {
            string originalSubscriberAddressTypeLang = JsonConvert.SerializeObject(subscriberAddressTypeLang);
            int ownId = GetImportRelationOwnId(portalId, "SUBSCRIBERADDRESSTYP", subscriberAddressTypeLang.SubscriberAddressTypeId, storeGuid);

            try
            {
                if (ownId > 0)
                {
                    Controller.DeleteSubscriberAddressTypeLang(ownId, subscriberAddressTypeLang.Language);
                    subscriberAddressTypeLang.SubscriberAddressTypeId = ownId;
                    Controller.NewSubscriberAddressTypeLang(subscriberAddressTypeLang);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SaveSubscriberAddressTypeLang: " + ex.Message + ": SubscriberAddressTypeLang=" + originalSubscriberAddressTypeLang + " ownSubscriberAddressTypeId=" + ownId.ToString());
            }
        }

        public void SaveUser(int portalId, int userId, string firstName, string lastName, string displayName, string email, string password, Guid storeGuid)
        {
            int ownId = GetImportRelationOwnId(portalId, "USER", userId, storeGuid);
            if (ownId > -1)
            {
                UserInfo user = UserController.GetUserById(portalId, ownId);
            }
            else
            {
                AppUserInfo newUser = new AppUserInfo()
                                      {
                                          DisplayName = displayName,
                                          FirstName =  firstName,
                                          LastName = lastName,
                                          Email = email,
                                          Password = password
                                      };

                UserInfo user = CreateUser(portalId, newUser);
                NewImportRelation(portalId, "USER", user.UserID, userId, storeGuid);
            }
            
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

        private void EnsureImportRelationIntegrity(int portalId, Guid storeId)
        {
            DataProvider.Instance().EnsureImportRelationIntegrity(portalId, storeId);
        }


        #endregion

        public UserInfo CreateUser(int portalId, AppUserInfo newUser)
        {
            UserInfo user = new UserInfo();
            user.Username = newUser.UserName;
            user.FirstName = newUser.FirstName;
            user.LastName = newUser.LastName;
            user.PortalID = portalId;
            user.Email = newUser.Email;
            user.DisplayName = newUser.DisplayName;
            user.Membership.Password = newUser.Password;
            user.IsSuperUser = false;

            user.Profile.InitialiseProfile(portalId);
            user.Profile.PreferredLocale = PortalSettings.Current.DefaultLanguage;
            user.Profile.PreferredTimeZone = PortalSettings.Current.TimeZone;
            user.Profile.FirstName = user.FirstName;
            user.Profile.LastName = user.LastName;
            
            UserCreateStatus status = MembershipProvider.Instance().CreateUser(ref user);

            if (status == UserCreateStatus.Success || status == UserCreateStatus.UsernameAlreadyExists)
            {
                if (status == UserCreateStatus.UsernameAlreadyExists)
                    UserController.AddUserPortal(portalId, user.UserID);

                // Add User to Standard Roles
                RoleController roleController = new RoleController();
                var roles = roleController.GetRoles(portalId);
                for (int i = 0; i < roles.Count - 1; i++)
                {
                    RoleInfo role = (RoleInfo)roles[i];
                    if (role.AutoAssignment == true)
                        roleController.AddUserRole(portalId, user.UserID, role.RoleID, Null.NullDate, Null.NullDate);
                }
            }
            return user;
        }
    }
}