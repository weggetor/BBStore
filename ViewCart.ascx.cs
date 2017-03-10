using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Services.Protocols;
using System.Web.UI;
using System.Web.UI.WebControls;
using Bitboxx.License;
using DotNetNuke.Application;
using DotNetNuke.Common;
using DotNetNuke.Common.Lists;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;
using ICSharpCode.SharpZipLib.Zip;

using Bitboxx.DNNModules.BBStore.Providers.Payment;
using Bitboxx.Web.GeneratedImage;


namespace Bitboxx.DNNModules.BBStore
{

    /// ----------------------------------------------------------------------------- 
    /// <summary> 
    /// The ViewBBStore class displays the content 
    /// </summary> 
    /// <remarks> 
    /// </remarks> 
    /// <history> 
    /// </history> 
    /// ----------------------------------------------------------------------------- 
    [DNNtc.PackageProperties("BBStore Cart", 2, "BBStore Cart", "BBStore cart", "BBStore.png", "Torsten Weggen", "bitboxx solutions", "http://www.bitboxx.net", "info@bitboxx.net", false)]
    [DNNtc.ModuleProperties("BBStore Cart", "BBStore Cart", 0)]
    [DNNtc.ModuleControlProperties("", "View Cart", DNNtc.ControlType.View, "", false, false)]
    partial class ViewCart : PortalModuleBase, IActionable
    {
        #region Fields

        private BBStoreController _controller;
        private bool _isChangeable;
        private CartInfo _cart;
        private bool _isConfigured = true;
        private Hashtable _storeSettings;
        private List<NavigationInfo> _navList = new List<NavigationInfo>();
        private int _shippingAddressId = -1;

        #endregion

        #region Properties

        public string Action
        {
            get
            {
                if (Request.QueryString["Action"] != null)
                    return Request.QueryString["Action"].ToLower();
                else
                    return "cart";
            }
        }
        public Guid CartId
        {
            get
            {
                string cartId;
                if (Request.Cookies["BBStoreCartId_" + PortalId.ToString()] != null)
                    cartId = (string)(Request.Cookies["BBStoreCartId_" + PortalId.ToString()].Value);
                else
                {
                    cartId = Guid.NewGuid().ToString();
                    HttpCookie keks = new HttpCookie("BBStoreCartId_" + PortalId.ToString());
                    keks.Value = cartId;
                    keks.Expires = DateTime.Now.AddDays(1);
                    Response.AppendCookie(keks);
                }
                return new Guid(cartId);
            }
        }

        public int CustomerId
        {
            get
            {
                CartInfo cart = Controller.GetCart(PortalId, CartId);

                if (cart.CustomerID > -1)
                {
                    List<CustomerInfo> customers = Controller.GetCustomersByUserId(PortalId, UserId);
                    foreach (CustomerInfo customer in customers)
                    {
                        if (customer.CustomerId == cart.CustomerID)
                            return cart.CustomerID;
                    }
                    return -1;
                }
                return -1;
            }
        }
        public BBStoreController Controller
        {
            get
            {
                if (_controller == null)
                    _controller = new BBStoreController();
                return _controller;
            }
        }
        public bool IsChangeable
        {
            get
            {
                return _isChangeable;
            }
            set
            {
                _isChangeable = value;
            }

        }

        public bool ShowNetPrice
        {
            get
            {
                if (StoreSettings["ShowNetpriceInCart"] == null || (string)StoreSettings["ShowNetpriceInCart"] == "0")
                    return true;
                return false;
            }
        }

        public ModuleKindEnum ModuleKind
        {
            get { return ModuleKindEnum.Cart; }
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
        public Hashtable StoreSettings
        {
            get
            {
                if (_storeSettings == null)
                    _storeSettings = Controller.GetStoreSettings(PortalId);
                return _storeSettings;
            }
        }
        public bool SkipPayment
        {
            get
            {
                List<SubscriberPaymentProviderInfo> lstEnabled =
                    Controller.GetSubscriberPaymentProviders(PortalId, 0).FindAll(pp => pp.IsEnabled == true && (pp.Role == String.Empty || UserInfo.IsInRole(pp.Role)));
                return (lstEnabled.Count == 0 || (Cart != null && Cart.Total + Cart.OrderTax + Cart.AdditionalTax + Cart.AdditionalTotal <= 0));
            }
        }
        public List<NavigationInfo> NavList
        {
            get
            {
                if (_navList.Count > 0)
                    return _navList;

                string returnUrl = HttpContext.Current.Request.RawUrl;
                returnUrl = HttpUtility.UrlEncode(returnUrl);

                _navList.Add(new NavigationInfo()
                {
                    Caption = Localization.GetString("navCart.Text", this.LocalResourceFile),
                    Action = "Cart",
                    Url = Globals.NavigateURL(TabId, "", "action=cart")
                });

                if (!Request.IsAuthenticated)
                {
                    _navList.Add(new NavigationInfo()
                    {
                        Caption = Localization.GetString("navLogin.Text", this.LocalResourceFile),
                        Action = "Login",
                        Url = Globals.NavigateURL(TabId, "", "action=login", "returnurl=" + returnUrl)
                    });
                }

                if (Settings["MultipleCustomers"] != null && Convert.ToBoolean(Settings["MultipleCustomers"]))
                {
                    _navList.Add(new NavigationInfo()
                    {
                        Caption = Localization.GetString("navCustomer.Text", this.LocalResourceFile),
                        Action = "Customer",
                        Url = Globals.NavigateURL(TabId, "", "action=customer")
                    });
                }
                _navList.Add(new NavigationInfo()
                {
                    Caption = Localization.GetString("navShipping.Text", this.LocalResourceFile),
                    Action = "Shipping",
                    Url = Globals.NavigateURL(TabId, "", "action=checkout")
                });
                if (!SkipPayment)
                    _navList.Add(new NavigationInfo()
                    {
                        Caption = Localization.GetString("navPayment.Text", this.LocalResourceFile),
                        Action = "Payment",
                        Url = Globals.NavigateURL(TabId, "", "action=payment")
                    });
                _navList.Add(new NavigationInfo()
                {
                    Caption = Localization.GetString("navConfirm.Text", this.LocalResourceFile),
                    Action = "Confirm",
                    Url = Globals.NavigateURL(TabId, "", "action=confirm")
                });
                _navList.Add(new NavigationInfo()
                {
                    Caption = Localization.GetString("navFinish.Text", this.LocalResourceFile),
                    Action = "Finish",
                    Url = Globals.NavigateURL(TabId, "", "action=finish")
                });
                return _navList;
            }
        }
        public int ShippingAddressId
        {
            get
            {
                if (_shippingAddressId < 0)
                    _shippingAddressId = Controller.GetCartAddressId(CartId, "Shipping");
                return _shippingAddressId;
            }
        }
        public string ErrorText
        {
            set
            {
                lblError.Text = value;
                pnlError.Visible = (value != String.Empty);
            }
            get { return lblError.Text; }
        }
        public CartInfo Cart
        {
            get
            {
                if (_cart == null)
                    _cart = Controller.GetCart(PortalId, CartId);
                return _cart;
            }
            set { _cart = value; }
        }

        #endregion

        #region Event Handlers
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            string fileName = System.IO.Path.GetFileNameWithoutExtension(this.AppRelativeVirtualPath);
            if (this.ID != null)
                //this will fix it when its placed as a ChildUserControl 
                this.LocalResourceFile = this.LocalResourceFile.Replace(this.ID, fileName);
            else
                // this will fix it when its dynamically loaded using LoadControl method 
                this.LocalResourceFile = this.LocalResourceFile + fileName + ".ascx.resx";
        }

        /// ----------------------------------------------------------------------------- 
        /// <summary> 
        /// Page_Load runs when the control is loaded 
        /// </summary> 
        /// ----------------------------------------------------------------------------- 
        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                ErrorText = "";
                // Init Steps Display
                navCart.Steps = NavList;

                cmdShopping.CssClass = (string)Settings["ShoppingButtonCssClass"] ?? "";
                cmdCheckout.CssClass = (string)Settings["CheckoutButtonCssClass"] ?? "";
                cmdDeleteCart.CssClass = (string)Settings["UploadButtonsCssClass"] ?? "";
                cmdLoadCart.CssClass = (string)Settings["UploadButtonsCssClass"] ?? "";
                cmdSaveCart.CssClass = (string)Settings["UploadButtonsCssClass"] ?? "";
                cmdFinish.CssClass = (string)Settings["OrderButtonCssClass"] ?? "";
                

                // Init empty cart display
                LocalResourceLangInfo localResourceLang = Controller.GetLocalResourceLang(PortalId, "EMPTYCART", CurrentLanguage);
                if (localResourceLang == null || localResourceLang.TextValue == String.Empty)
                    ltrEmptyCart.Text = Localization.GetString("lblEmptyCart.Text", LocalResourceFile);
                else
                {
                    ltrEmptyCart.Text = localResourceLang.TextValue;
                }

                // Mandatory Terms
                if (StoreSettings["TermsMandatory"] != null && Convert.ToBoolean(StoreSettings["TermsMandatory"]) == false)
                {
                    chkTerms.Visible = false;
                    lblTermsPre.Visible = false;
                    lnkTerms.Visible = false;
                    lblTermsPost.Visible = false;
                }

                // Mandatory Cancel Terms
                if (StoreSettings["CancelTermsMandatory"] != null && Convert.ToBoolean(StoreSettings["CancelTermsMandatory"]) == false)
                {
                    chkCancelTerms.Visible = false;
                    lblCancelTerms.Visible = false;
                }
                else
                {
                    lblCancelTerms.Text = (string) StoreSettings["CancelTerms"] ?? LocalizeString("lblCancelTerms.Text");
                }

                //Cart Navigation Control Style
                string[] navStyles = navCart.ImageStyles;
                if (Settings["CartNavigationStyle"] != null)
                    navCart.ImageStyle = (string)Settings["CartNavigationStyle"];

                // Check Settings
                if (Settings["ColVisibleImage"] == null)
                {
                    string message = Localization.GetString("NotConfigured.Message", this.LocalResourceFile);
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, message, ModuleMessage.ModuleMessageType.YellowWarning);
                    _isConfigured = false;
                    MultiView1.Visible = false;
                    navCart.Visible = false;
                }

                cmdFinish.Visible = true;

