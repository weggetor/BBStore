using System;
using System.Runtime.Serialization;

namespace Bitboxx.DNNModules.BBStore
{
	[Serializable()]
	[DataContract()]
	public class SimpleProductLangInfo:ILanguageEditorInfo
    {
        public SimpleProductLangInfo()
        {
            SimpleProductId = -1;
            Language = "";
            ShortDescription = "";
            ProductDescription = "";
            Attributes = "";
            Name = "";
        }

        [DataMember()]
		public int SimpleProductId { get; set; }
		[DataMember()]
		public string Language { get; set; }
        [DataMember()]
        [LanguageEditorAttribute("TextBox", Width = "500px", MaxLength = 60)]
        public string Name { get; set; }
		[DataMember()]
        [LanguageEditorAttribute("TextBox", Width = "500px", Rows=6, MaxLength = 500)]
		public string ShortDescription { get; set; }
		[DataMember()]
        [LanguageEditorAttribute("TextEditor", Width = "500px", Height = "600px")]
        public string ProductDescription { get; set; }
		[DataMember()]
        [LanguageEditorAttribute("TextBox", Width = "500px", Rows = 6, MaxLength = 500)]
        public string Attributes { get; set; }
		
    }
}
