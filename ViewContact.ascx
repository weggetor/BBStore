<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewContact.ascx.cs" Inherits="Bitboxx.DNNModules.BBStore.ViewContact" %>
<asp:Panel ID="pnlContactView" runat="server" CssClass="dnnForm bbstore-contact dnnClear">
    <asp:Panel ID="pnlContactData" runat="server">
	    <h3><asp:Label ID="lblContactData" runat="server" ResourceKey="lblContactData.Text" /></h3>
	    <hr />
	    <fieldset>
		    <div id="trCompany" runat="server" class="dnnFormItem">
		        <asp:Label ID="lblContactCompany" runat="server" CssClass="dnnTooltip" />
		        <asp:TextBox ID="txtContactCompany" runat="server" Columns="50" style="min-width: 0"/>
				<asp:RequiredFieldValidator ID="valContactCompany" runat="server"  ControlToValidate="txtContactCompany"/>
		    </div>
		    <div id="trPrefix" runat="server" class="dnnFormItem">
		        <asp:Label ID="lblContactPrefix" runat="server"  CssClass="dnnTooltip"/>
		        <asp:TextBox ID="txtContactPrefix" runat="server" Columns="20" style="min-width: 0"/>
				<asp:RequiredFieldValidator ID="valContactPrefix" runat="server"  ControlToValidate="txtContactPrefix"/>
		    </div>
		    <div id="trFirstname" runat="server" class="dnnFormItem">
		        <asp:Label ID="lblContactFirstname" runat="server" CssClass="dnnTooltip"/>
		        <asp:TextBox ID="txtContactFirstname" runat="server" Columns="50" style="min-width: 0"/>
				<asp:RequiredFieldValidator ID="valContactFirstname" runat="server"  ControlToValidate="txtContactFirstname"/>
		    </div>
		    <div id="trLastname" runat="server" class="dnnFormItem">
		        <asp:Label ID="lblContactLastname" runat="server" CssClass="dnnTooltip"/>
		        <asp:TextBox ID="txtContactLastname" runat="server" Columns="50" style="min-width: 0"/>
				<asp:RequiredFieldValidator ID="valContactLastname" runat="server" ControlToValidate="txtContactLastname"/>
		    </div>
		    <div id="trStreet" runat="server" class="dnnFormItem">
			    <div class="dnnTooltip">
                    <asp:Label ID="lblContactStreet" runat="server"></asp:Label>
			        <asp:Label ID="lblContactUnit" runat="server"></asp:Label>
                </div>
		        <asp:TextBox ID="txtContactStreet" runat="server" Columns="44" style="min-width: 0"/>
		        <asp:TextBox ID="txtContactUnit" runat="server" Columns="5" style="min-width: 0"/>
			    <asp:RequiredFieldValidator ID="valContactStreet" runat="server" ControlToValidate="txtContactStreet"/>
		    </div>
		    <div id="trRegion" runat="server" class="dnnFormItem">
		        <asp:Label ID="lblContactRegion" runat="server"  CssClass="dnnTooltip"/>
			    <asp:TextBox ID="txtContactRegion" runat="server" Columns="50"></asp:TextBox>
				<asp:RequiredFieldValidator ID="valContactRegion" runat="server" ControlToValidate="txtContactRegion"/>
		    </div>
		    <div id="trCity" runat="server" class="dnnFormItem">
		        <asp:Label ID="lblContactPostalcode" runat="server" CssClass="dnnTooltip"/>
		        <asp:TextBox ID="txtContactPostalcode" runat="server" Columns="10" style="min-width: 0"/>
		        <asp:TextBox ID="txtContactCity" runat="server" Columns="40" style="min-width: 0"/>
				<asp:RequiredFieldValidator ID="valContactCity" runat="server" ControlToValidate="txtContactCity"/>
		    </div>
		    <div id="trCountry" runat="server" class="dnnFormItem">
		        <asp:Label ID="lblContactCountry" runat="server"  CssClass="dnnTooltip"/>
		        <asp:DropDownList ID="ddlCountry" runat="server" Width="300px" style="min-width: 0" />
				<asp:RequiredFieldValidator ID="valContactCountry" runat="server" ControlToValidate="ddlCountry"/>
		    </div>
		    <div id="trPhone" runat="server" class="dnnFormItem">
		        <asp:Label ID="lblContactTelephone" runat="server" CssClass="dnnTooltip"/>
		        <asp:TextBox ID="txtContactTelephone" runat="server" Columns="30" style="min-width: 0"/>
				<asp:RequiredFieldValidator ID="valContactTelephone" runat="server" ControlToValidate="txtContactTelephone"/>
		    </div>
		    <div id="trCell" runat="server" class="dnnFormItem">
		        <asp:Label ID="lblContactCell" runat="server" CssClass="dnnTooltip"/>
		        <asp:TextBox ID="txtContactCell" runat="server" Columns="30" style="min-width: 0"/>
			    <asp:RequiredFieldValidator ID="valContactCell" runat="server" ControlToValidate="txtContactCell"/>
		    </div>
		    <div id="trFax" runat="server" class="dnnFormItem">
		        <asp:Label ID="lblContactFax" runat="server"  CssClass="dnnTooltip"/>
		        <asp:TextBox ID="txtContactFax" runat="server" Columns="30" style="min-width: 0"/>
			    <asp:RequiredFieldValidator ID="valContactFax" runat="server" ControlToValidate="txtContactFax"/>
		    </div>
		    <div id="trEmail" runat="server" class="dnnFormItem">
		        <asp:Label ID="lblContactEmail" runat="server" CssClass="dnnTooltip"/>
		        <asp:TextBox ID="txtContactEmail" runat="server" Columns="50" style="min-width: 0"/>
			    <asp:RequiredFieldValidator ID="valContactEmail" runat="server" ControlToValidate="txtContactEmail"/>
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