                if (_isConfigured)
                {
                    // Could we up- , download or delete the cart ?
                    
                    if (Settings["EnableCartUpload"] != null &&
                        (Convert.ToBoolean(Settings["EnableCartUpload"]) || 
                         Convert.ToBoolean(Settings["EnableCartDownload"]) ||
                         Convert.ToBoolean(Settings["EnableCartDelete"])))
                    {
                        pnlCartUpDownload.Visible = true;
                        cmdLoadCart.Visible = Convert.ToBoolean(Settings["EnableCartUpload"]);
                        cmdSaveCart.Visible = Convert.ToBoolean(Settings["EnableCartDownload"]);
                        cmdDeleteCart.Visible = Convert.ToBoolean(Settings["EnableCartDelete"]);

                    }
                    // First lets retrieve our cart from database
                    Cart = Controller.GetCart(PortalId, CartId);

                    if (Cart == null)
                    {
                        Cart = new CartInfo();
                        Cart.CartID = CartId;
                        Controller.NewCart(PortalId, Cart);
                    }
                    else
                    {
                        // If we logged out or checked in as another customer we have to reset the customer
                        if (Cart.CustomerID > -1)
                        {
                            bool found = false;
                            List<CustomerInfo> customers = Controller.GetCustomersByUserId(PortalId, UserId);
                            foreach (CustomerInfo customer in customers)
                            {
                                if (customer.CustomerId == Cart.CustomerID)
                                {
                                    found = true;
                                }
                            }
                            if (!found)
                            {
                                Cart.CustomerID = -1;
                                Controller.UpdateCartCustomerId(CartId, Cart.CustomerID);
                            }
                        }
                    }

                    // if user is logged in we connect customer to cart. If no customer exists we create one!
                    if (Request.IsAuthenticated)
                    {
                        UserInfo user = UserController.GetCurrentUserInfo();
                        List<CustomerInfo> customers = Controller.GetCustomersByUserId(PortalId, user.UserID);
                        if (customers.Count > 0)
                        {
                            Cart.CustomerID = customers[0].CustomerId;
                        }
                        else
                        {
                            CustomerInfo newCustomer = new CustomerInfo(user.UserID, PortalId, user.Username);
                            Cart.CustomerID = Controller.NewCustomer(newCustomer);
                        }
                        Controller.UpdateCartCustomerId(CartId, Cart.CustomerID);
                    }

                    switch (Action)
                    {
                        case "cart":
                            IsChangeable = true;
                            navCart.Visible = false;
                            MultiView1.SetActiveView(viewCartPane);
                            // We have to remove any Coupons editing the cart
                            Controller.DeleteCartAdditionalCost(CartId, "COUPON");
                            Controller.UpdateCartCouponId(CartId,-1);
                            ShowCart();
                            break;
                        case "login":
                            navCart.Action = "login";
                            MultiView1.SetActiveView(viewLogin);
                            ViewCartLogin cartLoginControl = LoadControl(@"~\DesktopModules\BBStore\ViewCartLogin.ascx") as ViewCartLogin;
                            cartLoginControl.MainControl = this;
                            cartLoginControl.LocalResourceFile = Localization.GetResourceFile(cartLoginControl, cartLoginControl.GetType().BaseType.Name + ".ascx");
                            cartLoginControl.ModuleConfiguration = this.ModuleConfiguration;
                            phLogin.Controls.Add(cartLoginControl);

                            break;
                        case "customer":
                            navCart.Action = "customer";
                            MultiView1.SetActiveView(viewCheckout);
                            ViewCartCustomer cartCustomerControl = LoadControl(@"~\DesktopModules\BBStore\ViewCartCustomer.ascx") as ViewCartCustomer;
                            cartCustomerControl.MainControl = this;
                            cartCustomerControl.ModuleConfiguration = this.ModuleConfiguration;
                            phCheckout.Controls.Add(cartCustomerControl);
                            break;
                        case "checkout":
                            IsChangeable = false;
                            navCart.Action = "shipping";
                            MultiView1.SetActiveView(viewCheckout);
                            ViewCartSelectAddresses cartSelectAddressesControl = LoadControl(@"~\DesktopModules\BBStore\ViewCartSelectAddresses.ascx") as ViewCartSelectAddresses;
                            cartSelectAddressesControl.MainControl = this;
                            cartSelectAddressesControl.ModuleConfiguration = this.ModuleConfiguration;
                            cartSelectAddressesControl.LocalResourceFile = this.LocalResourceFile;
                            phCheckout.Controls.Add(cartSelectAddressesControl);
                            break;
                        case "adredit":
                            navCart.Action = "shipping";
                            MultiView1.SetActiveView(viewAddressEdit);
                            ViewCartAddressEdit cartAddressEditControl = LoadControl(@"~\DesktopModules\BBStore\ViewCartAddressEdit.ascx") as ViewCartAddressEdit;
                            cartAddressEditControl.MainControl = this;
                            cartAddressEditControl.ModuleConfiguration = this.ModuleConfiguration;
                            cartAddressEditControl.LocalResourceFile = Localization.GetResourceFile(cartAddressEditControl, cartAddressEditControl.GetType().BaseType.Name + ".ascx");
                            phAddressEdit.Controls.Add(cartAddressEditControl);
                            break;
                        case "shipping":
                            MultiView1.SetActiveView(viewShipping);
                            ShowShipping();
                            break;
                        case "payment":
                            navCart.Action = "payment";
                            MultiView1.SetActiveView(viewPayment);
                            ViewCartPayment cartPaymentControl = LoadControl(@"~\DesktopModules\BBStore\ViewCartPayment.ascx") as ViewCartPayment;
                            cartPaymentControl.MainControl = this;
                            cartPaymentControl.ModuleConfiguration = this.ModuleConfiguration;
                            cartPaymentControl.LocalResourceFile = this.LocalResourceFile;
                            phPayment.Controls.Add(cartPaymentControl);
                            break;
                        case "confirm":
                            IsChangeable = false;
                            navCart.Action = "confirm";
                            MultiView1.SetActiveView(viewCartPane);
                            ShowConfirm();
                            ShowCart();
                            pnlCartUpDownload.Visible = false;
                            break;
                        case "finish":
                            IsChangeable = false;
                            navCart.Visible = false;
                            MultiView1.SetActiveView(viewFinish);
                            ShowFinish();
                            break;
                        case "paypalpdt":
                            IsChangeable = false;
                            navCart.Visible = false;
                            MultiView1.SetActiveView(viewPaypalIPN);
                            ShowPaypalPDT();
                            break;
                        case "paypalipn":
                            IsChangeable = false;
                            navCart.Visible = false;
                            MultiView1.SetActiveView(viewPaypalIPN);
                            ShowPaypalIPN();
                            break;
                        default:
                            navCart.Visible = false;
                            MultiView1.SetActiveView(viewCartPane);
                            break;
                    }
                }
            }

            catch (Exception exc)
            {
                //Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void Page_Prerender(object sender, System.EventArgs e)
        {
            if (_isConfigured)
            {
                UpdateDiscount();
                try
                {
                    switch (Action)
                    {
                        case "cart":
                            BindCart();
                            break;
                        case "login":
                            break;
                        case "checkout":
                            break;
                        case "confirm":
                            BindCart();
                            break;
                        case "finish":
                            break;
                        default:
                            break;
                    }


                    bool addressOk = Controller.CheckCartAddresses(CartId, PortalId, 0);
                    navCart.Enable("cart", true);
                    navCart.Enable("login", true);
                    navCart.Enable("customer", Request.IsAuthenticated);
                    navCart.Enable("shipping", Request.IsAuthenticated);
                    navCart.Enable("payment", Request.IsAuthenticated && addressOk);
                    navCart.Enable("confirm", Request.IsAuthenticated && addressOk && Cart.CustomerPaymentProviderID >= 0);
                    navCart.Enable("finish", false);

                    // We can set the Title of our Module
                    string titleLabelName = DotNetNukeContext.Current.Application.Version.Major < 6 ? "lblTitle" : "titleLabel";
                    Control ctl = Globals.FindControlRecursiveDown(this.ContainerControl, titleLabelName);
                    if ((ctl != null && Cart.CartName != String.Empty))
                    {
                        ((Label)ctl).Text = Cart.CartName;
                    }
                }
                catch (Exception exc)
                {
                    //Module failed to load 
                    Exceptions.ProcessModuleLoadException(this, exc);
                }
            }
            // Check licensing
            LicenseDataInfo license = Controller.GetLicense(PortalId, false);
            Controller.CheckLicense(license, this, ModuleKind);
        }

        protected void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            TextBox thisTextBox = (TextBox)sender;
            GridViewRow thisGridViewRow = (GridViewRow)thisTextBox.Parent.Parent;


            HiddenField hid = (HiddenField)thisGridViewRow.FindControl("HiddenCartProductId");
            int cartProductId = 0;
            if (int.TryParse(hid.Value, out cartProductId))
            {
                TextBox tb = (TextBox)thisGridViewRow.FindControl("txtQuantity");
                decimal newQuantity = 0.0m;
                if (decimal.TryParse(tb.Text, out newQuantity))
                {
                    CartProductInfo cartProduct = Controller.GetCartProduct(cartProductId);
                    int decimals = 0;
                    if (cartProduct != null)
                        decimals = cartProduct.Decimals;

                    if (newQuantity > 0)
                        Controller.UpdateCartProductQuantity(cartProductId, Math.Round(newQuantity, decimals));
                    else
                        Controller.DeleteCartProduct(cartProductId);
                    Cart = Controller.GetCart(PortalId, CartId);
                }
            }
        }

        protected void cmdDeleteRow_OnClick(object sender, EventArgs e)
        {
            LinkButton thisButton = (LinkButton) sender;
            GridViewRow thisGridViewRow = (GridViewRow) thisButton.Parent.Parent;


            HiddenField hid = (HiddenField) thisGridViewRow.FindControl("HiddenCartProductId");
            int cartProductId = 0;
            if (int.TryParse(hid.Value, out cartProductId))
            {
                Controller.DeleteCartProduct(cartProductId);
            }
        }

        protected void cmdShopping_Click(object sender, EventArgs e)
        {
            int shoppingTarget = Settings["ShoppingTarget"] == null ? 1 : Convert.ToInt32(Settings["ShoppingTarget"]);
            if (Request["ReturnUrl"] != null && shoppingTarget == 1)
            {
                string returnUrl = HttpUtility.UrlDecode(Request["ReturnUrl"]);
                Response.Redirect(returnUrl);
            }
            else
            {
                Hashtable StoreSettings = Controller.GetStoreSettings(PortalId);
                int StartPageTabId;
                if (StoreSettings["StartPage"] != null && Int32.TryParse((string)StoreSettings["StartPage"], out StartPageTabId))
                    Response.Redirect(Globals.NavigateURL(StartPageTabId));
                else
                    Response.Redirect(Globals.NavigateURL(PortalSettings.DefaultTabId));
            }
        }
        protected void cmdCheckout_Click(object sender, EventArgs e)
        {
            Response.Redirect(Globals.NavigateURL(TabId, "", "action=" + GetNextAction()));
        }


        protected void cmdFinish_Click(object sender, EventArgs e)
        {
            bool termsMandatory = (StoreSettings["TermsMandatory"] == null ||
                                   Convert.ToBoolean(StoreSettings["TermsMandatory"]) == true);

            bool cancelTermsMandatory = (StoreSettings["CancelTermsMandatory"] == null ||
                       Convert.ToBoolean(StoreSettings["CancelTermsMandatory"]) == true);

            if ((chkTerms.Checked || !termsMandatory) && (chkCancelTerms.Checked || !cancelTermsMandatory))
            {
                lblTermsError.Visible = false;
                lblCancelTermsError.Visible = false;
                
                // Fill the order and send mail & empty cart
                Cart.Comment = txtRemarks.Text;
                if (fulAttachment.HasFile)
                {
                    // Get the bytes from the uploaded file
                    byte[] fileData = new byte[fulAttachment.PostedFile.InputStream.Length];
                    fulAttachment.PostedFile.InputStream.Read(fileData, 0, fileData.Length);

                    Cart.Attachment = fileData;
                    FileInfo file = new FileInfo(fulAttachment.PostedFile.FileName);
                    Cart.AttachName = file.Name;
                    Cart.AttachContentType = fulAttachment.PostedFile.ContentType;
                }

                Controller.UpdateCart(PortalId, Cart);
                
                // If Coupon used we have to subtract the usages field
                if (Cart.CouponId > 0)
                    Controller.UpdateCouponCount(Cart.CouponId, -1);

                // Save the order
                Hashtable storeSettings = Controller.GetStoreSettings(PortalId);
                string numberMask = (string)storeSettings["OrderMask"];
                int OrderId = Controller.SaveOrder(Cart.CartID, PortalId, numberMask);

                // Mail Order, finish Paypal and delete Cart
                if (OrderId >= 0)
                {
                    MailOrder(OrderId);
                    if (Cart.CustomerPaymentProviderID > -1)
                    {
                        CustomerPaymentProviderInfo customerPaymentProvider = Controller.GetCustomerPaymentProvider(Cart.CustomerPaymentProviderID);
                        if (customerPaymentProvider.PaymentProviderId == 5) // Paypal
                        {
                            EndPaypalCheckout(customerPaymentProvider, OrderId);
                        }
                    }
                    Controller.DeleteCart(Cart.CartID);
                }
                
                // Delete the cookie
                HttpCookie keks = new HttpCookie("BBStoreCartId_" + PortalId.ToString());
                keks.Value = "";
                keks.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(keks);
                Response.Redirect(Globals.NavigateURL(TabId, "", "action=finish", "order=" + OrderId.ToString()));
            }
            else
            {
                if (!chkTerms.Checked && termsMandatory)
                    lblTermsError.Visible = true;
                if (!chkCancelTerms.Checked && cancelTermsMandatory)
                    lblCancelTermsError.Visible = true;
            }
        }

        protected void cmdLoadCart_Click(object sender, EventArgs e)
        {
            pnlEmptyCart.Visible = false;
            pnlFullCart.Visible = false;
            pnlCartUpDownload.Visible = false;
            pnlUploadCart.Visible = true;
        }
        protected void cmdSaveCart_Click(object sender, EventArgs e)
        {
            // TODO: Adressumstellung
            string xml = Controller.SerializeCart(Cart.CartID) + "\r\n";
            Response.Clear();
            //Response.BufferOutput = false;  // for large files
            Response.ContentType = "text/xml";
            Response.AddHeader("Content-Length", xml.Length.ToString());
            Response.AddHeader("Content-Disposition", "attachment; filename=Cart.xml");
            //Response.AppendHeader("Pragma", "no-cache");
            //Response.HeaderEncoding = Encoding.UTF8;
            //Response.ContentEncoding = Encoding.UTF8;
            Response.Write(xml);
            Response.Flush();
            Response.Close();
        }
        protected void cmdDeleteCart_Click(object sender, EventArgs e)
        {
            Controller.DeleteCart(CartId);
            Cart = new CartInfo();
            Cart.CartID = CartId;
            Controller.NewCart(PortalId, Cart);

            pnlEmptyCart.Visible = true;
            pnlFullCart.Visible = false;
            pnlCartUpDownload.Visible = cmdDeleteCart.Visible || cmdUploadCart.Visible || cmdSaveCart.Visible;
            pnlUploadCart.Visible = false;
        }

        protected void cmdUploadCart_Click(object sender, EventArgs e)
        {
            if (fulCart.HasFile)
            {
                // Get the bytes from the uploaded file
                byte[] fileData = new byte[fulCart.PostedFile.InputStream.Length];
                fulCart.PostedFile.InputStream.Read(fileData, 0, fileData.Length);

                System.Text.UTF8Encoding enc = new UTF8Encoding();
                string xml = enc.GetString(fileData);

                bool extendedPrice = Convert.ToBoolean(StoreSettings["ExtendedPrice"]);
                CartInfo cart = Controller.DeserializeCart(PortalId, UserId, CartId, xml, extendedPrice);
                //CustomerId = cart.CustomerID;
            }
            pnlEmptyCart.Visible = false;
            pnlFullCart.Visible = true;
            pnlUploadCart.Visible = false;
        }

        protected void cmdCoupon_Click(object sender, EventArgs e)
        {
            CouponInfo coupon = Controller.GetCouponByCode(txtCoupon.Text);
            if (coupon == null)
            {
                lblCouponError.Text = LocalizeString("CouponNotFound.Error");
                pnlCouponError.Visible = true;
            }
            else if (coupon.UsagesLeft <= 0)
            {
                lblCouponError.Text = String.Format(LocalizeString("CouponUsed.Error"), coupon.Caption);
                pnlCouponError.Visible = true;
            }
            else if (coupon.ValidUntil != null && coupon.ValidUntil < DateTime.Now)
            {
                lblCouponError.Text = String.Format(LocalizeString("CouponOutdated.Error"), coupon.Caption);
                pnlCouponError.Visible = true;
            }
            else
            {
                UpdateCoupon(coupon);
                pnlCouponError.Visible = false;
                txtCoupon.Text = "";
            }
        }

        protected void lstCustomerAddresses_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                CustomerAddressInfo customerAddress = ((ListViewDataItem)e.Item).DataItem as CustomerAddressInfo;
                Label lblAddressType = e.Item.FindControl("lblAddressType") as Label;
                lblAddressType.Text = customerAddress.AddressType;


                Label lblAddress = e.Item.FindControl("lblAddress") as Label;
                string template = Localization.GetString("AddressTemplate.Text", this.LocalResourceFile);
                lblAddress.Text = customerAddress.ToHtml(template, true);
            }
        }

        protected void grdCartProducts_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                CartProductInfo Product = e.Row.DataItem as CartProductInfo;
                List<CartProductOptionInfo> lstOpt = Controller.GetCartProductOptions(Product.CartProductId);
                if (lstOpt.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (CartProductOptionInfo Option in lstOpt)
                    {
                        sb.Append(Option.OptionName);
                        sb.Append(":");
                        sb.Append(Option.OptionValue);
                        sb.Append("<br />");
                    }
                    Label lblOption = e.Row.FindControl("lblOption") as Label;
                    lblOption.Text = sb.ToString();
                }

                GeneratedImage imgThumb = e.Row.FindControl("imgThumb") as GeneratedImage;
                if (Product.Image != String.Empty)
                {
                    int width = 80;
                    Int32.TryParse((string)Settings["ColWidthImage"], out width);
                    width -= 6; // padding abziehen
                    string fileName =
                            PortalSettings.HomeDirectoryMapPath.Replace(HttpContext.Current.Request.PhysicalApplicationPath, "") +
                            Product.Image.Replace('/', '\\');

                    imgThumb.Parameters.Add(new ImageParameter() { Name = "Width", Value = width.ToString() });
                    imgThumb.Parameters.Add(new ImageParameter() { Name = "File", Value = fileName });
                    // TODO:Watermark
                    //if (false)
                    //{
                    //    imgThumb.Parameters.Add(new ImageParameter() { Name = "WatermarkText", Value = Localization.GetString("Sold.Text", this.LocalResourceFile) });
                    //    imgThumb.Parameters.Add(new ImageParameter() { Name = "WatermarkFontFamily", Value = "Verdana" });
                    //    imgThumb.Parameters.Add(new ImageParameter() { Name = "WatermarkFontColor", Value = "Red" });
                    //    imgThumb.Parameters.Add(new ImageParameter() { Name = "WatermarkFontSize", Value = "20" });
                    //}
                }
                else
                    imgThumb.Visible = false;

                Label lblName = e.Row.FindControl("lblName") as Label;
                HyperLink lnkName = e.Row.FindControl("lnkName") as HyperLink;
                if (lnkName != null && lblName != null)
                {
                    lblName.Visible = (Product.ProductUrl == String.Empty);
                    lnkName.Visible = !lblName.Visible;
                }
                Label lblQuantity = e.Row.FindControl("lblQuantity") as Label;
                TextBox txtQuantity = e.Row.FindControl("txtQuantity") as TextBox;
                LinkButton cmdDeleteRow = e.Row.FindControl("cmdDeleteRow") as LinkButton;
                lblQuantity.Visible = !IsChangeable;
                txtQuantity.Visible = IsChangeable;
                cmdDeleteRow.Visible = IsChangeable;
            }
        }

        #endregion

        #region Methods
        private void ShowCart()
        {
            int ColWidthAmount = Convert.ToInt32(Settings["ColWidthAmount"]);
            int ColWidthPercent = Convert.ToInt32(Settings["ColWidthPercent"]);
            int ColWidthImage = Convert.ToInt32(Settings["ColWidthImage"]);
            int ColWidthQuantity = Convert.ToInt32(Settings["ColWidthQuantity"]);
            int ColWidthItemNo = Convert.ToInt32(Settings["ColWidthItemNo"]);
            int ColWidthUnit = Convert.ToInt32(Settings["ColWidthUnit"]);
            bool ColVisibleImage = Convert.ToBoolean(Settings["ColVisibleImage"]);
            bool ColVisibleItemNo = Convert.ToBoolean(Settings["ColVisibleItemNo"]);
            bool ColVisibleUnit = Convert.ToBoolean(Settings["ColVisibleUnit"]);
            bool ColVisibleUnitCost = Convert.ToBoolean(Settings["ColVisibleUnitCost"]);
            bool ColVisibleNetTotal = Convert.ToBoolean(Settings["ColVisibleNetTotal"]);
            bool ColVisibleTaxPercent = Convert.ToBoolean(Settings["ColVisibleTaxPercent"]);
            bool ColVisibleTaxTotal = Convert.ToBoolean(Settings["ColVisibleTaxTotal"]);
            bool ColVisibleSubTotal = Convert.ToBoolean(Settings["ColVisibleSubTotal"]);
            bool ShowSummary = Convert.ToBoolean(Settings["ShowSummary"]);


            grdCartProducts.Style.Add("width", "100%");
            grdCartProducts.Columns[0].Visible = false; // CartProductId
            grdCartProducts.Columns[1].Visible = false; // CartId
            grdCartProducts.Columns[2].Visible = false; // ProductId
            grdCartProducts.Columns[3].Visible = ColVisibleImage; // Image
            grdCartProducts.Columns[4].Visible = true; // Quantity
            grdCartProducts.Columns[5].Visible = ColVisibleUnit; // Unit
            grdCartProducts.Columns[6].Visible = ColVisibleItemNo; // ItemNo
            grdCartProducts.Columns[7].Visible = true; // Product
            grdCartProducts.Columns[8].Visible = ColVisibleUnitCost; // UnitCost
            grdCartProducts.Columns[9].Visible = ColVisibleNetTotal; // NetTotal
            grdCartProducts.Columns[10].Visible = ColVisibleTaxPercent; // TaxPercent
            grdCartProducts.Columns[11].Visible = ColVisibleTaxTotal; // TaxTotal
            grdCartProducts.Columns[12].Visible = ColVisibleSubTotal; // SubTotal
            grdCartProducts.Columns[3].ItemStyle.Width = ColWidthImage;
            grdCartProducts.Columns[4].ItemStyle.Width = ColWidthQuantity;
            grdCartProducts.Columns[4].ControlStyle.Width = ColWidthQuantity - 4;
            grdCartProducts.Columns[5].ItemStyle.Width = ColWidthUnit;
            grdCartProducts.Columns[6].ItemStyle.Width = ColWidthItemNo;
            grdCartProducts.Columns[8].ItemStyle.Width = ColWidthAmount;
            grdCartProducts.Columns[9].ItemStyle.Width = ColWidthAmount;
            grdCartProducts.Columns[10].ItemStyle.Width = ColWidthPercent;
            grdCartProducts.Columns[11].ItemStyle.Width = ColWidthAmount;
            grdCartProducts.Columns[12].ItemStyle.Width = ColWidthAmount;
            Localization.LocalizeGridView(ref grdCartProducts, this.LocalResourceFile);

            grdAdditionalCosts.Style.Add("width", "100%");
            grdAdditionalCosts.Style.Add("border-top-style", "solid");
            grdAdditionalCosts.Columns[0].Visible = false; // CartAdditionalCostId
            grdAdditionalCosts.Columns[1].Visible = false; // CartId
            grdAdditionalCosts.Columns[2].Visible = ColVisibleImage; // Image
            grdAdditionalCosts.Columns[3].Visible = true; // Quantity
            grdAdditionalCosts.Columns[4].Visible = ColVisibleUnit; // Unit
            grdAdditionalCosts.Columns[5].Visible = ColVisibleItemNo; // ItemNo
            grdAdditionalCosts.Columns[6].Visible = true; // Product
            grdAdditionalCosts.Columns[7].Visible = ColVisibleUnitCost; // UnitCost
            grdAdditionalCosts.Columns[8].Visible = ColVisibleNetTotal; // NetTotal
            grdAdditionalCosts.Columns[9].Visible = ColVisibleTaxPercent; // TaxPercent
            grdAdditionalCosts.Columns[10].Visible = ColVisibleTaxTotal; // TaxTotal
            grdAdditionalCosts.Columns[11].Visible = ColVisibleSubTotal; // SubTotal
            grdAdditionalCosts.Columns[2].ItemStyle.Width = ColWidthImage; // Image
            grdAdditionalCosts.Columns[3].ItemStyle.Width = ColWidthQuantity; // Quantity
            grdAdditionalCosts.Columns[4].ItemStyle.Width = ColWidthUnit; // Unit
            grdAdditionalCosts.Columns[5].ItemStyle.Width = ColWidthItemNo; // ItemNo
            grdAdditionalCosts.Columns[7].ItemStyle.Width = ColWidthAmount; // UnitCost
            grdAdditionalCosts.Columns[8].ItemStyle.Width = ColWidthAmount; // NetTotal
            grdAdditionalCosts.Columns[9].ItemStyle.Width = ColWidthPercent; // TaxPercent
            grdAdditionalCosts.Columns[10].ItemStyle.Width = ColWidthAmount; // TaxTotal
            grdAdditionalCosts.Columns[11].ItemStyle.Width = ColWidthAmount; // SubTotal


            grdSubTotal.Visible = ShowSummary;
            grdSubTotal.Style.Add("width", "100%");
            grdSubTotal.Style.Add("border-top-style", "double");
            grdSubTotal.Style.Add("border-bottom-style", "solid");
            grdSubTotal.Columns[0].Visible = true; // Description
            grdSubTotal.Columns[1].Visible = ColVisibleUnitCost; // UnitCost
            grdSubTotal.Columns[2].Visible = ColVisibleNetTotal; // NetTotal
            grdSubTotal.Columns[3].Visible = ColVisibleTaxPercent; // TaxPercent
            grdSubTotal.Columns[4].Visible = ColVisibleTaxTotal; // TaxTotal
            grdSubTotal.Columns[5].Visible = ColVisibleSubTotal; // SubTotal
            grdSubTotal.Columns[1].ItemStyle.Width = ColWidthAmount; // UnitCost
            grdSubTotal.Columns[2].ItemStyle.Width = ColWidthAmount; // NetTotal
            grdSubTotal.Columns[3].ItemStyle.Width = ColWidthPercent; // TaxPercent
            grdSubTotal.Columns[4].ItemStyle.Width = ColWidthAmount; // TaxTotal
            grdSubTotal.Columns[5].ItemStyle.Width = ColWidthAmount; // SubTotal

            grdSummary.Visible = ShowSummary;
            grdSummary.Style.Add("width", "100%");
            grdSummary.Columns[1].ItemStyle.Width = ColWidthAmount;
        }
        private void BindCart()
        {
            if (pnlUploadCart.Visible)
                return;

            if (ShippingAddressId > -1)
            {
                UpdateShipping();
            }

            if (Cart.CustomerPaymentProviderID > -1)
            {
                UpdatePayment();
            }

            Cart = Controller.GetCart(PortalId, CartId);

            // Now we need the products in the cart
            List<CartProductInfo> myProducts = Controller.GetCartProducts(CartId);
            grdCartProducts.DataSource = myProducts;
            grdCartProducts.DataBind();

            if (myProducts.Count % 2 == 0)
            {
                grdAdditionalCosts.RowStyle.CssClass = "bbstore-grid-row";
                grdAdditionalCosts.AlternatingRowStyle.CssClass = "bbstore-grid-alternaterow";
            }
            else
            {
                grdAdditionalCosts.RowStyle.CssClass = "bbstore-grid-alternaterow";
                grdAdditionalCosts.AlternatingRowStyle.CssClass = "bbstore-grid-row";
            }

            List<CartAdditionalCostInfo> myAdditionalCosts = Controller.GetCartAdditionalCosts(CartId);
            grdAdditionalCosts.DataSource = myAdditionalCosts;
            grdAdditionalCosts.DataBind();

            Decimal NetTotal = 0.00m;
            Decimal TaxTotal = 0.00m;
            Decimal SubTotal = 0.00m;

            List<CartRowInfo> lstRow = new List<CartRowInfo>();
            foreach (CartProductInfo cp in myProducts)
            {
                NetTotal += cp.NetTotal;
                TaxTotal += cp.TaxTotal;
                SubTotal += cp.SubTotal;
            }
            foreach (CartAdditionalCostInfo cap in myAdditionalCosts)
            {
                NetTotal += cap.NetTotal;
                TaxTotal += cap.TaxTotal;
                SubTotal += cap.SubTotal;
            }
            //SubTotal = decimal.Round(NetTotal + TaxTotal,2);
            //NetTotal = decimal.Round(NetTotal, 2);
            //TaxTotal = SubTotal - NetTotal;


            //if (NetTotal > 0)
            if (myProducts.Count > 0)
            {
                string TotalText = Localization.GetString("Total.Text", this.LocalResourceFile);
                lstRow.Add(new CartRowInfo(TotalText, NetTotal, TaxTotal, SubTotal));
                grdSubTotal.DataSource = lstRow;
                grdSubTotal.DataBind();
                pnlEmptyCart.Visible = false;
                pnlFullCart.Visible = true;
            }
            else
            {
                pnlEmptyCart.Visible = true;
                pnlFullCart.Visible = false;
            }

            if (grdSummary.Visible)
            {
                Hashtable ht = new Hashtable();
                foreach (CartProductInfo cp in myProducts)
                {
                    if (ht.ContainsKey(cp.TaxPercent))
                        ht[cp.TaxPercent] = (decimal)ht[cp.TaxPercent] + cp.TaxTotal;
                    else
                        ht.Add(cp.TaxPercent, cp.TaxTotal);
                }
                foreach (CartAdditionalCostInfo cap in myAdditionalCosts)
                {
                    if (ht.ContainsKey(cap.TaxPercent))
                        ht[cap.TaxPercent] = (decimal)ht[cap.TaxPercent] + cap.TaxTotal;
                    else
                        ht.Add(cap.TaxPercent, cap.TaxTotal);
                }


                List<CartSumInfo> lstSum = new List<CartSumInfo>();
                if (ShowNetPrice)
                {
                    lstSum.Add(new CartSumInfo(Localization.GetString("Nettotal.Text", this.LocalResourceFile), NetTotal));
                    foreach (DictionaryEntry tax in ht)
                    {
                        string text = string.Format(Localization.GetString("ExcludeTax.Text", this.LocalResourceFile), (decimal)tax.Key);
                        lstSum.Add(new CartSumInfo(text, (decimal)tax.Value));
                    }
                    lstSum.Add(new CartSumInfo(Localization.GetString("Total.Text", this.LocalResourceFile), SubTotal));
                }
                else
                {
                    lstSum.Add(new CartSumInfo(Localization.GetString("Total.Text", this.LocalResourceFile), SubTotal));
                    foreach (DictionaryEntry tax in ht)
                    {
                        string Text = string.Format(Localization.GetString("IncludeTax.Text", this.LocalResourceFile), (decimal)tax.Key);
                        lstSum.Add(new CartSumInfo(Text, (decimal)tax.Value));
                    }
                }

                grdSummary.DataSource = lstSum;
                grdSummary.DataBind();
            }
        }



        private void ShowShipping()
        {
            // Is he logged in ?
            if (!Request.IsAuthenticated)
            {
                // Attention ! returnUrl must be relative path (cross-site-scripting denying)
                string returnUrl = HttpContext.Current.Request.RawUrl;
                returnUrl = HttpUtility.UrlEncode(returnUrl);

                Response.Redirect(Globals.NavigateURL(TabId, "", "action=login", "returnurl=" + returnUrl));
            }
            UpdateShipping();
            string action = GetNextAction();
            Response.Redirect(Globals.NavigateURL(TabId, "", "action=" + GetNextAction()));
        }

        private void ShowConfirm()
        {
            pnlConfirm.Visible = true;
            pnlConfirm2.Visible = true;
            pnlCoupon.Visible = Cart.CouponId <= 0 && Convert.ToBoolean(StoreSettings["CouponsEnabled"] ?? "false");
            pnlCheckout.Visible = false;

            List<CustomerAddressInfo> cartAddresses = Controller.GetCustomerAddressesByCart(CartId, CurrentLanguage);
            lstCustomerAddresses.DataSource = cartAddresses;
            lstCustomerAddresses.DataBind();

            CustomerPaymentProviderInfo cp = Controller.GetCustomerPaymentProvider(Cart.CustomerPaymentProviderID);
            if (cp != null)
            {
                SubscriberPaymentProviderInfo sp = Controller.GetSubscriberPaymentProvider(PortalId, 0, cp.PaymentProviderId);
                PaymentProviderInfo pp = Controller.GetPaymentProvider(cp.PaymentProviderId, CurrentLanguage);
                PaymentProviderBase ctrl = this.LoadControl(@"~\DesktopModules\BBStore\Providers\Payment\" + pp.ProviderControl.Trim() + ".ascx") as PaymentProviderBase;
                ctrl.Values = cp.PaymentProviderValues;
                //We need to check if Paypal Values are set
                if (sp.PaymentProviderId == 5 && ctrl.Values == string.Empty)
                    Response.Redirect(Globals.NavigateURL(TabId, "", "action=Payment"));


                ctrl.Properties = sp.PaymentProviderProperties;
                ctrl.DisplayMode = ViewMode.Summary;
                phPaymentSummary.Controls.Add(ctrl);

                lblPaymentConfirm.Text = pp.ProviderName;
            }

            int TermsTabId;
            if (StoreSettings["TermsUrl"] != null && Int32.TryParse((string)StoreSettings["TermsUrl"], out TermsTabId))
                lnkTerms.PostBackUrl = Globals.NavigateURL(TermsTabId);

        }
        private void ShowFinish()
        {
            int orderId = 0;
            string orderNo = "";
            if (Int32.TryParse(Request["order"], out orderId))
            {
                OrderInfo order = Controller.GetOrder(orderId);
                if (order != null)
                    orderNo = order.OrderNo;
                LocalResourceLangInfo localResourceLang = Controller.GetLocalResourceLang(PortalId, "CONFIRMCART", CurrentLanguage);
                if (localResourceLang == null || localResourceLang.TextValue == String.Empty)
                    ltrConfirmCart.Text = Localization.GetString("lblConfirmCart.Text", LocalResourceFile).Replace("[ORDERNO]", orderNo);
                else
                {
                    ltrConfirmCart.Text = localResourceLang.TextValue.Replace("[ORDERNO]", orderNo);
                }
            }

        }
        private void ShowPaypalIPN()
        {
            string postUrl = "https://www.sandbox.paypal.com/cgi-bin/webscr";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(postUrl);

            //Set values for the request back
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            byte[] param = Request.BinaryRead(HttpContext.Current.Request.ContentLength);
            string strRequest = Encoding.ASCII.GetString(param);
            string ipnPost = strRequest;
            strRequest += "&cmd=_notify-validate";
            req.ContentLength = strRequest.Length;

            //for proxy
            //WebProxy proxy = new WebProxy(new Uri("http://url:port#"));
            //req.Proxy = proxy;

            //Send the request to PayPal and get the response
            StreamWriter streamOut = new StreamWriter(req.GetRequestStream(),
                                     System.Text.Encoding.ASCII);
            streamOut.Write(strRequest);
            streamOut.Close();

            StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream());
            string strResponse = streamIn.ReadToEnd();
            streamIn.Close();

            // logging ipn messages... be sure that you give write
            // permission to process executing this code
            string logPathDir = ResolveUrl("Messages");
            string logPath = string.Format("{0}\\{1}.txt",
                             Server.MapPath(logPathDir), DateTime.Now.Ticks);
            File.WriteAllText(logPath, ipnPost);
            //

            if (strResponse == "VERIFIED")
            {
                //check the payment_status is Completed
                //check that txn_id has not been previously processed
                //check that receiver_email is your Primary PayPal email
                //check that payment_amount/payment_currency are correct
                //process payment
            }
            else if (strResponse == "INVALID")
            {
                //log for manual investigation
            }
            else
            {
                //log response/ipn data for manual investigation
            }

            //string requestString = String.Empty;
            //for (int i = 0; i < Request.Params.Count; i++)
            //{
            //    requestString += Request.Params.Keys[i] + " = " + Request.Params[i] + "<br/>";
            //}
            //lblPaypalIPNText.Text = requestString;

        }

        private void ShowPaypalPDT()
        {


            string authToken = "mDIpSHD6vsHcaYCIQgOlFtquMU8b64GoO44kAIiDkwzBJs-Biq24LBWH7nq";
            string url = "https://www.sandbox.paypal.com/cgi-bin/webscr";

            //read in txn token from querystring
            string txToken = Request.QueryString.Get("tx");


            string query = string.Format("cmd=_notify-synch&tx={0}&at={1}",
                                  txToken, authToken);

            // Create the request back
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            // Set values for the request back
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = query.Length;

            // Write the request back IPN strings
            StreamWriter stOut = new StreamWriter(req.GetRequestStream(),
                                     System.Text.Encoding.ASCII);
            stOut.Write(query);
            stOut.Close();

            // Do the request to PayPal and get the response
            StreamReader stIn = new StreamReader(req.GetResponse().GetResponseStream());
            string strResponse = stIn.ReadToEnd();
            stIn.Close();

            //// If response was SUCCESS, parse response string and output details
            //if (strResponse.StartsWith("SUCCESS"))
            //{
            //    PDTHolder pdt = PDTHolder.Parse(strResponse);
            //    Label1.Text =
            //        string.Format("Thank you {0} {1} [{2}] for your payment of {3} {4}!",
            //        pdt.PayerFirstName, pdt.PayerLastName,
            //        pdt.PayerEmail, pdt.GrossTotal, pdt.Currency);
            //}
            //else
            //{
            //    Label1.Text = "Oooops, something went wrong...";
            //}

            lblPaypalIPNText.Text = strResponse;

        }

        private void UpdateShipping()
        {
            if (Cart == null)
            {
                ErrorText = Localization.GetString("NoCart.Error", this.LocalResourceFile);
                return;
            }

            Hashtable storeSettings = Controller.GetStoreSettings(PortalId);

            decimal shippingTaxPercent = 0.0m;
            if (storeSettings["ShippingTax"] != null)
                Decimal.TryParse(((string)storeSettings["ShippingTax"]).Replace(",", "."), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out shippingTaxPercent);

            Controller.DeleteCartAdditionalCost(CartId, "SHIPPING");

            // Lets retrieve the CartProducts
            List<CartProductInfo> cartProducts = Controller.GetCartProducts(CartId);
            var shippingModelIds = (from c in cartProducts select c.ShippingModelId).Distinct();

            foreach (int shippingModelId in shippingModelIds)
            {
                // if CartProducts have no ShippingModel, we skip these !
                if (shippingModelId == -1)
                    continue;

                int shippingZoneId = -1;
                decimal shippingSum = 0.00m;

                // Determine the sums for the products in cart with this shippingModelId
                decimal sumPieces = 0m, sumWeight = 0m, sumCost = 0m;
                var products = from c in cartProducts where c.ShippingModelId == shippingModelId select c;
                foreach (CartProductInfo product in products)
                {
                    sumPieces += product.Quantity;
                    sumWeight += product.Weight * product.Quantity;
                    sumCost += product.UnitCost * product.Quantity;
                }

                //Retrieve all ShippingCosts for this shippingmodel
                List<ShippingCostInfo> shippingCosts = Controller.GetShippingCostsByModelId(shippingModelId);
                string shippingCountry = "";
                string shippingCountryCode = (string)StoreSettings["VendorCountry"];

                bool costwithoutZoneId = (from s in shippingCosts where s.ShippingZoneID == -1 select s).Any();
                if (! costwithoutZoneId)
                {
                    // Retrieve shippingZoneId for Delivery address
                    CustomerAddressInfo shippingAddress = Controller.GetCustomerAddress(ShippingAddressId);
                    if (shippingAddress != null)
                    {
                        int postalCode = -1;
                        Int32.TryParse(shippingAddress.PostalCode, out postalCode);
                        if (!String.IsNullOrEmpty(shippingAddress.CountryCode))
                            shippingCountryCode = shippingAddress.CountryCode;
                        shippingZoneId = Controller.GetShippingZoneIdByAddress(shippingModelId, shippingCountryCode , postalCode);
                        shippingCountry = shippingAddress.Country;
                        if (shippingCountry == String.Empty)
                        {
                            ListController lc = new ListController();
                            ListEntryInfoCollection leic = lc.GetListEntryInfoCollection("Country", "");
                            foreach (ListEntryInfo lei in leic)
                            {
                                if (lei.Value == shippingCountryCode)
                                {
                                    shippingCountry = lei.Text;
                                    break;
                                }
                            }
                        }
                    }
                }

                
                if (shippingZoneId > -1 || costwithoutZoneId)
                {
                    // Do we have ShippingCosts with the shippingZoneId
                    bool costwithZoneId = (from s in shippingCosts where s.ShippingZoneID == shippingZoneId select s).Any();

                    if (costwithZoneId)
                        shippingCosts = (from s in shippingCosts where s.ShippingZoneID == shippingZoneId select s).ToList();
                    else
                        shippingCosts = (from s in shippingCosts where s.ShippingZoneID == -1 select s).ToList();

                    // Now we have all shippingcosts that fit to our situation. Lets check which one is the one!
                    ShippingCostInfo flat = (from s in shippingCosts where s.FlatCharge select s).FirstOrDefault();
                    ShippingCostInfo theOne = (from s in shippingCosts where s.PerPart select s).FirstOrDefault();
                    List<ShippingCostInfo> withWeight = (from s in shippingCosts where s.MinWeight > -1 select s).ToList();
                    List<ShippingCostInfo> withPrice = (from s in shippingCosts where s.MinPrice > -1 select s).ToList();
                    if (flat != null)
                    {
                        shippingSum += flat.ShippingPrice;
                    }
                    else if (theOne != null)
                    {
                        shippingSum += sumPieces * theOne.ShippingPrice;
                    }
                    else if (withWeight.Count > 0)
                    {
                        theOne =
                            (from w in withWeight where w.MinWeight <= sumWeight && w.MaxWeight >= sumWeight select w)
                                .FirstOrDefault();
                        if (theOne != null)
                        {
                            shippingSum += theOne.ShippingPrice;
                        }
                    }
                    else if (withPrice.Count > 0)
                    {
                        theOne =
                            (from w in withPrice where w.MinPrice <= sumCost && w.MaxPrice >= sumCost select w)
                                .FirstOrDefault();
                        if (theOne != null)
                        {
                            shippingSum += theOne.ShippingPrice;
                        }
                    }
                    bool addZeroShipping = Convert.ToBoolean(StoreSettings["AddZeroShipping"] ?? "false");
                    if (shippingSum > 0 || addZeroShipping)
                    {
                        ShippingZoneDisplayInfo zone = Controller.GetShippingZoneById(shippingZoneId, CurrentLanguage);
                        CartAdditionalCostInfo addCost = new CartAdditionalCostInfo();
                        addCost.CartId = CartId;
                        addCost.Area = "SHIPPING";
                        addCost.Quantity = 1.0m;
                        addCost.TaxPercent = shippingTaxPercent;
                        if (zone == null)
                        {
                            addCost.Name = LocalizeString("ShippingText.Text");
                            addCost.UnitCost = shippingSum;
                        }
                        else
                        {
                            addCost.Name = zone.OrderText;
                            if (zone.ExemptionLimit > sumCost || zone.ExemptionLimit < 0)
                                addCost.UnitCost = shippingSum;
                            else if (addZeroShipping)
                                addCost.UnitCost = 0m;
                        }
                        Controller.NewCartAdditionalCost(addCost);
                    }
                }
                else
                {
                    cmdFinish.Visible = false;
                    ErrorText += String.Format(LocalizeString("NoShipping.Text"), shippingCountry);
                }
            }


            //Hashtable storeSettings = Controller.GetStoreSettings(PortalId);

            //decimal shippingTaxPercent = 0.0m;
            //if (storeSettings["ShippingTax"] != null)
            //    Decimal.TryParse(((string)storeSettings["ShippingTax"]).Replace(",", "."), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out shippingTaxPercent);
            //decimal shippingCost = 0.0000m;
            //if (storeSettings["ShippingCost"] != null)
            //    Decimal.TryParse(((string)storeSettings["ShippingCost"]).Replace(",", "."), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out shippingCost);
            //decimal shippingCostInt = 0.0000m;
            //if (storeSettings["ShippingCostInt"] != null)
            //    Decimal.TryParse(((string)storeSettings["ShippingCostInt"]).Replace(",", "."), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out shippingCostInt);
            //decimal shippingFree = 0.0000m;
            //if (storeSettings["ShippingFree"] != null)
            //    Decimal.TryParse(((string)storeSettings["ShippingFree"]).Replace(",", "."), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out shippingFree);
            //decimal shippingFreeInt = 0.0000m;
            //if (storeSettings["ShippingFreeInt"] != null)
            //    Decimal.TryParse(((string)storeSettings["ShippingFreeInt"]).Replace(",", "."), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out shippingFreeInt);
            //string shippingType = (string)storeSettings["ShippingType"];
            //string shippingTypeInt = (string)storeSettings["ShippingTypeInt"];

            //// do we have to add Shipping Costs ?
            //// Last we eventually have to add extra cost to CartAdditionalCost
            //Controller.DeleteCartAdditionalCost(CartId, "SHIPPING");
            //if (shippingCost > 0 && Cart.OrderTotal > 0)
            //{
            //    CartAdditionalCostInfo addCost = new CartAdditionalCostInfo();
            //    addCost.CartId = CartId;
            //    addCost.Area = "SHIPPING";
            //    addCost.Quantity = 1.0m;
            //    addCost.TaxPercent = shippingTaxPercent;

            //    CustomerAddressInfo shippingAddress = Controller.GetCustomerAddress(ShippingAddressId);
            //    if (shippingAddress.CountryCode.Trim() != (string)StoreSettings["VendorCountry"])
            //    {
            //        if (shippingFreeInt > 0 && Cart.OrderTotal < shippingFreeInt)
            //        {
            //            addCost.Name = shippingTypeInt;
            //            addCost.UnitCost = shippingCostInt;
            //            Controller.NewCartAdditionalCost(addCost);
            //        }
            //    }
            //    else
            //    {
            //        if (shippingFree > 0 && Cart.OrderTotal < shippingFree)
            //        {
            //            addCost.Name = shippingType;
            //            addCost.UnitCost = shippingCost;
            //            Controller.NewCartAdditionalCost(addCost);
            //        }
            //    }

            //}
        }

        private void UpdatePayment()
        {
            // Last we eventually have to add extra cost to CartAdditionalCost
            Controller.DeleteCartAdditionalCost(CartId, "PAYMENT");
            Cart = Controller.GetCart(PortalId, CartId);
            SubscriberPaymentProviderInfo spp = Controller.GetSubscriberPaymentProviderByCPP(Cart.CustomerPaymentProviderID);
            if (spp != null)
            {
                decimal paymentCost = 0.0m;
                if (spp.CostPercent != 0)
                {
                    decimal total = Cart.OrderTotal;
                    paymentCost += total * spp.CostPercent / 100;
                }
                if (spp.Cost != 0)
                    paymentCost += spp.Cost;

                if (paymentCost != 0)
                {
                    CartAdditionalCostInfo addCost = new CartAdditionalCostInfo();
                    addCost.CartId = CartId;
                    addCost.Area = "PAYMENT";
                    PaymentProviderInfo pp = Controller.GetPaymentProvider(spp.PaymentProviderId, CurrentLanguage);
                    addCost.Name = pp.ProviderName;
                    addCost.Quantity = 1m;
                    addCost.UnitCost = paymentCost;
                    addCost.TaxPercent = spp.TaxPercent;
                    Controller.NewCartAdditionalCost(addCost);
                }
            }
        }

        private void UpdateCoupon(CouponInfo coupon)
        {
            // Last we eventually have to add extra cost to CartAdditionalCost
            Controller.DeleteCartAdditionalCost(CartId, "COUPON");
            Cart = Controller.GetCart(PortalId, CartId);
            decimal couponCost = 0.0m;
            if (coupon.DiscountPercent != null && coupon.DiscountPercent > 0)
            {
                decimal total = Cart.OrderTotal;
                couponCost += total * (decimal)coupon.DiscountPercent / 100;
            }
            if (coupon.DiscountValue != null && coupon.DiscountValue > 0)
                couponCost += (decimal)coupon.DiscountValue;

            if (couponCost > 0)
            {
                CartAdditionalCostInfo addCouponCost = new CartAdditionalCostInfo();
                addCouponCost.CartId = CartId;
                addCouponCost.Area = "COUPON";
                addCouponCost.Name = coupon.Caption;
                addCouponCost.Quantity = 1m;
                addCouponCost.UnitCost = (-1) * couponCost;
                addCouponCost.TaxPercent = coupon.TaxPercent;
                Controller.NewCartAdditionalCost(addCouponCost);
                Controller.UpdateCartCouponId(CartId,coupon.CouponId);
                pnlCoupon.Visible = false;
            }
        }


        private void UpdateDiscount()
        {
            Controller.DeleteCartAdditionalCost(CartId, "DISCOUNT");
            List<CartProductInfo> cpl = Controller.GetCartProducts(CartId);
            decimal Discount = 0.00m;
            Hashtable ht = new Hashtable();
            foreach (CartProductInfo cp in cpl)
            {
                if (cp.ProductDiscount == String.Empty)
                    continue;

                List<CartProductInfo> acpl = cpl.FindAll(p => p.ProductId == cp.ProductId);
                if (acpl.Count > 0 && acpl[0].ProductDiscount != String.Empty)
                {
                    decimal quantity = 0m;
                    foreach (CartProductInfo acp in acpl)
                    {
                        quantity += acp.Quantity;
                    }
                    Discount = cp.UnitCost - Controller.GetDiscount(acpl[0].ProductDiscount, quantity, cp.UnitCost, cp.TaxPercent);
                    if (Discount > 0)
                    {
                        if (ht.ContainsKey(cp.ProductId))
                            ht[cp.ProductId] = (decimal)ht[cp.ProductId] + Discount * cp.Quantity;
                        else
                            ht.Add(cp.ProductId, Discount * cp.Quantity);
                    }
                }
            }
            foreach (DictionaryEntry de in ht)
            {
                CartProductInfo cp = cpl.Find(p => p.ProductId == (int)de.Key);
                CartAdditionalCostInfo ac = new CartAdditionalCostInfo();
                ac.Area = "DISCOUNT";
                ac.CartId = CartId;
                ac.Name = Localization.GetString("Discount.Text", this.LocalResourceFile) + " " + cp.Name;
                ac.Quantity = 1m;
                ac.UnitCost = (-1) * (decimal)de.Value;
                ac.TaxPercent = cp.TaxPercent;
                Controller.NewCartAdditionalCost(ac);
            }
        }

        internal void MailOrder(int OrderId)
        {
            string storeEmail = (string)StoreSettings["StoreEmail"] ?? "";
            string storeName = (string)StoreSettings["StoreName"] ?? "";
            string storeReplyTo = (string)StoreSettings["StoreReplyTo"] ?? "";
            string storeAdmin = (string)StoreSettings["StoreAdmin"] ?? "";

            OrderInfo order = Controller.GetOrder(OrderId);

            string templateContent = GenOrderHtml(order, true);

            try
            {
                // http://www.systemnetmail.com

                MailMessage mail = new MailMessage();

                //set the addresses
                string smtpServer = DotNetNuke.Entities.Host.Host.SMTPServer;
                string smtpAuthentication = DotNetNuke.Entities.Host.Host.SMTPAuthentication;
                string smtpUsername = DotNetNuke.Entities.Host.Host.SMTPUsername;
                string smtpPassword = DotNetNuke.Entities.Host.Host.SMTPPassword;

                if (Convert.ToInt32(StoreSettings["SMTPSettings"] ?? "0") == 1)
                {
                    smtpServer = (string)StoreSettings["SMTPServer"];
                    smtpAuthentication = "1";
                    smtpUsername = (string)StoreSettings["SMTPUser"];
                    smtpPassword = (string)StoreSettings["SMTPPassword"]; ;
                }

                mail.From = new MailAddress("\"" + storeName.Trim() + "\" <" + storeEmail.Trim() + ">");
                mail.To.Add(UserInfo.Email.Trim());
                if (storeAdmin != string.Empty)
                    mail.To.Add(storeAdmin.Trim());
                if (storeReplyTo != string.Empty)
                    mail.ReplyTo = new MailAddress(storeReplyTo.Trim());

                //set the content
                string orderNo = order.OrderNo.Trim();
                if (!String.IsNullOrEmpty(order.OrderName))
                    orderNo += ": " + order.OrderName;

                mail.Subject = ((string)StoreSettings["StoreSubject"]).Replace("[ORDERNO]", orderNo);

                AlternateView av1 = AlternateView.CreateAlternateViewFromString(templateContent, null, "text/html");
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

                if (order.Attachment != null)
                {
                    MemoryStream stream = new MemoryStream(order.Attachment);
                    Attachment attachFile = new Attachment(stream, order.AttachName, order.AttachContentType);
                    mail.Attachments.Add(attachFile);
                }

                // Adding Images + Descriptions of Attributes (if any)
                foreach (OrderProductInfo orderProduct in Controller.GetOrderProducts(order.OrderID))
                {
                    MemoryStream zipOutMemoryStream = new MemoryStream();
                    ZipOutputStream zipOutStream = new ZipOutputStream(zipOutMemoryStream);
                    int attachCount = 0;

                    foreach (OrderProductOptionInfo option in Controller.GetOrderProductOptions(orderProduct.OrderProductId))
                    {
                        String fileName = option.OptionName;
                        //Byte[] bytes = Encoding.Convert(Encoding.UTF8, Encoding.Default,Encoding.UTF8.GetBytes(fileName));
                        //fileName = Encoding.Default.GetString(bytes);

                        if (option != null && option.OptionImage != null && option.OptionImage.Length > 0)
                        {
                            attachCount++;
                            MemoryStream stream = new MemoryStream(option.OptionImage);

                            ZipEntry entry = new ZipEntry(fileName + ".jpg");
                            zipOutStream.PutNextEntry(entry);

                            byte[] photoBytesBuffer = stream.ToArray();
                            zipOutStream.Write(photoBytesBuffer, 0, photoBytesBuffer.Length);

                            stream.Dispose();
                            zipOutStream.CloseEntry();
                        }
                        if (option != null && !String.IsNullOrEmpty(option.OptionDescription))
                        {
                            attachCount++;
                            byte[] byteArray = Encoding.Default.GetBytes(option.OptionDescription);

                            ZipEntry entry = new ZipEntry(fileName + ".txt");
                            zipOutStream.PutNextEntry(entry);

                            zipOutStream.Write(byteArray, 0, byteArray.Length);
                            zipOutStream.CloseEntry();
                        }
                    }
                    zipOutStream.Finish();
                    zipOutStream.Close();
                    zipOutMemoryStream.Close();
                    byte[] responseBytes = zipOutMemoryStream.ToArray();
                    zipOutMemoryStream.Dispose();
                    zipOutStream.Dispose();
                    if (attachCount > 0)
                    {
                        MemoryStream str = new MemoryStream(responseBytes);
                        Attachment attachFile = new Attachment(str, orderProduct.Name + ".zip", "application/zip");
                        mail.Attachments.Add(attachFile);
                    }
                }

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

        public string GenOrderHtml(OrderInfo order, bool replaceOrderState)
        {
            string vendorName = (string)StoreSettings["VendorName"] ?? "";
            string vendorStreet1 = (string)StoreSettings["VendorStreet1"] ?? "";
            string vendorStreet2 = (string)StoreSettings["VendorStreet2"] ?? "";
            string vendorZip = (string)StoreSettings["VendorZip"] ?? "";
            string vendorCity = (string)StoreSettings["VendorCity"] ?? "";
            string shippingType = (string)Settings["ShippingType"] ?? "";
            string headerMessage = Localization.GetString("EmailHeader.Message", this.LocalResourceFile);
            string footerMessage = Localization.GetString("EmailFooter.Message", this.LocalResourceFile);

            bool ColVisibleImage = Convert.ToBoolean(Settings["ColVisibleImage"]);
            bool ColVisibleUnit = Convert.ToBoolean(Settings["ColVisibleUnit"]);
            bool ColVisibleItemNo = Convert.ToBoolean(Settings["ColVisibleItemNo"]);
            bool ColVisibleUnitCost = Convert.ToBoolean(Settings["ColVisibleUnitCost"]);
            bool ColVisibleNetTotal = Convert.ToBoolean(Settings["ColVisibleNetTotal"]);
            bool ColVisibleTaxPercent = Convert.ToBoolean(Settings["ColVisibleTaxPercent"]);
            bool ColVisibleTaxTotal = Convert.ToBoolean(Settings["ColVisibleTaxTotal"]);
            bool ColVisibleSubTotal = Convert.ToBoolean(Settings["ColVisibleSubTotal"]);
            bool ShowSummary = Convert.ToBoolean(Settings["ShowSummary"]);


            // Lets retrieve the template
            TemplateControl tp = LoadControl("controls/TemplateControl.ascx") as TemplateControl;
            tp.Key = "Order";
            string template = tp.GetTemplate((string)(Settings["Template"] ?? "Order"));

            template = template.Replace("[BBSTORE-VENDORIMAGE]", (PortalSettings.LogoFile != string.Empty ? "<img src=\"cid:Logo\" />" : ""));
            template = template.Replace("[BBSTORE-HEADERMESSAGE]", headerMessage);

            template = template.Replace("[BBSTORE-ORDERTEXT]", Localization.GetString("EmailOrder.Text", this.LocalResourceFile));
            template = template.Replace("[BBSTORE-INFOTEXT]", Localization.GetString("EmailInfo.Text", this.LocalResourceFile));
            template = template.Replace("[BBSTORE-ADDRESSINFOTEXT]", Localization.GetString("EmailAddressInfo.Text", this.LocalResourceFile));
            template = template.Replace("[BBSTORE-PRODUCTSTEXT]", Localization.GetString("EmailProducts.Text", this.LocalResourceFile));
            template = template.Replace("[BBSTORE-COMMENTSTEXT]", Localization.GetString("EmailComments.Text", this.LocalResourceFile));
            template = template.Replace("[BBSTORE-PAYMENTTEXT]", Localization.GetString("EmailPayment.Text", this.LocalResourceFile));
            template = template.Replace("[BBSTORE-SHIPPINGTEXT]", Localization.GetString("EmailShipping.Text", this.LocalResourceFile));

            template = template.Replace("[BBSTORE-ORDERNO]", order.OrderNo);
            template = template.Replace("[BBSTORE-ORDERDATE]", order.OrderTime.Date.ToString("D"));

            if (replaceOrderState)
            {
                List<OrderStateInfo> orderStates = Controller.GetOrderStates(PortalId, CurrentLanguage);
                if (orderStates.Count == 0)
                    orderStates = Controller.GetOrderStates(PortalId, DefaultLanguage);
                if (orderStates.Count == 0)
                    throw new KeyNotFoundException("No Orderstates found!");
                string orderState = orderStates.Find(s => s.OrderStateId == order.OrderStateId).OrderState;
                template = template.Replace("[BBSTORE-ORDERSTATE]", orderState);
            }

            template = template.Replace("[BBSTORE-VENDORNAME]", vendorName);
            template = template.Replace("[BBSTORE-VENDORSTREET1]", vendorStreet1);
            template = template.Replace("[BBSTORE-VENDORSTREET2]", vendorStreet2);
            template = template.Replace("[BBSTORE-VENDORZIP]", vendorZip);
            template = template.Replace("[BBSTORE-VENDORCITY]", vendorCity);

            List<OrderAddressInfo> orderAddresses = Controller.GetOrderAddresses(order.OrderID, CurrentLanguage);
            string addressTemplate = Localization.GetString("AddressTemplate.Text", this.LocalResourceFile);
            string addressText = "";
            switch (orderAddresses.Count)
            {
                case 0:
                    break;
                case 1:
                    addressText = "<span class=\"Normal\"><b>" + orderAddresses[0].AddressType + "</b></span><br />" +
                        "<span class=\"Normal\">" + orderAddresses[0].ToHtml(addressTemplate, false) + "</span>";
                    break;
                case 2:
                    addressText = "<table style=\"width:100%;\"><tr>" +
                                  "<td style=\"width:50%;\" class=\"Normal\"><b>" + orderAddresses[0].AddressType + "</b></td>" +
                                  "<td style=\"width:50%;\" class=\"Normal\"><b>" + orderAddresses[1].AddressType + "</b></td>" +
                                  "</tr>" +
                                  "<tr>" +
                                  "<td width=\"50%\" class=\"Normal\">" + orderAddresses[0].ToHtml(addressTemplate, false) + "</td>" +
                                  "<td width=\"50%\" class=\"Normal\">" + orderAddresses[1].ToHtml(addressTemplate, false) + "</td>" +
                                  "</tr></table>";
                    break;
                default:
                    string head = "";
                    string body = "";
                    string html = "";
                    for (int i = 0; i < orderAddresses.Count; i++)
                    {
                        if (i % 3 == 0)
                        {
                            head += "<tr>";
                            body += "<tr>";
                        }
                        head += "<td style=\"width:33%;\" class=\"Normal\"><b>" + orderAddresses[i].AddressType + "</b></td>";
                        body += "<td style=\"width:33%;\" class=\"Normal\">" + orderAddresses[i].ToHtml(addressTemplate, false) + "</td>";
                        if (i % 3 == 2)
                        {
                            html = html + head + "</tr>" + body + "</tr>";
                            head = "";
                            body = "";
                        }
                    }
                    addressText = "<table style=\"width:100%;\">" + html + "</table>";
                    break;
            }
            template = template.Replace("[BBSTORE-ADDRESSTEXT]", addressText);

            List<OrderProductInfo> productList = Controller.GetOrderProducts(order.OrderID);
            List<OrderAdditionalCostInfo> additionalList = Controller.GetOrderAdditionalCosts(order.OrderID);
            string orderItems = "<tr class=\"Normal\" style=\"background-color:#ECECEC\">" +
                                "<th style=\"vertical-align:top;text-align:right;padding-right:5px\">" +
                                Localization.GetString("Quantity.Header", this.LocalResourceFile) + "</th>" +
                                (ColVisibleUnit
                                    ? "<th style=\"vertical-align:top;text-align:left\">" +
                                      Localization.GetString("Unit.Header", this.LocalResourceFile) + "</th>"
                                    : "") +
                                (ColVisibleItemNo
                                    ? "<th style=\"vertical-align:top;text-align:left\">" +
                                      Localization.GetString("ItemNo.Header", this.LocalResourceFile) + "</th>"
                                    : "") +
                                "<th style=\"vertical-align:top;text-align:left\">" +
                                Localization.GetString("Product.Header", this.LocalResourceFile) + "</th>" +
                                (ColVisibleUnitCost
                                    ? "<th style=\"vertical-align:top;text-align:right\">" +
                                      Localization.GetString("UnitCost.Header", this.LocalResourceFile) + "</th>"
                                    : "") +
                                (ColVisibleNetTotal
                                    ? "<th style=\"vertical-align:top;text-align:right\">" +
                                      Localization.GetString("NetTotal.Header", this.LocalResourceFile) + "</th>"
                                    : "") +
                                (ColVisibleTaxPercent
                                    ? "<th style=\"vertical-align:top;text-align:right\">" +
                                      Localization.GetString("TaxPercent.Header", this.LocalResourceFile) + "</th>"
                                    : "") +
                                (ColVisibleTaxTotal
                                    ? "<th style=\"vertical-align:top;text-align:right\">" +
                                      Localization.GetString("TaxTotal.Header", this.LocalResourceFile) + "</th>"
                                    : "") +
                                (ColVisibleSubTotal
                                    ? "<th style=\"vertical-align:top;text-align:right\">" +
                                      Localization.GetString("SubTotal.Header", this.LocalResourceFile) + "</th>"
                                    : "") +
                                "</tr>";

            string productTemplate = "<tr class=\"Normal\" style=\"[STYLE]\">" +
                                     "<td style=\"vertical-align:top;text-align:right;padding-right:5px\">[PRODUCTQUANTITY]</td>" +
                                     (ColVisibleUnit
                                        ? "<td style=\"vertical-align:top;text-align:left\">[PRODUCTUNIT]</td>"
                                        : "") +
                                     (ColVisibleItemNo
                                        ? "<td style=\"vertical-align:top;text-align:left\">[PRODUCTITEMNO]</td>"
                                        : "") +
                                     "<td style=\"vertical-align:top;text-align:left\">[PRODUCTNAME]</td>" +
                                     (ColVisibleUnitCost
                                        ? "<td style=\"vertical-align:top;text-align:right\">[PRODUCTUNITCOST]</td>"
                                        : "") +
                                     (ColVisibleNetTotal
                                        ? "<td style=\"vertical-align:top;text-align:right\">[PRODUCTNETTOTAL]</td>"
                                        : "") +
                                     (ColVisibleTaxPercent
                                        ? "<td style=\"vertical-align:top;text-align:right\">[PRODUCTTAXPERCENT]</td>"
                                        : "") +
                                     (ColVisibleTaxTotal
                                        ? "<td style=\"vertical-align:top;text-align:right\">[PRODUCTTAXTOTAL]</td>"
                                        : "") +
                                     (ColVisibleSubTotal
                                        ? "<td style=\"vertical-align:top;text-align:right\">[PRODUCTSUBTOTAL]</td>"
                                        : "") +
                                     "</tr>";

            int loop = 0;
            foreach (OrderProductInfo product in productList)
            {
                orderItems += productTemplate;
                string artname = product.Name.Trim();
                List<OrderProductOptionInfo> optionList = Controller.GetOrderProductOptions(product.OrderProductId);
                foreach (OrderProductOptionInfo option in optionList)
                {
                    artname += "<br /><span style=\"font-size:x-small;\">" + option.OptionName.Trim() + ": " + option.OptionValue.Trim() +
                               @"</span>";
                }
                orderItems = orderItems.Replace("[PRODUCTNAME]", artname);
                orderItems = orderItems.Replace("[PRODUCTQUANTITY]", String.Format("{0:G}", Convert.ToDouble(product.Quantity)));
                orderItems = orderItems.Replace("[PRODUCTUNIT]", product.Unit.Trim());
                orderItems = orderItems.Replace("[PRODUCTITEMNO]", product.ItemNo.Trim());
                orderItems = orderItems.Replace("[PRODUCTUNITCOST]", String.Format("{0:n2}", product.UnitCost));
                orderItems = orderItems.Replace("[PRODUCTNETTOTAL]", String.Format("{0:n2}", product.NetTotal));
                orderItems = orderItems.Replace("[PRODUCTTAXPERCENT]", String.Format("{0:n2}", product.TaxPercent));
                orderItems = orderItems.Replace("[PRODUCTTAXTOTAL]", String.Format("{0:n2}", product.TaxTotal));
                orderItems = orderItems.Replace("[PRODUCTSUBTOTAL]", String.Format("{0:n2}", product.SubTotal));
                if (loop % 2 == 0)
                    orderItems = orderItems.Replace("[STYLE]", "background-color:#F8F8F8");
                else
                    orderItems = orderItems.Replace("[STYLE]", "background-color:#FFFFFF");
                loop++;
            }
            foreach (OrderAdditionalCostInfo addCost in additionalList)
            {
                orderItems += productTemplate;

                string artname = addCost.Name.Trim();

                orderItems = orderItems.Replace("[PRODUCTNAME]", artname);
                orderItems = orderItems.Replace("[PRODUCTQUANTITY]", String.Format("{0:G}", Convert.ToDouble(addCost.Quantity)));
                orderItems = orderItems.Replace("[PRODUCTUNIT]", "");
                orderItems = orderItems.Replace("[PRODUCTITEMNO]", "");
                orderItems = orderItems.Replace("[PRODUCTUNITCOST]", String.Format("{0:n2}", addCost.UnitCost));
                orderItems = orderItems.Replace("[PRODUCTNETTOTAL]", String.Format("{0:n2}", addCost.NetTotal));
                orderItems = orderItems.Replace("[PRODUCTTAXPERCENT]", String.Format("{0:n2}", addCost.TaxPercent));
                orderItems = orderItems.Replace("[PRODUCTTAXTOTAL]", String.Format("{0:n2}", addCost.TaxTotal));
                orderItems = orderItems.Replace("[PRODUCTSUBTOTAL]", String.Format("{0:n2}", addCost.SubTotal));
                if (loop % 2 == 0)
                    orderItems = orderItems.Replace("[STYLE]", "background-color:#F8F8F8");
                else
                    orderItems = orderItems.Replace("[STYLE]", "background-color:#FFFFFF");
                loop++;
            }

            Decimal NetTotal = decimal.Round(order.OrderTotal, 2) + decimal.Round(order.AdditionalTotal, 2);
            Decimal TaxTotal = decimal.Round(order.OrderTax, 2) + decimal.Round(order.AdditionalTax, 2);
            Decimal SubTotal = decimal.Round(order.AdditionalTotal + order.AdditionalTax, 2) +
                               decimal.Round(order.OrderTotal + order.OrderTax, 2);

            orderItems += productTemplate;
            orderItems = orderItems.Replace("[STYLE]", "background-color:#ECECEC;font-weight:bold;");
            orderItems = orderItems.Replace("[PRODUCTNAME]", Localization.GetString("Total.Text", this.LocalResourceFile));
            orderItems = orderItems.Replace("[PRODUCTQUANTITY]", "");
            orderItems = orderItems.Replace("[PRODUCTUNIT]", "");
            orderItems = orderItems.Replace("[PRODUCTITEMNO]", "");
            orderItems = orderItems.Replace("[PRODUCTUNITCOST]", "");
            orderItems = orderItems.Replace("[PRODUCTNETTOTAL]", String.Format("{0:n2}", NetTotal));
            orderItems = orderItems.Replace("[PRODUCTTAXPERCENT]", "");
            orderItems = orderItems.Replace("[PRODUCTTAXTOTAL]", String.Format("{0:n2}", TaxTotal));
            orderItems = orderItems.Replace("[PRODUCTSUBTOTAL]", String.Format("{0:n2}", SubTotal));

            template = template.Replace("[BBSTORE-ORDERITEMS]", orderItems);
            template = template.Replace("[BBSTORE-COMMENT]", order.Comment);

            if (order.PaymentProviderId > 0)
            {
                PaymentProviderInfo pp = Controller.GetPaymentProvider(order.PaymentProviderId, CurrentLanguage);
                SubscriberPaymentProviderInfo sp = Controller.GetSubscriberPaymentProvider(PortalId, 0, order.PaymentProviderId);

                Page pageHolder = new Page();
                PaymentProviderBase ctrl =
                    pageHolder.LoadControl(@"~\DesktopModules\BBStore\Providers\Payment\" + pp.ProviderControl.Trim() + ".ascx") as
                    PaymentProviderBase;
                ctrl.LocalResourceFile = @"~\DesktopModules\BBStore\Providers\Payment\" + pp.ProviderControl.Trim() + ".ascx";
                ctrl.Values = order.PaymentProviderValues;
                ctrl.Properties = sp.PaymentProviderProperties;
                ctrl.DisplayMode = ViewMode.Summary;
                pageHolder.Controls.Add(ctrl);
                StringWriter sw = new StringWriter();
                HttpContext.Current.Server.Execute(pageHolder, sw, false);

                string desc = sw.ToString();
                string paymentText = "<b>" + pp.ProviderName + "</b><p>&nbsp;</p>" + desc;
                template = template.Replace("[BBSTORE-PAYMENT]", paymentText);
            }
            else
            {
                template = template.Replace("[BBSTORE-PAYMENT]", "");
            }
            template = template.Replace("[BBSTORE-SHIPPING]", shippingType);

            template = template.Replace("[BBSTORE-FOOTERMESSAGE]", footerMessage);
            return template;
        }


        public string GetNextAction()
        {
            
            if (Cart == null)
                return "cart";
            if (Settings["MultipleCustomers"] != null && Convert.ToBoolean(Settings["MultipleCustomers"]) && CustomerId == -1)
                return "customer";
            // if not IsAuthenticated, it leads first to login and after Login automatically to checkout
            if (!Request.IsAuthenticated)
                return "checkout";
            // ValidCardAddresses: Are all cart adresses valid (mandatory fields are set)
            int invalidCustomerAddressId = Controller.GetInvalidValidCartAddress(CartId, PortalId);
            if (invalidCustomerAddressId > -1)
                return "adredit&adrid=" + invalidCustomerAddressId.ToString();
            // CheckCartAddresses: Is for all mandatory Adresstypes an adress present ?
            if (!Controller.CheckCartAddresses(CartId, PortalId, 0))
                return "checkout";
            if (Cart.CustomerPaymentProviderID < 0 && !SkipPayment)
                return "payment";
            return "confirm";
        }

        private void EndPaypalCheckout(CustomerPaymentProviderInfo customerPaymentProvider, int orderId)
        {
            string[] buyerProps = customerPaymentProvider.PaymentProviderValues.Split(',');

            SubscriberPaymentProviderInfo subscriberPaymentProvider = Controller.GetSubscriberPaymentProvider(PortalId, Cart.SubscriberID, customerPaymentProvider.PaymentProviderId);
            string[] sellerProps = subscriberPaymentProvider.PaymentProviderProperties.Split(',');
            PaypalApi.UseSandbox = (sellerProps.Length == 3 || (sellerProps.Length > 3 && Convert.ToBoolean(sellerProps[3]) == true));

            PaypalUserCredentials cred = new PaypalUserCredentials(sellerProps[0], sellerProps[1], sellerProps[2]);

            PaypalPaymentDetails details = new PaypalPaymentDetails();
            details.Amt = Cart.OrderTax + Cart.OrderTotal + Cart.AdditionalTax + Cart.AdditionalTotal;
            details.CurrencyCode = "EUR";

            string response = PaypalApi.DoExpressCheckoutPayment(cred, details, buyerProps[0], buyerProps[1]);

            PaypalCommonResponse ppCommon = new PaypalCommonResponse(response);
            if (ppCommon.Ack == PaypalAckType.Success)
            {
                // Customerpaymentprovider-Values löschen
                customerPaymentProvider.PaymentProviderValues = "";
                Controller.UpdateCustomerPaymentProvider(customerPaymentProvider);

                PaypalPaymentResult payResult = new PaypalPaymentResult(response);
                if (payResult.PaymentStatus == PaypalPaymentStatus.Completed)
                {
                    Controller.SetOrderState(orderId, 5); // Payed
                }
            }
            else
            {
                string message = "";
                foreach (PaypalError error in ppCommon.Errors)
                {
                    message += String.Format("ErrorNo:{0} Message {1}", error.ErrorNo, error.LongMessage);
                }
                ErrorText += message;
            }
        }

        #endregion



        public ModuleActionCollection ModuleActions
        {
            get
            {
                ModuleActionCollection actions = new ModuleActionCollection();
                // actions.Add(GetNextActionID(), Localization.GetString(ModuleActionType.AddContent, this.LocalResourceFile),
                //   ModuleActionType.AddContent, "", "add.gif", EditUrl(), false, DotNetNuke.Security.SecurityAccessLevel.Edit,
                //    true, false);
                return actions;
            }
        }

        
    }
}