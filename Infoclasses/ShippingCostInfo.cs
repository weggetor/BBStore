using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    [Serializable]
    [DataContract()]
    public class ShippingCostInfo
    {
        public ShippingCostInfo()
        {
            ShippingCostID = 0;
            ShippingModelID = 0;
            ShippingZoneID = 0;
            ShippingPrice = 0.00m;
            PerPart = false;
            MinWeight = 0.00m;
            MaxWeight = 0.00m;
            MinPrice = 0.00m;
            MaxPrice = 0.00m;
        }
        [DataMember()]
        public Int32 ShippingCostID { get; set; }
        [DataMember()]
        public Int32 ShippingModelID { get; set; }
        [DataMember()]
        public Int32 ShippingZoneID { get; set; }
        [DataMember()]
        public Decimal ShippingPrice { get; set; }
        [DataMember()]
        public Boolean FlatCharge { get; set; }
        [DataMember()]
        public Boolean PerPart { get; set; }
        [DataMember()]
        public Decimal MinWeight { get; set; }
        [DataMember()]
        public Decimal MaxWeight { get; set; }
        [DataMember()]
        public Decimal MinPrice { get; set; }
        [DataMember()]
        public Decimal MaxPrice { get; set; }
    }

}