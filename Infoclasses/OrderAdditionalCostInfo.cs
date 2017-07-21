using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    [Serializable]
    [DataContract()]
    public class OrderAdditionalCostInfo
    {
        public OrderAdditionalCostInfo()
        {
            OrderAdditionalCostId = 0;
            Quantity = 0.0m;
            Name = "";
            Description = "";
            Area = "";
            UnitCost = 0.00M;
            NetTotal = 0.00M;
            TaxPercent = 0.0M;
            TaxTotal = 0.00M;
            SubTotal = 0.00M;
            _status = 0;
        }
        [DataMember()]
        public int OrderAdditionalCostId { get; set; }
        [DataMember()]
        public int OrderId { get; set; }
        [DataMember()]
        public decimal Quantity { get; set; }
        [DataMember()]
        public string Name { get; set; }
        [DataMember()]
        public string Description { get; set; }
        [DataMember()]
        public string Area { get; set; }
        [DataMember()]
        public decimal UnitCost { get; set; }
        [DataMember()]
        public decimal NetTotal { get; set; }
        [DataMember()]
        public decimal TaxPercent { get; set; }
        [DataMember()]
        public decimal TaxTotal { get; set; }
        [DataMember()]
        public decimal SubTotal { get; set; }
        [DataMember()]
        public int _status { get; set; }

    }
}
