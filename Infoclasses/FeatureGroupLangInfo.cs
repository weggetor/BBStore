using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Bitboxx.DNNModules.BBStore
{
	[Serializable]
	[DataContract()]
	public class FeatureGroupLangInfo
	{
		public FeatureGroupLangInfo()
		{
			FeatureGroupId = 0;
			Language = "";
			FeatureGroup = "";
		}
		[DataMember()]
		public Int32 FeatureGroupId { get; set; }
		[DataMember()]
		public string Language { get; set; }
		[DataMember()]
		public string FeatureGroup { get; set; }
	}

}