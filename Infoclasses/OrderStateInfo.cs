using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    [Serializable]
    public class OrderStateInfo
    {
        public OrderStateInfo()
        {
            OrderStateId = -1;
            OrderState = "";
            OrderAction = "";
            PortalId = -1;
        }

        public int OrderStateId { get; set; }
        public string OrderState { get; set; }
        public string OrderAction { get; set; }
        public int PortalId { get; set; }
    }
}
