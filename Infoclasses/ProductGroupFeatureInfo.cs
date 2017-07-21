using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
	[Serializable]
    [DataContract()]
    public class ProductGroupFeatureInfo
	{
        public ProductGroupFeatureInfo()
        {
            _status = 0;
        }

        [DataMember()]
        public int FeatureId { get; set; }
        [DataMember()]
        public int ProductGroupId { get; set; }
        [DataMember()]
        public int _status { get; set; }
    }
}