<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MiniBasketItemDetail.ascx.cs" Inherits="Admin_Orders_Create_MiniBasketItemDetail" EnableViewState="false" %>
<%-- 
<conlib>
<summary>Shows details of a single product which is currently in basket. Used to display details in minibasket control for all items.</summary>
<param name="LinkProducts" default="false">Possible values are true or false.  Indicates whether the basket items should have a link to the product page or not.</param>
<param name="ShowShipTo" default="false">Possible values are true or false.  Indicates whether the shipping address should be displayed with each item or not.</param>
<param name="ShowAssets" default="false">Possible values are true or false.  Indicates whether the assets like readme files and license agreements will be shown or not.</param>
<param name="ShowSubscription" default="false">Possible values are true or false.  Indicates whether subscriptions information will be shown or not.</param>
</conlib>
--%>
<div class="miniBasketItemTitle">
	<asp:PlaceHolder ID="phProductName" runat="server"></asp:PlaceHolder>
</div>
<asp:DataList ID="InputList" runat="server" CssClass="miniBasketSubItemTitle">
    <HeaderTemplate><br /></HeaderTemplate>
    <ItemTemplate>
        <b><asp:Literal ID="InputName" Runat="server" Text='<%#Eval("InputField.Name", "{0}:")%>' > </asp:Literal></b>
        <asp:Literal ID="InputValue" Runat="server" Text='<%#Eval("InputValue")%>' ></asp:Literal><br />
    </ItemTemplate>
</asp:DataList>
<asp:PlaceHolder ID="KitProductPanel" runat="server">
    <ul class="miniBasketSubItemTitle">
    <asp:Repeater ID="KitProductRepeater" runat="server">
        <ItemTemplate>
            <li ><asp:Literal ID="KitProductLabel" runat="server" Text='<%#Eval("Name")%>' /></li>
        </ItemTemplate>
    </asp:Repeater>
    </ul>
</asp:PlaceHolder>
<asp:Literal ID="WishlistLabel" runat="Server" Text="from {0}&#39;s Wish List<br />"></asp:Literal>
<asp:PlaceHolder ID="SubscriptionPanel" runat="server">
    <br />
    <asp:Literal ID="RecuringPaymentMessage" runat="server" Text="This item includes a recurring payment."></asp:Literal><br />
    <asp:Literal ID="InitialPayment" runat="server" Text="Initial Payment: {0:ulc}<br />"></asp:Literal>
    <asp:Literal ID="RecurringPayment" runat="server" Text="Recurring Payment: {0} payments of {1:ulc}, every {2}."></asp:Literal>
</asp:PlaceHolder>
<asp:PlaceHolder ID="AssetsPanel" runat="server">
    <ul class="miniBasketSubItemTitle">
    <asp:Repeater ID="AssetLinkList" runat="server">
        <ItemTemplate>
            <li><a href="<%#Eval("NavigateUrl")%>"><%#Eval("Text")%></a></li>
        </ItemTemplate>
    </asp:Repeater>
    </ul>
</asp:PlaceHolder>
