using System;
using System.Collections.Generic;
using System.Web;
using System.Globalization;
using System.Runtime.Serialization;

namespace Bitboxx.DNNModules.BBStore
{
    [Serializable]
	[DataContract()]
    public class SimpleProductInfo
    {
        public SimpleProductInfo()
        {
            SimpleProductId = -1;
            SubscriberId = -1;
			SupplierId = -1;
            PortalId = -1;
            Image = "";
            ItemNo = "";
            Name = "";
            ShortDescription = "";
            ProductDescription = "";
            UnitCost = 0.000m;
			OriginalUnitCost = 0.000m;
			HideCost = false;
            TaxPercent = 0.0m;
            UnitId = -1;
            Attributes = "";
            CreatedOnDate = DateTime.Parse("01.01.1900 00:00:00", CultureInfo.CreateSpecificCulture("de-DE"));
            CreatedByUserId = -1;
            LastModifiedByUserId = -1;
            LastModifiedOnDate = CreatedOnDate;
			Disabled = false;
        	NoCart = false;
        	SortNo = 0;
            Weight = 0.000m;
        }
        [DataMember()]
		public int SimpleProductId { get; set; }
		[DataMember()]
		public int SubscriberId { get; set; }
		[DataMember()]
		public int SupplierId { get; set; }
		[DataMember()]
		public int PortalId { get; set; }
		[DataMember()]
		public string Image { get; set; }
		[DataMember()]
		public string ItemNo { get; set; }
		[DataMember()]
		public string Name { get; set; }
		[DataMember()]
		public string ShortDescription { get; set; }
		[DataMember()]
		public string ProductDescription { get; set; }
		[DataMember()]
		public decimal UnitCost { get; set; }
		[DataMember()]
		public decimal OriginalUnitCost { get; set; }
		[DataMember()]
		public bool HideCost { get; set; }
		[DataMember()]
		public decimal TaxPercent { get; set; }
        [DataMember()]
        public int UnitId { get; set; }
		[DataMember()]
		public string Attributes { get; set; }
		[DataMember()]
		public DateTime CreatedOnDate { get; set; }
		[DataMember()]
		public int CreatedByUserId { get; set; }
		[DataMember()]
		public DateTime LastModifiedOnDate { get; set; }
		[DataMember()]
		public int LastModifiedByUserId { get; set; }
		[DataMember()]
		public bool Disabled { get; set; }
		[DataMember()]
		public bool NoCart { get; set; }
		[DataMember()]
		public int SortNo { get; set; }
        [DataMember()]
        public decimal Weight { get; set; }
    }
}
