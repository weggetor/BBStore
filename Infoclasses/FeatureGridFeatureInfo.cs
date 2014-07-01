using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Common.Utilities;

namespace Bitboxx.DNNModules.BBStore
{
	public class FeatureGridFeatureInfo:IHydratable
	{
		public FeatureGridFeatureInfo()
		{
			FeatureGroupId = -1;
			FeatureGroup = "";
			FeatureId = -1;
			Feature = "";
			Unit = "";
			Datatype = "";
			Control = "";
			Dimension = 0;
			Multiselect = false;
			FeatureListId = -1;
			MinValue = "";
			MaxValue = "";
			RegEx = "";
		}
		
		public int FeatureGroupId {get; set;}
		public string FeatureGroup {get; set;}
		public int FeatureId {get; set;}
		public string Feature {get; set;}
		public string Unit { get; set; }
		public string Datatype {get; set;}
		public string Control {get; set;}
		public int Dimension { get; set; }
		public bool Required { get; set; }
		public bool Multiselect {get; set;}
		public int? FeatureListId { get; set; }
		public string MinValue { get; set; }
		public string MaxValue { get; set; }
		public string RegEx {get; set;}
		#region IHydratable Members

		public void Fill(System.Data.IDataReader dr)
		{
			FeatureGroupId = Convert.ToInt32(Null.SetNull(dr["FeatureGroupId"], FeatureGroupId));
			FeatureGroup = Convert.ToString(Null.SetNull(dr["FeatureGroup"], FeatureGroup));
			FeatureId = Convert.ToInt32(Null.SetNull(dr["FeatureId"], FeatureId));
			Feature = Convert.ToString(Null.SetNull(dr["Feature"], Feature));
			Unit = Convert.ToString(Null.SetNull(dr["Unit"], Unit));
			Datatype = Convert.ToString(Null.SetNull(dr["Datatype"], Datatype));
			Control = Convert.ToString(Null.SetNull(dr["Control"], Control));
			Dimension = Convert.ToInt32(Null.SetNull(dr["Dimension"], Dimension));
			Required = Convert.ToBoolean(Null.SetNull(dr["Required"], Required));
			Multiselect = Convert.ToBoolean(Null.SetNull(dr["Multiselect"], Multiselect));
			FeatureListId = (dr["FeatureListId"] == DBNull.Value ? null : (int?)dr["FeatureListId"]);
			MinValue = (dr["MinValue"] == DBNull.Value ? null : (string)dr["MinValue"]);
			MaxValue = (dr["MaxValue"] == DBNull.Value ? null : (string)dr["MaxValue"]);
			RegEx = (dr["RegEx"] == DBNull.Value ? null : (string)dr["RegEx"]);
		}

		public int KeyID
		{
			get
			{
				return FeatureId;
			}
			set
			{
				FeatureId = value;
			}
		}
		#endregion

	}
}