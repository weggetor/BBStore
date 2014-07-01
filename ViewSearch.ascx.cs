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
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

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
    [DNNtc.PackageProperties("BBStore Product Search", 5, "BBStore Product Search", "BBStore Product Search", "BBStore.png", "Torsten Weggen", "bitboxx solutions", "http://www.bitboxx.net", "info@bitboxx.net", false)]
    [DNNtc.ModuleProperties("BBStore Product Search", "BBStore Search", 0)]
    [DNNtc.ModuleControlProperties("", "BBStore Product Search", DNNtc.ControlType.View, "", true, false)]
	partial class ViewSearch : PortalModuleBase, IActionable
	{
		#region Private Members
		private const string Currency = "EUR";
		BBStoreController _controller;
		private int _productGroupId = -1;
		#endregion

		#region Public Properties
		public Guid FilterSessionId
		{
			get
			{
				string filterSessionId;
				if (Request.Cookies["BBStoreFilterSessionId_"+ PortalId.ToString()] != null)
					filterSessionId = Request.Cookies["BBStoreFilterSessionId_" + PortalId.ToString()].Value;
				else
				{
					filterSessionId = Guid.NewGuid().ToString();
					HttpCookie keks = new HttpCookie("BBStoreFilterSessionId_" + PortalId.ToString());
					keks.Value = filterSessionId;
					keks.Expires = DateTime.Now.AddDays(1);
					Response.AppendCookie(keks);
				}
				return new Guid(filterSessionId);
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
	    public ModuleKindEnum ModuleKind
	    {
	        get { return ModuleKindEnum.Search; }
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
		protected string FilterValueText = "";
		protected string FilterValuePrice = "";
		protected int FilterValueStatic = -1;
		protected string FilterValueProductGroup = "";
		protected int DynamicPage
		{
			get
			{
				if (Settings["DynamicPage"] != null)
					return Convert.ToInt32(Settings["DynamicPage"]);
				else
					return TabId;
			}
		}
		#endregion

        
		#region Event Handlers
		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
			    bool showReset = false;
                lblCurrency.Text = Currency;
				
				// Search ProductGroup
				pnlSearchProductGroup.Visible = false;
				List<ProductFilterInfo> pgf = Controller.GetProductFilter(PortalId, FilterSessionId, "ProductGroup");
				if (pgf.Count > 0)
				{
				    showReset = true;
                    string[] values = pgf[0].FilterValue.Split('|');
					_productGroupId = Convert.ToInt32(values[0]);
				}

				if (Settings["ProductGroupSearchEnabled"] != null && Convert.ToBoolean(Settings["ProductGroupSearchEnabled"]))
				{
					ProductGroupInfo pg = Controller.GetProductGroup(PortalId, CurrentLanguage, _productGroupId);
					if (pg != null)
					{
						FilterValueProductGroup = pg.ProductGroupName;
						pnlSearchProductGroup.Visible = true;
					}
				}
				// Search FeatureList
				pnlSearchFeatureList.Visible = false;
				List<ProductFilterInfo> pfl = Controller.GetProductFilter(PortalId, FilterSessionId, "FeatureList");
				if (pfl.Count > 0)
				{
				    showReset = true;
                    string[] values = pfl[0].FilterValue.Split('|');

					int FeatureListId = Convert.ToInt32(values[0]);
					FeatureListInfo fli = Controller.GetFeatureListById(FeatureListId,CurrentLanguage);
					if (fli != null)
					{
						lblSearchFeatureListCap.Text = fli.FeatureList;
						int FeatureListItemId = Convert.ToInt32(values[1]);
						FeatureListItemLangInfo featureListItemLang = Controller.GetFeatureListItemLang(FeatureListItemId, CurrentLanguage);
						if (featureListItemLang != null)
						{
							lblSearchFeatureList.Text = featureListItemLang.FeatureListItem;
							pnlSearchFeatureList.Visible = true;
						}
					}

				}

				// Search for text
				if (Settings["TextSearchEnabled"] != null && Convert.ToBoolean(Settings["TextSearchEnabled"]))
				{
					List<ProductFilterInfo> fi = Controller.GetProductFilter(PortalId, FilterSessionId, "TextSearch");
					if (fi.Count == 0 || fi[0].FilterValue == String.Empty)
					{
						MultiViewText.ActiveViewIndex = 0;
						FilterValueText = (fi.Count == 0 ? "" : fi[0].FilterValue);
					}
					else
					{
					    showReset = true;
                        MultiViewText.ActiveViewIndex = 1;
						FilterValueText = fi[0].FilterValue;
					}
				}
				else
					pnlSearchText.Visible = false;

				// Static Search
				pnlSearchStatic.Visible = false;
				if (Settings["StaticSearchEnabled"] != null && Convert.ToBoolean(Settings["StaticSearchEnabled"]))
				{
					List<StaticFilterInfo> sf = Controller.GetStaticFilters(PortalId);
					if (sf.Count > 0)
					{
						List<ProductFilterInfo> fi = Controller.GetProductFilter(PortalId, FilterSessionId, "StaticSearch");
						if (fi.Count > 0 && fi[0].FilterValue != String.Empty)
						{
						    showReset = true;
                            MultiViewStatic.ActiveViewIndex = 1;
							FilterValueStatic = Convert.ToInt32(fi[0].FilterValue);
							StaticFilterInfo actFilter = sf.Find(x => x.StaticFilterId == FilterValueStatic);
							if (actFilter != null)
								lblSearchStatic.Text = actFilter.Token;
						}
						else
						{
							MultiViewStatic.ActiveViewIndex = 0;
							cboSearchStatic.DataSource = sf;
							cboSearchStatic.DataTextField = "Token";
							cboSearchStatic.DataValueField = "StaticFilterId";
							cboSearchStatic.Items.Add(new ListItem { Text = Localization.GetString("Select.Text",this.LocalResourceFile), Value = "0" });
							cboSearchStatic.AppendDataBoundItems = true;
							cboSearchStatic.DataBind();
						}
						pnlSearchStatic.Visible = true;
					}

				}

				// Search for Price
				if (Settings["PriceSearchEnabled"] != null && Convert.ToBoolean(Settings["PriceSearchEnabled"]))
				{
					List<ProductFilterInfo> fi = Controller.GetProductFilter(PortalId, FilterSessionId, "PriceSearch");
					if (fi.Count == 0 || fi[0].FilterValue == String.Empty)
					{
						MultiViewPrice.ActiveViewIndex = 0;
						FilterValuePrice = (fi.Count == 0 ? "" : fi[0].FilterValue);
					}
					else
					{
					    showReset = true;
                        MultiViewPrice.ActiveViewIndex = 1;
						FilterValuePrice = fi[0].FilterValue;
					}
				}
				else
					pnlSearchPrice.Visible = false;

				// Search for Features
				if (Settings["FeatureSearchEnabled"] != null && Convert.ToBoolean(Settings["FeatureSearchEnabled"]))
				{
                    List<ProductFilterInfo> fi = Controller.GetProductFilter(PortalId, FilterSessionId, "FeatureSearch");
                    if (fi.Count > 0 && fi[0].FilterValue != String.Empty)
					    showReset = true;
                    pnlFeatures.Visible = true;
					FeatureGrid.ProductGroupId = _productGroupId;
					FeatureGrid.FilterSessionId = FilterSessionId;
					FeatureGrid.SearchTabId = DynamicPage;
				}
				else
				    pnlFeatures.Visible = false;

			    pnlSearchReset.Visible = (Settings["ResetSearchEnabled"] != null && Convert.ToBoolean(Settings["ResetSearchEnabled"]) && showReset);

			}
			catch (Exception exc)
			{
				//Module failed to load 
				Exceptions.ProcessModuleLoadException(this, exc);
			}
		}

		protected void Page_PreRender(object sender, System.EventArgs e)
		{
           
            lblSearchProductGroup.Text = FilterValueProductGroup;
			string[] values = FilterValueText.Split('|');
			lblSearchText.Text = values[0];
			txtSearchText.Text = values[0];

			if (FilterValuePrice.Contains("|"))
			{
				values = FilterValuePrice.Split('|');
				decimal start = 0m;
				decimal end = 0m;
				if (Decimal.TryParse(values[0], NumberStyles.Number, CultureInfo.InvariantCulture, out start) &&
					Decimal.TryParse(values[1], NumberStyles.Number, CultureInfo.InvariantCulture, out end))
				{
					txtStartPrice.Text = start.ToString("F2", CultureInfo.CurrentUICulture);
					lblStartPrice.Text = txtStartPrice.Text;
					txtEndPrice.Text = end.ToString("F2", CultureInfo.CurrentUICulture);
					lblEndPrice.Text = txtEndPrice.Text;
				}
			}
            // Check licensing
            LicenseDataInfo license = Controller.GetLicense(PortalId, false);
            Controller.CheckLicense(license, this, ModuleKind);
		}

		protected void cmdSearchReset_Click(object sender, EventArgs e)
		{
			Controller.DeleteProductFilters(PortalId,FilterSessionId);
			if (Settings["ResetSearchPGEnabled"] == null || !Convert.ToBoolean(Settings["ResetSearchPGEnabled"]))
			{
				ProductFilterInfo fi = new ProductFilterInfo();
				fi.FilterSessionId = FilterSessionId;
				fi.FilterSource = "ProductGroup";
				fi.FilterValue = _productGroupId.ToString(CultureInfo.InvariantCulture);
				fi.PortalId = PortalId;
				Controller.NewProductFilter(fi);
			}			
			Response.Redirect(Globals.NavigateURL(DynamicPage));
		}
		protected void cmdDeleteProductGroup_Click(object sender, EventArgs e)
		{
			Controller.DeleteProductFilter(PortalId, FilterSessionId, "ProductGroup");
			Response.Redirect(Globals.NavigateURL(DynamicPage));
		}
		
		protected void cmdSearchText_Click(object sender, EventArgs e)
		{
			Controller.DeleteProductFilter(PortalId, FilterSessionId, "TextSearch");
			SetFilterText(txtSearchText.Text.Trim());
			Response.Redirect(Globals.NavigateURL(DynamicPage));
		}
		protected void cmdDeleteText_Click(object sender, EventArgs e)
		{
			Controller.DeleteProductFilter(PortalId, FilterSessionId, "TextSearch");
			Response.Redirect(Globals.NavigateURL(DynamicPage));
		}
		protected void cmdSearchStatic_Click(object sender, EventArgs e)
		{
			Controller.DeleteProductFilter(PortalId, FilterSessionId, "StaticSearch");
			SetFilterStatic(Convert.ToInt32(cboSearchStatic.SelectedValue));
			Response.Redirect(Globals.NavigateURL(DynamicPage));
		}
		protected void cmdDeleteStatic_Click(object sender, EventArgs e)
		{
			Controller.DeleteProductFilter(PortalId, FilterSessionId, "StaticSearch");
			Response.Redirect(Globals.NavigateURL(DynamicPage));
		}
		protected void cmdSearchPrice_Click(object sender, EventArgs e)
		{
			decimal start = 0m;
			decimal end = 0m;
			string startPrice = String.IsNullOrEmpty(txtStartPrice.Text) ? "0" : txtStartPrice.Text;
			string endPrice = String.IsNullOrEmpty(txtEndPrice.Text) ? "9999999" : txtEndPrice.Text;
			if (Decimal.TryParse(startPrice, NumberStyles.Number, CultureInfo.CurrentUICulture, out start) &&
				Decimal.TryParse(endPrice, NumberStyles.Number, CultureInfo.CurrentUICulture, out end))
			{
				Controller.DeleteProductFilter(PortalId, FilterSessionId, "PriceSearch");
				SetFilterPrice(start, end);
			}
			Response.Redirect(Globals.NavigateURL(DynamicPage));
		}

		protected void cmdDeletePrice_Click(object sender, EventArgs e)
		{
			Controller.DeleteProductFilter(PortalId, FilterSessionId, "PriceSearch");
			Response.Redirect(Globals.NavigateURL(DynamicPage));
		}

		protected void cmdDeleteFeatureList_Click(object sender, EventArgs e)
		{
			Controller.DeleteProductFilter(PortalId, FilterSessionId, "FeatureList");
			Response.Redirect(Globals.NavigateURL(DynamicPage));
		}

		
		#endregion

		#region Helper Methods
		private void SetFilterText(string SearchText)
		{
			if (SearchText != string.Empty)
			{
				ProductFilterInfo fi = new ProductFilterInfo();
				fi.FilterSessionId = FilterSessionId;
				fi.FilterSource = "TextSearch";
				fi.FilterValue = SearchText;
				fi.PortalId = PortalId;
				Controller.NewProductFilter(fi);
			}
		}
		private void SetFilterStatic(int StaticFilterId)
		{
			if (StaticFilterId > -1)
			{
				ProductFilterInfo fi = new ProductFilterInfo();
				fi.FilterSessionId = FilterSessionId;
				fi.FilterSource = "StaticSearch";
				fi.FilterValue = StaticFilterId.ToString();
				fi.PortalId = PortalId;
				Controller.NewProductFilter(fi);
			}
		}
		private void SetFilterPrice(decimal startPrice, decimal endPrice)
		{
			Controller.DeleteProductFilter(PortalId, FilterSessionId, "PriceSearch");
			if (startPrice > 0 || endPrice > 0)
			{
				Hashtable storeSettings = Controller.GetStoreSettings(PortalId);
				bool includeTax = true;
				if (storeSettings.Count > 0)
					includeTax = ((string)storeSettings["ShowNetpriceInCart"] == "1");
				ProductFilterInfo fi = new ProductFilterInfo();
				fi.FilterSessionId = FilterSessionId;
				fi.FilterSource = "PriceSearch";
				fi.FilterValue = startPrice.ToString(CultureInfo.InvariantCulture) + "|" + endPrice.ToString(CultureInfo.InvariantCulture)+"|"+includeTax.ToString();
				fi.PortalId = PortalId;
				Controller.NewProductFilter(fi);
			}
		}
		private string[] GetRedirectParams()
		{
			List<string> paraList = new List<string>();
			if (Request.QueryString["mod"] != null)
			{
				paraList.Add("mod");
				paraList.Add(Request.QueryString["mod"]);
			}
			if (Request.QueryString["productgroup"] != null)
			{
				paraList.Add("productgroup");
				paraList.Add(Request.QueryString["productgroup"]);
			}
			if (Request.QueryString["productid"] != null)
			{
				paraList.Add("productid");
				paraList.Add(Request.QueryString["productid"]);
			}
			if (Request.QueryString["includechilds"] != null)
			{
				paraList.Add("includechilds");
				paraList.Add(Request.QueryString["includechilds"]);
			}
			return paraList.ToArray();
		}
		private void AddValidators(TableCell Cell, string ControlID, ValidationDataType Type, string MinValue, string MaxValue, string RegEx, bool Required)
		{
			if (MinValue != null && MaxValue != null)
			{
				RangeValidator gvali = new RangeValidator();
				gvali.Type = ValidationDataType.Double;
				gvali.MinimumValue = Double.Parse(MinValue).ToString();
				gvali.MaximumValue = Double.Parse(MaxValue).ToString();
				gvali.ControlToValidate = ControlID;
				gvali.ErrorMessage = gvali.MinimumValue + ".." + gvali.MaximumValue;
				gvali.Attributes.Add("class", "searchValue");
				Cell.Controls.Add(new LiteralControl("&nbsp;"));
				Cell.Controls.Add(gvali);
			}
			else if (RegEx != null)
			{
				RegularExpressionValidator xvali = new RegularExpressionValidator();
				xvali.ValidationExpression = RegEx;
				xvali.ControlToValidate = ControlID;
				xvali.ErrorMessage = "Bitte gültiges Format angeben";
				xvali.Attributes.Add("class", "searchValue");
				Cell.Controls.Add(new LiteralControl("&nbsp;"));
				Cell.Controls.Add(xvali);
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