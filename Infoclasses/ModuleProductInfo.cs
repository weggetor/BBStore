using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    public class ModuleProductInfo
    {
        public ModuleProductInfo()
        {
            PortalId = 0;
            ModuleId = 0;
            ProductId = -1;
            ProductTemplateId = -1;
            Template = "";
            IsTaxIncluded = -1;
        }
        public int PortalId { get; set; }
        public int ModuleId { get; set; }
        public int ProductId { get; set; }
        public int ProductTemplateId { get; set; }
        public string Template { get; set; }
        public int IsTaxIncluded { get; set; }
    }
}
