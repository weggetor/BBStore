using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Bitboxx.License;
using DotNetNuke.Services.GeneratedImage;
using DotNetNuke.Common;
using DotNetNuke.Common.Lists;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

using Bitboxx.Web;
using DotNetNuke.UI.Skins.Controls;

namespace Bitboxx.DNNModules.BBStore
{
    [DNNtc.PackageProperties("BBStore Contact", 7, "BBStore Contact", "BBStore Contact", "BBStore.png", "Torsten Weggen", "bitboxx solutions", "http://www.bitboxx.net", "info@bitboxx.net", false)]
    [DNNtc.ModuleProperties("BBStore Contact", "BBStore Contact", 0)]
    [DNNtc.ModuleControlProperties("", "BBStore Contact", DNNtc.ControlType.View, "", true, false)]
	public partial class ViewContact : PortalModuleBase, IActionable
    {
        protected DotNetNuke.UI.UserControls.LabelControl lblContactCompany;
        protected DotNetNuke.UI.UserControls.LabelControl lblContactPrefix ;
        protected DotNetNuke.UI.UserControls.LabelControl lblContactFirstname;
        protected DotNetNuke.UI.UserControls.LabelControl lblContactLastname ;
        protected DotNetNuke.UI.UserControls.LabelControl lblContactStreet ;
        protected DotNetNuke.UI.UserControls.LabelControl lblContactRegion ;
        protected DotNetNuke.UI.UserControls.LabelControl lblContactPostalcode ;
        protected DotNetNuke.UI.UserControls.LabelControl lblContactCountry ;
        protected DotNetNuke.UI.UserControls.LabelControl lblContactTelephone ;
        protected DotNetNuke.UI.UserControls.LabelControl lblContactCell ;
        protected DotNetNuke.UI.UserControls.LabelControl lblContactFax ;
        protected DotNetNuke.UI.UserControls.LabelControl lblContactEmail;
        
        #region Fields
		private const string Currency = "EUR";
		private bool _hasChanged = false;
		BBStoreController _controller;
		List<SimpleProductInfo> Products;
        private string _template = "";
		#endregion

		#region Properties
		public BBStoreController Controller
		{
			get
			{
				if (_controller == null)
					_controller = new BBStoreController();
				return _controller;
			}
		}
		public Guid CartId
		{
			get
			{
				string _cartId;
				if (Request.Cookies["BBStoreCartId_" + PortalId.ToString()] != null)
					_cartId = (string)(Request.Cookies["BBStoreCartId_" + PortalId.ToString()].Value);
				else
				{
					_cartId = Guid.NewGuid().ToString();
					HttpCookie keks = new HttpCookie("BBStoreCartId_" + PortalId.ToString());
					keks.Value = _cartId;
					keks.Expires = DateTime.Now.AddDays(1);
					Response.AppendCookie(keks);
				}
				return new Guid(_cartId);
			}
		}
        
        public ModuleKindEnum ModuleKind
        {
            get { return ModuleKindEnum.Contact; }
        }

		protected string CurrentLanguage
		{
			get
			{
				return System.Threading.Thread.CurrentThread.CurrentCulture.Name;
			}
		}
		protected string DefaultLanguage
		{
			get
			{
				return this.PortalSettings.DefaultLanguage;
			}
		}
		protected bool IsConfigured
		{
			get
			{
				return (Settings["ShowCompany"] != null);
			}
		}

        //public ContactAddressInfo ContactAddress
        //{
        //    get
        //    {
        //        if (_contactAddress != null)
        //            return _contactAddress;

        //        if (Request.Cookies["ContactAddress"] != null)
        //        {
        //            ContactAddressInfo contact = (ContactAddressInfo) VfpInterop.DeserializeFromBase64String(Request.Cookies["ContactAddress"].Value);
        //            if (contact != null && contact.Lastname != null)
        //               _contactAddress = contact;
        //        }
        //        return null;
        //    }
        //    set
        //    {
        //        _contactAddress = value;
        //        string strVal = VfpInterop.SerializeToBase64String(value);
        //        if (Response.Cookies["ContactAddress"] != null)
        //            Response.Cookies["ContactAddress"].Value = strVal;
        //        else
        //        {
        //            HttpCookie cookie = new HttpCookie("ContactAddress");
        //            cookie.Value = strVal;
        //            Response.Cookies.Add(cookie);
        //        }
        //    }
        //}
		#endregion

