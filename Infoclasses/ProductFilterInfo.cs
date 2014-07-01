using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Common.Utilities;

namespace Bitboxx.DNNModules.BBStore
{
    public class ProductFilterInfo:IHydratable
    {
        public ProductFilterInfo()
        {
            FilterSessionId = new Guid();
            PortalId = 0;
            FilterSource = "";
			FilterValue = "";
			FilterCondition = "";
        }

        public Guid FilterSessionId { get; set; }
        public int PortalId { get; set; }
        public string FilterSource { get; set; }
		public string FilterValue { get; set; }
		public string FilterCondition { get; set; }

		#region IHydratable Members
		public void Fill(System.Data.IDataReader dr)
		{
			FilterSessionId = (Guid)(Null.SetNull(dr["FilterSessionId"], FilterSessionId));
			PortalId = Convert.ToInt32(Null.SetNull(dr["PortalId"], PortalId));
			FilterSource = Convert.ToString(Null.SetNull(dr["FilterSource"], FilterSource));
			FilterValue = Convert.ToString(Null.SetNull(dr["FilterValue"], FilterValue));
			FilterCondition = Convert.ToString(Null.SetNull(dr["FilterCondition"], FilterCondition));
		}

		// we dont nee this for our purpose
		public int KeyID
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}
		#endregion
	}
}