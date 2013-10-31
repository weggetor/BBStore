<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewCartCustomer.ascx.cs" Inherits="Bitboxx.DNNModules.BBStore.ViewCartCustomer" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="bbstore-cart-customer dnnClear">
    <p>&nbsp;</p>
	<div class="dnnFormMessage dnnFormInfo">
        <asp:Label ID="lblIntro" runat="server" ResourceKey="Intro" />
    </div>
    <p>&nbsp;</p>
    <asp:Panel runat="server" ID="pnlSearch" Visible="true">
        <asp:DataGrid ID="grdCustomers" runat="server" AllowSorting="True" DataKeyNames="CustomerID"
            AutoGenerateColumns="False" 
            BorderStyle="None" 
            GridLines="None"
            Width="98%" 
			ShowHeader="False"
            CssClass="dnnGrid" onitemcommand="grdCustomers_ItemCommand" OnItemDataBound="grdCustomers_ItemDataBound">
            <ItemStyle CssClass="dnnGridItem" horizontalalign="Left" />
            <AlternatingItemStyle cssclass="dnnGridAltItem" />
            <Edititemstyle cssclass="dnnFormInput" />
            <%--<SelectedItemStyle cssclass="bbstore-grid-sumrow" />--%>
            <FooterStyle cssclass="dnnGridFooter" />
            <PagerStyle cssclass="dnnGridPager" />
            <HeaderStyle CssClass="dnnGridHeader" />
            <Columns>
	            <asp:templatecolumn>
                    <itemtemplate>
                        <asp:ImageButton CommandArgument='<%# DataBinder.Eval(Container.DataItem,"CustomerId") %>' CommandName="Select" ID="cmdSelect" runat="server" />
                    </itemtemplate>
                </asp:templatecolumn>
                <asp:templatecolumn>
                    <itemtemplate>
                        <asp:ImageButton CommandArgument='<%# DataBinder.Eval(Container.DataItem,"CustomerId") %>' CommandName="Edit" IconKey="Edit" ID="cmdEdit" ImageUrl="~\Images\Edit.gif" runat="server" />
                    </itemtemplate>
                </asp:templatecolumn>
                <asp:templatecolumn>
                    <itemtemplate>
                        <asp:ImageButton CommandArgument='<%# DataBinder.Eval(Container.DataItem,"CustomerId") %>' CommandName="Delete" IconKey="Delete" ImageUrl="~\Images\Delete.gif" runat="server" />
                    </itemtemplate>
                </asp:templatecolumn>
                <asp:BoundColumn  DataField="CustomerName" HeaderText="CustomerName" ItemStyle-Width="98%" />
            </Columns>
        </asp:DataGrid>

    </asp:Panel>
    <asp:Panel runat="server" ID="pnlEdit" Visible="False">
        <div class="dnnFormItem">
            <dnn:Label id="lblCustomerName" runat="server" controlname="txtCustomerName" suffix=":" />
            <asp:TextBox id="txtCustomerName" runat="server" Columns="30" CssClass="dnnFormInput" />
            <asp:HiddenField runat="server" ID="hidCustomerId"/>
        </div>

    </asp:Panel>
    <ul class="dnnActions dnnClear">
        <li><asp:LinkButton ID="cmdNew" runat="server" class="dnnPrimaryAction" ResourceKey="cmdNew" Visible="True" onclick="cmdNew_Click"/></li>
        <li><asp:LinkButton ID="cmdSave" runat="server" CssClass="dnnPrimaryAction" ResourceKey="cmdSave" Visible="False" onclick="cmdSave_Click" /></li>
        <li><asp:LinkButton ID="cmdCancel" runat="server" CssClass="dnnSecondaryAction" ResourceKey="cmdCancel" Visible="False" onclick="cmdCancel_Click"/></li>
    </ul>
</div>


