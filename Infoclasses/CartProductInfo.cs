using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    [Serializable]
	public class CartProductInfo
    {
        public CartProductInfo()
        {
            CartProductId = 0;
            ProductId = 0;
            Image = "";
            ItemNo = "";
            Quantity = 0.0m;
            Unit = "";
            Decimals = 0;
            Name = "";
            ProductUrl = "";
            Description = "";
            UnitCost = 0.00M;
            NetTotal = 0.00M;
            TaxPercent = 0.0M;
            TaxTotal = 0.00M;
            SubTotal = 0.00M;
            ProductDiscount = "";
            Weight = 0.000m;
            ShippingModelId = -1;
			CartProductOptions = new List<CartProductOptionInfo>();
        }

        public int CartProductId { get; set; }
        public Guid CartId { get; set; }
        public int ProductId { get; set; }
        public string Image { get; set; }
        public string ItemNo { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public int Decimals { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductUrl { get; set; }
        public decimal UnitCost { get; set; }
        public decimal NetTotal { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TaxTotal { get; set; }
        public decimal SubTotal { get; set; }
        public string ProductDiscount { get; set; }
        public decimal Weight { get; set; }
        public int ShippingModelId { get; set; }
		public List<CartProductOptionInfo> CartProductOptions { get; set; }
    }
}
