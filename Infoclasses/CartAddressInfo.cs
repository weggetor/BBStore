using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
	[Serializable]
	public class CartAddressInfo
	{
		public CartAddressInfo()
		{
			CartID = new Guid();
			CustomerAddressId = -1;
			SubscriberAddressTypeId = -1;
		}

		public Guid CartID { get; set; }
		public int CustomerAddressId { get; set; }
		public int SubscriberAddressTypeId { get; set; }
		public CustomerAddressInfo CustomerAddress { get; set; }
	}
}