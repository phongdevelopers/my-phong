<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CatalogToolbar.ascx.cs" Inherits="Admin_Catalog_CatalogToolbar" %>
<table class="catalogToolbar" cellspacing="0" cellpadding="0" width="100%">
    <tr>
        <td>
            <asp:Label ID="CaptionLabel" runat="server" Text="" SkinID="FieldHeader"></asp:Label>
            <asp:HyperLink ID="EditLink" runat="server" NavigateUrl="~/Admin/Catalog/EditCategory.aspx" Text="[Edit]">
            </asp:HyperLink>
            <asp:HyperLink ID="PreviewLink" runat="server" NavigateUrl="" Text="[View]" Target="_blank">
            </asp:HyperLink>
        </td>
        <td align="Right" valign="top">
			<asp:TextBox runat="server" ID="SearchPhrase" Columns="10"></asp:TextBox>
			<asp:ImageButton ID="SearchButton" runat="server" AlternateText="Search" SkinID="SearchIcon" OnClick="SearchButton_Click"  />
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:Label ID="AddLabel2" runat="server" text="New:"></asp:Label>
            &nbsp;<asp:HyperLink ID="AddCategory" runat="server" NavigateUrl="~/Admin/Catalog/AddCategory.aspx" Text="Subcategory"></asp:HyperLink>
            &nbsp;|&nbsp;<asp:HyperLink ID="AddProduct" runat="server" NavigateUrl="~/Admin/Products/AddProduct.aspx" Text="Product"></asp:HyperLink>
            &nbsp;|&nbsp;<asp:HyperLink ID="AddWebpage" runat="server" NavigateUrl="~/Admin/Catalog/AddWebpage.aspx" Text="Webpage"></asp:HyperLink>
            &nbsp;|&nbsp;<asp:HyperLink ID="AddLink" runat="server" NavigateUrl="~/Admin/Catalog/AddLink.aspx" Text="Link"></asp:HyperLink>
        </td>
    </tr>
</table>
