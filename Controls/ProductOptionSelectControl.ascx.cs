using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Bitboxx.DNNModules.BBStore.Controls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;

namespace Bitboxx.DNNModules.BBStore
{
	public partial class ProductOptionSelectControl : PortalModuleBase
	{
		private class AreaInfo
		{
            public AreaInfo()
            {
                Dimension1Name = "";
                Dimension1Min = 0;
                Dimension1Max = 99999999m;
                Dimension1Unit = "";
                Dimension2Name = "";
                Dimension2Min = 0;
                Dimension2Max = 99999999m;
                Dimension2Unit = "";
                ResultName = "";
                ResultFactor = 1;
                ResultUnit = "";
            }
            public string Dimension1Name { get; set;}
            public decimal Dimension1Min { get; set; }
            public decimal Dimension1Max { get; set; }
            public string Dimension1Unit { get; set; }
            public string Dimension2Name { get; set; }
            public decimal Dimension2Min{ get; set;}
            public decimal Dimension2Max { get; set; }
            public string Dimension2Unit { get; set; }
            public string ResultName { get; set; }
            public decimal ResultFactor { get; set; }
            public string ResultUnit { get; set; }
		}
        
        private ViewProduct _productModule;
		public ViewProduct ProductModule
		{
			get { return _productModule; }
			set { _productModule = value; }
		}

		private bool _showNetPrice;
		public bool ShowNetPrice
		{
			get { return _showNetPrice; }
			set { _showNetPrice = value; }
		}

		private SimpleProductInfo _product;
		public SimpleProductInfo Product
		{ 
			get { return _product; }
			set { _product = value; }
		}	
		public List<OptionListInfo> ProductOptions { get; set; }

