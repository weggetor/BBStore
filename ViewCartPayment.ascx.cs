using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

using Bitboxx.DNNModules.BBStore.Providers.Payment;

namespace Bitboxx.DNNModules.BBStore
{
	partial class ViewCartPayment : PortalModuleBase
	{
		#region Fields
		private BBStoreController _controller;
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

		protected string CurrentLanguage
		{
		    get { return System.Threading.Thread.CurrentThread.CurrentCulture.Name; }
		}

		protected string DefaultLanguage
		{
		    get { return this.PortalSettings.DefaultLanguage; }
		}

        public ViewCart MainControl { get; set; }

	    public List<PaymentProviderInfo> PaymentProvider
	    {
	        get
	        {
	            if (ViewState["PaymentProvider"] != null)
	                return (List<PaymentProviderInfo>)ViewState["PaymentProvider"];
                return new List<PaymentProviderInfo>();
	        }
            set { ViewState["PaymentProvider"] = value; }
	    }
        public List<SubscriberPaymentProviderInfo> SubscriberPaymentProvider
         {
	        get
	        {
	            if (ViewState["SubscriberPaymentProvider"] != null)
                    return (List<SubscriberPaymentProviderInfo>)ViewState["SubscriberPaymentProvider"];
                return new List<SubscriberPaymentProviderInfo>();
	        }
             set { ViewState["SubscriberPaymentProvider"] = value; }
	    }
        
        #endregion

		#region Event Handlers
		
		/// ----------------------------------------------------------------------------- 
		/// <summary> 
		/// Page_Load runs when the control is loaded 
		/// </summary> 
		/// ----------------------------------------------------------------------------- 
		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
			    DotNetNuke.Framework.jQuery.RequestUIRegistration();

                // Is he logged in ?
                if (!Request.IsAuthenticated)
                {
                    // Attention ! returnUrl must be relative path (cross-site-scripting denying)
                    string returnUrl = HttpContext.Current.Request.RawUrl;
                    returnUrl = HttpUtility.UrlEncode(returnUrl);

                    Response.Redirect(Globals.NavigateURL(TabId, "", "action=login", "returnurl=" + returnUrl));
                }

			    if (!IsPostBack)
			    {
			        PaymentProvider = Controller.GetPaymentProviders(CurrentLanguage);
			        if (PaymentProvider.Count == 0)
			            PaymentProvider = Controller.GetPaymentProviders(DefaultLanguage);

			        // We only need enabled Providers with a role property the actual logged User is part of
			        SubscriberPaymentProvider = Controller.GetSubscriberPaymentProviders(PortalId, 0).FindAll(pp => pp.IsEnabled == true && (pp.Role == String.Empty || UserInfo.IsInRole(pp.Role)));

			        SubscriberPaymentProviderComparer comp = new SubscriberPaymentProviderComparer();
			        SubscriberPaymentProvider.Sort(comp);

                    // Coming from external payment ?
                    if (Request["result"] == "success")
                    {
                        int subscriberPaymentProviderId = Convert.ToInt32(Request["provider"]);
                        var selectedSubscriberPaymentProvider = (from s in SubscriberPaymentProvider
                                                                 where s.SubscriberPaymentProviderId == subscriberPaymentProviderId
                                                                 select s).FirstOrDefault();
                        if (selectedSubscriberPaymentProvider != null && selectedSubscriberPaymentProvider.PaymentProviderId == 5) // paypal
                        {
                            VerifyPaypalCheckout(selectedSubscriberPaymentProvider);
                        }
                    }
                    if (Request["result"] == "cancel")
                    {
                        // Wir setzen den gewählten Paymentprovider zurück
                        Controller.UpdateCartCustomerPaymentProviderId(MainControl.CartId,-1);
                        MainControl.Cart.CustomerPaymentProviderID = -1;
                    }
			    }
                lstPaymentProvider.DataSource = SubscriberPaymentProvider;
                lstPaymentProvider.DataBind();
			}

