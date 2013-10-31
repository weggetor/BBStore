<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditProduct.ascx.cs"  Inherits="Bitboxx.DNNModules.BBStore.EditProduct" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="bb" TagName="TaxControl" Src="Controls/TaxControl.ascx" %>
<%@ Register TagPrefix="bb" TagName="FeatureGridControl" Src="Controls/FeatureGridControl.ascx" %>
<%@ Register TagPrefix="bb" TagName="LanguageEditor" Src="Controls/LanguageEditorControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="UrlControl" Src="~/controls/urlcontrol.ascx" %>  
<div class="dnnForm bbstore-product-edit dnnClear" id="bbstore-editproduct">
    <ul class="dnnAdminTabNav">
        <li><a href="#pnlProduct"><%=LocalizeString("pnlProduct") %></a></li>
        <li><a href="#pnlLanguage"><%=LocalizeString("pnlLanguage") %></a></li>
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
            <dnn:Label ID="lblItemNo" runat="server" ControlName="txtItemNo" Suffix=":"></dnn:Label>
            <asp:TextBox ID="txtItemNo" runat="server" Columns="20" MaxLength="20" />
        </div>
	    <div class="dnnFormItem" id="pnlSupplier" runat="server" Visible="False">
            <dnn:Label ID="lblSupplier" runat="server" ControlName="cboSupplier" Suffix=":" />
    		<asp:DropDownList ID="cboSupplier" runat="server" />
		</div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblUnit" runat="server" ControlName="ddlUnit" Suffix=":" />
			<asp:DropDownList ID="ddlUnit" runat="server" />
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
        
    </div>
    <div id="pnlLanguage">
        <bb:LanguageEditor ID="lngSimpleProducts" runat="server" InternalType="Bitboxx.DNNModules.BBStore.SimpleProductLangInfo" />
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