		private List<OptionListInfo> _selectedOptions = new List<OptionListInfo>();
		public List<OptionListInfo> SelectedOptions
		{
			get
			{
				if (_selectedOptions.Count > 0)
					return _selectedOptions;

				foreach (ListViewItem lvi in ProductOptionListView.Items)
				{
					Label lblOption = (Label)lvi.FindControl("lblOption");
					DropDownList cboOptionValue = (DropDownList)lvi.FindControl("cboOptionValue");
					TextBox txtOptionvalue = (TextBox)lvi.FindControl("txtOptionvalue");
					TextBox txtOptionDescription = (TextBox)lvi.FindControl("txtOptionDescription");
                    TextBox txtDimen1 = lvi.FindControl("txtDimen1") as TextBox;
                    TextBox txtDimen2 = lvi.FindControl("txtDimen2") as TextBox;
					FileUpload upOptionImage = (FileUpload)lvi.FindControl("upOptionImage");
					
					string optionName = lblOption.Text;
					string optionValue = "";
					decimal optionPrice = 0.00m;
					byte[] optionImage = null;
					string optionDescription = txtOptionDescription.Text;

					if (upOptionImage.HasFile)
					{
						// Get the bytes from the uploaded file
						optionImage = new byte[upOptionImage.PostedFile.InputStream.Length];
						upOptionImage.PostedFile.InputStream.Read(optionImage, 0, optionImage.Length);
					}

				    if (cboOptionValue != null)
				    {
				        optionValue = cboOptionValue.SelectedItem.Value;
                        if (optionValue != String.Empty)
                        {
                            var oli = (from p in ProductOptions where p.OptionName == optionName && p.OptionValue == optionValue select p).FirstOrDefault();
                            if (oli != null)
                                optionPrice = oli.PriceAlteration;
                            _selectedOptions.Add(new OptionListInfo(optionName, "", optionValue, optionPrice, optionImage, optionDescription, true, false, false, false, false, "", ""));
                        }
				    }
				    else if (txtOptionvalue != null)
				    {
				        optionValue = txtOptionvalue.Text.Trim();
                        _selectedOptions.Add(new OptionListInfo(optionName, "", optionValue, 0.00m, optionImage, optionDescription, true, false, false, false, false, "", ""));
				    }
				    else if (txtDimen1 != null && txtDimen2 != null)
				    {
				        var oli = (from p in ProductOptions where p.OptionName == optionName select p).FirstOrDefault();
				        if (oli != null)
				        {
				            AreaInfo areaProps = ParseAreaProps(oli.ControlProps);
				            decimal dimen1 = 0, dimen2 = 0;

				            if (Decimal.TryParse(txtDimen1.Text, out dimen1) && Decimal.TryParse(txtDimen2.Text, out dimen2))
				            {
                                optionPrice = areaProps.ResultFactor * dimen1 * dimen2 * oli.PriceAlteration - oli.PriceAlteration;
                                string result = string.Format("{0:f2}{1}", dimen1*dimen2*areaProps.ResultFactor, areaProps.ResultUnit);
				                optionValue = txtDimen1.Text + areaProps.Dimension1Unit + " x " + txtDimen2.Text + areaProps.Dimension1Unit + " = " + result;
                                _selectedOptions.Add(new OptionListInfo(optionName, "", optionValue, optionPrice, optionImage, optionDescription, true, false, false, false, false, "", ""));
				            }
				        }

				    }
				}
				return _selectedOptions;
			}
			set
			{
				_selectedOptions = value;
				foreach (OptionListInfo ol in _selectedOptions)
				{
					foreach (ListViewItem lvi in ProductOptionListView.Items)
					{
						Label lblOption = (Label)lvi.FindControl("lblOption");
						DropDownList cboOptionValue = (DropDownList)lvi.FindControl("cboOptionValue");
						TextBox txtOptionValue = (TextBox)lvi.FindControl("txtOptionvalue");
						TextBox txtOptionDescription = (TextBox)lvi.FindControl("txtOptionDescription");
                        TextBox txtDimen1 = lvi.FindControl("txtDimen1") as TextBox;
                        TextBox txtDimen2 = lvi.FindControl("txtDimen2") as TextBox;
						if (lblOption != null)
						{
							string optionName = lblOption.Text;
							if (optionName == ol.OptionName)
							{
							    if (cboOptionValue != null)
							    {
							        cboOptionValue.SelectedValue = ol.OptionValue;
                                    cboOptionValue_SelectedIndexChanged(cboOptionValue,new EventArgs());
							    }
							    else if (txtOptionValue != null)
							    {
							        txtOptionValue.Text = ol.OptionValue;
							    }
                                else if (txtDimen1 != null && txtDimen2 != null)
                                {
                                    // 200cm x 150cm = 3,00qm
                                    string[] vals = ol.OptionValue.Split('=');
                                    vals = vals[0].Split(new string[] {" x "},StringSplitOptions.RemoveEmptyEntries);
                                    txtDimen1.Text = RemoveExtraText(vals[0]);
                                    txtDimen2.Text = RemoveExtraText(vals[1]);
                                }

							    txtOptionDescription.Text = ol.OptionDescription;
							}
						}
					}
				}
			}
		}
		public decimal PriceAlteration
		{
			get
			{
				decimal retVal = 0.00m;
				foreach (ListViewItem lvi in ProductOptionListView.Items)
				{
					Label lblOption = (Label)lvi.FindControl("lblOption");
					DropDownList cboOptionValue = (DropDownList)lvi.FindControl("cboOptionValue");
                    TextBox txtDimen1 = lvi.FindControl("txtDimen1") as TextBox;
                    TextBox txtDimen2 = lvi.FindControl("txtDimen2") as TextBox;
					if (cboOptionValue != null)
					{
						string optionName = lblOption.Text;
						string optionValue = cboOptionValue.SelectedItem.Value;
					    OptionListInfo oli = (from p in ProductOptions where p.OptionName == optionName && p.OptionValue == optionValue select p).FirstOrDefault();
                        if (oli != null)
    						retVal += oli.PriceAlteration;
					}
                    else if (txtDimen1 != null && txtDimen2 != null)
                    {
                        string optionName = lblOption.Text;
                        OptionListInfo oli = (from p in ProductOptions where p.OptionName == optionName select p).FirstOrDefault();
                        if (oli != null)
                        {
                            AreaInfo areaProps = ParseAreaProps(oli.ControlProps);
                            decimal dimen1 = 0, dimen2  = 0;
                            if (Decimal.TryParse(txtDimen1.Text,out dimen1) && Decimal.TryParse(txtDimen2.Text,out dimen2) )
                                retVal += areaProps.ResultFactor * dimen1 * dimen2 * oli.PriceAlteration - oli.PriceAlteration;
                        }
                    }
				}
				return retVal;
			}
		}

