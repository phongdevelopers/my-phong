<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OrderItemDetail.ascx.cs" Inherits="Admin_UserControls_OrderItemDetail" %>
<%-- 
<conlib>
<summary>Shows detials of an order item.</summary>
<param name="LinkProducts" default="false">Possible values are true or false.  Indicates whether the basket items should have a link to the product page or not.</param>
<param name="ShowShipTo" default="false">Possible values are true or false.  Indicates whether the shipping address should be displayed with each item or not.</param>
<param name="ShowAssets" default="false">Possible values are true or false.  Indicates whether the assets like readme files and license agreements will be shown or not.</param>
</conlib>
--%>
<asp:HyperLink ID="ProductLink" runat="Server" SkinID="OrderProductLink" NavigateUrl="" Target="_blank" />
<asp:Localize ID="ProductName" runat="server" Text="" />
<asp:Localize ID="KitMemberLabel" runat="Server" Text="<br />in {0}" Visible="false"></asp:Localize>
<asp:DataList ID="InputList" runat="server">
    <ItemTemplate>
        <asp:Localize ID="InputName" Runat="server" Text='<%#Eval("Name", "{0}:")%>' SkinID="FieldHeader"></asp:Localize>
        <asp:Localize ID="InputValue" Runat="server" Text='<%#Eval("InputValue")%>'></asp:Localize>
    </ItemTemplate>
    <FooterTemplate>
        <br />
    </FooterTemplate>
</asp:DataList>
<asp:PlaceHolder ID="KitProductPanel" runat="server">
    <ul class="BasketSubItemLabel">
    <asp:Repeater ID="KitProductRepeater" runat="server">
        <ItemTemplate>
            <li ><asp:Literal ID="KitProductLabel" runat="server" Text='<%#Eval("Name")%>' /></li>
        </ItemTemplate>
    </asp:Repeater>
    </ul>
</asp:PlaceHolder>
<asp:Label ID="WishlistLabel" runat="Server" Text="from {0}&#39;s Wish List<br />" SkinID="NoteText"></asp:Label>
<asp:Panel ID="ShipsToPanel" runat="server">
    <asp:Label ID="ShipsToLabel" Runat="server" Text="Shipping to:" SkinID="fieldheader"></asp:Label>
    <asp:Label ID="ShipsTo" Runat="server" Text="" SkinID="NoteText"></asp:Label>
</asp:Panel>
<asp:Panel ID="GiftWrapPanel" runat="server">
    <asp:Label ID="GiftWrapLabel" Runat="server" Text="Gift Wrap:" SkinID="fieldheader"></asp:Label>
    <asp:Label ID="GiftWrap" Runat="server" Text="" SkinID="NoteText"></asp:Label>
    <!-- <asp:Label ID="GiftWrapPrice" Runat="server" Text="" SkinID="NoteText"></asp:Label> -->
</asp:Panel>
<asp:Panel ID="GiftMessagePanel" runat="server">
    <asp:Label ID="GiftMessageLabel" Runat="server" Text="Gift Message:" SkinID="fieldheader"></asp:Label>
    <asp:Label ID="GiftMessage" Runat="server" Text="" SkinID="notetext"></asp:Label>
</asp:Panel>
<asp:Panel ID="AssetsPanel" runat="server">
    <ul>
    <asp:Repeater ID="AssetLinkList" runat="server">
        <ItemTemplate>
            <li><asp:HyperLink ID="AssetLink" runat="server" NavigateUrl='<%#Eval("NavigateUrl")%>' Text='<%#Eval("Text")%>'></asp:HyperLink></li>
        </ItemTemplate>
    </asp:Repeater>
    </ul>
</asp:Panel>