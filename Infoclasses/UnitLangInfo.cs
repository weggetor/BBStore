using System;
using System.Runtime.Serialization;

namespace Bitboxx.DNNModules.BBStore
{
    [Serializable]
    [DataContract()]
    public class UnitLangInfo: ILanguageEditorInfo
    {
        public UnitLangInfo()
        {
            UnitId = 0;
            Language = "";
            Unit = "";
            Symbol = "";
        }
        [DataMember()]
        public Int32 UnitId { get; set; }
        [DataMember()]
        public string Language { get; set; }
        [DataMember()]
        [LanguageEditorAttribute("TextBox", Width = "200px", MaxLength = 20)]
        public string Unit { get; set; }
        [DataMember()]
        [LanguageEditorAttribute("TextBox", Width = "100px", MaxLength = 10)]
        public string Symbol { get; set; }
    }
}