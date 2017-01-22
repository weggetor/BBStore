<%@ Control Language="C#" AutoEventWireup="false" Inherits="Bitboxx.DNNModules.BBStore.SettingsProductGroup" Codebehind="SettingsProductGroup.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="UrlControl" Src="~/controls/urlcontrol.ascx" %>
<%@ Register TagPrefix="bb" TagName="TabSelectControl" Src="Controls/TabSelectControl.ascx" %>
<% @ Register TagPrefix="bb" TagName ="TemplateControl" Src ="Controls/TemplateControl.ascx" %>  
<div class="dnnForm bbstore-productgroup-settings dnnClear">
    <fieldset>
        <div class="dnnFormItem">
            <dnn:Label id="lblViewMode" runat="server" controlname="rblViewMode" suffix=":"/>
            <asp:RadioButtonList ID="rblViewMode" runat="server" AutoPostBack="True" CssClass="dnnFormRadioButtons"
                    onselectedindexchanged="rblViewMode_SelectedIndexChanged">
                    <asp:ListItem Value="0">Template</asp:ListItem>
                    <asp:ListItem Value="1">Treeview</asp:ListItem>
                    <asp:ListItem Value="2">DropDown</asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblDynamicPage" runat="server" controlname="urlSelectDynamicPage" suffix=":"/>
            <bb:TabSelectControl ID="urlSelectDynamicPage" runat="server" Width="200" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblIncludeChilds" runat="server" controlname="chkIncludeChilds" suffix=":"/>
            <asp:CheckBox ID="chkIncludeChilds" runat="server" />
        </div>
   	    <div class="dnnFormItem">
   	        <dnn:Label id="lblShowProductCount" runat="server" controlname="chkShowProductCount" suffix=":"/>
            <asp:CheckBox ID="chkShowProductCount" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblRootLevel" runat="server" controlname="cboRootLevel" suffix=":" />
		    <asp:DropDownList ID="cboRootLevel" runat="server" />
	    </div>
	    <div class="dnnFormItem">
	        <dnn:Label id="lblRootLevelFixed" runat="server" controlname="chkRootLevelFixed" suffix=":"/>
            <asp:CheckBox ID="chkRootLevelFixed" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblDefaultProductGroup" runat="server" controlname="cboDefaultProductGroup" suffix=":" />
		    <asp:DropDownList ID="cboDefaultProductGroup" runat="server" />
	    </div>
	    <asp:Panel ID="pnlShowLevels" runat="server" CssClass="dnnFormItem">
	        <dnn:Label id="lblShowLevels" runat="server" controlname="txtShowLevels" suffix=":"/>
	        <asp:TextBox ID="txtShowLevels" runat="server" Columns="2"/>
	    </asp:Panel>
       <div class="dnnFormItem">
           <dnn:Label id="lblSetTitle" runat="server" controlname="chkSetTitle" suffix=":"/>
           <asp:CheckBox  ID="chkSetTitle" runat="server" />
       </div>
       <div class="dnnFormItem">
           <dnn:Label id="lblShowBreadcrumb" runat="server" controlname="chkShowBreadcrumb" suffix=":"/>
           <asp:CheckBox  ID="chkShowBreadcrumb" runat="server" />
       </div>
    </fieldset>
</div>
<asp:MultiView ID="MultiView1" runat="server">
    <asp:View ID="viewTemplate" runat="server">
        <fieldset>
   			<div class="dnnFormItem">
   			    <dnn:Label id="lblShowUpNavigation" runat="server" controlname="chkShowUpNavigation" suffix=":"/>
				<asp:CheckBox ID="chkShowUpNavigation" runat="server"/>
			</div>
            <div class="dnnFormItem">
   			    <dnn:Label id="lblShowThisNode" runat="server" controlname="chkShowThisNode" suffix=":"/>
				<asp:CheckBox ID="chkShowThisNode" runat="server"/>
			</div>
            <div class="dnnFormItem">
   			    <dnn:Label id="lblShowSubNodes" runat="server" controlname="chkShowSubNodes" suffix=":"/>
				<asp:CheckBox ID="chkShowSubNodes" runat="server"/>
			</div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblAllGroupsImage" runat="server" ControlName="AllGroupsImageSelector" Suffix=":"/>
				<div class="dnnLeft">
                    <asp:Image ID="imgImage" runat="server" Width="100" />
                    <dnn:UrlControl ID="AllGroupsImageSelector" runat="server" ShowFiles="true" ShowUrls="false" Required="false"
                                ShowTabs="false" ShowLog="false" ShowTrack="false" ShowUpLoad="true" ShowNewWindow="false" 
                                UrlType="F" />
                </div>
			</div>
			<div class="dnnFormItem">
			    <dnn:Label id="lblProductGroupsInRow" runat="server" controlname="txtProductGroupsInRow" suffix=":"/>
			    <asp:TextBox ID="txtProductGroupsInRow" runat="server" Columns="2" />
            </div>
            <bb:TemplateControl ID="tplTemplate" runat="server" Key="ProductGroup" ViewMode="View" EditorControl="TextBox"/>
        </fieldset>
    </asp:View>
    <asp:View ID="viewTree" runat="server">
        <fieldset>
           <div class="dnnFormItem">
               <dnn:Label id="lblShowExpandCollapse" runat="server" controlname="chkShowExpandCollapse" suffix=":"/>
               <asp:CheckBox ID="chkShowExpandCollapse" runat="server" /></td>
           </div>
		   <div class="dnnFormItem">
		       <dnn:Label id="lblWrapNode" runat="server" controlname="chkWrapNode" suffix=":"/>
               <asp:CheckBox ID="chkWrapNode" runat="server" /></td>
           </div>
           <div class="dnnFormItem">
               <dnn:Label id="lblShowIcons" runat="server" controlname="chkShowIcons" suffix=":"/>
               <asp:CheckBox ID="chkShowIcons" runat="server" /></td>
           </div>
        </fieldset>
    </asp:View>
    <asp:View ID="viewDropDown" runat="server"/>
</asp:MultiView>