		#region Event Handlers
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
			    if (!IsConfigured)
			    {
			        string message = Localization.GetString("Configure.Message", this.LocalResourceFile);
			        DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, message, ModuleMessage.ModuleMessageType.YellowWarning);
			        pnlContactView.Visible = false;
			    }
			    else
			    {
                    TemplateControl tp = LoadControl("controls/TemplateControl.ascx") as TemplateControl;
                    tp.Key = "Contact";
                    _template = tp.GetTemplate((string)Settings["ProductTemplate"]);
                    
                    if (!Page.IsPostBack)
			        {
			            ListController listController = new ListController();
			            IEnumerable<ListEntryInfo> countries = listController.GetListEntryInfoItems("Country");


			            Hashtable storeSettings = Controller.GetStoreSettings(PortalId);

			            ddlCountry.DataSource = countries;
			            ddlCountry.DataTextField = "Text";
			            ddlCountry.DataValueField = "Value";
			            ddlCountry.DataBind();

			            if (!String.IsNullOrEmpty((string) storeSettings["VendorCountry"]))
			                ddlCountry.SelectedValue = (string) storeSettings["VendorCountry"];

			            // Prefill data if logged in user
			            ContactAddressInfo contactAddress = null;
			            if (Request.Cookies["ContactAddress"] != null)
			                contactAddress = (ContactAddressInfo) VfpInterop.DeserializeFromBase64String(Request.Cookies["ContactAddress"].Value);

			            if (contactAddress == null)
			            {
			                if (Request.IsAuthenticated)
			                {
			                    UserInfo usr = UserController.GetUserById(PortalId, UserId);
			                    if (usr != null)
			                    {
			                        txtContactCompany.Text = (usr.Profile.GetPropertyValue("Company") ?? "");
			                        txtContactPrefix.Text = (usr.Profile.GetPropertyValue("Prefix") ?? "");
			                        txtContactFirstname.Text = (usr.FirstName ?? "");
			                        txtContactLastname.Text = (usr.LastName ?? "");
			                        txtContactUnit.Text = (usr.Profile.GetPropertyValue("Unit") ?? "");
			                        txtContactStreet.Text = (usr.Profile.Street ?? "");
			                        txtContactRegion.Text = (usr.Profile.GetPropertyValue("Region") ?? "");
			                        txtContactPostalcode.Text = (usr.Profile.PostalCode ?? "");
			                        txtContactCity.Text = (usr.Profile.City ?? "");
			                        if (ddlCountry.Items.FindByText(usr.Profile.Country ?? "") != null)
			                            ddlCountry.Items.FindByText(usr.Profile.Country ?? "").Selected = true;
			                        txtContactTelephone.Text = (usr.Profile.Telephone ?? "");
			                        txtContactFax.Text = (usr.Profile.Fax ?? "");
			                        txtContactEmail.Text = (usr.Email ?? "");
			                    }
			                }
			            }
			            else
			            {
			                txtContactCompany.Text = contactAddress.Company;
			                txtContactPrefix.Text = contactAddress.Prefix;
			                txtContactFirstname.Text = contactAddress.Firstname;
			                txtContactLastname.Text = contactAddress.Lastname;
			                txtContactUnit.Text = contactAddress.Unit;
			                txtContactStreet.Text = contactAddress.Street;
			                txtContactRegion.Text = contactAddress.Region;
			                txtContactPostalcode.Text = contactAddress.PostalCode;
			                txtContactCity.Text = contactAddress.City;
			                if (ddlCountry.Items.FindByText(contactAddress.Country) != null)
			                    ddlCountry.Items.FindByText(contactAddress.Country).Selected = true;
			                txtContactTelephone.Text = contactAddress.Telephone;
			                txtContactFax.Text = contactAddress.Fax;
			                txtContactEmail.Text = contactAddress.Email;
			                txtContactCell.Text = contactAddress.Cell;
			            }
			        }
			        string requiredText = Localization.GetString("Required.Validator", this.LocalResourceFile);
			        bool required;

			        trCompany.Visible = Convert.ToBoolean(Settings["ShowCompany"]);
			        lblContactCompany.Text = Localization.GetString("lblContactCompany.Text", this.LocalResourceFile) +
			                                 (Convert.ToBoolean(Settings["MandCompany"]) ? " *" : "");
			        txtContactCompany.TextChanged += txt_TextChanged;
			        required = Convert.ToBoolean(Settings["MandCompany"]);
			        valContactCompany.Visible = required;
			        if (required)
			        {
			            valContactCompany.Text = requiredText;
			            txtContactCompany.CssClass = "dnnFormRequired";
			        }

			        trPrefix.Visible = Convert.ToBoolean(Settings["ShowPrefix"]);
			        lblContactPrefix.Text = Localization.GetString("lblContactPrefix.Text", this.LocalResourceFile) +
			                                (Convert.ToBoolean(Settings["MandPrefix"]) ? " *" : "");
			        txtContactPrefix.TextChanged += txt_TextChanged;
			        required = Convert.ToBoolean(Settings["MandPrefix"]);
			        valContactPrefix.Visible = required;
			        if (required)
			        {
			            valContactPrefix.Text = requiredText;
			            txtContactPrefix.CssClass = "dnnFormRequired";
			        }

			        trFirstname.Visible = Convert.ToBoolean(Settings["ShowFirstname"]);
			        lblContactFirstname.Text = Localization.GetString("lblContactFirstname.Text", this.LocalResourceFile) +
			                                   (Convert.ToBoolean(Settings["MandFirstname"]) ? " *" : "");
			        txtContactFirstname.TextChanged += txt_TextChanged;
			        required = Convert.ToBoolean(Settings["MandFirstname"]);
			        valContactFirstname.Visible = required;
			        if (required)
			        {
			            valContactFirstname.Text = requiredText;
			            txtContactFirstname.CssClass = "dnnFormRequired";
			        }

			        trLastname.Visible = Convert.ToBoolean(Settings["ShowLastname"]);
			        lblContactLastname.Text = Localization.GetString("lblContactLastname.Text", this.LocalResourceFile) +
			                                  (Convert.ToBoolean(Settings["MandLastname"]) ? " *" : "");
			        txtContactLastname.TextChanged += txt_TextChanged;
			        required = Convert.ToBoolean(Settings["MandLastname"]);
			        valContactLastname.Visible = required;
			        if (required)
			        {
			            valContactLastname.Text = requiredText;
			            txtContactLastname.CssClass = "dnnFormRequired";
			        }

			        trStreet.Visible = Convert.ToBoolean(Settings["ShowStreet"]);
			        lblContactStreet.Text = Localization.GetString("lblContactStreet.Text", this.LocalResourceFile) +
			                                (Convert.ToBoolean(Settings["MandStreet"]) ? " *" : "");
			        txtContactStreet.TextChanged += txt_TextChanged;
			        required = Convert.ToBoolean(Settings["MandStreet"]);
			        valContactStreet.Visible = required;
			        if (required)
			        {
			            valContactStreet.Text = requiredText;
			            txtContactStreet.CssClass = "dnnFormRequired";
			        }

			        trRegion.Visible = Convert.ToBoolean(Settings["ShowRegion"]);
			        lblContactRegion.Text = Localization.GetString("lblContactRegion.Text", this.LocalResourceFile) +
			                                (Convert.ToBoolean(Settings["MandRegion"]) ? " *" : "");
			        txtContactRegion.TextChanged += txt_TextChanged;
			        required = Convert.ToBoolean(Settings["MandRegion"]);
			        valContactRegion.Visible = required;
			        if (required)
			        {
			            valContactRegion.Text = requiredText;
			            txtContactRegion.CssClass = "dnnFormRequired";
			        }

			        trCity.Visible = Convert.ToBoolean(Settings["ShowCity"]);
			        lblContactPostalcode.Text = Localization.GetString("lblContactPostalcode.Text", this.LocalResourceFile) +
			                                    (Convert.ToBoolean(Settings["MandCity"]) ? " *" : "");
			        txtContactCity.TextChanged += txt_TextChanged;
			        required = Convert.ToBoolean(Settings["MandCity"]);
			        valContactCity.Visible = required;
			        if (required)
			        {
			            valContactCity.Text = requiredText;
			            txtContactCity.CssClass = "dnnFormRequired";
			        }

			        trCountry.Visible = Convert.ToBoolean(Settings["ShowCountry"]);
			        lblContactCountry.Text = Localization.GetString("lblContactCountry.Text", this.LocalResourceFile) +
			                                 (Convert.ToBoolean(Settings["MandCountry"]) ? " *" : "");
			        ddlCountry.TextChanged += txt_TextChanged;
			        required = Convert.ToBoolean(Settings["MandCountry"]);
			        valContactCountry.Visible = required;
			        if (required)
			        {
			            valContactCountry.Text = requiredText;
			            ddlCountry.CssClass = "dnnFormRequired";
			        }

			        trPhone.Visible = Convert.ToBoolean(Settings["ShowPhone"]);
			        lblContactTelephone.Text = Localization.GetString("lblContactTelephone.Text", this.LocalResourceFile) +
			                                   (Convert.ToBoolean(Settings["MandPhone"]) ? " *" : "");
			        txtContactTelephone.TextChanged += txt_TextChanged;
			        required = Convert.ToBoolean(Settings["MandPhone"]);
			        valContactTelephone.Visible = required;
			        if (required)
			        {
			            valContactTelephone.Text = requiredText;
			            txtContactTelephone.CssClass = "dnnFormRequired";
			        }

			        trCell.Visible = Convert.ToBoolean(Settings["ShowCell"]);
			        lblContactCell.Text = Localization.GetString("lblContactCell.Text", this.LocalResourceFile) +
			                              (Convert.ToBoolean(Settings["MandCell"]) ? " *" : "");
			        txtContactCell.TextChanged += txt_TextChanged;
			        required = Convert.ToBoolean(Settings["MandCell"]);
			        valContactCell.Visible = required;
			        if (required)
			        {
			            valContactCell.Text = requiredText;
			            txtContactCell.CssClass = "dnnFormRequired";
			        }

			        trFax.Visible = Convert.ToBoolean(Settings["ShowFax"]);
			        lblContactFax.Text = Localization.GetString("lblContactFax.Text", this.LocalResourceFile) +
			                             (Convert.ToBoolean(Settings["MandFax"]) ? " *" : "");
			        txtContactFax.TextChanged += txt_TextChanged;
			        required = Convert.ToBoolean(Settings["MandFax"]);
			        valContactFax.Visible = required;
			        if (required)
			        {
			            valContactFax.Text = requiredText;
			            txtContactFax.CssClass = "dnnFormRequired";
			        }

			        trEmail.Visible = Convert.ToBoolean(Settings["ShowEmail"]);
			        lblContactEmail.Text = Localization.GetString("lblContactEmail.Text", this.LocalResourceFile) +
			                               (Convert.ToBoolean(Settings["MandEmail"]) ? " *" : "");
			        txtContactEmail.TextChanged += txt_TextChanged;
			        required = Convert.ToBoolean(Settings["MandEmail"]);
			        valContactEmail.Visible = required;
			        if (required)
			        {
			            valContactEmail.Text = requiredText;
			            txtContactEmail.CssClass = "dnnFormRequired";
			        }


			        Products = Controller.GetContactProductsByCartId(PortalId, CartId, CurrentLanguage);
			        if (Products.Count > 0)
			        {
			            lstProducts.DataSource = Products;
			            lstProducts.DataBind();
			        }
			        else
			            lstProducts.Visible = false;
			    }
			}
			catch (Exception exc)
			{
				Exceptions.ProcessModuleLoadException(this, exc);
			}
		}

        protected void Page_Prerender(object sender, EventArgs e)
        {
            // Check licensing
            LicenseDataInfo license = Controller.GetLicense(PortalId, false);
            Controller.CheckLicense(license, this, ModuleKind);
        }

		protected void lstProducts_ItemCreated(object sender, ListViewItemEventArgs e)
		{
			if (IsConfigured)
			{
                
                int imageWidth = 145;

				ListView lv = sender as ListView;
				ListViewDataItem item = e.Item as ListViewDataItem;
				SimpleProductInfo simpleProduct = item.DataItem as SimpleProductInfo;
				if (simpleProduct != null)
				{
                    string template = _template;

                    if (template.IndexOf("[IMAGE") > -1)
                    {
                        if (template.IndexOf("[IMAGE:") > -1)
                        {
                            string width;
                            width = template.Substring(template.IndexOf("[IMAGE:") + 7);
                            width = width.Substring(0, width.IndexOf("]"));
                            if (Int32.TryParse(width, out imageWidth) == false)
                                imageWidth = 200;
                            template = template.Replace("[IMAGE:" + width + "]", "<asp:PlaceHolder ID=\"phimgProduct\" runat=\"server\" />");
                        }
                        else
                            template = template.Replace("[IMAGE]", "<asp:PlaceHolder ID=\"phimgProduct\" runat=\"server\" />");
                    }
                    template = template.Replace("[ITEMNO]", simpleProduct.ItemNo);
				    template = template.Replace("[TITLE]", simpleProduct.Name);
                    template = template.Replace("[PRODUCTDESCRIPTION]", simpleProduct.ProductDescription);
                    template = template.Replace("[PRODUCTSHORTDESCRIPTION]", simpleProduct.ShortDescription);
                    template = template.Replace("[DELETE]", "<asp:ImageButton runat=\"server\" id=\"cmdDelete\" CommandName=\"Delete\" CausesValidation=\"False\" IconKey=\"Delete\"></asp:ImageButton>");
                    
					Control ctrl = ParseControl(template);

                    PlaceHolder phimgProduct = FindControlRecursive(ctrl, "phimgProduct") as PlaceHolder;
                    if (phimgProduct != null && simpleProduct.Image != null)
                    {
                        string fileName =
                                PortalSettings.HomeDirectoryMapPath.Replace(HttpContext.Current.Request.PhysicalApplicationPath, "") +
                                simpleProduct.Image.Replace('/', '\\');

                        GeneratedImage imgProduct = new GeneratedImage();
                        imgProduct.ImageHandlerUrl = "~/dnnImageHandler.ashx";
                        if (imageWidth > 0)
                            imgProduct.Parameters.Add(new ImageParameter() { Name = "w", Value = imageWidth.ToString() });
                        imgProduct.Parameters.Add(new ImageParameter() { Name = "file", Value = fileName });
						imgProduct.Parameters.Add(new ImageParameter() { Name = "mode", Value = "file" });
						// TODO: Watermark
						phimgProduct.Controls.Add(imgProduct);
                    }

                    PlaceHolder ph = e.Item.FindControl("productPlaceholder") as PlaceHolder;
					ph.Controls.Add(ctrl);
				}
			}
		}
		protected void txt_TextChanged(object sender, EventArgs e)
		{
			if (_hasChanged == false)
			{
    			ContactAddressInfo contactAddress = new ContactAddressInfo();
                contactAddress.Company = txtContactCompany.Text;
                contactAddress.Prefix = txtContactPrefix.Text;
                contactAddress.Firstname = txtContactFirstname.Text;
                contactAddress.Lastname = txtContactLastname.Text;
                contactAddress.Unit = txtContactUnit.Text;
                contactAddress.Street = txtContactStreet.Text;
                contactAddress.Region = txtContactRegion.Text;
                contactAddress.PostalCode = txtContactPostalcode.Text;
                contactAddress.City = txtContactCity.Text;
                contactAddress.Country = ddlCountry.SelectedValue;
                contactAddress.Telephone = txtContactTelephone.Text;
                contactAddress.Fax = txtContactFax.Text;
                contactAddress.Email = txtContactEmail.Text;
                contactAddress.Cell = txtContactCell.Text;
                string strVal = VfpInterop.SerializeToBase64String(contactAddress);
                if (Response.Cookies["ContactAddress"] != null)
                    Response.Cookies["ContactAddress"].Value = strVal;
                else
                {
                    HttpCookie cookie = new HttpCookie("ContactAddress");
                    cookie.Value = strVal;
                    Response.Cookies.Add(cookie);
                }
				_hasChanged = true;
			}
		}

        protected void lstProducts_ItemDeleting(object sender, ListViewDeleteEventArgs e)
        {
            int productId = (int)lstProducts.DataKeys[e.ItemIndex].Value;
            Controller.DeleteContactProduct(CartId, productId);
            Response.Redirect(Globals.NavigateURL(TabId));
        }
        protected void cmdSend_Click(object sender, EventArgs e)
        {
            ContactAddressInfo contactAddress = new ContactAddressInfo();
            contactAddress.Company = txtContactCompany.Text;
            contactAddress.Prefix = txtContactPrefix.Text;
            contactAddress.Firstname = txtContactFirstname.Text;
            contactAddress.Lastname = txtContactLastname.Text;
            contactAddress.Unit = txtContactUnit.Text;
            contactAddress.Street = txtContactStreet.Text;
            contactAddress.Region = txtContactRegion.Text;
            contactAddress.PostalCode = txtContactPostalcode.Text;
            contactAddress.City = txtContactCity.Text;
            contactAddress.Country = ddlCountry.SelectedValue;
            contactAddress.Telephone = txtContactTelephone.Text;
            contactAddress.Fax = txtContactFax.Text;
            contactAddress.Email = txtContactEmail.Text;
            contactAddress.Cell = txtContactCell.Text;
            int ContactAddressId = Controller.NewContactAddress(contactAddress);
            contactAddress.ContactAddressId = ContactAddressId;
            foreach (var p in Products)
            {
                Controller.UpdateContactProduct(CartId, p.SimpleProductId, ContactAddressId);
            }
            ContactReasonInfo reason = new ContactReasonInfo(ContactAddressId, txtRequest.Text, "Text");
            MailRequest(contactAddress);
            Controller.NewContactReason(reason);
            pnlContactData.Visible = false;
            pnlProducts.Visible = false;
            pnlRequestData.Visible = false;
            pnlConfirmData.Visible = true;
            pnlSend.Visible = false;
        }
        protected void cmdReturn_Click(object sender, EventArgs e)
        {
            int tabId = Convert.ToInt32(Settings["ShopHome"]);
            Response.Redirect(Globals.NavigateURL(tabId));
        }

		#endregion

		#region Helper Methods
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

        private void MailRequest(ContactAddressInfo contactAddress)
        {
            Hashtable storeSettings = Controller.GetStoreSettings(PortalId);

            string storeEmail = (string)storeSettings["StoreEmail"] ?? "";
            string storeName = (string)storeSettings["StoreName"] ?? "";
            string storeReplyTo = (string)storeSettings["StoreReplyTo"] ?? "";
            string storeAdmin = (string)storeSettings["StoreAdmin"] ?? "";
            string vendorName = (string)storeSettings["VendorName"] ?? "";
            string vendorStreet1 = (string)storeSettings["VendorStreet1"] ?? "";
            string vendorStreet2 = (string)storeSettings["VendorStreet2"] ?? "";
            string vendorZip = (string)storeSettings["VendorZip"] ?? "";
            string vendorCity = (string)storeSettings["VendorCity"] ?? "";
            string shippingType = (string)storeSettings["ShippingType"] ?? "";
            string headerMessage = Localization.GetString("EmailHeader.Message", this.LocalResourceFile);
            string footerMessage = Localization.GetString("EmailFooter.Message", this.LocalResourceFile);

            TemplateControl tp = LoadControl("controls/TemplateControl.ascx") as TemplateControl;
            tp.Key = "Request";
            string template = tp.GetTemplate((string)(Settings["Template"] ?? "Request"));

            template = template.Replace("[BBSTORE-VENDORIMAGE]",
                (PortalSettings.LogoFile != string.Empty ? "<img src=\"cid:Logo\" />" : ""));


            template = template.Replace("[BBSTORE-HEADERMESSAGE]", headerMessage);

            template = template.Replace("[BBSTORE-ORDERTEXT]", Localization.GetString("EmailOrder.Text", this.LocalResourceFile));
            template = template.Replace("[BBSTORE-INFOTEXT]", Localization.GetString("EmailInfo.Text", this.LocalResourceFile));
            template = template.Replace("[BBSTORE-ADDRESSINFOTEXT]", Localization.GetString("EmailAddressInfo.Text", this.LocalResourceFile));

            template = template.Replace("[BBSTORE-PRODUCTSTEXT]", Localization.GetString("EmailProducts.Text", this.LocalResourceFile));
            template = template.Replace("[BBSTORE-COMMENTSTEXT]", Localization.GetString("EmailComments.Text", this.LocalResourceFile));
            template = template.Replace("[BBSTORE-FIRMATEXT]", Localization.GetString("EmailCompany.Text", this.LocalResourceFile));
            template = template.Replace("[BBSTORE-NAMETEXT]", Localization.GetString("EmailName.Text", this.LocalResourceFile));
            template = template.Replace("[BBSTORE-ADDRESSTEXT]", Localization.GetString("EmailAddress.Text", this.LocalResourceFile));
            template = template.Replace("[BBSTORE-CITYTEXT]", Localization.GetString("EmailCity.Text", this.LocalResourceFile));
            template = template.Replace("[BBSTORE-ZIPTEXT]", Localization.GetString("EmailZip.Text", this.LocalResourceFile));
            template = template.Replace("[BBSTORE-COUNTRYTEXT]", Localization.GetString("EmailCountry.Text", this.LocalResourceFile));
            template = template.Replace("[BBSTORE-PHONETEXT]", Localization.GetString("EmailPhone.Text", this.LocalResourceFile));
            template = template.Replace("[BBSTORE-FAXTEXT]", Localization.GetString("EmailFax.Text", this.LocalResourceFile));
            template = template.Replace("[BBSTORE-EMAILTEXT]", Localization.GetString("EmailEmail.Text", this.LocalResourceFile));
            template = template.Replace("[BBSTORE-CELLTEXT]", Localization.GetString("EmailCell.Text", this.LocalResourceFile));

            template = template.Replace("[BBSTORE-REQUESTNOTEXT]", Localization.GetString("EmailRequestNo.Text", this.LocalResourceFile));
            template = template.Replace("[BBSTORE-REQUESTDATETEXT]", Localization.GetString("EmailRequestDate.Text", this.LocalResourceFile));
            template = template.Replace("[BBSTORE-REQUESTNO]", contactAddress.ContactAddressId.ToString());
            template = template.Replace("[BBSTORE-REQUESTDATE]", DateTime.Now.Date.ToString("D"));

            template = template.Replace("[BBSTORE-REQCOMPANY]", contactAddress.Company);
            template = template.Replace("[BBSTORE-REQNAME]", (contactAddress.Prefix + " " + contactAddress.Firstname + " " + contactAddress.Lastname).Trim());
            template = template.Replace("[BBSTORE-REQSTREET]", (contactAddress.Street + " " + contactAddress.Unit).Trim());
            template = template.Replace("[BBSTORE-REQCITY]", contactAddress.City);
            template = template.Replace("[BBSTORE-REQZIP]", contactAddress.PostalCode);
            template = template.Replace("[BBSTORE-REQCOUNTRY]", contactAddress.Country);
            template = template.Replace("[BBSTORE-REQPHONE]", contactAddress.Telephone);
            template = template.Replace("[BBSTORE-REQCELL]", contactAddress.Cell);
            template = template.Replace("[BBSTORE-REQFAX]", contactAddress.Fax);
            template = template.Replace("[BBSTORE-REQEMAIL]", contactAddress.Email);

            template = template.Replace("[BBSTORE-VENDORNAME]", vendorName);
            template = template.Replace("[BBSTORE-VENDORSTREET1]", vendorStreet1);
            template = template.Replace("[BBSTORE-VENDORSTREET2]", vendorStreet2);
            template = template.Replace("[BBSTORE-VENDORZIP]", vendorZip);
            template = template.Replace("[BBSTORE-VENDORCITY]", vendorCity);

            string requestItems = "<tr class=\"Normal\" style=\"background-color:#ECECEC\">" +
               "  <th style=\"vertical-align:top;text-align:left\">" + Localization.GetString("ItemNo.Header", this.LocalResourceFile) + "</th>" +
               "  <th style=\"vertical-align:top;text-align:left\">" + Localization.GetString("Product.Header", this.LocalResourceFile) + "</th>" +
               "</tr>";

            string productTemplate = "<tr class=\"Normal\" style=\"[STYLE]\">" +
                "  <td style=\"vertical-align:top;text-align:left\">[PRODUCTITEMNO]</td>" +
                "  <td style=\"vertical-align:top;text-align:left\">[PRODUCTNAME]<br><span style=\"font-size:10px\">[PRODUCTSHORTDESCRIPTION]</span></td>" +
                "</tr>";

            int loop = 0;
            foreach (SimpleProductInfo product in Products)
            {
                requestItems += productTemplate;
                string artname = product.Name.Trim();
                requestItems = requestItems.Replace("[PRODUCTNAME]", product.Name.Trim());
                requestItems = requestItems.Replace("[PRODUCTITEMNO]", product.ItemNo.Trim());
                requestItems = requestItems.Replace("[PRODUCTSHORTDESCRIPTION]", product.ShortDescription);
                if (loop % 2 == 0)
                    requestItems = requestItems.Replace("[STYLE]", "background-color:#F8F8F8");
                else
                    requestItems = requestItems.Replace("[STYLE]", "background-color:#FFFFFF");
                loop++;
            }
            template = template.Replace("[BBSTORE-REQUESTITEMS]", requestItems);

            template = template.Replace("[BBSTORE-COMMENT]", txtRequest.Text);
            template = template.Replace("[BBSTORE-FOOTERMESSAGE]", footerMessage);

            try
            {
                // http://www.systemnetmail.com

                MailMessage mail = new MailMessage();

                //set the addresses
                string smtpServer = DotNetNuke.Entities.Host.Host.SMTPServer;
                string smtpAuthentication = DotNetNuke.Entities.Host.Host.SMTPAuthentication;

                string smtpUsername = DotNetNuke.Entities.Host.Host.SMTPUsername;
                string smtpPassword = DotNetNuke.Entities.Host.Host.SMTPPassword;

                mail.From = new MailAddress("\"" + storeName.Trim() + "\" <" + storeEmail.Trim() + ">");
                mail.To.Add(contactAddress.Email);
                if (storeAdmin != string.Empty)
                    mail.To.Add(storeAdmin.Trim());
                if (storeReplyTo != string.Empty)
                    mail.ReplyToList.Add(new MailAddress(storeReplyTo.Trim()));

                //set the content
                mail.Subject = ((string)Settings["EmailSubject"]).Replace("[REQUESTNO]", contactAddress.ContactAddressId.ToString());

                AlternateView av1 = AlternateView.CreateAlternateViewFromString(template, null, "text/html");
                string logoFile = MapPath(PortalSettings.HomeDirectory + PortalSettings.LogoFile);

                if (PortalSettings.LogoFile != string.Empty && File.Exists(logoFile))
                {
                    LinkedResource linkedResource = new LinkedResource(logoFile);
                    linkedResource.ContentId = "Logo";
                    linkedResource.ContentType.Name = logoFile;
                    linkedResource.ContentType.MediaType = "image/jpeg";
                    av1.LinkedResources.Add(linkedResource);
                }
                mail.AlternateViews.Add(av1);
                mail.IsBodyHtml = true;

                SmtpClient emailClient = new SmtpClient(smtpServer);
                if (smtpAuthentication == "1")
                {
                    System.Net.NetworkCredential SMTPUserInfo = new System.Net.NetworkCredential(smtpUsername, smtpPassword);
                    emailClient.UseDefaultCredentials = false;
                    emailClient.Credentials = SMTPUserInfo;
                }
                emailClient.Send(mail);
            }
            catch (SmtpException sex)
            {
                Exceptions.LogException(sex);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
            }
        }

		#endregion

		#region IActionable
		public ModuleActionCollection ModuleActions
		{
			get
			{
				ModuleActionCollection Actions = new ModuleActionCollection();
				//Actions.Add(GetNextActionID(), Localization.GetString(ModuleActionType.AddContent, this.LocalResourceFile),
				//   ModuleActionType.AddContent, "", "add.gif", EditUrl(), false, DotNetNuke.Security.SecurityAccessLevel.Edit,
				//    true, false);
				return Actions;
			}
		}
		#endregion
	}
}