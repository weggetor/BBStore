using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    [Serializable]
    [DataContract()]
    public class OrderStateLangInfo
    {
        public OrderStateLangInfo()
        {
            OrderStateId = -1;
            OrderState = "";
            Language = "";
        }
        [DataMember()]
        public int OrderStateId { get; set; }
        [DataMember()]
        public string Language { get; set; }
        [DataMember()]
        public string OrderState { get; set; }
    }
}


