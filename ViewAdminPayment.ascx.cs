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
using System.IO;
using System.Resources;
using System.Web.UI;
using System.Web.UI.WebControls;
using Bitboxx.DNNModules.BBStore.Providers.Payment;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.UserControls;

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
    partial class ViewAdminPayment : PortalModuleBase
    {
        #region Private Members

        private const string Currency = "EUR";
        private BBStoreController _controller;

        #endregion

        #region Public Properties

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

        #endregion

        #region Event Handlers

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

        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                ShowPaymentProviders();
            }
            catch (Exception exc)
            {
                //Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdUpdate_Click(object sender, EventArgs e)
        {
            UpdatePaymentProvider();
            Response.Redirect(Globals.NavigateURL(), true);
        }

        protected void cmdCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(Globals.NavigateURL(), true);
        }

        #endregion

        #region Helper Methods
        private void ShowPaymentProviders()
        {
            // Payment Providers
            List<PaymentProviderInfo> lst = Controller.GetPaymentProviders(CurrentLanguage);
            if (lst.Count == 0)
                lst = Controller.GetPaymentProviders(DefaultLanguage);
            if (lst.Count > 0)
            {
                
                // Lets populate thbe Usergroups into Collection
                RoleController RoleController = new RoleController();
                ArrayList colArrayList = RoleController.GetPortalRoles(PortalId);

                // Create a ListItemCollection to hold the Roles 
                ListItemCollection colRoles = new ListItemCollection();

                // Add all defined Roles to the List 
                foreach (RoleInfo Role in colArrayList)
                {
                    ListItem RoleListItem = new ListItem();
                    RoleListItem.Text = Role.RoleName;
                    RoleListItem.Value = Role.RoleID.ToString();
                    colRoles.Add(RoleListItem);
                }
                ListItem SelectRoleListItem = new ListItem();
                SelectRoleListItem.Text = Localization.GetString("SelectRole.Text", this.LocalResourceFile);
                SelectRoleListItem.Value = "-1";
                colRoles.Insert(0, SelectRoleListItem);

                // And now we need the Payment Provider for this subscriber (Portal)
                List<SubscriberPaymentProviderInfo> lstSub = Controller.GetSubscriberPaymentProviders(PortalId, 0);

                Table myTable = new Table();
                myTable.Attributes.Add("class","dnnGrid");
                myTable.ID = "tblPaymentProvider";

                // First the header row
                TableHeaderRow tr = new TableHeaderRow();
                TableHeaderCell th = new TableHeaderCell();
                th.Attributes.Add("class","dnnGridHeader");

                LabelControl dnnLbl = LoadControl("~/controls/LabelControl.ascx") as LabelControl;
                dnnLbl.HelpText = Localization.GetString("ppEnabled.Help", this.LocalResourceFile);
                dnnLbl.Text = Localization.GetString("ppEnabled.Header", this.LocalResourceFile);
                th.Controls.Add(dnnLbl);
                tr.Controls.Add(th);

                th = new TableHeaderCell();
                th.Attributes.Add("class", "dnnGridHeader");
                dnnLbl = LoadControl("~/controls/LabelControl.ascx") as LabelControl;
                dnnLbl.Text = Localization.GetString("Vieworder.Header", this.LocalResourceFile);
                dnnLbl.HelpText = Localization.GetString("Vieworder.Help", this.LocalResourceFile);
                th.Controls.Add(dnnLbl);
                tr.Controls.Add(th);

                th = new TableHeaderCell();
                th.Attributes.Add("class", "dnnGridHeader");
                dnnLbl = LoadControl("~/controls/LabelControl.ascx") as LabelControl;
                dnnLbl.Text = Localization.GetString("Paymentprovider.Header", this.LocalResourceFile);
                dnnLbl.HelpText = Localization.GetString("Paymentprovider.Help", this.LocalResourceFile);
                th.Controls.Add(dnnLbl);
                tr.Controls.Add(th);

                th = new TableHeaderCell();
                th.Attributes.Add("class", "dnnGridHeader");
                dnnLbl = LoadControl("~/controls/LabelControl.ascx") as LabelControl;
                dnnLbl.Text = Localization.GetString("Price.Header", this.LocalResourceFile);
                dnnLbl.HelpText = Localization.GetString("Price.Help", this.LocalResourceFile);
                th.Controls.Add(dnnLbl);
                tr.Controls.Add(th);

                th = new TableHeaderCell();
                th.Attributes.Add("class", "dnnGridHeader");
                dnnLbl = LoadControl("~/controls/LabelControl.ascx") as LabelControl;
                dnnLbl.Text = Localization.GetString("Tax.Header", this.LocalResourceFile);
                dnnLbl.HelpText = Localization.GetString("Tax.Help", this.LocalResourceFile);
                th.Controls.Add(dnnLbl);
                tr.Controls.Add(th);

                th = new TableHeaderCell();
                th.Attributes.Add("class", "dnnGridHeader");
                dnnLbl = LoadControl("~/controls/LabelControl.ascx") as LabelControl;
                dnnLbl.Text = Localization.GetString("Userrole.Header", this.LocalResourceFile);
                dnnLbl.HelpText = Localization.GetString("Userrole.Help", this.LocalResourceFile);
                th.Controls.Add(dnnLbl);
                tr.Controls.Add(th);

                myTable.Controls.Add(tr);

                // and next the Provider Rows
                TableRow myRow;
                TableCell tblCell;
                int i = 0;
                foreach (PaymentProviderInfo pp in lst)
                {
                    // Skip if no Control defined
                    if (pp.ProviderControl.Trim() == String.Empty)
                        continue;
                    
                    i++;

                    //Is this Provider in the List for the Subscriber (Portal) ?
                    SubscriberPaymentProviderInfo Sub = lstSub.Find(sub => sub.PaymentProviderId == pp.PaymentProviderId);

                    bool isChecked = (Sub != null && Sub.IsEnabled);
                    string itemStyle = (i%2 == 1 ? "dnnGridItem" : "dnnGridAltItem");
                    
                    myRow = new TableRow();

                    // The Checkbox cell (first Column)
                    tblCell = new TableCell();
                    tblCell.Attributes.Add("class",itemStyle);
                    CheckBox chk = new CheckBox();
                    chk.ID = "chkEnabled" + pp.PaymentProviderId.ToString();
                    chk.EnableViewState = true;
                    chk.Checked = isChecked;
                    tblCell.Controls.Add(chk);
                    myRow.Controls.Add(tblCell);

                    // The Vieworder cell
                    tblCell = new TableCell();
                    tblCell.Attributes.Add("class", itemStyle);
                    TextBox txt = new TextBox();
                    txt.ID = "txtViewOrder" + pp.PaymentProviderId.ToString();
                    txt.EnableViewState = true;
                    txt.Columns = 2;
                    if (Sub != null)
                        txt.Text = Sub.ViewOrder.ToString();
                    else
                        txt.Text = "0";
                    tblCell.Controls.Add(txt);
                    myRow.Controls.Add(tblCell);

                    // The PaymentProvider-Cell
                    tblCell = new TableCell();
                    tblCell.Attributes.Add("class", itemStyle);
                    
                    PaymentProviderBase ctrl = this.LoadControl(@"~\DesktopModules\BBStore\Providers\Payment\" + pp.ProviderControl.Trim() + ".ascx") as PaymentProviderBase;
                    ctrl.DisplayMode = ViewMode.Edit;
                    ctrl.Title = pp.ProviderName;
                    ctrl.EnableViewState = true;
                    if (Sub != null)
                    {
                        ctrl.Properties = Sub.PaymentProviderProperties;
                        ctrl.Cost = Sub.Cost;
                    }
                    ctrl.ID = "pp" + pp.PaymentProviderId.ToString();
                    tblCell.Controls.Add(ctrl);
                    
                    myRow.Controls.Add(tblCell);


                    // We need to create the taxpercent control first (needed by TaxControl)
                    TextBox txtTaxPercent = new TextBox();
                    txtTaxPercent.ID = "txtTaxPercent" + pp.PaymentProviderId.ToString();
                    txtTaxPercent.EnableViewState = true;
                    txtTaxPercent.Columns = 6;
                    if (Sub != null)
                        txtTaxPercent.Text = String.Format("{0:f1}", Sub.TaxPercent);
                    else
                        txtTaxPercent.Text = String.Format("{0:f2}", 0.00m);

                    // The Price cell
                    tblCell = new TableCell();
                    tblCell.Attributes.Add("class", itemStyle);

                    TaxControl txtPrice = LoadControl("Controls/TaxControl.ascx") as TaxControl;
                    txtPrice.ID = "txtPrice" + pp.PaymentProviderId.ToString();
                    txtPrice.EnableViewState = true;
                    txtPrice.Orientation = "vertical";
                    txtPrice.Mode = "gross";
                    txtPrice.PercentControl = txtTaxPercent;
                    if (Sub != null)
                        txtPrice.Value = Sub.Cost;
                    else
                        txtPrice.Value = 0.00m;
                    tblCell.Controls.Add(txtPrice);
                    myRow.Controls.Add(tblCell);

                    // The Tax cell
                    tblCell = new TableCell();
                    tblCell.Attributes.Add("class", itemStyle);
                    tblCell.Controls.Add(txtTaxPercent);
                    myRow.Controls.Add(tblCell);

                    // The RoleDropdown Column
                    tblCell = new TableCell();
                    tblCell.Attributes.Add("class", itemStyle);

                    DropDownList ddl = new DropDownList();
                    ddl.ID = "ddlUserRole" + pp.PaymentProviderId.ToString();
                    ddl.EnableViewState = true;
                    ddl.DataSource = colRoles;
                    ddl.DataBind();
                    if (Sub != null)
                        ddl.SelectedValue = Sub.Role.ToString();
                    tblCell.Controls.Add(ddl);

                    myRow.Controls.Add(tblCell);

                    myTable.Controls.Add(myRow);
                }
                phPayment.Controls.Add(myTable);

            }
            else
            {
                string message = Localization.GetString("NoPaymentprovider.Message", this.LocalResourceFile);
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, message, ModuleMessage.ModuleMessageType.YellowWarning);
            }

        }

        private void UpdatePaymentProvider()
        {
            List<SubscriberPaymentProviderInfo> lstSub = Controller.GetSubscriberPaymentProviders(PortalId, 0);
            List<PaymentProviderInfo> lst = Controller.GetPaymentProviders(CurrentLanguage);
            if (lst.Count == 0)
                lst = Controller.GetPaymentProviders(DefaultLanguage);
            if (lst.Count > 0)
            {
                foreach (PaymentProviderInfo pp in lst)
                {
                    Boolean IsChecked = false;
                    string PaymentProviderProperties = "";
                    decimal Cost = 0.00m;
                    decimal taxPercent = 0.00m;
                    int ViewOrder = 0;
                    string Role = "";

                    //Is this Provider in the List for the Subscriber (Portal) ?
                    SubscriberPaymentProviderInfo Sub = lstSub.Find(sub => sub.PaymentProviderId == pp.PaymentProviderId);

                    CheckBox chk = Globals.FindControlRecursiveDown(phPayment, "chkEnabled" + pp.PaymentProviderId.ToString()) as CheckBox;
                    if (chk != null)
                        IsChecked = chk.Checked;

                    TextBox txt = Globals.FindControlRecursiveDown(phPayment, "txtVieworder" + pp.PaymentProviderId.ToString()) as TextBox;
                    if (txt != null)
                        Int32.TryParse(txt.Text, out ViewOrder);

                    PaymentProviderBase ppb = Globals.FindControlRecursiveDown(phPayment, "pp" + pp.PaymentProviderId.ToString()) as PaymentProviderBase;
                    if (ppb != null)
                    {
                        PaymentProviderProperties = ppb.Properties;
                        // Cost = ppb.Cost;
                    }
                    TaxControl tax = Globals.FindControlRecursiveDown(phPayment, "txtPrice" + pp.PaymentProviderId.ToString()) as TaxControl;
                    if (tax != null)
                        Cost = tax.Value;
                        //decimal.TryParse(txt.Text, out Cost);

                    txt = Globals.FindControlRecursiveDown(phPayment, "txtTaxPercent" + pp.PaymentProviderId.ToString()) as TextBox;
                    if (txt != null)
                        decimal.TryParse(txt.Text, out taxPercent);

                    DropDownList ddl = Globals.FindControlRecursiveDown(phPayment, "ddlUserRole" + pp.PaymentProviderId.ToString()) as DropDownList;
                    if (ddl != null && ddl.SelectedIndex > 0)
                    {
                        Role = ddl.SelectedValue;
                    }

                    if (Sub == null)
                    {
                        Sub = new SubscriberPaymentProviderInfo();
                        Sub.PaymentProviderId = pp.PaymentProviderId;
                        Sub.PortalId = PortalId;
                        Sub.Cost = Cost;
                        Sub.SubscriberId = 0;
                        Sub.ViewOrder = ViewOrder;
                        Sub.IsEnabled = IsChecked;
                        Sub.Role = Role;
                        Sub.PaymentProviderProperties = PaymentProviderProperties;
                        Sub.TaxPercent = taxPercent;
                        Controller.NewSubscriberPaymentProvider(Sub);
                    }
                    else
                    {
                        Sub.PaymentProviderId = pp.PaymentProviderId;
                        Sub.PortalId = PortalId;
                        Sub.Cost = Cost;
                        Sub.SubscriberId = 0;
                        Sub.ViewOrder = ViewOrder;
                        Sub.IsEnabled = IsChecked;
                        Sub.Role = Role;
                        Sub.PaymentProviderProperties = PaymentProviderProperties;
                        Sub.TaxPercent = taxPercent;
                        Controller.UpdateSubscriberPaymentProvider(Sub);
                    }
                }
            }
        }

        #endregion
    }
}