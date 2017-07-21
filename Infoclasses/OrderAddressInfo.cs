using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Text;

namespace Bitboxx.DNNModules.BBStore
{
    [Serializable()]
    [DataContract()]
    public class OrderAddressInfo
    {
        public OrderAddressInfo()
        {
            CountryCode = "";
            _status = 0;
        }

        [DataMember()]
        public int OrderAddressId { get; set; }
        [DataMember()]
        public int PortalId { get; set; }
        [DataMember()]
        public int OrderId { get; set; }
        [DataMember()]
        public int CustomerAddressId { get; set; }
        [DataMember()]
        public string Company { get; set; }
        [DataMember()]
        public string Prefix { get; set; }
        [DataMember()]
        public string Firstname { get; set; }
        [DataMember()]
        public string Middlename { get; set; }
        [DataMember()]
        public string Lastname { get; set; }
        [DataMember()]
        public string Suffix { get; set; }
        [DataMember()]
        public string Unit { get; set; }
        [DataMember()]
        public string Street { get; set; }
        [DataMember()]
        public string Region { get; set; }
        [DataMember()]
        public string PostalCode { get; set; }
        [DataMember()]
        public string City { get; set; }
        [DataMember()]
        public string Suburb { get; set; }
        [DataMember()]
        public string Country { get; set; }
        [DataMember()]
        public string CountryCode { get; set; }
        [DataMember()]
        public string Telephone { get; set; }
        [DataMember()]
        public string Cell { get; set; }
        [DataMember()]
        public string Fax { get; set; }
        [DataMember()]
        public string Email { get; set; }
        [DataMember()]
        public int SubscriberAddressTypeId { get; set; }
        [DataMember()]
        public string AddressType { get; set; }
        [DataMember()]
        public int _status { get; set; }

        public string ToHtml()
        {
            StringBuilder sb = new StringBuilder();
            if (!String.IsNullOrEmpty(Company))
                sb.Append(HttpUtility.HtmlEncode(Company) + "<br />");
            if (!String.IsNullOrEmpty(Prefix))
                sb.Append(HttpUtility.HtmlEncode(Prefix) + " ");
            if (!String.IsNullOrEmpty(Firstname))
                sb.Append(HttpUtility.HtmlEncode(Firstname) + " ");
            if (!String.IsNullOrEmpty(Middlename))
                sb.Append(HttpUtility.HtmlEncode(Middlename) + " ");
            if (!String.IsNullOrEmpty(Lastname))
                sb.Append(HttpUtility.HtmlEncode(Lastname) + " ");
            if (!String.IsNullOrEmpty(Suffix))
                sb.Append(HttpUtility.HtmlEncode(Suffix) + " ");
            if (!String.IsNullOrEmpty(sb.ToString().Trim()))
                sb.Append(HttpUtility.HtmlEncode(Suffix) + "<br />");
            if (!String.IsNullOrEmpty(Unit))
                sb.Append(HttpUtility.HtmlEncode(Unit) + "<br /> ");
            if (!String.IsNullOrEmpty(Street))
                sb.Append(HttpUtility.HtmlEncode(Street) + "<br /> ");
            if (!String.IsNullOrEmpty(Region))
                sb.Append(HttpUtility.HtmlEncode(Region) + "<br />");
            if (!String.IsNullOrEmpty(CountryCode))
                sb.Append(HttpUtility.HtmlEncode(CountryCode) + "-");
            if (!String.IsNullOrEmpty(PostalCode))
                sb.Append(HttpUtility.HtmlEncode(PostalCode) + " ");
            if (!String.IsNullOrEmpty(City))
                sb.Append(HttpUtility.HtmlEncode(City) + "<br />");
            return sb.ToString();
        }
        public string ToHtml(string template, bool includeEmptyLines)
        {
			StringBuilder sb = new StringBuilder();
			sb.Append(template);
			sb.Replace("\r\n", "\n");
			// count Params in template
			int anzParams = VfpInterop.Occurs('[', template);
			for (int i = 0; i < anzParams; i++)
			{
				string[] param = VfpInterop.StrExtract(template, "[", "]", i + 1, 1).Split(':');
				string search = "]";
				if (param.Length > 1)
				{
					string width = param[1];
					search = ":" + width + "]";
				}

				sb.Replace("[COMPANY" + search, HttpUtility.HtmlEncode(Company));
				sb.Replace("[PREFIX" + search, HttpUtility.HtmlEncode(Prefix));
				sb.Replace("[FIRSTNAME" + search, HttpUtility.HtmlEncode(Firstname));
				sb.Replace("[MIDDLENAME" + search, HttpUtility.HtmlEncode(Middlename));
				sb.Replace("[LASTNAME" + search, HttpUtility.HtmlEncode(Lastname));
				sb.Replace("[SUFFIX" + search, HttpUtility.HtmlEncode(Suffix));
				sb.Replace("[STREET" + search, HttpUtility.HtmlEncode(Street));
				sb.Replace("[UNIT" + search, HttpUtility.HtmlEncode(Unit));
				sb.Replace("[REGION" + search, HttpUtility.HtmlEncode(Region));
				sb.Replace("[POSTALCODE" + search, HttpUtility.HtmlEncode(PostalCode));
				sb.Replace("[CITY" + search, HttpUtility.HtmlEncode(City));
				sb.Replace("[SUBURB" + search, HttpUtility.HtmlEncode(Suburb));
				sb.Replace("[COUNTRY" + search, HttpUtility.HtmlEncode(Country));
				sb.Replace("[COUNTRYCODE" + search, HttpUtility.HtmlEncode(CountryCode));
				sb.Replace("[PHONE" + search, HttpUtility.HtmlEncode(Telephone));
				sb.Replace("[CELL" + search, HttpUtility.HtmlEncode(Cell));
				sb.Replace("[FAX" + search, HttpUtility.HtmlEncode(Fax));
				sb.Replace("[EMAIL" + search, HttpUtility.HtmlEncode(Email));
			}
			string[] resultLines = sb.ToString().Split('\n');
			string result = "";
			for (int i = 0; i < resultLines.Length; i++)
			{
				if (!includeEmptyLines)
				{
					if (resultLines[i].Trim() != String.Empty)
						result = result + resultLines[i] + "<br />";
				}
				else
				{
					result = result + resultLines[i] + "<br />";
				}
			}
			return result;

        }
    }
}
