using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Runtime.Serialization;

namespace Bitboxx.DNNModules.BBStore
{
	[Serializable]
	[DataContract()]
	public class ContactReasonInfo
	{
		public ContactReasonInfo()
		{
			ContactAddressId = 0;
			Reason = "";
			Token = "";
		}
		public ContactReasonInfo(int contactAddressId, string reason, string token)
		{
			ContactAddressId = contactAddressId;
			Reason = reason;
			Token = token;
		}

		[DataMember()]
		public Int32 ContactAddressId { get; set; }
		[DataMember()]
		public string Reason { get; set; }
		[DataMember()]
		public string Token { get; set; }
	}

}
