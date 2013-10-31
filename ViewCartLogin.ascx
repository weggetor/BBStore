<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewCartLogin.ascx.cs" Inherits="Bitboxx.DNNModules.BBStore.ViewCartLogin" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm bbstore-cart-login dnnClear">
    <p>&nbsp;</p>
	<div class="dnnFormMessage" id="divMessages" runat="server">
        <asp:Label ID="lblIntro" runat="server"  />
    </div>
	<asp:Panel runat="server" ID="pnlUser">
		<div class="dnnFormItem">
			<dnn:Label id="lblUserName" runat="server" controlname="txtUserName" suffix=":" />
			<asp:TextBox id="txtUserName" runat="server" Columns="30" CssClass="dnnFormInput" />
		</div>
		<asp:Panel runat="server" ID="pnlCheckUser">
			<p>&nbsp;</p>
			<div style="text-align:right;">
				<asp:Button ID="cmdCheckUser" runat="server" Resourcekey="NextStep.Text" onclick="cmdCheckuser_Click"/>
			</div>
		</asp:Panel>
		<asp:Panel runat="server" ID="pnlPassword" Visible="True">
			<div class="dnnFormItem">
				<dnn:Label id="lblPassword" runat="server" controlname="txtPassword" suffix=":" />
				<asp:TextBox id="txtPassword" runat="server" Columns="30" CssClass="dnnFormInput" TextMode="Password" />
			</div>
			<p>&nbsp;</p>
			<div style="text-align:right;">
				<asp:Button ID="cmdPassword" runat="server" Resourcekey="NextStep.Text" onclick="cmdPassword_Click"/>
			</div>
		</asp:Panel>
	</asp:Panel>
	<asp:Panel runat="server" ID="pnlConfirmUser" Visible="False">
		<div class="dnnFormItem">
			<dnn:Label id="lblUserName2" resourcekey="lblUserName.Text" runat="server" controlname="lblUserNameConfirm" suffix=":" />
			<asp:Label id="lblUserNameConfirm" runat="server" CssClass="dnnFormInput" />
		</div>
		<asp:Panel runat="server" ID="Panel1" Visible="True">
			<div class="dnnFormItem">
				<dnn:Label id="lblUserName2Repeat" runat="server" controlname="txtUserName2Repeat" suffix=":" />
				<asp:TextBox id="txtUserName2Repeat" runat="server" Columns="30" CssClass="dnnFormInput" />
			</div>
			<p>&nbsp;</p>
			<div style="text-align:right;">
				<asp:Button ID="cmdNewuser" runat="server" Resourcekey="NextStep.Text" onclick="cmdNewUser_Click"/>
			</div>
		</asp:Panel>
	</asp:Panel>
</div>
