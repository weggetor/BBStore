using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Common.Lists;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.ControlPanels;
using DotNetNuke.UI.Skins.Controls;

namespace Bitboxx.DNNModules.BBStore
{
	public partial class ViewCartSelectAddresses : PortalModuleBase
	{
		private BBStoreController _controller;
		private List<SubscriberAddressTypeInfo> _addressTypes;
		private string _mandatoryAddressTypes = "";
		
		public BBStoreController Controller
		{
			get
			{
				if (_controller == null)
					_controller = new BBStoreController();
				return _controller;
			}
		}
		public ViewCart MainControl { get; set; }
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

		protected void Page_Load(object sender, EventArgs e)
		{
            cmdAdrUse.CssClass = (string)Settings["CheckoutButtonCssClass"] ?? "";

            // Is he logged in ?
            if (!Request.IsAuthenticated)
			{
				// Attention ! returnUrl must be relative path (cross-site-scripting denying)
				string returnUrl = HttpContext.Current.Request.RawUrl;
				returnUrl = HttpUtility.UrlEncode(returnUrl);

				Response.Redirect(Globals.NavigateURL(TabId, "", "action=login", "returnurl=" + returnUrl));
			}
			else
			{
				//Lets check if we have a customer (only if logged in)
				int userId = UserController.Instance.GetCurrentUserInfo().UserID;
				List<CustomerInfo> customers = Controller.GetCustomersByUserId(PortalId, userId);
				if (customers.Count == 0)
				{
					int customerId = Controller.NewCustomer(new CustomerInfo(userId, PortalId, UserController.Instance.GetCurrentUserInfo().Username));
					Controller.UpdateCartCustomerId(this.MainControl.CartId, customerId);
				}

			}
			_addressTypes = Controller.GetSubscriberAddressTypes(PortalId, 0, CurrentLanguage);
			foreach (var addressType in _addressTypes)
			{
				if (addressType.Mandatory)
				{
					_mandatoryAddressTypes += (String.IsNullOrEmpty(_mandatoryAddressTypes) ? "" : ", ") + addressType.AddressType;
				}
			}

			_mandatoryAddressTypes = String.IsNullOrEmpty(_mandatoryAddressTypes) ? Localization.GetString("NoMandatoryTypes.Text", this.LocalResourceFile) : _mandatoryAddressTypes;
			lblIntro.Text = String.Format(Localization.GetString("Intro.Message", this.LocalResourceFile), _mandatoryAddressTypes);
			pnlIntro.Attributes.Add("class", "dnnFormMessage dnnFormInfo");

		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
	
			// Do we have an existing Main Address ?
			CustomerAddressInfo customerAddress = Controller.GetCustomerMainAddress(this.MainControl.CustomerId);
			if (customerAddress == null)
			{
				List<CustomerAddressInfo> customerSecondaryAddresses = Controller.GetCustomerAddresses(this.MainControl.CustomerId);

				if (customerSecondaryAddresses.Count > 0)
				{
					customerAddress = customerSecondaryAddresses[0];
					customerAddress.IsDefault = true;
					Controller.UpdateCustomerAddress(customerAddress);
				}
				else
				{
					// Create New Address
					customerAddress = new CustomerAddressInfo();

					UserInfo usr = UserController.GetUserById(PortalId, UserId);
					if (usr == null)
						throw new Exception("Userdata not found");

					customerAddress.CustomerId = this.MainControl.CustomerId;
					customerAddress.PortalId = PortalId;
					customerAddress.Company = (usr.Profile.GetPropertyValue("Company") ?? "");
					customerAddress.Prefix = (usr.Profile.GetPropertyValue("Prefix") ?? "");
					customerAddress.Firstname = (usr.FirstName ?? "");
					customerAddress.Middlename = (usr.Profile.GetPropertyValue("Middlename") ?? "");
					customerAddress.Lastname = (usr.LastName ?? "");
					customerAddress.Suffix = (usr.Profile.GetPropertyValue("Suffix") ?? "");
					customerAddress.Unit = (usr.Profile.GetPropertyValue("Unit") ?? "");
					customerAddress.Street = (usr.Profile.Street ?? "");
					customerAddress.Region = (usr.Profile.GetPropertyValue("Region") ?? "");
					customerAddress.PostalCode = (usr.Profile.PostalCode ?? "");
					customerAddress.City = (usr.Profile.City ?? "");
					customerAddress.Suburb = (usr.Profile.GetPropertyValue("Suburb") ?? "");
					customerAddress.Country = (usr.Profile.Country ?? "");
					customerAddress.CountryCode = GetCountryCode(customerAddress.Country);
					customerAddress.Cell = (usr.Profile.Cell ?? "");
					customerAddress.Telephone = (usr.Profile.Telephone ?? "");
					customerAddress.Fax = (usr.Profile.Fax ?? "");
					customerAddress.Email = (usr.Email ?? "");
					customerAddress.IsDefault = true;
					customerAddress.CustomerAddressId = Controller.NewCustomerAddress(customerAddress);

					// Add this address to all mandatory subscriberaddresstypes
					foreach (var addressType in _addressTypes)
					{
						if (addressType.Mandatory)
						{
							CartAddressInfo cartAddress = new CartAddressInfo()
								{
									CartID = this.MainControl.CartId,
									CustomerAddressId = customerAddress.CustomerAddressId,
									SubscriberAddressTypeId = addressType.SubscriberAddressTypeId
								};
							Controller.NewCartAddress(cartAddress);
						}
					}
					// Edit this address
					Response.Redirect(Globals.NavigateURL(TabId, "", "action=adredit", "adrid=" + customerAddress.CustomerAddressId.ToString()));
				}
			}
			
			// Now lets retrieve all adresses of the customer
			List<CustomerAddressInfo> customerAddresses = Controller.GetCustomerAddresses(this.MainControl.CustomerId);

			// Check if all adresses meet the requirements
			foreach (CustomerAddressInfo customerCheckAddress in customerAddresses)
			{
				if ((Settings["MandCompany"] != null && Convert.ToBoolean(Settings["MandCompany"]) && String.IsNullOrEmpty(customerCheckAddress.Company)) ||
					(Settings["MandPrefix"] != null && Convert.ToBoolean(Settings["MandPrefix"]) && String.IsNullOrEmpty(customerCheckAddress.Prefix)) ||
					(Settings["MandFirstName"] != null && Convert.ToBoolean(Settings["MandFirstName"]) && String.IsNullOrEmpty(customerCheckAddress.Firstname)) ||
					(Settings["MandMiddleName"] != null && Convert.ToBoolean(Settings["MandMiddleName"]) && String.IsNullOrEmpty(customerCheckAddress.Middlename)) ||
					(Settings["MandLastName"] != null && Convert.ToBoolean(Settings["MandLastName"]) && String.IsNullOrEmpty(customerCheckAddress.Lastname)) ||
					(Settings["MandSuffix"] != null && Convert.ToBoolean(Settings["MandSuffix"]) && String.IsNullOrEmpty(customerCheckAddress.Suffix)) ||
					(Settings["MandStreet"] != null && Convert.ToBoolean(Settings["MandStreet"]) && String.IsNullOrEmpty(customerCheckAddress.Street)) ||
					(Settings["MandUnit"] != null && Convert.ToBoolean(Settings["MandUnit"]) && String.IsNullOrEmpty(customerCheckAddress.Unit)) ||
					(Settings["MandRegion"] != null && Convert.ToBoolean(Settings["MandRegion"]) && String.IsNullOrEmpty(customerCheckAddress.Region)) ||
					(Settings["MandPostalCode"] != null && Convert.ToBoolean(Settings["MandPostalCode"]) && String.IsNullOrEmpty(customerCheckAddress.PostalCode)) ||
					(Settings["MandCity"] != null && Convert.ToBoolean(Settings["MandCity"]) && String.IsNullOrEmpty(customerCheckAddress.City)) ||
					(Settings["MandSuburb"] != null && Convert.ToBoolean(Settings["MandSuburb"]) && String.IsNullOrEmpty(customerCheckAddress.Suburb)) ||
					(Settings["MandCountry"] != null && Convert.ToBoolean(Settings["MandCountry"]) && String.IsNullOrEmpty(customerCheckAddress.Country)) ||
					(Settings["MandTelephone"] != null && Convert.ToBoolean(Settings["MandTelephone"]) && String.IsNullOrEmpty(customerCheckAddress.Telephone)) ||
					(Settings["MandCell"] != null && Convert.ToBoolean(Settings["MandCell"]) && String.IsNullOrEmpty(customerCheckAddress.Cell)) ||
					(Settings["MandFax"] != null && Convert.ToBoolean(Settings["MandFax"]) && String.IsNullOrEmpty(customerCheckAddress.Fax)) ||
					(Settings["MandEmail"] != null && Convert.ToBoolean(Settings["MandEmail"]) && String.IsNullOrEmpty(customerCheckAddress.Email)))
				{
					Response.Redirect(Globals.NavigateURL(TabId, "", "action=adredit", "adrid=" + customerCheckAddress.CustomerAddressId.ToString()));
				}
			}

			customerAddresses.Add(new CustomerAddressInfo());

			//if (customerAddresses.Count > 0)
			{
				lstCustomerAddresses.Visible = true;

				lstCustomerAddresses.DataSource = customerAddresses;
				lstCustomerAddresses.DataBind();
			}
			//else
			//{
			//    lstCustomerAddresses.Visible = false;
			//}

		}

