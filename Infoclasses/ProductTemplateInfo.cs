using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    public class ProductTemplateInfo
    {
        public ProductTemplateInfo()
        {
            PortalId = 0;
            ProductTemplateId = 0;
            SubscriberId = -1;
            TemplateName = "";
            Template = "";
            TemplateSource = "";
        }
        public ProductTemplateInfo(int productTemplateId, int portalId,int subscriberId, string templateName, string template, string templateSource)
        {
            PortalId = portalId;
            ProductTemplateId = productTemplateId;
            SubscriberId = subscriberId;
            TemplateName = templateName;
            Template = template;
            TemplateSource = templateSource;
        }
        public int PortalId { get; set; }
        public int ProductTemplateId { get; set; }
        public int SubscriberId { get; set; }
        public string TemplateName { get; set; }
        public string Template { get; set; }
        public string TemplateSource { get; set; }
    }
}
