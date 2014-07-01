<%@ Control Language="C#" AutoEventWireup="false" Inherits="Bitboxx.DNNModules.BBStore.SettingsFeatureList" Codebehind="SettingsFeatureList.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="bb" TagName="TabSelectControl" Src="Controls/TabSelectControl.ascx" %>  
<%@ Register TagPrefix="bb" TagName ="TemplateControl" Src="Controls/TemplateControl.ascx" %>
<div id="bbstore-panels" class="dnnForm bbstore-featurelist-settings dnnClear">
    <div class="dnnFormExpandContent"><a href=""><%=Localization.GetString("ExpandAll", Localization.SharedResourceFile)%></a></div>
    <h2 id="bbstore-featurelist-head1" class="dnnFormSectionHead"><a href="#"><%=LocalizeString("hdrGlobal")%></a></h2>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:Label id="lblFeatureList" runat="server" controlname="cboFeatureList" suffix=":"/>
            <asp:DropDownList ID="cboFeatureList" runat="server" /> 
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblProductListModulePage" runat="server" controlname="urlProductListModulePage" suffix=":"/>
            <bb:TabSelectControl ID="urlProductListModulePage" runat="server" />
        </div>
	    <div class="dnnFormItem"r>
		    <dnn:Label id="lblOnlyWithImage" runat="server" controlname="chkOnlyWithImage" suffix=":"/>
            <asp:CheckBox runat="server" ID="chkOnlyWithImage"/>
        </div>
	    <div class="dnnFormItem">
	        <dnn:Label id="lblViewMode" runat="server" controlname="rblViewMode" suffix=":"/>
            <asp:RadioButtonList runat="server" ID="rblViewMode" AutoPostBack="True" OnSelectedIndexChanged="rblViewMode_SelectedIndexChanged" RepeatDirection="Horizontal">
                <asp:ListItem ResourceKey="rblViewModeTable.Text" Value="0"/>
			    <asp:ListItem ResourceKey="rblViewModeScroller.Text" Value="1"/>
            </asp:RadioButtonList>
        </div>
	    <asp:Panel runat="server" ID="pnlTable" CssClass="dnnFormItem">
	        <dnn:Label id="lblFeaturesInRow" runat="server" controlname="txtFeaturesInRow" suffix=":"/>
	        <asp:TextBox ID="txtFeaturesInRow" runat="server" Columns="3" />
	    </asp:Panel>
	    <asp:Panel runat="server" ID="pnlRotator" CssClass="dnnFormItem">
	        <dnn:Label id="lblRotatorHeight" runat="server" controlname="txtRotatorHeight" suffix=":"/>
	        <asp:TextBox ID="txtRotatorHeight" runat="server" Columns="3" />
	    </asp:Panel>
    </fieldset>
    <h2 id="bbstore-productlist-head2" class="dnnFormSectionHead"><a href="#"><%=LocalizeString("hdrLanguage")%></a></h2>
    <fieldset>
	    <div class="dnnFormItem">
	        <dnn:Label id="lblHeaderText" runat="server" controlname="txtHeaderText" suffix=":"/>
            <asp:TextBox ID="txtHeaderText" runat="server" Columns="80" Rows="7" TextMode="MultiLine" />
	    </div>
	    <div class="dnnFormItem">
	        <dnn:Label id="lblFooterText" runat="server" controlname="txtFooterText" suffix=":"/>
            <asp:TextBox ID="txtFooterText" runat="server" Columns="80" Rows="7" TextMode="MultiLine" />
	    </div>
    </fieldset>
    <h2 id="bbstore-productlist-head3" class="dnnFormSectionHead"><a href="#"><%=LocalizeString("hdrTemplates")%></a></h2>
    <fieldset>
        <bb:TemplateControl ID="tplTemplate" runat="server" Key="FeatureList" ViewMode="View" EditorControl="TextBox"/>
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