<%@ Control Language="C#" AutoEventWireup="false" Inherits="Bitboxx.DNNModules.BBStore.SettingsAdmin" Codebehind="SettingsAdmin.ascx.cs" %>
<%@ Import Namespace="DotNetNuke.Services.Localization" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="UrlControl" Src="~/controls/UrlControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx"%>
<%@ Register TagPrefix="bb" TagName="TabSelectControl" Src="Controls/TabSelectControl.ascx" %>
<%@ Register TagPrefix="bb" TagName="SelectFolderControl" Src="Controls/SelectFolderControl.ascx" %>   
<%@ Register TagPrefix="bb" TagName="ColorPicker" Src="Controls/ColorPickerControl.ascx" %>
<div id="bbstore-panels" class="dnnForm bbstore-admin-settings dnnClear">
    <div class="dnnFormExpandContent"><a href=""><%=Localization.GetString("ExpandAll", Localization.SharedResourceFile)%></a></div>

<!--    <h2 id="bbstore-admin-head0" class="dnnFormSectionHead"><a href="#"><%=LocalizeString("shLicense")%></a></h2>
    <fieldset class="dnnClear">
        <div class="dnnFormItem">
            <dnn:Label id="lblInitialKey" runat="server" style="white-space:nowrap" controlname="txtInitialKey" suffix=":"/>
            <asp:TextBox ID="txtInitialKey" runat="server"  OnTextChanged="txtInitialKey_OnTextChanged" AutoPostBack="True"/>&nbsp;
            <asp:LinkButton runat="server" ID="cmdRefreshLicense" CssClass="dnnSecondaryAction" ResourceKey="cmdRefreshLicense.Text" OnClick="cmdRefreshLicense_OnClick"></asp:LinkButton>
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblLicense" runat="server" style="white-space:nowrap" controlname="ltrLicense" suffix=":"/>
            <div class="">
                <asp:Literal runat="server" ID="ltrLicense" />
            </div>
        </div>
    </fieldset>-->
    
    <h2 id="bbstore-admin-head1" class="dnnFormSectionHead"><a href="#"><%=LocalizeString("shGlobal")%></a></h2>
    <fieldset class="dnnClear">
        <div class="dnnFormItem">
            <dnn:Label id="lblShowNetpriceInCart" runat="server" style="white-space:nowrap" controlname="opgShowNetpriceInCart" suffix=":"/>
            <asp:RadioButtonList ID="opgShowNetpriceInCart" runat="server" RepeatDirection="Horizontal" CssClass="dnnFormRadioButtons">
                    <asp:ListItem Value="0" ResourceKey="lblNet.Text"></asp:ListItem>
                    <asp:ListItem Value="1" ResourceKey="lblGross.Text"></asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblExtendedPrice" runat="server" controlname="chkExtendedPrice" suffix=":"/>
            <asp:CheckBox ID="chkExtendedPrice" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblOrderMask" runat="server" controlname="txtOrderMask" suffix=":"/>
            <asp:TextBox ID="txtOrderMask" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblStartPage" runat="server" controlname="urlSelectStartPage" suffix=":"/>
            <bb:TabSelectControl ID="urlSelectStartPage" runat="server" Width="200" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblTermsPage" runat="server" controlname="urlSelectTerms" suffix=":"/>
            <bb:TabSelectControl ID="urlSelectTerms" runat="server" Width="200" />
        </div>
	    <div class="dnnFormItem">
	        <dnn:Label id="lblTermsMandatory" runat="server" controlname="chkTermsMandatory" suffix=":"/>
            <asp:CheckBox runat="server" ID="chkTermsMandatory"/>
        </div>
        <div class="dnnFormItem">
	        <dnn:Label id="lblCancelTermsMandatory" runat="server" controlname="chkCancelTermsMandatory" suffix=":"/>
            <asp:CheckBox runat="server" ID="chkCancelTermsMandatory"/>
        </div>
        <div class="dnnFormItem">
	        <dnn:Label id="lblCancelTerms" runat="server" controlname="txtCancelTerms" suffix=":"/>
            <asp:TextBox runat="server" ID="txtCancelTerms" Rows="6" TextMode="MultiLine"/>
        </div>
        <div class="dnnFormItem">
	        <dnn:Label id="lblCouponsEnabled" runat="server" controlname="chkCouponsEnabled" suffix=":"/>
            <asp:CheckBox runat="server" ID="chkCouponsEnabled"/>
        </div>
	    <div class="dnnFormItem">
	        <dnn:Label id="lblSupplierRole" runat="server" controlname="cboSupplierRole" suffix=":"/>
            <asp:DropDownList Width="200"  runat="server" ID="cboSupplierRole"/>
        </div>
    </fieldset>

    <h2 id="bbstore-admin-head2" class="dnnFormSectionHead"><a href="#" class="dnnSectionExpanded"><%=LocalizeString("shFolder")%></a></h2>
    <fieldset class="dnnClear">
	    <div class="dnnFormItem">
	        <dnn:label id="lblProductImageDir" runat="server" controlname="cboProductImageDir" suffix=":"/>
            <bb:SelectFolderControl runat="server" ID="cboProductImageDir" Permission="admin"/>
        </div>
	    <div class="dnnFormItem">
	        <dnn:label id="lblProductGroupImageDir" runat="server" controlname="cboProductGroupImageDir" suffix=":"/>
		    <bb:SelectFolderControl runat="server" ID="cboProductGroupImageDir" Permission="admin"/>
        </div>
	    <div class="dnnFormItem">
	        <dnn:label id="lblProductGroupIconDir" runat="server" controlname="cboProductGroupIconDir" suffix=":"/>
		    <bb:SelectFolderControl runat="server" ID="cboProductGroupIconDir" Permission="admin"/>
        </div>
    </fieldset>

    <h2 id="bbstore-admin-head3" class="dnnFormSectionHead"><a href="#"><%=LocalizeString("shVendor")%></a></h2>
    <fieldset class="dnnClear">
        <div class="dnnFormItem">
            <dnn:Label id="lblVendorName" runat="server"  controlname="txtVendorName" suffix=":"/>
            <asp:TextBox ID="txtVendorName" runat="server" Columns="40" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblVendorStreet1" runat="server"  controlname="txtVendorStreet1" suffix=":"/>
            <asp:TextBox ID="txtVendorStreet1" runat="server" Columns="40" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblVendorStreet2" runat="server"  controlname="txtVendorStreet2" suffix=":"/>
            <asp:TextBox ID="txtVendorStreet2" runat="server" Columns="40" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblVendorZip" runat="server"  controlname="txtVendorZip" suffix=":"/>
            <asp:TextBox ID="txtVendorZip" runat="server" Columns="10" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblVendorCity" runat="server"  controlname="txtVendorCity" suffix=":"/>
            <asp:TextBox ID="txtVendorCity" runat="server" Columns="40" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblVendorCountry" runat="server" controlname="ddlVendorCountry" suffix=":"/>
            <asp:DropDownList id="ddlVendorCountry" runat="server" />
        </div>
    </fieldset>
    
    <h2 id="bbstore-admin-head4" class="dnnFormSectionHead"><a href="#"><%=LocalizeString("shMail")%></a></h2>
    <fieldset class="dnnClear">
        <div class="dnnFormItem">
            <dnn:Label id="lblStoreEmail" runat="server" controlname="txtStoreEmail" suffix=":"/>
            <asp:TextBox ID="txtStoreEmail" runat="server" Columns="40" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblStoreName" runat="server" controlname="txtStoreName" suffix=":"/>
            <asp:TextBox ID="txtStoreName" runat="server" Columns="40" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblStoreAdmin" runat="server" controlname="txtStoreCC" suffix=":"/>
            <asp:TextBox ID="txtStoreAdmin" runat="server" Columns="40" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblStoreReplyTo" runat="server" controlname="txtStoreReplyTo" suffix=":"/>
            <asp:TextBox ID="txtStoreReplyTo" runat="server" Columns="40" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblStoreSubject" runat="server" controlname="txtStoreSubject" suffix=":"/>
            <asp:TextBox ID="txtStoreSubject" runat="server" Columns="40" />
        </div>
        <div class="dnnFormItem" style="white-space: nowrap;">
            <dnn:Label id="lblSMTPSettings" runat="server" controlname="rblSMTPSettings" suffix=":"/>
            <asp:RadioButtonList ID="rblSMTPSettings" runat="server" AutoPostBack="True" RepeatDirection="Horizontal"
                OnSelectedIndexChanged="rblSMTPSettings_OnSelectedIndexChanged"/>
        </div>
        <asp:Panel runat="server" ID="pnlSMTP" Visible="False">
            <div class="dnnFormItem">
                <dnn:Label id="lblSMTPServer" runat="server" controlname="txtSMTPServer" suffix=":"/>
                <asp:TextBox ID="txtSMTPServer" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label id="lblSMTPUser" runat="server" controlname="txtSMTPUser" suffix=":"/>
                <asp:TextBox ID="txtSMTPUser" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label id="lblSMTPPassword" runat="server" controlname="txtSMTPPassword" suffix=":"/>
                <asp:TextBox ID="txtSMTPPassword" runat="server" />
            </div>
        </asp:Panel>
    </fieldset>

    <h2 id="bbstore-admin-head5" class="dnnFormSectionHead"><a href="#"><%=LocalizeString("shColors")%></a></h2>
    <fieldset class="dnnClear">
        <div class="dnnFormMessage dnnFormInfo">
            <asp:Label ID="lblColorInfo" runat="server" Resourcekey="lblColorInfo.Text"/>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblColorHead" runat="server" controlname="txtColorHead" suffix=":"/>
            <bb:ColorPicker runat="server" ID="txtColorHead"/>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblColorRow" runat="server" controlname="txtColorRow" suffix=":"/>
            <bb:ColorPicker runat="server" ID="txtColorRow"/>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblColorAlt" runat="server" controlname="txtColorAlt" suffix=":"/>
            <bb:ColorPicker runat="server" ID="txtColorAlt"/>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblColorSum" runat="server" controlname="txtColorSum" suffix=":"/>
            <bb:ColorPicker runat="server" ID="txtColorSum"/>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblColorBorder" runat="server" controlname="txtColorBorder" suffix=":"/>
            <bb:ColorPicker runat="server" ID="txtColorBorder"/>
        </div>
        <ul class="dnnActions dnnClear">
            <asp:LinkButton ID="cmdRestoreCss" runat="server" Resourcekey="cmdRestoreCss.Text" onclick="cmdRestoreCss_Click" CssClass="dnnSecondaryAction dnnRight"/>    
        </ul>
    </fieldset>

    <h2 id="bbstore-admin-head6" class="dnnFormSectionHead"><a href="#"><%=LocalizeString("shAdmin")%></a></h2>
    <fieldset class="dnnClear">
		<div class="dnnFormItem">
		    <dnn:Label id="lblItemsPerPage" runat="server" controlname="txtItemsPerPage" suffix=":"/>
		    <asp:TextBox ID="txtItemsPerPage" runat="server" Columns="20" />
		</div>
    </fieldset>
    
    <h2 id="bbstore-admin-head7" class="dnnFormSectionHead"><a href="#"><%=LocalizeString("shMiniCart")%></a></h2>
    <fieldset class="dnnClear">
		<div class="dnnFormItem">
		    <dnn:Label id="lblMiniCartTemplate" runat="server" controlname="txtMiniCartTemplate" suffix=":"/>
		    <asp:TextBox ID="txtMiniCartTemplate" runat="server" Columns="20" Rows="8"  TextMode="MultiLine"/>
		</div>
        <div class="dnnFormItem">
		    <dnn:Label id="lblHideMiniCartIfEmpty" runat="server" controlname="chkHideMiniCartIfEmpty" suffix=":"/>
		    <asp:CheckBox ID="chkHideMiniCartIfEmpty" runat="server" />
		</div>
    </fieldset>

</div>
<script type="text/javascript">
    jQuery(function ($) {
        var setupModule = function () {
            $('#bbstore-panels .dnnFormExpandContent a').dnnExpandAll({
                targetArea: '#bbstore-panels'
            });
        };
        setupModule();
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            // note that this will fire when _any_ UpdatePanel is triggered,
            // which may or may not cause an issue
            setupModule();
        });
    });
</script>       