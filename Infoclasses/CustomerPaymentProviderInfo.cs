using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    public class CustomerPaymentProviderInfo
    {
        public CustomerPaymentProviderInfo()
        {
            CustomerPaymentProviderId = -1;
            CustomerId = 0;
            PaymentProviderId = 0;
            PaymentProviderValues = "";
        }
        public int CustomerPaymentProviderId { get; set; }
        public int CustomerId { get; set; }
        public int PaymentProviderId { get; set; }
        public string PaymentProviderValues { get; set; }
    }
}
