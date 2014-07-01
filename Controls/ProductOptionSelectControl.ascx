<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductOptionSelectControl.ascx.cs" Inherits="Bitboxx.DNNModules.BBStore.ProductOptionSelectControl" %>
<%@ Register tagPrefix="asp" namespace="System.Web.UI" assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" %>
<asp:ListView ID="ProductOptionListView" runat="server" onitemdatabound="ProductOptionListView_ItemDataBound">
    <LayoutTemplate>
        <div>
            <table cellpadding="1" cellspacing="0" class="grdTotal">
                <span id="itemPlaceholder" runat="server" />
            </table>
        </div>
    </LayoutTemplate>
     <ItemTemplate>
			<tr>
                <td style="vertical-align: top; ">
                	<asp:Literal ID="ltrCRLF" runat="server" Text="<br />" Visible="False"/>
                    <asp:Label ID="lblOption" runat="server"></asp:Label>&nbsp;
                </td>
                <td style="vertical-align: top; white-space: nowrap">
                    <asp:PlaceHolder runat="server" ID="phOptionValue" />
                    <asp:Label ID="lblMandatory" runat="server" Visible="false" Text="*"></asp:Label>
                </td>
            </tr>
            <tr id="trLine" style="vertical-align: top" runat="server" Visible="False">
				<td>&nbsp;</td>
				<td>
					<table cellpadding="1" cellspacing="0">
						<tr id="trAddImage" style="vertical-align: top" runat="server" Visible="False">
							<td><asp:Label runat="server" ID="lblImage">Image:</asp:Label></td>
							<td><asp:FileUpload ID="upOptionImage" runat="server" EnableViewState="true" /></td>
						</tr>
						<tr id="trAddDesc" style="vertical-align: top" runat="server" Visible="False">
							<td><asp:Label runat="server" ID="lblDesc">Desc:</asp:Label></td>
							<td><asp:TextBox ID="txtOptionDescription" runat="server" TextMode="MultiLine" Rows="3" Columns="60" /></td>
						</tr>
					</table>
				</td>
			</tr>

    </ItemTemplate>
</asp:ListView>
