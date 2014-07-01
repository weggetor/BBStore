<%@ Control Language="C#" AutoEventWireup="false" Inherits="Bitboxx.DNNModules.BBStore.SettingsMiniCart" Codebehind="SettingsMiniCart.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="bb" TagName ="TemplateControl" Src="Controls/TemplateControl.ascx" %>
<div class="dnnForm bbstore-minicart-settings dnnClear">
    <bb:TemplateControl ID="tplTemplate" runat="server" Key="MiniCart" ViewMode="View" EditorControl="TextEditor"/>
</div>