		protected void lstCustomerAddresses_ItemDataBound(object sender, ListViewItemEventArgs e)
		{
			if (e.Item.ItemType == ListViewItemType.DataItem)
			{
				ListViewDataItem dataItem = (ListViewDataItem)e.Item;
				CustomerAddressInfo lstCustomerAddress = dataItem.DataItem as CustomerAddressInfo;

				HtmlTableCell cellAddress = e.Item.FindControl("cellAddress") as HtmlTableCell;
				cellAddress.Style.Add("width", (100/lstCustomerAddresses.GroupItemCount).ToString() + "%");
				
				Label lblAdress = e.Item.FindControl("lblAddress") as Label;
				ImageButton imgAdrEditlst = e.Item.FindControl("imgAdrEditlst") as ImageButton;
				LinkButton cmdAdrEditlst = e.Item.FindControl("cmdAdrEditlst") as LinkButton;
				ImageButton imgAdrDeletelst = e.Item.FindControl("imgAdrDeletelst") as ImageButton;
				LinkButton cmdAdrDeletelst = e.Item.FindControl("cmdAdrDeletelst") as LinkButton;
				ImageButton imgAdrNewlst = e.Item.FindControl("imgAdrNewlst") as ImageButton;
				LinkButton cmdAdrNewlst = e.Item.FindControl("cmdAdrNewlst") as LinkButton;
				
				CheckBoxList lstAddressType = e.Item.FindControl("lstAddresstype") as CheckBoxList;
				HiddenField hidCustomerAddressId = e.Item.FindControl("hidCustomerAddressId") as HiddenField;

				string template = Localization.GetString("AddressTemplate.Text", this.LocalResourceFile.Replace("ViewCartSelectAddresses","ViewCartAddressEdit"));
				lblAdress.Text = lstCustomerAddress.ToHtml(template,true);


				string strCustomerAddressId = lstCustomerAddress.CustomerAddressId.ToString();
				hidCustomerAddressId.Value = strCustomerAddressId;

				imgAdrEditlst.CommandArgument = strCustomerAddressId;
				cmdAdrEditlst.CommandArgument = strCustomerAddressId;
				imgAdrDeletelst.CommandArgument = strCustomerAddressId;
				cmdAdrDeletelst.CommandArgument = strCustomerAddressId;

				lstAddressType.DataSource = _addressTypes;
				lstAddressType.DataBind();

				List<CartAddressInfo> adresses = Controller.GetCartAddressesByAddressId(this.MainControl.CartId, lstCustomerAddress.CustomerAddressId);
				bool isSelected = false;

				foreach (CartAddressInfo cartAddressInfo in adresses)
				{
					
					foreach (ListItem chk in lstAddressType.Items)
					{
						if (chk.Value == cartAddressInfo.SubscriberAddressTypeId.ToString())
						{
							chk.Selected = true;
							isSelected = true;
							break;
						}
					}
				}

				if (lstCustomerAddress.CustomerAddressId > -1)
				{
                    if (isSelected || Controller.HasOrderAddress(lstCustomerAddress.CustomerAddressId))
					{
						imgAdrDeletelst.Visible = false;
						cmdAdrDeletelst.Visible = false;
					}
					imgAdrNewlst.Visible = false;
					cmdAdrNewlst.Visible = false;
				}
				else
				{
					lstAddressType.Enabled = false;
					imgAdrDeletelst.Visible = false;
					cmdAdrDeletelst.Visible = false;
					imgAdrEditlst.Visible = false;
					cmdAdrEditlst.Visible = false;
					imgAdrNewlst.Visible = true;
					cmdAdrNewlst.Visible = true;
				}
			}
		}

