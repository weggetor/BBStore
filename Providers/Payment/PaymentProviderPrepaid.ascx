<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PaymentProviderPrepaid.ascx.cs" Inherits="Bitboxx.DNNModules.BBStore.Providers.Payment.PaymentProviderPrepaid" %>
<asp:MultiView ID="pnlShow" runat="server">
    <asp:View ID="View" runat="server">
        <table>
            <tr>
                <td><asp:Image runat="server" ID="imgDebit" ImageUrl="~/Desktopmodules/BBStore/Providers/Payment/Images/prepaid.png"/></td>
                <td style="text-align: justify"><asp:Label ID="lblDescription" runat="server" /></td>
            </tr>
       </table>
        <p>&nbsp;</p>
        <table style="border: medium double #AAAAAA; padding:10px; ">
            <tr>
                <td><b><asp:Label ID="lblAccountNameCapView" runat="server" /></b></td>
                <td><asp:Label ID="lblAccountNameView" runat="server" /></td>
                <td>&nbsp;</td>
                <td><b><asp:Label ID="lblAccountNoCapView" runat="server" /></b></td>
                <td><asp:Label ID="lblAccountNoView" runat="server" /></td>
            </tr>
            <tr>
                <td><b><asp:Label ID="lblBankNameCapView" runat="server" /></b></td>
                <td><asp:Label ID="lblBankNameView" runat="server"/></td>
                <td>&nbsp;</td>
                <td><b><asp:Label ID="lblIbanCapView" runat="server" /></b></td>
                <td><asp:Label ID="lblIbanView" runat="server" /></td>
            </tr>
            <tr>
                <td><b><asp:Label ID="lblBinCapView" runat="server" /></b></td>
                <td><asp:Label ID="lblBinView" runat="server" /></td>
                <td>&nbsp;</td>
                <td><b><asp:Label ID="lblBicCapView" runat="server" /></b></td>
                <td><asp:Label ID="lblBicView" runat="server" /></td>
            </tr>
        </table>
    </asp:View>
    <asp:View ID="Edit" runat="server">
        <h3><asp:Label ID="lblTitle" runat="server" Text="Title" /></h3>
        <table>
            <tr>
                <td><asp:Label ID="lblAccountNameEdit" runat="server" ResourceKey="lblAccountName.Text" /></td>
                <td><asp:TextBox ID="txtAccountName" Columns="40" runat="server" /></td>
            </tr>
            <tr>
                <td><asp:Label ID="lblBankNameEdit" runat="server" ResourceKey="lblBankName.Text" /></td>
                <td><asp:TextBox ID="txtBankName" Columns="40" runat="server" /></td>
            </tr>
            <tr>
                <td><asp:Label ID="lblBinEdit" runat="server" ResourceKey="lblBin.Text" /></td>
                <td><asp:TextBox ID="txtBin" Columns="16" runat="server" /></td>
            </tr>
            <tr>
                <td><asp:Label ID="lblAccountNoEdit" runat="server" ResourceKey="lblAccountNo.Text" /></td>
                <td><asp:TextBox ID="txtAccountNo" Columns="20" runat="server" /></td>
            </tr>
            <tr>
                <td><asp:Label ID="lblIbanEdit" runat="server" ResourceKey="lblIban.Text" /></td>
			    <td><asp:TextBox ID="txtIban" Columns="34" MaxLength="34" runat="server" /></td>
            </tr>
            <tr>
                <td><asp:Label ID="lblBicEdit" runat="server" ResourceKey="lblBic.Text" /></td>
                <td><asp:TextBox ID="txtBic" Columns="13" MaxLength="11" runat="server" /></td>
            </tr>
		</table>
    </asp:View>
    <asp:View ID="Summary" runat="server">
        <table style="padding:5px; ">
            <tr>
                <td><b><asp:Label ID="lblAccountNameCapSummary" runat="server" /></b></td>
                <td><asp:Label ID="lblAccountNameSummary" runat="server" /></td>
                <td>&nbsp;</td>
                <td><b><asp:Label ID="lblAccountNoCapSummary" runat="server" /></b></td>
                <td><asp:Label ID="lblAccountNoSummary" runat="server" /></td>
            </tr>
            <tr>
                <td><b><asp:Label ID="lblBankNameCapSummary" runat="server" /></b></td>
                <td><asp:Label ID="lblBankNameSummary" runat="server"/></td>
                <td>&nbsp;</td>
                <td><b><asp:Label ID="lblIbanCapSummary" runat="server" /></b></td>
                <td><asp:Label ID="lblIbanSummary" runat="server" /></td>
            </tr>
            <tr>
                <td><b><asp:Label ID="lblBinCapSummary" runat="server" /></b></td>
                <td><asp:Label ID="lblBinSummary" runat="server" /></td>
                <td>&nbsp;</td>
                <td><b><asp:Label ID="lblBicCapSummary" runat="server" /></b></td>
                <td><asp:Label ID="lblBicSummary" runat="server" /></td>
            </tr>
        </table>
    </asp:View>
</asp:MultiView>
