<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewCartSelectAddresses.ascx.cs" Inherits="Bitboxx.DNNModules.BBStore.ViewCartSelectAddresses" %>
<%@ Register TagPrefix="asp" Assembly="System.Web.Extensions" Namespace="System.Web.UI.WebControls"  %>
<div class="bbstore-cart-selectaddresses dnnClear">
    <p>&nbsp;</p>
	<div class="dnnFormMessage dnnFormInfo" id="pnlIntro" runat="server">
        <asp:Label ID="lblIntro" runat="server"  />
    </div>
    <p>&nbsp;</p>
    <asp:ListView id="lstCustomerAddresses" runat="server"  GroupItemCount="3" OnItemDataBound="lstCustomerAddresses_ItemDataBound"  >
        <LayoutTemplate>
            <table style="width: 100%" >
                <tr>
                <td>
                    <table border="0" cellpadding="5" style="width: 100%">
                        <asp:PlaceHolder runat="server" ID="groupPlaceHolder"></asp:PlaceHolder>
                    </table>
                </td>
                </tr>
            </table>
        </LayoutTemplate>
        <GroupTemplate>
            <tr>
                <asp:PlaceHolder runat="server" ID="itemPlaceHolder"></asp:PlaceHolder>
            </tr>
        </GroupTemplate>
        <ItemTemplate>
            <td valign="top" runat="server" id="cellAddress">
			    <asp:HiddenField runat="server" ID="hidCustomerAddressId" Value="-1"/>
			    <div class="bbstore-grid-alternaterow">
				    <asp:CheckBoxList runat="server" ID="lstAddressType" DataValueField="SubscriberAddressTypeId" DataTextField="AddressType" RepeatDirection="Vertical" />
			    </div>
			    <div class="bbstore-grid-row" style="padding: 5px">
				    <asp:Label ID="lblAddress" runat="server" />
			    </div>
                <p id="P1" runat="server" >
                    <asp:ImageButton ID="imgAdrEditlst" runat="server" resourcekey="cmdEdit"  ImageUrl="~/images/edit.gif"  OnClick="cmdAdrEdit_Click" /><asp:LinkButton cssClass="CommandButton" id="cmdAdrEditlst" runat="server" resourcekey="cmdEdit" OnClick="cmdAdrEdit_Click"/>
                    <asp:ImageButton ID="imgAdrDeletelst" runat="server" resourcekey="cmdDelete"  ImageUrl="~/images/delete.gif"  OnClick="cmdAdrDelete_Click" /><asp:LinkButton cssClass="CommandButton" id="cmdAdrDeletelst" runat="server" resourcekey="cmdDelete" OnClick="cmdAdrDelete_Click"/>
				    <asp:ImageButton ID="imgAdrNewlst" runat="server" resourcekey="cmdAdrNew.Text"  ImageUrl="~/images/add.gif"  OnClick="cmdAdrNew_Click" /><asp:LinkButton cssClass="CommandButton" id="cmdAdrNewlst" runat="server" resourcekey="cmdAdrNew.Text" OnClick="cmdAdrNew_Click"/>
                </p>
            </td>
        </ItemTemplate>
    </asp:ListView>        
    <div style="text-align: right">
	    <asp:Button runat="server" ID="cmdAdrUse" OnClick="cmdAdrUse_Click" resourcekey="cmdAdrUse"/>
    </div>
</div>