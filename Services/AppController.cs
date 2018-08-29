using System;
using System.Collections;
using DotNetNuke.Web.Api;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.Log.EventLog;
using Newtonsoft.Json;
using DotNetNuke.Entities.Portals;

namespace Bitboxx.DNNModules.BBStore.Services
{
    public class AppController : DnnApiController
    {
        [HttpGet]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        public HttpResponseMessage GetDatabase()
        {
            string logPath = PortalSettings.HomeDirectoryMapPath + @"AppLogs\" + PortalSettings.UserId.ToString() + @"\";
            string fileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss_ffff") + "_GetDatabase";
            Directory.CreateDirectory(logPath);

            try
            {
                BBStoreController controller = new BBStoreController();

                dynamic obj = new ExpandoObject();
                obj.structure = new ExpandoObject();
                obj.structure.tables = new ExpandoObject();

                obj.structure.tables.ProductGroupLang = "(ProductGroupId INTEGER,Language TEXT, ProductGroupName TEXT, ProductGroupShortDescription TEXT,ProductGroupDescription TEXT)";
                obj.structure.tables.ProductGroup = "(ProductGroupId INTEGER,ParentId INTEGER,SubscriberId INTEGER,PortalId INTEGER,Image TEXT,Icon TEXT,ProductListTabId INTEGER,Disabled INTEGER,ViewOrder INTEGER)";
                obj.structure.tables.ProductLang = "(SimpleProductId INTEGER,Language TEXT, Name TEXT, ShortDescription TEXT,ProductDescription TEXT, Attributes TEXT)";
                obj.structure.tables.Product = "(SimpleProductId INTEGER,SubscriberId INTEGER,PortalId INTEGER,Image TEXT,ItemNo TEXT,UnitCost DECIMAL(12,4),OriginalUnitCost DECIMAL(12,4),HideCost BOOLEAN,TaxPercent DECIMAL(4,1),CreatedOnDate DATETIME,CreatedByUserId INTEGER,LastModifiedOnDate DATETIME,LastModifiedByUserId INTEGER,Disabled BOOLEAN,NoCart BOOLEAN,SupplierId INTEGER,UnitId INTEGER,Weight DECIMAL(12,3))";
                obj.structure.tables.ProductInGroup = "(ProductGroupId INTEGER,SimpleProductId INTEGER)";
                obj.structure.tables.Orders = "(OrderID INTEGER,PortalId INTEGER,SubscriberID INTEGER,OrderNo TEXT,OrderTime DATETIME,OrderName TEXT,OrderStateId INTEGER,CustomerID INTEGER,Comment TEXT,Currency TEXT,PaymentProviderId INTEGER,PaymentProviderValues TEXT,Total DECIMAL(12,4),TaxId TEXT,AttachName TEXT,AttachContentType TEXT,Attachment TEXT)";
                obj.structure.tables.OrderProduct = "(OrderProductId INTEGER,OrderId INTEGER,ProductId INTEGER,Image TEXT,ItemNo TEXT,Quantity DECIMAL(12,3),Name TEXT,Description TEXT,UnitCost DECIMAL(12,4),TaxPercent DECIMAL(4,1),Unit TEXT)";
                obj.structure.tables.OrderProductOption = "(OrderProductOptionId INTEGER,OrderProductId INTEGER,OptionId INTEGER,OptionName TEXT,OptionValue TEXT,PriceAlteration DECIMAL(12,4),OptionImage TEXT,OptionDescription TEXT)";
                obj.structure.tables.OrderAdditionalCost = "(OrderAdditionalCostID INTEGER,Quantity DECIMAL(12,3),Name TEXT,Description TEXT,UnitCost DECIMAL(12,4),TaxPercent DECIMAL(4,1),Area TEXT)";

                obj.structure.tables.OrderAddress = "(OrderAddressId INTEGER PRIMARY KEY,PortalId INTEGER,OrderId INTEGER,CustomerAddressId INTEGER,Company TEXT,Prefix TEXT,Firstname TEXT,Middlename TEXT,Lastname TEXT,Suffix TEXT,Unit TEXT,Street TEXT,Region TEXT,PostalCode TEXT,City TEXT,Suburb TEXT,Country TEXT,CountryCode TEXT,Telephone TEXT,Cell TEXT,Fax TEXT,Email TEXT,SubscriberAddressTypeId INTEGER)";
                obj.structure.tables.Customer = "(CustomerId INTEGER,PortalId INTEGER,UserId INTEGER,CustomerName TEXT)";
                obj.structure.tables.SubscriberAddressType = "(SubscriberAddressTypeId INTEGER,PortalId INTEGER,SubscriberId INTEGER,KzAddressType TEXT,Mandatory BOOLEAN,ViewOrder INTEGER,IsOrderAddress BOOLEAN)";
                obj.structure.tables.SubscriberAddressTypeLang = "(SubscriberAddressTypeId INTEGER,Language TEXT,AddressType TEXT)";

                obj.data = new ExpandoObject();
                obj.data.inserts = new ExpandoObject();

                obj.data.inserts.ProductGroupLang = controller.GetProductGroupLangsByPortal(PortalSettings.PortalId).ToList();
                IEnumerable<ProductGroupInfo> pgs = controller.GetProductGroups(PortalSettings.PortalId);
                obj.data.inserts.ProductGroup = (from p in pgs select new { p.ProductGroupId, p.ParentId, p.SubscriberId, p.PortalId, p.Image, p.Icon, p.ProductListTabId, p.Disabled, p.ViewOrder }).ToList();
                obj.data.inserts.ProductLang = controller.GetSimpleProductLangsByPortal(PortalSettings.PortalId);
                IEnumerable<SimpleProductInfo> sps = controller.GetSimpleProducts(PortalSettings.PortalId);
                obj.data.inserts.Product = (from p in sps select new { p.SimpleProductId, p.SubscriberId, p.PortalId, p.Image, p.ItemNo, p.UnitCost, p.OriginalUnitCost, p.HideCost, p.TaxPercent, p.CreatedOnDate, p.CreatedByUserId, p.LastModifiedOnDate, p.LastModifiedByUserId, p.Disabled, p.NoCart, p.SupplierId, p.UnitId, p.Weight }).ToList();
                IEnumerable<ProductInGroupInfo> pigs = controller.GetProductsInGroupByPortal(PortalSettings.PortalId);
                obj.data.inserts.ProductInGroup = (from p in pigs select new { p.ProductGroupId, p.SimpleProductId }).ToList();
                IEnumerable<OrderInfo> os = controller.GetOrdersByPortalAndUser(PortalSettings.PortalId, PortalSettings.UserId, 7);
                int orderCnt = os.Count();
                obj.data.inserts.Orders = (from p in os select new { p.OrderID, p.PortalId, p.SubscriberID, p.OrderNo, p.OrderTime, p.OrderName, p.OrderStateId, p.CustomerID, p.Comment, p.Currency, p.PaymentProviderId, p.PaymentProviderValues, p.Total, p.TaxId, p.AttachName, p.AttachContentType, p.Attachment }).ToList();
                IEnumerable<OrderProductInfo> ops = controller.GetOrderProductsByPortalAndUser(PortalSettings.PortalId, PortalSettings.UserId, 7);
                obj.data.inserts.OrderProduct = (from p in ops select new { p.OrderProductId, p.OrderId, p.ProductId, p.Image, p.ItemNo, p.Quantity, p.Name, p.Description, p.UnitCost, p.TaxPercent, p.Unit }).ToList();
                IEnumerable<OrderProductOptionInfo> opos = controller.GetOrderProductOptionsByPortalAndUser(PortalSettings.PortalId, PortalSettings.UserId, 7);
                obj.data.inserts.OrderProductOption = (from p in opos select new { p.OrderProductOptionId, p.OrderProductId, p.OptionId, p.OptionName, p.OptionValue, p.PriceAlteration, p.OptionImage, p.OptionDescription }).ToList();
                IEnumerable<OrderAdditionalCostInfo> oacs = controller.GetOrderAdditionalCostsByPortalAndUser(PortalSettings.PortalId, PortalSettings.UserId, 7);
                obj.data.inserts.OrderAdditionalCost = (from p in oacs select new { p.OrderAdditionalCostId, p.Quantity, p.Name, p.Description, p.UnitCost, p.TaxPercent, p.Area }).ToList();
                IEnumerable<OrderAddressInfo> oas = controller.GetOrderAddressesByPortalAndUser(PortalSettings.PortalId, PortalSettings.UserId, 7);
                obj.data.inserts.OrderAddress = (from p in oas select new { p.OrderAddressId, p.PortalId, p.OrderId, p.CustomerAddressId, p.Company, p.Prefix, p.Firstname, p.Middlename, p.Lastname, p.Suffix, p.Unit, p.Street, p.Region, p.PostalCode, p.City, p.Suburb, p.Country, p.CountryCode, p.Telephone, p.Cell, p.Fax, p.Email, p.SubscriberAddressTypeId }).ToList();
                IEnumerable<CustomerInfo> cs = controller.GetCustomersByPortalAndUser(PortalSettings.PortalId, PortalSettings.UserId);
                obj.data.inserts.Customer = (from p in cs select new { p.CustomerId, p.PortalId, p.UserId, p.CustomerName }).ToList();
                IEnumerable<SubscriberAddressTypeInfo> sats = controller.GetSubscriberAddressTypesByPortal(PortalSettings.PortalId);
                obj.data.inserts.SubscriberAddressType = (from p in sats select new { p.SubscriberAddressTypeId, p.PortalId, p.SubscriberId, p.KzAddressType, p.Mandatory, p.ViewOrder, p.IsOrderAddress }).ToList();
                obj.data.inserts.SubscriberAddressTypeLang = controller.GetSubscriberAddressTypeLangsByPortal(PortalSettings.PortalId);

                string retVal = JsonConvert.SerializeObject(obj);
                while (retVal.IndexOf(@"//") > -1)
                {
                    retVal = retVal.Replace(@"//", @"/");
                }

                if (orderCnt > 0)
                {
                    var logInfo = new LogInfo() { LogTypeKey = "BBSTORE_APP" };
                    logInfo.AddProperty("Fetched Orders", orderCnt.ToString());
                    logInfo.AddProperty("UserID", UserInfo.UserID.ToString());
                    LogController.Instance.AddLog(logInfo);
                }
                File.WriteAllText(logPath + fileName + ".ok", retVal);
                return Request.CreateResponse(HttpStatusCode.OK, retVal);
            }
            catch (Exception ex)
            {
                File.WriteAllText(logPath + fileName + ".fail", ex.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        
        [HttpGet]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        public HttpResponseMessage GetProductGroupLangs()
        {
            BBStoreController controller = new BBStoreController();
            IEnumerable<ProductGroupLangInfo> items = controller.GetProductGroupLangsByPortal(PortalSettings.PortalId);
            return Request.CreateResponse(HttpStatusCode.OK, items);
        }

        [HttpGet]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        public HttpResponseMessage GetProductGroups()
        {
            BBStoreController controller = new BBStoreController();
            IEnumerable<ProductGroupInfo> items = controller.GetProductGroups(PortalSettings.PortalId);
            return Request.CreateResponse(HttpStatusCode.OK, items);
        }

        [HttpGet]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        public HttpResponseMessage GetProductLangs()
        {
            BBStoreController controller = new BBStoreController();
            IEnumerable<SimpleProductLangInfo> items = controller.GetSimpleProductLangsByPortal(PortalSettings.PortalId);
            return Request.CreateResponse(HttpStatusCode.OK, items);
        }

        [HttpGet]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        public HttpResponseMessage GetProducts()
        {
            BBStoreController controller = new BBStoreController();
            IEnumerable<SimpleProductInfo> items = controller.GetSimpleProducts(PortalSettings.PortalId);
            return Request.CreateResponse(HttpStatusCode.OK, items);
        }

        [HttpGet]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        public HttpResponseMessage GetProductInGroups()
        {
            BBStoreController controller = new BBStoreController();
            IEnumerable<ProductInGroupInfo> items = controller.GetProductsInGroupByPortal(PortalSettings.PortalId);
            return Request.CreateResponse(HttpStatusCode.OK, items);
        }

        [HttpGet]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        public HttpResponseMessage GetOrders()
        {
            BBStoreController controller = new BBStoreController();
            IEnumerable<OrderInfo> items = controller.GetOrdersByPortalAndUser(PortalSettings.PortalId, PortalSettings.UserId, 7);
            return Request.CreateResponse(HttpStatusCode.OK, items);
        }

        [HttpGet]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        public HttpResponseMessage GetOrderProducts()
        {
            BBStoreController controller = new BBStoreController();
            IEnumerable<OrderProductInfo> items = controller.GetOrderProductsByPortalAndUser(PortalSettings.PortalId, PortalSettings.UserId,7);
            return Request.CreateResponse(HttpStatusCode.OK, items);
        }

        [HttpGet]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        public HttpResponseMessage GetOrderProductOptions()
        {
            BBStoreController controller = new BBStoreController();
            IEnumerable<OrderProductOptionInfo> items = controller.GetOrderProductOptionsByPortalAndUser(PortalSettings.PortalId, PortalSettings.UserId,7);
            return Request.CreateResponse(HttpStatusCode.OK, items);
        }

        [HttpGet]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        public HttpResponseMessage GetOrderAdditionalCosts()
        {
            BBStoreController controller = new BBStoreController();
            IEnumerable<OrderAdditionalCostInfo> items = controller.GetOrderAdditionalCostsByPortalAndUser(PortalSettings.PortalId, PortalSettings.UserId,7);
            return Request.CreateResponse(HttpStatusCode.OK, items);
        }

        [HttpGet]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        public HttpResponseMessage GetOrderAddresses()
        {
            BBStoreController controller = new BBStoreController();
            IEnumerable<OrderAddressInfo> items = controller.GetOrderAddressesByPortalAndUser(PortalSettings.PortalId, PortalSettings.UserId,7);
            return Request.CreateResponse(HttpStatusCode.OK, items);
        }

        [HttpGet]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        public HttpResponseMessage GetCustomers()
        {
            BBStoreController controller = new BBStoreController();
            IEnumerable<CustomerInfo> items = controller.GetCustomersByPortalAndUser(PortalSettings.PortalId, PortalSettings.UserId);
            return Request.CreateResponse(HttpStatusCode.OK, items);
        }

        [HttpGet]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        public HttpResponseMessage GetUnits()
        {
            BBStoreController controller = new BBStoreController();
            IEnumerable<UnitInfo> items = controller.GetUnits(PortalSettings.PortalId);
            return Request.CreateResponse(HttpStatusCode.OK, items);
        }

        [HttpGet]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        public HttpResponseMessage GetUnitLangs()
        {
            BBStoreController controller = new BBStoreController();
            IEnumerable<UnitLangInfo> items = controller.GetUnitLangsByPortal(PortalSettings.PortalId);
            return Request.CreateResponse(HttpStatusCode.OK, items);
        }

        [HttpGet]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        public HttpResponseMessage GetSubscriberAddressTypes()
        {
            BBStoreController controller = new BBStoreController();
            IEnumerable<SubscriberAddressTypeInfo> items = controller.GetSubscriberAddressTypesByPortal(PortalSettings.PortalId);
            return Request.CreateResponse(HttpStatusCode.OK, items);
        }

        [HttpGet]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        public HttpResponseMessage GetSubscriberAddressTypeLangs()
        {
            BBStoreController controller = new BBStoreController();
            IEnumerable<SubscriberAddressTypeLangInfo> items = controller.GetSubscriberAddressTypeLangsByPortal(PortalSettings.PortalId);
            return Request.CreateResponse(HttpStatusCode.OK, items);
        }


        [HttpPost]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        public HttpResponseMessage ProcessChangedOrderProducts(List<OrderProductInfo> orderProducts)
        {
            string logPath = PortalSettings.HomeDirectoryMapPath + @"AppLogs\" + PortalSettings.UserId.ToString() + @"\";
            string fileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss_ffff") + "_ProcessChangedOrderProducts" ;
            Directory.CreateDirectory(logPath);
            try
            {
                File.WriteAllText(logPath + fileName + ".json", JsonConvert.SerializeObject(orderProducts));

                string message = "";
                BBStoreController controller = new BBStoreController();
                Dictionary<int, int> ids = new Dictionary<int, int>();
                foreach (OrderProductInfo orderProduct in orderProducts)
                {
                    OrderInfo o = controller.GetOrder(orderProduct.OrderId);
                    if (o != null)
                    {
                        OrderProductInfo op = controller.GetOrderProduct(orderProduct.OrderProductId);
                        if (op != null)
                        {
                            //System.IO.File.AppendAllText(@"C:\Temp\test.json", "-> UPDATE" + @"\r\n");
                            controller.UpdateOrderProduct(orderProduct);
                        }
                        else
                        {
                            //System.IO.File.AppendAllText(@"C:\Temp\test.json", "-> INSERT" + @"\r\n");
                            int oldId = orderProduct.OrderProductId;
                            int newId = controller.NewOrderProduct(orderProduct);
                            ids.Add(oldId, newId);
                        }
                    }
                    else
                    {
                        message += orderProduct.OrderId.ToString() + ",";
                    }
                }
                int orderCnt = orderProducts.GroupBy(op => op.OrderId).Select(op => op.First()).Count();
                if (orderCnt > 0)
                {
                    var logInfo = new LogInfo() {LogTypeKey = "BBSTORE_APP"};
                    logInfo.AddProperty("Changed Orders", orderCnt.ToString());
                    logInfo.AddProperty("UserID", UserInfo.UserID.ToString());
                    LogController.Instance.AddLog(logInfo);
                }
                if (!String.IsNullOrEmpty(message))
                {
                    message += " Orders not found (ignored statement)";
                    File.WriteAllText(logPath + fileName + ".warn", message);
                }
                else
                {
                    File.WriteAllText(logPath + fileName + ".ok", "");
                }
                return Request.CreateResponse(HttpStatusCode.OK, ids);
            }
            catch (Exception ex)
            {
                File.WriteAllText(logPath + fileName + ".fail", ex.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        public HttpResponseMessage ProcessChangedOrderProductOptions(List<OrderProductOptionInfo> orderProductOptions)
        {
            string logPath = PortalSettings.HomeDirectoryMapPath + @"AppLogs\" + PortalSettings.UserId.ToString() + @"\";
            string fileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss_ffff") + "_ProcessChangedOrderProductOptions";
            Directory.CreateDirectory(logPath);
            try
            {
                File.WriteAllText(logPath + fileName + ".json", JsonConvert.SerializeObject(orderProductOptions));

                string message = "";
                BBStoreController controller = new BBStoreController();
                foreach (OrderProductOptionInfo orderProductOption in orderProductOptions)
                {
                    OrderProductInfo op = controller.GetOrderProduct(orderProductOption.OrderProductId);
                    if (op != null)
                    {
                        OrderInfo order = controller.GetOrder(op.OrderId);
                        if (order != null)
                        {
                            OrderProductOptionInfo opo = controller.GetOrderProductOption(orderProductOption.OrderProductOptionId);
                            if (opo != null)
                                controller.UpdateOrderProductOption(orderProductOption);
                            else
                                controller.NewOrderProductOption(orderProductOption);
                        }
                        else
                        {
                            message += op.OrderId.ToString() + ",";
                        }
                    }
                }
                if (!String.IsNullOrEmpty(message))
                {
                    message += " Orders not found (ignored statement)";
                    File.WriteAllText(logPath + fileName + ".warn", message);
                }
                else
                {
                    File.WriteAllText(logPath + fileName + ".ok", "");
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                File.WriteAllText(logPath + fileName + ".fail", ex.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        public HttpResponseMessage ProcessChangedOrderAddresses(List<OrderAddressInfo> orderAddresses)
        {
            string logPath = PortalSettings.HomeDirectoryMapPath + @"AppLogs\" + PortalSettings.UserId.ToString() + @"\";
            string fileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss_ffff") + "_ProcessChangedOrderAdresses";
            Directory.CreateDirectory(logPath);
            try
            {
                string message = "";
                File.WriteAllText(logPath + fileName + ".json", JsonConvert.SerializeObject(orderAddresses));
                BBStoreController controller = new BBStoreController();
                foreach (OrderAddressInfo orderAddress in orderAddresses)
                {
                    OrderInfo order = controller.GetOrder(orderAddress.OrderId);
                    if (order != null)
                    {
                        OrderAddressInfo oa = controller.GetOrderAddress(orderAddress.OrderAddressId);

                        if (oa != null)
                            controller.UpdateOrderAddress(orderAddress);
                        else
                            controller.NewOrderAddress(orderAddress);
                    }
                    else
                    {
                        message += orderAddress.OrderId.ToString() + ",";
                    }

                }
                if (!String.IsNullOrEmpty(message))
                {
                    message += " Orders not found (ignored statement)";
                    File.WriteAllText(logPath + fileName + ".warn", message);
                }
                else
                {
                    File.WriteAllText(logPath + fileName + ".ok", "");
                }
                return Request.CreateResponse(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                File.WriteAllText(logPath + fileName + ".fail", ex.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        public HttpResponseMessage SetOrderState(int orderId, int state)
        {
            string logPath = PortalSettings.HomeDirectoryMapPath + @"AppLogs\" + PortalSettings.UserId.ToString() + @"\";
            string fileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss_ffff") + "_SetOrderState";
            Directory.CreateDirectory(logPath);
            try
            {
                File.WriteAllText(logPath + fileName + ".json", "{\"orderid\": " + orderId.ToString() + ",\"state\": " + state.ToString() + "}");
                BBStoreController controller = new BBStoreController();
                OrderInfo order = controller.GetOrder(orderId);
                if (order != null)
                {
                    order.OrderStateId = state;
                    controller.UpdateOrder(order);
                    File.WriteAllText(logPath + fileName + " .ok", "");
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    File.WriteAllText(logPath + fileName + ".fail", "Order not found");
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Order not found");
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(logPath + fileName + ".fail", ex.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        public HttpResponseMessage MailOrders(List<OrderInfo> orders)
        {
            string logPath = PortalSettings.HomeDirectoryMapPath + @"AppLogs\" + PortalSettings.UserId.ToString() + @"\";
            string fileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss_ffff") + "_MailOrders";
            Directory.CreateDirectory(logPath);
            try
            {
                File.WriteAllText(logPath + fileName + ".json", JsonConvert.SerializeObject(orders));

                string message = "";
                BBStoreController controller = new BBStoreController();
                foreach (OrderInfo order in orders)
                {
                    OrderInfo o = controller.GetOrder(order.OrderID);
                    if (o != null)
                    {
                        controller.MailOrder(PortalSettings.PortalId, order.OrderID);
                    }
                    else
                    {
                        message += order.OrderID.ToString() + ",";
                    }
                }
                if (!String.IsNullOrEmpty(message))
                {
                    message += " Orders not found (ignored statement)";
                    File.WriteAllText(logPath + fileName + ".warn", message);
                }
                else
                {
                    File.WriteAllText(logPath + fileName + ".ok", "");
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                File.WriteAllText(logPath + fileName + ".fail", ex.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        public HttpResponseMessage ProcessOrder(hOrderInfo hOrder)
        {
            string logPath = PortalSettings.HomeDirectoryMapPath + @"AppLogs\" + PortalSettings.UserId.ToString() + @"\";
            Directory.CreateDirectory(logPath);
            string fileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss_ffff") + $"_ProcessOrder_{hOrder.OrderID}";

            string message = "";
            bool ok = true;
            try
            {
                File.WriteAllText(logPath + fileName + ".json", JsonConvert.SerializeObject(hOrder));

                BBStoreController controller = new BBStoreController();
                
                OrderInfo order = controller.GetOrder(hOrder.OrderID);
                if (order != null)
                {
                    message += $"Order mit ID={hOrder.OrderID} im System vorhanden: {JsonConvert.SerializeObject(order)}\r\n";
                    foreach (hOrderProductInfo hOrderProduct in hOrder.Products)
                    {
                        OrderProductInfo op = controller.GetOrderProduct(hOrderProduct.OrderProductId);
                        int orderProductId;
                        if (op != null)
                        {
                            orderProductId = hOrderProduct.OrderProductId;
                            controller.UpdateOrderProduct(hOrderProduct.MapOrderProductInfo());
                            message += $"    OrderProduct mit ID={orderProductId} aktualisiert:\r\n" +
                                       $"        vorher:  {JsonConvert.SerializeObject(op)}\r\n" +
                                       $"        nachher: {JsonConvert.SerializeObject(hOrderProduct.MapOrderProductInfo())}\r\n\r\n";
                        }
                        else
                        {
                            orderProductId  = controller.NewOrderProduct(hOrderProduct.MapOrderProductInfo());
                            message += $"    OrderProduct mit mit ID={orderProductId} angelegt:\r\n" +
                                       $"        neu:     {JsonConvert.SerializeObject(hOrderProduct.MapOrderProductInfo())}\r\n\r\n";
                        }

                        foreach (hOrderProductOptionInfo hOrderProductOption in hOrderProduct.Options)
                        {
                            OrderProductOptionInfo hOpo = hOrderProductOption.MapOrderProductOption();
                            OrderProductOptionInfo opo = controller.GetOrderProductOption(hOrderProductOption.OrderProductOptionId);
                            if (opo != null)
                            {
                                controller.UpdateOrderProductOption(hOpo);
                                message += $"        OrderProductOption mit ID={hOrderProductOption.OrderProductOptionId} aktualisiert:\r\n" +
                                           $"            vorher:  {JsonConvert.SerializeObject(opo)}\r\n" +
                                           $"            nachher: {JsonConvert.SerializeObject(hOpo)}\r\n\r\n";
                            }
                            else
                            {
                                hOpo.OrderProductOptionId = controller.NewOrderProductOption(hOpo);
                                message += $"        OrderProductOption mit ID={ hOpo.OrderProductOptionId} angelegt:\r\n" +
                                           $"        neu:     {JsonConvert.SerializeObject(hOpo)}\r\n\r\n";
                            }
                        }
                    }

                    foreach (hOrderAddressInfo hOrderAddress in hOrder.Addresses)
                    {
                        OrderAddressInfo oa = controller.GetOrderAddress(hOrderAddress.OrderAddressId);

                        if (oa != null)
                        {
                            controller.UpdateOrderAddress(hOrderAddress.MapOrderAddressInfo());
                            message += $"    OrderAddress mit ID={hOrderAddress.OrderAddressId} aktualisiert:\r\n"+
                                       $"        vorher:  {JsonConvert.SerializeObject(oa)}\r\n" +
                                       $"        nachher:  {JsonConvert.SerializeObject(hOrderAddress.MapOrderAddressInfo())}\r\n\r\n";
                        }
                        else
                        {
                            int orderAddressId = controller.NewOrderAddress(hOrderAddress.MapOrderAddressInfo());
                            message += $"    OrderAddress mit mit ID={orderAddressId} angelegt:\r\n" +
                                       $"        neu: {JsonConvert.SerializeObject(hOrderAddress.MapOrderAddressInfo())}\r\n\r\n";
                        }
                    }

                    foreach (hOrderAdditionalCostInfo hOrderAdditionalCost in hOrder.AdditionalCosts)
                    {
                        OrderAdditionalCostInfo oac = controller.GetOrderAdditionalCost(hOrderAdditionalCost.OrderAdditionalCostId);

                        if (oac != null)
                        {
                            controller.UpdateOrderAdditionalCost(hOrderAdditionalCost.MapOrderAdditionalCost());
                            message += $"    OrderAdditionalCost mit ID={hOrderAdditionalCost.OrderAdditionalCostId} aktualisiert:\r\n"+
                                       $"        vorher:  {JsonConvert.SerializeObject(oac)}\r\n" +
                                       $"        nachher:  {JsonConvert.SerializeObject(hOrderAdditionalCost.MapOrderAdditionalCost())}\r\n\r\n";
                        }
                        else
                        {
                            int orderAdditionalCostId = controller.NewOrderAdditionalCost(hOrderAdditionalCost.MapOrderAdditionalCost());
                            message += $"    OrderAdditionalCost mit ID={orderAdditionalCostId} angelegt:\r\n"+
                                       $"        neu: {JsonConvert.SerializeObject(hOrderAdditionalCost.MapOrderAdditionalCost())}\r\n\r\n";
                        }
                    }
                    try
                    {
                        if (hOrder.PaymentProviderId > -1)
                        {
                            controller.MailOrder(PortalSettings.PortalId, hOrder.OrderID);
                            message += $"Order mit ID={hOrder.OrderID}: Mailversand erfolgt\r\n";
                        }
                        else
                        {
                            message += $"Order mit ID={hOrder.OrderID}: Kein Mailversand erforderlich\r\n";
                        }
                    }
                    catch (Exception ex)
                    {
                        message += $"Order {hOrder.OrderID}: Mailen fehlgeschlagen: {ex}\r\n";
                    }

                    order.OrderStateId = 7;
                    controller.UpdateOrder(order);
                }
                else
                {
                    message += $"Order {hOrder.OrderID} im System NICHT vorhanden\r\n";
                    ok = false;
                }

                if (ok)
                    File.WriteAllText(logPath + fileName + ".ok", message);
                else
                    File.WriteAllText(logPath + fileName + ".warn", message);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                File.WriteAllText(logPath + fileName + ".fail", ex.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [DnnAuthorize(AuthTypes = "JWT")]
        public HttpResponseMessage RegisterUser(AppUserInfo appUser)
        {
            // ACHTUNG: DNN WebApi braucht in der Info Klasse [DataContract()] und [DataMember()] Attribute damit das funktioniert !!!

            try
            {
                string testString = appUser.UserName + appUser.Password + DateTime.UtcNow.ToString("yyyyMMdd");

                Encoding hashEncoding = Encoding.UTF8;
                SHA1 hashAlgorithm = SHA1.Create();
                byte[] inputBytes = hashEncoding.GetBytes(testString);
                byte[] hashBytes = hashAlgorithm.ComputeHash(inputBytes);
                string computedSecret = BitConverter.ToString(hashBytes).Replace("-", "");

                if (!StringComparer.OrdinalIgnoreCase.Equals(appUser.Secret, computedSecret))
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, new NotSupportedException("token does not match"));
                }
                else
                {
                    BBStoreImportController ctrl = new BBStoreImportController();
                    ctrl.CreateUser(PortalSettings.PortalId, appUser);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        #region Mapping classes

        public class hOrderInfo
        {
            public int OrderID { get; set; }
            public int PortalId { get; set; }
            public int SubscriberID { get; set; }
            public string OrderNo { get; set; }
            public DateTime OrderTime { get; set; }
            public string OrderName { get; set; }
            public int OrderStateId { get; set; }
            public int CustomerID { get; set; }
            public string Comment { get; set; }
            public string Currency { get; set; }
            public int PaymentProviderId { get; set; }
            public string PaymentProviderValues { get; set; }
            public decimal OrderTotal { get; set; }
            public decimal OrderTax { get; set; }
            public decimal AdditionalTotal { get; set; }
            public decimal AdditionalTax { get; set; }
            public decimal Total { get; set; }
            public string TaxId { get; set; }
            public byte[] Attachment { get; set; }
            public string AttachName { get; set; }
            public string AttachContentType { get; set; }
            public int _status { get; set; }
            public List<hOrderProductInfo> Products { get; set; }
            public List<hOrderAddressInfo> Addresses { get; set; }
            public List<hOrderAdditionalCostInfo> AdditionalCosts { get; set; }

            public OrderInfo MapOrderInfo()
            {
                return new OrderInfo()
                       {
                           OrderID = this.OrderID,
                           PortalId = this.PortalId,
                           SubscriberID = this.SubscriberID,
                           OrderNo = this.OrderNo,
                           OrderTime = this.OrderTime,
                           OrderName = this.OrderName,
                           OrderStateId = this.OrderStateId,
                           CustomerID = this.CustomerID,
                           Comment = this.Comment,
                           Currency = this.Currency,
                           PaymentProviderId = this.PaymentProviderId,
                           PaymentProviderValues = this.PaymentProviderValues,
                           OrderTotal = this.OrderTotal,
                           OrderTax = this.OrderTax,
                           AdditionalTotal = this.AdditionalTotal,
                           AdditionalTax = this.AdditionalTax,
                           Total = this.Total,
                           TaxId = this.TaxId,
                           Attachment = this.Attachment,
                           AttachName = this.AttachName,
                           AttachContentType = this.AttachContentType,
                           _status = this._status
                       };
            }

        }
        public class hOrderProductInfo
        {
            public int OrderProductId { get; set; }
            public int OrderId { get; set; }
            public int ProductId { get; set; }
            public string Image { get; set; }
            public string Unit { get; set; }
            public string ItemNo { get; set; }
            public decimal Quantity { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string ProductUrl { get; set; }
            public decimal UnitCost { get; set; }
            public decimal NetTotal { get; set; }
            public decimal TaxPercent { get; set; }
            public decimal TaxTotal { get; set; }
            public decimal SubTotal { get; set; }
            public int _status { get; set; }
            public List<hOrderProductOptionInfo> Options { get; set; }

            public OrderProductInfo MapOrderProductInfo()
            {
                return new OrderProductInfo()
                       {
                           OrderProductId = this.OrderProductId,
                           OrderId = this.OrderId,
                           ProductId = this.ProductId,
                           Image = this.Image,
                           Unit = this.Unit,
                           ItemNo = this.ItemNo,
                           Quantity = this.Quantity,
                           Name = this.Name,
                           Description = this.Description,
                           ProductUrl = this.ProductUrl,
                           UnitCost = this.UnitCost,
                           NetTotal = this.NetTotal,
                           TaxPercent = this.TaxPercent,
                           TaxTotal = this.TaxTotal,
                           SubTotal = this.SubTotal,
                           _status = this._status
                       };
            }
        }
        public class hOrderProductOptionInfo
        {
            public int OrderProductOptionId { get; set; }
            public int OrderProductId { get; set; }
            public int OptionId { get; set; }
            public string OptionName { get; set; }
            public string OptionValue { get; set; }
            public byte[] OptionImage { get; set; }
            public string OptionDescription { get; set; }
            public decimal PriceAlteration { get; set; }
            public int _status { get; set; }

            public OrderProductOptionInfo MapOrderProductOption()
            {
                return new OrderProductOptionInfo()
                       {
                           OrderProductOptionId = this.OrderProductOptionId,
                           OrderProductId = this.OrderProductId,
                           OptionId = this.OptionId,
                           OptionName = this.OptionName,
                           OptionValue = this.OptionValue,
                           OptionImage = this.OptionImage,
                           OptionDescription = this.OptionDescription,
                           PriceAlteration = this.PriceAlteration,
                           _status = this._status
                       };
            }
        }
        public class hOrderAdditionalCostInfo
        {
            public int OrderAdditionalCostId { get; set; }
            public int OrderId { get; set; }
            public decimal Quantity { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Area { get; set; }
            public decimal UnitCost { get; set; }
            public decimal NetTotal { get; set; }
            public decimal TaxPercent { get; set; }
            public decimal TaxTotal { get; set; }
            public decimal SubTotal { get; set; }
            public int _status { get; set; }

            public OrderAdditionalCostInfo MapOrderAdditionalCost()
            {
                return new OrderAdditionalCostInfo()
                       {
                           OrderAdditionalCostId = this.OrderAdditionalCostId,
                           OrderId = this.OrderId,
                           Quantity = this.Quantity,
                           Name = this.Name,
                           Description = this.Description,
                           Area = this.Area,
                           UnitCost = this.UnitCost,
                           NetTotal = this.NetTotal,
                           TaxPercent = this.TaxPercent,
                           TaxTotal = this.TaxTotal,
                           SubTotal = this.SubTotal,
                           _status = this._status
                       };
            }

        }
        public class hOrderAddressInfo
        {
            public int OrderAddressId { get; set; }
            public int PortalId { get; set; }
            public int OrderId { get; set; }
            public int CustomerAddressId { get; set; }
            public string Company { get; set; }
            public string Prefix { get; set; }
            public string Firstname { get; set; }
            public string Middlename { get; set; }
            public string Lastname { get; set; }
            public string Suffix { get; set; }
            public string Unit { get; set; }
            public string Street { get; set; }
            public string Region { get; set; }
            public string PostalCode { get; set; }
            public string City { get; set; }
            public string Suburb { get; set; }
            public string Country { get; set; }
            public string CountryCode { get; set; }
            public string Telephone { get; set; }
            public string Cell { get; set; }
            public string Fax { get; set; }
            public string Email { get; set; }
            public int SubscriberAddressTypeId { get; set; }
            public int _status { get; set; }

            public OrderAddressInfo MapOrderAddressInfo()
            {
                return new OrderAddressInfo()
                       {
                           OrderAddressId = this.OrderAddressId,
                           PortalId = this.PortalId,
                           OrderId = this.OrderId,
                           CustomerAddressId = this.CustomerAddressId,
                           Company = this.Company,
                           Prefix = this.Prefix,
                           Firstname = this.Firstname,
                           Middlename = this.Middlename,
                           Lastname = this.Lastname,
                           Suffix = this.Suffix,
                           Unit = this.Unit,
                           Street = this.Street,
                           Region = this.Region,
                           PostalCode = this.PostalCode,
                           City = this.City,
                           Suburb = this.Suburb,
                           Country = this.Country,
                           CountryCode = this.CountryCode,
                           Telephone = this.Telephone,
                           Cell = this.Cell,
                           Fax = this.Fax,
                           Email = this.Email,
                           SubscriberAddressTypeId = this.SubscriberAddressTypeId,
                           _status = this._status
                       };
            }

        }
        
        #endregion Mapping classes
    }
}