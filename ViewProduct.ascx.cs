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
    [DNNtc.PackageProperties("BBStore Product",1, "BBStore Product", "BBStore Product", "", "Torsten Weggen", "bitboxx solutions", "http://www.bitboxx.net", "info@bitboxx.net",false)]
    [DNNtc.ModuleProperties("BBStore Product", "BBStore Product", 0)]
    [DNNtc.ModuleDependencies(DNNtc.ModuleDependency.CoreVersion, "06.00.00")]
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

        private BBStoreController _controller;
        private List<OptionListInfo> ProductOptions = new List<OptionListInfo>();
        private ModuleInfo CartModule;
        private Literal ProductDescription;
        private ProductOptionSelectControl ProductOptionSelect;
		private FeatureGridControl FeatureGrid;
        private Label lblTax;
        private Label lblUnit;
        private Label lblPrice;
		private Label lblOriginalPrice;
        private Label lblMandatory;
        private Label lblCurrency;
		private Label lblItemNo;
        private Label ShortDescription;
        private LinkButton lnkAddCart;
		private LinkButton lnkAskOffer;
        private ImageButton imgAddCart;
        private TextBox txtAmount;
        private Label lblTitle;
        private GeneratedImage imgProduct;
        private bool IsConfigured;
	    private bool IsVisible = true;
        private SimpleProductInfo SimpleProduct;
        private bool _hasCartModule = true;
        private bool _hasContactModule = true;
        private bool showNetPrice = true;
            
        
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
            CartModule = objModules.GetModuleByDefinition(PortalId, "BBStore Cart");
            if (CartModule == null)
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

            IsConfigured = true;

            if (Settings["Template"] == null)
            {
                string message = Localization.GetString("NotConfigured.Message", this.LocalResourceFile);
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, message, ModuleMessage.ModuleMessageType.YellowWarning);
                IsConfigured = false;
            }

          
            // Now we retrieve the SimpleProduct
            if (productId > -1)
            {
				SimpleProduct = Controller.GetSimpleProductByProductId(PortalId, productId, CurrentLanguage);
                if (SimpleProduct == null)
                    SimpleProduct = Controller.GetSimpleProductByProductId(PortalId, productId, DefaultLanguage);
                if (SimpleProduct == null)
                    IsConfigured = false;
            }
            else
            {
                IsConfigured = false;
                if (IsEditable)
                {
                    string message = Localization.GetString("Dynamic.Message", this.LocalResourceFile);
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, message, ModuleMessage.ModuleMessageType.BlueInfo);
                }
                else
                {
                    IsVisible = false;
                }
            }
            
            if (IsConfigured)
            {
                TemplateControl tp = LoadControl("controls/TemplateControl.ascx") as TemplateControl;
                tp.Key = "SimpleProduct";
                string template = tp.GetTemplate((string)Settings["Template"]);

                int IsTaxIncluded = Convert.ToInt32(Settings["ShowNetPrice"]);
				if (IsTaxIncluded == -1 && _hasCartModule)
				{
					Hashtable storeSettings = Controller.GetStoreSettings(PortalId);
					showNetPrice = (string)storeSettings["ShowNetpriceInCart"] == "0";
				}
				else if (IsTaxIncluded == 1)
					showNetPrice = false;

                int imageWidth = 0;

				template = template.Replace("[ITEMNO]", "<asp:Label ID=\"lblItemNo\" runat=\"server\" />");
                template = template.Replace("[PRODUCTSHORTDESCRIPTION]", "<asp:Label ID=\"lblShortDescription\" runat=\"server\" />");
                template = template.Replace("[PRODUCTDESCRIPTION]", "<asp:Literal ID=\"phProductDescription\" Mode=\"PassThrough\" runat=\"server\" />");
                template = template.Replace("[PRODUCTOPTIONS]", "<asp:PlaceHolder ID=\"phProductOptions\" runat=\"server\" />");
                template = template.Replace("[MANDATORYERROR]", "<asp:Label ID=\"lblMandatory\" runat=\"server\" Visible=\"false\" Resourcekey=\"Mandatory.Error\" />");
				template = template.Replace("[PRICE]", "<asp:Label ID=\"lblPrice\" runat=\"server\" />");
				template = template.Replace("[ORIGINALPRICE]", "<asp:Label ID=\"lblOriginalPrice\" runat=\"server\" />");

				if (template.IndexOf("[IMAGELINK") > -1)
				{
					string ImageUrl = Page.ResolveUrl("~\\BBImagehandler.ashx") + "?&Mode=FitSquare&File=" + HttpUtility.UrlEncode(PortalSettings.HomeDirectoryMapPath + SimpleProduct.Image);
					if (template.IndexOf("[IMAGELINK:") > -1)
					{
						string width;
						width = template.Substring(template.IndexOf("[IMAGELINK:") + 11);
						width = width.Substring(0, width.IndexOf("]"));
						if (Int32.TryParse(width, out imageWidth) == false)
							imageWidth = 200;
						template = template.Replace("[IMAGELINK:" + width + "]", ImageUrl + "&Width=" + imageWidth.ToString());
					}
					else
						template = template.Replace("[IMAGELINK]", ImageUrl);
				}

				int isWidth = 0;
				int isCount = 0;
				if (template.IndexOf("[IMAGESCROLLER:") > -1)
				{
					string parameters = template.Substring(template.IndexOf("[IMAGESCROLLER:") + 15);
					parameters = parameters.Substring(0, parameters.IndexOf("]"));
					string[] paraArr = parameters.Split('|');
					if (paraArr.Length == 2 && int.TryParse(paraArr[0], out isWidth) && int.TryParse(paraArr[1], out isCount))
					{
						template = template.Replace("[IMAGESCROLLER:" + parameters + "]", "<asp:PlaceHolder ID=\"phImageScroller\" runat=\"server\" />");
					}

				}

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

				if (template.IndexOf("[YOUTUBE:") > -1)
				{
					string width;
                    width = template.Substring(template.IndexOf("[YOUTUBE:") + 9);
                    width = width.Substring(0, width.IndexOf("]"));
					FeatureGridValueInfo fgv = Controller.GetFeatureGridValueByProductAndToken(PortalId, SimpleProduct.SimpleProductId, CurrentLanguage, "YOUTUBE");
					string embedCode = "";
					if (fgv != null && fgv.cValue.Trim() != String.Empty)
						 embedCode = String.Format("<iframe title=\"YouTube video player\" class=\"youtube-player\"	type=\"text/html\" width=\"{0}\" src=\"http://www.youtube.com/embed/{1}\" frameborder=\"0\"	allowFullScreen=\"true\"></iframe>",width,fgv.cValue.Trim());
					template = template.Replace("[YOUTUBE:" + width + "]", embedCode);
				}
				
				while (template.IndexOf("[RESOURCE:") > -1)
				{
					string resKey = template.Substring(template.IndexOf("[RESOURCE:") + 10);
					resKey = resKey.Substring(0, resKey.IndexOf("]"));
					template = template.Replace("[RESOURCE:" + resKey + "]",
						Localization.GetString(resKey, this.LocalResourceFile));
				}
                
                template = template.Replace("[CURRENCY]", "<asp:Label ID=\"lblCurrency\" runat=\"server\" />");
                template = template.Replace("[ADDCARTIMAGE]", "<asp:ImageButton ID=\"imgAddCart\" runat=\"server\" />");
				template = template.Replace("[ADDCARTLINK]", "<asp:LinkButton ID=\"lnkAddCart\" runat=\"server\" /><asp:LinkButton ID=\"lnkAskOffer\" runat=\"server\"  Visible=\"false\" />");
                template = template.Replace("[TAX]", "<asp:Label ID=\"lblTax\" runat=\"server\"/>");
                template = template.Replace("[TITLE]", "<asp:Label ID=\"lblTitle\" runat=\"server\" />");
                template = template.Replace("[AMOUNT]", "<asp:TextBox ID=\"txtAmount\" runat=\"server\" />");
                template = template.Replace("[UNIT]", "<asp:Label ID=\"lblUnit\" runat=\"server\"/>");
				template = template.Replace("[FEATURES]", "<asp:PlaceHolder ID=\"phFeatureGrid\" runat=\"server\" />");
				if (template.IndexOf("[FEATURE:") > -1)
				{
					while (template.IndexOf("[FEATURE:") > -1)
					{
						string token = template.Substring(template.IndexOf("[FEATURE:") + 9);
						token = token.Substring(0, token.IndexOf("]"));
						string prop = token.Substring(token.IndexOf(".") + 1);
						token = token.Substring(0, token.IndexOf("."));

						string value = "";
						FeatureGridValueInfo fgv = Controller.GetFeatureGridValueByProductAndToken(PortalId, SimpleProduct.SimpleProductId, CurrentLanguage, token.ToUpper());
						if (fgv != null)
						{
							PropertyInfo p = fgv.GetType().GetProperty(prop);
							if (p != null && p.CanRead)
								value = p.GetValue(fgv, null).ToString();
						}
						template = template.Replace("[FEATURE:" + token + "." + prop + "]", value);
					}
				}

                Control ctrl = ParseControl(template);


                ShortDescription = FindControlRecursive(ctrl, "lblShortDescription") as Label;
                ProductDescription = FindControlRecursive(ctrl, "phProductDescription") as Literal;
				lblItemNo = FindControlRecursive(ctrl, "lblItemNo") as Label;

                PlaceHolder phProduktOptions = FindControlRecursive(ctrl, "phProductOptions") as PlaceHolder;
                if (phProduktOptions != null)
                {
                    ProductOptionSelect = LoadControl("Controls/ProductOptionSelectControl.ascx") as ProductOptionSelectControl;
                    ProductOptionSelect.ProductModule = this;
                    phProduktOptions.Controls.Add(ProductOptionSelect);
                }
                lblMandatory = FindControlRecursive(ctrl, "lblMandatory") as Label;
                lblPrice = FindControlRecursive(ctrl, "lblPrice") as Label;
				lblOriginalPrice = FindControlRecursive(ctrl, "lblOriginalPrice") as Label;
				
				lnkAskOffer = FindControlRecursive(ctrl, "lnkAskOffer") as LinkButton;
				if (lnkAskOffer != null)
				{ 
					lnkAskOffer.Click += new EventHandler(lnkAskOffer_Click);
					lnkAskOffer.Text = Localization.GetString("lnkAskOffer.Text", this.LocalResourceFile);
				    lnkAskOffer.Visible = _hasContactModule && SimpleProduct.NoCart;
				}

                lblTax = FindControlRecursive(ctrl, "lblTax") as Label;
                lnkAddCart = FindControlRecursive(ctrl, "lnkAddCart") as LinkButton;
                if (lnkAddCart != null)
                {
					lnkAddCart.Click += new EventHandler(lnkAddCart_Click);
                    lnkAddCart.Text = Localization.GetString("lnkAddcart.Text", this.LocalResourceFile);
                    lnkAddCart.Visible = _hasCartModule && !SimpleProduct.NoCart;

                }
                imgAddCart = FindControlRecursive(ctrl, "imgAddCart") as ImageButton;
                if (imgAddCart != null)
                {
					imgAddCart.Click += new ImageClickEventHandler(imgAddCart_Click);
                    imgAddCart.ImageUrl = Localization.GetString("imgAddCart.ImageUrl", this.LocalResourceFile);
					imgAddCart.Visible = _hasCartModule && !SimpleProduct.NoCart;
                }
                txtAmount = FindControlRecursive(ctrl, "txtAmount") as TextBox;
                if (txtAmount != null)
                {
                    // AutoPostback only when Discount on Amounts (a line starts with @)
                    if (SimpleProduct.Attributes.Contains("\n@"))
                        txtAmount.AutoPostBack = true;
                    txtAmount.Text = "1";
                    txtAmount.Columns = 3;
					txtAmount.Visible = _hasCartModule && !SimpleProduct.NoCart;
                }
                lblUnit = FindControlRecursive(ctrl, "lblUnit") as Label;
                PlaceHolder phimgProduct = FindControlRecursive(ctrl, "phimgProduct") as PlaceHolder;
                if (phimgProduct != null && SimpleProduct.Image != null)
                {
					string fileName =
							PortalSettings.HomeDirectoryMapPath.Replace(HttpContext.Current.Request.PhysicalApplicationPath, "") +
							SimpleProduct.Image.Replace('/', '\\');
					
					imgProduct = new GeneratedImage();
                    imgProduct.ImageHandlerUrl = "~/BBImageHandler.ashx";
                    if (imageWidth > 0)
                        imgProduct.Parameters.Add(new ImageParameter() { Name = "Width", Value = imageWidth.ToString() });
                    imgProduct.Parameters.Add(new ImageParameter() { Name = "File", Value = fileName });
                    // TODO: Watermark
                    //if (false)
                    //{
                    //    imgProduct.Parameters.Add(new ImageParameter() { Name = "WatermarkText", Value = Localization.GetString("Sold.Text", this.LocalResourceFile) });
                    //    imgProduct.Parameters.Add(new ImageParameter() { Name = "WatermarkFontFamily", Value = "Verdana" });
                    //    imgProduct.Parameters.Add(new ImageParameter() { Name = "WatermarkFontColor", Value = "Red" });
                    //    imgProduct.Parameters.Add(new ImageParameter() { Name = "WatermarkFontSize", Value = "20" });
                    //}
                    phimgProduct.Controls.Add(imgProduct);
                }
 
                lblTitle = FindControlRecursive(ctrl, "lblTitle") as Label;
                
                lblCurrency = FindControlRecursive(ctrl, "lblCurrency") as Label;

				PlaceHolder phFeatureGrid = FindControlRecursive(ctrl, "phFeatureGrid") as PlaceHolder;
				if (phFeatureGrid != null)
				{
					FeatureGrid = LoadControl("Controls/FeatureGridControl.ascx") as FeatureGridControl;
					FeatureGrid.ProductId = productId;
					FeatureGrid.FeatureGridCssClass = "bbstore-grid";
					FeatureGrid.GroupCssClass = "bbstore-grid-header";
					FeatureGrid.FeatureRowCssClass = "bbstore-grid-row";
					FeatureGrid.FeatureCaptionCssClass = "bbstore-grid-caption";
					FeatureGrid.AlternateFeatureRowCssClass = "bbstore-grid-alternaterow";
					phFeatureGrid.Controls.Add(FeatureGrid);
				}

				PlaceHolder phImageScroller = FindControlRecursive(ctrl, "phImageScroller") as PlaceHolder;
				if (phImageScroller != null)
				{
					ImageScrollerControl imgScroller = LoadControl("Controls/ImageScrollerControl.ascx") as ImageScrollerControl;
					imgScroller.ImageWidth = isWidth;
					imgScroller.ImageCount = isCount;
					if (imgProduct != null)
						imgScroller.ImageControl = imgProduct;
					int posSlash = SimpleProduct.Image.LastIndexOf('/');
					if (posSlash > 0)
						imgScroller.ImageDirectory = SimpleProduct.Image.Substring(0, posSlash);
					else
						imgScroller.ImageDirectory = "";
					phImageScroller.Controls.Add(imgScroller);
				}
				
                PlaceHolder1.Controls.Add(ctrl);
                 
                // Handle the ProductOptions
				try
                {
                    if (ProductOptionSelect != null && SimpleProduct.Attributes != String.Empty)
                    {
                        string Attributes = SimpleProduct.Attributes;
                        Attributes = Attributes.Replace("\r", "");
						string[] AttrLines = Attributes.Split(new char[] { '\n' });
						
						// Parse the attributes and fill ProductOption
						ParseAttributes(AttrLines);
						ProductOptionSelect.ShowNetPrice = showNetPrice;
                    	ProductOptionSelect.Product = SimpleProduct;
                        ProductOptionSelect.ProductOptions = ProductOptions;
                    }
                    if (ProductDescription != null)
                        ProductDescription.Text = SimpleProduct.ProductDescription;
                    if (ShortDescription != null)
                        ShortDescription.Text = SimpleProduct.ShortDescription;
					if (lblItemNo != null)
						lblItemNo.Text = SimpleProduct.ItemNo;

                }

                catch (Exception exc)
                {
                    //Module failed to load 
                    Exceptions.ProcessModuleLoadException(this, exc);
                }
            }
        }

        protected void Page_PreRender(object sender, System.EventArgs e)
        {
            cmdEdit.Visible = IsConfigured;
			imgEdit.Visible = IsConfigured;

			this.ContainerControl.Visible = IsVisible;
			if (IsConfigured && IsVisible)
			{
				try
				{
                    // Perhaps we have some ProductOptions to set
                    // Lets read the selectedValues
                    if (ProductOptionSelect != null)
                    {
                        List<OptionListInfo> options = new List<OptionListInfo>();
                        if (Request.QueryString["cpoid"] != null)
                        {
                            lnkAddCart.Text = Localization.GetString("lnkUpdateCart.Text", this.LocalResourceFile);
                            if (!IsPostBack)
                            {
                                int cpoId = Convert.ToInt32(Request.QueryString["cpoid"]);
                                if (cpoId > 0)
                                {

                                    if (txtAmount != null)
                                    {
                                        CartProductInfo product = Controller.GetCartProduct(cpoId);
                                        txtAmount.Text = string.Format("{0:G}", Convert.ToDouble(product.Quantity));
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

                        ProductOptionSelect.SelectedOptions = options;
                    }
                    
                    
                    // Now lets calculate price and tax 
                    decimal unitCost = SimpleProduct.UnitCost;
					decimal originalUnitCost = SimpleProduct.OriginalUnitCost;
					decimal taxPercent = SimpleProduct.TaxPercent;

					decimal price = 0.00m;
					decimal originalPrice = 0.00m;
					string tax = "";

					// Handling Discount
					decimal amount = 1;
					if (txtAmount != null)
					{
						decimal.TryParse(txtAmount.Text, out amount);
					}
					decimal discountedUnitCost = Controller.GetDiscount(GetDiscountLine(SimpleProduct.Attributes), amount, unitCost, taxPercent);
					decimal discountedOriginalUnitCost = Controller.GetDiscount(GetDiscountLine(SimpleProduct.Attributes), amount, originalUnitCost, taxPercent);

                    // Add class to container if discount is reached
				    string discountClass = "bbstore-product-discountenabled";
				    string classes = divProduct.Attributes["class"] ?? "";
                    classes = classes.Replace(discountClass,"").Trim();

				    if (unitCost != discountedUnitCost)
				        classes += " " + discountClass;
				    divProduct.Attributes["class"] = classes.Trim();

				    unitCost = discountedUnitCost;
				    originalUnitCost = discountedOriginalUnitCost;

					if (showNetPrice)
					{
						price = unitCost + (ProductOptionSelect != null ? ProductOptionSelect.PriceAlteration : 0.00m);
						originalPrice = originalUnitCost + (ProductOptionSelect != null ? ProductOptionSelect.PriceAlteration : 0.00m);
						tax = Localization.GetString("ExcludeTax.Text", this.LocalResourceFile);
					}
					else
					{
						price = decimal.Round((unitCost + (ProductOptionSelect != null ? ProductOptionSelect.PriceAlteration : 0.00m)) * (100 + taxPercent) / 100, 2);
						originalPrice = decimal.Round((originalUnitCost + (ProductOptionSelect != null ? ProductOptionSelect.PriceAlteration : 0.00m)) * (100 + taxPercent) / 100, 2);
						tax = Localization.GetString("IncludeTax.Text", this.LocalResourceFile);
					}

					if (lblPrice != null)
						lblPrice.Text = String.Format("{0:n2}", price);

					if (lblOriginalPrice != null)
					{
						if (originalPrice > price)
							lblOriginalPrice.Text = String.Format("{0:n2} {1}", originalPrice, Currency);
						else
							lblOriginalPrice.Visible = false;
					}

					if (lblTax != null)
					{
						lblTax.Text = String.Format(tax, taxPercent);
						if (taxPercent == 0.00m)
							lblTax.Visible = false;
					}
					if (lblTitle != null)
						lblTitle.Text = SimpleProduct.Name;

                    if (lblUnit != null && SimpleProduct.UnitId > -1)
                    {
                        UnitInfo unit = Controller.GetUnit(SimpleProduct.UnitId, CurrentLanguage);
                        lblUnit.Text = unit.Symbol;
                    }

					if (lblCurrency != null)
						lblCurrency.Text = Currency;

 					if (SimpleProduct.HideCost)
					{
						if (lblPrice != null) lblPrice.Visible = false;
						if (lblOriginalPrice!= null) lblOriginalPrice.Visible = false;
						if (lblTax != null) lblTax.Visible = false;
						if (lblCurrency!= null) lblCurrency.Visible = false;
					}

					// We can set the Title of our Module
					string titleLabelName = DotNetNukeContext.Current.Application.Version.Major < 6 ? "lblTitle" : "titleLabel";
					Control ctl = Globals.FindControlRecursiveDown(this.ContainerControl, titleLabelName);
					if ((ctl != null))
					{
						((Label)ctl).Text = SimpleProduct.Name;
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
			Controller.NewContactProduct(CartId, SimpleProduct.SimpleProductId, -1);
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
				Response.Redirect(EditUrl("productgroup",Request.QueryString["Productgroup"],"","productId", SimpleProduct.SimpleProductId.ToString()));
			else
				Response.Redirect(EditUrl("productId", SimpleProduct.SimpleProductId.ToString()));
		}

        #endregion

        #region "Private methods"

        private void AddCart()
        {
            if (ProductOptionSelect != null && ProductOptionSelect.IsComplete == false)
            {
                if (lblMandatory != null)
                    lblMandatory.Visible = true;
            }
            else
            {
                decimal Amount = 1;

                if (lblMandatory != null)
                    lblMandatory.Visible = false;

                if (txtAmount != null )
                {
                    Decimal.TryParse(txtAmount.Text, out Amount);
                }

                // Determine ShippingModel
                int shippingModelId = -1;
                ProductShippingModelInfo productShippingModel = Controller.GetProductShippingModelsByProduct(SimpleProduct.SimpleProductId).FirstOrDefault();
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
					cartProduct = Controller.GetCartProductByProductIdAndSelectedOptions(CartId, SimpleProduct.SimpleProductId, ProductOptionSelect.SelectedOptions);
				}
                int CartProductId;
                if (cartProduct == null)
                {
                    cartProduct = new CartProductInfo();
                    cartProduct.CartId = CartId;
                    cartProduct.ProductId = SimpleProduct.SimpleProductId;
                    cartProduct.Image = SimpleProduct.Image;
                    cartProduct.ItemNo = SimpleProduct.ItemNo;
                    cartProduct.Name = SimpleProduct.Name;
                    cartProduct.TaxPercent = SimpleProduct.TaxPercent;
                    cartProduct.UnitCost = SimpleProduct.UnitCost;
                    cartProduct.Quantity = Amount;
                    cartProduct.Weight = SimpleProduct.Weight;
                    cartProduct.ShippingModelId = shippingModelId;
                    if (SimpleProduct.UnitId > -1)
                    {
                        UnitInfo unit = Controller.GetUnit(SimpleProduct.UnitId, CurrentLanguage);
                        cartProduct.Unit = unit.Symbol;
                        cartProduct.Decimals = unit.Decimals;
                        cartProduct.Quantity = Math.Round(Amount, cartProduct.Decimals);
                    }

                    cartProduct.ProductDiscount = GetDiscountLine(SimpleProduct.Attributes);
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
                foreach (OptionListInfo oli in ProductOptionSelect.SelectedOptions)
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
					Response.Redirect(Globals.NavigateURL(CartModule.TabID, "", "returnUrl=" + returnUrl));
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
            if (lblMandatory != null)
                lblMandatory.Visible = false;
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
			                                opgPrice = SimpleProduct.UnitCost*opgPrice - SimpleProduct.UnitCost;
			                                break;
			                        }
			                        if (operAtor != "*")
			                        {
			                            switch (lastchar)
			                            {
			                                case "T":
			                                case "B":
			                                case "G":
			                                    opgPrice = opgPrice/(1 + SimpleProduct.TaxPercent/100);
			                                    break;
			                                case "%":
			                                    opgPrice = SimpleProduct.UnitCost*opgPrice/100;
			                                    break;
			                                case "N":
			                                case " ":
			                                    break;
			                            }
			                        }
			                        ProductOptions.Add(new OptionListInfo(optionName, optionDim, opgVal, opgPrice, null, "", isMandatory, isDefault, askImage, askDesc, showDiff, control,controlProps));
			                    }
			                }
			                else
			                {
			                    opgVal = val.Trim();
			                    ProductOptions.Add(new OptionListInfo(optionName, optionDim, opgVal, 0.00m, null, "", isMandatory, isDefault, askImage, askDesc, false, control, controlProps));
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
			                    ProductOptions.Add(new OptionListInfo(optionName, optionDim, optionValues, 0.00m, null, "", isMandatory, false, askImage, askDesc, false, control, ""));
			            }
                        break;

                    case "header":
                        optionName = lineRest.Substring(1);
                        ProductOptions.Add(new OptionListInfo(optionName, "", "", 0.00m, null, "", false, false, false, false, false, control, ""));
                        break;

                    case "area":
                        optionName = lineRest;
                        if (optionName.Substring(0, 1) == "!")
                        {
                            isMandatory = true;
                            optionName = optionName.Substring(1);
                        }
                        ProductOptions.Add(new OptionListInfo(optionName, "", "", SimpleProduct.UnitCost, null, "", isMandatory, false, false, false, false, control, controlProps));
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

                if (SimpleProduct != null)
                {
                    string url = "";
                    if (Request.QueryString["Productgroup"] != null)
                        url = EditUrl("productgroup", Request.QueryString["Productgroup"], "", "productId", SimpleProduct.SimpleProductId.ToString());
                    else
                        url = EditUrl("productId", SimpleProduct.SimpleProductId.ToString());

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