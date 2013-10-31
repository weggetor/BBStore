using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Bitboxx.DNNModules.BBStore
{
	[Serializable]
	[DataContract()]
	public class FeatureListInfo
	{
		public FeatureListInfo()
		{
			FeatureListId = 0;
			PortalID = 0;
			FeatureList = "";
		}
		[DataMember()]
		public Int32 FeatureListId { get; set; }
		[DataMember()]
		public Int32 PortalID { get; set; }
		[DataMember()]
		public string FeatureList { get; set; }
	}
}