using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    public class OrderHistoryInfo
    {
        public OrderHistoryInfo()
        {
            OrderHistoryId = 0;
            OrderStateId = 0;
            OrderId = 0;
            CreatedOnDate = DateTime.MinValue;
            HistoryText = "";
        }
        public int OrderHistoryId { get; set; }
        public int OrderStateId { get; set; }
        public int OrderId { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string HistoryText { get; set; }
    }
}
