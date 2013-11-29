using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using System.Drawing;
using DotNetNuke.Services.Localization;

namespace Bitboxx.DNNModules.BBStore
{
	public enum FeatureGridMode
	{
		View,
		Edit,
		Search
	}

	public partial class FeatureGridControl : PortalModuleBase
	{

		#region "Properties"

		// Data Properties
		public FeatureGridMode Mode
		{
			set 
			{
				ViewState["Mode"] = value;
			}
			get
			{
				FeatureGridMode _mode = FeatureGridMode.View;
				if (ViewState["Mode"] != null)
					_mode = (FeatureGridMode)ViewState["Mode"];
				return _mode;
			}
		}
		public int FeatureGroupId
		{
			set { _FeatureGroupId = value; }
			get { return _FeatureGroupId; }
		}
		public int ProductId
		{
			set { _ProductId = value; }
			get { return _ProductId; }
		}
		public int ProductGroupId
		{
			set { _ProductGroupId = value; }
			get { return _ProductGroupId; }
		}
		public Guid FilterSessionId { get; set; }
		public int SearchTabId { get; set; }

		// Border
		public BorderStyle BorderStyle
		{
			set { _BorderStyle = value; }
			get { return _BorderStyle; }
		}
		public Unit BorderWidth
		{
			set { _BorderWidth = value; }
			get { return _BorderWidth; }
		}
		public System.Drawing.Color BorderColor
		{
			set { _BorderColor = value; }
			get { return _BorderColor; }
		}

		// Colors
		public string FeatureGridCssClass { get; set; }
		public string FeatureRowCssClass
		{
			set { _FeatureRowCssClass = value; }
			get { return _FeatureRowCssClass; }
		}
		public string AlternateFeatureRowCssClass
		{
			set { _AlternateFeatureRowCssClass = value; }
			get { return _AlternateFeatureRowCssClass; }
		}
		public string GroupCssClass
		{
			set { _GroupCssClass = value; }
			get { return _GroupCssClass; }
		}
		public string FeatureCaptionCssClass
		{
			set { _FeatureCaptionCssClass = value; }
			get { return _FeatureCaptionCssClass; }
		}
		//public System.Drawing.Color FeatureBackColor
		//{
		//    set { _FeatureBackColor = value; }
		//    get { return _FeatureBackColor; }
		//}
		//public System.Drawing.Color FeatureForeColor
		//{
		//    set { _FeatureForeColor = value; }
		//    get { return _FeatureForeColor; }
		//}
		
		//public System.Drawing.Color AlternateFeatureBackColor
		//{
		//    set { _AlternateFeatureBackColor = value; }
		//    get { return _AlternateFeatureBackColor; }
		//}
		//public System.Drawing.Color AlternateFeatureForeColor
		//{
		//    set { _AlternateFeatureForeColor = value; }
		//    get { return _AlternateFeatureForeColor; }
		//}
		
		//public System.Drawing.Color GroupBackColor
		//{
		//    set { _GroupBackColor = value; }
		//    get { return _GroupBackColor; }
		//}
		//public System.Drawing.Color GroupForeColor
		//{
		//    set { _GroupForeColor = value; }
		//    get { return _GroupForeColor; }
		//}
		
		//public System.Drawing.Color FeatureValueBackColor
		//{
		//    set { _FeatureValueBackColor = value; }
		//    get { return _FeatureValueBackColor; }
		//}
		//public System.Drawing.Color FeatureValueForeColor
		//{
		//    set { _FeatureValueForeColor = value; }
		//    get { return _FeatureValueForeColor; }
		//}
		
		//public System.Drawing.Color AlternateFeatureValueBackColor
		//{
		//    set { _AlternateFeatureValueBackColor = value; }
		//    get { return _AlternateFeatureValueBackColor; }
		//}
		//public System.Drawing.Color AlternateFeatureValueForeColor
		//{
		//    set { _AlternateFeatureValueForeColor = value; }
		//    get { return _AlternateFeatureValueForeColor; }
		//}
		

		// Language
		protected string CurrentLanguage
		{
			get
			{
				return System.Threading.Thread.CurrentThread.CurrentCulture.Name;
			}
		}
		protected string DefaultLanguage
		{
			get
			{
				return this.PortalSettings.DefaultLanguage;
			}
		}
		#endregion

		#region Fields
		private int _FeatureGroupId = -1;
		private int _ProductId;
		private int _ProductGroupId;
		private Unit _BorderWidth;
		private System.Drawing.Color _BorderColor;
		private BorderStyle _BorderStyle;
		private string _FeatureRowCssClass;
		private string _AlternateFeatureRowCssClass;
		private string _GroupCssClass;
		private string _FeatureCaptionCssClass;
		private BBStoreController Controller;

		//private System.Drawing.Color _FeatureBackColor;
		//private System.Drawing.Color _FeatureForeColor;
		//private System.Drawing.Color _AlternateFeatureBackColor;
		//private System.Drawing.Color _AlternateFeatureForeColor;
		//private System.Drawing.Color _GroupBackColor;
		//private System.Drawing.Color _GroupForeColor;
		//private System.Drawing.Color _FeatureValueBackColor;
		//private System.Drawing.Color _FeatureValueForeColor;
		//private System.Drawing.Color _AlternateFeatureValueBackColor;
		//private System.Drawing.Color _AlternateFeatureValueForeColor;
		
		#endregion

		#region Events
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			Controller = new BBStoreController();

