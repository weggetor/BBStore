<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LanguageEditorControl.ascx.cs" Inherits="Bitboxx.DNNModules.BBStore.LanguageEditorControl" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm bbLanguageForm dnnClear">
    <asp:FormView ID="formViewLang" runat="server" AllowPaging="false" DefaultMode="Edit" OnItemCreated="formViewLang_ItemCreated" RenderOuterTable="False">
	    <HeaderTemplate>
	        <div class="dnnFormItem">
	            <dnn:Label ID="lblLanguage" runat="server" Suffix=":" />
			    <div>
                    <div style="display:table-cell;border-bottom: solid; border-width: 1px; vertical-align: bottom; min-width:50px;"></div>
				    <asp:PlaceHolder ID="phLanguage" runat="server"></asp:PlaceHolder>
				    <div style="display:table-cell;border-bottom: solid; border-width: 1px; text-align: left; vertical-align: bottom; width:99%">&nbsp;</div>
			    </div>
            </div>
	    </HeaderTemplate>
    </asp:FormView>
</div>