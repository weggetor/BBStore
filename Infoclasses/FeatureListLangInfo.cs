using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Bitboxx.DNNModules.BBStore
{
	[Serializable]
	[DataContract()]
	public class FeatureListLangInfo:ILanguageEditorInfo
	{
		public FeatureListLangInfo()
		{
			FeatureListId = 0;
			Language = "";
			FeatureList = "";
		}
		[DataMember()]
		public Int32 FeatureListId { get; set; }
		[DataMember()]
		public string Language { get; set; }
		[DataMember()]
        [LanguageEditorAttribute("TextBox", Width = "200px", MaxLength = 40)]
		public string FeatureList { get; set; }
	}
}