			string FileName = System.IO.Path.GetFileNameWithoutExtension(this.AppRelativeVirtualPath);
			if (this.ID != null)
				//this will fix it when its placed as a ChildUserControl 
				this.LocalResourceFile = this.LocalResourceFile.Replace(this.ID, FileName);
			else
				// this will fix it when its dynamically loaded using LoadControl method 
				this.LocalResourceFile = this.LocalResourceFile + FileName + ".ascx.resx";
		}
		protected void Page_Load(object sender, EventArgs e)
		{
			switch (Mode)
			{
				case FeatureGridMode.View:
					RenderViewMode();
					break;
				case FeatureGridMode.Edit:
					RenderEditMode();
					break;
				case FeatureGridMode.Search:
					RenderSearchMode();
					break;
				default:
					RenderViewMode();
					break;
			}
		}
		protected void cmdSearchFeature_Click(object sender, EventArgs e)
		{
			Control ctrl = sender as Control;
			int FeatureId;
			string DataType = ctrl.ID.Substring(4, 1);

			if (ctrl is DropDownList && ((DropDownList)ctrl).SelectedValue != "0")
			{
				FeatureId = Convert.ToInt32(ctrl.ID.Substring(5));
				ProductFilterInfo pf = new ProductFilterInfo();
				pf.FilterSessionId = FilterSessionId;
				pf.FilterSource = "Feature";
				pf.FilterValue = FeatureId.ToString() + "|" + ((DropDownList)ctrl).SelectedValue.Trim();
				pf.PortalId = PortalId;
				Controller.NewProductFilter(pf);
			}
			else if (ctrl is TextBox && ((TextBox)ctrl).Text != string.Empty)
			{
				FeatureId = Convert.ToInt32(ctrl.ID.Substring(5));
				ProductFilterInfo pf = new ProductFilterInfo();
				pf.FilterSessionId = FilterSessionId;
				pf.FilterSource = "Feature";
				pf.FilterValue = FeatureId.ToString() + "|" + ((TextBox)ctrl).Text.Trim();
				pf.PortalId = PortalId;
				Controller.NewProductFilter(pf);
			}
			else if (ctrl is ListBox)
			{
				string val = "";
				ListBox lst = ctrl as ListBox;
				foreach (ListItem item in lst.Items)
				{
					if (item.Selected && item.Value != "0")
						val += "|" + item.Value;
				}
				if (val != string.Empty)
				{
					FeatureId = Convert.ToInt32(ctrl.ID.Substring(5));
					ProductFilterInfo pf = new ProductFilterInfo();
					pf.FilterSessionId = FilterSessionId;
					pf.FilterSource = "Feature";
					pf.FilterValue = FeatureId.ToString() + val;
					pf.PortalId = PortalId;
					Controller.NewProductFilter(pf);
				}
			}
			else if (ctrl is RadioButtonList)
			{
				string val = "";
				RadioButtonList rbl = ctrl as RadioButtonList;
				foreach (ListItem item in rbl.Items)
				{
					if (item.Selected && item.Value != "0")
						val += "|" + item.Value;
				}
				if (val != string.Empty)
				{
					FeatureId = Convert.ToInt32(ctrl.ID.Substring(5));
					ProductFilterInfo pf = new ProductFilterInfo();
					pf.FilterSessionId = FilterSessionId;
					pf.FilterSource = "Feature";
					pf.FilterValue = FeatureId.ToString() + val;
					pf.PortalId = PortalId;
					Controller.NewProductFilter(pf);
				}
			}
			else
			{
				// must be checkboxes
				Control TableRow = ctrl.Parent.Parent as TableRow;
				TableCell tblCell = TableRow.Controls[0] as TableCell;
				string val = "";
				foreach (Control ctrlChk in tblCell.Controls)
				{
					CheckBox chk = ctrlChk as CheckBox;
					if (chk != null && chk.Checked)
					{
						string chkId = ((CheckBox)ctrlChk).ID;
						val += "|" + chkId.Substring(chkId.LastIndexOf("_") + 1);
					}
				}
				string featureId = ctrl.ID.Substring(5);
				featureId = featureId.Substring(0, featureId.IndexOf('_'));
				FeatureId = Convert.ToInt32(featureId);
				Controller.DeleteProductFilter(PortalId, FilterSessionId, "Feature", FeatureId.ToString());
				if (val != string.Empty)
				{
					ProductFilterInfo pf = new ProductFilterInfo();
					pf.FilterSessionId = FilterSessionId;
					pf.FilterSource = "Feature";
					pf.FilterValue = FeatureId.ToString() + val;
					pf.PortalId = PortalId;
					Controller.NewProductFilter(pf);
				}
			}

			Response.Redirect(Globals.NavigateURL(SearchTabId));
		}
		protected void cmdDeleteFeature_Click(object sender, ImageClickEventArgs e)
		{
			ImageButton cmd = sender as ImageButton;
			if (cmd != null)
			{
				int FeatureId = Convert.ToInt32(cmd.ID.Substring(5));
				Controller.DeleteProductFilter(PortalId, FilterSessionId, "Feature", FeatureId.ToString());
				Response.Redirect(Globals.NavigateURL(SearchTabId));
			}
		}

		#endregion

