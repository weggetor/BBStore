using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.UI.Skins;
using DotNetNuke.Services.Localization;
using DotNetNuke.Common;

namespace Bitboxx.DNNModules.BBStore
{
    public partial class ProductGroupBreadCrumbSkinObject : SkinObjectBase
    {
        #region Private Members
        string _separator = "&nbsp;>&nbsp;";
		bool _IncludeChilds = false;
        #endregion

        #region Public Properties
        public string Separator
        {
            get { return _separator; }
            set { _separator = value; }
        }
		public bool IncludeChilds
		{
			get { return _IncludeChilds; }
			set { _IncludeChilds = value; }
		}
        #endregion


        
        protected void Page_Load(object sender, EventArgs e)
        {
            //TODO: Beim Aufruf der Seite ohne ProductGroupParameter wird der Breadcrumb nicht angezeigt (Info aus Filter)
			BBStoreController Controller = new BBStoreController();
            int ParentNode;
            if (Request["productgroup"] != null)
            {
            	ParentNode = Convert.ToInt32(Request["productgroup"]);
            
        		// We retrieve the treepath of the ParentNode
                string treePath = Controller.GetProductGroupPath(PortalSettings.PortalId, ParentNode);

                Label lbl = new Label();
                lbl.Text = Separator;
                phBreadCrumb.Controls.Add(lbl);

                // Now lets build the breadcrumb
                string[] bread = treePath.Split('/');
                LinkButton cmdBread;

                for (int i = 0; i < bread.Length; i++)
                {
                    cmdBread = new LinkButton();
                    int productGroupId = Convert.ToInt32(bread[i].Substring(1));
                    cmdBread.Attributes.Add("productgroup", productGroupId.ToString());
                    ProductGroupInfo pgi = Controller.GetProductGroup(PortalSettings.PortalId, System.Threading.Thread.CurrentThread.CurrentCulture.Name, productGroupId);
                    cmdBread.Text = pgi.ProductGroupName;
                    cmdBread.Click += new EventHandler(cmdBread_Click);
                    cmdBread.CssClass = "SkinObject";
                    phBreadCrumb.Controls.Add(cmdBread);

                    if (i + 1 < bread.Length)
                    {
                        lbl = new Label();
                        lbl.Text = Separator;
                        phBreadCrumb.Controls.Add(lbl);
                    }
                }
            }
        }
        void cmdBread_Click(object sender, EventArgs e)
        {

            LinkButton cmd = sender as LinkButton;
            if (cmd != null)
            {
                string strProductGroup = cmd.Attributes["productgroup"];
                if (strProductGroup != string.Empty)
                {
                    int productGroupId = Convert.ToInt32(strProductGroup);
					if (productGroupId == -1)
						Response.Redirect(Globals.NavigateURL(""));
					else
					{
						string[] param = new string[]
						                 	{
						                 		"productgroup=" + strProductGroup,
						                 		"includechilds=" + IncludeChilds.ToString().ToLower()
						                 	};
						Response.Redirect(Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "", param));
					}
                }
            }
        }

    }
}