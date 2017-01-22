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
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Bitboxx.License;
using Bitboxx.Web.GeneratedImage;
using DotNetNuke.Application;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;

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
    [DNNtc.PackageProperties("BBStore Product List", 4, "BBStore Product List", "BBStore Product List", "BBStore.png", "Torsten Weggen", "bitboxx solutions", "http://www.bitboxx.net", "info@bitboxx.net", false)]
    [DNNtc.ModuleProperties("BBStore Product List", "BBStore Product List", 0)]
    [DNNtc.ModuleControlProperties("", "BBStore Product List", DNNtc.ControlType.View, "", false, false)]
	partial class ViewList : PortalModuleBase, IActionable
	{
		#region Fields
		private const string Currency = "EUR";
		
		BBStoreController _controller;

		private Label lblShortDescription;
		private Literal ltrProductDescription;
		private ProductOptionSelectControl ProductOptionSelect;
		private Label lblTax;
		private Label lblPrice;
		private Label lblOriginalPrice;
		private Label lblMandatory;
        private Label lblUnit;
		private Label lblCurrency;
		private Label lblTitle;
		private Label lblItemNo;
		private GeneratedImage imgProduct;
		private bool IsConfigured = false;
		private SimpleProductInfo SimpleProduct;
		private List<SimpleProductInfo> _products;
	    private int _productsPerPage = 25;
	    private Hashtable _storeSettings = null;
		#endregion

		#region Properties

		public string SortExpression
		{
			get
			{
				if (Settings["RandomSort"] != null && Convert.ToBoolean(Settings["RandomSort"]))
					return "random";
                else if (Request.Cookies["SortExpression"] != null)
                    return Request.Cookies["SortExpression"].Value;
				else
					return "Name";
			}
			set
			{
                if (Request.Cookies["SortExpression"] != null)
                    Response.Cookies["SortExpression"].Value = value.ToString();
                else
                {
                    HttpCookie cookie = new HttpCookie("SortExpression");
                    cookie.Value = value.ToString();
                    cookie.Expires = DateTime.Now.AddDays(1);
                    Response.AppendCookie(cookie);
                }
			}
		}
		public string WhereExpression
		{
			get
			{
				string where = "";
				if (Settings["Selection"] == null || Convert.ToInt32(Settings["Selection"]) == 0)
				{
					List<ProductFilterInfo> filters = Controller.GetProductFilters(PortalId, FilterSessionId);
					int productGroupId = -1;
					foreach (ProductFilterInfo filter in filters)
					{
						if (filter.FilterSource == "ProductGroup")
						{
							string[] valuesProductGroup = filter.FilterValue.Split('|');
							productGroupId = Convert.ToInt32(valuesProductGroup[0]);
							break;
						}
					}
					foreach (ProductFilterInfo filter in filters)
					{
						//where += (where != String.Empty ? " AND " : "") + filter.FilterCondition;
						string FilterCondition = "";
						switch (filter.FilterSource)
						{
							case "TextSearch":
								FilterCondition = Controller.GetSearchTextFilter(PortalId, filter.FilterValue, CurrentLanguage);
								break;
							case "StaticSearch":
								int staticFilterId = Convert.ToInt32(filter.FilterValue);
								FilterCondition = Controller.GetSearchStaticFilter(staticFilterId, CurrentLanguage);
								break;
							case "ProductGroup":
								string[] valuesProductGroup = filter.FilterValue.Split('|');
								int pgi = Convert.ToInt32(valuesProductGroup[0]); 
								bool includeChilds = false;
								if (valuesProductGroup.Length > 1)
									includeChilds = Convert.ToBoolean(valuesProductGroup[1]);
								FilterCondition = Controller.GetProductGroupFilter(PortalId, pgi, includeChilds);
								break;
							case "PriceSearch":
								string[] valuesPrice = filter.FilterValue.Split('|');
								decimal start = 0m;
								decimal end = 0m;
								bool includeTax = (valuesPrice.Length == 3 ? Convert.ToBoolean(valuesPrice[2]) : false);
								if (Decimal.TryParse(valuesPrice[0], NumberStyles.Number, CultureInfo.InvariantCulture, out start) &&
									Decimal.TryParse(valuesPrice[1], NumberStyles.Number, CultureInfo.InvariantCulture, out end))
								{
									FilterCondition = Controller.GetSearchPriceFilter(PortalId, start, end, includeTax);
								}
								break;
							case "FeatureList":
								string[] valuesFeatureList = filter.FilterValue.Split('|');
								int featureListId = Convert.ToInt32(valuesFeatureList[0]);
								int featureListItemId = Convert.ToInt32(valuesFeatureList[1]);
								FilterCondition = Controller.GetSearchFeatureListFilter(featureListId, featureListItemId);
								break;
							case "Feature":
								string[] valuesFeature = filter.FilterValue.Split('|');
								int featureId = Convert.ToInt32(valuesFeature[0]);

								FeatureInfo fi = Controller.GetFeatureById(featureId);
								if (fi != null && Controller.IsFeatureInProductGroup(productGroupId,featureId))
								{
									string value = filter.FilterValue.Substring(filter.FilterValue.IndexOf('|') + 1);
									if (fi.Datatype == "L")
									{
										int fliId = Convert.ToInt32(value);
										if (Controller.IsFeatureListItemInProductGroup(productGroupId,fliId))
											FilterCondition = Controller.GetSearchFeatureFilter(fi.Datatype, value);
									}
									else
										FilterCondition = Controller.GetSearchFeatureFilter(fi.Datatype, value);
								}
								break;
							default:
								break;
						}
						if (!String.IsNullOrEmpty(FilterCondition))
							where += (where != String.Empty ? " AND " : "") + FilterCondition;
					}
				}
				else
				{
					where = Controller.GetSearchStaticFilter(Convert.ToInt32(Settings["StaticFilterId"]), CurrentLanguage);
				}

				if (!IncludeDisabled)
					where += (where != String.Empty ? " AND " : "") + " SimpleProduct.Disabled = 0";
				return where;
			}
		}
		public int TopN
		{
			get
			{
				if (Settings["TopN"] != null)
					return Convert.ToInt32(Settings["TopN"]);
				else
					return 0;
			}
		}
		public int ProductsPerPage
		{
			get
			{
                if (Request.Cookies["ProductsPerPage_" + TabModuleId.ToString()] != null)
                    return Convert.ToInt32(Request.Cookies["ProductsPerPage_" + TabModuleId.ToString()].Value);
				else
				{
					if (Settings["ProductsPerPage"] != null)
					{
						string[] ppp = ((string)Settings["ProductsPerPage"]).Split(',');
						foreach (string s in ppp)
						{
							if (s.IndexOf('!') == 0)
								return Int32.Parse(s.Substring(1));
						}
						return Int32.Parse(ppp[0]);
					}
					else
						return 25;
				}
			}
			set
			{
                if (Request.Cookies["ProductsPerPage_" + TabModuleId.ToString()] != null)
                    Response.Cookies["ProductsPerPage_" + TabModuleId.ToString()].Value = value.ToString();
                else
                {
                    HttpCookie cookie = new HttpCookie("ProductsPerPage_" + TabModuleId.ToString());
                    cookie.Value = value.ToString();
                    cookie.Expires = DateTime.Now.AddHours(1);
                    Response.AppendCookie(cookie);
                }

			}
		}
		public Guid FilterSessionId
		{
			get
			{
				string _filterSessionId;
				if (Request.Cookies["BBStoreFilterSessionId_"+ PortalId.ToString()] != null)
					_filterSessionId = (string)(Request.Cookies["BBStoreFilterSessionId_" + PortalId.ToString()].Value);
				else
				{
					_filterSessionId = Guid.NewGuid().ToString();
					HttpCookie keks = new HttpCookie("BBStoreFilterSessionId_" + PortalId.ToString());
					keks.Value = _filterSessionId;
					keks.Expires = DateTime.Now.AddDays(1);
					Response.AppendCookie(keks);
				}
				return new Guid(_filterSessionId);
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

		public List<SimpleProductInfo> Products
		{
			get
			{
			    if (_products == null)
			    {
			        bool extendedPrice = Convert.ToBoolean(StoreSettings["ExtendedPrice"]);
                    _products = Controller.GetSimpleProducts(PortalId, CurrentLanguage, SortExpression, WhereExpression, TopN, UserId, extendedPrice);
                }
				return _products;
			}
		}
        
        public ModuleKindEnum ModuleKind
        {
            get { return ModuleKindEnum.ProductList; }
        }

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
		protected bool IncludeDisabled
		{
			get
			{
				UserInfo user = UserController.GetCurrentUserInfo();
				return (user.IsInRole("Administrators") && IsEditable);
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
		
		#endregion

		#region Event Handlers
		protected void Page_Init(object sender, System.EventArgs e)
		{
			if (Settings["ProductsInRow"] != null && Settings["ProductsPerPage"] != null)
			{
				IsConfigured = true;
				lstProducts.GroupItemCount = Int32.Parse((string)Settings["ProductsInRow"]);
			}
			else
			{
				string message = Localization.GetString("Configure.Message", this.LocalResourceFile);
				DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, message, ModuleMessage.ModuleMessageType.YellowWarning);
			}
			
			if (Request["productgroup"] != null)
			{
				int productGroupId = Convert.ToInt32(Request["productgroup"]);
				List<ProductFilterInfo> oldFilters =  Controller.GetProductFilter(PortalId, FilterSessionId, "ProductGroup");
				bool includeChilds = false;
				if (oldFilters.Count > 0 )
				{
					string filterValue = oldFilters[0].FilterValue;
					includeChilds = Convert.ToBoolean(filterValue.Substring(filterValue.IndexOf('|') + 1));
				}
				Controller.DeleteProductFilter(PortalId, FilterSessionId, "ProductGroup");
				ProductFilterInfo fi = new ProductFilterInfo();
				fi.FilterSessionId = FilterSessionId;
				fi.FilterSource = "ProductGroup";
				fi.FilterValue = productGroupId.ToString() + "|" + includeChilds.ToString();
				fi.PortalId = PortalId;
				Controller.NewProductFilter(fi);
			}

		}
		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
                if (!Page.IsPostBack && IsConfigured)
				{

					string[] ppp = ((string)Settings["ProductsPerPage"]).Replace("!","").Split(',');
					if (ppp.Length == 1)
					{
						ddlPageSize.Visible = false;
						lblPageSize.Visible = false;
					}
					else
					{
						ddlPageSize.DataSource = ppp;
						ddlPageSize.DataBind();
						ddlPageSize.SelectedValue = ProductsPerPage.ToString();
						ddlPageSize.Visible = true;
						lblPageSize.Visible = true;
					}

					string[] SortKeys = Localization.GetString("Sortbox.Text", this.LocalResourceFile).Split(',');
					for (int i = 0; i < SortKeys.Length; i++)
					{
						ddlSortBox.Items.Add(new ListItem(SortKeys[i], SortKeys[i + 1]));
						i++;
					}

					ddlSortBox.SelectedValue = SortExpression;
					Pager.PageSize = ProductsPerPage;

					lstProducts.DataSource = Products;
					lstProducts.DataBind();

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
            if (IsConfigured)
			{
				bool setTitle = (Settings["SetTitle"] != null ? Convert.ToBoolean((string)Settings["SetTitle"]) : false);

				if (setTitle)
				{
					bool breadcrumb = (Settings["TitleBreadcrumb"] != null ? Convert.ToBoolean((string)Settings["TitleBreadcrumb"]) : false);
					
					string productGroupPath = "";
				    string rootText = Localization.GetString("MainProductGroup.Text", this.LocalResourceFile);
					List<ProductFilterInfo> fi = Controller.GetProductFilter(PortalId, FilterSessionId, "ProductGroup");
					if (fi.Count > 0)
					{
						string[] values = fi[0].FilterValue.Split('|');
						int productGroupId = Convert.ToInt32(values[0]);
						if (breadcrumb)
						{
							string link = Globals.NavigateURL(TabId, "", "productgroup={1}");
							string linkTemplate = "<a href=\"" + link + "\">{0}</a>";
							productGroupPath = Controller.GetProductGroupPath(PortalId, productGroupId, CurrentLanguage, false, " > ", linkTemplate, rootText);
						}
						else if (productGroupId > -1)
						{
							ProductGroupInfo pg = Controller.GetProductGroup(PortalId, CurrentLanguage, productGroupId);
							productGroupPath = pg.ProductGroupName;
						}
					}

                    string titleLabelName = DotNetNukeContext.Current.Application.Version.Major < 6 ? "lblTitle" : "titleLabel";
                    Control ctl = Globals.FindControlRecursiveDown(this.ContainerControl, titleLabelName);
                    if (ctl != null)
                    {
                        ((Label)ctl).Text = productGroupPath != string.Empty ? productGroupPath : rootText;
                    }
				}

                // Header and Footer area
                pnlListHead.Visible = (Settings["ShowListHead"] == null || Convert.ToBoolean(Settings["ShowListHead"]) == true);
                pnlListFooter.Visible = (Products.Count > Pager.PageSize);
				if (Settings["ShowPaging"] != null && Convert.ToBoolean(Settings["ShowPaging"]) == false)
					pnlListFooter.Visible = false;

                // Header and Footer text
                LocalResourceLangInfo localResourceLang = Controller.GetLocalResourceLang(PortalId, "PRODUCTLISTHEADER", CurrentLanguage);
                if (localResourceLang != null && localResourceLang.TextValue != String.Empty)
                    ltrHead.Text = localResourceLang.TextValue;
                
                localResourceLang = Controller.GetLocalResourceLang(PortalId, "PRODUCTLISTFOOTER", CurrentLanguage);
                if (localResourceLang != null && localResourceLang.TextValue != String.Empty)
                    ltrFoot.Text = localResourceLang.TextValue;

			    // No Products found Display
                if (Products.Count == 0)
			    {
			        localResourceLang = Controller.GetLocalResourceLang(PortalId, "PRODUCTLISTEMPTY", CurrentLanguage);
			        if (localResourceLang != null && localResourceLang.TextValue != String.Empty)
			        {
			            ltrEmpty.Text = localResourceLang.TextValue;
			            ltrEmpty.Visible = true;
			        }
			    }


			    LinkButton lnkShowAll = lnkShowAllTop;

				if (Settings["ShowAllLinkPos"] != null && Convert.ToInt32(Settings["ShowAllLinkPos"]) > 0 &&
					_products.Count > 0 && _products.Count > ProductsPerPage)
				{
					int pos = Convert.ToInt32(Settings["ShowAllLinkPos"]);
					switch (pos)
					{
						case 1:
							lnkShowAllTop.Visible = true;
							lnkShowAll = lnkShowAllTop;
							divShowAllTop.Style.Add("text-align","left");
							break;
						case 2:
							lnkShowAllTop.Visible = true;
							lnkShowAll = lnkShowAllTop;
							divShowAllTop.Style.Add("text-align", "center");
							break;
						case 3:
							lnkShowAllTop.Visible = true;
							lnkShowAll = lnkShowAllTop;
							divShowAllTop.Style.Add("text-align", "right");
							break;
						case 4:
							lnkShowAllBottom.Visible = true;
							lnkShowAll = lnkShowAllBottom;
							divShowAllBottom.Style.Add("text-align", "left");
							break;
						case 5:
							lnkShowAllBottom.Visible = true;
							lnkShowAll = lnkShowAllBottom;
							divShowAllBottom.Style.Add("text-align", "center");
							break;
						case 6:
							lnkShowAllBottom.Visible = true;
							lnkShowAll = lnkShowAllBottom;
							divShowAllBottom.Style.Add("text-align", "right");
							break;
					}
					lnkShowAll.Text = string.Format(Localization.GetString("ShowAllLink.Text", this.LocalResourceFile), Products.Count);
					lnkShowAll.CssClass = "bbstore-productlist-showall";
				}
			}
			else
			{
				pnlListHead.Visible = false;
				pnlListFooter.Visible = false;
			}

            if (Convert.ToBoolean(Settings["HideEmptyModule"] ?? "false") == true && _products.Any() == false && !IsEditable)
                this.ContainerControl.Visible = false;

            // Check licensing
            LicenseDataInfo license = Controller.GetLicense(PortalId, false);
            Controller.CheckLicense(license, this, ModuleKind);
		}

        protected override void Render(HtmlTextWriter writer)
		{
			foreach (ListViewDataItem dataItem in lstProducts.Items)
			{
				if (dataItem.ItemType == ListViewItemType.DataItem)
				{
					Button btn = dataItem.FindControl("cmdSelect") as Button;
					string script = Page.ClientScript.GetPostBackClientHyperlink(btn, "", true);
					Panel p = dataItem.FindControl("pnlItem") as Panel;
					p.Attributes.Add("onclick", script);
				}
			}
			base.Render(writer);
		}

		protected void Pager_PreRender(object sender, EventArgs e)
		{
			lstProducts.DataSource = Products;
			lstProducts.DataBind();
		}

		protected void ddlSort_SelectedIndexChanged(object sender, EventArgs e)
		{
			DropDownList ddl = sender as DropDownList;
			lstProducts.Sort(ddl.SelectedValue,SortDirection.Ascending);
			Pager.SetPageProperties(0, Pager.PageSize, true);
            Response.Redirect(Globals.NavigateURL(TabId));
		}
		
		protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
		{
			DropDownList ddl = sender as DropDownList;
			int size = 25;
			if (Int32.TryParse(ddlPageSize.SelectedValue, out size))
			{
				ProductsPerPage = size;
				Pager.PageSize = size;
				lstProducts.Sort(ddlSortBox.SelectedValue, SortDirection.Ascending);
				Pager.SetPageProperties(0, Pager.PageSize, true);
			}
		}

		protected void lstProducts_ItemCreated(object sender, ListViewItemEventArgs e)
		{

			if (IsConfigured)
			{
				string Template;
				int imageWidth = 200;
				ListView lv = sender as ListView;
				ListViewDataItem item = e.Item as ListViewDataItem;
				SimpleProduct = item.DataItem as SimpleProductInfo;
				if (SimpleProduct != null)
				{
					Panel pnl = e.Item.FindControl("pnlItem") as Panel;
					pnl.Attributes["onmouseover"] = "this.style.cursor='pointer';";
					
					PlaceHolder ph = e.Item.FindControl("productPlaceholder") as PlaceHolder;

				    TemplateControl tp = LoadControl("controls/TemplateControl.ascx") as TemplateControl;
				    tp.Key = "ProductList";
                    Template = tp.GetTemplate((string)Settings["Template"]);
					
					//Template = ProductTemplate;
					Template = Template.Replace("[ITEMNO]", "<asp:Label ID=\"lblItemNo\" runat=\"server\" />");
					Template = Template.Replace("[PRODUCTSHORTDESCRIPTION]", "<asp:Label ID=\"lblShortDescription\" runat=\"server\" />");
					Template = Template.Replace("[PRODUCTDESCRIPTION]", "<asp:Literal ID=\"ltrProductDescription\" Mode=\"PassThrough\" runat=\"server\" />");
					Template = Template.Replace("[PRICE]", "<asp:Label ID=\"lblPrice\" runat=\"server\" />");
					Template = Template.Replace("[ORIGINALPRICE]", "<asp:Label ID=\"lblOriginalPrice\" runat=\"server\" />");
				    
                    int linkCnt = 0;
				    while (Template.Contains("[LINK]"))
				    {
				        linkCnt ++;
                        Template = Template.ReplaceFirst("[LINK]", "<asp:Literal ID=\"ltrLink" + linkCnt.ToString() + "\" runat=\"server\" />");
				    }
                    
					if (Template.IndexOf("[IMAGE") > -1)
					{
						if (Template.IndexOf("[IMAGE:") > -1)
						{
							string width = Template.Substring(Template.IndexOf("[IMAGE:") + 7);
							width = width.Substring(0, width.IndexOf("]"));
							if (Int32.TryParse(width, out imageWidth) == false)
								imageWidth = 200;
							Template = Template.Replace("[IMAGE:" + width + "]", "<asp:PlaceHolder ID=\"phimgProduct\" runat=\"server\" />");
						}
						else
							Template = Template.Replace("[IMAGE]", "<asp:PlaceHolder ID=\"phimgProduct\" runat=\"server\" />");
					}
					while (Template.IndexOf("[RESOURCE:") > -1)
					{
						string resKey = Template.Substring(Template.IndexOf("[RESOURCE:") + 10);
						resKey = resKey.Substring(0, resKey.IndexOf("]"));
						Template = Template.Replace("[RESOURCE:"+resKey+"]",
							Localization.GetString(resKey,this.LocalResourceFile));
					}

					if (Template.IndexOf("[FEATURE:") > -1)
					{
						while (Template.IndexOf("[FEATURE:") > -1)
						{
							string token = Template.Substring(Template.IndexOf("[FEATURE:") + 9);
							token = token.Substring(0, token.IndexOf("]"));
							string prop = token.Substring(token.IndexOf(".") + 1);
							token = token.Substring(0, token.IndexOf(".")).ToUpper();

							string value = "";
							FeatureGridValueInfo fgv = Controller.GetFeatureGridValueByProductAndToken(PortalId, SimpleProduct.SimpleProductId, CurrentLanguage, token);
							if (fgv != null)
							{
								PropertyInfo p = fgv.GetType().GetProperty(prop);
								if (p != null && p.CanRead)
									value = p.GetValue(fgv, null).ToString();
							}
							Template = Template.Replace("[FEATURE:" + token + "." + prop + "]", value);
						}
					}
					Template = Template.Replace("[CURRENCY]", "<asp:Label ID=\"lblCurrency\" runat=\"server\" />");
					Template = Template.Replace("[TAX]", "<asp:Label ID=\"lblTax\" runat=\"server\"/>");
                    Template = Template.Replace("[UNIT]", "<asp:Label ID=\"lblUnit\" runat=\"server\"/>");
					Template = Template.Replace("[TITLE]", "<asp:Label ID=\"lblTitle\" runat=\"server\" />");
				    Template = Template.Replace("[IMAGEURL]", SimpleProduct.Image);

					Control ctrl = ParseControl(Template);
					lblItemNo = FindControlRecursive(ctrl, "lblItemNo") as Label;
					if (lblItemNo != null)
						lblItemNo.Text = SimpleProduct.ItemNo;
					lblShortDescription = FindControlRecursive(ctrl, "lblShortDescription") as Label;
					if (lblShortDescription != null)
						lblShortDescription.Text = SimpleProduct.ShortDescription;
					ltrProductDescription = FindControlRecursive(ctrl, "ltrProductDescription") as Literal;
					if (ltrProductDescription != null)
						ltrProductDescription.Text = SimpleProduct.ProductDescription;

                    lblCurrency = FindControlRecursive(ctrl, "lblCurrency") as Label;
                    lblMandatory = FindControlRecursive(ctrl, "lblMandatory") as Label;
					lblPrice = FindControlRecursive(ctrl, "lblPrice") as Label;
					lblOriginalPrice = FindControlRecursive(ctrl, "lblOriginalPrice") as Label;
					lblTax = FindControlRecursive(ctrl, "lblTax") as Label;
                    lblUnit = FindControlRecursive(ctrl, "lblUnit") as Label;


				    for (int i = 1; i < linkCnt+1; i++)
				    {
                        Literal ltrLink = FindControlRecursive(ctrl, "ltrLink"+i.ToString()) as Literal;
                        if (ltrLink != null)
                        {
                            int productModuleTabId = Convert.ToInt32(Settings["ProductModulePage"] ?? TabId.ToString());
                            ltrLink.Text = Globals.NavigateURL(productModuleTabId, "", "productid=" + SimpleProduct.SimpleProductId.ToString());
                        }
				    }
                    
 
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
					if (lblTitle != null)
						lblTitle.Text = SimpleProduct.Name;

					bool showNetPrice = (StoreSettings.Count > 0) && ((string) StoreSettings["ShowNetpriceInCart"] == "0");

					decimal unitCost = SimpleProduct.UnitCost;
					decimal originalUnitCost = SimpleProduct.OriginalUnitCost;
					decimal taxPercent = SimpleProduct.TaxPercent;

					decimal price = 0.00m;
					decimal originalPrice = 0.00m;
					string tax = "";

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
					{
						if (SimpleProduct.HideCost)
							lblPrice.Text = Localization.GetString("AskOffer.Text", this.LocalResourceFile);
						else
							lblPrice.Text = String.Format("{0:n2}", price);
					}

					if (lblOriginalPrice != null)
					{
						if (originalPrice > price && !SimpleProduct.HideCost)
							lblOriginalPrice.Text = String.Format("{0:n2} {1}", originalPrice, Currency);
						else
							lblOriginalPrice.Visible = false;
					}

					if (lblTax != null)
					{
						lblTax.Text = String.Format(tax, taxPercent);
						lblTax.Visible = (taxPercent > 0.00m && SimpleProduct.HideCost == false);
					}

                    if (lblUnit != null && SimpleProduct.UnitId > -1)
                    {
                        UnitInfo unit = Controller.GetUnit(SimpleProduct.UnitId, CurrentLanguage);
                        lblUnit.Text = unit.Symbol;
                    }

					if (lblCurrency != null)
					{
						lblCurrency.Text = Currency;
						lblCurrency.Visible = (SimpleProduct.HideCost == false);
					}
					ph.Controls.Add(ctrl);
				}
			}
			else
			{
				Pager.Visible = false;
				lstProducts.Visible = false;
			}

		}
		protected void lstProducts_SelectedIndexChanging(object sender, ListViewSelectEventArgs e)
		{
			LinkButton btn = sender as LinkButton;

            int TabId = Convert.ToInt32(Settings["ProductModulePage"]);
			int productId = (int)lstProducts.DataKeys[e.NewSelectedIndex].Value;

			List<string> param = new List<string>();
			if (Request["productgroup"] != null)
				param.Add("productgroup="+Request["productgroup"]);
			param.Add("productid="+productId.ToString());

			Response.Redirect(Globals.NavigateURL(TabId, "", param.ToArray()));
		}
		protected void lstProducts_Sorting(object sender, ListViewSortEventArgs e)
		{
			SortExpression = e.SortExpression;
			lstProducts.DataSource = Products;
			lstProducts.DataBind();
		}
		protected void lnkShowAll_Click(object sender, EventArgs e)
		{
			if (Settings["StaticFilterId"] != null && Convert.ToInt32(Settings["StaticFilterId"]) > -1)
			{
				Controller.DeleteProductFilters(PortalId, FilterSessionId);
				ProductFilterInfo fi = new ProductFilterInfo();
				fi.FilterSessionId = FilterSessionId;
				fi.FilterSource = "StaticSearch";
				fi.FilterValue = (string)Settings["StaticFilterId"];
				fi.PortalId = PortalId;
				Controller.NewProductFilter(fi);
			}
			int TabId = Convert.ToInt32(Settings["ProductListModulePage"]);
			Response.Redirect(Globals.NavigateURL(TabId));
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
		private string GetProperty(string Name, object Obj)
		{
			string retval = "";
			PropertyInfo p = Obj.GetType().GetProperty(Name);
			if (p != null && p.CanRead)
				retval =  p.GetValue(Obj, null).ToString();
			return retval;
		}
		protected string GetWidth()
		{
			return (100 / lstProducts.GroupItemCount).ToString();
		}
		#endregion

	 }
}