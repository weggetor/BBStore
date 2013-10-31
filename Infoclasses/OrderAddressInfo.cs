using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace Bitboxx.DNNModules.BBStore
{
    public class OrderAddressInfo
    {
        public OrderAddressInfo()
        {
            CountryCode = "";
        }

        public int OrderAddressId { get; set; }
        public int PortalId { get; set; }
        public int OrderId { get; set; }
        public int CustomerAddressId { get; set; }
        public string Company { get; set; }
        public string Prefix { get; set; }
        public string Firstname { get; set; }
        public string Middlename { get; set; }
        public string Lastname { get; set; }
        public string Suffix { get; set; }
        public string Unit { get; set; }
        public string Street { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Suburb { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string Telephone { get; set; }
        public string Cell { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
		public string AddressType { get; set; }

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
