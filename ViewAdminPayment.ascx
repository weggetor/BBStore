<%@ Control Language="C#" Inherits="Bitboxx.DNNModules.BBStore.ViewAdminPayment" AutoEventWireup="true" CodeBehind="ViewAdminPayment.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="bb" TagName="TaxControl" Src="Controls/TaxControl.ascx" %>
<div class="bbstore-admin bbstore-admin-payment dnnClear">
    <asp:PlaceHolder runat="server" ID="phPayment" />
    <ul class="dnnActions dnnClear">
        <li><asp:LinkButton runat="server" ID="cmdUpdate" OnClick="cmdUpdate_Click" ResourceKey="cmdUpdate.Text" CssClass="dnnPrimaryAction"/></li>
        <li><asp:LinkButton runat="server" ID="cmdCancel" OnClick="cmdCancel_Click" ResourceKey="cmdCancel.Text" CssClass="dnnSecondaryAction"/></li>
    </ul>
</div>