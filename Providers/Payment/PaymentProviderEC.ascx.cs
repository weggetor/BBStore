using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Services.Localization;
using System.Text;

namespace Bitboxx.DNNModules.BBStore.Providers.Payment
{
    public partial class PaymentProviderEC : PaymentProviderBase
    {
        public override string Title
        {
            get
            {
                return lblTitle.Text;
            }
            set
            {
                lblTitle.Text = value;
            }
        }
        public override string Values
        {
            get
            {
                return ddlCountry.SelectedItem.Value + "," + txtAccountName.Text + "," + txtBankName.Text + "," + txtBin.Text + "," + txtAccountNo.Text + "," + txtIban.Text + "," + txtBic.Text ;
            }
            set
            {
                string[] props = value.Split(new char[] { ',' });
                if (props.Length == 7)
                {
                    ddlCountry.SelectedValue = props[0];
                    txtAccountName.Text = props[1];
                    lblAccountName.Text = props[1];
                    txtBankName.Text = props[2];
                    lblBankName.Text = props[2];
                    txtBin.Text = props[3];
                    lblBin.Text = props[3];
                    txtAccountNo.Text = props[4];
                    lblAccountNo.Text = props[4];
                    txtIban.Text = props[5];
                    lblIban.Text = props[5];
                    txtBic.Text = props[6];
                    lblBic.Text = props[6];
                }
            }
        }
        
        public override bool IsValid
        {
            get
            {
                bool isfilled = txtAccountName.Text != string.Empty &&
                                txtAccountNo.Text != string.Empty &&
                                txtBankName.Text != string.Empty &&
                                txtBin.Text != string.Empty &&
                                txtIban.Text != string.Empty &&
                                txtBic.Text != string.Empty;
                bool bicOK = txtBic.Text.Trim().Length == 8 || txtBic.Text.Trim().Length == 11;
                bool ibanOk = (hidValid.Value == "true");
                return isfilled && bicOK && ibanOk;
            }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.ClientScript.IsClientScriptBlockRegistered("Iban"))
            {
                string calcTaxScript = "<script src=\"" + ControlPath + "js/iban.js\" type=\"text/javascript\"></script>";
                Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Iban", calcTaxScript);
            }
            FillCountry();
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            switch (DisplayMode)
            {
                case ViewMode.View:
                    pnlShow.SetActiveView(View);
                    break;
                case ViewMode.Edit:
                    pnlShow.SetActiveView(Edit);
                    break;
                case ViewMode.Summary:
                    pnlShow.SetActiveView(Summary);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            string description = "";
            decimal fixedAmount = 0.0m;
            if (this._cost > 0)
            {
                description = LocalizeString("lblCost.Text");
                fixedAmount = this._cost * (ShowNetprice ? 1 : (100 + TaxPercent) / 100);
            }
            else if (this._cost < 0)
            {
                description = LocalizeString("lblDiscount.Text");
                fixedAmount = (-1) * this._cost * (ShowNetprice ? 1 : (100 + TaxPercent) / 100);
            }
            else
            {
                description = LocalizeString("lblFree.Text");
            }

            string costText = "";
            if (this._cost != 0 && this._costPercent != 0)
                costText = String.Format(LocalizeString("lblFixed.Text"), fixedAmount) + " " + LocalizeString("lblConcat.Text") + " " + String.Format(LocalizeString("lblPercentage.Text"), this._costPercent);
            else if (this._cost != 0)
                costText = String.Format(LocalizeString("lblFixed.Text"), fixedAmount);
            else if (this._costPercent != 0)
                costText = String.Format(LocalizeString("lblPercentage.Text"), this._costPercent);

            lblDescription.Text = String.Format(LocalizeString("lblDescription.Text"), String.Format(description, costText));

            txtAccountNo.Attributes.Add("onblur", "CreateIBAN(" + ddlCountry.ClientID + "," + txtBin.ClientID + "," + txtAccountNo.ClientID + "," + txtIban.ClientID + "," + hidValid.ClientID + ");");
            txtIban.Attributes.Add("onblur", "CheckIBAN(" + ddlCountry.ClientID + "," + txtBin.ClientID + "," + txtAccountNo.ClientID + "," + txtIban.ClientID + "," + hidValid.ClientID + ");");
            txtBic.Attributes.Add("onblur", "CheckBic(" + txtBic.ClientID + ");");
            lblCountry.Text = ddlCountry.SelectedItem.Text;
            lblAccountNameSummary.Text = Localization.GetString("lblAccountName.Text", this.LocalResourceFile);
            lblCountrySummary.Text = Localization.GetString("lblCountry.Text", this.LocalResourceFile);
            lblBankNameSummary.Text = Localization.GetString("lblBankName.Text", this.LocalResourceFile);
            lblBinSummary.Text = Localization.GetString("lblBin.Text", this.LocalResourceFile);
            lblAccountNoSummary.Text = Localization.GetString("lblAccountNo.Text", this.LocalResourceFile);
            lblIbanSummary.Text = Localization.GetString("lblIban.Text", this.LocalResourceFile);
            lblBicSummary.Text = Localization.GetString("lblBic.Text", this.LocalResourceFile);
        }

