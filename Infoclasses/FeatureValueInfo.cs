using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;

namespace Bitboxx.DNNModules.BBStore
{
	[Serializable]
	[DataContract()]
	public class FeatureValueInfo:IHydratable
	{
		public FeatureValueInfo()
		{
			FeatureValueId = 0;
			FeatureId = 0;
			ProductId = 0;
			FeatureListItemId = -1;
		}
		[DataMember()]
		public Int32 FeatureValueId { get; set; }
		[DataMember()]
		public Int32 FeatureId { get; set; }
		[DataMember()]
		public Int32 ProductId { get; set; }
		[DataMember()]
		public Int32? FeatureListItemId { get; set; }
		[DataMember()]
		public string cValue { get; set; }
		[DataMember()]
		public DateTime? tValue { get; set; }
		[DataMember()]
		public Decimal? nValue { get; set; }
		[DataMember()]
		public Int32? iValue { get; set; }
		[DataMember()]
		public Double? fValue { get; set; }
		[DataMember()]
		public Boolean? bValue { get; set; }

	    public void Fill(IDataReader dr)
	    {
            FeatureValueId = Null.SetNullInteger(dr["FeatureValueId"]);
            FeatureId = Null.SetNullInteger(dr["FeatureId"]);
            ProductId = Null.SetNullInteger(dr["ProductId"]);
            FeatureListItemId =(dr["FeatureListItemId"]== DBNull.Value ? null : (Int32?)dr["FeatureListItemId"]);
            cValue = (dr["cValue"]== DBNull.Value ? null : (string)dr["cValue"]);
            tValue = (dr["tValue"] == DBNull.Value ? null : (DateTime?)dr["tValue"]);
            nValue = (dr["nValue"] == DBNull.Value ? null : (Decimal?)dr["nValue"]);
            iValue = (dr["iValue"] == DBNull.Value ? null : (Int32?)dr["iValue"]);
            fValue = (dr["fValue"] == DBNull.Value ? null : (Double?)dr["fValue"]);
            bValue = (dr["bValue"]== DBNull.Value ? null : (Boolean?)dr["bValue"]);
	    }

	    public int KeyID
	    {
	        get { return FeatureValueId; } 
	        set { FeatureValueId = value; }
	    }
	}

}