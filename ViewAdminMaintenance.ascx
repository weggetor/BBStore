<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewAdminMaintenance.ascx.cs" Inherits="Bitboxx.DNNModules.BBStore.ViewAdminMaintenance" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<div class="dnnForm bbstore-admin-maintenance dnnClear">
    <div class="bbSection">
		<h3><asp:Label id="lblCheckCap" ResourceKey="lblCheck.Text" runat="server"/></h3>
        <asp:Label runat="server" ID="lblCheck" ResourceKey="lblCheck.Help" CssClass="dnnFormMessage dnnFormInfo"/>
		<asp:LinkButton runat="server" ID="cmdCheck" OnClick="cmdCheck_Click" ResourceKey="cmdCheck.Text" CssClass="dnnPrimaryAction bbAction"/>
        <asp:CheckBox runat="server" ID="chkCheckOnly" ResourceKey="chkCheck.Text" Checked="True"/>
	</div>
    <div class="bbSection">
		<h3><asp:Label id="lblResetCap" ResourceKey="lblReset.Text" runat="server"/></h3>
        <asp:Label runat="server" ID="lblReset" ResourceKey="lblReset.Help" CssClass="dnnFormMessage dnnFormInfo"/>
		<asp:LinkButton runat="server" ID="cmdReset" OnClick="cmdReset_Click" ResourceKey="cmdReset.Text" CssClass="dnnPrimaryAction bbAction"/>
        <asp:DropDownList runat="server" ID="ddlSelectReset" DataTextField="Text" DataValueField="Value" />
	</div>
    <div class="bbSection">
        <h3><asp:Label id="lblExportCap" ResourceKey="lblExport.Text" runat="server"/></h3>
        <asp:Label runat="server" ID="lblExport" ResourceKey="lblExport.Help" CssClass="dnnFormMessage dnnFormInfo"/>
        <asp:LinkButton runat="server" ID="cmdExport" OnClick="cmdExport_Click" ResourceKey="cmdExport.Text" CssClass="dnnPrimaryAction bbAction"/>
        <asp:DropDownList runat="server" ID="ddlSelectExport" DataTextField="Text" DataValueField="Value" />
    </div>
    <div class="bbSection">
        <h3><asp:Label id="lblImportCap" ResourceKey="lblImport.Text" runat="server"/></h3>
        <asp:Label runat="server" ID="lblImport" ResourceKey="lblImport.Help" CssClass="dnnFormMessage dnnFormInfo"/>
        <p><asp:FileUpload runat="server" ID="fulImport" />
        <p><asp:LinkButton runat="server" ID="cmdImport" OnClick="cmdImport_Click" ResourceKey="cmdImport.Text" CssClass="dnnPrimaryAction bbAction"/></p>
    </div>
</div>