<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SelectFolderControl.ascx.cs" Inherits="Bitboxx.DNNModules.BBStore.Controls.SelectFolderControl" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<asp:ImageButton runat="server" ID="cmdAdd" ImageUrl="Images/SelectFolderControl/plus.png" ImageAlign="AbsMiddle" OnClick="cmdAdd_Click"/>
<asp:DropDownList runat="server" ID="ddlFolders"/>
<asp:Panel runat="server" ID="pnlAddFolder" Visible="False" CssClass="dnnFormItem">
    <dnn:Label runat="server" ID="lblNewFolder" ResourceKey="lblNewFolder" Suffix=":" />
    <asp:TextBox runat="server" ID="txtNewFolder" Width="300"/><span style="color:red"><asp:Label runat="server" ID="lblError" Visible="False"/></span>
    <asp:LinkButton runat="server" ID="cmdSave" OnClick="cmdSave_Click" CssClass="dnnPrimaryAction"/>&nbsp;<asp:LinkButton runat="server" ID="cmdCancel" OnClick="cmdCancel_Click" CssClass="dnnSecondaryAction"/>
</asp:Panel>