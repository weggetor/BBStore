using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Serialization;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;


namespace Bitboxx.DNNModules.BBStore
{
    public partial class ViewAdminMaintenance : PortalModuleBase
    {
        #region Properties

        public Guid FilterSessionId
        {
            get
            {
                string _filterSessionId;
                if (Request.Cookies["BBStoreFilterSessionId_" + PortalId.ToString()] != null)
                    _filterSessionId = (string)(Request.Cookies["BBStoreFilterSessionId_" + PortalId.ToString()].Value);
                else
                {
                    _filterSessionId = Guid.NewGuid().ToString();
                    HttpCookie keks = new HttpCookie("BBStoreFilterSessionId_" + PortalId.ToString());
                    keks.Value = _filterSessionId;
                    keks.Expires = DateTime.Now.AddDays(1);
                    Response.AppendCookie(keks);
                }
                return new Guid(_filterSessionId);
            }
        }
      
        #endregion

        #region Event Handlers
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            string FileName = System.IO.Path.GetFileNameWithoutExtension(this.AppRelativeVirtualPath);
            if (this.ID != null)
                //this will fix it when its placed as a ChildUserControl 
                this.LocalResourceFile = this.LocalResourceFile.Replace(this.ID, FileName);
            else
                // this will fix it when its dynamically loaded using LoadControl method 
                this.LocalResourceFile = this.LocalResourceFile + FileName + ".ascx.resx";
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    cmdReset.Attributes.Add("onClick", "javascript:return confirm('" + Localization.GetString("ResetConfirm.Text", this.LocalResourceFile) + "');");
                }
                
                //TOKILL: Kann weg wenn alle Shops konvertiert sind
                ConvertTemplates(PortalId);
                
                BBStoreImportController controller = new BBStoreImportController();
                List<Guid> Guids = controller.GetStoreGuids(PortalId);

