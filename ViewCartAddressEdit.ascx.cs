using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Common.Lists;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;

namespace Bitboxx.DNNModules.BBStore
{
	public partial class ViewCartAddressEdit : PortalModuleBase
	{
		private BBStoreController _controller;
		private Hashtable _storeSettings;
		private DropDownList ddlCountry;
		private TextBox txtAdrEditCompany;
		private TextBox txtAdrEditPrefix;
		private TextBox txtAdrEditFirstname;
		private TextBox txtAdrEditMiddlename;
		private TextBox txtAdrEditLastname;
		private TextBox txtAdrEditSuffix;
		private TextBox txtAdrEditUnit;
		private TextBox txtAdrEditStreet;
		private TextBox txtAdrEditRegion;
		private TextBox txtAdrEditPostalCode;
		private TextBox txtAdrEditCity;
		private TextBox txtAdrEditSuburb;
		private TextBox txtAdrEditPhone;
		private TextBox txtAdrEditFax;
		private TextBox txtAdrEditCell;
		private TextBox txtAdrEditEmail;


		
		public BBStoreController Controller
		{
			get
			{
				if (_controller == null)
					_controller = new BBStoreController();
				return _controller;
			}
		}
		public Hashtable StoreSettings
		{
			get
			{
				if (_storeSettings == null)
					_storeSettings = Controller.GetStoreSettings(PortalId);
				return _storeSettings;
			}
		}
		public ViewCart MainControl { get; set; }
		