        protected void FillCountry()
        {
            ddlCountry.Items.Add(new ListItem("Andorra", "AD"));
            ddlCountry.Items.Add(new ListItem("Albania", "AL"));
            ddlCountry.Items.Add(new ListItem("Austria", "AT"));
            ddlCountry.Items.Add(new ListItem("Bosnia and Herzegovina", "BA"));
            ddlCountry.Items.Add(new ListItem("Belgium", "BE"));
            ddlCountry.Items.Add(new ListItem("Bulgaria", "BG"));
            ddlCountry.Items.Add(new ListItem("Switzerland", "CH"));
            ddlCountry.Items.Add(new ListItem("Cyprus", "CY"));
            ddlCountry.Items.Add(new ListItem("Czech Republic", "CZ"));
            ddlCountry.Items.Add(new ListItem("Germany", "DE"));
            ddlCountry.Items.Add(new ListItem("Denmark", "DK"));
            ddlCountry.Items.Add(new ListItem("Estonia", "EE"));
            ddlCountry.Items.Add(new ListItem("Spain", "ES"));
            ddlCountry.Items.Add(new ListItem("Finland", "FI"));
            ddlCountry.Items.Add(new ListItem("Faroe Islands", "FO"));
            ddlCountry.Items.Add(new ListItem("France", "FR"));
            ddlCountry.Items.Add(new ListItem("United Kingdom", "GB"));
            ddlCountry.Items.Add(new ListItem("Georgia", "GE"));
            ddlCountry.Items.Add(new ListItem("Gibraltar", "GI"));
            ddlCountry.Items.Add(new ListItem("Greenland", "GL"));
            ddlCountry.Items.Add(new ListItem("Greece", "GR"));
            ddlCountry.Items.Add(new ListItem("Croatia", "HR"));
            ddlCountry.Items.Add(new ListItem("Hungary", "HU"));
            ddlCountry.Items.Add(new ListItem("Ireland", "IE"));
            ddlCountry.Items.Add(new ListItem("Israel", "IL"));
            ddlCountry.Items.Add(new ListItem("Iceland", "IS"));
            ddlCountry.Items.Add(new ListItem("Italy", "IT"));
            ddlCountry.Items.Add(new ListItem("Kuwait", "KW"));
            ddlCountry.Items.Add(new ListItem("Kazakhstan", "KZ"));
            ddlCountry.Items.Add(new ListItem("Lebanon", "LB"));
            ddlCountry.Items.Add(new ListItem("Liechtenstein", "LI"));
            ddlCountry.Items.Add(new ListItem("Lithuania", "LT"));
            ddlCountry.Items.Add(new ListItem("Luxembourg", "LU"));
            ddlCountry.Items.Add(new ListItem("Latvia", "LV"));
            ddlCountry.Items.Add(new ListItem("Monaco", "MC"));
            ddlCountry.Items.Add(new ListItem("Montenegro", "ME"));
            ddlCountry.Items.Add(new ListItem("Macedonia, Former Yugoslav Republic of", "MK"));
            ddlCountry.Items.Add(new ListItem("Mauritania", "MR"));
            ddlCountry.Items.Add(new ListItem("Malta", "MT"));
            ddlCountry.Items.Add(new ListItem("Mauritius", "MU"));
            ddlCountry.Items.Add(new ListItem("Netherlands", "NL"));
            ddlCountry.Items.Add(new ListItem("Norway", "NO"));
            ddlCountry.Items.Add(new ListItem("Poland", "PL"));
            ddlCountry.Items.Add(new ListItem("Portugal", "PT"));
            ddlCountry.Items.Add(new ListItem("Romania", "RO"));
            ddlCountry.Items.Add(new ListItem("Serbia", "RS"));
            ddlCountry.Items.Add(new ListItem("Saudi Arabia", "SA"));
            ddlCountry.Items.Add(new ListItem("Sweden", "SE"));
            ddlCountry.Items.Add(new ListItem("Slovenia", "SI"));
            ddlCountry.Items.Add(new ListItem("Slovak Republic", "SK"));
            ddlCountry.Items.Add(new ListItem("San Marino", "SM"));
            ddlCountry.Items.Add(new ListItem("Tunisia", "TN"));
            ddlCountry.Items.Add(new ListItem("Turkey", "TR"));

            BBStoreController controller = new BBStoreController();

            Hashtable storeSettings = controller.GetStoreSettings(PortalId);
            string countryCode = (string) storeSettings["VendorCountry"];
            try
            {
                ddlCountry.SelectedValue = countryCode;
            }
            catch (Exception)
            {
            }
        }
    }
}