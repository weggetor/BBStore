using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    public class TaxInfo
    {
        public TaxInfo()
        {
            TaxPercent = 0.0m;
            TaxTotal = 0.00m;
        }
        public decimal TaxPercent { get; set; }
        public decimal TaxTotal { get; set; }
    }
}
