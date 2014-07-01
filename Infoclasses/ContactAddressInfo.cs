using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace Bitboxx.DNNModules.BBStore
{
    [Serializable()]
	public class ContactAddressInfo
    {
        public ContactAddressInfo()
        {
        }

        public int ContactAddressId { get; set; }
        public int PortalId { get; set; }
        public string Company { get; set; }
        public string Prefix { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Unit { get; set; }
        public string Street { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Telephone { get; set; }
		public string Cell { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
     }
}
