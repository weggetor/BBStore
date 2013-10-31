using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Bitboxx.DNNModules.BBStore
{
    [Serializable()]
	[DataContract()]
    public class ProductGroupLangInfo:ILanguageEditorInfo
    {
        public ProductGroupLangInfo()
        {
            ProductGroupId = -1;
            Language = "";
            ProductGroupName = "";
            ProductGroupDescription = "";
            ProductGroupShortDescription = "";
        }

		[DataMember()]
		public int ProductGroupId { get; set; }
		[DataMember()]
        public string Language { get; set; }
		[DataMember()]
        [LanguageEditorAttribute("TextBox", Width = "380px", MaxLength = 120)]
        public string ProductGroupName { get; set; }
		[DataMember()]
        [LanguageEditorAttribute("TextBox", Width = "380px", Rows = 6, MaxLength = 500)]
        public string ProductGroupShortDescription { get; set; }
		[DataMember()]
        [LanguageEditorAttribute("TextEditor", Width = "380px", Height = "400px")]
        public string ProductGroupDescription { get; set; }
    }
}