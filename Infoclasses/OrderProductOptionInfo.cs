using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    public class OrderProductOptionInfo
    {
        public OrderProductOptionInfo()
        {
            OrderProductOptionId = 0;
            OrderProductId = 0;
            OptionId = 0;
            OptionName = "";
            OptionValue = "";
	        OptionDescription = "";
            PriceAlteration = 0.00M;
        }
        public int OrderProductOptionId { get; set; }
        public int OrderProductId { get; set; }
        public int OptionId { get; set; }
        public string OptionName { get; set; }
        public string OptionValue { get; set; }
		public byte[] OptionImage { get; set; }
		public string OptionDescription { get; set; }
        public decimal PriceAlteration { get; set; }
    }
}
