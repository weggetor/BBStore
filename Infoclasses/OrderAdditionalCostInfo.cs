using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
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
        }
        public int OrderAdditionalCostId { get; set; }
        public int OrderId { get; set; }
        public decimal Quantity { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Area { get; set; }
        public decimal UnitCost { get; set; }
        public decimal NetTotal { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TaxTotal { get; set; }
        public decimal SubTotal { get; set; }
        
    }
}