		protected void cmdAdrUse_Click(object sender, EventArgs e)
		{
		    foreach (ListViewDataItem dataItem in lstCustomerAddresses.Items)
		    {
                HiddenField hidCustomerAddressId = dataItem.FindControl("hidCustomerAddressId") as HiddenField;
                int customerAddressId = Convert.ToInt32(hidCustomerAddressId.Value);
		        CheckBoxList cbl = dataItem.FindControl("lstAddressType") as CheckBoxList;
                foreach (ListItem chk in cbl.Items)
                {
                    int subscriberAddressId = Convert.ToInt32(chk.Value);
                    Controller.UpdateCartAddressType(this.MainControl.CartId, PortalId, 0, customerAddressId, subscriberAddressId, chk.Selected);
                }
		    }
            
            if (Controller.CheckCartAddresses(this.MainControl.CartId, PortalId, 0))
				Response.Redirect(Globals.NavigateURL(TabId, "", "action=shipping"));
			else
			{
				lblIntro.Text  = String.Format(Localization.GetString("MissingAddress.Message", this.LocalResourceFile), _mandatoryAddressTypes);
				pnlIntro.Attributes.Add("class","dnnFormMessage dnnFormWarning");
			}
		}
		protected void cmdAdrEdit_Click(object sender, EventArgs e)
		{
			string strAddressId;
			LinkButton cmd = sender as LinkButton;
			if (cmd != null)
				strAddressId = cmd.CommandArgument;
			else
			{
				ImageButton img = sender as ImageButton;
				strAddressId = img.CommandArgument;
			}
			int CustomerAdressId = Convert.ToInt32(strAddressId);
			Response.Redirect(Globals.NavigateURL(TabId, "", "action=adredit", "adrid=" + CustomerAdressId.ToString()));
		}

