<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TaxControl.ascx.cs" Inherits="Bitboxx.DNNModules.BBStore.TaxControl" %>
<div class="bb-taxcontrol" runat="server" id="divMain">
    <asp:TextBox ID="txtAmount" runat="server" Columns="6" CssClass="bb-taxcontrol-textbox" />
    <asp:Literal ID="ltrLineBreak" runat="server" Mode="PassThrough"/>
    <asp:RadioButton GroupName="Tax" id="rdbNetto" runat="server" CssClass="bb-taxcontrol-radiobutton"/>
    <asp:Literal ID="ltrLineBreak2" runat="server" Mode="PassThrough"/>
    <asp:RadioButton GroupName="Tax" id="rdbBrutto" runat="server" CssClass="bb-taxcontrol-radiobutton"/>
    <asp:HiddenField ID="hidNetto" runat="server" /><asp:HiddenField ID="hidBrutto" runat="server" />
</div>