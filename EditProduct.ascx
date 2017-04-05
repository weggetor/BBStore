<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditProduct.ascx.cs"  Inherits="Bitboxx.DNNModules.BBStore.EditProduct" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="bb" TagName="TaxControl" Src="Controls/TaxControl.ascx" %>
<%@ Register TagPrefix="bb" TagName="FeatureGridControl" Src="Controls/FeatureGridControl.ascx" %>
<%@ Register TagPrefix="bb" TagName="LanguageEditor" Src="Controls/LanguageEditorControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web.Deprecated" %>
<%@ Register TagPrefix="dnn" TagName="UrlControl" Src="~/controls/urlcontrol.ascx" %>  
<div class="dnnForm bbstore-product-edit dnnClear" id="bbstore-editproduct">
    <ul class="dnnAdminTabNav">
        <li><a href="#pnlProduct"><%=LocalizeString("pnlProduct") %></a></li>
        <li><a href="#pnlLanguage"><%=LocalizeString("pnlLanguage") %></a></li>
        <li><a href="#pnlPrice"><%=LocalizeString("pnlPrice") %></a></li>
        <li><a href="#pnlProductGroup"><%=LocalizeString("pnlProductGroup") %></a></li>
        <li><a href="#pnlProductGroupFeature"><%=LocalizeString("pnlProductGroupFeatures") %></a></li>
    </ul>
    <div id="pnlProduct">
   		<div class="dnnFormItem">
			<dnn:Label ID="lblDisabled" runat="server" controlname="chkDisabled" suffix=":"></dnn:Label>
			<asp:CheckBox ID="chkDisabled" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblImage" runat="server" ControlName="ImageSelector" Suffix=":"></dnn:Label>
            <div class="dnnLeft">
                <dnn:UrlControl ID="ImageSelector" runat="server" ShowFiles="true" ShowUrls="false" Required="false"
                                        ShowTabs="false" ShowLog="false" ShowTrack="false" ShowUpLoad="true" ShowNewWindow="false"
                                        UrlType="F"/>
            </div>
            <div class="dnnLeft">
                <asp:ImageButton ID="imgRefreshImg" runat="server" ImageUrl="~/images/action_refresh.gif" OnClick="imgRefreshImg_Click"/>
		        <asp:LinkButton ID="cmdRefreshImg" runat="server" ResourceKey="cmdRefresh" OnClick="imgRefreshImg_Click"/><br/>
                <asp:Image ID="imgImage" runat="server" Width="100" />
            </div>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblItemNo" runat="server" ControlName="txtItemNo" Suffix=":"/>
            <asp:TextBox ID="txtItemNo" runat="server" Columns="20" MaxLength="20" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblUnit" runat="server" ControlName="ddlUnit" Suffix=":" />
			<asp:DropDownList ID="ddlUnit" runat="server" />
		</div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblWeight" runat="server" ControlName="txtWeight" Suffix=":"/>
            <asp:TextBox ID="txtWeight" runat="server" Columns="18" MaxLength="13" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblTaxPercent" runat="server" ControlName="txtTaxPercent" Suffix=":" />
            <asp:TextBox ID="txtTaxPercent" runat="server" Columns="5" />
            <asp:CompareValidator ID="ValidatorTaxPercent" runat="server" ControlToValidate="txtTaxPercent" Resourcekey="ValidatorTaxPercent.Error"
                                  Type="Double" Operator="DataTypeCheck" CultureInvariantValues="true" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblUnitCost" runat="server" ControlName="txtUnitCost" Suffix=":"/>
            <bb:TaxControl ID="taxUnitCost" runat="server" Orientation="Horizontal" ShortCaps="false" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblOriginalUnitCost" runat="server" ControlName="taxOriginalUnitCost" Suffix=":" />
            <bb:TaxControl ID="taxOriginalUnitCost" runat="server" Orientation="Horizontal" ShortCaps="false" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblHideCost" runat="server" controlname="chkHideCost" suffix=":"/>
			<asp:CheckBox ID="chkHideCost" runat="server" />
		</div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblNoCart" runat="server" controlname="chkNoCart" suffix=":"/>
			<asp:CheckBox ID="chkNoCart" runat="server" />
		</div>
        <div class="dnnFormItem" id="pnlSupplier" runat="server" Visible="False">
            <dnn:Label ID="lblSupplier" runat="server" ControlName="cboSupplier" Suffix=":" />
    		<asp:DropDownList ID="cboSupplier" runat="server" />
		</div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblShippingModel" runat="server" ControlName="cboShippingModel" Suffix=":" />
    		<asp:DropDownList ID="cboShippingModel" runat="server" />
		</div>
    </div>
    <div id="pnlLanguage">
        <bb:LanguageEditor ID="lngSimpleProducts" runat="server" InternalType="Bitboxx.DNNModules.BBStore.SimpleProductLangInfo" />
    </div>
    <div id="pnlPrice">
        <asp:Panel runat="server" ID="pnlPriceList">
            <asp:GridView ID="grdPriceList" runat="server" 
	            AutoGenerateColumns="False" 
	            DataKeyNames="ProductPriceId" 
	            AllowPaging="False" 
	            AllowSorting="False" 
	            CellPadding="2"
	            GridLines="None" 
	            Width="100%"
	            ShowFooter="True" 
	            EnableModelValidation="True" 
	            onrowcommand="grdPriceList_RowCommand" 
	            onrowdatabound="grdPriceList_RowDataBound" 
	            onrowcreated="grdPriceList_RowCreated" 
	            onrowdeleting="grdPriceList_RowDeleting"	>
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
		            <asp:BoundField DataField="ProductPriceId" HeaderText="ProductPriceId" SortExpression="ProductPriceId" />
		            <asp:BoundField DataField="UnitCost" HeaderText="UnitCost"  SortExpression="UnitCost" DataFormatString="{0:n2}" >
			            <ItemStyle HorizontalAlign="Right" />
		            </asp:BoundField>
		            <asp:BoundField DataField="OriginalUnitCost" HeaderText="OriginalUnitCost"  SortExpression="OriginalUnitCost" DataFormatString="{0:n2}" >
			            <ItemStyle HorizontalAlign="Right" />
		            </asp:BoundField>
                    <asp:BoundField DataField="TaxPercent" HeaderText="TaxPercent"  SortExpression="TaxPercent" DataFormatString="{0:n1}" >
			            <ItemStyle HorizontalAlign="Right" />
		            </asp:BoundField>
		            <asp:BoundField DataField="UserRole" HeaderText="UserRole" SortExpression="UserRole" />
	                <asp:BoundField DataField="Startdate" HeaderText="Startdate"  SortExpression="Startdate" />
                    <asp:BoundField DataField="EndDate" HeaderText="EndDate"  SortExpression="Enddate" />
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
            </asp:GridView>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlPriceEdit" Visible="False">
            <fieldset>
		        <asp:HiddenField ID="hidProductPriceId" runat="server" />
                <div class="dnnFormItem">
                    <dnn:Label ID="lblPriceTaxPercent" runat="server" ControlName="txtPriceTaxPercent" Suffix=":" />
                    <asp:TextBox ID="txtPriceTaxPercent" runat="server" Columns="5" />
                    <asp:CompareValidator ID="valComPriceTaxPercent" runat="server" ControlToValidate="txtPriceTaxPercent" Resourcekey="ValidatorTaxPercent.Error"
                                          Type="Double" Operator="DataTypeCheck" CultureInvariantValues="true" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblPriceUnitCost" runat="server" ControlName="taxPriceUnitCost" Suffix=":"/>
                    <bb:TaxControl ID="taxPriceUnitCost" runat="server" Orientation="Horizontal" ShortCaps="false" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblPriceOriginalUnitCost" runat="server" ControlName="taxPriceOriginalUnitCost" Suffix=":" />
                    <bb:TaxControl ID="taxPriceOriginalUnitCost" runat="server" Orientation="Horizontal" ShortCaps="false" />
                </div>
		        <div class="dnnFormItem">
			        <dnn:Label ID="lblPriceRoleId" runat="server" ControlName="ddlPriceRoleId" Suffix=":"/>
			        <asp:DropDownList ID="ddlPriceRoleId" runat="server" />
		        </div>
		        <div class="dnnFormItem">
			        <dnn:Label ID="lblPriceStartdate" runat="server" ControlName="dtpPriceStartdate" Suffix=":"/>
			        <dnn:DnnDatePicker ID="dtpPriceStartdate" runat="server" /> 
		        </div>
		        <div class="dnnFormItem">
			        <dnn:Label ID="lblPriceEndDate" runat="server" ControlName="dtpPriceEndDate" Suffix=":"/>
			        <dnn:DnnDatePicker ID="dtpPriceEndDate" runat="server" /> 
		        </div>
            </fieldset>
            <ul class="dnnActions dnnClear" style="margin-left:20%">
                <li><asp:LinkButton runat="server" ID="cmdSaveEditPrice" CssClass="dnnPrimaryAction" ResourceKey="cmdSaveEditPrice.Text" OnClick="cmdSaveEditPrice_OnClick"/></li>
                <li><asp:LinkButton runat="server" ID="cmdCancelEditPrice" CssClass="dnnSecondaryAction" ResourceKey="cmdCancelEditPrice.Text" OnClick="cmdCancelEditPrice_OnClick"/></li>
            </ul>
        </asp:Panel>
    </div>
    <div id="pnlProductGroup">
        <div class="dnnFormMessage dnnFormInfo">
            <%=LocalizeString("lblProductGroup") %>
        </div>
        <asp:TreeView ID="treeProductGroup" ShowCheckBoxes="All"  Width="250"
                      ExpandDepth="1" runat="server"  
                      ontreenodepopulate="treeProductGroup_TreeNodePopulate"/>
    </div>
    <div id="pnlProductGroupFeature">
        <div class="dnnFormMessage dnnFormInfo">
            <%=LocalizeString("lblFeatureGrid") %>
        </div>
        <bb:FeatureGridControl ID="FeatureGrid" runat="server" Mode="Edit" />
        <asp:LinkButton ID="cmdRefreshFeatures" runat="server" CssClass="dnnSecondaryAction" onclick="cmdRefreshFeatures_Click" CausesValidation="false" ResourceKey="cmdRefreshFeatures.Text" />
    </div>
</div>
<ul class="dnnActions dnnClear">
    <li><asp:LinkButton CssClass="dnnPrimaryAction" ID="cmdUpdate" runat="server" resourcekey="cmdUpdate" OnClick="cmdUpdate_Click" /></li>
    <li><asp:LinkButton CssClass="dnnSecondaryAction" ID="cmdCancel" runat="server" resourcekey="cmdCancel" OnClick="cmdCancel_Click" CausesValidation="false"/></li>
</ul>
<script type="text/javascript">
    jQuery(function ($) {
        $('#bbstore-editproduct').dnnTabs();
    });
</script>   