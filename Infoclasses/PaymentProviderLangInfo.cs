using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    public class PaymentProviderLangInfo
    {
        public PaymentProviderLangInfo()
        {
            PaymentProviderId = -1;
            Language = "";
            ProviderName = "";
        }
        
        public int PaymentProviderId { get; set; }
        public string Language { get; set; }
        public string ProviderName { get; set; }
    }
}
