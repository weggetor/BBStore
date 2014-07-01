using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Bitboxx.DNNModules.BBStore
{
    [Serializable]
	[DataContract()]
    public class ProductGroupInfo
    {
        public ProductGroupInfo()
        {
            PortalId = 0;
            SubscriberId = -1;
            ProductGroupId = -1;
            ParentId = -1;
            Image = "";
            Icon = "";
            ProductGroupName = "";
	        ProductGroupShortDescription = "";
	        ProductGroupDescription = "";
			ProductListTabId = -1;
			ProductCount = 0;
			Disabled = false;
			ViewOrder = 0;
        }
        
        [DataMember()]
		public int PortalId { get; set; }
		[DataMember()]
		public int SubscriberId { get; set; }
		[DataMember()]
		public int ProductGroupId { get; set; }
		[DataMember()]
		public int ParentId { get; set; }
		[DataMember()]
        public string ProductGroupName { get; set; }
		[DataMember()]
		public string ProductGroupShortDescription { get; set; }
		[DataMember()]
		public string ProductGroupDescription { get; set; }
		[DataMember()]
        public string Image { get; set; }
		[DataMember()]
        public string Icon { get; set; }
		[DataMember()]
        public int ProductListTabId { get; set; }
		[DataMember()]
		public int ProductCount { get; set; }
		[DataMember()]
		public bool Disabled { get; set; }
		[DataMember()]
		public int ViewOrder { get; set; }

    }
}