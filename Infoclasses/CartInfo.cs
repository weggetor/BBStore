using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
	[Serializable]
	public class CartInfo
	{
		public CartInfo()
		{
		    StoreGuid = BBStoreController.StoreGuid;
            CartID = new Guid();
			SubscriberID = 0;
			CustomerID = -1;
			CustomerPaymentProviderID = -1;
			CartName = "";
			Comment = "";
			Currency = "EUR";
			OrderTotal = 0.00M;
			OrderTax = 0.00M;
			AdditionalTax = 0.00M;
			AdditionalTotal = 0.00M;
			Total = 0.00M;
			AttachName = "";
			AttachContentType = "";
			CartAddresses = new List<CartAddressInfo>();
			CartAdditionalCosts = new List<CartAdditionalCostInfo>();
			CartProducts = new List<CartProductInfo>();
		}

	    public Guid StoreGuid { get; set; }
        public Guid CartID { get; set; }
		public int SubscriberID { get; set; }
		public int CustomerID { get; set; }
		public int CustomerPaymentProviderID { get; set; }
		public string CartName { get; set; }
		public string Comment { get; set; }
		public string Currency { get; set; }
		public decimal OrderTotal { get; set; }
		public decimal OrderTax { get; set; }
		public decimal AdditionalTotal { get; set; }
		public decimal AdditionalTax { get; set; }
		public decimal Total { get; set; }
		public byte[] Attachment { get; set; }
		public string AttachName { get; set; }
		public string AttachContentType { get; set; }
		public List<CartAddressInfo> CartAddresses { get; set; }
		public List<CartAdditionalCostInfo> CartAdditionalCosts { get; set; }
		public List<CartProductInfo> CartProducts { get; set; }
        public DateTime CreatedOnDate { get; set; }
	}
}

