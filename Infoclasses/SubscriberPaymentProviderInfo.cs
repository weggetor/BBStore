using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    [Serializable]
    public class SubscriberPaymentProviderInfo
    {
        public SubscriberPaymentProviderInfo()
        {
            SubscriberPaymentProviderId = -1;
            PortalId = 0;
            SubscriberId = 0;
            PaymentProviderId = 0;
            ViewOrder = 1;
            PaymentProviderProperties = "";
            IsEnabled = false;
        	TaxPercent = 0.00m;
            CostPercent = 0.0m;
        }
        public int SubscriberPaymentProviderId { get; set; }
        public int PortalId { get; set; }
        public int SubscriberId { get; set; }
        public int PaymentProviderId { get; set; }
        public int ViewOrder { get; set; }
        public string PaymentProviderProperties { get; set; }
        public decimal Cost { get; set; }
        public decimal CostPercent { get; set; }
        public decimal TaxPercent { get; set; }
        public Boolean IsEnabled { get; set; }
        public string Role { get; set; }

    }
    public class SubscriberPaymentProviderComparer : IComparer<SubscriberPaymentProviderInfo>
    {
        public int Compare(SubscriberPaymentProviderInfo x, SubscriberPaymentProviderInfo y)
        {
            if (x == null)
            {
                if (y == null) // If x is null and y is null, they're equal.
                    return 0;
                else  // If x is null and y is not null, y is greater. 
                    return -1;
            }
            else // If x is not null...
            {
                if (y == null) // ...and y is null, x is greater.
                    return 1;
                else // Compare the ViewOrder
                    return x.ViewOrder.CompareTo(y.ViewOrder);
            }
        }
    }

}