		#region "Render methods"
		private void RenderViewMode()
		{
			List<FeatureGridValueInfo> myValues = Controller.GetFeatureGridValues(PortalId, ProductId, CurrentLanguage, -1, FeatureGroupId );

			// Wir bauen die Ausgabe
			if (myValues.Count() > 0)
			{
				Table myTable = new Table();
				myTable.CellPadding = 0;
				myTable.CellSpacing = 0;
				myTable.BorderWidth = _BorderWidth;
				myTable.BorderColor = _BorderColor;
				myTable.BorderStyle = _BorderStyle;
				myTable.CssClass = FeatureGridCssClass;
				myTable.Width = new Unit(100, UnitType.Percentage);
				TableRow myRow;
				TableCell tblCell;
				int loop = 0;
				string LastFeatureGroup = "";
				string LastFeature = "";
				bool isAlternate = true;

				foreach (var loValue in myValues)
				{
					loop++;
					myRow = new TableRow();

					// FeatureGroup
					if (LastFeatureGroup != loValue.FeatureGroup)
					{
						isAlternate = true;
						tblCell = new TableHeaderCell();
						tblCell.ColumnSpan = 3;

						Label lblFeatureGroup = new Label();
						lblFeatureGroup.ID = "lblFeatureGroup" + loop.ToString();
						lblFeatureGroup.Text = loValue.FeatureGroup;
						tblCell.Controls.Add(lblFeatureGroup);

						myRow.CssClass = _GroupCssClass;

						myRow.Cells.Add(tblCell);
						myTable.Rows.Add(myRow);
						myRow = new TableRow();
					}

					// Feature
					tblCell = new TableCell();

					if (LastFeature != loValue.Feature)
					{
						isAlternate = !isAlternate;
						Label lblFeature = new Label();
						lblFeature.ID = "lblFeature" + loop.ToString();
						lblFeature.Text = loValue.Feature;
						lblFeature.CssClass = _FeatureCaptionCssClass;
						tblCell.Controls.Add(lblFeature);
					}
					else
					{
						tblCell.Controls.Add(new LiteralControl("&nbsp;"));
					}

					myRow.Cells.Add(tblCell);

					// FeatureValues
					tblCell = new TableCell();
					Label lbl = new Label();
					//lbl.ForeColor = _FeatureValueForeColor;

					switch (loValue.Datatype)
					{
						case "L":
							lbl.ID = "lblL" + loop.ToString();
							lbl.Text = String.Format("{0} {1}", loValue.FeatureListItem, loValue.Unit);
							tblCell.Controls.Add(lbl);
							break;
						case "I":
							lbl.ID = "lblI" + loop.ToString();
							lbl.Text = String.Format("{0} {1}", loValue.iValue, loValue.Unit);
							tblCell.Controls.Add(lbl);
							break;
						case "N":
							lbl.ID = "lblN" + loop.ToString();
							lbl.Text = String.Format("{0} {1}", loValue.nValue, loValue.Unit);
							tblCell.Controls.Add(lbl);
							break;
						case "F":
							lbl.ID = "lblF" + loop.ToString();
							lbl.Text = String.Format("{0} {1}", loValue.fValue, loValue.Unit);
							tblCell.Controls.Add(lbl);
							break;
						case "C":
							lbl.ID = "lblC" + loop.ToString();
							lbl.Text = String.Format("{0} {1}", loValue.cValue, loValue.Unit);
							tblCell.Controls.Add(lbl);
							break;
						case "T":
							lbl.ID = "lblT" + loop.ToString();
							lbl.Text = String.Format("{0} {1}", loValue.tValue, loValue.Unit);
							tblCell.Controls.Add(lbl);
							break;
						case "B":
							lbl.ID = "lblB" + loop.ToString();
							lbl.Text = String.Format("{0} {1}", loValue.bValue, loValue.Unit);
							tblCell.Controls.Add(lbl);
							break;
						default:
							tblCell.Controls.Add(new LiteralControl("&nbsp;"));
							break;
					}
					if (isAlternate)
						myRow.CssClass = _AlternateFeatureRowCssClass;
					else
						myRow.CssClass = _FeatureRowCssClass;
					
					myRow.Cells.Add(tblCell);

					myTable.Rows.Add(myRow);
					LastFeatureGroup = loValue.FeatureGroup;
					LastFeature = loValue.Feature;
				}
				PlaceHolder1.Controls.Add(myTable);
			}
		}
		private void RenderEditMode()
		{
			List<FeatureGridFeatureInfo> myFeatures = Controller.GetFeatureGridFeaturesByProduct(PortalId, ProductId, CurrentLanguage, -1, FeatureGroupId);

			// Wir bauen die Ausgabe
			if (myFeatures.Count > 0)
			{
				Table myTable = new Table();
                myTable.Attributes.Add("class","dnnGrid");
				TableRow myRow;
				TableCell tblCell;
				int loop = 0;
				string LastFeatureGroup = "";
				string LastFeature = "";
				bool isAlternate = true;

				foreach (var loValue in myFeatures)
				{
                    // FeatureGroup
					if (LastFeatureGroup != loValue.FeatureGroup)
					{
						TableHeaderCell tblHeaderCell = new TableHeaderCell();
                        tblHeaderCell.Attributes.Add("class","dnnGridHeader");
                        tblHeaderCell.ColumnSpan = 3;
						Label lblFeatureGroup = new Label();
						lblFeatureGroup.ID = "lblFeatureGroup" + loop.ToString();
						lblFeatureGroup.Text = loValue.FeatureGroup;
                        tblHeaderCell.Controls.Add(lblFeatureGroup);
                        myRow = new TableRow();
                        myRow.Cells.Add(tblHeaderCell);
						myTable.Rows.Add(myRow);
					}
                    else
                        loop++;

                    myRow = new TableRow();
                    
                    if (loop % 2 == 0)
                        myRow.Attributes.Add("class", "dnnGridItem");
                    else
                        myRow.Attributes.Add("class", "dnnGridAltItem");

					// Feature
					TableCell tblCaptionCell = new TableCell();

					if (LastFeature != loValue.Feature)
					{
						Label lblFeature = new Label();
						lblFeature.ID = "lblFeature" + loop.ToString();
						lblFeature.Text = loValue.Feature;
                        tblCaptionCell.Controls.Add(lblFeature);
					}
					else
					{
                        tblCaptionCell.Controls.Add(new LiteralControl("&nbsp;"));
					}
                    tblCaptionCell.Style.Add("vertical-align", "top");

					// FeatureValues
					tblCell = new TableCell();
				    bool doAdd = true;
					switch (loValue.Datatype)
					{
						case "L":
							List<FeatureListItemInfo> myList = Controller.GetFeatureListItemsByListAndProduct(Convert.ToInt32(loValue.FeatureListId), ProductId, CurrentLanguage);
							switch (loValue.Control)
							{
								case "checkbox":
									int i = 0;
									CheckBox chk;
									foreach (var myListitem in myList)
									{
										i++;
										chk = new CheckBox();
										chk.Text = myListitem.FeatureListItem;
										chk.ID = "ctrlL" + loValue.FeatureId.ToString() + "_" +
											loValue.FeatureListId.ToString() + "_" +
											myListitem.FeatureListItemId.ToString();
										chk.Style.Add("margin-left", "1em");
										tblCell.Controls.Add(chk);
										if (i % loValue.Dimension == 0)
										{
											tblCell.Controls.Add(new LiteralControl("<br/>"));
										}
									}
									break;
								case "combobox":
									DropDownList cbo = new DropDownList();
									cbo.ID = "ctrlL" + loValue.FeatureId.ToString() + "_" +
										loValue.FeatureListId.ToString();
									cbo.DataSource = myList;
									cbo.DataTextField = "FeatureListItem";
									cbo.DataValueField = "FeatureListItemId";
									cbo.Items.Add(new ListItem { Text = "(Bitte auswählen)", Value = "0" });
									cbo.AppendDataBoundItems = true;
									cbo.DataBind();
									cbo.Style.Add("margin-left", "1em");
									tblCell.Controls.Add(cbo);
							        if (myList.Count == 0)
							            doAdd = false;
									break;
								case "listbox":
									ListBox lst = new ListBox();
									lst.ID = "ctrlL" + loValue.FeatureId.ToString() + "_" +
										loValue.FeatureListId.ToString();
									lst.DataSource = myList;
									lst.DataTextField = "FeatureListItem";
									lst.DataValueField = "FeatureListItemId";
									//lst.Items.Add(new ListItem { Text = "(Bitte auswählen)", Value = "0" });
									//lst.AppendDataBoundItems = true;
									if (loValue.Dimension > 0) 
										lst.Rows = loValue.Dimension;
									if (loValue.Multiselect == true) 
										lst.SelectionMode = ListSelectionMode.Multiple;
									lst.DataBind();
									lst.Style.Add("margin-left", "1em");
									tblCell.Controls.Add(lst);
									break;
								case "radiobutton":
									RadioButtonList rbl = new RadioButtonList();
									rbl.ID = "ctrlL" + loValue.FeatureId.ToString() + "_" +
										loValue.FeatureListId.ToString();
									rbl.DataTextField = "FeatureListItem";
									rbl.DataValueField = "FeatureListItemId";
									rbl.DataSource = myList;
									rbl.DataBind();
									rbl.Style.Add("padding-left", "1em");
									tblCell.Controls.Add(rbl);
									break;
							}
							break;
						case "I":
							TextBox txtI = new TextBox();
							txtI.ID = "ctrlI" + loValue.FeatureId.ToString();
							txtI.Style.Add("margin-left", "1em");
							if (loValue.Dimension > 0) 
								txtI.Columns = (int)loValue.Dimension;
							tblCell.Controls.Add(txtI);
							AddValidators(tblCell, txtI.ID, ValidationDataType.Integer, loValue.MinValue, loValue.MaxValue, loValue.RegEx, loValue.Required);
							break;
						case "N":
							TextBox txtN = new TextBox();
							txtN.ID = "ctrlN" + loValue.FeatureId.ToString();
							txtN.Style.Add("margin-left", "1em");
							if (loValue.Dimension > 0) 
								txtN.Columns = (int)loValue.Dimension;
							tblCell.Controls.Add(txtN);
							AddValidators(tblCell, txtN.ID, ValidationDataType.Double, loValue.MinValue, loValue.MaxValue, loValue.RegEx, loValue.Required);
							break;
						case "F":
							TextBox txtF = new TextBox();
							txtF.ID = "ctrlF" + loValue.FeatureId.ToString();
							txtF.Style.Add("margin-left", "1em");
							if (loValue.Dimension > 0) 
								txtF.Columns = (int)loValue.Dimension;
							tblCell.Controls.Add(txtF);
							AddValidators(tblCell, txtF.ID, ValidationDataType.Double, loValue.MinValue, loValue.MaxValue, loValue.RegEx, loValue.Required);
							break;
						case "C":
							TextBox txtC = new TextBox();
							txtC.ID = "ctrlC" + loValue.FeatureId.ToString();
							txtC.Style.Add("margin-left", "1em");
							if (loValue.Dimension > 0)
								txtC.Columns = (int)loValue.Dimension;
							tblCell.Controls.Add(txtC);
							AddValidators(tblCell, txtC.ID, ValidationDataType.String, loValue.MinValue, loValue.MaxValue, loValue.RegEx, loValue.Required);
							break;
						case "T":
							TextBox txtT = new TextBox();
							txtT.ID = "ctrlT" + loValue.FeatureId.ToString();
							txtT.Style.Add("margin-left", "1em");
							tblCell.Controls.Add(txtT);
							if (loValue.Dimension > 0) 
								txtT.Columns = (int)loValue.Dimension;
							AddValidators(tblCell, txtT.ID, ValidationDataType.Date, loValue.MinValue, loValue.MaxValue, loValue.RegEx, loValue.Required);
							break;
						case "B":
							CheckBox chkB = new CheckBox();
							chkB.ID = "ctrlB" + loValue.FeatureId.ToString();
							chkB.Style.Add("margin-left", "1em");
							tblCell.Controls.Add(chkB);
							break;
						default:
							tblCell.Controls.Add(new LiteralControl("&nbsp;"));
							break;
					}
				    if (doAdd)
				    {
				        myRow.Cells.Add(tblCaptionCell);
				        myRow.Cells.Add(tblCell);
                        myTable.Rows.Add(myRow);
				    }
				    
					LastFeatureGroup = loValue.FeatureGroup;
					LastFeature = loValue.Feature;
				}
				PlaceHolder1.Controls.Add(myTable);

				SetControlValues();
			}
		}
		private void RenderSearchMode()
		{
			List<FeatureGridFeatureInfo> featureGridFeatures = Controller.GetFeatureGridFeaturesByProductGroup(PortalId, ProductGroupId,
			                                                                                          CurrentLanguage, -1, -1,
			                                                                                          true);
			List<ProductFilterInfo> fl = Controller.GetProductFilter(PortalId, FilterSessionId, "Feature");
			var selectedFeatures = from f in fl
			               select new
			                      	{
			                      		FeatureId = Convert.ToInt32(f.FilterValue.Substring(0, f.FilterValue.IndexOf('|'))),
			                      		Values = f.FilterValue.Substring(f.FilterValue.IndexOf('|') + 1)
			                      	};

			// Wir bauen die Ausgabe
			if (featureGridFeatures.Count > 0)
			{
				Table myTable = new Table();
				myTable.CellPadding = 0;
				myTable.CellSpacing = 0;
				myTable.Width = new Unit(100, UnitType.Percentage);
				TableRow myRow, myCaptionRow;
				TableCell tblCell;
				int loop = 0;
			    string lastFeature = "";

				foreach (var loValue in featureGridFeatures)
				{
					loop++;
					int valCount = 1;
					myCaptionRow = new TableRow();

					// Feature
					TableCell tblCaptionCell = new TableCell();

					if (lastFeature != loValue.Feature)
					{
						Label lblFeature = new Label();
						lblFeature.ID = "lblFeature" + loop.ToString();
						lblFeature.Text = loValue.Feature;
						lblFeature.Attributes.Add("class", "bbstore-search-title");
						tblCaptionCell.Controls.Add(lblFeature);
					}
					else
					{
						tblCaptionCell.Controls.Add(new LiteralControl("&nbsp;"));
					}
					tblCaptionCell.Style.Add("vertical-align", "top");
					tblCaptionCell.Attributes.Add("colspan", "2");
					myCaptionRow.Cells.Add(tblCaptionCell);

					// FeatureValues
					myRow = new TableRow();
					tblCell = new TableCell();

					switch (loValue.Datatype)
					{
						case "L":
							List<FeatureListItemInfo> featureListItems =
								Controller.GetFeatureListItemsByListAndProductGroup(Convert.ToInt32(loValue.FeatureListId), ProductGroupId,
								                                                    CurrentLanguage);
							valCount = featureListItems.Count();

							var featureL = (from f in selectedFeatures where f.FeatureId == loValue.FeatureId select f);
							string[] selListItemIds;
							string selText = "";

							foreach (var featureListItem in featureL)
							{
								selListItemIds = featureListItem.Values.Split('|');
								foreach (string selId in selListItemIds)
								{
									int id = Convert.ToInt32(selId);
									var query = from li in featureListItems where li.FeatureListItemId == id select li;
									if (query.Any())
										selText += query.First().FeatureListItem + ", ";
								}
								if (selText.EndsWith(", "))
									selText = selText.Substring(0, selText.Length - 2);
							}

							switch (loValue.Control)
							{
								case "checkbox":
									int i = 0;
									CheckBox chk;

							        if (featureL.Count() == 0 || loValue.Multiselect)
							        {
							            foreach (var myListitem in featureListItems)
							            {
							                i++;
							                chk = new CheckBox();
							                chk.Text = myListitem.FeatureListItem;
							                chk.CssClass = "bbstore-search-select";
							                chk.ID = "ctrlL" + loValue.FeatureId.ToString() + "_" +
							                         loValue.FeatureListId.ToString() + "_" +
							                         myListitem.FeatureListItemId.ToString();
							                if (featureL.Count() > 0)
							                {
							                    selListItemIds = featureL.First().Values.Split('|');
							                    foreach (string selId in selListItemIds)
							                    {
							                        if (Convert.ToInt32(selId) == myListitem.FeatureListItemId)
							                        {
							                            chk.Checked = true;
							                            break;
							                        }
							                    }
							                }
							                chk.CheckedChanged += new EventHandler(cmdSearchFeature_Click);
							                chk.AutoPostBack = true;
							                tblCell.Controls.Add(chk);
							                if (i%loValue.Dimension == 0)
							                    tblCell.Controls.Add(new LiteralControl("<br/>"));
							            }
							        }
							        else
							        {
							            chk = new CheckBox();
							            foreach (var myListitem in featureListItems)
							            {
							                i++;

							                if (featureL.Count() > 0)
							                {
							                    selListItemIds = featureL.First().Values.Split('|');
							                    foreach (string selId in selListItemIds)
							                    {
							                        if (Convert.ToInt32(selId) == myListitem.FeatureListItemId)
							                        {

							                            chk.Text = myListitem.FeatureListItem;
							                            chk.CssClass = "bbstore-search-value";
							                            chk.ID = "ctrlL" + loValue.FeatureId.ToString() + "_" +
							                                     loValue.FeatureListId.ToString() + "_" +
							                                     myListitem.FeatureListItemId.ToString();
							                            chk.Checked = true;
                                                        chk.CheckedChanged += new EventHandler(cmdSearchFeature_Click);
							                            chk.AutoPostBack = true;
							                            tblCell.Controls.Add(chk);
							                            break;
							                        }
							                    }
							                }
							            }
							        }
							        myRow.Cells.Add(tblCell);

									tblCell = new TableCell();
									tblCell.Width = new Unit(16);
									if (featureL.Count() > 1)
									{
										ImageButton cmdDLChk = new ImageButton();
										cmdDLChk.ID = "cmdDL" + loValue.FeatureId.ToString();
										cmdDLChk.ImageUrl = "~/images/Delete.gif";
										cmdDLChk.Click += new ImageClickEventHandler(cmdDeleteFeature_Click);
										tblCell.Controls.Add(cmdDLChk);
									}
									myRow.Cells.Add(tblCell);
									break;
								case "combobox":
									if (String.IsNullOrEmpty(selText))  
									{
										DropDownList cbo = new DropDownList();
										cbo.ID = "ctrlL" + loValue.FeatureId.ToString();
										cbo.DataSource = featureListItems;
									    cbo.CssClass = "bbstore-search-select";
										cbo.DataTextField = "FeatureListItem";
										cbo.DataValueField = "FeatureListItemId";
										cbo.Items.Add(new ListItem {Text = Localization.GetString("Select.Text", this.LocalResourceFile), Value = "0"});
										cbo.AppendDataBoundItems = true;
										cbo.AutoPostBack = true;
										cbo.SelectedIndexChanged += new EventHandler(cmdSearchFeature_Click);
										cbo.DataBind();
										cbo.Width = new Unit(100, UnitType.Percentage);
										tblCell.Controls.Add(cbo);
										myRow.Cells.Add(tblCell);

										tblCell = new TableCell();
										tblCell.Width = new Unit(16);
										myRow.Cells.Add(tblCell);
									}
									else
									{
										Label lblLCbo = new Label();
										lblLCbo.Attributes.Add("class", "bbstore-search-value");
										lblLCbo.ID = lblLCbo + loValue.FeatureId.ToString();
										lblLCbo.Text = selText;
										tblCell.Controls.Add(lblLCbo);
										myRow.Cells.Add(tblCell);

										tblCell = new TableCell();
										tblCell.Width = new Unit(16);
										ImageButton cmdDLCbo = new ImageButton();
										cmdDLCbo.ID = "cmdDL" + loValue.FeatureId.ToString();
										cmdDLCbo.ImageUrl = "~/images/Delete.gif";
										cmdDLCbo.Click += new ImageClickEventHandler(cmdDeleteFeature_Click);
										tblCell.Controls.Add(cmdDLCbo);
										myRow.Cells.Add(tblCell);
									}
									break;

								case "listbox":
									if (featureL.Count() == 0)
									{
										ListBox lst = new ListBox();
										lst.ID = "ctrlL" + loValue.FeatureId.ToString();
										lst.DataSource = featureListItems;
                                        lst.CssClass = "bbstore-search-select";
										lst.DataTextField = "FeatureListItem";
										lst.DataValueField = "FeatureListItemId";
										//lst.Items.Add(new ListItem { Text = Localization.GetString("Select.Text",this.LocalResourceFile), Value = "0" });
										//lst.AppendDataBoundItems = true;
										if (loValue.Dimension > 0)
											lst.Rows = loValue.Dimension;
										if (loValue.Multiselect == true)
											lst.SelectionMode = ListSelectionMode.Multiple;
										lst.TextChanged += new EventHandler(cmdSearchFeature_Click);
										lst.Attributes.Add("onBlur", "__doPostBack('" + lst.ID + "','');");
										lst.DataBind();
										lst.Width = new Unit(100, UnitType.Percentage);
										tblCell.Controls.Add(lst);
										myRow.Cells.Add(tblCell);

										tblCell = new TableCell();
										tblCell.Width = new Unit(16);
										myRow.Cells.Add(tblCell);
									}
									else
									{
										Label lblLLst = new Label();
										lblLLst.Attributes.Add("class", "bbstore-search-value");
										lblLLst.ID = lblLLst + loValue.FeatureId.ToString();
										lblLLst.Text = selText;
										tblCell.Controls.Add(lblLLst);
										myRow.Cells.Add(tblCell);

										tblCell = new TableCell();
										tblCell.Width = new Unit(16);
										ImageButton cmdDLLst = new ImageButton();
										cmdDLLst.ID = "cmdDL" + loValue.FeatureId.ToString();
										cmdDLLst.ImageUrl = "~/images/Delete.gif";
										cmdDLLst.Click += new ImageClickEventHandler(cmdDeleteFeature_Click);
										tblCell.Controls.Add(cmdDLLst);
										myRow.Cells.Add(tblCell);
									}
									break;
								case "radiobutton":
									if (featureL.Count() == 0)
									{
										RadioButtonList rbl = new RadioButtonList();
										rbl.ID = "ctrlL" + loValue.FeatureId.ToString();
                                        rbl.CssClass = "bbstore-search-select";
										rbl.DataTextField = "FeatureListItem";
										rbl.DataValueField = "FeatureListItemId";
										rbl.DataSource = featureListItems;
										rbl.DataBind();
										rbl.SelectedIndexChanged += new EventHandler(cmdSearchFeature_Click);
										rbl.AutoPostBack = true;
										tblCell.Controls.Add(rbl);
										tblCell.Attributes.Add("colspan", "2");
										myRow.Cells.Add(tblCell);

										tblCell = new TableCell();
										tblCell.Width = new Unit(16);
										myRow.Cells.Add(tblCell);
									}
									else
									{
										Label lblLRdb = new Label();
										lblLRdb.Attributes.Add("class", "bbstore-search-value");
										lblLRdb.ID = lblLRdb + loValue.FeatureId.ToString();
										lblLRdb.Text = selText;
										tblCell.Controls.Add(lblLRdb);
										myRow.Cells.Add(tblCell);

										tblCell = new TableCell();
										tblCell.Width = new Unit(16);
										ImageButton cmdDLRdb = new ImageButton();
										cmdDLRdb.ID = "cmdDL" + loValue.FeatureId.ToString();
										cmdDLRdb.ImageUrl = "~/images/Delete.gif";
										cmdDLRdb.Click += new ImageClickEventHandler(cmdDeleteFeature_Click);
										tblCell.Controls.Add(cmdDLRdb);
										myRow.Cells.Add(tblCell);
									}
									break;
							}
							break;
						case "I":
							var featureI = (from f in selectedFeatures where f.FeatureId == loValue.FeatureId select f);
							if (featureI.Count() == 0)
							{
								TextBox txtI = new TextBox();
                                txtI.CssClass = "bbstore-search-select";
								txtI.ID = "ctrlI" + loValue.FeatureId.ToString();
								if (loValue.Dimension > 0)
									txtI.Columns = (int) loValue.Dimension;
								txtI.Width = new Unit(100, UnitType.Percentage);
								txtI.TextChanged += new EventHandler(cmdSearchFeature_Click);
								txtI.AutoPostBack = true;
								tblCell.Controls.Add(txtI);
								AddValidators(tblCaptionCell, txtI.ID, ValidationDataType.Integer, loValue.MinValue, loValue.MaxValue,
								              loValue.RegEx, false);
								myRow.Cells.Add(tblCell);

								tblCell = new TableCell();
								tblCell.Width = new Unit(16);
								myRow.Cells.Add(tblCell);
							}
							else
							{
								Label lblI = new Label();
								lblI.Attributes.Add("class", "bbstore-search-value");
								lblI.ID = lblI + loValue.FeatureId.ToString();
								lblI.Text = featureI.First().Values;
								tblCell.Controls.Add(lblI);
								myRow.Cells.Add(tblCell);

								tblCell = new TableCell();
								tblCell.Width = new Unit(16);
								ImageButton cmdDI = new ImageButton();
								cmdDI.ID = "cmdDI" + loValue.FeatureId.ToString();
								cmdDI.ImageUrl = "~/images/Delete.gif";
								cmdDI.Click += new ImageClickEventHandler(cmdDeleteFeature_Click);
								tblCell.Controls.Add(cmdDI);
								myRow.Cells.Add(tblCell);
							}
							break;
						case "N":
							var featureN = (from f in selectedFeatures where f.FeatureId == loValue.FeatureId select f);
							if (featureN.Count() == 0)
							{
								TextBox txtN = new TextBox();
                                txtN.CssClass = "bbstore-search-select";
								txtN.ID = "ctrlN" + loValue.FeatureId.ToString();
								if (loValue.Dimension > 0)
									txtN.Columns = (int) loValue.Dimension;
								txtN.Width = new Unit(100, UnitType.Percentage);
								txtN.TextChanged += new EventHandler(cmdSearchFeature_Click);
								txtN.AutoPostBack = true;

								tblCell.Controls.Add(txtN);
								AddValidators(tblCaptionCell, txtN.ID, ValidationDataType.Double, loValue.MinValue, loValue.MaxValue,
								              loValue.RegEx, false);
								myRow.Cells.Add(tblCell);

								tblCell = new TableCell();
								tblCell.Width = new Unit(16);
								myRow.Cells.Add(tblCell);
							}
							else
							{
								Label lblN = new Label();
								lblN.Attributes.Add("class", "bbstore-search-value");
								lblN.ID = lblN + loValue.FeatureId.ToString();
								lblN.Text = featureN.First().Values;
								tblCell.Controls.Add(lblN);
								myRow.Cells.Add(tblCell);

								tblCell = new TableCell();
								tblCell.Width = new Unit(16);
								ImageButton cmdDN = new ImageButton();
								cmdDN.ID = "cmdDN" + loValue.FeatureId.ToString();
								cmdDN.ImageUrl = "~/images/Delete.gif";
								cmdDN.Click += new ImageClickEventHandler(cmdDeleteFeature_Click);
								tblCell.Controls.Add(cmdDN);
								myRow.Cells.Add(tblCell);
							}

							break;
						case "F":
							var featureF = (from f in selectedFeatures where f.FeatureId == loValue.FeatureId select f);
							if (featureF.Count() == 0)
							{
								TextBox txtF = new TextBox();
                                txtF.CssClass = "bbstore-search-select";
								txtF.ID = "ctrlF" + loValue.FeatureId.ToString();
								if (loValue.Dimension > 0)
									txtF.Columns = (int) loValue.Dimension;
								txtF.Width = new Unit(100, UnitType.Percentage);
								txtF.TextChanged += new EventHandler(cmdSearchFeature_Click);
								txtF.AutoPostBack = true;

								tblCell.Controls.Add(txtF);
								AddValidators(tblCaptionCell, txtF.ID, ValidationDataType.Double, loValue.MinValue, loValue.MaxValue,
								              loValue.RegEx, false);
								myRow.Cells.Add(tblCell);

								tblCell = new TableCell();
								tblCell.Width = new Unit(16);
								myRow.Cells.Add(tblCell);
							}
							else
							{
								Label lblF = new Label();
								lblF.Attributes.Add("class", "bbstore-search-value");
								lblF.ID = lblF + loValue.FeatureId.ToString();
								lblF.Text = featureF.First().Values;
								tblCell.Controls.Add(lblF);
								myRow.Cells.Add(tblCell);

								tblCell = new TableCell();
								tblCell.Width = new Unit(16);
								ImageButton cmdDF = new ImageButton();
								cmdDF.ID = "cmdDF" + loValue.FeatureId.ToString();
								cmdDF.ImageUrl = "~/images/Delete.gif";
								cmdDF.Click += new ImageClickEventHandler(cmdDeleteFeature_Click);
								tblCell.Controls.Add(cmdDF);
								myRow.Cells.Add(tblCell);
							}

							break;
						case "C":
							var featureC = (from f in selectedFeatures where f.FeatureId == loValue.FeatureId select f);
							if (featureC.Count() == 0)
							{
								TextBox txtC = new TextBox();
                                txtC.CssClass = "bbstore-search-select";
								txtC.ID = "ctrlC" + loValue.FeatureId.ToString();
								if (loValue.Dimension > 0)
									txtC.Columns = (int) loValue.Dimension;
								txtC.Width = new Unit(100, UnitType.Percentage);
								txtC.TextChanged += new EventHandler(cmdSearchFeature_Click);
								txtC.AutoPostBack = true;

								tblCell.Controls.Add(txtC);
								AddValidators(tblCaptionCell, txtC.ID, ValidationDataType.String, loValue.MinValue, loValue.MaxValue,
								              loValue.RegEx, false);
								myRow.Cells.Add(tblCell);

								tblCell = new TableCell();
								tblCell.Width = new Unit(16);
								myRow.Cells.Add(tblCell);
							}
							else
							{
								Label lblC = new Label();
								lblC.Attributes.Add("class", "bbstore-search-value");
								lblC.ID = lblC + loValue.FeatureId.ToString();
								lblC.Text = featureC.First().Values;
								tblCell.Controls.Add(lblC);
								myRow.Cells.Add(tblCell);

								tblCell = new TableCell();
								tblCell.Width = new Unit(16);
								ImageButton cmdDC = new ImageButton();
								cmdDC.ID = "cmdDC" + loValue.FeatureId.ToString();
								cmdDC.ImageUrl = "~/images/Delete.gif";
								cmdDC.Click += new ImageClickEventHandler(cmdDeleteFeature_Click);
								tblCell.Controls.Add(cmdDC);
								myRow.Cells.Add(tblCell);
							}
							break;
						case "T":
							var featureT = (from f in selectedFeatures where f.FeatureId == loValue.FeatureId select f);
							if (featureT.Count() == 0)
							{
								TextBox txtT = new TextBox();
                                txtT.CssClass = "bbstore-search-select";
								txtT.ID = "ctrlT" + loValue.FeatureId.ToString();
								if (loValue.Dimension > 0)
									txtT.Columns = (int) loValue.Dimension;
								txtT.Width = new Unit(100, UnitType.Percentage);
								txtT.TextChanged += new EventHandler(cmdSearchFeature_Click);
								txtT.AutoPostBack = true;

								tblCell.Controls.Add(txtT);
								AddValidators(tblCaptionCell, txtT.ID, ValidationDataType.Date, loValue.MinValue, loValue.MaxValue,
								              loValue.RegEx, false);
								myRow.Cells.Add(tblCell);

								tblCell = new TableCell();
								tblCell.Width = new Unit(16);
								//ImageButton cmdST = new ImageButton();
								//cmdST.ID = "cmdST" + loValue.FeatureId.ToString();
								//cmdST.ImageUrl = "~/images/action_source.gif";
								//cmdST.Click += new ImageClickEventHandler(cmdSearchFeature_Click);
								//tblCell.Controls.Add(cmdST);
								myRow.Cells.Add(tblCell);
							}
							else
							{
								Label lblT = new Label();
								lblT.Attributes.Add("class", "bbstore-search-value");
								lblT.ID = lblT + loValue.FeatureId.ToString();
								lblT.Text = featureT.First().Values;
								tblCell.Controls.Add(lblT);
								myRow.Cells.Add(tblCell);

								tblCell = new TableCell();
								tblCell.Width = new Unit(16);
								ImageButton cmdDT = new ImageButton();
								cmdDT.ID = "cmdDT" + loValue.FeatureId.ToString();
								cmdDT.ImageUrl = "~/images/Delete.gif";
								cmdDT.Click += new ImageClickEventHandler(cmdDeleteFeature_Click);
								tblCell.Controls.Add(cmdDT);
								myRow.Cells.Add(tblCell);
							}

							break;
						default:
							tblCell.Controls.Add(new LiteralControl("&nbsp;"));
							myRow.Cells.Add(tblCell);
							tblCell = new TableCell();
							myRow.Cells.Add(tblCell);
							break;
					}
					if (valCount > 0)
					{
						myTable.Rows.Add(myCaptionRow);
						myTable.Rows.Add(myRow);
					}
				    lastFeature = loValue.Feature;
				}
				PlaceHolder1.Controls.Add(myTable);
			}
		}

