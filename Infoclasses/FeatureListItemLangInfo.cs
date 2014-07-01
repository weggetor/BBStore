using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Bitboxx.DNNModules.BBStore
{
	[Serializable]
	[DataContract()]
	public class FeatureListItemLangInfo:ILanguageEditorInfo
	{
		public FeatureListItemLangInfo()
		{
			FeatureListItemId = 0;
			Language = "";
			FeatureListItem = "";
		}
		[DataMember()]
		public Int32 FeatureListItemId { get; set; }
		[DataMember()]
		public string Language { get; set; }
		[DataMember()]
        [LanguageEditorAttribute("TextBox", Width = "200px", MaxLength = 60)]
		public string FeatureListItem { get; set; }
	}

}