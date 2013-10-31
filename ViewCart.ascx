<%@ Control Language="C#" Inherits="Bitboxx.DNNModules.BBStore.ViewCart" AutoEventWireup="true" CodeBehind="ViewCart.ascx.cs" %>
<%@ Register TagPrefix="bb" Assembly="Bitboxx.Web.GeneratedImage" Namespace="Bitboxx.Web.GeneratedImage" %>
<%@ Register TagPrefix="bb" TagName="CartNavigationControl" Src="Controls/CartNavigationControl.ascx" %>  
<div class="bbstore-cart dnnClear">
	<bb:CartNavigationControl ID="navCart" runat="server" />
    <asp:Panel runat="server" ID="pnlError" Visible="false">
        <p>&nbsp;</p>
        <div class="dnnFormMessage dnnFormValidationSummary">
            <asp:Label ID="lblError" runat="server" />
        </div>
    </asp:Panel>
    <asp:MultiView ID="MultiView1" runat="server">
        <asp:View ID="viewCartPane" runat="server">
   			<asp:Panel ID="pnlCartUpDownload" runat="server" Visible="False">
				<asp:Button ID="cmdLoadCart" runat="server" Resourcekey="cmdLoadCart.Text" onclick="cmdLoadCart_Click" />
				<asp:Button ID="cmdSaveCart" runat="server" Resourcekey="cmdSaveCart.Text" onclick="cmdSaveCart_Click" />
				<asp:Button ID="cmdDeleteCart" runat="server" Resourcekey="cmdDeleteCart.Text" onclick="cmdDeleteCart_Click" />
				<br/>
				<br/>
			</asp:Panel>
            <asp:Panel ID="pnlFullCart" runat="server">
                <asp:Panel ID="pnlConfirm" runat="server" Visible="false">
                    <p>&nbsp;</p>
                    <div class="dnnFormMessage dnnFormInfo">
                        <asp:Label ID="lblConfirmHeader" runat="server" Resourcekey="lblConfirmHeader.Text"/>
                    </div>
                    <p>&nbsp;</p>
                    <hr />
					<asp:ListView ID="lstCustomerAddresses" runat="server"  GroupItemCount="3" OnItemDataBound="lstCustomerAddresses_ItemDataBound">
						<LayoutTemplate>
							<table style="width: 100%" >
								<asp:PlaceHolder runat="server" ID="groupPlaceHolder"></asp:PlaceHolder>
							</table>
						</LayoutTemplate>
						<GroupTemplate>
							<tr>
								<asp:PlaceHolder runat="server" ID="itemPlaceHolder"></asp:PlaceHolder>
							</tr>
						</GroupTemplate>
						<ItemTemplate>
							<td style="width:33%">
								<b><asp:Label runat="server" ID="lblAddressType" /></b><br/>
								<asp:Label runat="server" ID="lblAddress" />
							</td>
						</ItemTemplate>
					</asp:ListView>        
                    <hr />
                    <p><b><asp:Label ID="lblPaymentConfirmCap" runat="server" ResourceKey="lblPaymentConfirm.Text" /></b> <asp:Label ID="lblPaymentConfirm" runat="server"/></p>
                    <asp:PlaceHolder runat="server" ID="phPaymentSummary"/>
                    <hr />
                </asp:Panel>
                <asp:GridView ID="grdCartProducts" runat="server" AutoGenerateColumns="False" 
                    CssClass="bbstore-grid" onrowdatabound="grdCartProducts_RowDataBound">
                    <Columns>
                        <asp:BoundField DataField="CartProductId" HeaderText="CartProductId" SortExpression="CartProductId" Visible="false" />
                        <asp:BoundField DataField="CartId" HeaderText="CartId" SortExpression="CartId" Visible="False" />
                        <asp:BoundField DataField="ProductId" HeaderText="ProductId" SortExpression="ProductId" Visible="False" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <bb:GeneratedImage ID="imgThumb" runat="server" ImageHandlerUrl="~/BBImageHandler.ashx"/>
                            </ItemTemplate>
                        </asp:TemplateField>   
                        <asp:TemplateField HeaderText="Quantity">
                            <ItemTemplate>
                                <asp:HiddenField ID="HiddenCartProductId" runat="server" Value='<%# Eval("CartProductId") %>' />
                                <asp:TextBox ID="txtQuantity" runat="server" Text='<%# String.Format("{0:G}",Convert.ToDouble(Eval("Quantity"))) %>' 
                                    ontextchanged="txtQuantity_TextChanged" AutoPostBack="True"></asp:TextBox><asp:Label ID="lblQuantity" runat="server" Text='<%# String.Format("{0:G}",Convert.ToDouble(Eval("Quantity"))) %>' Visible="false"/>
                            </ItemTemplate>
                        </asp:TemplateField> 
                        <asp:BoundField DataField="Unit" HeaderText="Unit" SortExpression="Unit" />               
                        <asp:BoundField DataField="ItemNo" HeaderText="ItemNo" SortExpression="ItemNo" />
                        <asp:TemplateField HeaderText="Product">
                            <ItemTemplate>
                                <asp:Label ID="lblName" runat="server" Text='<%# Eval("Name") %>'/>
                                <asp:HyperLink ID="lnkName" runat="server" Text='<%# Eval("Name") %>' NavigateUrl='<%# Eval("ProductUrl") %>'/>
                                <br /><asp:Label ID="lblOption" runat="server" style="font-size:x-small"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="UnitCost" HeaderText="UnitCost" SortExpression="UnitCost" DataFormatString="{0:n2}" >
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                        <asp:BoundField DataField="NetTotal" HeaderText="NetTotal" SortExpression="NetTotal" DataFormatString="{0:n2}" >
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TaxPercent" HeaderText="TaxPercent" SortExpression="TaxPercent" DataFormatString="{0:F1}" >
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TaxTotal" HeaderText="TaxTotal" SortExpression="TaxTotal" DataFormatString="{0:n2}" >
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                        <asp:BoundField DataField="SubTotal" HeaderText="SubTotal" SortExpression="SubTotal" DataFormatString="{0:n2}" >
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                    </Columns>
                    <HeaderStyle CssClass="bbstore-grid-header" />
                    <RowStyle CssClass="bbstore-grid-row" />
                    <AlternatingRowStyle CssClass="bbstore-grid-alternaterow" />
                </asp:GridView>
                <asp:GridView ID="grdAdditionalCosts" runat="server" AutoGenerateColumns="False" ShowHeader="False" CssClass="bbstore-grid">
                    <Columns>
                        <asp:BoundField DataField="CartAdditionalCostId" HeaderText="CartAdditionalCostId" 
                            SortExpression="CartAdditionalCostId" Visible="False" />
                        <asp:BoundField DataField="CartId" HeaderText="CartId" 
                            SortExpression="CartId" Visible="False" />
                        <asp:BoundField HeaderText="Image" />
                        <asp:TemplateField HeaderText="Quantity" SortExpression="Quantity">
                            <ItemTemplate>
                                <span style="padding:4px;"><asp:Label ID="Label1" runat="server" Text='<%# String.Format("{0:G}",Convert.ToDouble(Eval("Quantity"))) %>'></asp:Label></span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Unit" />
                        <asp:BoundField HeaderText="ItemNo" />
                        <asp:TemplateField HeaderText="Product">
                            <ItemTemplate>
                                <span style="padding:4px;"><asp:Label ID="Label1" runat="server" Text='<%# Eval("Name") %>'></asp:Label></span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="UnitCost" DataFormatString="{0:n2}" 
                            HeaderText="UnitCost" SortExpression="UnitCost">
                        <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                        <asp:BoundField DataField="NetTotal" DataFormatString="{0:n2}" 
                            HeaderText="NetTotal" SortExpression="NetTotal">
                        <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TaxPercent" DataFormatString="{0:F1}" 
                            HeaderText="TaxPercent" SortExpression="TaxPercent">
                        <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TaxTotal" DataFormatString="{0:n2}" 
                            HeaderText="TaxTotal" SortExpression="TaxTotal">
                        <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                        <asp:BoundField DataField="SubTotal" DataFormatString="{0:n2}" 
                            HeaderText="SubTotal" SortExpression="SubTotal">
                        <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                    </Columns>
                </asp:GridView>        
                <asp:GridView ID="grdSubTotal" runat="server" AutoGenerateColumns="False" 
                    ShowHeader="False" AllowSorting="True" CssClass="bbstore-grid">
                    <Columns>
                        <asp:BoundField DataField="Description" HeaderText="Description">
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                        <asp:BoundField  HeaderText="UnitCost" />
                        <asp:BoundField DataField="NetTotal" DataFormatString="{0:n2}" HeaderText="NetTotal" SortExpression="NetTotal">
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                        <asp:BoundField  HeaderText="TaxPercent" />
                        <asp:BoundField DataField="TaxTotal" DataFormatString="{0:n2}" HeaderText="TaxTotal" SortExpression="TaxTotal">
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                        <asp:BoundField DataField="SubTotal" DataFormatString="{0:n2}" HeaderText="SubTotal" SortExpression="SubTotal">
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                     </Columns>
                     <RowStyle CssClass="bbstore-grid-sumrow" />
                </asp:GridView>
                <asp:GridView ID="grdSummary" runat="server" AutoGenerateColumns="False" ShowHeader="False" 
                AllowSorting="True" GridLines="None" CssClass="bbstore-grid-summary">
                    <Columns>
                        <asp:BoundField DataField="Text"  HeaderText="Text" >
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Amount" DataFormatString="{0:n2}" HeaderText="Amount">
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                     </Columns>
                </asp:GridView>
                <p>&nbsp;</p>
                <asp:Panel ID="pnlConfirm2" runat="server" Visible="false">
                    <p><asp:CheckBox ID="chkTerms" runat="server" />&nbsp;
                        <asp:Label ID="lblTermsPre" runat="server" Resourcekey="lblTermsPre.Text"/>&nbsp;
                        <asp:LinkButton ID="lnkTerms" runat="server" ResourceKey="lnkTerms.Text" />&nbsp;
                        <asp:Label ID="lblTermsPost" runat="server" Resourcekey="lblTermsPost.Text"/></p>
                    <p><asp:Label ID="lblRemarks" runat="server" Resourcekey="lblRemarks.Text"/><br />
                        <asp:TextBox ID="txtRemarks" runat="server" Rows="5" TextMode="MultiLine" Columns="80" /></p>
					<p><asp:Label ID="lblAttachment" runat="server" Resourcekey="lblAttachment.Text"/><asp:FileUpload runat="server" ID="fulAttachment"/></p>                
                    <asp:Label ID="lblTermsError" runat="server" CssClass="dnnFormMessage dnnFormValidationSummary" Visible="false" ResourceKey="lblTerms.Error"/>
                    <div style="text-align:right"><asp:Button ID="cmdFinish" runat="server" Resourcekey="cmdFinish.Text" onclick="cmdFinish_Click" /></div>
                </asp:Panel>
                <asp:Panel ID="pnlCheckout" runat="server" Visible="true">
					<table style="width:100%;">
						<tr>
							<td style="text-align:left;"><asp:Button ID="cmdShopping" runat="server" Resourcekey="cmdShopping.Text" onclick="cmdShopping_Click" /></td>
							<td style="text-align:right;"><asp:Button ID="cmdCheckout" runat="server" Resourcekey="cmdCheckout.Text" onclick="cmdCheckout_Click" /></td>
						</tr>
					</table>
                </asp:Panel>
            </asp:Panel>
            <asp:Panel ID="pnlEmptyCart" runat="server" Visible="false">
	            <asp:Literal ID="ltrEmptyCart" runat="server" />
            </asp:Panel>
			<asp:Panel ID="pnlUploadCart" runat="server" Visible="False">
				<asp:FileUpload ID="fulCart" runat="server" />	
				<asp:Button ID="cmdUploadCart" Resourcekey="cmdUploadCart.Text" runat="server" OnClick="cmdUploadCart_Click" />	
			</asp:Panel>
		</asp:View>
        <asp:View ID="viewLogin" runat="server" >
            <asp:PlaceHolder ID="phLogin" runat="server" />
        </asp:View>
        <asp:View ID="viewCheckout" runat="server">
	        <asp:PlaceHolder runat="server" ID="phCheckout" />
        </asp:View>
        <asp:View ID="viewAddressEdit" runat="server">
	        <asp:PlaceHolder runat="server" ID="phAddressEdit" />
         </asp:View>
        <asp:View ID="viewShipping" runat="server">
             Shipping
        </asp:View>
        <asp:View ID="viewPayment" runat="server">
            <asp:PlaceHolder runat="server" ID="phPayment" />
         </asp:View>
        <asp:View ID="viewFinish" runat="server">
             <asp:Literal ID="ltrConfirmCart" runat="server" />
             <p><asp:Button ID="cmdShopping2" runat="server" Resourcekey="cmdShopping.Text" onclick="cmdShopping_Click" /></p>
        </asp:View>
		<asp:View ID="viewPaypalIPN" runat="server">
             <p><asp:Label ID="lblPaypalIPNHead" runat="server" ResourceKey="lblFinishHead.Text"></asp:Label></p>
             <p><asp:Label ID="lblPaypalIPNText" runat="server"></asp:Label></p>
             <p><asp:Button ID="cmdShopping3" runat="server" Resourcekey="cmdShopping.Text" onclick="cmdShopping_Click" /></p>
        </asp:View>

    </asp:MultiView>
</div>