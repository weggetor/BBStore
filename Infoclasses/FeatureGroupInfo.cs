using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Bitboxx.DNNModules.BBStore
{
	[Serializable]
	[DataContract()]
	public class FeatureGroupInfo
	{
		public FeatureGroupInfo()
		{
			FeatureGroupId = 0;
			PortalID = 0;
			FeatureGroup = "";
			ViewOrder = 0;
		}
		[DataMember()]
		public Int32 FeatureGroupId { get; set; }
		[DataMember()]
		public Int32 PortalID { get; set; }
		[DataMember()]
		public string FeatureGroup { get; set; }
		[DataMember()]
		public Int32 ViewOrder { get; set; }
	}

}