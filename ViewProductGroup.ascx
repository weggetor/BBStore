<%@ Control Language="C#" Inherits="Bitboxx.DNNModules.BBStore.ViewProductGroup" AutoEventWireup="true" CodeBehind="ViewProductGroup.ascx.cs" %>
<div class="bbstore-productgroup dnnClear" id="maindiv" runat="server">
    <asp:MultiView ID="MultiView1" runat="server">
        <asp:View ID="viewTemplate" runat="server">
            <asp:ListView ID="lstProductGroups" runat="server" GroupItemCount="3" 
                onitemcreated="lstProductGroups_ItemCreated" 
                onselectedindexchanging="lstProductGroups_SelectedIndexChanging"
                DataKeyNames="ProductGroupId" >
                <Layouttemplate>
                    <table id="Table1" runat="server" cellpadding="0" cellspacing="0" border="0" style="width:100%">
                        <tr runat="server" id="groupPlaceholder" />
                    </table>
                </Layouttemplate>
                <GroupTemplate>
                    <tr id="Tr1" runat="server"><td id="itemPlaceholder" /></tr>
                </GroupTemplate>
                <ItemTemplate>
                    <td id="Td1" runat="server" class="bbstore-productgroup-list">
                        <asp:LinkButton ID="lnkProductGroup" runat="server"  CommandName="Select">
                            <asp:PlaceHolder ID="productGroupPlaceHolder" runat="server" />
                        </asp:LinkButton>
                    </td>
                </ItemTemplate>
            </asp:ListView>

        </asp:View>
        <asp:View ID="viewTree" runat="server">
            <asp:TreeView ID="treeProductGroup" ShowCheckBoxes="None"   
                runat="server" ExpandDepth="0"
                ontreenodepopulate="treeProductGroup_TreeNodePopulate" 
                onselectednodechanged="treeProductGroup_SelectedNodeChanged" 
			    CssClass="bbstore-productgroup-tree" NodeIndent="15">
            </asp:TreeView>
        </asp:View>
	    <asp:View ID="viewDropDown" runat="server">
		    <asp:PlaceHolder ID="phDropDown" runat="server" />
	    </asp:View>
    </asp:MultiView>
    <asp:Panel ID="pnlAdmin" runat="server" Visible="false">
	    <hr />
	    <div style="text-align:right">
	    <span style="white-space:nowrap">
		    <asp:ImageButton ID="imgEdit" runat="server" resourcekey="cmdEdit" ImageUrl="~/images/edit.gif"
			    OnClick="cmdEdit_Click" />&nbsp;<asp:LinkButton CssClass="CommandButton" ID="cmdEdit"
				    runat="server" resourcekey="cmdEdit" OnClick="cmdEdit_Click" /></span>&nbsp;
    
	    </div>
    </asp:Panel>
</div>