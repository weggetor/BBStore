using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    [Serializable]
    [DataContract()]
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
            TaxId = "";
        	AttachName = "";
        	AttachContentType = "";
            _status = 0;
        }

        [DataMember()]
        public int OrderID { get; set; }
        [DataMember()]
        public int PortalId { get; set; }
        [DataMember()]
        public int SubscriberID { get; set; }
        [DataMember()]
        public string OrderNo { get; set; }
        [DataMember()]
        public DateTime OrderTime { get; set; }
        [DataMember()]
        public string OrderName { get; set; }
        [DataMember()]
        public int OrderStateId { get; set; }
        [DataMember()]
        public int CustomerID { get; set; }
        [DataMember()]
        public string Comment { get; set; }
        [DataMember()]
        public string Currency { get; set; }
        [DataMember()]
        public int PaymentProviderId { get; set; }
        [DataMember()]
        public string PaymentProviderValues { get; set; }
        [DataMember()]
        public decimal OrderTotal { get; set; }
        [DataMember()]
        public decimal OrderTax { get; set; }
        [DataMember()]
        public decimal AdditionalTotal { get; set; }
        [DataMember()]
        public decimal AdditionalTax { get; set; }
        [DataMember()]
        public decimal Total { get; set; }
        [DataMember()]
        public string TaxId { get; set; }
        [DataMember()]
        public byte[] Attachment { get; set; }
        [DataMember()]
        public string AttachName { get; set; }
        [DataMember()]
        public string AttachContentType { get; set; }
        [DataMember()]
        public int _status { get; set; }

    }
}
