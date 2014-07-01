using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    public class LanguageEditorAttribute : System.Attribute
    {
        public string Control;
        public string Width;
        public string Height;
        public int Rows;
        public int MaxLength;
        public string Label;

        public LanguageEditorAttribute(string control)
        {
            this.Control = control;
            Width = "300px";
            Height = "20px";
            Rows = 1;
            MaxLength = 120;
            Label = "";
        }
    }
}