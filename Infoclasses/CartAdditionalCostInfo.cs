using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    [Serializable]
	public class CartAdditionalCostInfo
    {
        public CartAdditionalCostInfo()
        {
            CartAdditionalCostId = 0;
            Quantity = 0;
            Name = "";
            Description = "";
            Area = "";
            UnitCost = 0.00M;
            NetTotal = 0.00M;
            TaxPercent = 0.0M;
            TaxTotal = 0.00M;
            SubTotal = 0.00M;
        }
        public int CartAdditionalCostId { get; set; }
        public Guid CartId { get; set; }
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
