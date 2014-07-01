<%@ Control Language="C#" AutoEventWireup="false" Inherits="Bitboxx.DNNModules.BBStore.SettingsSearch" Codebehind="SettingsSearch.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="bb" TagName="TabSelectControl" Src="Controls/TabSelectControl.ascx" %>  
<div class="dnnForm bbstore-search-settings dnnClear">
    <fieldset>
        <div class="dnnFormItem">
		    <dnn:Label id="lblResetSearchEnabled" runat="server" controlname="chkResetSearchEnabled" suffix=":" />
            <asp:CheckBox ID="chkResetSearchEnabled" runat="server" />
        </div>
        <div class="dnnFormItem">
		    <dnn:Label id="lblResetSearchPGEnabled" runat="server" controlname="chkResetSearchPGEnabled" suffix=":" />
            <asp:CheckBox ID="chkResetSearchPGEnabled" runat="server" />
        </div>
        <div class="dnnFormItem">
		    <dnn:Label id="lblProductGroupSearchEnabled" runat="server" controlname="chkProductGroupSearchEnabled" suffix=":" />
            <asp:CheckBox ID="chkProductGroupSearchEnabled" runat="server" />
        </div>
        <div class="dnnFormItem">
		    <dnn:Label id="lblTextSearchEnabled" runat="server" controlname="chkTextSearchEnabled" suffix=":" />
            <asp:CheckBox ID="chkTextSearchEnabled" runat="server" />
        </div>
        <div class="dnnFormItem">
		    <dnn:Label id="lblStaticSearchEnabled" runat="server" controlname="chkStaticSearchEnabled" suffix=":" />
            <asp:CheckBox ID="chkStaticSearchEnabled" runat="server" />
        </div>
        <div class="dnnFormItem">
		    <dnn:Label id="lblPriceSearchEnabled" runat="server" controlname="chkPriceSearchEnabled" suffix=":" />
            <asp:CheckBox ID="chkPriceSearchEnabled" runat="server" />
        </div>
        <div class="dnnFormItem">
		    <dnn:Label id="lblFeatureSearchEnabled" runat="server" controlname="chkFeatureSearchEnabled" suffix=":" />
            <asp:CheckBox ID="chkFeatureSearchEnabled" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblDynamicPage" runat="server" controlname="urlSelectDynamicPage" suffix=":"/>
            <bb:TabSelectControl ID="urlSelectDynamicPage" runat="server" Width="200" />
        </div>
    </fieldset>
</div>
