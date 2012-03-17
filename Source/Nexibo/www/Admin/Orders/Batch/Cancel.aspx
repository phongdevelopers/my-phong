<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Cancel.aspx.cs" Inherits="Admin_Orders_Batch_Cancel" Title="Invoices" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <asp:Panel ID="CommentPanel" runat="server">
        <div class="pageHeader">
        	<div class="caption">
        		<h1><asp:Localize ID="Caption" runat="server" Text="Cancel Orders"></asp:Localize></h1>
    	    </div>
    	</div>
        <table class="innerLayout">
            <tr>
                <td align="center">
                    You have requested to <b>CANCEL</b> these orders: <asp:Label ID="OrderList" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td align="center">
                    <asp:Label ID="CommentLabel" runat="server" Text="If desired, you can enter a comment or explanation below.  This comment will be attached to all orders."></asp:Label><br /><br />
                    <asp:TextBox ID="Comment" runat="server" TextMode="MultiLine" Rows="4" Columns="50"></asp:TextBox><br />
                    <asp:CheckBox ID="IsPrivate" runat="server" Text="Make comment private." /><br /><br />
                    <asp:HyperLink ID="BackButton" runat="server" Text="&laquo; Back" SkinID="Button" NavigateUrl="../Default.aspx" />
                    <asp:Button ID="CancelButton" runat="server" Text="Cancel OrderS" OnClick="CancelButton_Click" />
                    <br /><br />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:GridView ID="OrderGrid" runat="server" SkinID="PagedList" Width="100%" AllowPaging="False"
                        AllowSorting="false" AutoGenerateColumns="False" EnableViewState="false">
                        <Columns>
                            <asp:TemplateField HeaderText="Order #" SortExpression="OrderNumber">
                                <ItemStyle HorizontalAlign="center" />
                                <ItemTemplate>
                                    <asp:Label ID="OrderNumber" runat="server" Text='<%# Eval("OrderNumber") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Status" SortExpression="OrderStatusId">
                                <ItemStyle HorizontalAlign="center" Height="30px" />
                                <ItemTemplate>
                                    <asp:Label ID="OrderStatus" runat="server" Text='<%# GetOrderStatus(Eval("OrderStatusId")) %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Customer" SortExpression="BillToLastName">
                                <ItemStyle HorizontalAlign="center" />
                                <ItemTemplate>
                                    <asp:Label ID="CustomerName" runat="server" Text='<%# string.Format("{1}, {0}", Eval("BillToFirstName"), Eval("BillToLastName")) %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Amount" SortExpression="TotalCharges">
                                <ItemStyle HorizontalAlign="center" />
                                <ItemTemplate>
                                    <asp:Label ID="Label5" runat="server" Text='<%# Eval("TotalCharges", "{0:lc}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Date" SortExpression="OrderDate">
                                <ItemStyle HorizontalAlign="center" />
                                <ItemTemplate>
                                    <asp:Label ID="Label6" runat="server" Text='<%# Eval("OrderDate", "{0:d}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Payment">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" />
                                <ItemTemplate>
                                    <asp:PlaceHolder ID="phPaymentStatus" runat="server"></asp:PlaceHolder>
                                    <asp:Label ID="PaymentStatus" runat="server" Text='<%# GetPaymentStatus(Container.DataItem) %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Shipment">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" />
                                <ItemTemplate>
                                    <asp:PlaceHolder ID="phShipmentStatus" runat="server"></asp:PlaceHolder>
                                    <asp:Label ID="ShipmentStatus" runat="server" Text='<%# Eval("ShipmentStatus") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="ConfirmPanel" runat="server" Visible="false">
		<div class="pageHeader">
			<div class="caption">
				<h1><asp:Localize ID="Caption2" runat="server" Text="Cancel Orders"></asp:Localize></h1>
        	</div>
        </div>
        <table cellpadding="2" cellspacing="0" class="innerLayout">
            <tr>
                <td>
                    The selected orders have been cancelled: <asp:Label ID="OrderList2" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:HyperLink ID="FinishButton" runat="server" Text="Finish &raquo;" SkinID="Button" NavigateUrl="../Default.aspx" /><br /><br />
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>