<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditFeatureList.ascx.cs"
    Inherits="Bitboxx.DNNModules.BBStore.EditFeatureList" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="bb" TagName="LanguageEditor" Src="Controls/LanguageEditorControl.ascx" %>
<div class="dnnForm bbstore-featurelist-edit dnnClear">
    <asp:Panel ID="pnlFeatureLists" runat="server" CssClass="dnnFormItem">
	    <dnn:Label ID="lblSelFL" runat="server" ControlName="lstFeatureList" Suffix=":" />
	    <asp:ListBox ID="lstFeatureList" runat="server"  OnSelectedIndexChanged="lstFeatureList_SelectedIndexChanged" AutoPostBack="True" Rows="24" />
    </asp:Panel>
	<asp:Panel ID="pnlFeatureListDetails" runat="server" Visible="false">
		<h3><asp:Label runat="server" ID="lblFLDetailsCaption" Visible="False"/> <asp:Label ID="lblFLDetails" runat="server" Visible="false" /></h3>
		<bb:LanguageEditor ID="lngFeatureLists" runat="server" InternalType="Bitboxx.DNNModules.BBStore.FeatureListLangInfo" />
        <h3><asp:Label runat="server" ID="lblFLIDetailsCaption" />&nbsp;
            <asp:ImageButton ID="imgEditItems" runat="server" resourcekey="cmdEditItems" ImageUrl="~/images/edit.gif"	OnClick="cmdEditItems_Click" Visible="False" /></h3>
        <asp:Panel runat="server" ID="pnlFeatureListItems" Visible="False">
			<asp:Literal runat="server" ID="ltrListItems"></asp:Literal>				
		</asp:Panel>
        <ul class="dnnActions dnnClear">
		    <li><asp:LinkButton CssClass="dnnPrimaryAction" ID="cmdUpdate"	runat="server" resourcekey="cmdUpdate" OnClick="cmdUpdate_Click" visible="false"  /></li>
            <li><asp:LinkButton CssClass="dnnSecondaryAction" ID="cmdDelete" runat="server" resourcekey="cmdDelete" onclick="cmdDelete_Click" visible="false" /></li>
		    <li><asp:LinkButton CssClass="dnnSecondaryAction" ID="cmdCancel" runat="server" resourcekey="cmdCancel" OnClick="cmdCancel_Click" visible="false"  /></li>
        </ul>            
	</asp:Panel>
    <div class="dnnClear"></div>
    <asp:Panel ID="pnlBack" runat="server">
        <hr />
        <p>
            <asp:ImageButton ID="imgBack" runat="server" resourcekey="cmdBack" ImageUrl="~/images/lt.gif" OnClick="cmdBack_Click" />
            <asp:LinkButton CssClass="CommandButton" ID="cmdBack" runat="server" resourcekey="cmdBack" OnClick="cmdBack_Click" />
        </p>
    </asp:Panel>
</div>