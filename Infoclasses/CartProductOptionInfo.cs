using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    [Serializable]
	public class CartProductOptionInfo
    {
        public CartProductOptionInfo()
        {
            CartProductOptionId = 0;
            CartProductId = 0;
            OptionId = 0;
            OptionName = "";
            OptionValue = "";
			OptionDescription = "";
            PriceAlteration = 0.00M;
        }
        public int CartProductOptionId { get; set; }
        public int CartProductId { get; set; }
        public int OptionId { get; set; }
        public string OptionName { get; set; }
        public string OptionValue { get; set; }
		public byte[] OptionImage { get; set; }
		public string OptionDescription { get; set; }
        public decimal PriceAlteration { get; set; }
    }
}
