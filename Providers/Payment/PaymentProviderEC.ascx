<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PaymentProviderEC.ascx.cs" Inherits="Bitboxx.DNNModules.BBStore.Providers.Payment.PaymentProviderEC" %>
<asp:MultiView ID="pnlShow" runat="server">
    <asp:View ID="View" runat="server">
        <table>
            <tr>
                <td><asp:Image runat="server" ID="imgDebit" ImageUrl="~/Desktopmodules/BBStore/Providers/Payment/Images/debit.png"/></td>
                <td style="text-align: justify"><asp:Label ID="lblDescription" runat="server" /></td>
            </tr>
        </table>
        <table style="width: 400px;">
            <tr>
                <td>
                    <table>
                        <tr>
                            <td><asp:Label ID="lblCountryView" runat="server"  ResourceKey="lblCountry.Text" /></td>
                            <td><asp:DropDownList ID="ddlCountry" runat="server" /></td>
                        </tr>
                        <tr>
                            <td><asp:Label ID="lblAccountNameView" runat="server"  ResourceKey="lblAccountName.Text" /></td>
                            <td><asp:TextBox ID="txtAccountName" Columns="40" runat="server" /></td>
                        </tr>
                        <tr>
                            <td><asp:Label ID="lblBankNameView" runat="server" ResourceKey="lblBankName.Text" /></td>
                            <td><asp:TextBox ID="txtBankName" Columns="40" runat="server"/></td>
                        </tr>
                        <tr>
                            <td><asp:Label ID="lblBinView" runat="server" ResourceKey="lblBin.Text" /></td>
                            <td><asp:TextBox ID="txtBin" Columns="16" runat="server" /></td>
                        </tr>
                        <tr>
                            <td><asp:Label ID="lblAccountNoView" runat="server" ResourceKey="lblAccountNo.Text" /></td>
                            <td><asp:TextBox ID="txtAccountNo" Columns="20" runat="server" /></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
               <td>
                    <div style ="border: medium double #AAAAAA;padding:10px">
                        <table>
                            <tr>
                                <td><asp:Label ID="lblIbanView" runat="server" ResourceKey="lblIban.Text" /></td>
                                <td><asp:TextBox ID="txtIban" Columns="34" runat="server" MaxLength="34"/></td>
                            </tr>
                            <tr>
                                <td><asp:Label ID="lblBicView" runat="server" ResourceKey="lblBic.Text" /></td>
                                <td><asp:TextBox ID="txtBic" Columns="13" runat="server" MaxLength="11" /></td>
                            </tr>
                        </table>
                    </div>
                    <asp:HiddenField runat="server" ID="hidValid" Value="true"/>
                </td> 
            </tr>
        </table>
    </asp:View>
    <asp:View ID="Edit" runat="server">
        <h3><asp:Label ID="lblTitle" runat="server" Text="Title" /></h3>
    </asp:View>
    <asp:View ID="Summary" runat="server">
        <table style="border: 0; border-collapse: collapse;">
            <tr>
                <td><b><asp:Label ID="lblAccountNameSummary" runat="server" /></b></td>
                <td><asp:Label ID="lblAccountName" runat="server" /></td>
                <td>&nbsp;</td>
                <td><b><asp:Label ID="lblAccountNoSummary" runat="server" /></b></td>
                <td><asp:Label ID="lblAccountNo" runat="server" /></td>

            </tr>
            <tr>
                <td><b><asp:Label ID="lblBankNameSummary" runat="server" /></b></td>
                <td><asp:Label ID="lblBankName" runat="server" /></td>
                <td>&nbsp;</td>
                <td><b><asp:Label ID="lblIbanSummary" runat="server" /></b></td>
                <td><asp:Label ID="lblIban" runat="server" /></td>

            </tr>
            <tr>
                <td><b><asp:Label ID="lblBinSummary" runat="server" /></b></td>
                <td><asp:Label ID="lblBin" runat="server" /></td>
                <td>&nbsp;</td>
                <td><b><asp:Label ID="lblBicSummary" runat="server" /></b></td>
                <td><asp:Label ID="lblBic" runat="server" /></td>

            </tr>
            <tr>
                <td><b><asp:Label ID="lblCountrySummary" runat="server" /></b></td>
                <td colspan="4"><asp:Label ID="lblCountry" runat="server" /></td>
            </tr>
        </table>
    </asp:View>
</asp:MultiView>
