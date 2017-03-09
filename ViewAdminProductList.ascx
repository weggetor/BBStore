<%@ Control Language="C#" Inherits="Bitboxx.DNNModules.BBStore.ViewAdminProductList" AutoEventWireup="true" CodeBehind="ViewAdminProductList.ascx.cs" %>
<div class="bbstore-admin bbstore-admin-productlist dnnForm">
    <table style="width:100%" class="bbstore-admin-editgrid-search">
	    <tr>
		    <td><asp:Label ID="lblFilterCap" runat="server" ResourceKey="lblFilterCap.Text" /></td>
		    <td><asp:Label ID="lblProduct" ResourceKey="lblProduct.Text" runat="server" /></td>
		    <td><asp:TextBox ID="txtProduct" runat="server" /></td>
		    <td align="left"><asp:Button ID="cmdFilter" CssClass="dnnSecondaryAction" runat="server" onclick="cmdFilter_Click" ResourceKey="cmdFilter.Text" /></td>
		    <td width="99%" align="right"><asp:Label ID="lblPageSize" runat="server" ResourceKey="lblPageSize.Text" /></td>
		    <td align="right">
		        <asp:DropDownList ID="ddlProductPageSize" runat="server" OnSelectedIndexChanged="ddlProductPageSize_SelectedIndexChanged" AutoPostBack="true" /> 
		    </td>
	    </tr>
    </table>    
    <asp:GridView ID="grdProduct" runat="server" 
	    AutoGenerateColumns="False" 
	    DataKeyNames="SimpleProductId" 
	    AllowPaging="True" 
	    AllowSorting="True" 
	    CellPadding="2"
	    GridLines="None" 
	    Width="100%"
	    ShowFooter="True" 
	    EnableModelValidation="True" 
	    onpageindexchanging="grdProduct_PageIndexChanging" 
	    onrowcommand="grdProduct_RowCommand" 
	    onsorting="grdProduct_Sorting"
	    onrowdatabound="grdProduct_RowDataBound" 
	    ondatabound="grdProduct_DataBound"
	    onrowcreated="grdProduct_RowCreated" 
	    onrowdeleting="grdProduct_RowDeleting"	>
	    <Columns>
		    <asp:TemplateField ShowHeader="False">
			    <ItemTemplate>
				    <asp:ImageButton ID="imgDelete" runat="server" CausesValidation="false" CommandName="Delete"  CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" ImageUrl="~/images/Delete.gif" />
				    <asp:LinkButton ID="cmdSelect" runat="server" CausesValidation="false" CommandName="Select"></asp:LinkButton>
			    </ItemTemplate>
			    <FooterTemplate>
				    <asp:ImageButton ID="imgNew" runat="server" ImageAlign="AbsMiddle" CausesValidation="false" CommandName="Insert" ImageUrl="~/images/Add.gif" />
				    <asp:LinkButton ID="cmdNew" CommandName="Insert" CssClass="CommandButton" ResourceKey="New" runat="server"></asp:LinkButton>
			    </FooterTemplate>
		    </asp:TemplateField>
		    <asp:BoundField DataField="SimpleProductId" HeaderText="SimpleProductId" SortExpression="SimpleProductId" />
		    <asp:BoundField DataField="ItemNo" HeaderText="ItemNo" SortExpression="ItemNo" />
		    <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
		    <asp:BoundField DataField="UnitCost" HeaderText="UnitCost"  SortExpression="UnitCost" DataFormatString="{0:n2}" >
			    <ItemStyle HorizontalAlign="Right" />
		    </asp:BoundField>
	    </Columns>
	    <RowStyle CssClass="bbstore-admin-editgrid-row" />
	    <SelectedRowStyle CssClass="bbstore-admin-editgrid-selectedrow" />
	    <AlternatingRowStyle CssClass="bbstore-admin-editgrid-alternaterow" />
	    <EditRowStyle CssClass="bbstore-admin-editgrid-selectedrow" />
	    <FooterStyle CssClass="bbstore-admin-editgrid-footer" />
	    <HeaderStyle CssClass="bbstore-admin-editgrid-header" />
	    <PagerStyle CssClass="bbstore-admin-editgrid-pager" />
	    <EmptyDataTemplate>
		    <table cellpadding="0" cellspacing="0" border="0" width="100%">
			    <tr>
				    <td valign="middle" class="bbstore-admin-editgrid-footer">
					    <asp:ImageButton ID="imgNew" runat="server" ImageAlign="AbsMiddle" CausesValidation="false" CommandName="Insert" ImageUrl="~/images/Add.gif" />
					    <asp:LinkButton ID="cmdNew" CommandName="Insert" CssClass="CommandButton" ResourceKey="New" runat="server"></asp:LinkButton>
				    </td>
			    </tr>
		    </table>
	    </EmptyDataTemplate>
	    <PagerTemplate>
		    <table cellpadding="0" cellspacing="0" border="0" width="100%">
			    <tr>
				    <td width="16" valign="middle"><asp:ImageButton ID="imgFirst" runat="server" ImageUrl="images/firstgrey.png" CommandName="Page" CommandArgument="First" /></td>
				    <td width="16" valign="middle"><asp:ImageButton ID="imgPrev" runat="server" ImageUrl="images/prevgrey.png" CommandName="Page" CommandArgument="Prev"/></td>
				    <td valign="middle" style="white-space:nowrap">
					    <asp:Label ID="lblPage" ResourceKey="lblPage.Text" runat="server"></asp:Label>
					    <asp:DropDownList ID="ddlProductPages" runat="server" AutoPostBack="True" 
					    onselectedindexchanged="ddlProductPages_SelectedIndexChanged"></asp:DropDownList>
					    <asp:Label ID="lblOf" ResourceKey="lblOf.Text" runat="server"></asp:Label>
					    <asp:Label ID="lblPageCount" runat="server"></asp:Label>
				    </td>
				    <td width="16" valign="middle"><asp:ImageButton ID="imgNext" runat="server" ImageUrl="images/nextgrey.png" CommandName="Page" CommandArgument="Next"/></td>
				    <td width="20%" style="text-align:left" valign="middle"><asp:ImageButton ID="imgLast" runat="server" ImageUrl="images/lastgrey.png" CommandName="Page" CommandArgument="Last"/></td>
				    <td width="99%" style="text-align:right"><asp:Label ID="lblProductItemCount" runat="server"></asp:Label></td>
			    </tr>
		    </table>
	    </PagerTemplate>
    </asp:GridView>
</div>	