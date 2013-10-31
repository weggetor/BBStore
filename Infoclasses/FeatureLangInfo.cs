using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Bitboxx.DNNModules.BBStore
{
	[Serializable]
	[DataContract()]
	public class FeatureLangInfo
	{
		public FeatureLangInfo()
		{
			FeatureId = 0;
			Language = "";
			Feature = "";
			Unit = "";
		}
		[DataMember()]
		public Int32 FeatureId { get; set; }
		[DataMember()]
		public string Language { get; set; }
		[DataMember()]
		public string Feature { get; set; }
		[DataMember()]
		public string Unit { get; set; }
	}

}