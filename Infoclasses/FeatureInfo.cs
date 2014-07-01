using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Bitboxx.DNNModules.BBStore
{
	[Serializable]
	[DataContract()]
	public class FeatureInfo
	{
		public FeatureInfo()
		{
			FeatureId = 0;
			PortalID = 0;
			FeatureGroupId = 0;
			FeatureListId = 0;
			Datatype = "";
			Multiselect = false;
			Control = "";
			Dimension = 0;
			Required = false;
			MinValue = "";
			MaxValue = "";
			RegEx = "";
			RoleID = 0;
			ShowInSearch = false;
			SearchGroups = "";
			Feature = "";
			Unit = "";
			FeatureToken = "";
			ViewOrder = 0;
		}
		[DataMember()]
		public Int32 FeatureId { get; set; }
		[DataMember()]
		public Int32 PortalID { get; set; }
		[DataMember()]
		public Int32 FeatureGroupId { get; set; }
		[DataMember()]
		public Int32 FeatureListId { get; set; }
		[DataMember()]
		public string Datatype { get; set; }
		[DataMember()]
		public Boolean Multiselect { get; set; }
		[DataMember()]
		public string Control { get; set; }
		[DataMember()]
		public Int32 Dimension { get; set; }
		[DataMember()]
		public Boolean Required { get; set; }
		[DataMember()]
		public string MinValue { get; set; }
		[DataMember()]
		public string MaxValue { get; set; }
		[DataMember()]
		public string RegEx { get; set; }
		[DataMember()]
		public Int32 RoleID { get; set; }
		[DataMember()]
		public Boolean ShowInSearch { get; set; }
		[DataMember()]
		public string SearchGroups { get; set; }
		[DataMember()]
		public string Feature { get; set; }
		[DataMember()]
		public string Unit { get; set; }
		[DataMember()]
		public string FeatureToken { get; set; }
		[DataMember()]
		public int ViewOrder { get; set; }
	}

}