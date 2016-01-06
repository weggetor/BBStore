<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditCoupon.ascx.cs"  Inherits="Bitboxx.DNNModules.BBStore.EditCoupon" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="bb" TagName="TaxControl" Src="Controls/TaxControl.ascx" %>
<div class="dnnForm bbstore-coupon-edit dnnClear">
    <div class="dnnFormItem">
        <dnn:Label ID="lblCaption" runat="server" controlname="txtCaption" suffix=":" />
	    <asp:TextBox ID="txtCaption" runat="server" MaxLength="100" style="width:350px;"/>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblCode" runat="server" controlname="txtCode" suffix=":" />
	    <asp:TextBox ID="txtCode" runat="server" MaxLength="30" style="width:250px;"/>
        <asp:LinkButton CssClass="dnnSecondaryAction" ID="cmdGenerateCode" runat="server" resourcekey="cmdGenerateCode" OnClick="cmdGenerateCode_Click" CausesValidation="false">Click</asp:LinkButton>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblValidUntil" runat="server" controlname="txtValidUntil" suffix=":" />
	    <asp:TextBox ID="txtValidUntil" runat="server" TextMode="Date" style="width:120px;"/>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblTaxPercent" runat="server" ControlName="txtTaxPercent" Suffix=":" />
        <asp:TextBox ID="txtTaxPercent" runat="server" Columns="5" />
        <asp:CompareValidator ID="ValidatorTaxPercent" runat="server" ControlToValidate="txtTaxPercent" Resourcekey="ValidatorTaxPercent.Error"
                                Type="Double" Operator="DataTypeCheck" CultureInvariantValues="true" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblDiscountPercent" runat="server" controlname="txtDiscountPercent" suffix=":" />
	    <asp:TextBox ID="txtDiscountPercent" runat="server" style="width:70px;" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblDiscountValue" runat="server" controlname="txtDiscountValue" suffix=":" />
	    <bb:TaxControl ID="taxDiscountValue" runat="server" Orientation="Horizontal" ShortCaps="false" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblMaxUsages" runat="server" controlname="txtMaxUsages" suffix=":" />
	    <asp:TextBox ID="txtMaxUsages" runat="server" type="Number" step="1" style="width:50px;" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblUsagesLeft" runat="server" controlname="txtUsagesLeft" suffix=":" />
	    <asp:TextBox ID="txtUsagesLeft" runat="server" type="Number" step="1" style="width:50px;"/>
    </div>
    <ul class="dnnActions dnnClear">
        <li><asp:LinkButton CssClass="dnnPrimaryAction" ID="cmdUpdate" runat="server" resourcekey="cmdUpdate" OnClick="cmdUpdate_Click" /></li>
        <li><asp:LinkButton CssClass="dnnSecondaryAction" ID="cmdCancel" runat="server" resourcekey="cmdCancel" OnClick="cmdCancel_Click" CausesValidation="false"/></li>
    </ul>
</div>
