<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProductMenu.ascx.cs" Inherits="Admin_Products_ProductMenu" EnableViewState="false"%>
<asp:Panel ID="ProductMenuPanel" runat="server">
    <asp:HyperLink ID="ProductDetails" runat="server" NavigateUrl="EditProduct.aspx" Text="Product Details" CssClass="contextMenuButton"></asp:HyperLink>
    <asp:HyperLink ID="Assets" runat="server" NavigateUrl="Assets/Images.aspx" Text="Images and Assets" CssClass="contextMenuButton"></asp:HyperLink>
    <asp:HyperLink ID="Variants" runat="server" NavigateUrl="Variants/Options.aspx" Text="Options and Variants" CssClass="contextMenuButton"></asp:HyperLink>
    <asp:HyperLink ID="DigitalGoods" runat="server" NavigateUrl="DigitalGoods/DigitalGoods.aspx" Text="Digital Goods" CssClass="contextMenuButton"></asp:HyperLink>
    <asp:HyperLink ID="Kitting" runat="server" NavigateUrl="Kits/EditKit.aspx" Text="Kits / Bundles" CssClass="contextMenuButton"></asp:HyperLink>
    <asp:HyperLink ID="Discounts" runat="server" NavigateUrl="ProductDiscounts.aspx" Text="Volume Discounts" CssClass="contextMenuButton"></asp:HyperLink>
    <asp:HyperLink ID="PricingRules" runat="server" NavigateUrl="Specials/Default.aspx" Text="Pricing Rules" CssClass="contextMenuButton"></asp:HyperLink>
    <asp:HyperLink ID="SimilarProducts" runat="server" NavigateUrl="EditSimilarProducts.aspx" Text="Similar Products" CssClass="contextMenuButton"></asp:HyperLink>
    <asp:HyperLink ID="ProductAccessories" runat="server" NavigateUrl="EditProductAccessories.aspx" Text="Product Accessories" CssClass="contextMenuButton"></asp:HyperLink>
    <asp:HyperLink ID="Categories" runat="server" NavigateUrl="EditProductCategories.aspx" Text="Categories" CssClass="contextMenuButton"></asp:HyperLink>
    <asp:HyperLink ID="ProductTemplate" runat="server" NavigateUrl="EditProductTemplate.aspx" Text="Product Template" CssClass="contextMenuButton"></asp:HyperLink>
    <asp:HyperLink ID="Subscriptions" runat="server" NavigateUrl="EditSubscription.aspx" Text="Subscriptions" CssClass="contextMenuButton"></asp:HyperLink>
    <asp:LinkButton ID="DeleteProduct" runat="server" Text="Delete Product" OnClick="DeleteProduct_Click" CssClass="contextMenuButton" ></asp:LinkButton>
    <asp:HyperLink ID="Preview" runat="server" NavigateUrl="" Text="Preview!" CssClass="contextMenuButton" Target="_blank"></asp:HyperLink>
</asp:Panel>
