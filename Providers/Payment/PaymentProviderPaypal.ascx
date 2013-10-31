<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PaymentProviderPaypal.ascx.cs" Inherits="Bitboxx.DNNModules.BBStore.Providers.Payment.PaymentProviderPaypal" %>
<asp:MultiView ID="pnlShow" runat="server">
    <asp:View ID="View" runat="server">
        <table>
            <tr>
                <td><asp:Image runat="server" ID="imgPaypal" ImageUrl="~/Desktopmodules/BBStore/Providers/Payment/Images/Paypal.png"/></td>
                <td style="text-align: justify"><asp:Label ID="lblDescription" runat="server" /></td>
            </tr>
        </table>
        <p>&nbsp;</p>
    </asp:View>
    <asp:View ID="Edit" runat="server">
        <h3><asp:Label ID="lblTitle" runat="server" Text="Title" /></h3>
        <table>
            <tr>
                <td><asp:Label ID="lblSandbox" runat="server" ResourceKey="lblPPSandbox.Text" /></td>
                <td><asp:CheckBox runat="server" ID="chkSandbox" Checked="True"/></td>
            </tr>
            <tr>
                <td><asp:Label ID="lblPPUser" runat="server" ResourceKey="lblPPUser.Text" /></td>
                <td><asp:TextBox ID="txtPPUser" Columns="40" runat="server" /></td>
            </tr>
            <tr>
                <td><asp:Label ID="lblPPPassword" runat="server" ResourceKey="lblPPPassword.Text" /></td>
                <td><asp:TextBox ID="txtPPPassword" Columns="40" runat="server" /></td>
            </tr>
            <tr>
                <td><asp:Label ID="lblPPSignature" runat="server"  ResourceKey="lblPPSignature.Text" /></td>
                <td><asp:TextBox ID="txtPPSignature" Columns="40" runat="server" /></td>
            </tr>
		</table>

    </asp:View>
    <asp:View ID="Summary" runat="server">
    </asp:View>
</asp:MultiView>
