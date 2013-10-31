<%@ Control Language="C#" Inherits="Bitboxx.DNNModules.BBStore.ViewSearch" AutoEventWireup="true" CodeBehind="ViewSearch.ascx.cs" %>
<%@ Register TagPrefix="bb" TagName="FeatureGridControl" Src="Controls/FeatureGridControl.ascx" %>
<div id="SearchText" runat="server" class="bbstore-search dnnClear">
    <asp:Panel runat="server" ID="pnlSearchReset">
	    <asp:Label ID="lblSearchResetCap" runat="server" Resourcekey="lblSearchResetCap.Text" CssClass="bbstore-searchTitle" />
	    <table cellpadding="3" cellspacing="0" border="0" style="width:100%">
		    <tr>
			    <td><asp:Button ID="cmdSearchReset" runat="server" Resourcekey="cmdSearchReset.Text" OnClick="cmdSearchReset_Click"></asp:Button></td>
			    <td width="16px">&nbsp;</td>
		    </tr>
	    </table>
    </asp:Panel>
    <asp:Panel ID="pnlSearchProductGroup" runat="server">
	    <asp:Label ID="lblSearchProductGroupCap" runat="server" Resourcekey="lblSearchProductGroupCap.Text" CssClass="bbstore-searchTitle" />
	    <table cellpadding="3" cellspacing="0" border="0" style="width:100%">
		    <tr>
			    <td><span class="bbstore-searchValue"><asp:Label ID="lblSearchProductGroup" runat="server"  /></span></td>
			    <td width="16px"><asp:ImageButton ID="cmdDeleteProductGroup" runat="server" ResourceKey="Delete" ImageUrl="~/images/Delete.gif" onclick="cmdDeleteProductGroup_Click" CssClass="CommandButton"/></td>
		    </tr>
	    </table>
    </asp:Panel>
    <asp:Panel ID="pnlSearchFeatureList" runat="server">
	    <asp:Label ID="lblSearchFeatureListCap" runat="server" CssClass="bbstore-searchTitle" />
	    <table cellpadding="3" cellspacing="0" border="0" style="width:100%">
		    <tr>
			    <td><span class="bbstore-searchValue"><asp:Label ID="lblSearchFeatureList" runat="server"  /></span></td>
			    <td width="16px"><asp:ImageButton ID="cmdDeleteFeatureList" runat="server" ResourceKey="Delete" ImageUrl="~/images/Delete.gif" onclick="cmdDeleteFeatureList_Click" CssClass="CommandButton"/></td>
		    </tr>
	    </table>
    </asp:Panel>
    <asp:Panel ID="pnlSearchText" runat="server">
	    <asp:Label ID="lblSearchTextCap" runat="server" Resourcekey="lblSearchTextCap.Text" CssClass="bbstore-searchTitle" />
	    <asp:MultiView ID="MultiViewText" runat="server" ActiveViewIndex="0">
		    <asp:View ID="viewSearchText" runat="server">
			    <table cellpadding="3" cellspacing="0" border="0" style="width:100%">
				    <tr>
					    <td><asp:TextBox ID="txtSearchText" runat="server" Width="100%" AutoPostBack="True" ontextchanged="cmdSearchText_Click" /></td>
					    <td width="16px">&nbsp;</td>
				    </tr>
			    </table>
		    </asp:View>
		    <asp:View ID="viewShowText" runat="server">
			    <table cellpadding="3" cellspacing="0" border="0" style="width:100%">
				    <tr>
					    <td><span class="bbstore-searchValue"><asp:Label ID="lblSearchText" runat="server" /></span></td>
					    <td width="16px"><asp:ImageButton ID="cmdDeleteText" runat="server" ResourceKey="Delete" ImageUrl="~/images/Delete.gif" onclick="cmdDeleteText_Click" CssClass="CommandButton"/></td>
				    </tr>
			    </table>
		    </asp:View>
	    </asp:MultiView>
    </asp:Panel>
    <asp:Panel ID="pnlSearchStatic" runat="server">
	    <asp:Label ID="lblSearchStaticCap" runat="server" Resourcekey="lblSearchStaticCap.Text" CssClass="bbstore-searchTitle" />
	    <asp:MultiView ID="MultiViewStatic" runat="server" ActiveViewIndex="0">
		    <asp:View ID="viewSearchStatic" runat="server">
			    <table cellpadding="3" cellspacing="0" border="0" style="width:100%">
				    <tr>
					    <td><asp:DropDownList ID="cboSearchStatic" runat="server" Width="100%"  AutoPostBack="True"  OnSelectedIndexChanged="cmdSearchStatic_Click"/></td>
					    <td width="16px">&nbsp;</td>
				    </tr>
			    </table>
		    </asp:View>
		    <asp:View ID="viewShowStatic" runat="server">
			    <table cellpadding="3" cellspacing="0" border="0" style="width:100%">
				    <tr>
					    <td><span class="bbstore-searchValue"><asp:Label ID="lblSearchStatic" runat="server" /></span></td>
					    <td width="16px"><asp:ImageButton ID="cmdDeleteStatic" runat="server" ResourceKey="Delete" ImageUrl="~/images/Delete.gif" onclick="cmdDeleteStatic_Click" CssClass="CommandButton"/></td>
				    </tr>
			    </table>
		    </asp:View>
	    </asp:MultiView>
    </asp:Panel>
    <asp:Panel ID="pnlSearchPrice" runat="server">
	    <asp:Label ID="lblSearchPrice" runat="server" Resourcekey="lblSearchPriceCap.Text" CssClass="bbstore-searchTitle"/>
	    <asp:MultiView ID="MultiViewPrice" runat="server" ActiveViewIndex="0">
		    <asp:View ID="viewSearchPrice" runat="server">
			    <table cellpadding="3" cellspacing="0" border="0" width="100%">
				    <tr>
					    <td style="padding-left: 0px;">
						    <table cellpadding="0" cellspacing="0" border="0" width="100%">
							    <tr>
								    <td width="49%"><asp:TextBox ID="txtStartPrice" runat="server" Width="100%" /> </td>
								    <td width="49%"><asp:TextBox ID="txtEndPrice" runat="server" Width="100%" /></td>
								    <td width="20px"><asp:ImageButton runat="server" ID="cmdSearchPrice" OnClick="cmdSearchPrice_Click" ImageUrl="~/images/Add.gif"/></td>
							    </tr>
						    </table>
					    </td>
					    <td width="16px">&nbsp;</td>
				    </tr>
			    </table>
		    </asp:View>
		    <asp:View ID="viewShowPrice" runat="server">
			    <table cellpadding="3" cellspacing="0" border="0" style="width:100%">
				    <tr>
					    <td><span class="bbstore-searchValue"><asp:Label ID="lblStartPrice" runat="server" /> - <asp:Label ID="lblEndPrice" runat="server" /> <asp:Label ID="lblCurrency" runat="server" /></span></td>
					    <td width="16px"><asp:ImageButton ID="cmdDeletePrice" runat="server" ResourceKey="Delete" ImageUrl="~/images/Delete.gif" onclick="cmdDeletePrice_Click" CssClass="CommandButton"/></td>
				    </tr>
			    </table>
		    </asp:View>
	    </asp:MultiView>
    </asp:Panel>
    <asp:Panel ID="pnlFeatures" runat="server">
	    <bb:FeatureGridControl ID="FeatureGrid" runat="server" Mode="Search" />
    </asp:Panel>
</div>