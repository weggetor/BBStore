using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    [Serializable]
    public class OrderListInfo
    {
        public OrderListInfo()
        {
            OrderID = -1;
            PortalId = 0;
            SubscriberID = 0;
            OrderNo = "";
            OrderTime = DateTime.MinValue;
            OrderState = "";
            CustomerID = -1;
        	FirstName = "";
        	LastName = "";
        	Company = "";
        	Street = "";
        	Postalcode = "";
        	City = "";
        	CountryCode = "";
            Comment = "";
            Currency = "EUR";
            PaymentProviderId = -1;
            PaymentProviderValues = "";
        	PaymentProvider = "";
            Total = 0.00M;
        }

        public int OrderID { get; set; }
        public int PortalId { get; set; }
        public int SubscriberID { get; set; }
        public string OrderNo { get; set; }
        public DateTime OrderTime { get; set; }
        public string OrderState { get; set; }
        public int CustomerID { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Company { get; set; }
		public string Street { get; set; }
		public string Postalcode { get; set; }
		public string City { get; set; }
		public string CountryCode { get; set; }
		public string Comment { get; set; }
        public string Currency { get; set; }
        public int PaymentProviderId { get; set; }
        public string PaymentProviderValues { get; set; }
		public string PaymentProvider { get; set; }
        public decimal Total { get; set; }
    }
}
