using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    [Serializable]
    public class OrderStatsInfo
    {
        public decimal Amount { get; set; }
        public decimal Sum { get; set; }
        public string Product { get; set; }
    }
}
