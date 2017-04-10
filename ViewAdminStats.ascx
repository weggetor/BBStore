<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewAdminStats.ascx.cs" Inherits="Bitboxx.DNNModules.BBStore.ViewAdminStats" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<div class="bbstore-admin bbstore-admin-stats dnnClear">
    <b><asp:Label runat="server" id="lblTimeFrame" Resourcekey="lblTimeframe.Text"/></b>
    <asp:DropDownList runat="server" id="ddlTimeframe" OnSelectedIndexChanged="ddlTimeframe_OnSelectedIndexChanged" AutoPostBack="True">
        <asp:ListItem Value="0" ResourceKey="ddlTimeframe_0.Text" Selected="True"></asp:ListItem>
        <asp:ListItem Value="1" ResourceKey="ddlTimeframe_1.Text"></asp:ListItem>
        <asp:ListItem Value="2" ResourceKey="ddlTimeframe_2.Text"></asp:ListItem>
        <asp:ListItem Value="3" ResourceKey="ddlTimeframe_3.Text"></asp:ListItem>
        <asp:ListItem Value="4" ResourceKey="ddlTimeframe_4.Text"></asp:ListItem>
        <asp:ListItem Value="5" ResourceKey="ddlTimeframe_5.Text"></asp:ListItem>
        <asp:ListItem Value="6" ResourceKey="ddlTimeframe_6.Text"></asp:ListItem>
        <asp:ListItem Value="7" ResourceKey="ddlTimeframe_7.Text"></asp:ListItem>
        <asp:ListItem Value="8" ResourceKey="ddlTimeframe_8.Text"></asp:ListItem>
        <asp:ListItem Value="9" ResourceKey="ddlTimeframe_9.Text"></asp:ListItem>
    </asp:DropDownList>
    <asp:Panel runat="server" ID="pnlSelectDates" Visible="False">
        <asp:Label runat="server" id="lblStartDate" Resourcekey="lblStartDate.Text"/>  <input type="date" runat="server" id="txtStartDate" class="dnnFormItem" style="line-height:inherit"/>
        <asp:Label runat="server" id="lblEndDate" Resourcekey="lblEndDate.Text"/> <input type="date" runat="server" id="txtEndDate" class="dnnFormItem" style="line-height:inherit"/>
        <asp:Button runat="server" id="cmdRefresh" Resourcekey="cmdRefresh.Text" OnClick="cmdRefresh_OnClick"/>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlShowDates" Visible="True">
        <b><asp:Label runat="server" id="lblStartDate2Caption" Resourcekey="lblStartDate.Text"/></b> <asp:Label runat="server" id="lblStartDate2Value"/>   
        <b><asp:Label runat="server" id="lblEndDate2Caption" Resourcekey="lblEndDate.Text"/></b>  <asp:Label runat="server" id="lblEndDate2Value"/>  
    </asp:Panel>
    <div style="margin-bottom: 10px; line-height: 0;"></div>
    <h4><asp:Label runat="server" id="lblOrders" Resourcekey="lblOrders.Text"/></h4>
    <asp:GridView ID="grdStats" runat="server" 
	    AutoGenerateColumns="False" 
	    cellpadding="10"
        cellspacing="5"
	    GridLines="None" 
	    Width="100%">
	    <Columns>
		    <asp:BoundField DataField="Amount" HeaderText="Amount" DataFormatString="{0:n}" ItemStyle-Width="100px">
		        <ItemStyle HorizontalAlign="Right" Width="100px" />
            </asp:BoundField>
		    <asp:BoundField DataField="Sum" HeaderText="Sum" DataFormatString="{0:n}">
                <ItemStyle HorizontalAlign="Right" Width="150px" />
            </asp:BoundField>
		    <asp:BoundField DataField="Product" HeaderText="Product"  />
	    </Columns>
	    <RowStyle CssClass="bbstore-admin-editgrid-row" />
	    <SelectedRowStyle CssClass="bbstore-admin-editgrid-selectedrow" />
	    <AlternatingRowStyle CssClass="bbstore-admin-editgrid-alternaterow" />
	    <FooterStyle CssClass="bbstore-admin-editgrid-footer" />
	    <HeaderStyle CssClass="bbstore-admin-editgrid-header" />
    </asp:GridView>
</div>