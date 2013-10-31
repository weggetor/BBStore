using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Bitboxx.DNNModules.BBStore
{
	[Serializable]
	[DataContract()]
	public class LocalResourceInfo
	{
        public LocalResourceInfo()
		{
			LocalResourceId = 0;
            PortalId = -1;
            LocalResourceToken = "";
		}
		[DataMember()]
        public Int32 LocalResourceId { get; set; }
		[DataMember()]
		public Int32 PortalId { get; set; }
		[DataMember()]
		public string LocalResourceToken { get; set; }
	}
}