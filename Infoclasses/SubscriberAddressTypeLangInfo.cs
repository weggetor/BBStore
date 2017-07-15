using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    [Serializable()]
    [DataContract()]
    public class SubscriberAddressTypeLangInfo
    {
		public SubscriberAddressTypeLangInfo()
        {
			SubscriberAddressTypeId = -1;
            Language = "";
			AddressType = "";
        }
        [DataMember()]
        public int SubscriberAddressTypeId { get; set; }
        [DataMember()]
        public string Language { get; set; }
        [DataMember()]
        public string AddressType { get; set; }
    }
}
