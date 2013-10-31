<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditUnit.ascx.cs"  Inherits="Bitboxx.DNNModules.BBStore.EditUnit" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="bb" TagName="LanguageEditor" Src="Controls/LanguageEditorControl.ascx" %>
<div class="dnnForm bbstore-unit-edit dnnClear">
    <div class="dnnFormItem">
        <dnn:Label ID="lblDecimals" runat="server" controlname="txtDecimals" suffix=":" />
	    <asp:TextBox ID="txtDecimals" runat="server" Columns="2" MaxLength="1" />
    </div>
    <bb:LanguageEditor ID="lngUnits" runat="server" InternalType="Bitboxx.DNNModules.BBStore.UnitLangInfo" />
    <ul class="dnnActions dnnClear">
        <li><asp:LinkButton CssClass="dnnPrimaryAction" ID="cmdUpdate" runat="server" resourcekey="cmdUpdate" OnClick="cmdUpdate_Click" /></li>
        <li><asp:LinkButton CssClass="dnnSecondaryAction" ID="cmdCancel" runat="server" resourcekey="cmdCancel" OnClick="cmdCancel_Click" CausesValidation="false"/></li>
    </ul>
</div>