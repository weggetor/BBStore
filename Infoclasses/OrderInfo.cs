using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    [Serializable]
    public class OrderInfo
    {
        public OrderInfo()
        {
            OrderID = -1;
            PortalId = 0;
            SubscriberID = 0;
            OrderNo = "";
            OrderTime = DateTime.MinValue;
	        OrderName = "";
            OrderStateId = 0;
            CustomerID = -1;
            Comment = "";
            Currency = "EUR";
            PaymentProviderId = -1;
            PaymentProviderValues = "";
            OrderTotal = 0.00M;
            OrderTax = 0.00M;
            AdditionalTax = 0.00M;
            AdditionalTotal = 0.00M;
            Total = 0.00M;
        	AttachName = "";
        	AttachContentType = "";
        }

        public int OrderID { get; set; }
        public int PortalId { get; set; }
        public int SubscriberID { get; set; }
        public string OrderNo { get; set; }
        public DateTime OrderTime { get; set; }
	    public string OrderName { get; set; }
	    public int OrderStateId { get; set; }
        public int CustomerID { get; set; }
        public string Comment { get; set; }
        public string Currency { get; set; }
        public int PaymentProviderId { get; set; }
        public string PaymentProviderValues { get; set; }
        public decimal OrderTotal { get; set; }
        public decimal OrderTax { get; set; }
        public decimal AdditionalTotal { get; set; }
        public decimal AdditionalTax { get; set; }
        public decimal Total { get; set; }
    	public byte[] Attachment { get; set; }
		public string AttachName { get; set; }
		public string AttachContentType { get; set; }

    }
}
