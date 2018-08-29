// 
// DotNetNuke� - http://www.dotnetnuke.com 
// Copyright (c) 2002-2010 
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
using System.Data;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.UserControls;

namespace Bitboxx.DNNModules.BBStore
{

    /// ----------------------------------------------------------------------------- 
    /// <summary> 
    /// The EditSampleModule class is used to manage content 
    /// </summary> 
    /// <remarks> 
    /// </remarks> 
    /// <history> 
    /// </history> 
    /// ----------------------------------------------------------------------------- 
    [DNNtc.PackageProperties("BBStore Product")]
    [DNNtc.ModuleProperties("BBStore Product")]
    [DNNtc.ModuleControlProperties("EDIT", "BBStore Edit Simple Product", DNNtc.ControlType.Edit, "", false, false)]
    partial class EditProduct : PortalModuleBase
    {
        protected DotNetNuke.UI.UserControls.UrlControl ImageSelector;

        #region "Private Members"

        private bool HasProductGroupModule = true;
        private bool HasProductFeatureModule = true;
        private BBStoreController Controller;
        private string _imageDir = "";

        #endregion

        #region "Public Properties"

        public int ProductId
        {
            get
            {
                if (ViewState["ProductId"] != null)
                    return (int) ViewState["ProductId"];
                else
                    return -1;
            }
            set { ViewState["ProductId"] = value; }
        }

        protected string CurrentLanguage
        {
            get { return System.Threading.Thread.CurrentThread.CurrentCulture.Name; }
        }

        protected string DefaultLanguage
        {
            get { return this.PortalSettings.DefaultLanguage; }
        }

        public IEnumerable<ProductPriceInfo> ProductPrices
        {
            get
            {
                return Controller.GetProductPricesByProductId(ProductId);
            }
        }

        #endregion

