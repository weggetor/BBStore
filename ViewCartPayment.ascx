<%@ Control Language="C#" Inherits="Bitboxx.DNNModules.BBStore.ViewCartPayment" AutoEventWireup="true" CodeBehind="ViewCartPayment.ascx.cs" %>
<%@ Register TagPrefix="asp" Assembly="System.Web.Extensions" Namespace="System.Web.UI.WebControls"  %>
<div class="bbstore-cart-payment dnnClear" runat="server" id="divMain">
    <p>&nbsp;</p>
    <div class="dnnFormMessage dnnFormInfo">
        <asp:Label ID="lblPayment" runat="server" Resourcekey="lblPayment.Text" />
    </div>
    <p>&nbsp;</p>
    <div id="jaccordion">
        <asp:ListView ID="lstPaymentProvider" runat="server"
            OnItemDataBound="lstPaymentProvider_ItemDataBound"
            DataKeyNames="SubscriberPaymentProviderId" EnableViewState="True">
            <LayoutTemplate>
                <div id="itemPlaceHolder" runat="server" ></div>
            </LayoutTemplate>
            <ItemTemplate>
                <asp:Panel runat="server" ID="pnlSubscriberPaymentProvider">
                    <asp:Literal runat="server" ID="ltrTitle" />
                    <div class="bbstore-cart-payment-content">
                        <asp:PlaceHolder runat="server" ID="phPaymentProvider"/>
                        <asp:LinkButton runat="server" ID="lnkSelect" />
                    </div>
                </asp:Panel>
            </ItemTemplate>
        </asp:ListView>
    </div>
    <asp:HiddenField runat="server" ID="hidPPSelectedIndex" EnableViewState="true" Value="-1"/>
    <p>&nbsp;</p>
    <div style="text-align:right;"><asp:Button ID="cmdConfirm" runat="server" Resourcekey="NextStep.Text" onclick="cmdConfirm_Click" /><asp:ImageButton runat="server" ID="cmdPaypal" ImageUrl="https://www.paypal.com/en_US/i/btn/btn_xpressCheckout.gif" OnClick="cmdPaypal_Click" /></div>
</div>
<script>
    $(function () {
        var changeFunc = function (event, ui) {
            if (ui.newHeader) {
                $(ui.newHeader).attr("class", "bbstore-cart-payment-selectedheader");
                var hiddenElement = $("#<%=hidPPSelectedIndex.ClientID%>");
                hiddenElement.val(ui.options.active);
            }
            if (ui.oldHeader) {
                $(ui.oldHeader).attr("class", "bbstore-cart-payment-header");
            }
            if (ui.newContent && $(ui.newContent).find("img").attr("src").contains("Paypal.png")) {
                $("#<%=cmdConfirm.ClientID%>").hide();
                $("#<%=cmdPaypal.ClientID%>").show();
            } else {
                $("#<%=cmdConfirm.ClientID%>").show();
                $("#<%=cmdPaypal.ClientID%>").hide();
            }
        };
        var activateFunc = function (event, ui) {
            if (ui.newHeader) {
                $(ui.newHeader).attr("class", "bbstore-cart-payment-selectedheader");
                var hiddenElement = $("#<%=hidPPSelectedIndex.ClientID%>");
                var active = $("#jaccordion").accordion("option", "active");
                hiddenElement.val(active);
            }
            if (ui.oldHeader) {
                $(ui.oldHeader).attr("class", "bbstore-cart-payment-header");
            }
            if (ui.newPanel && $(ui.newPanel).find("img").attr("src").indexOf("Paypal.png") > -1) {
                $("#<%=cmdConfirm.ClientID%>").hide();
                $("#<%=cmdPaypal.ClientID%>").show();
            } else {
                $("#<%=cmdConfirm.ClientID%>").show();
                $("#<%=cmdPaypal.ClientID%>").hide();
            }
        };
        if (parseInt(jQuery.fn.jquery.split('.').join('')) > 172) {
            $("#jaccordion").accordion({ heightStyle: "content", header: "div h3", activate: activateFunc, active: false, collapsible: true });
        } else {
            $("#jaccordion").accordion({ autoHeight: false, header: "div h3", change: changeFunc, active: false });
        }
        if ((typeof paneIndex != 'undefined'))
            $("#jaccordion").accordion("option", "active", paneIndex);
        $("#<%=cmdPaypal.ClientID%>").hide();
    });
</script>
