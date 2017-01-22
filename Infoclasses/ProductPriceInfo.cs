using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;

namespace Bitboxx.DNNModules.BBStore
{
    [Serializable]
    [DataContract()]
    public class ProductPriceInfo : IHydratable
    {
        public ProductPriceInfo()
        {
            ProductPriceId = 0;
            SimpleProductId = 0;
            UnitCost = 0.00m;
            OriginalUnitCost = 0.00m;
            TaxPercent = 0.00m;
            RoleId = 0;
            Startdate = DateTime.Now;
            EndDate = DateTime.Now.AddYears(500);
            UserRole = "";
        }
        [DataMember()]
        public Int32 ProductPriceId { get; set; }
        [DataMember()]
        public Int32 SimpleProductId { get; set; }
        [DataMember()]
        public Decimal UnitCost { get; set; }
        [DataMember()]
        public Decimal OriginalUnitCost { get; set; }
        [DataMember()]
        public Decimal TaxPercent { get; set; }
        [DataMember()]
        public Int32 RoleId { get; set; }
        [DataMember()]
        public DateTime? Startdate { get; set; }
        [DataMember()]
        public DateTime? EndDate { get; set; }

        public string UserRole { get; set; }


        public void Fill(IDataReader dr)
        {
            ProductPriceId = Null.SetNullInteger(dr["ProductPriceId"]);
            SimpleProductId = Null.SetNullInteger(dr["SimpleProductId"]);
            UnitCost = (Decimal) dr["UnitCost"];
            OriginalUnitCost = (Decimal)dr["OriginalUnitCost"];
            TaxPercent = (Decimal)dr["TaxPercent"];
            RoleId = Null.SetNullInteger(dr["RoleId"]);
            Startdate = dr["Startdate"] == DBNull.Value ? null : (DateTime?)dr["Startdate"];
            EndDate = dr["EndDate"] == DBNull.Value ? null : (DateTime?)dr["EndDate"];
            UserRole = (string)dr["UserRole"];
        }

        public int KeyID { get; set; }
    }
}
