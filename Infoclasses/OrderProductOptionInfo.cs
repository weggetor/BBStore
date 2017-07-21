using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    [Serializable]
    [DataContract()]
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
            _status = 0;
        }
        [DataMember()]
        public int OrderProductOptionId { get; set; }
        [DataMember()]
        public int OrderProductId { get; set; }
        [DataMember()]
        public int OptionId { get; set; }
        [DataMember()]
        public string OptionName { get; set; }
        [DataMember()]
        public string OptionValue { get; set; }
        [DataMember()]
        public byte[] OptionImage { get; set; }
        [DataMember()]
        public string OptionDescription { get; set; }
        [DataMember()]
        public decimal PriceAlteration { get; set; }
        [DataMember()]
        public int _status { get; set; }
    }
}
