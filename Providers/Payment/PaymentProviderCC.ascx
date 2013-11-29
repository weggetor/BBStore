<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PaymentProviderCC.ascx.cs" Inherits="Bitboxx.DNNModules.BBStore.Providers.Payment.PaymentProviderCC" %>
<asp:MultiView ID="pnlShow" runat="server">
    <asp:View ID="View" runat="server">
        <table>
            <tr>
                <td><asp:Image runat="server" ID="imgCC" ImageUrl="~/Desktopmodules/BBStore/Providers/Payment/Images/creditcard.png"/></td>
                <td style="text-align: justify"><asp:Label ID="lblDescription" runat="server" /></td>
            </tr>
        </table>
        <ul class="cards" id="ulCards" runat="server">
            
        </ul>
        <table style="width: 400px;clear:both">
            <tr>
                <td>
                    <table>
                        <tr>
                            <td><asp:Label ID="lblCCNoCapView" runat="server"/></td>
                            <td colspan="4"><asp:TextBox ID="txtCCNo" Columns="40" runat="server" /></td>
                        </tr>
                        <tr>
                            <td><asp:Label ID="lblCCValidCapView" runat="server" /></td>
                            <td><asp:TextBox ID="txtCCValid" Columns="6" MaxLength="5" runat="server"/></td>
                            <td>&nbsp;</td>
                            <td><asp:Label ID="lblCCCvvCapView" runat="server" /></td>
                            <td><asp:TextBox ID="txtCCCvv" Columns="4" MaxLength="3" runat="server" /></td>
                        </tr>
                        <tr>
                            <td><asp:Label ID="lblCCNameCapView" runat="server" /></td>
                            <td colspan="4"><asp:TextBox ID="txtCCName" Columns="40" runat="server" /></td>
                        </tr>
                    </table>
                    <asp:HiddenField runat="server" ID="hidCCType" EnableViewState="True"/>
                    <asp:HiddenField runat="server" ID="hidCCTypeList" EnableViewState="True"/>
                </td>
            </tr>
         </table>

    </asp:View>
    <asp:View ID="Edit" runat="server">
        <h3><asp:Label ID="lblTitle" runat="server" Text="Title" /></h3>
        <asp:CheckBoxList runat="server" ID="lstCCType">
            <asp:ListItem Text="American Express" Value="amex" />
            <asp:ListItem Text="Diners Club Carte Blanche" Value="diners_club_carte_blanche" />
            <asp:ListItem Text="Diners Club International" Value="diners_club_international" />
            <asp:ListItem Text="Discover Card" Value="discover" />
            <asp:ListItem Text="JCB" Value="jcb" />
            <asp:ListItem Text="Laser" Value="laser" />
            <asp:ListItem Text="Maestro" Value="maestro" />
            <asp:ListItem Text="MasterCard" Value="mastercard" />
            <asp:ListItem Text="Visa" Value="visa" />
            <asp:ListItem Text="Visa Electron" Value="visa_electron" />
        </asp:CheckBoxList>
    </asp:View>
    <asp:View ID="Summary" runat="server">
        <table style="border: 0; border-collapse: collapse;">
            <tr>
                <td><b><asp:Label ID="lblCCTypeCapSummary" runat="server" /></b></td>
                <td><asp:Label ID="lblCCTypeSummary" runat="server" /></td>
            </tr>
            <tr>
                <td><b><asp:Label ID="lblCCNoCapSummary" runat="server" /></b></td>
                <td><asp:Label ID="lblCCNoSummary" runat="server" /> (<asp:Label ID="lblCCCvvSummary" runat="server" />)</td>
            </tr>
            <tr>
                <td><b><asp:Label ID="lblCCValidCapSummary" runat="server" /></b></td>
                <td><asp:Label ID="lblCCValidSummary" runat="server" /></td>
            </tr>
            <tr>
                <td><b><asp:Label ID="lblCCNameCapSummary" runat="server" /></b></td>
                <td><asp:Label ID="lblCCNameSummary" runat="server" /></td>
            </tr>
        </table>
    </asp:View>
</asp:MultiView>
<script type="text/javascript">
    $(document).ready(function () {
        var txtCCNo = $("#<%=txtCCNo.ClientID%>");
        var hidCCType = $("#<%=hidCCType.ClientID%>");
        txtCCNo.validateCreditCard(function (result) {
            if (result.length_valid == true && result.luhn_valid == true && result.card_type != null) {
                txtCCNo.addClass("valid");
                hidCCType.val(result.card_type.name);
            } else {
                txtCCNo.removeClass("valid");
                hidCCType.val("");
            }
            
            $(".cards li").each(function() {
                var current = $(this);
                if (result.card_type != null) {
                    if (current.hasClass(result.card_type.name)) {
                        current.removeClass("off");
                    } else {
                        current.addClass("off");
                    }
                } else {
                    current.removeClass("off");
                }
            });
        }, { accept: [<%=hidCCTypeList.Value%>] });
    });
</script>
