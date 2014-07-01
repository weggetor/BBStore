using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    public class SubscriberAddressTypeLangInfo
    {
		public SubscriberAddressTypeLangInfo()
        {
			SubscriberAddressTypeId = -1;
            Language = "";
			AddressType = "";
        }
        public int SubscriberAddressTypeId { get; set; }
        public string Language { get; set; }
        public string AddressType { get; set; }
    }
}
