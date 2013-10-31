<%@ Control Language="C#" Inherits="Bitboxx.DNNModules.BBStore.ViewList" AutoEventWireup="true" CodeBehind="ViewList.ascx.cs" %>
<%@ Register TagPrefix="asp" Assembly="System.Web.Extensions" Namespace="System.Web.UI.WebControls"  %>
<div class="bbstore-productlist dnnClear">
    <asp:Literal ID="ltrHead" runat="server" Mode="PassThrough"/>
    <div id="divShowAllTop" runAt="server">
	    <asp:LinkButton ID="lnkShowAllTop" runat="server" onclick="lnkShowAll_Click" visible="false"/>
    </div>
    <asp:Panel ID="pnlListHead" runat="server">
	    <table border="0" cellpadding="5" cellspacing="0" class="bbstore-grid-alternaterow" style="width:100%">
		    <tr>
			    <td style="text-align:left">
			        <asp:Label ID="lblSort" runat="server" ResourceKey="lblSort.Text" />
				    <asp:DropDownList AutoPostBack="true" ID="ddlSortBox" runat="server" OnSelectedIndexChanged="ddlSort_SelectedIndexChanged" />
			    </td>
			    <td style="text-align:right">

			        <asp:Label ID="lblPageSize" runat="server" ResourceKey="lblPageSize.Text" />
				    <asp:DropDownList AutoPostBack="true" ID="ddlPageSize" runat="server" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged" />
			    </td>
		    </tr>
	    </table>
    </asp:Panel>
    <asp:ListView ID="lstProducts" runat="server" GroupItemCount="3" 
        onitemcreated="lstProducts_ItemCreated" 
        onselectedindexchanging="lstProducts_SelectedIndexChanging"
        DataKeyNames="SimpleProductId" onsorting="lstProducts_Sorting" >
        <Layouttemplate>
            <table runat="server" style="width:100%" border="0" cellpadding="0" cellspacing="0">
                <tr runat="server" id="groupPlaceholder" />
            </table>
        </Layouttemplate>
        <GroupTemplate>
            <tr runat="server"><td id="itemPlaceholder" /></tr>
        </GroupTemplate>
        <ItemTemplate>
            <td runat="server" style="vertical-align:top"  width='<%# GetWidth()+"%" %>'>
		       <asp:Panel ID="pnlItem" runat="server">
				    <asp:Button ID="cmdSelect" CommandName="Select" runat="server" Visible="false" />
				    <asp:PlaceHolder ID="productPlaceHolder" runat="server" />
			    </asp:Panel>
            </td>
        </ItemTemplate>
    </asp:ListView>
    <asp:Panel ID="pnlListFooter" runat="server">
	    <div class="bbstore-grid-alternaterow" style="margin-top:2px; padding:5px 2px 0px 5px">
		    <asp:DataPager ID="Pager" runat="server" PageSize="6" 
			    PagedControlID="lstProducts" onprerender="Pager_PreRender" EnableViewState="False" >                       
			    <Fields>
				    <asp:TemplatePagerField>
					    <PagerTemplate>
						    <asp:Label ID="lblPage" runat="server" ResourceKey="lblPage.Text"></asp:Label>
						    <asp:Label runat="server" ID="CurrentPageLabel" Text="<%# Container.TotalRowCount>0 ? (Container.StartRowIndex / Container.PageSize) + 1 : 0 %>" />
						    <asp:Label ID="lblOfPage" runat="server" ResourceKey="lblOfPage.Text"></asp:Label>
						    <asp:Label runat="server" ID="TotalPagesLabel" Text="<%# Math.Ceiling ((double)Container.TotalRowCount / Container.PageSize) %>" />
						    (<asp:Label runat="server" ID="TotalItemsLabel" Text="<%# Container.TotalRowCount%>" /> 
						    <asp:Label ID="lblProduct" runat="server" ResourceKey="lblProduct.Text"></asp:Label>):
					    </PagerTemplate>
				    </asp:TemplatePagerField>
				    <asp:NumericPagerField ButtonCount="10" />
			    </Fields>
		    </asp:DataPager>
	    </div>
    </asp:Panel>
    <asp:Literal runat="server" ID="ltrEmpty" Visible="False"/>
    <div id="divShowAllBottom" runat="server">
	    <asp:LinkButton ID="lnkShowAllBottom" runat="server" onclick="lnkShowAll_Click" Visible="false" />
    </div>
    <asp:Literal ID="ltrFoot" runat="server" Mode="PassThrough" />
</div>