using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;

namespace Bitboxx.DNNModules.BBStore
{
	public partial class CartNavigationControl : PortalModuleBase
	{
        private List<NavigationInfo> _steps = new List<NavigationInfo>();
		private string _action;
	    private string _imageStyle = "standard";

		public List<NavigationInfo> Steps
		{
			get { return _steps; }
			set { _steps = value; }
		}

	    public string ImageStyle
	    {
            get { return _imageStyle; }
            set { _imageStyle = value; }
	    }

	    public string[] ImageStyles
	    {
	        get
	        {
	            List<string> imageStyles = new List<string>();
                string cssContent = "";

                string moduleCssFile = PathUtils.Instance.MapPath("~/DesktopModules/BBStore/module.css");
                if (File.Exists(moduleCssFile))
                {
                    cssContent += File.ReadAllText(moduleCssFile);
                }

                string portalCssFile = this.PortalSettings.HomeDirectoryMapPath + "portal.css";
	            if (File.Exists(portalCssFile))
	            {
	                cssContent += File.ReadAllText(portalCssFile);
	            }

	            string[] lines = cssContent.Split(new string[] {"\r\n", "\n"}, StringSplitOptions.None);
	            foreach (string line in lines)
	            {
	                if (line.StartsWith(".bb-navigationcontrol."))
	                {
	                    string imageStyle = VfpInterop.StrExtract(line, ".bb-navigationcontrol.", " ", 1, 1);
	                    if (!imageStyles.Contains(imageStyle))
	                        imageStyles.Add(imageStyle);
	                }
	            }
	            return imageStyles.ToArray();
	        }
	    }

