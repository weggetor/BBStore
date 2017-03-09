<%@ Control Language="C#" Inherits="Bitboxx.DNNModules.BBStore.ViewAdminShipping" AutoEventWireup="true" CodeBehind="ViewAdminShipping.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="bb" TagName="TaxControl" Src="Controls/TaxControl.ascx" %>
<div class="dnnForm bbstore-admin bbstore-admin-shipping dnnClear">
     <fieldset>
        <div class="dnnFormItem">
            <dnn:Label id="lblAddZeroShipping" runat="server" controlname="chkAddZeroShipping" suffix=":" />
            <asp:Checkbox ID="chkAddZeroShipping" runat="server" />
        </div>
    </fieldset>

    <ul class="dnnActions dnnClear">
        <li><asp:LinkButton runat="server" ID="cmdUpdate" OnClick="cmdUpdate_Click" ResourceKey="cmdUpdate.Text" CssClass="dnnPrimaryAction"/></li>
        <li><asp:LinkButton runat="server" ID="cmdCancel" OnClick="cmdCancel_Click" ResourceKey="cmdCancel.Text" CssClass="dnnSecondaryAction"/></li>
    </ul>
</div>