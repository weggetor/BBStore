<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TemplateControl.ascx.cs" Inherits="Bitboxx.DNNModules.BBStore.TemplateControl" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx" %>
<asp:Panel ID="pnlHelp" runat="server" CssClass="dnnFormMessage dnnFormInfo dnnClear" Visible="False">
	<asp:Label runat="server" ID="ltrHelp" />
</asp:Panel>
<div class="dnnFormItem dnnClear">
    <dnn:Label id="lblCaption" runat="server" ControlName="ddlTemplate" suffix=":"/>
    <asp:Panel runat="server" ID="pnlView" CssClass="dnnLeft">
        <div><asp:Label runat="server" ID="lblTemplateName" /></div>
        <div><asp:DropDownList runat="server" ID="ddlTemplate" onselectedindexchanged="ddlTemplate_SelectedIndexChanged"  AutoPostBack="True" /></div>
        <div><asp:Image ID="imgThumb" runat="server" width="400" /></div>
        <div>
            <asp:LinkButton ID="cmdNew" runat="server" resourcekey="cmdNew" OnClick="cmdNew_Click" CssClass="dnnSecondaryAction"/>
            <asp:LinkButton ID="cmdEdit" runat="server" resourcekey="cmdEdit" OnClick="cmdEdit_Click" CssClass="dnnSecondaryAction"/>
        </div>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlEdit" Visible="False" CssClass="dnnLeft">
	    <asp:Panel runat="server" ID="pnlNewTemplate">
		    <dnn:Label runat="server" id="lblName" controlname="txtName" suffix=":" />
		    <asp:TextBox ID="txtName" runat="server" />
		    <asp:RequiredFieldValidator ID="valNameRequired" ControlToValidate="txtName" runat="server" Resourcekey="valNameRequired.Error"/>
	    </asp:Panel>
	    <asp:Panel runat="server" ID="pnlEditTemplate">
		    <div class="dnnClear">
			    <dnn:Label runat="server" id="lblMode" controlname="rblMode" suffix=":" />
                <asp:RadioButtonList ID="rblMode" runat="server"  RepeatDirection="Horizontal" 
				    AutoPostBack="True" OnSelectedIndexChanged="rblMode_SelectedIndexChanged">
				    <asp:ListItem resourcekey="rblMode0" Value="0" />
				    <asp:ListItem resourcekey="rblMode1" Value="1" />
			    </asp:RadioButtonList>
		    </div>
		    <div class="dnnClear">
			    <dnn:Label runat="server" id="lblLanguage" controlname="ddlLanguage" suffix=":" />
			    <asp:DropDownList runat="server" ID="ddlLanguage" AutoPostBack="True" OnSelectedIndexChanged="ddlLanguage_SelectedIndexChanged"/>
		    </div>
		    <div class="dnnClear">
			    <dnn:Label runat="server" id="lblFileCap" controlname="lblFile" suffix=":" />
			    <asp:Label runat="server" ID="lblFile" />
		    </div>
	    </asp:Panel>
	    <div class="dnnClear">
		    <asp:Textbox id="txtTemplate" CssClass="dnnFormInput bbstore-template" textmode="MultiLine" rows="10" runat="server" />
	        <dnn:TextEditor ID="edtTemplate" runat="server"  TextRenderMode="Raw" HtmlEncode="False" defaultmode="Rich"  choosemode="False" visible="false" />
	    </div>
	    <ul class="dnnActions dnnClear">
		    <li><asp:LinkButton ID="cmdSave" runat="server" resourcekey="cmdSave" OnClick="cmdSave_Click" CausesValidation="True"/></li>
		    <li><asp:LinkButton ID="cmdCancelEdit" runat="server" resourcekey="cmdCancelEdit" OnClick="cmdCancelEdit_Click" CausesValidation="False"/></li>
	    </ul>
    </asp:Panel>
</div>