	    public string Action
		{
			get { return _action; }
			set
			{
				_action = value;
				var state = NavigationInfo.NavigationState.Set;

				foreach (NavigationInfo navigationInfo in _steps)
				{
					navigationInfo.State = state;
					if (navigationInfo.Action.ToLower() == _action.ToLower())
					{
						navigationInfo.State = NavigationInfo.NavigationState.Active;
						state = NavigationInfo.NavigationState.Empty;
					}
				}
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (_steps.Count > 0)

			{
				Table mainTable = new Table();
				mainTable.CellPadding = 0;
				mainTable.CellSpacing = 0;
				mainTable.BorderWidth = 0;
				mainTable.Width = new Unit(100, UnitType.Percentage);

				Unit colwidth = new Unit(100/_steps.Count, UnitType.Percentage);

			    TableRow imageRow = new TableRow();
                

			    int i = 0;
				foreach (NavigationInfo step in _steps)
				{
				    i++;

                    // cell for as outer cell for table with leftimage + middleimage + rightimage
                    TableCell imageCell = new TableCell();
                    imageCell.Width = colwidth;
                    imageCell.Style.Add("border", "0");
                    imageCell.Style.Add("padding", "0");
                    imageCell.Style.Add("margin", "0");
                    
                    // corresponding table
                    Table stepTable = new Table();
                    stepTable.Attributes.Add("class", "steptable");

					// corresponding table row 
                    TableRow stepRow = new TableRow();
                    stepRow.Attributes.Add("class", "steprow");

					TableCell leftOuterCell = new TableCell();
					leftOuterCell.ID = "cell_leftouter_" + step.Action;
                    leftOuterCell.Attributes.Add("class","leftouterstep");
                    if (i==1)
                    {
                        leftOuterCell.Attributes.Add("class", leftOuterCell.Attributes["class"] + " first");
                    }
					stepRow.Cells.Add(leftOuterCell);

                    TableCell leftCell = new TableCell();
                    leftCell.ID = "cell_left_" + step.Action;
                    leftCell.Attributes.Add("class", "leftstep");
                    stepRow.Cells.Add(leftCell);

					TableCell middleCell = new TableCell();
                    middleCell.ID = "cell_middle_" + step.Action;
                    middleCell.Attributes.Add("class", "middlestep");
                    
                    // add link / caption
                    LinkButton lnk = new LinkButton();
                    lnk.ID = "cmd_" + step.Action;
                    lnk.Click += lnk_Click;
                    lnk.Text = step.Caption;
                    lnk.CausesValidation = false;
                    middleCell.Controls.Add(lnk);

                    Label lbl = new Label();
                    lbl.ID = "lbl_" + step.Action;
                    lbl.Text = step.Caption;
                    middleCell.Controls.Add(lbl);
                    stepRow.Cells.Add(middleCell);

                    TableCell rightCell = new TableCell();
                    rightCell.ID = "cell_right_" + step.Action;
                    rightCell.Attributes.Add("class", "rightstep");
                    stepRow.Cells.Add(rightCell);
                    
                    TableCell rightOuterCell = new TableCell();
					rightOuterCell.ID = "cell_rightouter_" + step.Action;
                    rightOuterCell.Attributes.Add("class", "rightouterstep");
                    if (i == Steps.Count)
                    {
                        rightOuterCell.Attributes.Add("class", rightOuterCell.Attributes["class"] + " last");
                    }
					stepRow.Cells.Add(rightOuterCell);
					stepTable.Rows.Add(stepRow);
					imageCell.Controls.Add(stepTable);

					imageRow.Cells.Add(imageCell);
				}
                mainTable.Rows.Add(imageRow);
				PlaceHolder1.Controls.Add(mainTable);
			}
			else
			{
				string message = Localization.GetString("NoSteps.Text", this.LocalResourceFile);
				DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, message, ModuleMessage.ModuleMessageType.YellowWarning);
			}

		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			divMain.Attributes.Add("class","bb-navigationcontrol " + ImageStyle);
            foreach (NavigationInfo step in _steps)
            {
                NavigationInfo.NavigationState state = step.State;

                Label lbl = this.FindControl("lbl_" + step.Action) as Label;
				LinkButton cmd = this.FindControl("cmd_" + step.Action) as LinkButton;
				cmd.Visible = step.Enabled;
				lbl.Visible = !step.Enabled;
                lbl.Attributes.Add("class",state.ToString().ToLower());
                cmd.Attributes.Add("class", state.ToString().ToLower());

				TableCell leftOuterCell = this.FindControl("cell_leftouter_" + step.Action) as TableCell;
                TableCell leftCell = this.FindControl("cell_left_" + step.Action) as TableCell;
                TableCell middleCell = this.FindControl("cell_middle_" + step.Action) as TableCell;
				TableCell rightCell = this.FindControl("cell_right_" + step.Action) as TableCell;
                TableCell rightOuterCell = this.FindControl("cell_rightouter_" + step.Action) as TableCell;

                leftOuterCell.Attributes.Add("class", leftOuterCell.Attributes["class"] + " " + state.ToString().ToLower());
                leftCell.Attributes.Add("class", leftCell.Attributes["class"] + " " + state.ToString().ToLower());
                middleCell.Attributes.Add("class", middleCell.Attributes["class"] + " " + state.ToString().ToLower());
                rightCell.Attributes.Add("class", rightCell.Attributes["class"] + " " + state.ToString().ToLower());
                rightOuterCell.Attributes.Add("class", rightOuterCell.Attributes["class"] + " " + state.ToString().ToLower());
 			}
		}

		protected void lnk_Click(object sender, EventArgs e)
		{
			LinkButton lnk = sender as LinkButton;

			string command = lnk.ID.Substring(4).ToLower();

			var query = (from s in Steps where s.Action.ToLower() == command select s).FirstOrDefault();
			if (query != null)
			{
				Response.Redirect(query.Url);
			}
		}

		public void Enable(string navigationAction, bool value)
		{
			var query = (from s in Steps where s.Action.ToLower() == navigationAction.ToLower() select s).FirstOrDefault();
			if (query != null)
				query.Enabled = value;
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