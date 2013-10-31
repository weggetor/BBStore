<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PaymentProviderInvoice.ascx.cs" Inherits="Bitboxx.DNNModules.BBStore.Providers.Payment.PaymentProviderInvoice" %>
<asp:MultiView ID="pnlShow" runat="server">
    <asp:View ID="View" runat="server">
        <table>
            <tr>
                <td><asp:Image runat="server" ID="imgCOD" ImageUrl="~/Desktopmodules/BBStore/Providers/Payment/Images/invoice.png"/></td>
                <td style="text-align: justify"><asp:Label ID="lblDescription" runat="server" /></td>
            </tr>
       </table>
    </asp:View>
    <asp:View ID="Edit" runat="server">
        <h3><asp:Label ID="lblTitle" runat="server" Text="Title" /></h3>
    </asp:View>
     <asp:View ID="Summary" runat="server">
    </asp:View>
</asp:MultiView>
