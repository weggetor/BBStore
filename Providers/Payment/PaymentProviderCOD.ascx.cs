using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Services.Localization;

namespace Bitboxx.DNNModules.BBStore.Providers.Payment
{
    public partial class PaymentProviderCOD : PaymentProviderBase
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
                cost = String.Format(Localization.GetString("lblCost.Text", this.LocalResourceFile), this.cost * (ShowNetprice ? 1 : (100 + TaxPercent) / 100));
            else if (this.cost < 0)
                cost = String.Format(Localization.GetString("lblDiscount.Text", this.LocalResourceFile), (-1) * this.cost * (ShowNetprice ? 1 : (100 + TaxPercent) / 100));
            else
                cost = Localization.GetString("lblFree.Text", this.LocalResourceFile);

            lblDescription.Text = String.Format(Localization.GetString("lblDescription.Text", this.LocalResourceFile), cost);
        }

    }
}