        #region "Event Handlers"

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            string FileName = System.IO.Path.GetFileNameWithoutExtension(this.AppRelativeVirtualPath);
            if (this.ID != null)
                //this will fix it when its placed as a ChildUserControl 
                this.LocalResourceFile = this.LocalResourceFile.Replace(this.ID, FileName);
            else
                // this will fix it when its dynamically loaded using LoadControl method 
                this.LocalResourceFile = this.LocalResourceFile + FileName + ".ascx.resx";
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (grdPriceList.Rows.Count > 0)
            {
                foreach (GridViewRow row in grdPriceList.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(grdPriceList, "Select$" + row.RowIndex, true));
                    }
                }
            }
            base.Render(writer);
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                Controller = new BBStoreController();

                taxUnitCost.PercentControl = txtTaxPercent;
                taxOriginalUnitCost.PercentControl = txtTaxPercent;

                taxPriceUnitCost.PercentControl = txtPriceTaxPercent;
                taxPriceOriginalUnitCost.PercentControl = txtPriceTaxPercent;

                LocaleController lc = new LocaleController();
                Dictionary<string, Locale> loc = lc.GetLocales(PortalId);

                //TODO: Panels ausblenden wenn kein Modul verwendet


                ModuleController objModules = new ModuleController();
                if (Controller.GetModuleByName(PortalId, "BBStore Product Groups") == null)
                    HasProductGroupModule = false;

                if (Controller.GetModuleByName(PortalId, "BBStore Product Features") == null)
                    HasProductFeatureModule = false;

                Hashtable storeSettings = Controller.GetStoreSettings(PortalId);
                if (storeSettings != null)
                    _imageDir = (string) (storeSettings["ProductImageDir"] ?? "");

                // If this is the first visit to the page 
                if (Page.IsPostBack == false)
                {
                    // Show Supplier ?
                    if (storeSettings != null && storeSettings["SupplierRole"] != null && (string) storeSettings["SupplierRole"] != "-1")
                    {
                        pnlSupplier.Visible = true;
                        IList<UserInfo> aUsers = RoleController.Instance.GetUsersByRole(PortalId, (string)storeSettings["SupplierRole"]);
                        ListItemCollection users = new ListItemCollection();
                        foreach (UserInfo user in aUsers)
                        {
                            users.Add(new ListItem(user.DisplayName, user.UserID.ToString()));
                        }
                        string selText = Localization.GetString("SelectSupplier.Text", this.LocalResourceFile);
                        users.Insert(0, new ListItem(selText, "-1"));
                        cboSupplier.DataSource = users;
                        cboSupplier.DataValueField = "Value";
                        cboSupplier.DataTextField = "Text";
                        cboSupplier.DataBind();
                    }

                    // Shipping Models
                    List<ShippingModelInfo> shippingModels = Controller.GetShippingModels(PortalId);
                    
                    cboShippingModel.DataSource = shippingModels;
                    cboShippingModel.DataValueField = "ShippingModelId";
                    cboShippingModel.DataTextField = "Name";
                    cboShippingModel.DataBind();

                    string selUnitText = Localization.GetString("SelectUnit.Text", this.LocalResourceFile);
                    ddlUnit.Items.Add(new ListItem(selUnitText, "-1"));
                    foreach (UnitInfo unit in Controller.GetUnits(PortalId,CurrentLanguage,"Unit"))
                    {
                        ddlUnit.Items.Add(new ListItem(unit.Unit,unit.UnitId.ToString()));
                    }
                    ddlUnit.DataValueField = "Value";
                    ddlUnit.DataTextField = "Text";
                    ddlUnit.DataBind();
                    

                    // Set ProductGroups Visible / not Visible
                    //pnlProductGroup.Visible = HasProductGroupModule;

                    SimpleProductInfo SimpleProduct = null;

                    if (Request["productid"] != null)
                        ProductId = Convert.ToInt32(Request["productid"]);

                    // if product exists
                    if (ProductId > 0)
                        SimpleProduct = Controller.GetSimpleProductByProductId(PortalId, ProductId);

                    List<ILanguageEditorInfo> dbLangs = new List<ILanguageEditorInfo>();

                    if (SimpleProduct == null)
                    {
                        taxUnitCost.Value = 0.00m;
                        taxUnitCost.Mode = "gross";
                        txtTaxPercent.Text = 0.0m.ToString();
                        taxOriginalUnitCost.Value = 0.00m;
                        taxOriginalUnitCost.Mode = "gross";
                        ImageSelector.Url = _imageDir + "This_fileName-Should_not_3xist";
                        cboSupplier.SelectedValue = "-1";
                        dbLangs.Add(new SimpleProductLangInfo() { Language = CurrentLanguage });
                        lngSimpleProducts.Langs = dbLangs;
                        txtWeight.Text = 0.000m.ToString();
                    }
                    else
                    {
                        // Fill in the Language information
                        foreach (SimpleProductLangInfo simpleProductLang in Controller.GetSimpleProductLangs(SimpleProduct.SimpleProductId))
                        {
                            dbLangs.Add(simpleProductLang);
                        }
                        lngSimpleProducts.Langs = dbLangs;


                        // Set Image Info
                        int fileId = -1;
                        if (!String.IsNullOrEmpty(SimpleProduct.Image))
                        {
                            try
                            {
                                IFileInfo file = FileManager.Instance.GetFile(PortalId, SimpleProduct.Image);
                                if (file != null)
                                    fileId = file.FileId;
                            }
                            catch (Exception)
                            {
                                fileId = -1;
                            }
                        }
                        string imageUrl = "";
                        if (fileId > -1)
                            imageUrl = "FileID=" + fileId.ToString();
                        else
                            imageUrl = _imageDir + "This_fileName-Should_not_3xist";

                        // Set other fields
                        txtItemNo.Text = SimpleProduct.ItemNo;
                        txtTaxPercent.Text = SimpleProduct.TaxPercent.ToString();
                        taxUnitCost.Mode = "gross";
                        taxUnitCost.Value = SimpleProduct.UnitCost;
                        taxOriginalUnitCost.Mode = "gross";
                        taxOriginalUnitCost.Value = SimpleProduct.OriginalUnitCost;
                        chkDisabled.Checked = SimpleProduct.Disabled;
                        chkHideCost.Checked = SimpleProduct.HideCost;
                        chkNoCart.Checked = SimpleProduct.NoCart;
                        cboSupplier.SelectedValue = SimpleProduct.SupplierId.ToString();
                        ddlUnit.SelectedValue = SimpleProduct.UnitId.ToString();
                        ImageSelector.Url = imageUrl;
                        imgImage.ImageUrl = BBStoreHelper.FileNameToImgSrc(imageUrl, PortalSettings);
                        txtWeight.Text = SimpleProduct.Weight.ToString();

                        // Set ShippingModel
                        List<ProductShippingModelInfo> productshippingModels = Controller.GetProductShippingModelsByProduct(SimpleProduct.SimpleProductId);
                        if (productshippingModels.Count > 0)
                            cboShippingModel.SelectedValue = productshippingModels[0].ShippingModelId.ToString();
                    }

                    // Treeview Basenode
                    TreeNode newNode = new TreeNode(Localization.GetString("treeProductGroups.Text", this.LocalResourceFile), "_-1");
                    newNode.SelectAction = TreeNodeSelectAction.Expand;
                    newNode.PopulateOnDemand = true;
                    newNode.ImageUrl = @"~\images\category.gif";
                    newNode.ShowCheckBox = false;
                    treeProductGroup.Nodes.Add(newNode);
                    //newNode.Expanded = false;


                    // Product Price
                    Localization.LocalizeGridView(ref grdPriceList, LocalResourceFile);
                    grdPriceList.DataSource = ProductPrices;
                    grdPriceList.DataBind();

                    RoleController roleController1 = new RoleController();
                    IList<RoleInfo> aRoles = roleController1.GetRoles(PortalId);
                    ListItemCollection roles = new ListItemCollection();
                    foreach (RoleInfo role in aRoles)
                    {
                        roles.Add(new ListItem(role.RoleName, role.RoleID.ToString()));
                    }
                    string selText1 = Localization.GetString("SelectRole.Text", this.LocalResourceFile);
                    roles.Insert(0, new ListItem(selText1, "-1"));
                    ddlPriceRoleId.DataSource = roles;
                    ddlPriceRoleId.DataValueField = "Value";
                    ddlPriceRoleId.DataTextField = "Text";
                    ddlPriceRoleId.DataBind();

                }
                if (HasProductFeatureModule)
                {
                    FeatureGrid.ProductId = ProductId;
                }
            }

            catch (Exception exc)
            {
                //Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdCancel_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> addParams = new List<string>();
                if (!String.IsNullOrEmpty(Request.QueryString["adminmode"]))
                    addParams.Add("adminmode=productlist");

                if (!String.IsNullOrEmpty(Request.QueryString["productgroup"]))
                    addParams.Add("productgroup=" + Request.QueryString["productgroup"]);
                if (!String.IsNullOrEmpty(Request.QueryString["productid"]))
                    addParams.Add("productId=" + Request.QueryString["productid"]);

                if (addParams.Count > 0)
                    Response.Redirect(Globals.NavigateURL(TabId, "", addParams.ToArray()), true);
                else
                    Response.Redirect(Globals.NavigateURL(), true);
            }
            catch (Exception exc)
            {
                //Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                // First lets save the product
                SimpleProductInfo SimpleProduct = null;
                bool isNew = false;

                if (ProductId >= 0)
                    SimpleProduct = Controller.GetSimpleProductByProductId(PortalId, ProductId);
                else
                    isNew = true;

                if (SimpleProduct != null)
                {
                    SimpleProduct.Image = BBStoreHelper.GetRelativeFilePath(ImageSelector.Url);
                    SimpleProduct.ItemNo = txtItemNo.Text.Trim();
                    SimpleProduct.UnitCost = taxUnitCost.NetPrice;
                    SimpleProduct.OriginalUnitCost = taxOriginalUnitCost.NetPrice;
                    SimpleProduct.TaxPercent = Convert.ToDecimal(txtTaxPercent.Text.Trim());
                    SimpleProduct.LastModifiedByUserId = UserId;
                    SimpleProduct.LastModifiedOnDate = DateTime.Now;
                    SimpleProduct.Disabled = chkDisabled.Checked;
                    SimpleProduct.HideCost = chkHideCost.Checked;
                    SimpleProduct.NoCart = chkNoCart.Checked;
                    SimpleProduct.SupplierId = String.IsNullOrEmpty(cboSupplier.SelectedValue) ? -1 : Convert.ToInt32(cboSupplier.SelectedValue);
                    SimpleProduct.UnitId = String.IsNullOrEmpty(ddlUnit.SelectedValue) ? -1 : Convert.ToInt32(ddlUnit.SelectedValue);
                    SimpleProduct.Weight = Convert.ToDecimal(txtWeight.Text.Trim());
                    Controller.UpdateSimpleProduct(SimpleProduct);
                }
                else
                {
                    SimpleProduct = new SimpleProductInfo();
                    SimpleProduct.PortalId = PortalId;
                    SimpleProduct.Image = BBStoreHelper.GetRelativeFilePath(ImageSelector.Url);
                    SimpleProduct.ItemNo = txtItemNo.Text.Trim();
                    SimpleProduct.UnitCost = taxUnitCost.NetPrice;
                    SimpleProduct.OriginalUnitCost = taxOriginalUnitCost.NetPrice;
                    SimpleProduct.TaxPercent = Convert.ToDecimal(txtTaxPercent.Text.Trim());
                    SimpleProduct.CreatedOnDate = DateTime.Now;
                    SimpleProduct.LastModifiedOnDate = DateTime.Now;
                    SimpleProduct.CreatedByUserId = UserId;
                    SimpleProduct.LastModifiedByUserId = UserId;
                    SimpleProduct.Disabled = chkDisabled.Checked;
                    SimpleProduct.HideCost = chkHideCost.Checked;
                    SimpleProduct.NoCart = chkNoCart.Checked;
                    SimpleProduct.SupplierId = String.IsNullOrEmpty(cboSupplier.SelectedValue) ? -1 : Convert.ToInt32(cboSupplier.SelectedValue);
                    SimpleProduct.UnitId = String.IsNullOrEmpty(ddlUnit.SelectedValue) ? -1 : Convert.ToInt32(ddlUnit.SelectedValue);
                    SimpleProduct.Weight = Convert.ToDecimal(txtWeight.Text.Trim());
                    ProductId = Controller.NewSimpleProduct(SimpleProduct);
                }

                // Lets update the ShippingModel
                Controller.DeleteProductShippingModelByProduct(ProductId);
                int shippingModelId = -1;
                if (cboShippingModel.SelectedValue != null && Int32.TryParse(cboShippingModel.SelectedValue,out shippingModelId))
                    Controller.InsertProductShippingModel(new ProductShippingModelInfo() {ShippingModelId = shippingModelId, SimpleProductId = ProductId});
                

                // Now lets update Language information
                lngSimpleProducts.UpdateLangs();
                Controller.DeleteSimpleProductLangs(ProductId);
                foreach (SimpleProductLangInfo si in lngSimpleProducts.Langs)
                {
                    si.SimpleProductId = ProductId;
                    Controller.NewSimpleProductLang(si);
                }

                // Lets handle the Product Groups
                int redirProductGroupId = 0;
                if (HasProductGroupModule)
                {

                    if (Request.QueryString["productgroup"] != null)
                        redirProductGroupId = Convert.ToInt32(Request.QueryString["productgroup"]);


                    Controller.DeleteProductInGroups(ProductId);
                    foreach (TreeNode node in treeProductGroup.CheckedNodes)
                    {
                        int ProductGroupId = Convert.ToInt32(node.Value.Substring(1));
                        Controller.NewProductInGroup(ProductId, ProductGroupId);
                        if (redirProductGroupId == 0)
                            redirProductGroupId = ProductGroupId;
                    }
                }

                // If we created a new product, we bound this as a fixed product to the module
                if (isNew && this.Parent.NamingContainer.ID != "ViewAdmin")
                {
                    ModuleController module = new ModuleController();
                    module.UpdateModuleSetting(ModuleId,"ProductId",ProductId.ToString());
                }
                FeatureGrid.SaveFeatures();

                List<string> addParams = new List<string>();

                if (Request["adminmode"] != null)
                    addParams.Add("adminmode=productlist");
                if (redirProductGroupId > 0)
                    addParams.Add("productgroup=" + redirProductGroupId.ToString());
                addParams.Add("productId=" + ProductId.ToString());

                Response.Redirect(Globals.NavigateURL(TabId, "", addParams.ToArray()), true);

            }
            catch (Exception exc)
            {
                //Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void treeProductGroup_TreeNodePopulate(object sender, TreeNodeEventArgs e)
        {
            // http://quickstarts.asp.net/QuickStartv20/aspnet/doc/ctrlref/navigation/treeview.aspx
            TreeNode parent = e.Node;
            int ProductGroupId;
            ProductGroupId = Convert.ToInt32(parent.Value.Substring(1));

            Controller = new BBStoreController();
            DataTable selectedProductGroups = Controller.GetProductsInGroupByProduct(ProductId);
            List<ProductGroupInfo> productGroups = Controller.GetProductSubGroupsByNode(PortalId, CurrentLanguage, ProductGroupId, false, false, true);
            foreach (ProductGroupInfo productGroup in productGroups)
            {
                TreeNode newNode = new TreeNode(productGroup.ProductGroupName, "_" + productGroup.ProductGroupId.ToString());
                DataRow[] zosn = selectedProductGroups.Select("ProductgroupId = " + productGroup.ProductGroupId.ToString());
                if (zosn.Length > 0)
                    newNode.Checked = true;
                newNode.SelectAction = TreeNodeSelectAction.Expand;
                newNode.PopulateOnDemand = true;
                newNode.Expand();
                parent.ChildNodes.Add(newNode);
            }
        }


        protected void treeProductGroup_TreeNodeCheckChanged(object sender, TreeNodeEventArgs e)
        {
            Controller.DeleteProductInGroups(ProductId);
            foreach (TreeNode node in treeProductGroup.CheckedNodes)
            {
                int ProductGroupId = Convert.ToInt32(node.Value.Substring(1));
                Controller.NewProductInGroup(ProductId, ProductGroupId);
            }
        }

        protected void cmdRefreshFeatures_Click(object sender, EventArgs e)
        {
            Controller.DeleteProductInGroups(ProductId);
            foreach (TreeNode node in treeProductGroup.CheckedNodes)
            {
                int ProductGroupId = Convert.ToInt32(node.Value.Substring(1));
                Controller.NewProductInGroup(ProductId, ProductGroupId);
            }
        }

        protected void imgRefreshImg_Click(object sender, EventArgs e)
        {
            imgImage.ImageUrl = BBStoreHelper.FileNameToImgSrc(ImageSelector.Url, PortalSettings);
        }

        #endregion

        protected void grdPriceList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                switch (e.CommandName)
                {
                    case "Select":
                        int index = Convert.ToInt32(e.CommandArgument);
                        int productPriceId = (int)grdPriceList.DataKeys[index].Value;

                        ProductPriceInfo price = Controller.GetProductPriceById(productPriceId);

                        hidProductPriceId.Value = productPriceId.ToString();
                        txtPriceTaxPercent.Text = price.TaxPercent.ToString();
                        taxPriceUnitCost.Value = price.UnitCost;
                        taxPriceUnitCost.Mode = "net";
                        taxPriceOriginalUnitCost.Value = price.OriginalUnitCost;
                        taxPriceOriginalUnitCost.Mode = "net";
                        dtpPriceStartdate.SelectedDate = price.Startdate;
                        dtpPriceEndDate.SelectedDate = price.EndDate;
                        ddlPriceRoleId.SelectedValue = price.RoleId.ToString();

                        pnlPriceEdit.Visible = true;
                        pnlPriceList.Visible = false;
                        break;

                    case "Insert":

                        hidProductPriceId.Value = "-1";
                        txtPriceTaxPercent.Text = 0.0m.ToString();
                        taxPriceUnitCost.Value = 0.00m; ;
                        taxPriceUnitCost.Mode = "net";
                        taxPriceOriginalUnitCost.Value = 0.00m;
                        taxPriceOriginalUnitCost.Mode = "net";
                        dtpPriceStartdate.SelectedDate = DateTime.Now;
                        dtpPriceEndDate.SelectedDate = null;
                        ddlPriceRoleId.SelectedValue = "-1";

                        pnlPriceEdit.Visible = true;
                        pnlPriceList.Visible = false;
                        break;
                }
            }
            catch (Exception exc)
            {
                //Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void grdPriceList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';";
            }
        }

        protected void grdPriceList_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                int m = e.Row.Cells.Count;
                for (int i = m - 1; i >= 1; i--)
                {
                    e.Row.Cells.RemoveAt(i);
                }
                e.Row.Cells[0].ColumnSpan = m;
            }
        }

        protected void grdPriceList_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var dataKey = grdPriceList.DataKeys[e.RowIndex];
            if (dataKey != null)
            {
                int productPriceId = (int)dataKey.Value;
                Controller.DeleteProductPrice(productPriceId);
            }
            grdPriceList.DataSource = ProductPrices;
            grdPriceList.DataBind();
        }

        protected void cmdCancelEditPrice_OnClick(object sender, EventArgs e)
        {
            pnlPriceList.Visible = true;
            pnlPriceEdit.Visible = false;
        }

        protected void cmdSaveEditPrice_OnClick(object sender, EventArgs e)
        {
            ProductPriceInfo price = new ProductPriceInfo();
            price.UnitCost = taxPriceUnitCost.Value;
            price.OriginalUnitCost = taxPriceOriginalUnitCost.Value;
            price.TaxPercent = Convert.ToDecimal(txtPriceTaxPercent.Text.Trim());
            price.Startdate = dtpPriceStartdate.SelectedDate;
            price.EndDate = dtpPriceEndDate.SelectedDate;
            price.RoleId = Convert.ToInt32(ddlPriceRoleId.SelectedValue);
            price.SimpleProductId = ProductId;

            int productPriceId = Convert.ToInt32(hidProductPriceId.Value);
            if (productPriceId < 0)
                Controller.NewProductPrice(price);
            else
            {
                price.ProductPriceId = productPriceId;
                Controller.UpdateProductPrice(price);
            }

            grdPriceList.DataSource = ProductPrices;
            grdPriceList.DataBind();

            pnlPriceList.Visible = true;
            pnlPriceEdit.Visible = false;
        }
    }
}
