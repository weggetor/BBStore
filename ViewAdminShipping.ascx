<%@ Control Language="C#" Inherits="Bitboxx.DNNModules.BBStore.ViewAdminShipping" AutoEventWireup="true" CodeBehind="ViewAdminShipping.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="bb" TagName="TaxControl" Src="Controls/TaxControl.ascx" %>
<div class="dnnForm bbstore-admin-shipping dnnClear">
     <fieldset>
        <div class="dnnFormItem">
            <dnn:Label id="lblShippingTax" runat="server" controlname="txtShippingTax" suffix=":" />
            <asp:TextBox ID="txtShippingTax"  Columns="5" runat="server" />
        </div>
         <p>&nbsp;</p>
        <div class="dnnFormItem">
            <dnn:Label id="lblShippingCost" runat="server" controlname="txtShippingCost" suffix=":" />
            <bb:TaxControl ID="txtShippingCost" runat="server" Orientation="Horizontal" ShortCaps="false" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblShippingType" runat="server" controlname="txtShippingType" suffix=":" />
            <asp:TextBox ID="txtShippingType" Columns="30" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblShippingFree" runat="server" controlname="txtShippingFree" suffix=":" />
            <bb:TaxControl ID="txtShippingFree" runat="server" Orientation="Horizontal" ShortCaps="false" />
        </div>
        <p>&nbsp;</p>
        <div class="dnnFormItem">
            <dnn:Label id="lblShippingCostInt" runat="server" controlname="txtShippingCostInt" suffix=":" />
            <bb:TaxControl ID="txtShippingCostInt" runat="server" Orientation="Horizontal" ShortCaps="false" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblShippingTypeInt" runat="server" controlname="txtShippingTypeInt" suffix=":" />
     	    <asp:TextBox ID="txtShippingTypeInt" Columns="30" runat="server"/>
        </div>
        <div class="dnnFormItem">        	
            <dnn:Label id="lblShippingFreeInt" runat="server" controlname="txtShippingFreeInt" suffix=":" />
            <bb:TaxControl ID="txtShippingFreeInt" runat="server" Orientation="Horizontal" ShortCaps="false" />
        </div>
    </fieldset>

    <ul class="dnnActions dnnClear">
        <li><asp:LinkButton runat="server" ID="cmdUpdate" OnClick="cmdUpdate_Click" ResourceKey="cmdUpdate.Text" CssClass="dnnPrimaryAction"/></li>
        <li><asp:LinkButton runat="server" ID="cmdCancel" OnClick="cmdCancel_Click" ResourceKey="cmdCancel.Text" CssClass="dnnSecondaryAction"/></li>
    </ul>
</div>