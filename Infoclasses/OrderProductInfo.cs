using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    public class OrderProductInfo
    {
        public OrderProductInfo()
        {
            OrderProductId = 0;
            ProductId = 0;
            Image = "";
            Unit = "";
            ItemNo = "";
            Quantity = 0.0m;
            Name = "";
            Description = "";
            UnitCost = 0.00M;
            NetTotal = 0.00M;
            TaxPercent = 0.0M;
            TaxTotal = 0.00M;
            SubTotal = 0.00M;

        }

        public int OrderProductId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string Image { get; set; }
        public string Unit { get; set; }
        public string ItemNo { get; set; }
        public decimal Quantity { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductUrl { get; set; }
        public decimal UnitCost { get; set; }
        public decimal NetTotal { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TaxTotal { get; set; }
        public decimal SubTotal { get; set; }
    }
}
