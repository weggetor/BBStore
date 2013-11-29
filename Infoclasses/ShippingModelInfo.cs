using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    [Serializable]
    public class ShippingModelInfo
    {
        public int ShippingModelID { get; set; }
        public int PortalId { get; set; }
        public string Name { get; set; }
        public decimal Tax { get; set; }
        public bool Enabled { get; set; }
    }
}