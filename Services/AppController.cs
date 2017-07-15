using DotNetNuke.Web.Api;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace Bitboxx.DNNModules.BBStore.Services
{
    public class AppController : DnnApiController
    {
  
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
            IEnumerable<OrderInfo> items = controller.GetOrdersByPortalAndUser(PortalSettings.PortalId, PortalSettings.UserId);
            return Request.CreateResponse(HttpStatusCode.OK, items);
        }

        [HttpGet]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        public HttpResponseMessage GetOrderProducts()
        {
            BBStoreController controller = new BBStoreController();
            IEnumerable<OrderProductInfo> items = controller.GetOrderProductsByPortalAndUser(PortalSettings.PortalId, PortalSettings.UserId);
            return Request.CreateResponse(HttpStatusCode.OK, items);
        }

        [HttpGet]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        public HttpResponseMessage GetOrderProductOptions()
        {
            BBStoreController controller = new BBStoreController();
            IEnumerable<OrderProductOptionInfo> items = controller.GetOrderProductOptionsByPortalAndUser(PortalSettings.PortalId, PortalSettings.UserId);
            return Request.CreateResponse(HttpStatusCode.OK, items);
        }

        [HttpGet]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        public HttpResponseMessage GetOrderAdditionalCosts()
        {
            BBStoreController controller = new BBStoreController();
            IEnumerable<OrderAdditionalCostInfo> items = controller.GetOrderAdditionalCostsByPortalAndUser(PortalSettings.PortalId, PortalSettings.UserId);
            return Request.CreateResponse(HttpStatusCode.OK, items);
        }

        [HttpGet]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        public HttpResponseMessage GetOrderAddresses()
        {
            BBStoreController controller = new BBStoreController();
            IEnumerable<OrderAddressInfo> items = controller.GetOrderAddressesByPortalAndUser(PortalSettings.PortalId, PortalSettings.UserId);
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

    }
}