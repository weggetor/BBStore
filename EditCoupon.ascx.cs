// 
// DotNetNuke® - http://www.dotnetnuke.com 
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
using DotNetNuke.Web.UI.WebControls;

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
    partial class EditCoupon : PortalModuleBase
    {

        #region "Private Members"

        private BBStoreController Controller;

        #endregion

        #region "Public Properties"

        public int CouponId
        {
            get
            {
                if (ViewState["CouponId"] != null)
                    return (int)ViewState["CouponId"];
                else
                    return -1;
            }
            set { ViewState["CouponId"] = value; }
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

        #region "Event Handlers"

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            
            taxDiscountValue.PercentControl = txtTaxPercent;

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
                Controller = new BBStoreController();

                // If this is the first visit to the page 
                if (Page.IsPostBack == false)
                {
                    CouponInfo coupon = null;

                    if (Request["couponid"] != null)
                        CouponId = Convert.ToInt32(Request["couponid"]);

                    // if coupon exists
                    if (CouponId > 0)
                        coupon = Controller.GetCouponById(CouponId);

                    //TODO: Weitere Felder
                    taxDiscountValue.Mode = "gross";
                    if (coupon == null)
                    {
                        txtCaption.Text = "";
                        txtCode.Text = "";
                        txtTaxPercent.Text = 0.0m.ToString();
                        txtDiscountPercent.Text = "";
                        taxDiscountValue.Value = 0.0m;
                        txtMaxUsages.Text = "1";
                        txtUsagesLeft.Text = "1";
                        txtValidUntil.Text = DateTime.Now.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        txtCaption.Text = coupon.Caption;
                        txtCode.Text = coupon.Code;
                        txtTaxPercent.Text = coupon.TaxPercent.ToString();
                        txtDiscountPercent.Text = coupon.DiscountPercent.ToString();
                        taxDiscountValue.Value = (coupon.DiscountValue == null ? 0 : (decimal)coupon.DiscountValue);
                        txtMaxUsages.Text = coupon.MaxUsages.ToString();
                        txtUsagesLeft.Text = coupon.UsagesLeft.ToString();
                        if (coupon.ValidUntil != null)
                            txtValidUntil.Text = ((DateTime) coupon.ValidUntil).ToString("yyyy-MM-dd");
                        else
                            txtValidUntil.Text = "";
                    }
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
                    addParams.Add("adminmode=couponlist");

                if (!String.IsNullOrEmpty(Request.QueryString["couponid"]))
                    addParams.Add("couponid=" + Request.QueryString["couponid"]);

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
                CouponInfo coupon = new CouponInfo();
                bool isNew = false;

                if (CouponId >= 0)
                    coupon = Controller.GetCouponById(CouponId);
                else
                    isNew = true;

                coupon.Caption = txtCaption.Text;
                coupon.Code = txtCode.Text;

                if (String.IsNullOrEmpty(txtDiscountPercent.Text))
                    coupon.DiscountPercent = null;
                else
                    coupon.DiscountPercent = Convert.ToDecimal(txtDiscountPercent.Text);
                    
                if (taxDiscountValue.NetPrice <= 0)
                    coupon.DiscountValue = null;
                else
                    coupon.DiscountValue = taxDiscountValue.NetPrice;

                coupon.TaxPercent = Convert.ToDecimal(txtTaxPercent.Text.Trim());
                coupon.MaxUsages = Convert.ToInt32(txtMaxUsages.Text);
                coupon.UsagesLeft = Convert.ToInt32(txtUsagesLeft.Text);

                if (String.IsNullOrEmpty(txtValidUntil.Text))
                    coupon.ValidUntil = null;
                else
                    coupon.ValidUntil = Convert.ToDateTime(txtValidUntil.Text);
                    

                if (isNew)
                    CouponId = Controller.NewCoupon(coupon);
                else
                    Controller.UpdateCoupon(coupon);

                List<string> addParams = new List<string>();

                if (Request["adminmode"] != null)
                    addParams.Add("adminmode=couponlist");
                addParams.Add("couponId=" + CouponId.ToString());

                Response.Redirect(Globals.NavigateURL(TabId, "", addParams.ToArray()), true);

            }
            catch (Exception exc)
            {
                //Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        

        protected void cmdGenerateCode_Click(object sender, EventArgs e)
        {
            string key = "";
            Random rnd = new Random();
            int zufall = 0;
            for (int i = 0; i < 25; i++)
            {
                do
                {
                    zufall = rnd.Next(48, 91);
                } while (zufall > 57 && zufall < 65 || (char)zufall == '0' || (char)zufall == 'O');

                key = key + (char)zufall;
                if (i % 5 == 4 && i < 24)
                    key = key + "-";

            }
            txtCode.Text = key;
        }

        #endregion
    }
}