			catch (Exception exc)
			{
				//Module failed to load 
				Exceptions.ProcessModuleLoadException(this, exc);
			}
		}

	    protected void lstPaymentProvider_ItemDataBound(object sender, ListViewItemEventArgs e)
	    {
            ListView lv = sender as ListView;
            ListViewDataItem item = e.Item as ListViewDataItem;
            SubscriberPaymentProviderInfo spp = item.DataItem as SubscriberPaymentProviderInfo;
	        if (spp != null)
	        {
	            PlaceHolder ph = e.Item.FindControl("phPaymentProvider") as PlaceHolder;
	            PaymentProviderInfo pp = PaymentProvider.Find(p => p.PaymentProviderId == spp.PaymentProviderId);
                
	            if (pp != null)
	            {
	                Literal ltrTitle = e.Item.FindControl("ltrTitle") as Literal;
                    ltrTitle.Text = "<h3 class=\"bbstore-cart-payment-header\">" + pp.ProviderName +"</h3>";
                    
                    PaymentProviderBase ctrl = this.LoadControl(@"~\DesktopModules\BBStore\Providers\Payment\" + pp.ProviderControl.Trim() + ".ascx") as PaymentProviderBase;
	                ctrl.DisplayMode = ViewMode.View;
	                ctrl.Title = pp.ProviderName;
	                ctrl.EnableViewState = true;
	                ctrl.Properties = spp.PaymentProviderProperties;
	                ctrl.Cost = spp.Cost;
	                ctrl.CostPercent = spp.CostPercent;
	                ctrl.TaxPercent = spp.TaxPercent;
	                ctrl.PaymentProviderId = spp.PaymentProviderId;
                    ctrl.ShowNetprice = MainControl.ShowNetPrice;
                    ctrl.ID = "pp" + spp.SubscriberPaymentProviderId.ToString();

	                CustomerPaymentProviderInfo cusPP = Controller.GetCustomerPaymentProvider(MainControl.CustomerId, spp.PaymentProviderId);
	                if (cusPP != null)
	                {
	                    ctrl.Values = cusPP.PaymentProviderValues;
	                    if (cusPP.CustomerPaymentProviderId == MainControl.Cart.CustomerPaymentProviderID)
	                    {
	                        int index = item.DataItemIndex;
	                        string script = "<script type=\"text/javascript\">var paneIndex = " + index.ToString() + ";" + "</script>";
	                        Page.ClientScript.RegisterStartupScript(typeof (Page), "SetActivePane", script);
	                    }
	                }
	                ph.Controls.Add(ctrl);
	                ctrl = null;
	            }
	        }
	    }

	    protected void cmdConfirm_Click(object sender, EventArgs e)
	    {
	        ProcessPayment("standard");
	    }

	    protected void cmdPaypal_Click(object sender, ImageClickEventArgs e)
        {
            ProcessPayment("paypal");
        }

        #endregion

        #region Helper methods
        
        private void ProcessPayment(string method)
        {
            Boolean hasError = false;
            string errorText = "";

            int selectedIndex = Convert.ToInt32(hidPPSelectedIndex.Value);
            int paymentProviderId = -1;

            PaymentProviderBase ppUI = null;

            // if we have any Paymentproviders defined
            if (selectedIndex < 0)
            {
                hasError = true;
                errorText = Localization.GetString("ValidPaymentProvider.Error", this.LocalResourceFile);
            }
            else
            {
                paymentProviderId = (int)lstPaymentProvider.DataKeys[selectedIndex].Value;
                if (paymentProviderId < 0)
                {
                    hasError = true;
                    errorText = Localization.GetString("ValidPaymentProvider.Error", this.LocalResourceFile);
                }
                else
                {
                    ppUI = FindControlRecursive(divMain, "pp" + paymentProviderId.ToString()) as PaymentProviderBase;
                    if (ppUI == null)
                    {
                        hasError = true;
                        errorText = Localization.GetString("ValidPaymentProvider.Error", this.LocalResourceFile);
                    }
                    else if (ppUI.IsValid == false)
                    {
                        hasError = true;
                        errorText = Localization.GetString("MandatoryFields.Error", this.LocalResourceFile);
                    }
                }
            }

            if (hasError)
            {
                MainControl.ErrorText = errorText;
            }
            else
            {
                MainControl.ErrorText = "";
                UpdatePayment(ppUI);
                switch (method)
                {
                    case "standard":
                        Response.Redirect(Globals.NavigateURL(TabId, "", "action=" + MainControl.GetNextAction()));
                        break;

                    case "paypal":
                        StartPaypalCheckout(ppUI.Properties, paymentProviderId);
                        break;
                }
            }
        }

	    private void UpdatePayment(PaymentProviderBase pp)
		{
            // First we'll save the CustomerPaymentprovider
            int customerPaymentProviderId = 0;
            CustomerPaymentProviderInfo cpp = Controller.GetCustomerPaymentProvider(MainControl.CustomerId, pp.PaymentProviderId);
            if (cpp == null)
            {
                cpp = new CustomerPaymentProviderInfo();
                cpp.PaymentProviderId = pp.PaymentProviderId;
                cpp.PaymentProviderValues = pp.Values;
                cpp.CustomerId = MainControl.CustomerId;
                customerPaymentProviderId = Controller.NewCustomerPaymentProvider(cpp);
            }
            else
            {
                customerPaymentProviderId = cpp.CustomerPaymentProviderId;
                cpp.PaymentProviderId = pp.PaymentProviderId;
                cpp.PaymentProviderValues = pp.Values;
                cpp.CustomerId = MainControl.CustomerId;
                Controller.UpdateCustomerPaymentProvider(cpp);
            }

            // Next we need to update our PaymentProviderId in the cart
            Controller.UpdateCartCustomerPaymentProviderId(MainControl.CartId, customerPaymentProviderId);
            // and set the ID on our Cart Variable
            MainControl.Cart.CustomerPaymentProviderID = customerPaymentProviderId;
		}

	    private void StartPaypalCheckout(string properties, int paymentProviderId)
	    {
	        string[] props = properties.Split(',');
            PaypalApi.UseSandbox = (props.Length == 3 || (props.Length > 3 && Convert.ToBoolean(props[3]) == true));


	        PaypalUserCredentials cred = new PaypalUserCredentials(props[0], props[1], props[2]);
	        PaypalSetExpressCheckoutParameters param = new PaypalSetExpressCheckoutParameters()
	                                                       {
	                                                           CancelUrl = Globals.NavigateURL(this.TabId, "", "action=payment", "provider=" + paymentProviderId.ToString(), "result=cancel"),
	                                                           ReturnUrl = Globals.NavigateURL(this.TabId, "", "action=payment", "provider=" + paymentProviderId.ToString(), "result=success"),
	                                                           NoShipping = 1, // PayPal does not display shipping address fields whatsoever.
	                                                           AllowNote = 0, // The buyer is unable to enter a note to the merchant.
	                                                       };

            PaypalPaymentDetails request = new PaypalPaymentDetails();

	        CartInfo cart = Controller.GetCart(PortalId, MainControl.CartId);
            request.Amt = cart.OrderTotal + cart.AdditionalTotal + cart.OrderTax + cart.AdditionalTax; 
	        request.TaxAmt = cart.OrderTax + cart.AdditionalTax;
	        request.ItemAmt = cart.OrderTotal + cart.AdditionalTotal;
	        request.InvNum = cart.CartID.ToString();
            request.CurrencyCode = "EUR";

            List<PaypalPaymentDetailsItem> items = new List<PaypalPaymentDetailsItem>();
            PaypalPaymentDetailsItem item = new PaypalPaymentDetailsItem();
            item.Amt = request.ItemAmt;
	        item.Name = String.Format(Localization.GetString("PPCart.Text", this.LocalResourceFile), Controller.GetStoreSettings(PortalId)["VendorName"] ?? "BBStore");
	        List<CartProductInfo> products = Controller.GetCartProducts(cart.CartID);
	        item.Desc = "";
	        foreach (CartProductInfo product in products)
	        {
                item.Desc += string.Format("{0} {1}\r\n", (product.Quantity % 1 == 0 ? (int)product.Quantity : product.Quantity), product.Name);
	        }
            item.TaxAmt = request.TaxAmt;
            items.Add(item);

	        string response = PaypalApi.SetExpressCheckout(cred, request, items, param);

	        PaypalCommonResponse ppCommon = new PaypalCommonResponse(response);
	        if (ppCommon.Ack == PaypalAckType.Success)
	        {
	            if (PaypalApi.UseSandbox)
	                Response.Redirect("https://www.sandbox.paypal.com/cgi-bin/webscr?cmd=_express-checkout&token=" + ppCommon.Token);
	            else
	                Response.Redirect("https://www.paypal.com/cgi-bin/webscr?cmd=_express-checkout&token=" + ppCommon.Token);
	        }
	        else
	        {
	            string message = "";
	            foreach (PaypalError error in ppCommon.Errors)
	            {
	                message += String.Format("ErrorNo:{0} Message {1}<br/>\r\n", error.ErrorNo, error.LongMessage);
	            }
	            MainControl.ErrorText += message;
	        }
	    }

        private void VerifyPaypalCheckout(SubscriberPaymentProviderInfo subscriberPaymentProvider)
        {
            string token = Request["token"];
            if (subscriberPaymentProvider != null)
            {
                string[] userCredentials = subscriberPaymentProvider.PaymentProviderProperties.Split(',');
                PaypalUserCredentials cred = new PaypalUserCredentials(userCredentials[0], userCredentials[1], userCredentials[2]);
                PaypalApi.UseSandbox = (userCredentials.Length == 3 || (userCredentials.Length > 3 && Convert.ToBoolean(userCredentials[3]) == true));
                string response = PaypalApi.GetExpressCheckoutDetails(cred, token);
                PaypalCommonResponse ppCommon = new PaypalCommonResponse(response);
                if (ppCommon.Ack == PaypalAckType.Success)
                {
                    // TODO: Paypal Payerinfo auswerten ?
                    PaypalPayerInfo payer = new PaypalPayerInfo(response);
                    CustomerPaymentProviderInfo customerPaymentProvider = Controller.GetCustomerPaymentProvider(MainControl.Cart.CustomerPaymentProviderID);

                    // Save token and payerid in customerpaymentprovider for later confirmation of payment in paypal
                    customerPaymentProvider.PaymentProviderValues = ppCommon.Token + "," + payer.PayerId;
                    Controller.UpdateCustomerPaymentProvider(customerPaymentProvider);
                    Response.Redirect(Globals.NavigateURL(TabId, "", "action=" + MainControl.GetNextAction()));
                }
                else
                {
                    string message = "";
                    foreach (PaypalError error in ppCommon.Errors)
                    {
                        message += String.Format("ErrorNo:{0} Message {1}<br/>\r\n", error.ErrorNo, error.LongMessage);
                    }
                    MainControl.ErrorText += message;
                }
            }
        }

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

        #endregion
	}
}