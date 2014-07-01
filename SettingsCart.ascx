<%@ Control Language="C#" AutoEventWireup="false" Inherits="Bitboxx.DNNModules.BBStore.SettingsCart" Codebehind="SettingsCart.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="bb" TagName="LanguageEditor" Src="Controls/LanguageEditorControl.ascx" %>
<%@ Register TagPrefix="bb" TagName ="TemplateControl" Src="Controls/TemplateControl.ascx" %> 
<div id="bbstore-panels" class="dnnForm bbstore-cart-settings dnnClear">
    <div class="dnnFormExpandContent"><a href=""><%=Localization.GetString("ExpandAll", Localization.SharedResourceFile)%></a></div>
    <h2 id="bbstore-productlist-head1" class="dnnFormSectionHead"><a href="#"><%=LocalizeString("hdrLayout")%></a></h2>
    <fieldset class="dnnClear">
        <table class="dnnGrid">
	        <tr class="dnnGridHeader">
		        <td>&nbsp;</td>
		        <td><asp:Label ID="lblImage" runat="server" Resourcekey="lblImage.Text"/></td>
		        <td><asp:Label ID="lblQuantity" runat="server" Resourcekey="lblQuantity.Text"/></td>
                <td><asp:Label ID="lblUnits" runat="server" Resourcekey="lblUnits.Text"/></td>
		        <td><asp:Label ID="lblItemNo" runat="server" Resourcekey="lblItemNo.Text"/></td>
		        <td><asp:Label ID="lblProduct" runat="server" Resourcekey="lblProduct.Text"/></td>
		        <td><asp:Label ID="lblUnitCost" runat="server" Resourcekey="lblUnitCost.Text"/></td>
		        <td><asp:Label ID="lblNetTotal" runat="server" Resourcekey="lblNetTotal.Text"/></td>
	            <td><asp:Label ID="lblTaxTotal" runat="server" Resourcekey="lblTaxTotal.Text"/></td>
	            <td><asp:Label ID="lblTaxPercent" runat="server" Resourcekey="lblTaxPercent.Text"/></td>
		        <td><asp:Label ID="lblSubTotal" runat="server" Resourcekey="lblSubTotal.Text"/></td>
	        </tr>
	        <tr class="dnnGridAltItem">
		        <td><dnn:Label id="lblShow" runat="server"/></td>
		        <td><asp:CheckBox ID="chkColVisibleImage" runat="server"/></td>
		        <td>&nbsp;</td>
                <td><asp:CheckBox ID="chkColVisibleUnit" runat="server"/></td>
		        <td><asp:CheckBox ID="chkColVisibleItemNo" runat="server"/></td>
		        <td>&nbsp;</td>
		        <td><asp:CheckBox ID="chkColVisibleUnitCost" runat="server"/></td>
		        <td><asp:CheckBox ID="chkColVisibleNetTotal" runat="server"/></td>
		        <td><asp:CheckBox ID="chkColVisibleTaxPercent" runat="server"/></td>
		        <td><asp:CheckBox ID="chkColVisibleTaxTotal" runat="server"/></td>
		        <td><asp:CheckBox ID="chkColVisibleSubTotal" runat="server"/></td>
	        </tr>
	        <tr class="dnnGridAltItem">
		        <td><dnn:Label id="lblWidth" runat="server"/></td>
	            <td><asp:TextBox ID="txtColWidthImage" runat="server" Columns="5" /></td>
	            <td><asp:TextBox ID="txtColWidthQuantity" runat="server" Columns="5" /></td>
	            <td><asp:TextBox ID="txtColWidthUnit" runat="server" Columns="5" /></td>
	            <td><asp:TextBox ID="txtColWidthItemNo" runat="server" Columns="5" /></td>
		        <td>&nbsp;</td>
	            <td><asp:TextBox ID="txtColWidthAmount" runat="server" Columns="5" /></td>
		        <td>&nbsp;</td>
	            <td><asp:TextBox ID="txtColWidthPercent" runat="server" Columns="5" /></td>
		        <td>&nbsp;</td>
		        <td>&nbsp;</td>
	        </tr>
        </table>

        <div class="dnnFormItem">
            <dnn:Label id="lblShowSummary" runat="server" controlname="chkShowSummary" suffix=":"/>
            <asp:CheckBox ID="chkShowSummary" runat="server" />
        </div>
	    <div class="dnnFormItem">
	        <dnn:Label id="lblShoppingTarget" runat="server" controlname="rblShoppingTarget" suffix=":"/>
            <asp:RadioButtonList runat="server" ID="rblShoppingTarget" RepeatDirection="Horizontal" CssClass="dnnFormRadioButtons">
           	    <asp:ListItem resourcekey="rblhoppingTargetShopHome.Text" Value="0" />
			    <asp:ListItem resourcekey="rblhoppingTargetProduct.Text" Value="1" />
            </asp:RadioButtonList>
        </div>
	    <div class="dnnFormItem">
	        <dnn:Label id="lblEnableCartUpload" runat="server" controlname="chkEnableCartUpload" suffix=":"/>
            <asp:CheckBox ID="chkEnableCartUpload" runat="server" />
        </div>
	    <div class="dnnFormItem">
	        <dnn:Label id="lblEnableCartDownload" runat="server" controlname="chkEnableCartDownload" suffix=":"/>
            <asp:CheckBox ID="chkEnableCartDownload" runat="server" />
        </div>
	    <div class="dnnFormItem">
	        <dnn:Label id="lblMultipleCustomers" runat="server" controlname="chkMultipleCustomers" suffix=":"/>
            <asp:CheckBox ID="chkMultipleCustomers" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblCartNavigationStyle" runat="server" controlname="ddlCartNavigationStyle" suffix=":"/>
            <asp:DropDownList runat="server" ID="ddlCartNavigationStyle"/>
        </div>
    </fieldset>
    <h2 id="bbstore-productlist-head2" class="dnnFormSectionHead"><a href="#"><%=LocalizeString("hdrLanguage")%></a></h2>
    <fieldset class="dnnClear">
        <bb:LanguageEditor ID="lngEmptyCart" runat="server" InternalType="Bitboxx.DNNModules.BBStore.LocalResourceLangInfo" FixedDisplay="Control=Texteditor,Height=400px,Width=500px,Label=EmptyCart" />
        <bb:LanguageEditor ID="lngConfirmCart" runat="server" InternalType="Bitboxx.DNNModules.BBStore.LocalResourceLangInfo" FixedDisplay="Control=Texteditor,Height=400px,Width=500px,Label=ConfirmCart" />
    </fieldset>
    <h2 id="bbstore-productlist-head3" class="dnnFormSectionHead"><a href="#"><%=LocalizeString("hdrAddress")%></a></h2>
    <fieldset class="dnnClear">
	    <div class="dnnFormItem">
		    <dnn:Label id="lblAddressFields" runat="server" suffix=":" />
            <div class="dnnLeft">
                <table class="dnnGrid">
	                <tr class="dnnGridHeader">
		                <td><asp:Label runat="server" id="lblCapAddressPart1" ResourceKey="lblCapAddressPart.Text" /></td>
		                <td><asp:Label runat="server" id="lblCapMandatory1" Text="Mandatory"  ResourceKey="lblCapMandatory.Text" /></td>
                        <td>&nbsp;</td>
                        <td><asp:Label runat="server" id="lblCapAddressPart2" ResourceKey="lblCapAddressPart.Text" /></td>
		                <td><asp:Label runat="server" id="lblCapMandatory2" Text="Mandatory"  ResourceKey="lblCapMandatory.Text" /></td>
	                </tr>
	                <tr class="dnnGridItem">
		                <td><asp:Label runat="server" id="lblCompany" ResourceKey="lblCompany.Text" /></td>
		                <td><asp:CheckBox runat="server" id="chkMandCompany" /></td>
                        <td>&nbsp;</td>
                        <td><asp:Label runat="server" id="lblPrefix" ResourceKey="lblPrefix.Text" /></td>
		                <td><asp:CheckBox runat="server" id="chkMandPrefix" /></td>
	                </tr>
	                <tr class="dnnGridAltItem">
		                <td><asp:Label runat="server" id="lblFirstname" ResourceKey="lblFirstname.Text" /></td>
		                <td><asp:CheckBox runat="server" id="chkMandFirstname" /></td>
		                <td>&nbsp;</td>
                        <td><asp:Label runat="server" id="lblMiddlename" ResourceKey="lblMiddlename.Text" /></td>
		                <td><asp:CheckBox runat="server" id="chkMandMiddlename" /></td>
	                </tr>
	                <tr class="dnnGridItem">
		                <td><asp:Label runat="server" id="lblLastname" ResourceKey="lblLastname.Text" /></td>
		                <td><asp:CheckBox runat="server" id="chkMandLastname" /></td>
		                <td>&nbsp;</td>
                        <td><asp:Label runat="server" id="lblSuffix" ResourceKey="lblSuffix.Text" /></td>
		                <td><asp:CheckBox runat="server" id="chkMandSuffix" /></td>
	                </tr>
	                <tr class="dnnGridAltItem">
		                <td><asp:Label runat="server" id="lblStreet" ResourceKey="lblStreet.Text" /></td>
		                <td><asp:CheckBox runat="server" id="chkMandStreet" /></td>
		                <td>&nbsp;</td>
                        <td><asp:Label runat="server" id="lblUnit" ResourceKey="lblUnit.Text" /></td>
		                <td><asp:CheckBox runat="server" id="chkMandUnit" /></td>
	                </tr>
	                <tr class="dnnGridItem">
		                <td><asp:Label runat="server" id="lblPostalCode" ResourceKey="lblPostalCode.Text" /></td>
		                <td><asp:CheckBox runat="server" id="chkMandPostalCode" /></td>
		                <td>&nbsp;</td>
                        <td><asp:Label runat="server" id="lblCity" ResourceKey="lblCity.Text" /></td>
		                <td><asp:CheckBox runat="server" id="chkMandCity" /></td>
	                </tr>
	                <tr class="dnnGridAltItem">
		                <td><asp:Label runat="server"  id="lblSuburb" ResourceKey="lblSuburb.Text" /></td>
		                <td><asp:CheckBox runat="server" id="chkMandSuburb" /></td>
		                <td>&nbsp;</td>
                        <td><asp:Label runat="server"  id="lblRegion" ResourceKey="lblRegion.Text" /></td>
		                <td><asp:CheckBox runat="server" id="chkMandRegion" /></td>
	                </tr>
	                <tr class="dnnGridItem">
		                <td><asp:Label runat="server"  id="lblCountry" ResourceKey="lblCountry.Text" /></td>
		                <td><asp:CheckBox runat="server" id="chkMandCountry" /></td>
		                <td>&nbsp;</td>
                        <td><asp:Label runat="server"  id="lblPhone" ResourceKey="lblPhone.Text" /></td>
		                <td><asp:CheckBox runat="server" id="chkMandPhone" /></td>
	                </tr>
	                <tr class="dnnGridAltItem">
		                <td><asp:Label runat="server"  id="lblCell" ResourceKey="lblCell.Text" /></td>
		                <td><asp:CheckBox runat="server" id="chkMandCell" /></td>
                        <td>&nbsp;</td>
		                <td><asp:Label runat="server"  id="lblFax" ResourceKey="lblFax.Text" /></td>
		                <td><asp:CheckBox runat="server" id="chkMandFax" /></td>
	                </tr>
	                <tr class="dnnGridItem">
		                <td><asp:Label runat="server" id="lblEmail" ResourceKey="lblEmail.Text" /></td>
		                <td><asp:CheckBox runat="server" id="chkMandEmail" /></td>
                        <td colspan="3">&nbsp;</td>
	                </tr>
                </table>
            </div>
        </div>
	    <div class="dnnFormItem">
		    <dnn:Label runat="server" id="lblAddressTemplate" controlname="txtAddressTemplate" />
		    <asp:TextBox runat="server" ID="txtAddressTemplate" TextMode="MultiLine" Rows="6" Columns="40" CssClass="bbstore-template" />
		    <asp:CustomValidator runat="server" id="valAddressTemplate" controltovalidate="txtAddressTemplate" 
			    onservervalidate="valAddressTemplate_ServerValidate" CssClass="dnnFormMessage dnnFormError" ResourceKey="valAddressTemplate.Error" />
	    </div>
    </fieldset>
    <h2 id="bbstore-productlist-head4" class="dnnFormSectionHead"><a href="#"><%=LocalizeString("hdrTemplates")%></a></h2>
    <fieldset class="dnnClear">
        <bb:TemplateControl ID="tplTemplate" runat="server" Key="Order" ViewMode="View" EditorControl="TextEditor" />
    </fieldset>
</div> 



