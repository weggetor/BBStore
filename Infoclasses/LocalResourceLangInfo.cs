using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Bitboxx.DNNModules.BBStore
{
	[Serializable]
	[DataContract()]
	public class LocalResourceLangInfo:ILanguageEditorInfo
	{
        public LocalResourceLangInfo()
        {
            LocalResourceId = 0;
            Language = "";
            TextValue = "";
        }
		[DataMember()]
		public Int32 LocalResourceId { get; set; }
		[DataMember()]
		public string Language { get; set; }
		[DataMember()]
        [LanguageEditorAttribute("TextEditor", Width = "300px", Height="400px")]
		public string TextValue { get; set; }
	}

}