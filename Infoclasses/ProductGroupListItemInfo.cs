﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    [DataContract()]
    public class ProductGroupListItemInfo
    {
        public ProductGroupListItemInfo()
        {
            _status = 0;
        }

        [DataMember()]
        public int ProductGroupId { get; set; }
        [DataMember()]
        public int FeatureListItemId { get; set; }
        [DataMember()]
        public int _status { get; set; }
    }
}