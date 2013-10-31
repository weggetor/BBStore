<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditFeatureListItem.ascx.cs" Inherits="Bitboxx.DNNModules.BBStore.EditFeatureListItem" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="UrlControl" Src="~/controls/UrlControl.ascx" %>
<%@ Register TagPrefix="bb" TagName="LanguageEditor" Src="Controls/LanguageEditorControl.ascx" %>  
<div class="dnnForm bbstore-featurelistitem-edit dnnClear">
	<asp:Panel ID="pnlFeatureListsItems" runat="server" CssClass="dnnFormItem">
		<dnn:Label ID="lblSelFLI" runat="server" ControlName="lstFeatureListItems" Suffix=":" />
	    <asp:ListBox ID="lstFeatureListItems" runat="server"  OnSelectedIndexChanged="lstFeatureListItems_SelectedIndexChanged" AutoPostBack="True" Rows="24" />
	</asp:Panel>
	<asp:Panel ID="pnlFeatureListItemDetails" runat="server" Visible="false">
		<h3><asp:Label runat="server" ID="lblFLIDetailsCaption" Visible="False"/> <asp:Label ID="lblFLIDetails" runat="server" Visible="false" /></h3>
        <bb:LanguageEditor ID="lngFeatureListItems" runat="server" InternalType="Bitboxx.DNNModules.BBStore.FeatureListItemLangInfo" />
        <div class="dnnFormItem">
            <dnn:label ID="lblViewOrder" runat="server" style="white-space:nowrap" controlname="txtViewOrder" suffix=":"/>
            <asp:TextBox ID="txtViewOrder" runat="server" Columns="3" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblImage" runat="server" ControlName="ImageSelector" Suffix=":"/>
            <div class="dnnLeft">
                <dnn:UrlControl ID="ImageSelector" runat="server" ShowFiles="true" ShowUrls="false" Required="false"
                                ShowTabs="false" ShowLog="false" ShowTrack="false" ShowUpLoad="true" ShowNewWindow="false" 
                                UrlType="F" />            
            </div>
            <div class="dnnLeft">
                <asp:ImageButton ID="imgRefreshImg" runat="server" ImageUrl="~/images/action_refresh.gif" OnClick="imgRefreshImg_Click"/>
		        <asp:LinkButton ID="cmdRefreshImg" runat="server" ResourceKey="cmdRefresh" OnClick="imgRefreshImg_Click"/><br/>
                <asp:Image ID="imgImage" runat="server" Width="100" />
            </div>
        </div>
        <ul class="dnnActions dnnClear">
		    <li><asp:LinkButton CssClass="dnnPrimaryAction" ID="cmdUpdate"	runat="server" resourcekey="cmdUpdate" OnClick="cmdUpdate_Click" visible="false"  /></li>
            <li><asp:LinkButton CssClass="dnnSecondaryAction" ID="cmdDelete" runat="server" resourcekey="cmdDelete" onclick="cmdDelete_Click" visible="false" /></li>
		    <li><asp:LinkButton CssClass="dnnSecondaryAction" ID="cmdCancel" runat="server" resourcekey="cmdCancel" OnClick="cmdCancel_Click" visible="false"  /></li>
        </ul>
	</asp:Panel>
    <asp:Panel ID="pnlBack" runat="server">
        <hr />
        <p>
            <asp:ImageButton ID="imgBack" runat="server" resourcekey="cmdBack" ImageUrl="~/images/lt.gif" OnClick="cmdBack_Click" />
            <asp:LinkButton CssClass="dnnSecondaryAction" ID="cmdBack" runat="server" resourcekey="cmdBack" OnClick="cmdBack_Click" />
        </p>
    </asp:Panel>
</div>