        public bool IsComplete
		{
			get
			{
				int cntNotComplete = 0;
				foreach (ListViewItem lvi in ProductOptionListView.Items)
				{
					Label lblOption = (Label)lvi.FindControl("lblOption");
					DropDownList cboOptionValue = (DropDownList)lvi.FindControl("cboOptionValue");
					TextBox txtOptionvalue = (TextBox)lvi.FindControl("txtOptionvalue");
				    TextBox txtDimen1 = lvi.FindControl("txtDimen1") as TextBox;
                    TextBox txtDimen2 = lvi.FindControl("txtDimen2") as TextBox;

                    bool isMandatory = (from p in ProductOptions where p.OptionName == lblOption.Text && p.IsMandatory select p).Any();
					if (isMandatory)
					{
						// Is a Value in cbo selected or something written in Textbox ?
						if (cboOptionValue != null)
						{
							if (cboOptionValue.SelectedItem.Value == String.Empty)
							{
								cntNotComplete++;
								lblOption.Style.Add("color","red");
							}
							else
							{
								lblOption.Style.Remove("color");
							}
						}
						else if (txtOptionvalue != null)
						{
							if (txtOptionvalue.Text == String.Empty)
							{
								cntNotComplete++;
								lblOption.Style.Add("color", "red");                                                                                                                                                                                                 
							}
							else
							{
								lblOption.Style.Remove("color");
							}
						}
                        else if (txtDimen1 != null && txtDimen2 != null)
					    {
                            if (txtDimen1.Text == String.Empty || txtDimen2.Text == String.Empty)
                            {
                                cntNotComplete++;
                                lblOption.Style.Add("color", "red");
                            }
                            else
                            {
                                lblOption.Style.Remove("color");
                            }
					    }
					}
				}
				return (cntNotComplete == 0);
			}

		}
		
		protected void Page_Load(object sender, EventArgs e)
		{
			
			if (ProductOptions != null)
			{
				string lastOption = "";
				List<string> Options = new List<string>();
				foreach (OptionListInfo li in ProductOptions)
				{
					if (li.OptionName != lastOption)
					{
						lastOption = li.OptionName;
						Options.Add(lastOption);
					}
				}
				ProductOptionListView.DataSource = Options;
				ProductOptionListView.DataBind();
			}
		}