		protected void Page_Load(object sender, EventArgs e)
		{
			// Is he logged in ?
			if (!Request.IsAuthenticated)
			{
				// Attention ! returnUrl must be relative path (cross-site-scripting denying)
				string returnUrl = HttpContext.Current.Request.RawUrl;
				returnUrl = HttpUtility.UrlEncode(returnUrl);

				Response.Redirect(Globals.NavigateURL(TabId, "", "action=login", "returnurl=" + returnUrl));
			}

		    if (true)
		    {
		        lblIntro.Text = LocalizeString("Intro.Message");
                
                // Lets retrieve the CustomerAdressId and the CustomerAdress from Database
		        hidAdrEditCustomerAddressId.Value = (Request.QueryString["adrid"] != null ? Request.QueryString["adrid"].ToString() : "-1");

		        string form = GenerateForm();
		        Control ctrl = ParseControl(form);
		        string selectedCountryCode = "";

		        ddlCountry = FindControlRecursive(ctrl, "ddlCountry") as DropDownList;
		        if (ddlCountry != null)
		        {
		            ListController ListController = new ListController();
		            ListEntryInfoCollection Countries = ListController.GetListEntryInfoCollection("Country");
		            ddlCountry.DataSource = Countries;
		            ddlCountry.DataTextField = "Text";
		            ddlCountry.DataValueField = "Value";
		            ddlCountry.DataBind();
		            if (!String.IsNullOrEmpty((string) StoreSettings["VendorCountry"]))
		                selectedCountryCode = (string) StoreSettings["VendorCountry"];
		        }
		        txtAdrEditCompany = FindControlRecursive(ctrl, "txtAdrEditCompany") as TextBox;
		        txtAdrEditPrefix = FindControlRecursive(ctrl, "txtAdrEditPrefix") as TextBox;
		        txtAdrEditFirstname = FindControlRecursive(ctrl, "txtAdrEditFirstname") as TextBox;
		        txtAdrEditMiddlename = FindControlRecursive(ctrl, "txtAdrEditMiddlename") as TextBox;
		        txtAdrEditLastname = FindControlRecursive(ctrl, "txtAdrEditLastname") as TextBox;
		        txtAdrEditSuffix = FindControlRecursive(ctrl, "txtAdrEditSuffix") as TextBox;
		        txtAdrEditUnit = FindControlRecursive(ctrl, "txtAdrEditUnit") as TextBox;
		        txtAdrEditStreet = FindControlRecursive(ctrl, "txtAdrEditStreet") as TextBox;
		        txtAdrEditRegion = FindControlRecursive(ctrl, "txtAdrEditRegion") as TextBox;
		        txtAdrEditPostalCode = FindControlRecursive(ctrl, "txtAdrEditPostalCode") as TextBox;
		        txtAdrEditCity = FindControlRecursive(ctrl, "txtAdrEditCity") as TextBox;
		        txtAdrEditSuburb = FindControlRecursive(ctrl, "txtAdrEditSuburb") as TextBox;
		        txtAdrEditPhone = FindControlRecursive(ctrl, "txtAdrEditPhone") as TextBox;
		        txtAdrEditFax = FindControlRecursive(ctrl, "txtAdrEditFax") as TextBox;
		        txtAdrEditCell = FindControlRecursive(ctrl, "txtAdrEditCell") as TextBox;
		        txtAdrEditEmail = FindControlRecursive(ctrl, "txtAdrEditEmail") as TextBox;
		        phAddressEdit.Controls.Add(ctrl);

		        if (hidAdrEditCustomerAddressId.Value != "-1")
		        {
		            int CustomerAddressId;
		            if (Int32.TryParse(hidAdrEditCustomerAddressId.Value, out CustomerAddressId))
		            {
		                CustomerAddressInfo CustomerAddress = Controller.GetCustomerAddress(CustomerAddressId);

		                if (CustomerAddress != null && CustomerAddress.CustomerId == this.MainControl.CustomerId)
		                {
		                    if (txtAdrEditCompany != null) txtAdrEditCompany.Text = CustomerAddress.Company.Trim();
                            if (txtAdrEditPrefix != null) txtAdrEditPrefix.Text = CustomerAddress.Prefix.Trim();
                            if (txtAdrEditFirstname != null) txtAdrEditFirstname.Text = CustomerAddress.Firstname.Trim();
                            if (txtAdrEditMiddlename != null) txtAdrEditMiddlename.Text = CustomerAddress.Middlename.Trim();
                            if (txtAdrEditLastname != null) txtAdrEditLastname.Text = CustomerAddress.Lastname.Trim();
                            if (txtAdrEditSuffix != null) txtAdrEditSuffix.Text = CustomerAddress.Suffix.Trim();
                            if (txtAdrEditUnit != null) txtAdrEditUnit.Text = CustomerAddress.Unit.Trim();
                            if (txtAdrEditStreet != null) txtAdrEditStreet.Text = CustomerAddress.Street.Trim();
                            if (txtAdrEditRegion != null) txtAdrEditRegion.Text = CustomerAddress.Region.Trim();
                            if (txtAdrEditPostalCode != null) txtAdrEditPostalCode.Text = CustomerAddress.PostalCode.Trim();
                            if (txtAdrEditCity != null) txtAdrEditCity.Text = CustomerAddress.City.Trim();
                            if (txtAdrEditSuburb != null) txtAdrEditSuburb.Text = CustomerAddress.Suburb.Trim();
                            if (txtAdrEditPhone != null) txtAdrEditPhone.Text = CustomerAddress.Telephone.Trim();
                            if (txtAdrEditFax != null) txtAdrEditFax.Text = CustomerAddress.Fax.Trim();
                            if (txtAdrEditCell != null) txtAdrEditCell.Text = CustomerAddress.Cell.Trim();
                            if (txtAdrEditEmail != null) txtAdrEditEmail.Text = CustomerAddress.Email.Trim();
		                    selectedCountryCode = CustomerAddress.CountryCode;
		                }
		                else
		                    hidAdrEditCustomerAddressId.Value = "-1";
		            }
		            else
		                hidAdrEditCustomerAddressId.Value = "-1";
		        }
		        if (ddlCountry != null && selectedCountryCode != string.Empty)
		        {
		            ListItem itemToSelect = ddlCountry.Items.FindByValue(selectedCountryCode.Trim());
                    if (itemToSelect != null)
                        ddlCountry.SelectedIndex = ddlCountry.Items.IndexOf(itemToSelect);
		        }
		    }
		}

		protected void cmdAdrEditCancel_Click(object sender, EventArgs e)
		{
			Response.Redirect(Globals.NavigateURL(TabId, "", "action=checkout"));
		}

