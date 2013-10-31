<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewCartAddressEdit.ascx.cs" Inherits="Bitboxx.DNNModules.BBStore.ViewCartAddressEdit" %>
<div class="dnnForm bbstore-cart-adressedit dnnClear">
    <p>&nbsp;</p>
	<div class="dnnFormMessage dnnFormInfo" id="pnlIntro" runat="server">
        <asp:Label ID="lblIntro" runat="server"  />
    </div>
    <p>&nbsp;</p>
	<asp:HiddenField runat="server" ID="hidAdrEditCustomerAddressId"/>
	<asp:PlaceHolder runat="server" ID="phAddressEdit" />
	<table style="width:100%;">
		<tr>
			<td style="text-align:left;"><asp:Button ID="cmdAdrEditCancel" runat="server" Resourcekey="cmdCancel" onclick="cmdAdrEditCancel_Click" CausesValidation="False" /></td>
			<td style="text-align:right;"><asp:Button id="cmdAdrEditSave" runat="server" resourcekey="cmdUpdate" OnClick="cmdAdrEditSave_Click"/></td>
		</tr>
	</table>
 </div>