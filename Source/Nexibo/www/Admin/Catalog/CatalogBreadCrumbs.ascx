<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CatalogBreadCrumbs.ascx.cs" Inherits="Admin_Catalog_CatalogBreadCrumbs" EnableViewState="false"  %>
<div class="breadCrumbsPanel">
<asp:HyperLink ID="AdministrationLink" runat="server" NavigateUrl="~/Admin/Default.aspx" Text="Administration"></asp:HyperLink>
&nbsp;&gt;&nbsp;
<asp:HyperLink ID="CatalogLink" runat="server" NavigateUrl="~/Admin/Catalog/Browse.aspx" Text="Catalog"></asp:HyperLink>
<asp:Repeater ID="Repeater1" runat="server">
    <HeaderTemplate>&nbsp;&gt;&nbsp;</HeaderTemplate>
    <ItemTemplate>
        <asp:HyperLink ID="BreadCrumbsLink" runat="server" NavigateUrl='<%# Eval("CatalogNodeId", "~/Admin/Catalog/Browse.aspx?CategoryId={0}") %>' Text='<%#Eval("Name")%>'></asp:HyperLink>
    </ItemTemplate>
    <SeparatorTemplate>&nbsp;&gt;&nbsp;</SeparatorTemplate>
</asp:Repeater>
</div>