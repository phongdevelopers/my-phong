<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OrderMenu.ascx.cs" Inherits="Admin_Orders_OrderMenu" EnableViewState="false" %>
<asp:Panel ID="OrderMenuPanel" runat="server">
    <asp:HyperLink ID="Summary" runat="server" NavigateUrl="ViewOrder.aspx" Text="Summary" CssClass="contextMenuButton menu"></asp:HyperLink>
    <asp:HyperLink ID="Payments" runat="server" NavigateUrl="Payments/Default.aspx" Text="Payments" CssClass="contextMenuButton menu"></asp:HyperLink>
    <asp:HyperLink ID="Shipments" runat="server" NavigateUrl="Shipments/Default.aspx" Text="Shipments" CssClass="contextMenuButton menu"></asp:HyperLink>
    <asp:HyperLink ID="History" runat="server" NavigateUrl="OrderHistory.aspx" Text="History and Notes" CssClass="contextMenuButton menu"></asp:HyperLink>
    <asp:HyperLink ID="Subscriptions" runat="server" NavigateUrl="ViewSubscriptions.aspx" Text="Subscriptions" CssClass="contextMenuButton menu"></asp:HyperLink>
    <asp:HyperLink ID="DigitalGoods" runat="server" NavigateUrl="ViewDigitalGoods.aspx" Text="Digital Goods" CssClass="contextMenuButton menu"></asp:HyperLink>
	<asp:HyperLink ID="GiftCertificates" runat="server" NavigateUrl="ViewGiftCertificates.aspx" Text="Gift Certificates" CssClass="contextMenuButton menu"></asp:HyperLink>
    <asp:HyperLink ID="EditAddresses" runat="server" NavigateUrl="EditAddresses.aspx" Text="Edit Addresses" CssClass="contextMenuButton menu"></asp:HyperLink>
    <asp:HyperLink ID="EditOrderItems" runat="server" NavigateUrl="Edit/EditOrderItems.aspx" Text="Edit Order Items" CssClass="contextMenuButton menu"></asp:HyperLink>
    <asp:HyperLink ID="Customer" runat="server" NavigateUrl="../People/Users/EditUser.aspx" Text="Customer Profile" CssClass="contextMenuButton menu"></asp:HyperLink>
    <asp:Label ID="test" runat="server"></asp:Label>
</asp:Panel>