		protected void cmdAdrEditSave_Click(object sender, EventArgs e)
		{
			int customerAddressId = -1;
			Int32.TryParse(hidAdrEditCustomerAddressId.Value, out customerAddressId);
			CustomerAddressInfo CustomerAddress = new CustomerAddressInfo();
			bool IsNewAddress;

			if (customerAddressId > 0)
				CustomerAddress = Controller.GetCustomerAddress(customerAddressId);

			if (CustomerAddress.CustomerId != this.MainControl.CustomerId)
			{
				CustomerAddress = new CustomerAddressInfo();
				CustomerAddress.CustomerId = this.MainControl.CustomerId;
				IsNewAddress = true;
				CustomerAddress.PortalId = PortalId;
			}
			else
				IsNewAddress = false;

            if (txtAdrEditCompany != null) CustomerAddress.Company = txtAdrEditCompany.Text.Trim();
            if (txtAdrEditPrefix != null) CustomerAddress.Prefix = txtAdrEditPrefix.Text.Trim();
            if (txtAdrEditFirstname != null) CustomerAddress.Firstname = txtAdrEditFirstname.Text.Trim();
            if (txtAdrEditMiddlename != null) CustomerAddress.Middlename = txtAdrEditMiddlename.Text.Trim();
            if (txtAdrEditLastname != null) CustomerAddress.Lastname = txtAdrEditLastname.Text.Trim();
            if (txtAdrEditSuffix != null) CustomerAddress.Suffix = txtAdrEditSuffix.Text.Trim();
            if (txtAdrEditUnit != null) CustomerAddress.Unit = txtAdrEditUnit.Text.Trim();
            if (txtAdrEditStreet != null) CustomerAddress.Street = txtAdrEditStreet.Text.Trim();
            if (txtAdrEditRegion != null) CustomerAddress.Region = txtAdrEditRegion.Text.Trim();
            if (txtAdrEditPostalCode != null) CustomerAddress.PostalCode = txtAdrEditPostalCode.Text.Trim();
            if (txtAdrEditCity != null) CustomerAddress.City = txtAdrEditCity.Text.Trim();
            if (txtAdrEditSuburb != null) CustomerAddress.Suburb = txtAdrEditSuburb.Text.Trim();
			if (ddlCountry != null)
			{
				CustomerAddress.Country = ddlCountry.SelectedItem.Text;
				CustomerAddress.CountryCode = ddlCountry.SelectedItem.Value;
			}

            if (txtAdrEditPhone != null) CustomerAddress.Telephone = txtAdrEditPhone.Text.Trim();
            if (txtAdrEditFax != null) CustomerAddress.Fax = txtAdrEditFax.Text.Trim();
            if (txtAdrEditCell != null) CustomerAddress.Cell = txtAdrEditCell.Text.Trim();
            if (txtAdrEditEmail != null) CustomerAddress.Email = txtAdrEditEmail.Text.Trim();
			if (IsNewAddress)
				Controller.NewCustomerAddress(CustomerAddress);
			else
				Controller.UpdateCustomerAddress(CustomerAddress);
			Response.Redirect(Globals.NavigateURL(TabId, "", "action=checkout"));
		}

		#region Helper methods
		private Control FindControlRecursive(Control rootControl, string controlID)
		{
			if (rootControl.ID == controlID)
				return rootControl;

			foreach (Control controlToSearch in rootControl.Controls)
			{
				Control controlToReturn = FindControlRecursive(controlToSearch, controlID);
				if (controlToReturn != null)
					return controlToReturn;
			}
			return null;
		}

		private string GetCountryCode(string Country)
		{
			ListController oController = new ListController();
			ListEntryInfoCollection oCountries = oController.GetListEntryInfoCollection("Country");
			foreach (ListEntryInfo oCountry in oCountries)
			{
				if (oCountry.Text.ToLower() == Country.ToLower())
				{
					return oCountry.Value;
				}
			}
			return "";
		}

