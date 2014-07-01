<%@ Control Language="C#" AutoEventWireup="false" Inherits="Bitboxx.DNNModules.BBStore.SettingsContact" Codebehind="SettingsContact.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="bb" TagName="TabSelectControl" Src="Controls/TabSelectControl.ascx" %>  
<%@ Register TagPrefix="bb" TagName ="TemplateControl" Src="Controls/TemplateControl.ascx" %>
<div class="dnnForm bbstore-contact-settings dnnClear">
    <div class="dnnFormItem">
	    <dnn:Label id="lblAddressFields" runat="server" style="white-space:nowrap"  suffix=":" />
        <div class="dnnLeft">
            <table class="dnnGrid">
	            <tr class="dnnGridHeader">
		            <td style="width:150px"><asp:Label runat="server" Text="AddressPart" id="lblCapAddressPart" ResourceKey="lblCapAddressPart.Text" /></td>
		            <td><asp:Label runat="server" Text="Show" id="lblCapShow" ResourceKey="lblCapShow.Text" /></td>
		            <td><asp:Label runat="server" Text="Mandatory" id="lblCapMandatory" ResourceKey="lblCapMandatory.Text" /></td>
	            </tr>
	            <tr class="dnnGridItem">
		            <td><asp:Label runat="server" Text="Company" id="lblCompany" ResourceKey="lblCompany.Text" /></td>
		            <td><asp:CheckBox runat="server" id="chkShowCompany" /></td>
		            <td><asp:CheckBox runat="server" id="chkMandCompany" /></td>
	            </tr>
	            <tr class="dnnGridAltItem">
		            <td><asp:Label runat="server" Text="Prefix" id="lblPrefix" ResourceKey="lblPrefix.Text" /></td>
		            <td><asp:CheckBox runat="server" id="chkShowPrefix" /></td>
		            <td><asp:CheckBox runat="server" id="chkMandPrefix" /></td>
	            </tr>
	            <tr class="dnnGridItem">
		            <td><asp:Label runat="server" Text="Firstname" id="lblFirstname" ResourceKey="lblFirstname.Text" /></td>
		            <td><asp:CheckBox runat="server" id="chkShowFirstname" /></td>
		            <td><asp:CheckBox runat="server" id="chkMandFirstname" /></td>
	            </tr>
	            <tr class="dnnGridAltItem">
		            <td><asp:Label runat="server" Text="Lastname" id="lblLastname" ResourceKey="lblLastname.Text" /></td>
		            <td><asp:CheckBox runat="server" id="chkShowLastname" /></td>
		            <td><asp:CheckBox runat="server" id="chkMandLastname" /></td>
	            </tr>
	            <tr class="dnnGridItem">
		            <td><asp:Label runat="server" Text="Street" id="lblStreet" ResourceKey="lblStreet.Text" /></td>
		            <td><asp:CheckBox runat="server" id="chkShowStreet" /></td>
		            <td><asp:CheckBox runat="server" id="chkMandStreet" /></td>
	            </tr>
	            <tr class="dnnGridAltItem">
		            <td><asp:Label runat="server" Text="Region" id="lblRegion" ResourceKey="lblRegion.Text" /></td>
		            <td><asp:CheckBox runat="server" id="chkShowRegion" /></td>
		            <td><asp:CheckBox runat="server" id="chkMandRegion" /></td>
	            </tr>
	            <tr class="dnnGridItem">
		            <td><asp:Label runat="server" Text="City" id="lblCity" ResourceKey="lblCity.Text" /></td>
		            <td><asp:CheckBox runat="server" id="chkShowCity" /></td>
		            <td><asp:CheckBox runat="server" id="chkMandCity" /></td>
	            </tr>
	            <tr class="dnnGridAltItem">
		            <td><asp:Label runat="server" Text="Country" id="lblCountry" ResourceKey="lblCountry.Text" /></td>
		            <td><asp:CheckBox runat="server" id="chkShowCountry" /></td>
		            <td><asp:CheckBox runat="server" id="chkMandCountry" /></td>
	            </tr>
	            <tr class="dnnGridItem">
		            <td><asp:Label runat="server" Text="Phone" id="lblPhone" ResourceKey="lblPhone.Text" /></td>
		            <td><asp:CheckBox runat="server" id="chkShowPhone" /></td>
		            <td><asp:CheckBox runat="server" id="chkMandPhone" /></td>
	            </tr>
	            <tr class="dnnGridAltItem">
		            <td><asp:Label runat="server" Text="Cell" id="lblCell" ResourceKey="lblCell.Text" /></td>
		            <td><asp:CheckBox runat="server" id="chkShowCell" /></td>
		            <td><asp:CheckBox runat="server" id="chkMandCell" /></td>
	            </tr>
	            <tr class="dnnGridItem">
		            <td><asp:Label runat="server" Text="Fax" id="lblFax" ResourceKey="lblFax.Text" /></td>
		            <td><asp:CheckBox runat="server" id="chkShowFax" /></td>
		            <td><asp:CheckBox runat="server" id="chkMandFax" /></td>
	            </tr>
	            <tr class="dnnGridAltItem">
		            <td><asp:Label runat="server" Text="Email" id="lblEmail" ResourceKey="lblEmail.Text" /></td>
		            <td><asp:CheckBox runat="server" id="chkShowEmail" /></td>
		            <td><asp:CheckBox runat="server" id="chkMandEmail" /></td>
	            </tr>
            </table>
        </div>
    </div>
    <div class="dnnFormItem">
        <dnn:Label id="lblShopHome" runat="server" controlname="urlShopHome" suffix=":"/>
		<bb:TabSelectControl ID="urlShopHome" runat="server" Width="200" />
    </div>
	<div class="dnnFormItem">
	    <dnn:Label id="lblEmailSubject" runat="server" controlname="txtEmailSubject" suffix=":"/>
        <asp:TextBox ID="txtEmailSubject" runat="server" Columns="60" ></asp:TextBox>
    </div>
    <bb:TemplateControl ID="tplProduct" runat="server" Key="Contact" ViewMode="View" EditorControl="TextEditor"/>
    <bb:TemplateControl ID="tplTemplate" runat="server" Key="Request" ViewMode="View" EditorControl="TextEditor"/>
</div>
