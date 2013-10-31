<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImageScrollerControl.ascx.cs" Inherits="Bitboxx.DNNModules.BBStore.ImageScrollerControl" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<div class="BBImageScroller">
	<asp:Image ID="imgDetail" runat="server" />
	<div style="height:10px; min-height:10px;">&nbsp;</div>
	<telerik:RadRotator ID="RadRotator1" runat="server" 
		ScrollDuration="500" 
		FrameDuration="2000" 
		RotatorType="AutomaticAdvance">
		<ItemTemplate>
			<div style="cursor: pointer">
				<img class="item" src='<%# Container.DataItem %>' alt="Customer Image" />
			</div>
		</ItemTemplate>
	</telerik:RadRotator>
</div>