<%@ Control Language="C#" AutoEventWireup="false" Inherits="Bitboxx.DNNModules.BBStore.SettingsList" Codebehind="SettingsList.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="bb" TagName="TabSelectControl" Src="Controls/TabSelectControl.ascx" %>
<%@ Register TagPrefix="bb" TagName="LanguageEditor" Src="Controls/LanguageEditorControl.ascx" %> 
<%@ Register TagPrefix="bb" TagName="TemplateControl" Src="Controls/TemplateControl.ascx" %>  

<div id="bbstore-panels" class="dnnForm bbstore-productlist-settings dnnClear">
    <div class="dnnFormExpandContent"><a href=""><%=Localization.GetString("ExpandAll", Localization.SharedResourceFile)%></a></div>
    <h2 id="bbstore-productlist-head1" class="dnnFormSectionHead"><a href="#"><%=LocalizeString("hdrGlobal")%></a></h2>
    <fieldset class="dnnClear">
        <div class="dnnFormItem">
            <dnn:Label id="lblProductsInRow" runat="server" controlname="txtProductsInRow" suffix=":"/>
            <asp:TextBox ID="txtProductsInRow" runat="server" Columns="3" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblProductsPerPage" runat="server" controlname="txtProductsPerPage" suffix=":"/>
            <asp:TextBox ID="txtProductsPerPage" runat="server" Columns="20" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblTopN" runat="server" controlname="txtTopN" suffix=":"/>
            <asp:TextBox ID="txtTopN" runat="server" Columns="3" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblSetTitle" runat="server" controlname="chkSetTitle" suffix=":"/>
            <asp:CheckBox  ID="chkSetTitle" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblTitleBreadcrumb" runat="server" controlname="chkTitleBreadcrumb" suffix=":"/>
            <asp:CheckBox  ID="chkTitleBreadcrumb" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblShowListHead" runat="server" controlname="chkShowListHead" suffix=":"/>
            <asp:CheckBox  ID="chkShowListHead" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblShowPaging" runat="server" controlname="chkShowPaging" suffix=":"/>
            <asp:CheckBox  ID="chkShowPaging" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblRandomSort" runat="server" controlname="chkRandomSort" suffix=":"/>
            <asp:CheckBox  ID="chkRandomSort" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblProductModulePage" runat="server" controlname="urlProductModulePage" suffix=":"/>
            <bb:TabSelectControl ID="urlProductModulePage" runat="server" Width="200" />
        </div>
        <hr/>
        <div class="dnnFormItem">
            <dnn:Label id="lblShowAllLink" runat="server" controlname="chkShowAllLink" suffix=":"/>
            <asp:DropDownList ID="cboShowAllLink" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblProductListModulePage" runat="server" controlname="urlProductListModulePage" suffix=":"/>
            <bb:TabSelectControl ID="urlProductListModulePage" runat="server" Width="200" />
        </div>
    </fieldset>
    <h2 id="bbstore-productlist-head2" class="dnnFormSectionHead"><a href="#"><%=LocalizeString("hdrLanguage")%></a></h2>
    <fieldset class="dnnClear">
        <bb:LanguageEditor ID="lngHeaderText" runat="server" InternalType="Bitboxx.DNNModules.BBStore.LocalResourceLangInfo" FixedDisplay="Control=Texteditor,Height=300px,Width=500px,Label=HeaderText" />
        <bb:LanguageEditor ID="lngEmptyList" runat="server" InternalType="Bitboxx.DNNModules.BBStore.LocalResourceLangInfo" FixedDisplay="Control=Texteditor,Height=300px,Width=500px,Label=EmptyList" />
        <bb:LanguageEditor ID="lngFooterText" runat="server" InternalType="Bitboxx.DNNModules.BBStore.LocalResourceLangInfo" FixedDisplay="Control=Texteditor,Height=300px,Width=500px,Label=FooterText" />
    </fieldset>
    <h2 id="bbstore-productlist-head3" class="dnnFormSectionHead"><a href="#"><%=LocalizeString("hdrTemplates")%></a></h2>
    <fieldset class="dnnClear">
        <bb:TemplateControl ID="tplTemplate" runat="server" Key="ProductList" ViewMode="View" EditorControl="TextEditor"/>
    </fieldset>
    <h2 id="bbstore-productlist-head4" class="dnnFormSectionHead"><a href="#"><%=LocalizeString("hdrData")%></a></h2>
    <fieldset class="dnnClear">
        <table cellspacing="0" cellpadding="2" border="0">
	        <tr>
                <td class="SubHead" width="300"><dnn:Label id="lblSelection" runat="server" style="white-space:nowrap" controlname="rblSelection" suffix=":"></dnn:Label></td>
                <td>
			        <asp:RadioButtonList ID="rblSelection" runat="server"  
				        RepeatDirection="Horizontal" AutoPostBack="True" 
				        onselectedindexchanged="rblSelection_SelectedIndexChanged" />
                </td>
            </tr>
	        <tr id="trStaticSelection" runat="server">
                <td class="SubHead" width="300" valign="top">
                    <dnn:Label ID="lblStaticFilterSelection" runat="server" ControlName="cboStaticFilter" Suffix=":"></dnn:Label>
                </td>
                <td>
                    <asp:DropDownList ID="cboStaticFilter" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cboStaticFilter_SelectedIndexChanged"></asp:DropDownList>
                    <asp:ImageButton ID="cmdDeleteStaticFilter" runat="server" resourcekey="cmdDelete"
                        ImageUrl="~/images/delete.gif" OnClick="cmdDeleteStaticFilter_Click" />
                    <table>
                        <tr>
                            <td>
                                <dnn:Label ID="lblStaticFilterToken" runat="server" ControlName="txtStaticFilterToken"
                                    Suffix=":"></dnn:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtStaticFilterToken" runat="server" />
                                <asp:ImageButton ID="cmdSaveStaticFilter" runat="server" resourcekey="cmdUpdate"
                                    ImageUrl="~/images/save.gif" OnClick="cmdSaveStaticFilter_Click" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
	        <tr id="trStaticText" runat="server">
		        <td class="SubHead" width="300"><dnn:Label id="lblStaticFilter" runat="server" style="white-space:nowrap; vertical-align:top" controlname="txtStaticFilter" suffix=":"></dnn:Label></td>
                <td>
			        <asp:TextBox ID="txtStaticFilter" runat="server" Columns="80" Rows="7" TextMode="MultiLine" />
                </td>	
	        </tr>
        </table>
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