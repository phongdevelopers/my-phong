<%@ Page Language="C#" MasterPageFile="~/Admin/Orders/Order.master" AutoEventWireup="true" CodeFile="ViewOrder.aspx.cs" Inherits="Admin_Orders_ViewOrder" Title="View Order" %>
<%@ Register Src="../UserControls/OrderItemDetail.ascx" TagName="OrderItemDetail" TagPrefix="uc" %>
<%@ Register Src="../UserControls/OrderTotalSummary.ascx" TagName="OrderTotalSummary" TagPrefix="uc" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
        <div class="caption">
            <h1><asp:Localize ID="Caption" runat="server" Text="Order #{0} - {1}" EnableViewState="false"></asp:Localize></h1>
        </div>
    </div>
    <table class="inputForm" border="0" cellpadding="4" cellspacing="0" width="100%">
        <tr>
            <th class="rowHeader">
                <asp:Label ID="OrderActionLabel" runat="server" Text="Tasks:" SkinID="FieldHeader"></asp:Label>
            </th>
            <td colspan="3">
                <table cellpadding="0" cellspacing="5" border="0" >
                    <tr>
                        <td><asp:DropDownList ID="OrderAction" runat="server"></asp:DropDownList></td>
                        <td align="left"><asp:ImageButton ID="OrderActionButton" runat="server" SkinID="GoIcon" OnClick="OrderActionButton_Click" /></td>
                    </tr>
                </table>
            </td>        
            <td colspan="2">&nbsp;</td>
        </tr>
        <tr>
             <th class="rowHeader">
                <asp:Label ID="OrderReferrerLabel" runat="server" Text="Referrer:" SkinID="FieldHeader"></asp:Label>
            </th>
            <td colspan="5">
                <asp:HyperLink ID="OrderReferrer" runat="server" SkinID="Link"></asp:HyperLink>
            </td>
        </tr>
        <tr>
            <th class="rowHeader">
                <asp:Label ID="OrderDateLabel" runat="server" Text="Date:" SkinID="FieldHeader"></asp:Label>
            </th>
            <td>
                <asp:Label ID="OrderDate" runat="server" Text=""></asp:Label>
            </td>
            <th class="rowHeader">
                <asp:Label ID="CurrentPaymentStatusLabel" runat="server" Text="Payment:" SkinID="FieldHeader"></asp:Label>
            </th>
            <td  align="left">
                <asp:Label ID="CurrentPaymentStatus" runat="server" Text=""></asp:Label>
            </td>
            <th class="rowHeader">
                <asp:Label ID="AffiliateLabel" runat="server" Text="Affiliate:" SkinID="FieldHeader"></asp:Label>
            </th>
            <td>
                <asp:Label ID="Affiliate" runat="server" Text=""></asp:Label>
            </td>
           
        </tr>
        <tr>
            <th class="rowHeader">
                <asp:Label ID="OrderTotalLabel" runat="server" Text="Total:" SkinID="FieldHeader"></asp:Label>
            </th>
            <td>
                <asp:Label ID="OrderTotal" runat="server" Text=""></asp:Label>
            </td>
            <th class="rowHeader">
                <asp:Label ID="CurrentShipmentStatusLabel" runat="server" Text="Shipment:" SkinID="FieldHeader"></asp:Label>
            </th>
            <td>
                <asp:Label ID="CurrentShipmentStatus" runat="server" Text=""></asp:Label>
            </td>
            <th class="rowHeader" nowrap>
                <asp:Label ID="CustomerIPLabel" runat="server" Text="Customer IP:" SkinID="FieldHeader"></asp:Label>
            </th>
            <td>
                <asp:Label ID="CustomerIP" runat="server" Text=""></asp:Label>&nbsp;
                <asp:Label ID="CustomerIPBlocked" runat="server" Text="BLOCKED" SkinID="ErrorCondition"></asp:Label>
                <asp:ImageButton ID="BlockCustomerIP" runat="server" SkinID="BlockIcon" AlternateText="Block IP" OnClientClick="return confirm('Are you sure you want to block the IP {0}?')" OnClick="BlockCustomerIP_Click" />
            </td>
        </tr>
    </table><br />
    <table>
        <tr>
            <td colspan="2" width="200px" valign="top">
                <h3 style="margin:2px;"><asp:Localize ID="BillToCaption" runat="server" Text="Bill To:"></asp:Localize></h3>
                <asp:Label ID="BillToAddress" runat="server" Text=""></asp:Label><br />
                <ajax:UpdatePanel ID="OrderEmailAjax" runat="server" >
                <ContentTemplate>                    
                    <asp:HyperLink ID="BillToEmail" runat="server" Text="" NavigateUrl="Email/SendMail.aspx" SkinID="Link" EnableViewState="false"></asp:HyperLink>
					<asp:PlaceHolder ID="phEditBillToEmail" runat="server">						
						<asp:ImageButton ID="EditEmailIcon" runat="server" SkinID="editicon" OnClick="ChangeOrderEmailButton_Click"  />
					</asp:PlaceHolder>
                    <asp:Panel ID="EditOrderEmailPanel" runat="server" Visible="false">
                        <asp:TextBox ID="OrderEmail" runat="server" MaxLength="50" Columns="25"></asp:TextBox>
                        <asp:ImageButton ID="SaveOrderEmailButton" runat="server" ValidationGroup="OrderEmail" OnClick="SaveOrderEmailButton_Click" SkinID="saveIcon"  />
                        <cb:EmailAddressValidator ID="EmailAddressValidator1" runat="server" ControlToValidate="OrderEmail" ValidationGroup="OrderEmail" Required="true" ErrorMessage="Email address should be in the format of name@domain.tld." Text="Email address should be in the format of name@domain.tld." Display="dynamic"></cb:EmailAddressValidator>                        
                    </asp:Panel>
                </ContentTemplate>
                </ajax:UpdatePanel>                
                <asp:Localize ID="BillToPhone" runat="server" Text="Phone: {0}<br />" />
                <asp:Localize ID="BillToFax" runat="server" Text="Fax: {0}<br />" />
            </td>
            <td colspan="2" width="200px" valign="top">
                <asp:Label ID="ShipToCaption" runat="server" Text="Ship To:" SkinID="FieldHeader"></asp:Label><br />
                <asp:Label ID="ShipToAddress" runat="server" Text=""></asp:Label><br /><br />
                <asp:Panel id="CurrentShipmentStatusPanel" runat="server">
                    <asp:Label ID="ShipMethodLabel" runat="server" Text="Ship via: " SkinID="FieldHeader"></asp:Label><asp:Label ID="ShipMethod" runat="server" Text=""></asp:Label><br />
                </asp:Panel>
            </td>
            <td colspan="2" width="200px" valign="top">
                <h3 style="margin:2px;"><asp:Localize ID="LastPaymentCaption" runat="server" Text="Payment:"></asp:Localize></h3>
                <asp:Panel id="LastPaymentPanel" runat="server">
                    <asp:Label ID="LastPaymentAmountLabel" runat="server" Text="Amount: "></asp:Label><asp:Label ID="LastPaymentAmount" runat="server" Text=""></asp:Label><br />
                    <asp:Label ID="LastPaymentStatusLabel" runat="server" Text="Status: "></asp:Label><asp:Label ID="LastPaymentStatus" runat="server" Text=""></asp:Label><br />
                    <asp:Label ID="LastPaymentReferenceLabel" runat="server" Text="Ref: "></asp:Label><asp:Label ID="LastPaymentReference" runat="server" Text=""></asp:Label><br />
                    <asp:Label ID="LastPaymentAVSLabel" runat="server" Text="AVS: "></asp:Label><asp:Label ID="LastPaymentAVS" runat="server" Text=""></asp:Label><br />
                    <asp:Label ID="LastPaymentCVVLabel" runat="server" Text="CVV: "></asp:Label><asp:Label ID="LastPaymentCVV" runat="server" Text=""></asp:Label><br />	
				    <asp:HyperLink ID="AllPaymentsLink" runat="server" Text="View All Payments" NavigateUrl='Payments/Default.aspx?OrderNumber={0}&OrderId={1}' Visible="false"></asp:HyperLink>
                </asp:Panel>
                <asp:Panel ID="ButtonPanel" runat="server" Visible="false">
                    <asp:HyperLink ID="DetailsLink" runat="server" SkinID="Button" Text="Details"></asp:HyperLink>
                    <asp:Button ID="ReceivedButton" runat="server" Text="Received" OnClick="ReceivedButton_Click"></asp:Button>
                    <asp:HyperLink ID="VoidLink" runat="server" SkinID="Button" Text="Void"></asp:HyperLink>
                    <asp:HyperLink ID="CaptureLink" runat="server" SkinID="Button" Text="Capture"></asp:HyperLink>
                </asp:Panel>
            </td>
            <td colspan="2" width="200px" valign="top">
                <asp:Label ID="BalanceCaption" runat="server" Text="Balance:" SkinID="FieldHeader"></asp:Label><br />
                <asp:Label ID="OrderTotal1" runat="server" Text="Order Total: {0:lc}"></asp:Label><br />
			    <asp:Label ID="TotalPayment" runat="server" Text="Payments Total: {0:lc}"></asp:Label><br />
			    <asp:Label ID="RemainingBalance" runat="server" Text="Remaining Balance: {0:lc}"></asp:Label><br /><br />
                <asp:Label ID="PaymentProcessingCaption" runat="server" Text="Payment Processing:" SkinID="FieldHeader"></asp:Label><br />
			    <asp:Label ID="ProcessedPayments" runat="server" Text="Processed Payments: {0:lc}"></asp:Label><br />
			    <asp:Label ID="UnprocessedPayments" runat="server" Text="Unprocessed Payments: {0:lc}"></asp:Label><br />
            </td>
        </tr>
    </table><br />
    <div class="sectionHeader">
        <h2><asp:Localize ID="Localize1" runat="server" Text="Order Contents"></asp:Localize></h2>
    </div>
    <asp:GridView ID="OrderItemGrid" runat="server" AutoGenerateColumns="False" 
        DataKeyNames="OrderItemId" SkinID="NestedList" CellPadding="4" CellSpacing="0"
        Width="800px" OnDataBinding="OrderItemGrid_DataBinding">
        <Columns>
            <asp:TemplateField HeaderText="SKU" SortExpression="Sku">
                <ItemStyle HorizontalAlign="center" />
                <ItemTemplate>
                    <asp:Label ID="Sku" runat="server" Text='<%# ProductHelper.GetSKU(Container.DataItem) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Name" SortExpression="Name">
                <ItemTemplate>
					<asp:HyperLink ID="GiftCertsLink" runat="server" Visible='<%# IsGiftCert(Container.DataItem) %>'
                        NavigateUrl='<%# String.Format("ViewGiftCertificates.aspx?OrderNumber={0}&OrderId={1}", Eval("Order.OrderNumber"), Eval("OrderId")) %>'>
                        <asp:Image ID="GiftCertIcon" runat="server" SkinID="GiftCertIcon" AlternateText="View Gift Certificates" ToolTip="View Gift Certificates" />
                   </asp:HyperLink>
					<asp:HyperLink ID="DigitalGoodsLink" runat="server" Visible='<%# IsDigitalGood(Container.DataItem) %>'
                        NavigateUrl='<%# String.Format("ViewDigitalGoods.aspx?OrderNumber={0}&OrderId={1}", Eval("Order.OrderNumber"), Eval("OrderId")) %>'>
                        <asp:Image ID="DigitalGoodIcon" runat="server" SkinID="DigitalGoodIcon" AlternateText="View Digital Goods" ToolTip="View Digital Goods"/>
                   </asp:HyperLink>
                    <uc:OrderItemDetail ID="OrderItemDetail1" runat="server" OrderItem='<%#(OrderItem)Container.DataItem%>' ShowAssets="False" LinkProducts="True" />                                    
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Tax">
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" Width="40px" />
                <ItemTemplate>
                    <%#TaxHelper.GetTaxRate((Order)Eval("Order"), (OrderItem)Container.DataItem).ToString("0.####")%>%
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Price" SortExpression="Price" ItemStyle-HorizontalAlign="right">
                <ItemTemplate>
                    <asp:Label ID="Price" runat="server" Text='<%# Eval("Price", "{0:lc}") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Quantity" SortExpression="Quantity" ItemStyle-HorizontalAlign="center">
                <ItemTemplate>
                    <asp:Label ID="Quantity" runat="server" Text='<%# Eval("Quantity") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Total" SortExpression="ExtendedPrice" ItemStyle-HorizontalAlign="right">
                <ItemTemplate>
                    <asp:Label ID="ExtendedPrice" runat="server" Text='<%# Eval("ExtendedPrice", "{0:lc}") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>    
    <table width="800px" cellpadding="2" cellspacing="1" border="0">
        <tr>
          <td width="70%">&nbsp;</td>
          <td class="orderSummary">
            <uc:OrderTotalSummary ID="OrderTotalSummary1" runat="server" />
          </td>  
        </tr>
    </table>
</asp:Content>