		protected void cmdAdrDelete_Click(object sender, EventArgs e)
		{
			string strAddressId;
			LinkButton cmd = sender as LinkButton;
			if (cmd != null)
				strAddressId = cmd.CommandArgument;
			else
			{
				ImageButton img = sender as ImageButton;
				strAddressId = img.CommandArgument;
			}
			int customerAdressId = Convert.ToInt32(strAddressId);

            BBStoreImportController importController = new BBStoreImportController();
		    try
		    {
		        importController.DeleteCustomerAddress(PortalId, customerAdressId);
		        Response.Redirect(Globals.NavigateURL(TabId, "", "action=checkout"));
		    }
		    catch (SqlException)
		    {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("DeleteCustomerAddress.Error", this.LocalResourceFile), ModuleMessage.ModuleMessageType.RedError);
            }
		    catch (Exception ex)
		    {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, ex.Message, ModuleMessage.ModuleMessageType.RedError);
		    }
		}

		protected void cmdAdrNew_Click(object sender, EventArgs e)
		{
			Response.Redirect(Globals.NavigateURL(TabId, "", "action=adredit"));
		}

		#region Helper methods
		private string GetCountryCode(string Country)
		{
			ListController oController = new ListController();
			IEnumerable<ListEntryInfo> oCountries = oController.GetListEntryInfoItems("Country");
			foreach (ListEntryInfo oCountry in oCountries)
			{
				if (oCountry.Text.ToLower() == Country.ToLower())
				{
					return oCountry.Value;
				}
			}
			return "";
		}
		#endregion
	}
}