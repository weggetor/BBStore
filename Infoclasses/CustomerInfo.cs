using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    [Serializable]
    [DataContract()]
    public class CustomerInfo
    {
        public CustomerInfo()
        {
            CustomerId = -1;
            UserId = -1;
            PortalId = -1;
	        CustomerName = "";
            _status = 0;
        }
        public CustomerInfo(int userId, int portalId, string customerName)
        {
            CustomerId = -1;
            UserId = userId;
            PortalId = portalId;
	        CustomerName = customerName;
            _status = 0;
        }

        [DataMember()]
        public int CustomerId { get; set; }
        [DataMember()]
        public int UserId { get; set; }
        [DataMember()]
        public int PortalId { get; set; }
        [DataMember()]
        public string CustomerName { get; set; }
        [DataMember()]
        public int _status { get; set; }
    }
}
