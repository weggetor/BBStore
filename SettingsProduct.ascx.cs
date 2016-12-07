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
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

namespace Bitboxx.DNNModules.BBStore
{

    /// ----------------------------------------------------------------------------- 
    /// <summary> 
    /// The Settings class manages Module Settings 
    /// </summary> 
    /// <remarks> 
    /// </remarks> 
    /// <history> 
    /// </history> 
    /// ----------------------------------------------------------------------------- 
    [DNNtc.PackageProperties("BBStore Product")]
    [DNNtc.ModuleProperties("BBStore Product")]
    [DNNtc.ModuleControlProperties("Settings", "BBStore Product Settings", DNNtc.ControlType.Edit, "", true, false)]
    partial class SettingsProduct : ModuleSettingsBase
    {
		BBStoreController _controller;
		int _pageIndex;

		public string Sort
		{
			get
			{
				if (ViewState["Sort"] != null)
					return ViewState["Sort"].ToString();
				else
					return "SimpleProductId";
			}
			set
			{
				ViewState["Sort"] = value;
			}
		}

		public string Where
		{
			get
			{
				if (ViewState["Search"] != null)
				{
					return _controller.GetSearchTextFilter(PortalId, (string)ViewState["Search"], CurrentLanguage);
				}
				else
					return "";
			}
		}
		public int ProductId
		{
			get
			{
				if (ViewState["ProductId"] != null)
					return (int)ViewState["ProductId"];
				else
					return -1;
			}
			set
			{
				ViewState["ProductId"] = value;
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


        #region "Base Method Implementations"
		protected override void OnInit(EventArgs e)
		{
			try
			{
				_controller = new BBStoreController();
				Localization.LocalizeGridView(ref GridView1, this.LocalResourceFile);

				if (!IsPostBack)
				{
					int prodid = 0;
                    if (Request["productid"] != null && Int32.TryParse(Request["ProductId"], out prodid))
                        ProductId = prodid;
                    else if (Settings["ProductId"] != null)
                        ProductId = Convert.ToInt32(Settings["ProductId"]);
                    else
                        ProductId = -1;

					// Fill the net / gross / see cart Optiongroup
					rblShowNetPrice.Items.Add(new ListItem(Localization.GetString("ShowNetPrice.Cart.Text", this.LocalResourceFile), "-1"));
					rblShowNetPrice.Items.Add(new ListItem(Localization.GetString("ShowNetPrice.Net.Text", this.LocalResourceFile), "0"));
					rblShowNetPrice.Items.Add(new ListItem(Localization.GetString("ShowNetPrice.Gross.Text", this.LocalResourceFile), "1"));

					_pageIndex = 0;
					Sort = "SimpleProductId";
					GridView1.PageIndex = _pageIndex;
				}
			    tplTemplate.CreateImageCallback = CreateThumbHtml;
			}
			catch (Exception exc)
			{
				//Module failed to load 
				Exceptions.ProcessModuleLoadException(this, exc);
			}
			base.OnLoad(e);
		}
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			List<SimpleProductInfo> products = _controller.GetSimpleProducts(PortalId, Thread.CurrentThread.CurrentCulture.Name, Sort, Where);
			GridView1.DataSource = products;
			GridView1.DataBind();

			if (products != null && products.Count > 0)
			{
				try
				{
                    if (ProductId == -1)
					{
					    lblSelected.Text = Localization.GetString("DynamicSelected.Message", this.LocalResourceFile);
					    rblSelectType.SelectedValue = "0";
					    pnlStatic.Visible = false;
                        divMessage.Attributes.Add("class", "dnnFormMessage dnnFormInfo");
					}
                    else if (ProductId == -2)
					{
					    lblSelected.Text = Localization.GetString("NoSelected.Message", this.LocalResourceFile);
                        pnlStatic.Visible = false;
                        divMessage.Attributes.Add("class","dnnFormMessage dnnFormWarning");
					}
                    else if (ProductId == -3)
                    {
                        lblSelected.Text = Localization.GetString("NoSelected.Message", this.LocalResourceFile);
                        pnlStatic.Visible = true;
                        rblSelectType.SelectedValue = "1";
                        divMessage.Attributes.Add("class", "dnnFormMessage dnnFormWarning");
                    }
					else
					{
						SimpleProductInfo pi = _controller.GetSimpleProductByProductId(PortalId, ProductId, CurrentLanguage);
					    if (pi != null)
					        lblSelected.Text = "(" + ProductId.ToString() + ") " + pi.ItemNo + " " + pi.Name;
					    else
					        lblSelected.Text = "(" + ProductId.ToString() + ")";

                        rblSelectType.SelectedValue = "1";
                        pnlStatic.Visible = true;
                        divMessage.Attributes.Add("class", "dnnFormMessage dnnFormInfo");
					}
				}
				catch (Exception exc)
				{
					//Module failed to load 
					Exceptions.ProcessModuleLoadException(this, exc);
				}
			}
			else
			{
				// pnlSelectProduct.Visible = false;
				lblSelected.Text = Localization.GetString("DynamicSelected.Message", this.LocalResourceFile);
			}

		}
        public override void LoadSettings()
        {
            if (ModuleSettings["ProductId"] != null)
                ProductId = Convert.ToInt32(ModuleSettings["ProductId"]);
            else
                ProductId = -1;

            if (ModuleSettings["ShowNetPrice"] != null)
                rblShowNetPrice.SelectedValue = (string)ModuleSettings["ShowNetPrice"];
            else
                rblShowNetPrice.SelectedValue = "-1";

            if (ModuleSettings["OpenCartOnAdd"] != null)
				chkOpenCartOnAdd.Checked = Convert.ToBoolean(ModuleSettings["OpenCartOnAdd"]);
			else
				chkOpenCartOnAdd.Checked = true;

            if (ModuleSettings["Template"] != null)
                tplTemplate.Value = (string) ModuleSettings["Template"];
            
            if (ModuleSettings["ContactModulePage"] != null)
				urlContactModulePage.Url = (string)ModuleSettings["ContactModulePage"];

            if (ModuleSettings["ListModulePage"] != null)
                urlListModulePage.Url = (string)ModuleSettings["ListModulePage"];
        }
        public override void UpdateSettings()
        {
			try
			{
				ModuleController objModules = new ModuleController();
				objModules.UpdateModuleSetting(ModuleId, "OpenCartOnAdd", chkOpenCartOnAdd.Checked.ToString());
				objModules.UpdateModuleSetting(ModuleId, "ContactModulePage", urlContactModulePage.Url);
                objModules.UpdateModuleSetting(ModuleId, "ListModulePage", urlListModulePage.Url);
                objModules.UpdateModuleSetting(ModuleId, "ProductId", ProductId.ToString());
                int isTaxIncluded = (string.IsNullOrEmpty(rblShowNetPrice.SelectedValue) ? 1 : Convert.ToInt32(rblShowNetPrice.SelectedValue));
                objModules.UpdateModuleSetting(ModuleId, "ShowNetPrice" ,isTaxIncluded.ToString() );
	            objModules.UpdateModuleSetting(ModuleId, "Template", tplTemplate.Value);
			}
			catch (Exception exc)
			{
				//Module failed to load 
				Exceptions.ProcessModuleLoadException(this, exc);
			}
        }

		protected void txtSearch_TextChanged(object sender, EventArgs e)
		{
			ViewState["Search"] = txtSearch.Text;
		}

        protected void rblSelectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (rblSelectType.SelectedValue)
            {
                case "0":
                    ProductId = -1;
                    pnlStatic.Visible = false;
                    break;
                case "1":
                    ProductId = -3;
                    pnlStatic.Visible = true;
                    break;
            }
        }

		protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			try
			{
				if (e.CommandName == "Select")
				{
					int index = Convert.ToInt32(e.CommandArgument);
					int SimpleProductId = (int)GridView1.DataKeys[index].Values["SimpleProductId"];
					ProductId = SimpleProductId;
				}
                //else if (e.CommandName == "Delete")
                //{
                //    int index = Convert.ToInt32(e.CommandArgument);
                //    int SimpleProductId = (int)GridView1.DataKeys[index].Values["SimpleProductId"];
                //    Controller.DeleteSimpleProduct(SimpleProductId);
                //    Response.Redirect(EditUrl("Select"), true);
                //}
			}
			catch (Exception exc)
			{
				//Module failed to load 
				Exceptions.ProcessModuleLoadException(this, exc);
			}
		}
		protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
		{
			Sort = e.SortExpression;
			_pageIndex = 0;
			GridView1.PageIndex = _pageIndex;
			GridView1.DataSource = _controller.GetSimpleProducts(PortalId, Thread.CurrentThread.CurrentCulture.Name, Sort);
			GridView1.DataBind();
		}
		protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
		{
			_pageIndex = e.NewPageIndex;
			GridView1.PageIndex = _pageIndex;
			GridView1.DataSource = _controller.GetSimpleProducts(PortalId, Thread.CurrentThread.CurrentCulture.Name, Sort);
			GridView1.DataBind();
		}

        #endregion

		#region Helper methods
		private string CreateThumbHtml(string template)
		{
			StringBuilder sb = new StringBuilder(template);
		    string imageUrl = Request.Url.Scheme + "://" + Request.Url.Host + "/dnnimagehandler.ashx?mode=placeholder&nocache=1";
            if (template.IndexOf("[IMAGE:") > -1)
            {
                string imageDimText = VfpInterop.StrExtract(sb.ToString(), "[IMAGE:", "]", 1, 1);
                if (imageDimText != String.Empty)
                {

                    int imageDim = 0;
                    if (Int32.TryParse(imageDimText, out imageDim))
                        imageUrl += string.Format("&w={0}&h={1}&text={0}", imageDim, (int) (imageDim*2/3));
                    
                    sb.Replace("[IMAGE:" + imageDimText + "]", "<img src=\"" + imageUrl + "\" />");
                }
            }
            else if (template.IndexOf("[IMAGE]") > -1)
            {
                imageUrl += "&w=200&h=150&text=Unresized+Image";
                sb.Replace("[IMAGE]", "<img src=\"" + imageUrl + "\" />");
            }
			sb.Replace("[IMAGELINK]", imageUrl + "&w=300&h=300&text=Sample+Image");
            sb.Replace("[TITLE]", "Product Title");
			sb.Replace("[PRODUCTSHORTDESCRIPTION]", "Product short description explains in 80 chars");
			sb.Replace("[PRODUCTDESCRIPTION]", "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum .");
			sb.Replace("[PRODUCTOPTIONS]", "<span>ProductOption</span>&nbsp;<select name=\"Select1\"><option>Option1</option></select>");
			sb.Replace("[MANDATORYERROR]", "Error Message");
			sb.Replace("[PRICE]", "123.44");
			sb.Replace("[CURRENCY]", PortalSettings.Currency);
			sb.Replace("[ADDCARTIMAGE]", "<img src=\"file:///" + Server.MapPath("~/images/cart.gif") + "\" />");
			sb.Replace("[ADDCARTLINK]", "Add to cart");
			sb.Replace("[TAX]", "includes tax (19%)");
            sb.Replace("[UNIT]", "pcs.");
			sb.Replace("[AMOUNT]", "<input name=\"Text1\" type=\"text\" size=\"3\" value=\"1\"/>");
		    return sb.ToString();
		}

		#endregion
    }
}

