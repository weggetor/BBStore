﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
	public class BBStoreSecurityToken
	{
		public int UserId { get; set; }
		public string UserName { get; set; }
		public int PortalId { get; set; }
	}
}