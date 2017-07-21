// 
// DotNetNuke® - http://www.dotnetnuke.com 
// Copyright (c) 2002-2009 
// by DotNetNuke Corporation 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions: 
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software. 
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE. 
// 

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Bitboxx.License;
using DotNetNuke.Application;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;

using Bitboxx.Web.GeneratedImage;
using System.Reflection;


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
    [DNNtc.PackageProperties("BBStore Product", 1, "BBStore Product", "BBStore Product", "BBStore.png", "Torsten Weggen", "bitboxx solutions", "http://www.bitboxx.net", "info@bitboxx.net", false)]
    [DNNtc.ModuleProperties("BBStore Product", "BBStore Product", 0)]
    [DNNtc.ModuleDependencies(DNNtc.ModuleDependency.CoreVersion, "08.00.00")]
    [DNNtc.ModuleDependencies(DNNtc.ModuleDependency.Package, "BBImageHandler")]
    [DNNtc.ModuleControlProperties("", "BBStore Simple Product", DNNtc.ControlType.View, "", false, false)]
    partial class ViewProduct : PortalModuleBase, IActionable
    {

        public string Currency = "EUR";
        
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
                string  cartId;
                if (Request.Cookies["BBStoreCartId_"+PortalId.ToString()] != null)
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

        public ModuleKindEnum ModuleKind
        {
            get { return ModuleKindEnum.Product; }
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
                {
                    _storeSettings = Controller.GetStoreSettings(PortalId);
                }
                return _storeSettings;
            }
        }

        private BBStoreController _controller;
        private List<OptionListInfo> _productOptions = new List<OptionListInfo>();
        private ModuleInfo _cartModule;
        private ProductOptionSelectControl _productOptionSelect;
		private FeatureGridControl _featureGrid;
        private Label _lblPrice;
		private Label _lblOriginalPrice;
        private Label _lblMandatory;
        private LinkButton _lnkAddCart;
		private LinkButton _lnkAskOffer;
        private ImageButton _imgAddCart;
        private TextBox _txtAmount;
        private GeneratedImage _imgProduct;
        private bool _isConfigured;
	    private bool _isVisible = true;
        private SimpleProductInfo _simpleProduct;
        private bool _hasCartModule = true;
        private bool _hasContactModule = true;
        private bool _showNetPrice = true;
        private Hashtable _storeSettings = null;
            
        
        #region "Event Handlers"

        /// ----------------------------------------------------------------------------- 
        /// <summary> 
        /// Page_Load runs when the control is loaded 
        /// </summary> 
        /// ----------------------------------------------------------------------------- 
        protected void Page_Init(object sender, System.EventArgs e)
        {
            // Are we in Admin mode
			UserInfo user = UserController.GetCurrentUserInfo();
			if (user.IsInRole("Administrators") && IsEditable)
				pnlAdmin.Visible = true;

			ModuleController objModules = new ModuleController();
            _cartModule = objModules.GetModuleByDefinition(PortalId, "BBStore Cart");
            if (_cartModule == null)
                _hasCartModule = false;

            ModuleInfo contactModule = objModules.GetModuleByDefinition(PortalId, "BBStore Contact");
            if (contactModule == null)
                _hasContactModule = false;


            // First lets check if we have a cart. if not lets create one
            CartInfo myCart = Controller.GetCart(PortalId, CartId);
            if (myCart == null)
            {
                myCart = new CartInfo();
                myCart.CartID = CartId;
                Controller.NewCart(PortalId, myCart); 
            }

            // First we determine the associating ProductId
            int productId = -1; // dynamic

            if (Settings["ProductId"] != null)
                productId = Convert.ToInt32(Settings["ProductId"]);

            // Do we have a dynamic module and receive a product with the Url ?
            if (productId == -1)
            {
                if (Request.QueryString["ProductId"] != null)
                {
                    productId = Int32.Parse(Request.QueryString["ProductId"]);
                }
            }

            _isConfigured = true;

            if (Settings["Template"] == null)
            {
                string message = Localization.GetString("NotConfigured.Message", this.LocalResourceFile);
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, message, ModuleMessage.ModuleMessageType.YellowWarning);
                _isConfigured = false;
            }

          
            // Now we retrieve the SimpleProduct
            if (productId > -1)
            {
                bool extendedPrice = Convert.ToBoolean(StoreSettings["ExtendedPrice"]);
                _simpleProduct = Controller.GetSimpleProductByProductId(PortalId, productId, CurrentLanguage,UserId, extendedPrice);
                if (_simpleProduct == null)
                    _simpleProduct = Controller.GetSimpleProductByProductId(PortalId, productId, DefaultLanguage,UserId, extendedPrice);
                if (_simpleProduct == null)
                    _isConfigured = false;
            }
            else
            {
                _isConfigured = false;
                if (IsEditable)
                {
                    string message = Localization.GetString("Dynamic.Message", this.LocalResourceFile);
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, message, ModuleMessage.ModuleMessageType.BlueInfo);
                }
                else
                {
                    if (Settings["ListModulePage"] != null)
                    {
                        int listModuleTabId = Convert.ToInt32(Settings["ListModulePage"]);
                        Response.Redirect(Globals.NavigateURL(listModuleTabId, "", "productgroup=-1"),true);
                    }
                    else
                        _isVisible = false;
                }
            }
            
            if (_isConfigured)
            {
                SimpleProductInfo product = _simpleProduct;
                var ctrl = RenderItem(product);
                PlaceHolder1.Controls.Add(ctrl);
            }
        }

        protected void Page_PreRender(object sender, System.EventArgs e)
        {
            cmdEdit.Visible = _isConfigured;
			imgEdit.Visible = _isConfigured;

			this.ContainerControl.Visible = _isVisible;
			if (_isConfigured && _isVisible)
			{
				try
				{
                    // Perhaps we have some ProductOptions to set
                    // Lets read the selectedValues
                    if (_productOptionSelect != null)
                    {
                        List<OptionListInfo> options = new List<OptionListInfo>();
                        if (Request.QueryString["cpoid"] != null)
                        {
                            if (_lnkAddCart != null)
                                _lnkAddCart.Text = Localization.GetString("lnkUpdateCart.Text", this.LocalResourceFile);
                            if (!IsPostBack)
                            {
                                int cpoId = Convert.ToInt32(Request.QueryString["cpoid"]);
                                if (cpoId > 0)
                                {

                                    if (_txtAmount != null)
                                    {
                                        CartProductInfo product = Controller.GetCartProduct(cpoId);
                                        _txtAmount.Text = string.Format("{0:G}", Convert.ToDouble(product.Quantity));
                                    }

                                    List<CartProductOptionInfo> cpo = Controller.GetCartProductOptions(cpoId);
                                    foreach (CartProductOptionInfo po in cpo)
                                    {
                                        OptionListInfo ol = new OptionListInfo();
                                        ol.OptionName = po.OptionName;
                                        ol.OptionValue = po.OptionValue;
                                        ol.OptionImage = po.OptionImage;
                                        ol.OptionDescription = po.OptionDescription;
                                        options.Add(ol);
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (string urlParameter in Request.QueryString.AllKeys)
                            {
                                if (urlParameter.ToLower() != "productid")
                                {
                                    OptionListInfo ol = new OptionListInfo();
                                    ol.OptionName = urlParameter;
                                    ol.OptionValue = Request.QueryString[urlParameter];
                                    options.Add(ol);
                                }
                            }
                        }

                        _productOptionSelect.SelectedOptions = options;
                    }
                    
                    
                    // Now lets calculate price and tax 
                    decimal unitCost = _simpleProduct.UnitCost;
					decimal originalUnitCost = _simpleProduct.OriginalUnitCost;
					decimal taxPercent = _simpleProduct.TaxPercent;

					decimal price = 0.00m;
					decimal originalPrice = 0.00m;

					// Handling Discount
					decimal amount = 1;
					if (_txtAmount != null)
					{
						decimal.TryParse(_txtAmount.Text, out amount);
					}
					decimal discountedUnitCost = Controller.GetDiscount(GetDiscountLine(_simpleProduct.Attributes), amount, unitCost, taxPercent);
					decimal discountedOriginalUnitCost = Controller.GetDiscount(GetDiscountLine(_simpleProduct.Attributes), amount, originalUnitCost, taxPercent);

                    // Add class to container if discount is reached
				    string discountClass = "bbstore-product-discountenabled";
				    string classes = divProduct.Attributes["class"] ?? "";
                    classes = classes.Replace(discountClass,"").Trim();

				    if (unitCost != discountedUnitCost)
				        classes += " " + discountClass;
				    divProduct.Attributes["class"] = classes.Trim();

				    unitCost = discountedUnitCost;
				    originalUnitCost = discountedOriginalUnitCost;

					if (_showNetPrice)
					{
						price = unitCost + (_productOptionSelect != null ? _productOptionSelect.PriceAlteration : 0.00m);
						originalPrice = originalUnitCost + (_productOptionSelect != null ? _productOptionSelect.PriceAlteration : 0.00m);
					}
					else
					{
						price = decimal.Round((unitCost + (_productOptionSelect != null ? _productOptionSelect.PriceAlteration : 0.00m)) * (100 + taxPercent) / 100, 2);
						originalPrice = decimal.Round((originalUnitCost + (_productOptionSelect != null ? _productOptionSelect.PriceAlteration : 0.00m)) * (100 + taxPercent) / 100, 2);
					}

					if (_lblPrice != null)
						_lblPrice.Text = String.Format("{0:n2}", price);

					if (_lblOriginalPrice != null)
					{
						if (originalPrice > price)
							_lblOriginalPrice.Text = String.Format("{0:n2} {1}", originalPrice, Currency);
						else
							_lblOriginalPrice.Visible = false;
					}


 					if (_simpleProduct.HideCost)
					{
						if (_lblPrice != null) _lblPrice.Visible = false;
						if (_lblOriginalPrice!= null) _lblOriginalPrice.Visible = false;
					}

					// We can set the Title of our Module
				    if (Convert.ToBoolean(Settings["SetModuleTitle"] ?? "true"))
				    {
				        string titleLabelName = DotNetNukeContext.Current.Application.Version.Major < 6 ? "lblTitle" : "titleLabel";
				        Control ctl = Globals.FindControlRecursiveDown(this.ContainerControl, titleLabelName);
				        if ((ctl != null))
				        {
				            ((Label) ctl).Text = _simpleProduct.Name;
				        }
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
        protected void imgAddCart_Click(object sender, ImageClickEventArgs e)
        {
            AddCart();
        }
        protected void lnkAddCart_Click(object sender, EventArgs e)
        {
            AddCart();
        }
		protected void lnkAskOffer_Click(object sender, EventArgs e)
		{
		    string selectedAttributes = "";
            foreach (OptionListInfo oli in _productOptionSelect.SelectedOptions)
            {
                selectedAttributes += oli.OptionValue != String.Empty ? "<br/>" + oli.OptionName + " = " + oli.OptionValue : "";
            }
            
            Controller.NewContactProduct(CartId, _simpleProduct.SimpleProductId, -1, selectedAttributes);
			if (Settings["ContactModulePage"] != null)
			{
				int TabId = Convert.ToInt32(Settings["ContactModulePage"]);
				if (TabId > 0)
					Response.Redirect(Globals.NavigateURL(TabId));
			}
		}

		protected void cmdNew_Click(object sender, EventArgs e)
		{
			Response.Redirect(EditUrl());
		}
		protected void cmdEdit_Click(object sender, EventArgs e)
		{
			if (Request.QueryString["Productgroup"] != null)
				Response.Redirect(EditUrl("productgroup",Request.QueryString["Productgroup"],"","productId", _simpleProduct.SimpleProductId.ToString()));
			else
				Response.Redirect(EditUrl("productId", _simpleProduct.SimpleProductId.ToString()));
		}

        #endregion

        #region "Private methods"

        private Control RenderItem(SimpleProductInfo product)
        {
            int isTaxIncluded = Convert.ToInt32(Settings["ShowNetPrice"]);
            if (isTaxIncluded == -1 && _hasCartModule)
            {
                Hashtable storeSettings = Controller.GetStoreSettings(PortalId);
                _showNetPrice = (string)storeSettings["ShowNetpriceInCart"] == "0";
            }
            else
                _showNetPrice = (isTaxIncluded == 0);

            decimal taxPercent = product.TaxPercent;
            string tax = _showNetPrice ? Localization.GetString("ExcludeTax.Text", this.LocalResourceFile) : Localization.GetString("IncludeTax.Text", this.LocalResourceFile);

            int imageWidth = 0;

            TemplateControl tp = LoadControl("controls/TemplateControl.ascx") as TemplateControl;
            tp.Key = "SimpleProduct";
            string template = tp.GetTemplate((string)Settings["Template"]);

            template = template.Replace("[ITEMNO]", product.ItemNo);
            template = template.Replace("[PRODUCTSHORTDESCRIPTION]", product.ShortDescription);
            template = template.Replace("[PRODUCTDESCRIPTION]", product.ProductDescription);
            template = template.Replace("[TITLE]", product.Name);
            template = template.Replace("[IMAGEURL]", product.Image);

            template = template.Replace("[PRICE]", "<asp:Label ID=\"lblPrice\" runat=\"server\" />");
            template = template.Replace("[ORIGINALPRICE]", "<asp:Label ID=\"lblOriginalPrice\" runat=\"server\" />");

            if (!product.HideCost)
                template = template.Replace("[CURRENCY]", Currency);
            else
                template = template.Replace("[CURRENCY]", "");

            if (taxPercent > 0.00m && !product.HideCost)
                template = template.Replace("[TAX]", String.Format(tax, taxPercent));
            else
                template = template.Replace("[TAX]", "");

            if (product.UnitId > -1)
            {
                UnitInfo unit = Controller.GetUnit(product.UnitId, CurrentLanguage);
                template = template.Replace("[UNIT]", unit.Symbol);
            }
            else
                template = template.Replace("[UNIT]", "");

            template = template.Replace("[IMAGE]", "<img src=\"" + PortalSettings.HomeDirectory + product.Image + "\" alt=\"" + product.Name + "\" />");

            while (template.IndexOf("[IMAGELINK") > -1)
            {
                string imageUrlHandler = Page.ResolveUrl("~\\BBImagehandler.ashx") + "?&Mode=FitSquare&File=" + HttpUtility.UrlEncode(PortalSettings.HomeDirectoryMapPath + product.Image);
                string imageUrl = PortalSettings.HomeDirectory + product.Image;

                if (template.IndexOf("[IMAGELINK:") > -1)
                {
                    string width;
                    width = template.Substring(template.IndexOf("[IMAGELINK:") + 11);
                    width = width.Substring(0, width.IndexOf("]"));
                    if (Int32.TryParse(width, out imageWidth) == false)
                        imageWidth = 200;
                    template = template.Replace("[IMAGELINK:" + width + "]", imageUrlHandler + "&Width=" + imageWidth.ToString());
                }
                else
                    template = template.Replace("[IMAGELINK]", imageUrl);
            }

            int imageScrollerWidth = 0;
            int imageScrollerCount = 0;
            if (template.IndexOf("[IMAGESCROLLER:") > -1)
            {
                string parameters = template.Substring(template.IndexOf("[IMAGESCROLLER:") + 15);
                parameters = parameters.Substring(0, parameters.IndexOf("]"));
                string[] paraArr = parameters.Split('|');
                if (paraArr.Length == 2 && int.TryParse(paraArr[0], out imageScrollerWidth) && int.TryParse(paraArr[1], out imageScrollerCount))
                {
                    template = template.Replace("[IMAGESCROLLER:" + parameters + "]", "<asp:PlaceHolder ID=\"phImageScroller\" runat=\"server\" />");
                }
            }

            int imageCnt = 0;
            Queue<int> imageWidths = new Queue<int>();
            while (template.Contains("[IMAGE:"))
            {
                imageCnt++;
                
                string width = template.Substring(template.IndexOf("[IMAGE:") + 7);
                width = width.Substring(0, width.IndexOf("]"));
                if (Int32.TryParse(width, out imageWidth) == false)
                    imageWidth = 200;
                imageWidths.Enqueue(imageWidth);
                template = template.ReplaceFirst("[IMAGE:" + width + "]", "<asp:PlaceHolder ID=\"phimgProduct" + imageCnt.ToString() + "\" runat=\"server\" />");
            }

            if (template.IndexOf("[YOUTUBE:") > -1)
            {
                string width;
                width = template.Substring(template.IndexOf("[YOUTUBE:") + 9);
                width = width.Substring(0, width.IndexOf("]"));
                FeatureGridValueInfo fgv = Controller.GetFeatureGridValueByProductAndToken(PortalId, product.SimpleProductId, CurrentLanguage, "YOUTUBE");
                string embedCode = "";
                if (fgv != null && fgv.cValue.Trim() != String.Empty)
                    embedCode = String.Format("<iframe title=\"YouTube video player\" class=\"youtube-player\"	type=\"text/html\" width=\"{0}\" src=\"http://www.youtube.com/embed/{1}\" frameborder=\"0\"	allowFullScreen=\"true\"></iframe>", width, fgv.cValue.Trim());
                template = template.Replace("[YOUTUBE:" + width + "]", embedCode);
            }

            while (template.IndexOf("[RESOURCE:") > -1)
            {
                string resKey = template.Substring(template.IndexOf("[RESOURCE:") + 10);
                resKey = resKey.Substring(0, resKey.IndexOf("]"));
                template = template.Replace("[RESOURCE:" + resKey + "]",
                    Localization.GetString(resKey, this.LocalResourceFile));
            }

            if (template.IndexOf("[FEATURE:") > -1)
            {
                while (template.IndexOf("[FEATURE:") > -1)
                {
                    string token = template.Substring(template.IndexOf("[FEATURE:") + 9);
                    token = token.Substring(0, token.IndexOf("]"));
                    string prop = token.Substring(token.IndexOf(".") + 1);
                    token = token.Substring(0, token.IndexOf("."));

                    string value = "";
                    FeatureGridValueInfo fgv = Controller.GetFeatureGridValueByProductAndToken(PortalId, product.SimpleProductId, CurrentLanguage, token.ToUpper());
                    if (fgv != null)
                    {
                        PropertyInfo p = fgv.GetType().GetProperty(prop);
                        if (p != null && p.CanRead)
                            value = p.GetValue(fgv, null).ToString();
                    }
                    template = template.Replace("[FEATURE:" + token + "." + prop + "]", value);
                }
            }
            template = template.Replace("[PRODUCTOPTIONS]", "<asp:PlaceHolder ID=\"phProductOptions\" runat=\"server\" />");
            template = template.Replace("[MANDATORYERROR]", "<asp:Label ID=\"lblMandatory\" runat=\"server\" Visible=\"false\" Resourcekey=\"Mandatory.Error\" />");
            template = template.Replace("[ADDCARTIMAGE]", "<asp:ImageButton ID=\"imgAddCart\" runat=\"server\" />");
            template = template.Replace("[ADDCARTLINK]", "<asp:LinkButton ID=\"lnkAddCart\" runat=\"server\" /><asp:LinkButton ID=\"lnkAskOffer\" runat=\"server\"  Visible=\"false\" />");
            template = template.Replace("[AMOUNT]", "<asp:TextBox ID=\"txtAmount\" runat=\"server\" />");
            template = template.Replace("[FEATURES]", "<asp:PlaceHolder ID=\"phFeatureGrid\" runat=\"server\" />");


            Control ctrl = ParseControl(template);

            PlaceHolder phProduktOptions = FindControlRecursive(ctrl, "phProductOptions") as PlaceHolder;
            if (phProduktOptions != null)
            {
                _productOptionSelect = LoadControl("Controls/ProductOptionSelectControl.ascx") as ProductOptionSelectControl;
                _productOptionSelect.ProductModule = this;
                phProduktOptions.Controls.Add(_productOptionSelect);
            }
            _lblMandatory = FindControlRecursive(ctrl, "lblMandatory") as Label;
            _lblPrice = FindControlRecursive(ctrl, "lblPrice") as Label;
            _lblOriginalPrice = FindControlRecursive(ctrl, "lblOriginalPrice") as Label;

            _lnkAskOffer = FindControlRecursive(ctrl, "lnkAskOffer") as LinkButton;
            if (_lnkAskOffer != null)
            {
                _lnkAskOffer.Click += new EventHandler(lnkAskOffer_Click);
                _lnkAskOffer.Text = Localization.GetString("lnkAskOffer.Text", this.LocalResourceFile);
                _lnkAskOffer.Visible = _hasContactModule && product.NoCart;
            }

            _lnkAddCart = FindControlRecursive(ctrl, "lnkAddCart") as LinkButton;
            if (_lnkAddCart != null)
            {
                _lnkAddCart.Click += new EventHandler(lnkAddCart_Click);
                _lnkAddCart.Text = Localization.GetString("lnkAddcart.Text", this.LocalResourceFile);
                _lnkAddCart.Visible = _hasCartModule && !_simpleProduct.NoCart;
            }
            _imgAddCart = FindControlRecursive(ctrl, "imgAddCart") as ImageButton;
            if (_imgAddCart != null)
            {
                _imgAddCart.Click += new ImageClickEventHandler(imgAddCart_Click);
                _imgAddCart.ImageUrl = Localization.GetString("imgAddCart.ImageUrl", this.LocalResourceFile);
                _imgAddCart.Visible = _hasCartModule && !product.NoCart;
            }
            _txtAmount = FindControlRecursive(ctrl, "txtAmount") as TextBox;
            if (_txtAmount != null)
            {
                // AutoPostback only when Discount on Amounts (a line starts with @)
                if (product.Attributes.Contains("\n@") || product.Attributes.StartsWith("@"))
                    _txtAmount.AutoPostBack = true;
                _txtAmount.Text = "1";
                _txtAmount.Columns = 3;
                _txtAmount.Visible = _hasCartModule && !product.NoCart;
            }

            imageCnt = 1;
            PlaceHolder phimgProduct = FindControlRecursive(ctrl, "phimgProduct" + imageCnt.ToString()) as PlaceHolder;
            while (phimgProduct != null && product.Image != null)
            {
                string fileName =
                    PortalSettings.HomeDirectoryMapPath.Replace(HttpContext.Current.Request.PhysicalApplicationPath, "") +
                    product.Image.Replace('/', '\\');

                _imgProduct = new GeneratedImage();
                _imgProduct.ImageHandlerUrl = "~/BBImageHandler.ashx";
                imageWidth = imageWidths.Dequeue();
                if (imageWidth > 0)
                    _imgProduct.Parameters.Add(new ImageParameter() { Name = "Width", Value = imageWidth.ToString() });
                _imgProduct.Parameters.Add(new ImageParameter() { Name = "File", Value = fileName });
                // TODO: Watermark
                //if (false)
                //{
                //    imgProduct.Parameters.Add(new ImageParameter() { Name = "WatermarkText", Value = Localization.GetString("Sold.Text", this.LocalResourceFile) });
                //    imgProduct.Parameters.Add(new ImageParameter() { Name = "WatermarkFontFamily", Value = "Verdana" });
                //    imgProduct.Parameters.Add(new ImageParameter() { Name = "WatermarkFontColor", Value = "Red" });
                //    imgProduct.Parameters.Add(new ImageParameter() { Name = "WatermarkFontSize", Value = "20" });
                //}
                phimgProduct.Controls.Add(_imgProduct);

                imageCnt++;
                phimgProduct = FindControlRecursive(ctrl, "phimgProduct" + imageCnt.ToString()) as PlaceHolder;
            }


            PlaceHolder phFeatureGrid = FindControlRecursive(ctrl, "phFeatureGrid") as PlaceHolder;
            if (phFeatureGrid != null)
            {
                _featureGrid = LoadControl("Controls/FeatureGridControl.ascx") as FeatureGridControl;
                _featureGrid.ProductId = product.SimpleProductId;
                _featureGrid.FeatureGridCssClass = "bbstore-grid";
                _featureGrid.GroupCssClass = "bbstore-grid-header";
                _featureGrid.FeatureRowCssClass = "bbstore-grid-row";
                _featureGrid.FeatureCaptionCssClass = "bbstore-grid-caption";
                _featureGrid.AlternateFeatureRowCssClass = "bbstore-grid-alternaterow";
                phFeatureGrid.Controls.Add(_featureGrid);
            }

            PlaceHolder phImageScroller = FindControlRecursive(ctrl, "phImageScroller") as PlaceHolder;
            if (phImageScroller != null)
            {
                ImageScrollerControl imgScroller = LoadControl("Controls/ImageScrollerControl.ascx") as ImageScrollerControl;
                imgScroller.ImageWidth = imageScrollerWidth;
                imgScroller.ImageCount = imageScrollerCount;
                if (_imgProduct != null)
                    imgScroller.ImageControl = _imgProduct;
                int posSlash = product.Image.LastIndexOf('/');
                if (posSlash > 0)
                    imgScroller.ImageDirectory = product.Image.Substring(0, posSlash);
                else
                    imgScroller.ImageDirectory = "";
                phImageScroller.Controls.Add(imgScroller);
            }

            // Handle the ProductOptions
            try
            {
                if (_productOptionSelect != null && product.Attributes != String.Empty)
                {
                    string Attributes = product.Attributes;
                    Attributes = Attributes.Replace("\r", "");
                    string[] AttrLines = Attributes.Split(new char[] { '\n' });

                    // Parse the attributes and fill ProductOption
                    ParseAttributes(AttrLines);
                    _productOptionSelect.ShowNetPrice = _showNetPrice;
                    _productOptionSelect.Product = product;
                    _productOptionSelect.ProductOptions = _productOptions;
                }
            }
            catch (Exception exc)
            {
                //Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }

            return ctrl;
        }

        private void AddCart()
        {
            if (_productOptionSelect != null && _productOptionSelect.IsComplete == false)
            {
                if (_lblMandatory != null)
                    _lblMandatory.Visible = true;
            }
            else
            {
                decimal Amount = 1;

                if (_lblMandatory != null)
                    _lblMandatory.Visible = false;

                if (_txtAmount != null )
                {
                    Decimal.TryParse(_txtAmount.Text, out Amount);
                }

                // Determine ShippingModel
                int shippingModelId = -1;
                ProductShippingModelInfo productShippingModel = Controller.GetProductShippingModelsByProduct(_simpleProduct.SimpleProductId).FirstOrDefault();
                if (productShippingModel != null)
                    shippingModelId = productShippingModel.ShippingModelId;
                
                // First add / update the Product in Cart
            	CartProductInfo cartProduct;
            	bool isUpdate = false;
				if (Request.QueryString["cpoid"] != null)
				{
					cartProduct = Controller.GetCartProduct(Convert.ToInt32(Request.QueryString["cpoid"]));
					isUpdate = true;
				}
				else
				{
					cartProduct = Controller.GetCartProductByProductIdAndSelectedOptions(CartId, _simpleProduct.SimpleProductId, _productOptionSelect.SelectedOptions);
				}
                int CartProductId;
                if (cartProduct == null)
                {
                    cartProduct = new CartProductInfo();
                    cartProduct.CartId = CartId;
                    cartProduct.ProductId = _simpleProduct.SimpleProductId;
                    cartProduct.Image = _simpleProduct.Image;
                    cartProduct.ItemNo = _simpleProduct.ItemNo;
                    cartProduct.Name = _simpleProduct.Name;
                    cartProduct.TaxPercent = _simpleProduct.TaxPercent;
                    cartProduct.UnitCost = _simpleProduct.UnitCost;
                    cartProduct.Quantity = Amount;
                    cartProduct.Weight = _simpleProduct.Weight;
                    cartProduct.ShippingModelId = shippingModelId;
                    if (_simpleProduct.UnitId > -1)
                    {
                        UnitInfo unit = Controller.GetUnit(_simpleProduct.UnitId, CurrentLanguage);
                        cartProduct.Unit = unit.Symbol;
                        cartProduct.Decimals = unit.Decimals;
                        cartProduct.Quantity = Math.Round(Amount, cartProduct.Decimals);
                    }

                    cartProduct.ProductDiscount = GetDiscountLine(_simpleProduct.Attributes);
                    CartProductId = Controller.NewCartProduct(CartId, cartProduct);

					// Building the options list for ProductUrl
					List<string> options = new List<string>();
					options.Add("ProductId=" + cartProduct.ProductId.ToString());
					options.Add("cpoid=" + CartProductId);

					cartProduct.ProductUrl = Globals.NavigateURL(TabId, "", options.ToArray());
                	cartProduct.CartProductId = CartProductId;
					Controller.UpdateCartProduct(cartProduct);

                }
                else 
                {
                    CartProductId = cartProduct.CartProductId;

                    if (!isUpdate)
                        cartProduct.Quantity = cartProduct.Quantity + Amount;
                    else
                        cartProduct.Quantity = Amount;

                    cartProduct.Quantity = Math.Round(cartProduct.Quantity, cartProduct.Decimals);

                    Controller.UpdateCartProductQuantity(CartProductId, cartProduct.Quantity);
                }
                // Next delete ProductOptions and add new
                Controller.DeleteCartProductOptions(CartProductId);
                foreach (OptionListInfo oli in _productOptionSelect.SelectedOptions)
                {
                    if (oli.OptionValue != String.Empty)
                    {
                        CartProductOptionInfo CartProductOption = new CartProductOptionInfo();
                        CartProductOption.CartProductId = CartProductId;
                        CartProductOption.OptionName = oli.OptionName;
                        CartProductOption.OptionValue = oli.OptionValue;
	                    CartProductOption.OptionImage = oli.OptionImage;
	                    CartProductOption.OptionDescription = oli.OptionDescription;
                        CartProductOption.PriceAlteration = oli.PriceAlteration;
                        Controller.NewCartProductOption(CartProductId, CartProductOption);
                    }
                }

				string returnUrl;
				if (Request["productgroup"] != null)
					returnUrl = Globals.NavigateURL(TabId, "", "productgroup=" + Request["productgroup"],
													"productid=" + cartProduct.ProductId.ToString());
				else
					returnUrl = Globals.NavigateURL(TabId, "", "productid=" + cartProduct.ProductId.ToString());

				if (Settings["OpenCartOnAdd"] != null && Convert.ToBoolean(Settings["OpenCartOnAdd"]))
				{
					returnUrl = HttpUtility.UrlEncode(returnUrl);
					Response.Redirect(Globals.NavigateURL(_cartModule.TabID, "", "returnUrl=" + returnUrl));
				}
				else
				{ 
					string message = Localization.GetString("AddCart.Message", this.LocalResourceFile);
					DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, message, ModuleMessage.ModuleMessageType.GreenSuccess);
				}
            }
        }
        private string GetDiscountLine(string Attributes)
        {
            string attributes = Attributes.Replace("\r", "");
            string[] AttrLines = attributes.Split(new char[] { '\n' });
            string DiscountLine = "";
            foreach (string Line in AttrLines)
            {
                if (Line.StartsWith("@"))
                {
                    int pos = Line.IndexOf(':');
                    if (pos == -1)
                        pos = 0;
                    DiscountLine = Line.Substring(pos + 1);
                    break;
                }
            }
            return DiscountLine;

        }
        public void ResetWarning()
        {
            if (_lblMandatory != null)
                _lblMandatory.Visible = false;
        }

		private void ParseAttributes(string[] attrLines)
		{
			foreach (string Line in attrLines)
			{
				// Discount is not handled here
				if (Line.StartsWith("@"))
					continue;

			    string lineRest = Line;
                string optionName;
				string optionValues;
				bool isMandatory = false;
				string optionDim = "";
				string optionAdds = "";
				bool askImage = false;
				bool askDesc = false;
			    string control = "";
			    string controlProps = "";

                if (Line.IndexOf('|') > -1)
                {
                    optionAdds = Line.Substring(Line.IndexOf('|') + 1);
                    if (optionAdds.IndexOf("<I>") > -1)
                        askImage = true;
                    if (optionAdds.IndexOf("<D>") > -1)
                        askDesc = true;
                    lineRest = Line.Substring(0, Line.IndexOf('|')).Trim();
                }

			    if (optionAdds.IndexOf("<CONTROL ") > -1)
			    {
			        control = VfpInterop.StrExtract(optionAdds, "<CONTROL ", ">", 1, 1).Trim();
			        int atpos = control.IndexOf(' ');
			        if (atpos > -1)
			        {
			            controlProps = control.Substring(atpos).Trim();
			            control = control.Substring(0, atpos).Trim().ToLower();
			        }
			    }
			    if (control == String.Empty)
			    {
			        if (lineRest.StartsWith("^"))
			            control = "header";
                    else if (lineRest.IndexOf(':') > -1 && lineRest.Substring(lineRest.IndexOf(':')+1) != String.Empty)
			            control = "dropdown";
			        else
			            control = "textbox";
			    }
			    switch (control)
                {
                    case "dropdown":
                    case "colorbox":

                        optionName = lineRest;
			            optionValues = optionName.Substring(optionName.IndexOf(':') + 1);
			            optionName = optionName.Substring(0, optionName.IndexOf(':'));

			            if (optionName.Substring(0, 1) == "!")
			            {
			                isMandatory = true;
			                optionName = optionName.Substring(1);
			            }
			            if (optionName.IndexOf('{') > 0 && optionName.IndexOf('}') > 0)
			            {
			                optionDim = optionName;
			                optionName = optionDim.Substring(0, optionDim.IndexOf('{'));
			                optionDim = optionDim.Substring(optionDim.IndexOf('{') + 1);
			                optionDim = optionDim.Trim().Substring(0, optionDim.Trim().Length - 1);
			            }

			            string[] optionValueArr = optionValues.Split(',');
			            foreach (string optionValue in optionValueArr)
			            {
			                string val = optionValue;
			                string opgVal;
			                string price;
			                decimal opgPrice;
			                bool isDefault = false;
			                bool showDiff = false;

			                if (val.StartsWith("&"))
			                {
			                    isDefault = true;
			                    val = val.Substring(1);
			                }

			                // Check for price diff
			                if (val.IndexOf('[') > 0 && val.IndexOf(']') > 0)
			                {
			                    opgVal = val.Substring(0, val.IndexOf('['));
			                    price = val.Substring(val.IndexOf('[') + 1);
			                    price = price.Trim().Substring(0, price.Trim().Length - 1);
			                    string lastchar = price.Substring(price.Length - 1).ToUpper();
			                    if (lastchar == "T" || lastchar == "%" || lastchar == "N" || lastchar == "B" || lastchar == "G")
			                        price = price.Substring(0, price.Length - 1);
			                    else
			                        lastchar = " ";
			                    if (price.Contains("#"))
			                    {
			                        showDiff = true;
			                        price = price.Replace("#", "");
			                    }

			                    string firstChar = price.Substring(0, 1);
			                    string operAtor = "+";
			                    if (firstChar == "+" || firstChar == "-" || firstChar == "*")
			                    {
			                        operAtor = firstChar;
			                        price = price.Substring(1);
			                    }

			                    if (decimal.TryParse(price, NumberStyles.Number, CultureInfo.InvariantCulture, out opgPrice))
			                    {
			                        switch (operAtor)
			                        {
			                            case "+":
			                                break;
			                            case "-":
			                                opgPrice = (-1)*opgPrice;
			                                break;
			                            case "*":
			                                opgPrice = _simpleProduct.UnitCost*opgPrice - _simpleProduct.UnitCost;
			                                break;
			                        }
			                        if (operAtor != "*")
			                        {
			                            switch (lastchar)
			                            {
			                                case "T":
			                                case "B":
			                                case "G":
			                                    opgPrice = opgPrice/(1 + _simpleProduct.TaxPercent/100);
			                                    break;
			                                case "%":
			                                    opgPrice = _simpleProduct.UnitCost*opgPrice/100;
			                                    break;
			                                case "N":
			                                case " ":
			                                    break;
			                            }
			                        }
			                        _productOptions.Add(new OptionListInfo(optionName, optionDim, opgVal, opgPrice, null, "", isMandatory, isDefault, askImage, askDesc, showDiff, control,controlProps));
			                    }
			                }
			                else
			                {
			                    opgVal = val.Trim();
			                    _productOptions.Add(new OptionListInfo(optionName, optionDim, opgVal, 0.00m, null, "", isMandatory, isDefault, askImage, askDesc, false, control, controlProps));
			                }
			            }
                        break;

                    case "textbox":

                        optionName = lineRest;
                        if (optionName.IndexOf(':') > 0)
                            optionName = optionName.Substring(0, optionName.IndexOf(':'));
			            if (!String.IsNullOrEmpty(optionName))
			            {
			                optionValues = "";
			                if (optionName.Substring(0, 1) == "!")
			                {
			                    isMandatory = true;
			                    optionName = optionName.Substring(1);
			                }

			                if (optionName.IndexOf('{') > 0 && optionName.IndexOf('}') > 0)
			                {
			                    optionDim = optionName;
			                    optionName = optionDim.Substring(0, optionDim.IndexOf('{'));
			                    optionDim = optionDim.Substring(optionDim.IndexOf('{') + 1);
			                    optionDim = optionDim.Trim().Substring(0, optionDim.Trim().Length - 1);
			                }
			                if (!String.IsNullOrEmpty(optionName))
			                    _productOptions.Add(new OptionListInfo(optionName, optionDim, optionValues, 0.00m, null, "", isMandatory, false, askImage, askDesc, false, control, ""));
			            }
                        break;

                    case "header":
                        optionName = lineRest.Substring(1);
                        _productOptions.Add(new OptionListInfo(optionName, "", "", 0.00m, null, "", false, false, false, false, false, control, ""));
                        break;

                    case "area":
                        optionName = lineRest;
                        if (optionName.Substring(0, 1) == "!")
                        {
                            isMandatory = true;
                            optionName = optionName.Substring(1);
                        }
                        _productOptions.Add(new OptionListInfo(optionName, "", "", _simpleProduct.UnitCost, null, "", isMandatory, false, false, false, false, control, controlProps));
                        break;
                }
			}
		}
        #endregion

        #region "Optional Interfaces"

        /// ----------------------------------------------------------------------------- 
        /// <summary> 
        /// Registers the module actions required for interfacing with the portal framework 
        /// </summary> 
        /// <value></value> 
        /// <returns></returns> 
        /// <remarks></remarks> 
        /// <history> 
        /// </history> 
        /// ----------------------------------------------------------------------------- 
        public ModuleActionCollection ModuleActions
        {
            get
            {
                ModuleActionCollection actions = new ModuleActionCollection();
                actions.Add(GetNextActionID(), Localization.GetString("cmdNew.Text", this.LocalResourceFile), ModuleActionType.AddContent, "", "/images/icon_unknown_16px.gif", EditUrl(), false, DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);

                if (_simpleProduct != null)
                {
                    string url = "";
                    if (Request.QueryString["Productgroup"] != null)
                        url = EditUrl("productgroup", Request.QueryString["Productgroup"], "", "productId", _simpleProduct.SimpleProductId.ToString());
                    else
                        url = EditUrl("productId", _simpleProduct.SimpleProductId.ToString());

                    actions.Add(GetNextActionID(), Localization.GetString("cmdEdit.Text", this.LocalResourceFile), ModuleActionType.EditContent, "", "edit.gif", url, false, DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);
                }
                return actions;
            }
        }
        #endregion

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

		#endregion
    }
}