		private void SetControlValues()
		{
			List<FeatureGridValueInfo> myValues = Controller.GetFeatureGridValues(PortalId, ProductId, CurrentLanguage, -1, FeatureGroupId);

			if (myValues.Count > 0)
			{
				foreach (var loValue in myValues)
				{
					switch (loValue.Datatype)
					{
						case "L":
							string ControlID = "ctrlL" + loValue.FeatureId.ToString() + "_" +
								loValue.FeatureListId.ToString();
							switch (loValue.Control)
							{
								case "checkbox":
									CheckBox chk = (CheckBox)FindControl(ControlID + "_" + loValue.FeatureListItemId.ToString());
									if (chk != null)
										chk.Checked = true;
									break;
								case "combobox":
									DropDownList cbo = (DropDownList)FindControl(ControlID);
									if (cbo != null)
									{
										foreach (ListItem loItem in cbo.Items)
										{
											if (loItem.Value == loValue.FeatureListItemId.ToString())
											{
												loItem.Selected = true;
											}
										}
									}
									break;
								case "listbox":
									ListBox lst = (ListBox)FindControl(ControlID);
									if (lst != null)
									{
										foreach (ListItem loItem in lst.Items)
										{
											if (loItem.Value == loValue.FeatureListItemId.ToString())
											{
												loItem.Selected = true;
											}
										}
									}
									break;
								case "radiobutton":
									RadioButtonList rbl = (RadioButtonList)FindControl(ControlID);
									if (rbl != null)
									{
										foreach (ListItem loItem in rbl.Items)
										{
											if (loItem.Value == loValue.FeatureListItemId.ToString())
											{
												loItem.Selected = true;
											}
										}
									}
									break;
							}

							break;
						case "I":
							TextBox txtI = (TextBox)FindControl("ctrlI" + loValue.FeatureId.ToString());
							txtI.Text = loValue.iValue.ToString();
							break;
						case "N":
							TextBox txtN = (TextBox)FindControl("ctrlN" + loValue.FeatureId.ToString());
							txtN.Text = loValue.nValue.ToString();
							break;
						case "F":
							TextBox txtF = (TextBox)FindControl("ctrlF" + loValue.FeatureId.ToString());
							txtF.Text = loValue.fValue.ToString();
							break;
						case "C":
							TextBox txtC = (TextBox)FindControl("ctrlC" + loValue.FeatureId.ToString());
							txtC.Text = loValue.cValue;
							break;
						case "T":
							TextBox txtT = (TextBox)FindControl("ctrlT" + loValue.FeatureId.ToString());
							txtT.Text = loValue.tValue.ToString();
							break;
						case "B":
							CheckBox chkB = (CheckBox)FindControl("ctrlB" + loValue.FeatureId.ToString());
							chkB.Checked = loValue.bValue ?? false;
							break;

					}
				}
			}
		}
		#endregion

