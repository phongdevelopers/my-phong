<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BasketItemDetail.ascx.cs" Inherits="Admin_Orders_Create_BasketItemDetail" EnableViewState="false" %>
<asp:PlaceHolder ID="phProductName" runat="server"></asp:PlaceHolder>
<asp:Localize ID="KitMemberLabel" runat="Server" Text="<br />in {0}" Visible="false"></asp:Localize>
<asp:DataList ID="InputList" runat="server">
    <HeaderTemplate><br /></HeaderTemplate>
    <ItemTemplate>
        <b><asp:Literal ID="InputName" Runat="server" Text='<%#Eval("InputField.Name", "{0}:")%>'></asp:Literal></b>
        <asp:Literal ID="InputValue" Runat="server" Text='<%#Eval("InputValue")%>'></asp:Literal><br />
    </ItemTemplate>
</asp:DataList>
<asp:PlaceHolder ID="KitProductPanel" runat="server" Visible="false">
    <ul class="BasketSubItemLabel">
    <asp:Repeater ID="KitProductRepeater" runat="server">
        <ItemTemplate>
            <li><asp:Literal ID="KitProductLabel" runat="server" Text='<%#Eval("Name")%>' /></li>
        </ItemTemplate>
    </asp:Repeater>
    </ul>
</asp:PlaceHolder>
<asp:Literal ID="WishlistLabel" runat="Server" Text="<br />from {0}&#39;s Wish List"></asp:Literal>
<asp:PlaceHolder ID="ShipsToPanel" runat="server">
    <br />
    <b><asp:Literal ID="ShipsToLiteral" Runat="server" Text="Shipping to:"></asp:Literal></b>
    <asp:Literal ID="ShipsTo" Runat="server" Text=""></asp:Literal>
</asp:PlaceHolder>
<asp:PlaceHolder ID="GiftWrapPanel" runat="server">
    <br /><b><asp:Literal ID="GiftWrapLiteral" Runat="server" Text="Gift Wrap:"></asp:Literal></b>
    <asp:Literal ID="GiftWrap" Runat="server" Text=""></asp:Literal>
</asp:PlaceHolder>
<asp:PlaceHolder ID="GiftMessagePanel" runat="server">
    <br /><b><asp:Literal ID="GiftMessageLiteral" Runat="server" Text="Gift Message:"></asp:Literal></b>
    <asp:Literal ID="GiftMessage" Runat="server" Text=""></asp:Literal>
</asp:PlaceHolder>
<asp:PlaceHolder ID="SubscriptionPanel" runat="server">
    <br />
    <asp:Literal ID="RecuringPaymentMessage" runat="server" Text="This item includes a recurring payment."></asp:Literal><br />
    <asp:Literal ID="InitialPayment" runat="server" Text="Initial Payment: {0:lc}<br />"></asp:Literal>
    <asp:Literal ID="RecurringPayment" runat="server" Text="Recurring Payment: {0} payments of {1:lc}, every {2}."></asp:Literal>
</asp:PlaceHolder>
<asp:PlaceHolder ID="AssetsPanel" runat="server">
    <ul class="BasketSubItemLabel">
    <asp:Repeater ID="AssetLinkList" runat="server">
        <ItemTemplate>
            <li><a href="<%#Eval("NavigateUrl")%>"><%#Eval("Text")%></a></li>
        </ItemTemplate>
    </asp:Repeater>
    </ul>
</asp:PlaceHolder>