		private string GenerateForm()
		{
			string template = Localization.GetString("AddressTemplate.Text", this.LocalResourceFile.Replace("ViewCartAddressEdit","ViewCart"));

			bool mandCompany = Settings["MandCompany"] != null && Convert.ToBoolean(Settings["MandCompany"]);
			bool mandPrefix = Settings["MandPrefix"] != null && Convert.ToBoolean(Settings["MandPrefix"]);
			bool mandFirstname = Settings["MandFirstname"] != null && Convert.ToBoolean(Settings["MandFirstname"]);
			bool mandMiddlename = Settings["MandMiddlename"] != null && Convert.ToBoolean(Settings["MandMiddlename"]);
			bool mandLastname = Settings["MandLastname"] != null && Convert.ToBoolean(Settings["MandLastname"]);
			bool mandSuffix = Settings["MandSuffix"] != null && Convert.ToBoolean(Settings["MandSuffix"]);
			bool mandStreet = Settings["MandStreet"] != null && Convert.ToBoolean(Settings["MandStreet"]);
			bool mandUnit = Settings["MandUnit"] != null && Convert.ToBoolean(Settings["MandUnit"]);
			bool mandRegion = Settings["MandRegion"] != null && Convert.ToBoolean(Settings["MandRegion"]);
			bool mandPostalCode = Settings["MandPostalCode"] != null && Convert.ToBoolean(Settings["MandPostalCode"]);
			bool mandCity = Settings["MandCity"] != null && Convert.ToBoolean(Settings["MandCity"]);
			bool mandSuburb = Settings["MandSuburb"] != null && Convert.ToBoolean(Settings["MandSuburb"]);
			bool mandCountry = Settings["MandCountry"] != null && Convert.ToBoolean(Settings["MandCountry"]);
			bool mandPhone = Settings["MandPhone"] != null && Convert.ToBoolean(Settings["MandPhone"]);
			bool mandCell = Settings["MandCell"] != null && Convert.ToBoolean(Settings["MandCell"]);
			bool mandFax = Settings["MandFax"] != null && Convert.ToBoolean(Settings["MandFax"]);
			bool mandEmail = Settings["MandEmail"] != null && Convert.ToBoolean(Settings["MandEmail"]);

			string[] lines = template.Replace("\r", "").Split('\n');
			StringBuilder sb = new StringBuilder();
			foreach (string line in lines)
			{
				string caption = "", input = "", validator = "";
				sb.Append("<div class=\"dnnFormItem\">");
				int anzParams = VfpInterop.Occurs('[', line);
				for (int i = 0; i < anzParams; i++)
				{

					string[] param = VfpInterop.StrExtract(line, "[", "]", i + 1, 1).Split(':');
					string paraName = param[0];
					string width = "50";
					if (param.Length > 1)
						width = param[1];

					string mandChar = " *";

					if (paraName == "COMPANY")
					{
						caption = caption + (String.IsNullOrEmpty(caption) ? "" : " / ") + Localization.GetString("lblAdrEditCompany.Text", this.LocalResourceFile) + (mandCompany ? mandChar : "");
						input = input + "<asp:Textbox runat=\"server\" ID=\"txtAdrEditCompany\" EnableViewState=\"True\" MaxLength=\"50\" Style=\"width:" + width + "px;min-width:10px\" " + (mandCompany ? "CssClass=\"dnnFormRequired\"" : "") + " />";
						validator = validator + (mandCompany ? "<asp:RequiredFieldValidator runat=\"server\" ID=\"valCompany\" CssClass=\"dnnFormMessage dnnFormError\" Display=\"Dynamic\" style=\"left:inherit\" Text=\"&#171;\" ControlToValidate=\"txtAdrEditCompany\" />" : "");
					}

					if (paraName == "PREFIX")
					{
						caption = caption + (String.IsNullOrEmpty(caption) ? "" : " / ") + Localization.GetString("lblAdrEditPrefix.Text", this.LocalResourceFile) + (mandPrefix ? mandChar : "");
                        input = input + "<asp:TextBox id=\"txtAdrEditPrefix\" runat=\"server\" EnableViewState=\"True\" MaxLength=\"20\" Style=\"width:" + width + "px;min-width:10px\" " + (mandPrefix ? "CssClass=\"dnnFormRequired\"" : "") + " />";
                        validator = validator + (mandPrefix ? "<asp:RequiredFieldValidator runat=\"server\" ID=\"valPrefix\" CssClass=\"dnnFormMessage dnnFormError\" Display=\"Dynamic\" style=\"left:inherit\" Text=\"&#171;\" ControlToValidate=\"txtAdrEditPrefix\" />" : "");

					}
					if (paraName == "FIRSTNAME")
					{
						caption = caption + (String.IsNullOrEmpty(caption) ? "" : " / ") + Localization.GetString("lblAdrEditFirstname.Text", this.LocalResourceFile) + (mandFirstname ? mandChar : "");
                        input = input + "<asp:TextBox id=\"txtAdrEditFirstname\" runat=\"server\" EnableViewState=\"True\" MaxLength=\"50\" Style=\"width:" + width + "px;min-width:10px\" " + (mandFirstname ? "CssClass=\"dnnFormRequired\"" : "") + " />";
                        validator = validator + (mandFirstname ? "<asp:RequiredFieldValidator runat=\"server\" ID=\"valFirstname\" CssClass=\"dnnFormMessage dnnFormError\" Display=\"Dynamic\" style=\"left:inherit\" Text=\"&#171;\" ControlToValidate=\"txtAdrEditFirstname\" />" : "");

					}
					if (paraName == "MIDDLENAME")
					{
						caption = caption + (String.IsNullOrEmpty(caption) ? "" : " / ") + Localization.GetString("lblAdrEditMiddlename.Text", this.LocalResourceFile) + (mandMiddlename ? mandChar : "");
                        input = input + "<asp:TextBox id=\"txtAdrEditMiddlename\" runat=\"server\" EnableViewState=\"True\" MaxLength=\"50\" Style=\"width:" + width + "px;min-width:10px\" " + (mandMiddlename ? "CssClass=\"dnnFormRequired\"" : "") + " />";
                        validator = validator + (mandMiddlename ? "<asp:RequiredFieldValidator runat=\"server\" ID=\"valMiddlename\" CssClass=\"dnnFormMessage dnnFormError\" Display=\"Dynamic\" style=\"left:inherit\" Text=\"&#171;\" ControlToValidate=\"txtAdrEditMiddlename\" />" : "");

					}
					if (paraName == "LASTNAME")
					{
						caption = caption + (String.IsNullOrEmpty(caption) ? "" : " / ") + Localization.GetString("lblAdrEditLastname.Text", this.LocalResourceFile) + (mandLastname ? mandChar : "");
                        input = input + "<asp:TextBox id=\"txtAdrEditLastname\" runat=\"server\" EnableViewState=\"True\" MaxLength=\"50\" Style=\"width:" + width + "px;min-width:10px\" " + (mandLastname ? "CssClass=\"dnnFormRequired\"" : "") + " />";
                        validator = validator + (mandLastname ? "<asp:RequiredFieldValidator runat=\"server\" ID=\"valLastname\" CssClass=\"dnnFormMessage dnnFormError\" Display=\"Dynamic\" style=\"left:inherit\" Text=\"&#171;\" ControlToValidate=\"txtAdrEditLastname\" />" : "");

					}
					if (paraName == "SUFFIX")
					{
						caption = caption + (String.IsNullOrEmpty(caption) ? "" : " / ") + Localization.GetString("lblAdrEditSuffix.Text", this.LocalResourceFile) + (mandSuffix ? mandChar : "");
                        input = input + "<asp:TextBox id=\"txtAdrEditSuffix\" runat=\"server\" EnableViewState=\"True\" MaxLength=\"50\" Style=\"width:" + width + "px;min-width:10px\" " + (mandSuffix ? "CssClass=\"dnnFormRequired\"" : "") + " />";
                        validator = validator + (mandSuffix ? "<asp:RequiredFieldValidator runat=\"server\" ID=\"valSuffix\" CssClass=\"dnnFormMessage dnnFormError\" Display=\"Dynamic\" style=\"left:inherit\" Text=\"&#171;\" ControlToValidate=\"txtAdrEditSuffix\" />" : "");

					}
					if (paraName == "STREET")
					{
						caption = caption + (String.IsNullOrEmpty(caption) ? "" : " / ") + Localization.GetString("lblAdrEditStreet.Text", this.LocalResourceFile) + (mandStreet ? mandChar : "");
                        input = input + "<asp:TextBox id=\"txtAdrEditStreet\" runat=\"server\" EnableViewState=\"True\" MaxLength=\"100\" Style=\"width:" + width + "px;min-width:10px\" " + (mandStreet ? "CssClass=\"dnnFormRequired\"" : "") + " />";
                        validator = validator + (mandStreet ? "<asp:RequiredFieldValidator runat=\"server\" ID=\"valStreet\" CssClass=\"dnnFormMessage dnnFormError\" Display=\"Dynamic\" style=\"left:inherit\" Text=\"&#171;\" ControlToValidate=\"txtAdrEditStreet\" />" : "");

					}
					if (paraName == "UNIT")
					{
						caption = caption + (String.IsNullOrEmpty(caption) ? "" : " / ") + Localization.GetString("lblAdrEditUnit.Text", this.LocalResourceFile) + (mandUnit ? mandChar : "");
                        input = input + "<asp:TextBox ID=\"txtAdrEditUnit\" runat=\"server\" EnableViewState=\"True\" MaxLength=\"50\" Style=\"width:" + width + "px;min-width:10px\" " + (mandUnit ? "CssClass=\"dnnFormRequired\"" : "") + " />";
                        validator = validator + (mandUnit ? "<asp:RequiredFieldValidator runat=\"server\" ID=\"valUnit\" CssClass=\"dnnFormMessage dnnFormError\" Display=\"Dynamic\" style=\"left:inherit\" Text=\"&#171;\" ControlToValidate=\"txtAdrEditUnit\" />" : "");

					}
					if (paraName == "REGION")
					{
						caption = caption + (String.IsNullOrEmpty(caption) ? "" : " / ") + Localization.GetString("lblAdrEditRegion.Text", this.LocalResourceFile) + (mandRegion ? mandChar : "");
                        input = input + "<asp:TextBox id=\"txtAdrEditRegion\" runat=\"server\" EnableViewState=\"True\" MaxLength=\"50\" Style=\"width:" + width + "px;min-width:10px\" " + (mandRegion ? "CssClass=\"dnnFormRequired\"" : "") + " />";
                        validator = validator + (mandRegion ? "<asp:RequiredFieldValidator runat=\"server\" ID=\"valRegion\" CssClass=\"dnnFormMessage dnnFormError\" Display=\"Dynamic\" style=\"left:inherit\" Text=\"&#171;\" ControlToValidate=\"txtAdrEditRegion\" />" : "");

					}
					if (paraName == "POSTALCODE")
					{
						caption = caption + (String.IsNullOrEmpty(caption) ? "" : " / ") + Localization.GetString("lblAdrEditPostalCode.Text", this.LocalResourceFile) + (mandPostalCode ? mandChar : "");
                        input = input + "<asp:TextBox id=\"txtAdrEditPostalCode\" runat=\"server\" EnableViewState=\"True\" MaxLength=\"10\" Style=\"width:" + width + "px;min-width:10px\" " + (mandPostalCode ? "CssClass=\"dnnFormRequired\"" : "") + " />";
                        validator = validator + (mandPostalCode ? "<asp:RequiredFieldValidator runat=\"server\" ID=\"valPostalCode\" CssClass=\"dnnFormMessage dnnFormError\" Display=\"Dynamic\" style=\"left:inherit\" Text=\"&#171;\" ControlToValidate=\"txtAdrEditPostalCode\" />" : "");

					}
					if (paraName == "CITY")
					{
						caption = caption + (String.IsNullOrEmpty(caption) ? "" : " / ") + Localization.GetString("lblAdrEditCity.Text", this.LocalResourceFile) + (mandCity ? mandChar : "");
                        input = input + "<asp:TextBox id=\"txtAdrEditCity\" runat=\"server\" EnableViewState=\"True\" MaxLength=\"100\" Style=\"width:" + width + "px;min-width:10px\" " + (mandCity ? "CssClass=\"dnnFormRequired\"" : "") + " />";
                        validator = validator + (mandCity ? "<asp:RequiredFieldValidator runat=\"server\" ID=\"valCity\" CssClass=\"dnnFormMessage dnnFormError\" Display=\"Dynamic\" style=\"left:inherit\" Text=\"&#171;\" ControlToValidate=\"txtAdrEditCity\" />" : "");
					}
					if (paraName == "SUBURB")
					{
						caption = caption + (String.IsNullOrEmpty(caption) ? "" : " / ") + Localization.GetString("lblAdrEditSuburb.Text", this.LocalResourceFile) + (mandSuburb ? mandChar : "");
                        input = input + "<asp:TextBox id=\"txtAdrEditSuburb\" runat=\"server\" EnableViewState=\"True\" MaxLength=\"100\" Style=\"width:" + width + "px;min-width:10px\" " + (mandSuburb ? "CssClass=\"dnnFormRequired\"" : "") + " />";
                        validator = validator + (mandSuburb ? "<asp:RequiredFieldValidator runat=\"server\" ID=\"valSuburb\" CssClass=\"dnnFormMessage dnnFormError\" Display=\"Dynamic\" style=\"left:inherit\" Text=\"&#171;\" ControlToValidate=\"txtAdrEditSuburb\" />" : "");

					}
					if (paraName == "COUNTRY")
					{
						caption = caption + (String.IsNullOrEmpty(caption) ? "" : " / ") + Localization.GetString("lblAdrEditCountry.Text", this.LocalResourceFile) + (mandCountry ? mandChar : "");
                        input = input + "<asp:DropDownList id=\"ddlCountry\" runat=\"server\" EnableViewState=\"True\" MaxLength=\"100\" Style=\"width:" + width + "px;min-width:10px\" " + (mandCountry ? "CssClass=\"dnnFormRequired\"" : "") + " />";
                        validator = validator + (mandCountry ? "<asp:RequiredFieldValidator runat=\"server\" ID=\"valCountry\" CssClass=\"dnnFormMessage dnnFormError\" Display=\"Dynamic\" style=\"left:inherit\" Text=\"&#171;\" ControlToValidate=\"ddlCountry\" />" : "");

					}
					if (paraName == "PHONE")
					{
						caption = caption + (String.IsNullOrEmpty(caption) ? "" : " / ") + Localization.GetString("lblAdrEditPhone.Text", this.LocalResourceFile) + (mandPhone ? mandChar : "");
                        input = input + "<asp:TextBox id=\"txtAdrEditPhone\" runat=\"server\" EnableViewState=\"True\" MaxLength=\"20\" Style=\"width:" + width + "px;min-width:10px\" " + (mandPhone ? "CssClass=\"dnnFormRequired\"" : "") + " />";
                        validator = validator + (mandPhone ? "<asp:RequiredFieldValidator runat=\"server\" ID=\"valPhone\" CssClass=\"dnnFormMessage dnnFormError\" Display=\"Dynamic\" style=\"left:inherit\" Text=\"&#171;\" ControlToValidate=\"txtAdrEditPhone\" />" : "");

					}
					if (paraName == "CELL")
					{
						caption = caption + (String.IsNullOrEmpty(caption) ? "" : " / ") + Localization.GetString("lblAdrEditCell.Text", this.LocalResourceFile) + (mandCell ? mandChar : "");
                        input = input + "<asp:TextBox id=\"txtAdrEditCell\" runat=\"server\" EnableViewState=\"True\" MaxLength=\"20\" Style=\"width:" + width + "px;min-width:10px\" " + (mandCell ? "CssClass=\"dnnFormRequired\"" : "") + " />";
                        validator = validator + (mandCell ? "<asp:RequiredFieldValidator runat=\"server\" ID=\"valCell\" CssClass=\"dnnFormMessage dnnFormError\" Display=\"Dynamic\" style=\"left:inherit\" Text=\"&#171;\" ControlToValidate=\"txtAdrEditFax\" />" : "");

					}
					if (paraName == "FAX")
					{
						caption = caption + (String.IsNullOrEmpty(caption) ? "" : " / ") + Localization.GetString("lblAdrEditFax.Text", this.LocalResourceFile) + (mandFax ? mandChar : "");
                        input = input + "<asp:TextBox id=\"txtAdrEditFax\" runat=\"server\" EnableViewState=\"True\" MaxLength=\"20\" Style=\"width:" + width + "px;min-width:10px\" " + (mandFax ? "CssClass=\"dnnFormRequired\"" : "") + " />";
                        validator = validator + (mandFax ? "<asp:RequiredFieldValidator runat=\"server\" ID=\"valFax\" CssClass=\"dnnFormMessage dnnFormError\" Display=\"Dynamic\" style=\"left:inherit\" Text=\"&#171;\" ControlToValidate=\"txtAdrEditCell\" />" : "");

					}
					if (paraName == "EMAIL")
					{
						caption = caption + (String.IsNullOrEmpty(caption) ? "" : " / ") + Localization.GetString("lblAdrEditEmail.Text", this.LocalResourceFile) + (mandEmail ? mandChar : "");
                        input = input + "<asp:TextBox id=\"txtAdrEditEmail\" runat=\"server\" EnableViewState=\"True\" MaxLength=\"120\" Style=\"width:" + width + "px;min-width:10px\" " + (mandEmail ? "CssClass=\"dnnFormRequired\"" : "") + " />";
                        validator = validator + (mandEmail ? "<asp:RequiredFieldValidator runat=\"server\" ID=\"valEmail\" CssClass=\"dnnFormMessage dnnFormError\" Display=\"Dynamic\" style=\"left:inherit\" Text=\"&#171;\" ControlToValidate=\"txtAdrEditEmail\" />" : "");

					}
				}
				if (!String.IsNullOrEmpty(caption))
				{
					sb.Append("<span class=\"dnnFormLabel dnnLabel\">");
					sb.Append(caption);
					sb.Append("</span>");
				    sb.Append("<div class=\"dnnLeft\">");
					sb.Append(input);
					sb.Append(validator);
				    sb.Append("</div>");
				}
				sb.Append("</div>");
			}
			return sb.ToString();
		}
		#endregion
	}
}