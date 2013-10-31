<%@ Control Language="C#" Inherits="Bitboxx.DNNModules.BBStore.ViewAdmin" AutoEventWireup="true" CodeBehind="ViewAdmin.ascx.cs" %>
<div class="bbstore-admin dnnClear">
    <h3><asp:Label ID="lblTitle" runat="server"/></h3>
    <asp:Panel ID="pnlMain" runat="server">
        <asp:PlaceHolder runat="server" ID="phButtons" />
    </asp:Panel>
    <asp:Panel ID="pnlPlaceholder" runat="server">
        <asp:PlaceHolder ID="phContent" runat="server" />
    </asp:Panel>
    <asp:Panel ID="pnlBackLink" runat="server">
    <asp:ImageButton ID="imgMainMenu" runat="server" resourcekey="cmdMainMenu" ImageUrl="~/images/lt.gif"
            OnClick="cmdMainMenu_Click" /><asp:LinkButton CssClass="CommandButton" ID="cmdMainMenu"
                runat="server" resourcekey="cmdMainMenu" OnClick="cmdMainMenu_Click" />
    </asp:Panel>
</div>