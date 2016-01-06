using System;
using System.Data;
using System.Data.SqlTypes;
using System.Runtime.Serialization;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;

namespace Bitboxx.DNNModules.BBStore
{
    [Serializable]
    public class CouponInfo:IHydratable
    {
        public CouponInfo()
        {
            CouponId = 0;
            PortalId = 0;
            Code = "";
            MaxUsages = 1;
            UsagesLeft = 1;
            DiscountPercent = 0m;
            DiscountValue = 0m;
            TaxPercent = 0m;

        }
        public Int32 CouponId { get; set; }
        public Int32 PortalId { get; set; }
        public string Caption { get; set; }
        public string Code { get; set; }
        public DateTime? ValidUntil { get; set; }
        public int MaxUsages { get; set; }
        public int UsagesLeft { get; set; }
        public decimal? DiscountPercent { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal TaxPercent { get; set; }
        
        
        public void Fill(IDataReader dr)
        {
            CouponId = (int)dr["CouponId"];
            PortalId = (int)dr["PortalId"];
            Code = Null.SetNullString(dr["Code"]);
            Caption = Null.SetNullString(dr["Caption"]);
            
            if (dr["ValidUntil"] == null || dr["ValidUntil"] == DBNull.Value)
                ValidUntil = null;
            else
                ValidUntil = (DateTime)dr["ValidUntil"];

            MaxUsages = (int)dr["MaxUsages"];
            UsagesLeft = (int)dr["UsagesLeft"];
            
            if (dr["DiscountPercent"] == null || dr["DiscountPercent"] == DBNull.Value)
                DiscountPercent = null;
            else
                DiscountPercent = (decimal) dr["DiscountPercent"];
            
            if (dr["DiscountValue"] == null || dr["DiscountValue"] == DBNull.Value)
                DiscountValue = null;
            else
                DiscountValue = (decimal) dr["DiscountValue"];

            TaxPercent = (decimal)dr["TaxPercent"];
        }

        public int KeyID { get; set; }
    }
}