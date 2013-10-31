using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Common;
using DotNetNuke.Services.Localization;

namespace Bitboxx.DNNModules.BBStore
{
    public partial class TaxControl : PortalModuleBase
    {
        private string _orientation;
        private bool _shortCaps;
        private WebControl _percentControl;
        private string _cssClass;

        public string Mode
        {
            get 
            {
                if (rdbBrutto.Checked)
                    return "gross";
                else
                    return "net";

            }
            set 
            {
                if (value == "gross")
                    rdbBrutto.Checked = true;
                else
                    rdbNetto.Checked = true;
            }
        }
        
        public WebControl PercentControl
        {
            set { _percentControl = value; }
            get { return _percentControl; }
        }
        public decimal Value
        {
            get
            {
                return decimal.Parse(hidNetto.Value, System.Globalization.CultureInfo.InvariantCulture);
            }
            set
            {
                decimal taxPercent = 0.00m;
                if (_percentControl != null)
                {
                    TextBox ctrl = _percentControl as TextBox;
                    if (ctrl != null)
                    {
                        string strPercent = ctrl.Text.Replace(',', '.');
                        Decimal.TryParse(strPercent, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out taxPercent);
                    }
                }
                hidNetto.Value = value.ToString(System.Globalization.CultureInfo.InvariantCulture);
                hidBrutto.Value = String.Format(System.Globalization.CultureInfo.InvariantCulture,"{0:f2}",(value * (1 + taxPercent / 100)));
                if (rdbBrutto.Checked)
                    txtAmount.Text = hidBrutto.Value;
                else
                    txtAmount.Text = hidNetto.Value;
            }
        }
        public decimal NetPrice 
        {
            get
            {
                return Value;
            }
        }
        public decimal GrossPrice 
        {
            get
            {
                return decimal.Parse(hidBrutto.Value, System.Globalization.CultureInfo.InvariantCulture);
            }
        }
        public string Orientation
        {
            set
            {
                _orientation = value;
            }
        }
        public bool ShortCaps
        {
            set
            {
                _shortCaps = value;
            }
        }
        public string CssClass
        {
            get { return _cssClass; }
            set { _cssClass = value; }
        }
        

        protected void Page_Load(object sender, EventArgs e)
        {
            this.EnableViewState = true;
            if (!Page.ClientScript.IsClientScriptBlockRegistered("CalcTax"))
            {
                string calcTaxScript = "<script src=\"" + ControlPath + "js/CalcTax.js\" type=\"text/javascript\"></script>";
                Page.ClientScript.RegisterClientScriptBlock(typeof(Page),"CalcTax", calcTaxScript);
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (_orientation.ToLower() == "vertical")
                ltrLineBreak.Text = "<br />";
            else
                ltrLineBreak.Text = "";

            if (_shortCaps)
            {
                rdbNetto.Text = Localization.GetString("NetShort.Text", this.LocalResourceFile);
                rdbBrutto.Text = Localization.GetString("GrossShort.Text", this.LocalResourceFile);
            }
            else
            {
                rdbNetto.Text = Localization.GetString("Net.Text", this.LocalResourceFile);
                rdbBrutto.Text = Localization.GetString("Gross.Text", this.LocalResourceFile);
            }
            if (_cssClass != string.Empty)
            {
                divMain.Attributes.Add("class",divMain.Attributes["class"] + " " + _cssClass);
            }

            // Rebind events
            txtAmount.Attributes.Remove("onkeyup");
            rdbBrutto.Attributes.Remove("onclick");
            rdbNetto.Attributes.Remove("onclick");
            PercentControl.Attributes.Remove("onkeyup");
            string cIds = _percentControl.ClientID + "," + txtAmount.ClientID + "," + rdbNetto.ClientID + "," + rdbBrutto.ClientID + "," + hidBrutto.ClientID + "," + hidNetto.ClientID;
            txtAmount.Attributes.Add("onkeyup", "CalculateTax(" + cIds + ");");
            rdbBrutto.Attributes.Add("onclick", "ShowGross(" + cIds + ");");
            rdbNetto.Attributes.Add("onclick", "ShowNet(" + cIds + ");");
            PercentControl.Attributes.Add("onkeyup", "CalculateTax(" + cIds + ");");


        }
        #region PortalModuleBase Overrides
        protected override void OnLoad(EventArgs e)
        {
            this.LocalResourceFile = Localization.GetResourceFile(this, this.GetType().BaseType.Name + ".ascx");
            base.OnLoad(e);
        }
        #endregion


    }
}