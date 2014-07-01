using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    public class CustomerInfo
    {
        public CustomerInfo()
        {
            CustomerId = -1;
            UserId = -1;
            PortalId = -1;
	        CustomerName = "";
        }
        public CustomerInfo(int userId, int portalId, string customerName)
        {
            CustomerId = -1;
            UserId = userId;
            PortalId = portalId;
	        CustomerName = customerName;
        }

        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public int PortalId { get; set; }
        public string CustomerName { get; set; }
    }
}
