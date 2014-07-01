<%@ Control Language="C#" Inherits="Bitboxx.DNNModules.BBStore.ViewFeatureList" AutoEventWireup="true" CodeBehind="ViewFeatureList.ascx.cs" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="dnn" %>
<div class="bbstore-featurelist dnnClear">
    <asp:Literal ID="ltrHead" runat="server" Mode="PassThrough"/>
    <asp:Panel runat="server" ID="pnlList" Visible="false">
	    <asp:ListView ID="lstFeatures" runat="server" GroupItemCount="3" 
		    OnItemCreated="lstFeatures_ItemCreated" 
		    OnSelectedIndexChanging="lstFeatures_SelectedIndexChanging"
		    DataKeyNames="FeatureListItemId">
		    <Layouttemplate>
			    <table id="Table1" runat="server" style="width:100%" border="0" cellpadding="0" cellspacing="0">
				    <tr runat="server" id="groupPlaceholder" />
			    </table>
		    </Layouttemplate>
		    <GroupTemplate>
			    <tr id="Tr1" runat="server"><td id="itemPlaceholder" /></tr>
		    </GroupTemplate>
		    <ItemTemplate>
			    <td id="Td1" runat="server" style="vertical-align:top" width='<%# GetWidth()+"%" %>'>
				    <asp:LinkButton ID="lnkSelect" runat="server" CommandName="Select">
					    <asp:PlaceHolder ID="featurePlaceHolder" runat="server" />
				    </asp:LinkButton>
			    </td>
		    </ItemTemplate>
	    </asp:ListView>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlRotator" Visible="False">
	    <dnn:RadRotator ID="rotFeatures" runat="server" CssClass="bbstore-imagescroller"
		    ScrollDuration="500" 
		    FrameDuration="2000" 
		    RotatorType="AutomaticAdvance" 
		    OnItemDataBound="rotFeatures_DataBound"  
		    OnItemClick="rotFeatures_ItemClick" 
		    Width="100%" >
		    <ItemTemplate>
			    <div style="cursor: pointer">
				    <asp:PlaceHolder ID="featurePlaceHolder" runat="server" />
			    </div>
		    </ItemTemplate>
	    </dnn:RadRotator>
    </asp:Panel>
    <asp:Literal ID="ltrFoot" runat="server" Mode="PassThrough"/>
</div>