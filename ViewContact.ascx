<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewContact.ascx.cs" Inherits="Bitboxx.DNNModules.BBStore.ViewContact" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<asp:Panel ID="pnlContactView" runat="server" CssClass="dnnForm bbstore-contact bbstore-admin dnnClear">
    <asp:Panel ID="pnlContactData" runat="server">
	    <h3><asp:Label ID="lblContactData" runat="server" ResourceKey="lblContactData.Text" /></h3>
	    <hr />
	    <fieldset>
		    <div id="trCompany" runat="server" class="dnnFormItem">
		        <dnn:Label ID="lblContactCompany" runat="server" />
		        <asp:TextBox ID="txtContactCompany" runat="server" Columns="50" />
				<asp:RequiredFieldValidator ID="valContactCompany" runat="server"  ControlToValidate="txtContactCompany" CssClass="dnnFormMessage dnnFormError"  />
		    </div>
		    <div id="trPrefix" runat="server" class="dnnFormItem">
		        <dnn:Label ID="lblContactPrefix" runat="server" />
		        <asp:TextBox ID="txtContactPrefix" runat="server" Columns="20" />
				<asp:RequiredFieldValidator ID="valContactPrefix" runat="server"  ControlToValidate="txtContactPrefix" CssClass="dnnFormMessage dnnFormError"/>
		    </div>
		    <div id="trFirstname" runat="server" class="dnnFormItem">
		        <dnn:Label ID="lblContactFirstname" runat="server"/>
		        <asp:TextBox ID="txtContactFirstname" runat="server" Columns="50" />
				<asp:RequiredFieldValidator ID="valContactFirstname" runat="server"  ControlToValidate="txtContactFirstname" CssClass="dnnFormMessage dnnFormError"/>
		    </div>
		    <div id="trLastname" runat="server" class="dnnFormItem">
		        <dnn:Label ID="lblContactLastname" runat="server"/>
		        <asp:TextBox ID="txtContactLastname" runat="server" Columns="50" />
				<asp:RequiredFieldValidator ID="valContactLastname" runat="server" ControlToValidate="txtContactLastname" CssClass="dnnFormMessage dnnFormError"/>
		    </div>
		    <div id="trStreet" runat="server" class="dnnFormItem">
		        <dnn:Label ID="lblContactStreet" runat="server"/>
		        <asp:TextBox ID="txtContactStreet" runat="server" Columns="44" />
		        <asp:TextBox ID="txtContactUnit" runat="server" Columns="5" />
			    <asp:RequiredFieldValidator ID="valContactStreet" runat="server" ControlToValidate="txtContactStreet" CssClass="dnnFormMessage dnnFormError"/>
		    </div>
		    <div id="trRegion" runat="server" class="dnnFormItem">
		        <dnn:Label ID="lblContactRegion" runat="server"/>
		        <asp:TextBox ID="txtContactRegion" runat="server" Columns="50" />
				<asp:RequiredFieldValidator ID="valContactRegion" runat="server" ControlToValidate="txtContactRegion" CssClass="dnnFormMessage dnnFormError"/>
		    </div>
		    <div id="trCity" runat="server" class="dnnFormItem">
		        <dnn:Label ID="lblContactPostalcode" runat="server"/>
		        <asp:TextBox ID="txtContactPostalcode" runat="server" Columns="10" />
		        <asp:TextBox ID="txtContactCity" runat="server" Columns="40" />
				<asp:RequiredFieldValidator ID="valContactCity" runat="server" ControlToValidate="txtContactCity" CssClass="dnnFormMessage dnnFormError"/>
		    </div>
		    <div id="trCountry" runat="server" class="dnnFormItem">
		        <dnn:Label ID="lblContactCountry" runat="server"/>
		        <asp:DropDownList ID="ddlCountry" runat="server" Width="300px" />
				<asp:RequiredFieldValidator ID="valContactCountry" runat="server" ControlToValidate="ddlCountry" CssClass="dnnFormMessage dnnFormError"/>
		    </div>
		    <div id="trPhone" runat="server" class="dnnFormItem">
		        <dnn:Label ID="lblContactTelephone" runat="server" />
		        <asp:TextBox ID="txtContactTelephone" runat="server" Columns="30" />
				<asp:RequiredFieldValidator ID="valContactTelephone" runat="server" ControlToValidate="txtContactTelephone" CssClass="dnnFormMessage dnnFormError"/>
		    </div>
		    <div id="trCell" runat="server" class="dnnFormItem">
		        <dnn:Label ID="lblContactCell" runat="server" />
		        <asp:TextBox ID="txtContactCell" runat="server" Columns="30" />
			    <asp:RequiredFieldValidator ID="valContactCell" runat="server" ControlToValidate="txtContactCell" CssClass="dnnFormMessage dnnFormError"/>
		    </div>
		    <div id="trFax" runat="server" class="dnnFormItem">
		        <dnn:Label ID="lblContactFax" runat="server"  />
		        <asp:TextBox ID="txtContactFax" runat="server" Columns="30" />
			    <asp:RequiredFieldValidator ID="valContactFax" runat="server" ControlToValidate="txtContactFax" CssClass="dnnFormMessage dnnFormError"/>
		    </div>
		    <div id="trEmail" runat="server" class="dnnFormItem">
		        <dnn:Label ID="lblContactEmail" runat="server" />
		        <asp:TextBox ID="txtContactEmail" runat="server" Columns="50" />
			    <asp:RequiredFieldValidator ID="valContactEmail" runat="server" ControlToValidate="txtContactEmail" CssClass="dnnFormMessage dnnFormError"/>
		    </div>
	    </fieldset>
    </asp:Panel>
    <asp:Panel ID="pnlProducts" runat="server">
	    <h3><asp:Label ID="lblProductData" runat="server" ResourceKey="lblProductData.Text" /></h3>
	    <hr />
	    <asp:ListView ID="lstProducts" runat="server" 
		    onitemcreated="lstProducts_ItemCreated" 
		    DataKeyNames="SimpleProductId" onitemdeleting="lstProducts_ItemDeleting">
		    <ItemTemplate>
			    <div class="dnnFormItem">
			        <div class="dnnTooltip">&nbsp;</div>
				    <div class="dnnLeft">
				        <asp:PlaceHolder ID="productPlaceHolder" runat="server" />
                    </div>
			    </div>
		    </ItemTemplate>
	    </asp:ListView>
        <div class="dnnFormItem">
			<div class="dnnTooltip">&nbsp;</div>
	        <asp:LinkButton ID="cmdReturn" runat="server" ResourceKey="cmdReturn.Text" OnClick="cmdReturn_Click" CausesValidation="False"/>
        </div>
    </asp:Panel>
    <asp:Panel ID="pnlRequestData" runat="server">
	    <h3><asp:Label ID="lblRequestData" runat="server" ResourceKey="lblRequestData.Text" /></h3>
        <hr/>
	    <div class="dnnFormItem">
	        <asp:Label ID="lblRequest" runat="server" Resourcekey="lblRequest.Text" CssClass="dnnTooltip"/>
	        <asp:TextBox ID="txtRequest" runat="server" Columns="50" Rows="6" TextMode="MultiLine" style="min-width: 0"/>
	    </div>
    </asp:Panel>
    <asp:Panel ID="pnlConfirmData" runat="server" Visible="false">
	    <h3><asp:Label ID="lblConfirmData" runat="server" ResourceKey="lblConfirmData.Text" /></h3>
	    <hr />
	    <asp:Label ID="lblConfirm" runat="server" Resourcekey="lblConfirm.Text" />
    </asp:Panel>
    <asp:Panel ID="pnlSend" runat="server">
	    <ul class="dnnActions dnnClear">
	        <li><asp:LinkButton ID="cmdSend" runat="server" ResourceKey="cmdSend.Text" OnClick="cmdSend_Click" CssClass="dnnPrimaryAction"/></li>
        </ul>
    </asp:Panel>
</asp:Panel>