using System;
using System.Collections;
using DotNetNuke.Web.Api;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;


namespace Bitboxx.DNNModules.BBStore.Services
{
    public class AppController : DnnApiController
    {
        [HttpGet]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        public HttpResponseMessage GetDatabase()
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
            obj.structure.tables.Orders ="(OrderID INTEGER,PortalId INTEGER,SubscriberID INTEGER,OrderNo TEXT,OrderTime DATETIME,OrderName TEXT,OrderStateId INTEGER,CustomerID INTEGER,Comment TEXT,Currency TEXT,PaymentProviderId INTEGER,PaymentProviderValues TEXT,Total DECIMAL(12,4),TaxId TEXT,AttachName TEXT,AttachContentType TEXT,Attachment TEXT)";
            obj.structure.tables.OrderProduct = "(OrderProductId INTEGER,OrderId INTEGER,ProductId INTEGER,Image TEXT,ItemNo TEXT,Quantity DECIMAL(12,3),Name TEXT,Description TEXT,UnitCost DECIMAL(12,4),TaxPercent DECIMAL(4,1),Unit TEXT)";
            obj.structure.tables.OrderProductOption = "(OrderProductOptionId INTEGER PRIMARY KEY,OrderProductId INTEGER,OptionId INTEGER,OptionName TEXT,OptionValue TEXT,PriceAlteration DECIMAL(12,4),OptionImage TEXT,OptionDescription TEXT)";
            obj.structure.tables.OrderAdditionalCost = "(OrderAdditionalCostID INTEGER,Quantity DECIMAL(12,3),Name TEXT,Description TEXT,UnitCost DECIMAL(12,4),TaxPercent DECIMAL(4,1),Area TEXT)";

            obj.structure.tables.OrderAddress = "(OrderAddressId INTEGER PRIMARY KEY,PortalId INTEGER,OrderId INTEGER,CustomerAddressId INTEGER,Company TEXT,Prefix TEXT,Firstname TEXT,Middlename TEXT,Lastname TEXT,Suffix TEXT,Unit TEXT,Street TEXT,Region TEXT,PostalCode TEXT,City TEXT,Suburb TEXT,Country TEXT,CountryCode TEXT,Telephone TEXT,Cell TEXT,Fax TEXT,Email TEXT,SubscriberAddressTypeId INTEGER)";
            obj.structure.tables.Customer = "(CustomerId INTEGER,PortalId INTEGER,UserId INTEGER,CustomerName TEXT)";
            obj.structure.tables.SubscriberAddressType = "(SubscriberAddressTypeId INTEGER,PortalId INTEGER,SubscriberId INTEGER,KzAddressType TEXT,Mandatory BOOLEAN,ViewOrder INTEGER,IsOrderAddress BOOLEAN)";
            obj.structure.tables.SubscriberAddressTypeLang = "(SubscriberAddressTypeId INTEGER,Language TEXT,AddressType TEXT)";
            
            obj.data = new ExpandoObject();
            obj.data.inserts = new ExpandoObject();

            obj.data.inserts.ProductGroupLang = controller.GetProductGroupLangsByPortal(PortalSettings.PortalId).ToList();
            IEnumerable<ProductGroupInfo> pgs = controller.GetProductGroups(PortalSettings.PortalId);
            obj.data.inserts.ProductGroup = (from p in pgs select new {p.ProductGroupId, p.ParentId, p.SubscriberId, p.PortalId, p.Image, p.Icon, p.ProductListTabId, p.Disabled, p.ViewOrder}).ToList();
            obj.data.inserts.ProductLang = controller.GetSimpleProductLangsByPortal(PortalSettings.PortalId);
            IEnumerable<SimpleProductInfo> sps = controller.GetSimpleProducts(PortalSettings.PortalId);
            obj.data.inserts.Product = (from p in sps select new {p.SimpleProductId, p.SubscriberId, p.PortalId, p.Image, p.ItemNo, p.UnitCost, p.OriginalUnitCost, p.HideCost, p.TaxPercent, p.CreatedOnDate, p.CreatedByUserId, p.LastModifiedOnDate, p.LastModifiedByUserId, p.Disabled, p.NoCart, p.SupplierId, p.UnitId, p.Weight}).ToList();
            IEnumerable<ProductInGroupInfo> pigs = controller.GetProductsInGroupByPortal(PortalSettings.PortalId);
            obj.data.inserts.ProductInGroup = (from p in pigs select new {p.ProductGroupId, p.SimpleProductId}).ToList();
            IEnumerable<OrderInfo> os = controller.GetOrdersByPortalAndUser(PortalSettings.PortalId, PortalSettings.UserId, 7);
            obj.data.inserts.Orders = (from p in os select new {p.OrderID, p.PortalId, p.SubscriberID, p.OrderNo, p.OrderTime, p.OrderName, p.OrderStateId, p.CustomerID, p.Comment, p.Currency, p.PaymentProviderId, p.PaymentProviderValues, p.Total, p.TaxId, p.AttachName, p.AttachContentType, p.Attachment}).ToList();
            IEnumerable<OrderProductInfo> ops = controller.GetOrderProductsByPortalAndUser(PortalSettings.PortalId, PortalSettings.UserId, 7);
            obj.data.inserts.OrderProduct = (from p in ops select new {p.OrderProductId, p.OrderId, p.ProductId, p.Image, p.ItemNo, p.Quantity, p.Name, p.Description, p.UnitCost, p.TaxPercent, p.Unit}).ToList();
            IEnumerable<OrderProductOptionInfo> opos = controller.GetOrderProductOptionsByPortalAndUser(PortalSettings.PortalId, PortalSettings.UserId, 7);
            obj.data.inserts.OrderProductOption = (from p in opos select new {p.OrderProductOptionId, p.OrderProductId, p.OptionId, p.OptionName, p.OptionValue, p.PriceAlteration, p.OptionImage, p.OptionDescription}).ToList();
            IEnumerable<OrderAdditionalCostInfo> oacs = controller.GetOrderAdditionalCostsByPortalAndUser(PortalSettings.PortalId, PortalSettings.UserId, 7);
            obj.data.inserts.OrderAdditionalCost = (from p in oacs select new {p.OrderAdditionalCostId, p.Quantity, p.Name, p.Description, p.UnitCost, p.TaxPercent, p.Area}).ToList();
            IEnumerable<OrderAddressInfo> oas = controller.GetOrderAddressesByPortalAndUser(PortalSettings.PortalId, PortalSettings.UserId, 7);
            obj.data.inserts.OrderAddress = (from p in oas select new {p.OrderAddressId, p.PortalId, p.OrderId, p.CustomerAddressId, p.Company, p.Prefix, p.Firstname, p.Middlename, p.Lastname, p.Suffix, p.Unit, p.Street, p.Region, p.PostalCode, p.City, p.Suburb, p.Country, p.CountryCode, p.Telephone, p.Cell, p.Fax, p.Email, p.SubscriberAddressTypeId}).ToList();
            IEnumerable<CustomerInfo> cs = controller.GetCustomersByPortalAndUser(PortalSettings.PortalId, PortalSettings.UserId);
            obj.data.inserts.Customer = (from p in cs select new {p.CustomerId, p.PortalId, p.UserId, p.CustomerName}).ToList();
            IEnumerable<SubscriberAddressTypeInfo> sats = controller.GetSubscriberAddressTypesByPortal(PortalSettings.PortalId);
            obj.data.inserts.SubscriberAddressType = (from p in sats select new {p.SubscriberAddressTypeId, p.PortalId, p.SubscriberId, p.KzAddressType, p.Mandatory, p.ViewOrder, p.IsOrderAddress}).ToList();
            obj.data.inserts.SubscriberAddressTypeLang = controller.GetSubscriberAddressTypeLangsByPortal(PortalSettings.PortalId);