                if (Guids.Count != ddlSelectExport.Items.Count)
                {
                    BBStoreImportController importController = new BBStoreImportController();
                    foreach (Guid guid in Guids)
                    {
                        string name;
                        if (guid == BBStoreController.StoreGuid)
                            name = BBStoreController.StoreName;
                        else
                            name = importController.GetImportStoreName(guid);
                        ddlSelectExport.Items.Add(new ListItem(name,guid.ToString()));
                        ddlSelectReset.Items.Add(new ListItem(name,guid.ToString()));
                    }
                    ddlSelectExport.SelectedValue = BBStoreController.StoreGuid.ToString();
                    ddlSelectReset.SelectedValue = BBStoreController.StoreGuid.ToString();
                }
                if (Guids.Count < 2)
                {
                    ddlSelectExport.Style.Add("visibility", "hidden");
                    ddlSelectReset.Style.Add("visibility", "hidden");
                }
                else
                {
                    ddlSelectExport.Style.Remove("visibility");
                    ddlSelectReset.Style.Remove("visibility");
                }

            }
            catch (Exception exc)
            {
                //Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdImport_Click(object sender, EventArgs e)
        {
            if (fulImport.HasFile)
            {
                // Get the bytes from the uploaded file
                byte[] fileData = new byte[fulImport.PostedFile.InputStream.Length];
                fulImport.PostedFile.InputStream.Read(fileData, 0, fileData.Length);

                System.Text.UTF8Encoding enc = new UTF8Encoding();
                string xml = enc.GetString(fileData);
                BBStoreInfo bbStore;

                XmlSerializer ser = new XmlSerializer(typeof(BBStoreInfo));
                using (StringReader stringReader = new StringReader(xml))
                {
                    using (XmlTextReader xmlReader = new XmlTextReader(stringReader))
                    {
                        bbStore = (BBStoreInfo) ser.Deserialize(xmlReader);
                    }
                }
                BBStoreImportController importController = new BBStoreImportController();
                if (bbStore != null)
                    importController.ImportStore(PortalId,bbStore);

                Response.Redirect(Globals.NavigateURL(TabId, "", "adminmode=maintenance"));
            }
        }

        protected void cmdExport_Click(object sender, EventArgs e)
        {
            BBStoreImportController importController = new BBStoreImportController();

            BBStoreInfo bbstore = importController.ExportStore(PortalId, new Guid(ddlSelectExport.SelectedValue));

            XmlSerializer xmlSerializer = new XmlSerializer(bbstore.GetType());
            MemoryStream stream = new MemoryStream();
            UTF8Encoding enc = new UTF8Encoding();
            XmlTextWriter xmlSink = new XmlTextWriter(stream, enc);
            xmlSerializer.Serialize(xmlSink, bbstore);
            byte[] utf8EncodedData = stream.ToArray();
            string xml = enc.GetString(utf8EncodedData);

            Response.Clear();
            Response.ContentType = "text/xml";
            Response.AddHeader("Content-Length", utf8EncodedData.Length.ToString());
            Response.AddHeader("Content-Disposition", "attachment; filename=BBStore-Export.xml");
            Response.Write(xml);
            Response.Flush();
            Response.Close();

        }
 
        protected void cmdReset_Click(object sender, EventArgs e)
        {
            BBStoreImportController importController = new BBStoreImportController();
            importController.ResetStore(PortalId,false,FilterSessionId,new Guid(ddlSelectReset.SelectedValue));
            Response.Redirect(Globals.NavigateURL(TabId, "", "adminmode=maintenance"));
        }

        protected void cmdCheck_Click(object sender, EventArgs e)
        {
            Response.Redirect(Globals.NavigateURL(TabId, "", "adminmode=check",(chkCheckOnly.Checked ? "checkonly=1": "checkonly=0")));
        }
        #endregion

        public void ConvertTemplates(int portalId)
        {
            BBStoreController controller = new BBStoreController();
            ModuleController moduleController = new ModuleController();
            List<ModuleProductInfo> moPros = controller.GetModuleProducts(portalId);
            List<ProductTemplateInfo> templates = controller.GetProductTemplates(portalId);

            TemplateControl templateControl = LoadControl("Controls/TemplateControl.ascx") as TemplateControl;
            if (templateControl != null)
            {
                // ModuleProducts
                templateControl.Key = "SimpleProduct";
                foreach (ModuleProductInfo moPro in moPros)
                {
                    ModuleInfo module = moduleController.GetModule(moPro.ModuleId);
                    if (moPro.Template != String.Empty)
                    {
                        string name = "Module_" + moPro.ModuleId.ToString();
                        templateControl.SaveTemplate(moPro.Template, name, BBStore.TemplateControl.TemplateEnum.Neutral);
                        moduleController.UpdateModuleSetting(moPro.ModuleId,"Template",name);
                    }
                    else
                    {
                        ProductTemplateInfo templateInfo = (from t in templates where t.ProductTemplateId == moPro.ProductTemplateId select t).FirstOrDefault();
                        if (templateInfo != null)
                        {
                            string name = templateInfo.TemplateName;
                            templateControl.SaveTemplate(templateInfo.Template, name, BBStore.TemplateControl.TemplateEnum.Neutral);
                            moduleController.UpdateModuleSetting(moPro.ModuleId, "Template", name);
                            moduleController.UpdateModuleSetting(moPro.ModuleId, "ProductId", moPro.ProductId.ToString());
                            moduleController.UpdateModuleSetting(moPro.ModuleId, "ShowNetPrice", moPro.IsTaxIncluded.ToString());
                        }
                    }
                    controller.DeleteModuleProduct(PortalId, moPro.ModuleId);
                }
                    
                // ProductTemplates
                ArrayList allModules = moduleController.GetAllModules();
                foreach (ProductTemplateInfo template in templates)
                {
                    templateControl.Key = template.TemplateSource;
                    templateControl.SaveTemplate(template.Template, template.TemplateName, BBStore.TemplateControl.TemplateEnum.Neutral);

                    // We need to update the settings for all modules which have this ProductTemplateId as setting
                    int id = template.ProductTemplateId;
                    
                    foreach (ModuleInfo module in allModules)
                    {
                        Hashtable settings = moduleController.GetModuleSettings(module.ModuleID);
                        if (settings["ProductTemplateId"] != null && Convert.ToInt32(settings["ProductTemplateId"]) == id)
                        {
                            moduleController.UpdateModuleSetting(module.ModuleID,"Template", template.TemplateName);
                            moduleController.DeleteModuleSetting(module.ModuleID,"ProductTemplateId");
                        }
                        if (settings["ProductGroupTemplateId"] != null && Convert.ToInt32(settings["ProductGroupTemplateId"]) == id)
                        {
                            moduleController.UpdateModuleSetting(module.ModuleID, "Template", template.TemplateName);
                            moduleController.DeleteModuleSetting(module.ModuleID, "ProductGroupTemplateId");
                        }
                    }
                    controller.DeleteProductTemplate(template.ProductTemplateId);
                }
                // DynamicPage -> ProductModulePage for Product List Modules
                foreach (ModuleInfo module in allModules)
                {
                    Hashtable settings = moduleController.GetModuleSettings(module.ModuleID);
                    if (settings["DynamicPage"] != null && settings["CssHover"] == null)
                    {
                        moduleController.UpdateModuleSetting(module.ModuleID, "ProductModulePage", (string)settings["DynamicPage"]);
                        moduleController.DeleteModuleSetting(module.ModuleID, "DynamicPage");
                    }
                }

                // Mailtemplates
                string templateContent = "";
                string templateFile = this.PortalSettings.HomeDirectoryMapPath + @"Templates\order.html";
                if (File.Exists(templateFile))
                {
                    templateContent = File.ReadAllText(templateFile);
                    templateControl.Key = "Order";
                    templateControl.SaveTemplate(templateContent, "Order", BBStore.TemplateControl.TemplateEnum.Portal);
                    File.Delete(templateFile);
                }
                templateFile = MapPath(ControlPath + @"\Templates\order.html");
                if (File.Exists(templateFile))
                {
                    templateContent = File.ReadAllText(templateFile);
                    templateControl.Key = "Order";
                    templateControl.SaveTemplate(templateContent, "Order", BBStore.TemplateControl.TemplateEnum.Neutral);
                    File.Delete(templateFile);
                }
                templateFile = this.PortalSettings.HomeDirectoryMapPath + @"Templates\request.html";
                if (File.Exists(templateFile))
                {
                    templateContent = File.ReadAllText(templateFile);
                    templateControl.Key = "Request";
                    templateControl.SaveTemplate(templateContent, "Request", BBStore.TemplateControl.TemplateEnum.Portal);
                    File.Delete(templateFile);
                }
                templateFile = MapPath(ControlPath + @"\Templates\request.html");
                if (File.Exists(templateFile))
                {
                    templateContent = File.ReadAllText(templateFile);
                    templateControl.Key = "Request";
                    templateControl.SaveTemplate(templateContent, "Request", BBStore.TemplateControl.TemplateEnum.Neutral);
                    File.Delete(templateFile);
                }
            }
        }
    }
}