		#region "Helper methods"
		protected void AddValidators(TableCell Cell, string ControlID, ValidationDataType Type, string MinValue, string MaxValue, string RegEx, bool Required)
		{
			if (Required == true)
			{
				RequiredFieldValidator rvali = new RequiredFieldValidator();
				rvali.ControlToValidate = ControlID;
				rvali.ErrorMessage = Localization.GetString("FillData.Error",this.LocalResourceFile);
				Cell.Controls.Add(rvali);
			}

			if (MinValue != null && MaxValue != null)
			{
				RangeValidator gvali = new RangeValidator();
				gvali.Type = ValidationDataType.Double;
				gvali.MinimumValue = Double.Parse(MinValue).ToString();
				gvali.MaximumValue = Double.Parse(MaxValue).ToString();
				gvali.ControlToValidate = ControlID;
				gvali.ErrorMessage = gvali.MinimumValue + ".." + gvali.MaximumValue;
				Cell.Controls.Add(gvali);
			}
			else if (RegEx != null)
			{
				RegularExpressionValidator xvali = new RegularExpressionValidator();
				xvali.ValidationExpression = RegEx;
				xvali.ControlToValidate = ControlID;
				xvali.ErrorMessage = Localization.GetString("ValidFormat.Error", this.LocalResourceFile);
				Cell.Controls.Add(xvali);
			}
		}
		protected ArrayList GetControls(ArrayList myControls, Control BaseControl)
		{
			if (BaseControl.ID != null && BaseControl.ID.Substring(0, 4) == "ctrl")
			{
				myControls.Add(BaseControl);
			}
			if (BaseControl.HasControls())
			{
				foreach (Control loControl in BaseControl.Controls)
				{
					myControls = GetControls(myControls, loControl);
				}
			}

			return myControls;
		}
		public void SaveFeatures()
		{
			ArrayList myControls = GetControls(new ArrayList(), PlaceHolder1);
			if (myControls.Count > 0)
			{
				// Delete all FeatureValues of ProductId
				Controller.DeleteFeatureValuesByProductId(ProductId,FeatureGroupId);

				string controldataType;
				string[] controlIDs;
				FeatureValueInfo newValue;

				foreach (Control loControl in myControls)
				{
					controldataType = loControl.ID.Substring(4, 1);
					controlIDs = loControl.ID.Substring(5).Split('_');

					if (loControl is TextBox)
					{
						TextBox myTextBox = (TextBox)loControl;
						if (myTextBox.Text != String.Empty)
						{
							newValue = new FeatureValueInfo();
							newValue.FeatureId = Int32.Parse(controlIDs[0]);
							newValue.ProductId = ProductId;

							switch (controldataType)
							{
								case "I":
									int iResult;
									if (Int32.TryParse(myTextBox.Text, out iResult))
										newValue.iValue = iResult;
									break;
								case "N":
									Double nResult;
									if (Double.TryParse(myTextBox.Text, out nResult)) 
										newValue.nValue = (Decimal)nResult;
									break;
								case "F":
									double fResult;
									if (double.TryParse(myTextBox.Text, out fResult)) 
										newValue.fValue = (double)fResult;
									break;
								case "C":
									newValue.cValue = myTextBox.Text;
									break;
								case "T":
									DateTime tResult;
									if (DateTime.TryParse(myTextBox.Text, out tResult)) 
										newValue.tValue = tResult;
									break;
								default:
									break;
							}
							Controller.NewFeatureValue(newValue);
						}
					}

					else if (loControl is CheckBox)
					{
						CheckBox myCheckBox = (CheckBox) loControl;

						newValue = new FeatureValueInfo();
						newValue.FeatureId = Int32.Parse(controlIDs[0]);
						newValue.ProductId = ProductId;

						if (controldataType == "B")
						{
							newValue.bValue = myCheckBox.Checked;
							Controller.NewFeatureValue(newValue);
						}
						else
						{
							if (myCheckBox.Checked)
							{
								newValue.FeatureListItemId = Int32.Parse(controlIDs[2]);
								Controller.NewFeatureValue(newValue);
							}
						}
					}
					else if (loControl is ListBox)
					{
						ListBox myListbox = (ListBox)loControl;
						foreach (ListItem loItem in myListbox.Items)
						{
							if (loItem.Selected)
							{
								newValue = new FeatureValueInfo();
								newValue.FeatureId = Int32.Parse(controlIDs[0]);
								newValue.ProductId = ProductId;
								newValue.FeatureListItemId = Int32.Parse(loItem.Value);
								Controller.NewFeatureValue(newValue);
							}

						}
					}
					else if (loControl is DropDownList)
					{
						DropDownList myDropDownList = (DropDownList)loControl;
						foreach (ListItem loItem in myDropDownList.Items)
						{
							if (loItem.Selected && loItem.Value != "0")
							{
								newValue = new FeatureValueInfo();
								newValue.FeatureId = Int32.Parse(controlIDs[0]);
								newValue.ProductId = ProductId;
								newValue.FeatureListItemId = Int32.Parse(loItem.Value);
								Controller.NewFeatureValue(newValue);
							}

						}
					}
					else if (loControl is RadioButtonList)
					{
						RadioButtonList myRadioButtonList = (RadioButtonList)loControl;
						foreach (ListItem loItem in myRadioButtonList.Items)
						{
							if (loItem.Selected)
							{
								newValue = new FeatureValueInfo();
								newValue.FeatureId = Int32.Parse(controlIDs[0]);
								newValue.ProductId = ProductId;
								newValue.FeatureListItemId = Int32.Parse(loItem.Value);
								Controller.NewFeatureValue(newValue);
							}

						}
					}
				}
			}
		}
		#endregion
	}
}

