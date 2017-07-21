using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    [Serializable()]
    [DataContract()]
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
            _status = 0;
        }
        [DataMember()]
        public int SubscriberAddressTypeId { get; set; }
        [DataMember()]
        public int PortalId { get; set; }
        [DataMember()]
        public int SubscriberId { get; set; }
        [DataMember()]
        public string KzAddressType { get; set; }
        [DataMember()]
        public bool Mandatory { get; set; }
        [DataMember()]
        public string AddressType { get; set; }
        [DataMember()]
        public int ViewOrder { get; set; }
        [DataMember()]
        public bool IsOrderAddress { get; set; }
        [DataMember()]
        public int _status { get; set; }
    }
}
