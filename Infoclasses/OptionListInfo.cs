using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    public class OptionListInfo
    {
        public OptionListInfo()
        {
        	OptionName = "";
        	OptionValue = "";
        	OptionDim = "";
	        OptionImage = null;
	        OptionDescription = "";
        	PriceAlteration = 0.00m;
        	IsMandatory = false;
        	IsDefault = false;
	        AskImage = false;
	        AskDescription = false;
            ShowDiff = false;
            Control = "";
            ControlProps = "";
        }

		public OptionListInfo(string opName, string opDim, string opValue, decimal opPrice, byte[] opImage, string opDesc, bool isMandatory, bool isDefault, bool askImage, bool askDesc, bool showDiff, string control, string controlProps)
        {
            OptionName = opName;
			OptionValue = opValue;
			OptionDim = opDim;
			OptionImage = opImage;
			OptionDescription = opDesc;
            PriceAlteration = opPrice;
            IsMandatory = isMandatory;
        	IsDefault = isDefault;
			AskImage = askImage;
			AskDescription = askDesc;
		    ShowDiff = showDiff;
		    Control = control;
		    ControlProps = controlProps;
        }

        public string OptionName { get; set; }
        public string OptionValue { get; set; }
		public string OptionDim { get; set; }
		public byte[] OptionImage { get; set; }
		public string OptionDescription { get; set; }
        public decimal PriceAlteration { get; set; }
        public bool IsMandatory { get; set; }
		public bool IsDefault { get; set; }
		public bool AskImage { get; set; }
		public bool AskDescription { get; set; }
        public bool ShowDiff { get; set; }
        public string Control { get; set; }
        public string ControlProps { get; set; }
    }
}
