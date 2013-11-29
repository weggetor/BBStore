using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Services.Localization;
using System.Text;

namespace Bitboxx.DNNModules.BBStore.Providers.Payment
{
    public partial class PaymentProviderPrepaid : PaymentProviderBase
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
        public override string Properties
        {
            get
            {
                return txtAccountName.Text + "," + txtBankName.Text + "," + txtBin.Text + "," + txtAccountNo.Text+ "," + txtIban.Text + "," + txtBic.Text;
            }
            set
            {
                string[] props = value.Split(new char[] { ',' });
                if (props.Length > 3)
                {
                    txtAccountName.Text = props[0];
                    lblAccountNameView.Text = props[0];
                    lblAccountNameSummary.Text = props[0];
                    txtBankName.Text = props[1];
                    lblBankNameView.Text = props[1];
                    lblBankNameSummary.Text = props[1];
                    txtBin.Text = props[2];
                    lblBinView.Text = props[2];
                    lblBinSummary.Text = props[2];
                    txtAccountNo.Text = props[3];
                    lblAccountNoView.Text = props[3];
                    lblAccountNoSummary.Text = props[3];
                }
                if (props.Length > 4)
                {
                    txtIban.Text = props[4];
                    lblIbanView.Text = props[4];
                    lblIbanSummary.Text = props[4];
                }
                if (props.Length > 5)
                {
                    txtBic.Text = props[5];
                    lblBicView.Text = props[5];
                    lblBicSummary.Text = props[5];
                }
            }
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

            lblAccountNameCapView.Text = Localization.GetString("lblAccountName.Text", this.LocalResourceFile);
            lblBankNameCapView.Text = Localization.GetString("lblBankName.Text", this.LocalResourceFile);
            lblBinCapView.Text = Localization.GetString("lblBin.Text", this.LocalResourceFile);
            lblAccountNoCapView.Text = Localization.GetString("lblAccountNo.Text", this.LocalResourceFile);
            lblIbanCapView.Text = Localization.GetString("lblIban.Text", this.LocalResourceFile);
            lblBicCapView.Text = Localization.GetString("lblBic.Text", this.LocalResourceFile);

            lblAccountNameCapSummary.Text = Localization.GetString("lblAccountName.Text", this.LocalResourceFile);
            lblBankNameCapSummary.Text = Localization.GetString("lblBankName.Text", this.LocalResourceFile);
            lblBinCapSummary.Text = Localization.GetString("lblBin.Text", this.LocalResourceFile);
            lblAccountNoCapSummary.Text = Localization.GetString("lblAccountNo.Text", this.LocalResourceFile);
            lblIbanCapSummary.Text = Localization.GetString("lblIban.Text", this.LocalResourceFile);
            lblBicCapSummary.Text = Localization.GetString("lblBic.Text", this.LocalResourceFile);

        }
    }
}