		protected void ProductOptionListView_ItemDataBound(object sender, ListViewItemEventArgs e)
		{
			Label lblOption = (Label)e.Item.FindControl("lblOption");
		    PlaceHolder phOptionValue = e.Item.FindControl("phOptionValue") as PlaceHolder;
			
			Label lblMandatory = (Label)e.Item.FindControl("lblMandatory");
			Literal ltrCRLF = (Literal)e.Item.FindControl("ltrCRLF");
			HtmlTableRow trLine = (HtmlTableRow)e.Item.FindControl("trLine");
			HtmlTableRow trAddImage = (HtmlTableRow)e.Item.FindControl("trAddImage");
			HtmlTableRow trAddDesc = (HtmlTableRow)e.Item.FindControl("trAddDesc");
			
            Label lblImage = (Label)e.Item.FindControl("lblImage");
			lblImage.Text = Localization.GetString("lblImage.Text", this.LocalResourceFile);
			Label lblDesc = (Label)e.Item.FindControl("lblDesc");
			lblDesc.Text = Localization.GetString("lblDesc.Text", this.LocalResourceFile);

		    if (e.Item.ItemType == ListViewItemType.DataItem)
		    {
		        ListViewDataItem currentItem = (ListViewDataItem) e.Item;
		        lblOption.Text = currentItem.DataItem.ToString();
		        int anzOption = 0;

		        List<OptionListInfo> options = (from p in ProductOptions where p.OptionName == lblOption.Text select p).ToList();

		        if (options.Count > 0)
		        {
		            string control = options[0].Control;

		            switch (control)
		            {
		                case "dropdown":
                        case "colorbox":
		                    DropDownList cboOptionValue = new DropDownList();
		                    cboOptionValue.ID = "cboOptionValue";
		                    cboOptionValue.Items.Clear();
		                    cboOptionValue.Items.Add(new ListItem(Localization.GetString("SelectOption.Text", ProductModule.LocalResourceFile), ""));
                            if (control == "colorbox")
                                cboOptionValue.Items[0].Attributes.Add("style","background-color:#FFFFFF");

		                    foreach (OptionListInfo lid in options)
		                    {
		                        if (lid.OptionValue != String.Empty)
		                        {
		                            anzOption++;
		                            string optionDisplayvalue = lid.OptionValue;
		                            string optionColor = "";
		                            if (control == "colorbox")
		                            {
		                                optionColor = VfpInterop.StrExtract(optionDisplayvalue, "(", ")",1,1);
		                                optionDisplayvalue = optionDisplayvalue.Substring(0, optionDisplayvalue.IndexOf("(")).Trim();
		                                lid.OptionValue = optionDisplayvalue;
		                            }
		                            if (lid.PriceAlteration != 0.00m)
		                            {
		                                string sign = (lid.PriceAlteration > 0 ? "+" : "-");
		                                decimal alteration = ShowNetPrice ? lid.PriceAlteration : lid.PriceAlteration*(1 + Product.TaxPercent/100);
		                                string format = ShowNetPrice ? "0.0000" : "0.00";
		                                if (lid.ShowDiff)
		                                    optionDisplayvalue += " (" + sign + Math.Abs(alteration).ToString(format).Trim() + " " + ProductModule.Currency + ")";
		                            }
                                    cboOptionValue.Items.Add(new ListItem(optionDisplayvalue, lid.OptionValue));
                                    if (control == "colorbox")
                                        cboOptionValue.Items[cboOptionValue.Items.Count - 1].Attributes.Add("style","background-color:" + optionColor);

		                            if (lid.IsDefault)
		                            {
		                                cboOptionValue.SelectedValue = lid.OptionValue;
		                                // If Mandatory and Default, we delete the "(Select option)"
		                                if (lid.IsMandatory)
		                                    cboOptionValue.Items.RemoveAt(0);
		                            }
		                            if (lid.PriceAlteration != 0.00m)
		                            {
		                                cboOptionValue.SelectedIndexChanged += cboOptionValue_SelectedIndexChanged;
		                                cboOptionValue.AutoPostBack = true;
		                            }
		                        }
		                        lblMandatory.Visible = lid.IsMandatory;
		                        if (lid.OptionDim != String.Empty)
		                        {
		                            string[] dimen = lid.OptionDim.Split(',');
		                            if (dimen.Length > 0)
		                            {
		                                cboOptionValue.Width = new Unit(dimen[0]);
		                            }
		                        }
		                        trAddImage.Visible = lid.AskImage;
		                        trAddDesc.Visible = lid.AskDescription;
		                        trLine.Visible = lid.AskImage || lid.AskDescription;
		                    }
		                    phOptionValue.Controls.Add(cboOptionValue);
		                    break;

		                case "textbox":
		                    TextBox txtOptionValue = new TextBox();
		                    txtOptionValue.ID = "txtOptionValue";
                            
                            OptionListInfo lit = options[0];
		                    if (lit.OptionValue != String.Empty)
		                    {
		                        if (lit.PriceAlteration != 0.00m)
		                        {
		                            txtOptionValue.TextChanged += txtOptionValue_TextChanged;
		                        }
		                    }
		                    lblMandatory.Visible = lit.IsMandatory;
		                    if (lit.OptionDim != String.Empty)
		                    {
		                        string[] dimen = lit.OptionDim.Split(',');
		                        if (dimen.Length == 1)
		                        {
		                            txtOptionValue.Width = new Unit(dimen[0]);
		                        }
		                        if (dimen.Length == 2)
		                        {
		                            txtOptionValue.Width = new Unit(dimen[0]);
		                            txtOptionValue.TextMode = TextBoxMode.MultiLine;
		                            txtOptionValue.Height = new Unit(dimen[1]);
		                        }
		                    }
		                    trAddImage.Visible = lit.AskImage;
		                    trAddDesc.Visible = lit.AskDescription;
		                    trLine.Visible = lit.AskImage || lit.AskDescription;
                            phOptionValue.Controls.Add(txtOptionValue);
		                    break;

                        case "header":
                            lblOption.Style.Add("font-weight", "bold");
		                    lblOption.Style.Add("padding-top", "10px");
		                    ltrCRLF.Visible = true;
		                    break;

                        case "area":
                            OptionListInfo lia = options[0];

                            // Breite[50.0cm-270.00cm],Länge[50cm-180cm],Fläche[0.01qm]

		                    string controlProps = lia.ControlProps;
		                    var areaProps = ParseAreaProps(controlProps);

		                    HtmlTable table = new HtmlTable();
		                    HtmlTableRow rowDimen1 = new HtmlTableRow();
                            HtmlTableRow rowDimen2 = new HtmlTableRow();

                            HtmlTableCell cell = new HtmlTableCell();
                            Label lblDimen1 = new Label();
                            lblDimen1.ID = "lblDimen1";
                            lblDimen1.Text = areaProps.Dimension1Name + " (" + areaProps.Dimension1Unit +")";
                            cell.Controls.Add(lblDimen1);
                            rowDimen1.Controls.Add(cell);

                            cell = new HtmlTableCell();
                            TextBox txtDimen1 = new TextBox();
                            txtDimen1.ID = "txtDimen1";
		                    txtDimen1.Width = 50;
		                    //txtDimen1.AutoPostBack = true;
		                    txtDimen1.CausesValidation = true;
                            cell.Controls.Add(txtDimen1);
                            rowDimen1.Controls.Add(cell);

                            cell = new HtmlTableCell();
                            RangeValidator valDimen1Range = new RangeValidator();
		                    valDimen1Range.ID = "valDimen1Range";
                            valDimen1Range.Display = ValidatorDisplay.Dynamic;
                            valDimen1Range.Type = ValidationDataType.Double;
		                    valDimen1Range.MinimumValue = ((double)areaProps.Dimension1Min).ToString();
                            valDimen1Range.MaximumValue = ((double)areaProps.Dimension1Max).ToString();
		                    valDimen1Range.ControlToValidate = "txtDimen1";
		                    valDimen1Range.ErrorMessage = " (" + areaProps.Dimension1Min.ToString() + ".." + areaProps.Dimension1Max.ToString()+")";
                            valDimen1Range.Style.Add("color","red");
                            cell.Controls.Add(valDimen1Range);
                            rowDimen1.Controls.Add(cell);

                            cell = new HtmlTableCell();
                            Label lblDimen2 = new Label();
                            lblDimen2.ID = "lblDimen2";
                            lblDimen2.Text = areaProps.Dimension2Name + " (" + areaProps.Dimension2Unit + ")";
                            cell.Controls.Add(lblDimen2);
                            rowDimen2.Controls.Add(cell);

                            cell = new HtmlTableCell();
                            TextBox txtDimen2 = new TextBox();
                            txtDimen2.ID = "txtDimen2";
		                    txtDimen2.Width = 50;
                            txtDimen2.AutoPostBack = true;
		                    txtDimen2.CausesValidation = true;
                            cell.Controls.Add(txtDimen2);
                            rowDimen2.Controls.Add(cell);

                            cell = new HtmlTableCell();
                            RangeValidator valDimen2Range = new RangeValidator();
		                    valDimen2Range.ID = "valDimen2Range";
                            valDimen2Range.Display = ValidatorDisplay.Dynamic;
                            valDimen2Range.Type = ValidationDataType.Double;
                            valDimen2Range.MinimumValue = ((double)areaProps.Dimension2Min).ToString();
                            valDimen2Range.MaximumValue = ((double)areaProps.Dimension2Max).ToString();
                            valDimen2Range.ControlToValidate = "txtDimen2";
                            valDimen2Range.ErrorMessage = " (" + areaProps.Dimension2Min.ToString() + ".." + areaProps.Dimension2Max.ToString() + ")";
                            valDimen2Range.Style.Add("color", "red");
                            cell.Controls.Add(valDimen2Range);
                            rowDimen2.Controls.Add(cell);

                            table.Controls.Add(rowDimen1);
                            table.Controls.Add(rowDimen2);

		                    phOptionValue.Controls.Add(table);
		                    break;
		            }
		        }
		    }
		}

