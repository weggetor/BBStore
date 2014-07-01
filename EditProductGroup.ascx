<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditProductGroup.ascx.cs"
    Inherits="Bitboxx.DNNModules.BBStore.EditProductGroup" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="UrlControl" Src="~/controls/urlcontrol.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register TagPrefix="bb" TagName="TabSelectControl" Src="Controls/TabSelectControl.ascx" %>
<%@ Register TagPrefix="bb" TagName="LanguageEditor" Src="Controls/LanguageEditorControl.ascx" %>  
<div class="dnnForm bbstore-productgroup-edit dnnClear">
	<asp:Panel ID="pnlProductGroupTree" runat="server">
	    <div class="dnnFormItem">
		    <dnn:Label ID="lblSelPG" runat="server" ControlName="treeProductGroup"  Suffix=":" />
            <div class="dnnLeft">
		        <asp:TreeView ID="treeProductGroup" ExpandDepth="1" runat="server"  SkipLinkText="abc"
			        ontreenodepopulate="treeProductGroup_TreeNodePopulate" 
			        onselectednodechanged="treeProductGroup_SelectedNodeChanged">
		        </asp:TreeView>
            </div>
        </div>
	</asp:Panel>
 	<asp:Panel ID="pnlProductGroupDetails" runat="server" Visible="false">
		<h3><asp:Label ID="lblPGDetails" runat="server" Visible="false" /></h3>
        <asp:Panel ID="pnlProductGroupEditDetails" runat="server" Visible="false">
            
   		    <div class="dnnFormItem">
			    <dnn:Label ID="lblDisabled" runat="server" style="white-space:nowrap" controlname="chkDisabled" suffix=":"></dnn:Label>
			    <asp:CheckBox ID="chkDisabled" runat="server" />
		    </div>
		    <div class="dnnFormItem">
			    <dnn:Label ID="lblViewOrder" runat="server" style="white-space:nowrap" controlname="txtViewOrder" suffix=":"></dnn:Label>
			    <asp:TextBox ID="txtViewOrder" runat="server" Columns="3" />
		    </div>
		    <div class="dnnRight">
			    <asp:ImageButton ID="imgRefreshImg" runat="server" ImageUrl="~/images/action_refresh.gif" OnClick="imgRefreshImg_Click"/>
			    <asp:LinkButton ID="cmdRefreshImg" runat="server" ResourceKey="cmdRefresh" OnClick="imgRefreshImg_Click"/>
		    </div>
		    <div class="dnnFormItem">
		        <dnn:Label ID="lblImage" runat="server" ControlName="ImageSelector" Suffix=":"/>
                <div class="dnnLeft">
                    <dnn:UrlControl ID="ImageSelector" runat="server" ShowFiles="true" ShowUrls="false" Required="false"
                                    ShowTabs="false" ShowLog="false" ShowTrack="false" ShowUpLoad="true" ShowNewWindow="false" 
                                    UrlType="F" />
                </div>
                <div class="dnnLeft">
                    <asp:Image ID="imgImage" runat="server" Width="100" />
                </div>
		    </div>
		    <div class="dnnFormItem">
		        <dnn:Label ID="lblIcon" runat="server" ControlName="txtIcon" Suffix=":"/>
			
                <div class="dnnLeft">
                    <dnn:UrlControl ID="IconSelector" runat="server" ShowFiles="true" ShowUrls="false" Required="false"
                                    ShowTabs="false" ShowLog="false" ShowTrack="false" ShowUpLoad="true" ShowNewWindow="false"
                                    UrlType="F" />
                </div>
                <div class="dnnLeft">
                    <asp:Image ID="imgIcon" runat="server" Width="16" />
                </div>
		    </div>
		    <div class="dnnFormItem">
			    <dnn:Label ID="lblTarget" runat="server" style="white-space:nowrap" controlname="urlTarget" suffix=":"></dnn:Label>
			    <bb:TabSelectControl ID="urlTarget" runat="server" Width="200"/>
		    </div>

            <bb:LanguageEditor ID="lngProductGroups" runat="server" InternalType="Bitboxx.DNNModules.BBStore.ProductGroupLangInfo" />

            <div class="dnnFormItem">
		        <dnn:Label id="lblFeatureLists" runat="server" controlname="ctlFeatureLists" suffix=":" />
		        <dnn:DualListBox id="ctlFeatureLists" runat="server" 
			        DataValueField="FeatureListId" 
			        DataTextField="FeatureList" 
			        AddKey="AddList" 
			        RemoveKey="RemoveList" 
			        AddAllKey="AddAllLists" 
			        RemoveAllKey="RemoveAllLists" 
			        AddImageURL="~/Images/FileManager/MoveNext.gif"
			        AddAllImageURL="~/Images/FileManager/MoveLast.gif"
			        RemoveImageURL = "~/Images/FileManager/MovePrevious.gif"
			        RemoveAllImageURL = "~/Images/FileManager/MoveFirst.gif">
			        <AvailableListBoxStyle Height="130px" Width="225px" />
			        <HeaderStyle />
			        <SelectedListBoxStyle Height="130px" Width="225px"  />
		        </dnn:DualListBox>
            </div>
        </asp:Panel>
        <ul class="dnnActions dnnClear">
            <li><asp:LinkButton CssClass="dnnPrimaryAction" ID="cmdUpdate"	runat="server" resourcekey="cmdUpdate" OnClick="cmdUpdate_Click" visible="false" /></li>
            <li><asp:LinkButton CssClass="dnnSecondaryAction" ID="cmdDelete" runat="server" resourcekey="cmdDelete" onclick="cmdDelete_Click" visible="false" /></li>
            <li><asp:LinkButton CssClass="dnnSecondaryAction" ID="cmdAdd" runat="server" resourcekey="cmdAdd" onclick="cmdAdd_Click" visible="false" /></li>
            <li><asp:LinkButton CssClass="dnnSecondaryAction" ID="cmdCancel" runat="server" resourcekey="cmdCancel" OnClick="cmdCancel_Click" visible="false" /></li>
        </ul>
	</asp:Panel>
    <asp:Panel ID="pnlBack" runat="server">
        <hr />
        <p>
            <asp:ImageButton ID="imgBack" runat="server" resourcekey="cmdBack" ImageUrl="~/images/lt.gif" OnClick="cmdBack_Click" />
            <asp:LinkButton CssClass="dnnSecondaryAction" ID="cmdBack" runat="server" resourcekey="cmdBack" OnClick="cmdBack_Click" />
        </p>
    </asp:Panel>
</div>


				
