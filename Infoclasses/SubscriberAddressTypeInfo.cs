using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    public class SubscriberAddressTypeInfo
    {
		public SubscriberAddressTypeInfo()
        {
			SubscriberAddressTypeId = -1;
            PortalId = 0;
            SubscriberId = 0;
			KzAddressType = "";
			Mandatory = false;
			AddressType = "";
			ViewOrder = 0;
		    IsOrderAddress = false;
        }
        public int SubscriberAddressTypeId { get; set; }
        public int PortalId { get; set; }
        public int SubscriberId { get; set; }
        public string KzAddressType { get; set; }
        public bool Mandatory { get; set; }
        public string AddressType { get; set; }
		public int ViewOrder { get; set; }
        public bool IsOrderAddress { get; set; }
    }
}
