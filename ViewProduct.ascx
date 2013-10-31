<%@ Control Language="C#" Inherits="Bitboxx.DNNModules.BBStore.ViewProduct"  AutoEventWireup="true" CodeBehind="ViewProduct.ascx.cs" %>
<div id="divProduct" runat="server" class="bbstore-product dnnClear">
	<asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
	<asp:Panel ID="pnlAdmin" runat="server" Visible="false">
		<hr />
		<div style="text-align:right">
		<span style="white-space:nowrap">
			<asp:ImageButton ID="imgNew" runat="server" resourcekey="cmdNew" ImageUrl="~/images/icon_unknown_16px.gif"
				OnClick="cmdNew_Click" />&nbsp;<asp:LinkButton CssClass="CommandButton" ID="cmdNew"
					runat="server" resourcekey="cmdNew" OnClick="cmdNew_Click" /></span>&nbsp;
		<span style="white-space:nowrap">
			<asp:ImageButton ID="imgEdit" runat="server" resourcekey="cmdEdit" ImageUrl="~/images/edit.gif"
				OnClick="cmdEdit_Click" />&nbsp;<asp:LinkButton CssClass="CommandButton" ID="cmdEdit"
					runat="server" resourcekey="cmdEdit" OnClick="cmdEdit_Click" /></span>&nbsp;
		</div>
	</asp:Panel>
</div>


