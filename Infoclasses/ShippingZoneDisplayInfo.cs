using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    public class ShippingZoneDisplayInfo
    {
        public ShippingZoneDisplayInfo()
        {
            ShippingZoneID = 0;
            ShippingModelID = 0;
            Name = "";
            Description = "";
            OrderText = "";
            ExemptionLimit = -1m;
        }

        public Int32 ShippingZoneID { get; set; }
        public Int32 ShippingModelID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string OrderText { get; set; }
        public decimal ExemptionLimit { get; set; }
    }
}