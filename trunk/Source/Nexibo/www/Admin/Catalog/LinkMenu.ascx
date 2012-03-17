<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LinkMenu.ascx.cs" Inherits="Admin_Catalog_LinkMenu" %>
<asp:Panel ID="WebPageMenuPanel" runat="server">
    <asp:HyperLink ID="LinkDetails" runat="server" NavigateUrl="~/Admin/Catalog/EditLink.aspx" Text="Link Details" CssClass="contextMenuButton"></asp:HyperLink>
    <asp:HyperLink ID="ManageCategories" runat="server" NavigateUrl="~/Admin/Catalog/EditLinkCategories.aspx" Text="Categories" CssClass="contextMenuButton"></asp:HyperLink>
	<asp:LinkButton ID="DeleteLink" runat="server" Text="Delete Link" OnClick="DeleteLink_Click" CssClass="contextMenuButton" ></asp:LinkButton>
    <asp:HyperLink ID="Preview" runat="server" NavigateUrl="" Text="Preview!" CssClass="contextMenuButton" Target="_blank"></asp:HyperLink>
</asp:Panel>
