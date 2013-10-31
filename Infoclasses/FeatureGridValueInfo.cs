using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Common.Utilities;

namespace Bitboxx.DNNModules.BBStore
{
	public class FeatureGridValueInfo:IHydratable

	{
		public FeatureGridValueInfo()
		{
			FeatureGroupId = -1;
			FeatureGroup = "";
			FeatureId = -1;
			Feature = "";
			FeatureToken = "";
			Unit = "";
			FeatureListId = -1;
			FeatureList = "";
			Datatype = "";
			Control = "";
			Multiselect = false;
			FeatureListItemId = null;
			FeatureListItem = "";
			nValue = null;
			cValue = null;
			tValue = null;
			iValue = null;
			fValue = null;
			bValue = null;
		}
		
		public int FeatureGroupId {get; set;}
		public string FeatureGroup {get; set;}

		public int FeatureId { get; set; }
		public string Feature {get; set;}
		public string FeatureToken { get; set; }
		public string Unit { get; set; }
		public int FeatureListId { get; set; }
		public string FeatureList { get; set; }
		public string Datatype {get; set;}
		public string Control {get; set;}
		public bool Multiselect {get; set;}

		public int? FeatureListItemId { get; set; }
		public string FeatureListItem { get; set; }
		public decimal? nValue {get; set;}
		public string cValue {get; set;}
		public DateTime? tValue {get; set;}
		public int? iValue {get; set;}
		public double? fValue { get; set; }
		public Boolean? bValue { get; set; }
		
		#region IHydratable Members

		public void  Fill(System.Data.IDataReader dr)
		{
			FeatureGroupId = Convert.ToInt32(Null.SetNull(dr["FeatureGroupId"], FeatureGroupId));
			FeatureGroup = Convert.ToString(Null.SetNull(dr["FeatureGroup"], FeatureGroup));
			FeatureId = Convert.ToInt32(Null.SetNull(dr["FeatureId"], FeatureId));
			Feature = Convert.ToString(Null.SetNull(dr["Feature"], Feature));
			FeatureToken = Convert.ToString(Null.SetNull(dr["FeatureToken"], FeatureToken));
			Unit = Convert.ToString(Null.SetNull(dr["Unit"], Unit));
			FeatureListId = Convert.ToInt32(Null.SetNull(dr["FeatureListId"], FeatureListId));
			FeatureList = Convert.ToString(Null.SetNull(dr["FeatureList"], FeatureList));
			Datatype = Convert.ToString(Null.SetNull(dr["Datatype"], Datatype));
			Control = Convert.ToString(Null.SetNull(dr["Control"], Control));
			Multiselect = Convert.ToBoolean(Null.SetNull(dr["Multiselect"], Multiselect));
			FeatureListItemId = (dr["FeatureListItemid"] == DBNull.Value ? null : (int?)dr["FeatureListItemId"]);
			FeatureListItem = Convert.ToString(Null.SetNull(dr["FeatureListItem"], FeatureListItem));
			nValue = (dr["nValue"] == DBNull.Value ? null : (decimal?)dr["nValue"]);
			cValue = (dr["cValue"] == DBNull.Value ? null : (string)dr["cValue"]);
			tValue = (dr["tValue"] == DBNull.Value ? null : (DateTime?)dr["tValue"]);
			iValue = (dr["iValue"] == DBNull.Value ? null : (int?)dr["iValue"]);
			fValue = (dr["fValue"] == DBNull.Value ? null : (double?)dr["fValue"]);
			bValue = (dr["bValue"] == DBNull.Value ? null : (bool?)dr["bValue"]);
		}

		public int  KeyID
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