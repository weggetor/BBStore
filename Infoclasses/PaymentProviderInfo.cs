using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    [Serializable]
    public class PaymentProviderInfo
    {
        public PaymentProviderInfo()
        {
            PaymentProviderId = -1;
            ProviderLogo = "";
            ProviderTag = "";
            ProviderName = "";
            ProviderControl = "";
        }
        
        public int PaymentProviderId { get; set; }
        public string ProviderLogo { get; set; }
        public string ProviderTag { get; set; }
        public string ProviderControl { get; set; }
        public string ProviderName { get; set; }
    }
}
