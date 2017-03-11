<%@ Control Language="C#" AutoEventWireup="false" Inherits="Bitboxx.DNNModules.BBStore.SettingsProduct" Codebehind="SettingsProduct.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="bb" TagName="TabSelectControl" Src="Controls/TabSelectControl.ascx" %> 
<%@ Register TagPrefix="bb" TagName="TemplateControl" Src="Controls/TemplateControl.ascx" %> 
<div id="bbstore-panels" class="dnnForm bbstore-product-settings dnnClear">
    <div class="dnnFormExpandContent"><a href=""><%=Localization.GetString("ExpandAll", Localization.SharedResourceFile)%></a></div>
    <h2 id="bbstore-product-head1" class="dnnFormSectionHead"><a href="#"><%=LocalizeString("hdrGlobal")%></a></h2>
    <fieldset class="dnnClear">
        <div class="dnnFormItem">
            <dnn:Label ID="lblShowNetPrice" runat="server" ControlName="rblShowNetPrice" Suffix=":"/>   
            <asp:RadioButtonList ID="rblShowNetPrice" runat="server" RepeatDirection="Horizontal" CssClass="dnnFormRadioButtons"/>
        </div>
        <div class="dnnFormItem">
		    <dnn:Label id="lblListModulePage" runat="server" controlname="urlListModulePage" suffix=":"/>
			<bb:TabSelectControl ID="urlListModulePage" runat="server" Width="200" />
		</div>    
		<div class="dnnFormItem">
		    <dnn:Label id="lblContactModulePage" runat="server" controlname="urlContactModulePage" suffix=":"/>
			<bb:TabSelectControl ID="urlContactModulePage" runat="server" Width="200" />
		</div>
		<div class="dnnFormItem">
		    <dnn:Label ID="lblOpenCartOnAdd" runat="server" ControlName="chkOpenCartOnAdd" Suffix=":"/>
            <asp:CheckBox runat="server" ID="chkOpenCartOnAdd" />
        </div>
        <div class="dnnFormItem">
		    <dnn:Label ID="lblSetModuleTitle" runat="server" ControlName="chkSetModuleTitle" Suffix=":"/>
            <asp:CheckBox runat="server" ID="chkSetModuleTitle" />
        </div>
    </fieldset>
    <h2 id="bbstore-product-head2" class="dnnFormSectionHead"><a href="#"><%=LocalizeString("hdrTemplates")%></a></h2>
    <fieldset class="dnnClear">
        <bb:TemplateControl ID ="tplTemplate" runat ="server" Key ="SimpleProduct" ViewMode="View" EditorControl="TextBox"/>
    </fieldset>
    <h2 id="bbstore-product-head3" class="dnnFormSectionHead"><a href="#"><%=LocalizeString("hdrProduct")%></a></h2>
    <fieldset>
        <div id="divMessage" runat="server">
            <asp:Label ID="lblSelectedCap" runat="server" ResourceKey="lblSelected.Text" /><br/>
            <span style="font-size:150%; font-weight: bold"><asp:Label ID="lblSelected" runat="server" /></span>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblSelectType" runat="server" ControlName="rblSelectType" />
            <asp:RadioButtonList runat="server" ID="rblSelectType" CssClass="dnnFormRadioButtons" 
                RepeatDirection="Horizontal" 
                AutoPostBack="True"
                OnSelectedIndexChanged="rblSelectType_SelectedIndexChanged">
                <asp:ListItem ResourceKey="rblSelectTypeDynamic" Value="0" />
                <asp:ListItem ResourceKey="rblSelectTypeStatic" Value="1" />
            </asp:RadioButtonList>
        </div>
        <asp:Panel runat="server" ID="pnlStatic" Visible="False" CssClass="dnnFormItem">
	        <dnn:Label ID="lblStatic" runat="server" />
            <div class="dnnLeft">
	            <table cellpadding="3" cellspacing="0" border="0">
		            <tr>
			            <td><dnn:label id="lblSearch" runat="server" controlname="txtSearch" /></td>
			            <td><asp:TextBox ID="txtSearch" runat="server" AutoPostBack="true" OnTextChanged="txtSearch_TextChanged"/></td>
		            </tr>
	            </table>
	            <asp:GridView ID="GridView1" runat="server" AllowSorting="True" DataKeyNames="SimpleProductId"
		            AutoGenerateColumns="False" CssClass="dnnGrid" 
		            onrowcommand="GridView1_RowCommand" AllowPaging="True" 
		            onpageindexchanging="GridView1_PageIndexChanging" 
		            onsorting="GridView1_Sorting" BorderWidth="0">
		            <RowStyle CssClass="dnnGridItem" BorderWidth="0" />
		            <Columns>
			            <asp:CommandField ShowSelectButton="True" />
			            <asp:BoundField DataField="SimpleProductId" HeaderText="SimpleProductId" SortExpression="SimpleProductId" >
			                <ItemStyle HorizontalAlign="Right" />
			            </asp:BoundField>
			            <asp:BoundField DataField="ItemNo" HeaderText="ItemNo" SortExpression="ItemNo" />
			            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
			            <asp:BoundField DataField="UnitCost" HeaderText="UnitCost"  SortExpression="UnitCost" DataFormatString="{0:n2}" >
			                <ItemStyle HorizontalAlign="Right" />
			            </asp:BoundField>
		            </Columns>
		            <HeaderStyle CssClass="dnnGridHeader" />
		            <AlternatingRowStyle CssClass="dnnGridAltItem" BorderWidth="0" />
	            </asp:GridView>
            </div>
        </asp:Panel>
    </fieldset>
</div>
<script type="text/javascript">
    jQuery(function ($) {
        var setupModule = function () {
            $('#bbstore-panels .dnnFormExpandContent a').dnnExpandAll({
                targetArea: '#bbstore-panels'
            });
        };
        setupModule();
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            // note that this will fire when _any_ UpdatePanel is triggered,
            // which may or may not cause an issue
            setupModule();
        });
    });
</script>      