            string retVal = JsonConvert.SerializeObject(obj);

            return Request.CreateResponse(HttpStatusCode.OK, retVal);
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
            IEnumerable<OrderInfo> items = controller.GetOrdersByPortalAndUser(PortalSettings.PortalId, PortalSettings.UserId,7);
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
            try
            {
                //System.IO.File.WriteAllText(@"C:\Temp\test.json", JsonConvert.SerializeObject(orderProductOptions));
                BBStoreController controller = new BBStoreController();
                BBStoreImportController importCtrl = new BBStoreImportController();
                Dictionary<int, int> ids = new Dictionary<int, int>();
                foreach (OrderProductInfo orderProduct in orderProducts)
                {
                    //System.IO.File.AppendAllText(@"C:\Temp\test.json", orderProductOption.OrderProductOptionId.ToString() + @"\r\n");
                    OrderProductInfo op = controller.GetOrderProduct(orderProduct.OrderProductId);
                    if (op != null)
                        controller.UpdateOrderProduct(orderProduct);
                    else
                    {
                        int oldId = orderProduct.OrderProductId;
                        int newId = controller.NewOrderProduct(orderProduct);
                        ids.Add(oldId,newId);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, ids);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        public HttpResponseMessage ProcessChangedOrderProductOptions(List<OrderProductOptionInfo> orderProductOptions)
        {
            try
            {
                //System.IO.File.WriteAllText(@"C:\Temp\test.json", JsonConvert.SerializeObject(orderProductOptions));
                BBStoreController controller = new BBStoreController();
                foreach (OrderProductOptionInfo orderProductOption in orderProductOptions)
                {
                    //System.IO.File.AppendAllText(@"C:\Temp\test.json", orderProductOption.OrderProductOptionId.ToString() + @"\r\n");
                    OrderProductOptionInfo opo = controller.GetOrderProductOption(orderProductOption.OrderProductOptionId);
                    if (opo != null)
                        controller.UpdateOrderProductOption(orderProductOption);
                    else
                        controller.NewOrderProductOption(orderProductOption);
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        public HttpResponseMessage ProcessChangedOrderAddresses(List<OrderAddressInfo> orderAddresses)
        {
            try
            {
                BBStoreController controller = new BBStoreController();
                foreach (OrderAddressInfo orderAddress in orderAddresses)
                {
                    OrderAddressInfo oa = controller.GetOrderAddress(orderAddress.OrderAddressId);
                    if (oa != null)
                        controller.UpdateOrderAddress(orderAddress);
                    else
                        controller.NewOrderAddress(orderAddress);
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        public HttpResponseMessage SetOrderState(int orderId, int state)
        {
            try
            {
                BBStoreController controller = new BBStoreController();
                OrderInfo order = controller.GetOrder(orderId);
                if (order != null)
                {
                    order.OrderStateId = state;
                    controller.UpdateOrder(order);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Order not found");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        public HttpResponseMessage MailOrders(List<OrderInfo> orders)
        {
            try
            {
                BBStoreController controller = new BBStoreController();
                foreach (OrderInfo order in orders)
                {
                    controller.MailOrder(PortalSettings.PortalId,order.OrderID);
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

    }
}