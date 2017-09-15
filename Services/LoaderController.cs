using System;
using DotNetNuke.Web.Api;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Dispatcher;
using System.Web.Http;
using DotNetNuke.Entities.Portals;
using Newtonsoft.Json;

namespace Bitboxx.DNNModules.BBStore.Services
{
    public class LoaderController : DnnApiController
    {

        [HttpPost]
        [DnnAuthorize(StaticRoles = "Administrators")]
        public HttpResponseMessage ResetStore()
        {
            try
            {
                var content = Request.Content;
                string jsonContent = content.ReadAsStringAsync().Result;

                dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(jsonContent);

                int portalId = -1;
                Guid storeGuid = Guid.Empty;
                bool skipImages = true;

                if (((IDictionary<String, object>)obj).ContainsKey("PortalId"))
                    portalId = Convert.ToInt32(obj.PortalId);
                else
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "PortalId must be zero or greater");

                if (((IDictionary<String, object>)obj).ContainsKey("StoreId"))
                    storeGuid = new Guid(obj.StoreId);
                else
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "StoreGuid must be valid!");

                if (((IDictionary<String, object>)obj).ContainsKey("SkipImages"))
                    skipImages = obj.SkipImages;

                BBStoreImportController importCtrl = new BBStoreImportController();
                importCtrl.ResetStore(portalId, skipImages, storeGuid);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        [DnnAuthorize(StaticRoles = "Administrators")]
        public HttpResponseMessage GetChangedOrders()
        {
            try
            {
                var content = Request.Content;
                string jsonContent = content.ReadAsStringAsync().Result;

                dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(jsonContent);

                int portalId = -1;
                Guid storeGuid = Guid.Empty;

                if (((IDictionary<String, object>)obj).ContainsKey("PortalId"))
                    portalId = Convert.ToInt32(obj.PortalId);
                else
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "PortalId must be zero or greater");

                if (((IDictionary<String, object>)obj).ContainsKey("StoreId"))
                    storeGuid = new Guid(obj.StoreId);
                else
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "StoreGuid must be valid!");

                BBStoreImportController importCtrl = new BBStoreImportController();
                BBStoreInfo bbstore = importCtrl.GetAppOrders(portalId, storeGuid);

                string json = JsonConvert.SerializeObject(bbstore);

                return Request.CreateResponse(HttpStatusCode.OK, json);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        [DnnAuthorize(StaticRoles = "Administrators")]
        public HttpResponseMessage ImportStore()
        {
            try
            {
                var content = Request.Content;
                string jsonContent = content.ReadAsStringAsync().Result;

                BBStoreInfo bbStore = JsonConvert.DeserializeObject<BBStoreInfo>(jsonContent);

                BBStoreImportController importCtrl = new BBStoreImportController();
                importCtrl.ImportStore(PortalSettings.PortalId,bbStore);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [DnnAuthorize(StaticRoles = "Administrators")]
        public HttpResponseMessage DeleteProductImages()
        {
            try
            {
                var content = Request.Content;
                string jsonContent = content.ReadAsStringAsync().Result;

                dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(jsonContent);

                int portalId = -1;
                Guid storeGuid = Guid.Empty;
                int productId = -1;

                if (((IDictionary<String, object>)obj).ContainsKey("PortalId"))
                    portalId = Convert.ToInt32(obj.PortalId);
                else
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "PortalId must be zero or greater");

                if (((IDictionary<String, object>)obj).ContainsKey("StoreId"))
                    storeGuid = new Guid(obj.StoreId);
                else
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "StoreGuid must be valid!");

                if (((IDictionary<String, object>)obj).ContainsKey("SimpleProductId"))
                    productId = Convert.ToInt32(obj.SimpleProductId);
                else
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "SimpleProductId must be zero or greater");

                BBStoreImportController importCtrl = new BBStoreImportController();

                importCtrl.DeleteProductImages(portalId,productId,storeGuid);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        [DnnAuthorize(StaticRoles = "Administrators")]
        public HttpResponseMessage SaveImage()
        {
            try
            {
                var content = Request.Content;
                string jsonContent = content.ReadAsStringAsync().Result;

                dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(jsonContent);

                int portalId = -1;
                Guid storeGuid = Guid.Empty;
                int productId = -1;
                byte[] pictureData;
                string newFileName;

                if (((IDictionary<String, object>)obj).ContainsKey("PortalId"))
                    portalId = Convert.ToInt32(obj.PortalId);
                else
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "PortalId must be zero or greater");

                if (((IDictionary<String, object>)obj).ContainsKey("SimpleProductId"))
                    productId = Convert.ToInt32(obj.SimpleProductId);
                else
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "SimpleProductId must be zero or greater");

                if (((IDictionary<String, object>)obj).ContainsKey("StoreId"))
                    storeGuid = new Guid(obj.StoreId);
                else
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "StoreGuid must be valid!");

                if (((IDictionary<String, object>)obj).ContainsKey("Imagedata"))
                    pictureData = Convert.FromBase64String(obj.Imagedata);
                else
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "No imagedata available!");

                if (((IDictionary<String, object>)obj).ContainsKey("NewFilename"))
                    newFileName = obj.NewFilename;
                else
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "No Filename sended!");


                BBStoreImportController importCtrl = new BBStoreImportController();
                importCtrl.SaveImage(portalId, productId, pictureData, newFileName, storeGuid);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [DnnAuthorize(StaticRoles = "Administrators")]
        public HttpResponseMessage DeleteEmptyImageDirectories()
        {
            try
            {
                BBStoreImportController importCtrl = new BBStoreImportController();
                importCtrl.DeleteEmptyImageDirectories(PortalSettings.PortalId);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}