	    private AreaInfo ParseAreaProps(string controlProps)
	    {
	        AreaInfo areaProps = new AreaInfo();


	        int atPos = controlProps.IndexOf("[");
	        if (atPos == -1) return areaProps;

	        areaProps.Dimension1Name = controlProps.Substring(0, atPos).Trim();
	        controlProps = controlProps.Substring(atPos + 1);
	        atPos = controlProps.IndexOf("]");

	        if (atPos == -1) return areaProps;

	        string[] dimen1Props = controlProps.Substring(0, atPos).Split('-');
	        areaProps.Dimension1Min = Convert.ToDecimal(RemoveExtraText(dimen1Props[0]), CultureInfo.InvariantCulture);
	        areaProps.Dimension1Max = Convert.ToDecimal(RemoveExtraText(dimen1Props[1]), CultureInfo.InvariantCulture);
	        areaProps.Dimension1Unit = dimen1Props[0].Replace(RemoveExtraText(dimen1Props[0]), "");
	        if (areaProps.Dimension1Unit == String.Empty)
	            areaProps.Dimension1Unit = dimen1Props[1].Replace(RemoveExtraText(dimen1Props[1]), "");

	        controlProps = controlProps.Substring(atPos + 2);

	        atPos = controlProps.IndexOf("[");
	        if (atPos == -1) return areaProps;

	        areaProps.Dimension2Name = controlProps.Substring(0, atPos).Trim();
	        controlProps = controlProps.Substring(atPos + 1);
	        atPos = controlProps.IndexOf("]");

	        if (atPos == -1) return areaProps;

	        string[] dimen2Props = controlProps.Substring(0, atPos).Split('-');
	        areaProps.Dimension2Min = Convert.ToDecimal(RemoveExtraText(dimen2Props[0]), CultureInfo.InvariantCulture);
	        areaProps.Dimension2Max = Convert.ToDecimal(RemoveExtraText(dimen2Props[1]), CultureInfo.InvariantCulture);
	        areaProps.Dimension2Unit = dimen2Props[0].Replace(RemoveExtraText(dimen2Props[0]), "");
	        if (areaProps.Dimension2Unit == String.Empty)
	            areaProps.Dimension2Unit = dimen2Props[1].Replace(RemoveExtraText(dimen2Props[1]), "");

	        controlProps = controlProps.Substring(atPos + 2);

	        atPos = controlProps.IndexOf("[");

	        if (atPos == -1) return areaProps;

	        areaProps.ResultName = controlProps.Substring(0, atPos).Trim();
	        controlProps = controlProps.Substring(atPos + 1);
	        atPos = controlProps.IndexOf("]");

	        if (atPos == -1) return areaProps;

	        string resultProps = controlProps.Substring(0, atPos);
	        areaProps.ResultFactor = Convert.ToDecimal(RemoveExtraText(resultProps), CultureInfo.InvariantCulture);
	        areaProps.ResultUnit = resultProps.Replace(RemoveExtraText(resultProps), "");
	        return areaProps;
	    }

	    protected void cboOptionValue_SelectedIndexChanged(object sender, EventArgs e)
		{
			_selectedOptions = new List<OptionListInfo>();
		    DropDownList cbo = sender as DropDownList;
            if (cbo != null && cbo.SelectedItem != null)
            {
                cbo.Attributes.Remove("style");
                cbo.Attributes.Add("style",cbo.SelectedItem.Attributes["style"]);
            }

			ProductModule.ResetWarning();
		}
		protected void txtOptionValue_TextChanged(object sender, EventArgs e)
		{
			_selectedOptions = new List<OptionListInfo>();
			ProductModule.ResetWarning();
		}

		#region PortalModuleBase Overrides
		protected override void OnLoad(EventArgs e)
		{
			this.LocalResourceFile = Localization.GetResourceFile(this, this.GetType().BaseType.Name + ".ascx");
			base.OnLoad(e);
		}
		#endregion

        private string RemoveExtraText(string value)
        {
            var allowedChars = "01234567890.,";
            return new string(value.Where(c => allowedChars.Contains(c)).ToArray());
        }

	 }
}