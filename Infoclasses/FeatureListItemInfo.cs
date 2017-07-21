using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Bitboxx.DNNModules.BBStore
{
	[Serializable]
	[DataContract()]
	public class FeatureListItemInfo
	{
		public FeatureListItemInfo()
		{
			FeatureListItemId = 0;
			FeatureListId = 0;
			FeatureListItem = "";
			Image = "";
			ViewOrder = 0;
		    _status = 0;
		}
		[DataMember()]
		public Int32 FeatureListItemId { get; set; }
		[DataMember()]
		public Int32 FeatureListId { get; set; }
		[DataMember()]
		public string FeatureListItem { get; set; }
		[DataMember()]
		public string Image { get; set; }
		[DataMember()]
		public int ViewOrder { get; set; }
        [DataMember()]
        public int _status { get; set; }

    }

}