using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    [Serializable]
    [DataContract()]
    public class ProductInGroupInfo
    {
        [DataMember()]
        public int SimpleProductId { get; set; }
        [DataMember()]
        public int ProductGroupId { get; set; }
    }
}