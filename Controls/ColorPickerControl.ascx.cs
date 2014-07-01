using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Bitboxx.DNNModules.BBStore.Controls
{
    public partial class ColorPickerControl : System.Web.UI.UserControl
    {
        public string Text
        {
            get { return txtColor.Text; }
            set { txtColor.Text = value; }
        }
        
        protected override void OnInit(EventArgs e)
        {
            //Find for jquery and css using header control id
            //If not included then include to the page header
            //Include js file
            HtmlGenericControl scriptInclude = (HtmlGenericControl)Page.Header.FindControl("colorpicker");
            if (scriptInclude == null)
            {
                scriptInclude = new HtmlGenericControl("script");
                scriptInclude.Attributes["type"] = "text/javascript";
                scriptInclude.Attributes["src"] = this.TemplateSourceDirectory + "/js/jquery.colorpicker.js";
                scriptInclude.ID = "colorpicker";
                Page.Header.Controls.Add(scriptInclude);
            }
            base.OnInit(e);
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}