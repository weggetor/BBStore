using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Services.Localization;

namespace Bitboxx.DNNModules.BBStore.Providers.Payment
{
    public partial class PaymentProviderPaypal : PaymentProviderBase
    {
        public override string Title
        {
            get { return lblTitle.Text; }
            set { lblTitle.Text = value; }
        }

        public override bool IsValid
        {
            get { return true;}
        }

        public override string Properties
        {
            get { return txtPPUser.Text + "," + txtPPPassword.Text + "," + txtPPSignature.Text + "," + chkSandbox.Checked.ToString(); }
            set
            {
                string[] props = value.Split(',');
                if (props.Length > 2)
                {
                    txtPPUser.Text = props[0];
                    txtPPPassword.Text = props[1];
                    txtPPSignature.Text = props[2];
                }

                if (props.Length > 3)
                    chkSandbox.Checked = Convert.ToBoolean(props[3]);
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

            string cost = "";
            if (this.cost > 0)
                cost = String.Format(Localization.GetString("lblCost.Text", this.LocalResourceFile), this.cost*(ShowNetprice ? 1 : (100 + TaxPercent)/100));
            else if (this.cost < 0)
                cost = String.Format(Localization.GetString("lblDiscount.Text", this.LocalResourceFile), (-1)*this.cost*(ShowNetprice ? 1 : (100 + TaxPercent)/100));
            else
                cost = Localization.GetString("lblFree.Text", this.LocalResourceFile);

            lblDescription.Text = String.Format(Localization.GetString("lblDescription.Text", this.LocalResourceFile), cost);
        }
    }
}