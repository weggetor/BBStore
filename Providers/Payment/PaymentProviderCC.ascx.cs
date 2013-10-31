using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DotNetNuke.Services.Localization;
using System.Text;

namespace Bitboxx.DNNModules.BBStore.Providers.Payment
{
    public partial class PaymentProviderCC : PaymentProviderBase
    {
        public override string Title
        {
            get { return lblTitle.Text; }
            set { lblTitle.Text = value; }
        }
        public override string Values
        {
            get
            {
                return txtCCName.Text + "," + txtCCNo.Text + "," + txtCCValid.Text + "," + txtCCCvv.Text +"," + hidCCType.Value ;
            }
            set
            {
                string[] props = value.Split(new char[] { ',' });
                if (props.Length == 5)
                {
                    txtCCName.Text = props[0];
                    lblCCNameSummary.Text = props[0];
                    txtCCNo.Text = props[1];
                    lblCCNoSummary.Text = props[1];
                    txtCCValid.Text = props[2];
                    lblCCValidSummary.Text = props[2];
                    txtCCCvv.Text = props[3];
                    lblCCCvvSummary.Text = props[3];
                    hidCCType.Value = props[4];
                    foreach (ListItem item in lstCCType.Items)
                    {
                        if (item.Value == props[4])
                        {
                            lblCCTypeSummary.Text = item.Text;
                            break;
                        }
                    }
                }
            }
        }
        public override string Properties
        {
            get
            {
                string result = "";
                bool first = true;
                foreach (ListItem item in lstCCType.Items)
                {
                    if (item.Selected)
                    {
                        result += (first ? "" : ",") + item.Value;
                        first = false;
                    }
                }
                return result;
            }
            set
            {
                string[] props = value.Split(new char[] { ',' });
                foreach (ListItem item in lstCCType.Items)
                {
                    if (props.Contains(item.Value))
                        item.Selected = true;
                }
            }
        }

        public override bool IsValid
        {
            get { return hidCCType.Value != string.Empty; }
        }
        
        //Find for jquery and css using header control id
        //If not included then include to the page header
        protected void Page_Init(object sender, System.EventArgs e)
        {
            //Include js file
            HtmlGenericControl scriptInclude = (HtmlGenericControl) Page.Header.FindControl("CCValidator");
            if (scriptInclude == null)
            {
                scriptInclude = new HtmlGenericControl("script");
                scriptInclude.Attributes["type"] = "text/javascript";
                scriptInclude.Attributes["src"] = this.TemplateSourceDirectory + "/js/jquery.creditCardValidator.js";
                scriptInclude.ID = "CCValidator";

                Page.Header.Controls.Add(scriptInclude);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            DotNetNuke.Framework.jQuery.RegisterJQuery(Page);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            string ccTypeList = "";
                bool first = true;
            foreach (ListItem item in lstCCType.Items)
            {
                if (item.Selected)
                {
                    HtmlGenericControl li = new HtmlGenericControl("li");
                    li.Attributes.Add("class", item.Value);
                    li.InnerText = item.Text;
                    ulCards.Controls.Add(li);
                    ccTypeList += (first ? "" : ",") + "'" + item.Value + "'";
                    first = false;
                }
            }
            hidCCTypeList.Value = ccTypeList;

            switch (DisplayMode)
            {
                case ViewMode.View:
                    pnlShow.SetActiveView(View);
                    break;
                case ViewMode.Edit:
                    pnlShow.SetActiveView(Edit);
                    break;
                case ViewMode.Summary:
                    pnlShow.SetActiveView(Summery);
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


            lblCCNoCapView.Text = Localization.GetString("lblCCNo.Text", this.LocalResourceFile);
            lblCCValidCapView.Text = Localization.GetString("lblCCValid.Text", this.LocalResourceFile);
            lblCCCvvCapView.Text = Localization.GetString("lblCCCvv.Text", this.LocalResourceFile);
            lblCCNameCapView.Text = Localization.GetString("lblCCName.Text", this.LocalResourceFile);

            lblCCNoCapSummary.Text = Localization.GetString("lblCCNo.Text", this.LocalResourceFile);
            lblCCValidCapSummary.Text = Localization.GetString("lblCCValid.Text", this.LocalResourceFile);
            lblCCNameCapSummary.Text = Localization.GetString("lblCCName.Text", this.LocalResourceFile);
            lblCCTypeCapSummary.Text = Localization.GetString("lblCCType.Text", this.LocalResourceFile);
        }
    }
}