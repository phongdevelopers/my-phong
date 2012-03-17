<%@ Page Language="C#" MasterPageFile="~/Admin/Catalog/Webpage-Link.master" CodeFile="EditLinkCategories.aspx.cs" Inherits="Admin_Catalog_EditLinkCategories" Title="Edit Link Categories" %>
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls"
    TagPrefix="cb" %>
<%@ Register Src="~/Admin/Catalog/LinkMenu.ascx" TagName="LinkMenu" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PrimarySidebarContent" Runat="Server">
<uc:LinkMenu ID="LinkMenu1" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
        <div class="caption">
            <h1><asp:Localize ID="Caption" runat="server" Text="Categories for {0}"></asp:Localize></h1>
        </div>
    </div>
    <asp:Label ID="SuccessMessage" runat="server" Text="" SkinID="GoodCondition" Visible="false"></asp:Label>
    <asp:Label ID="FailureMessage" runat="server" Text="" SkinID="ErrorCondition" Visible="false"></asp:Label>
    <div style="clear: both;margin-top:6px;">
        <ComponentArt:TreeView id="CategoryTree" runat="server" EnableViewState="true">
        </ComponentArt:TreeView>
    </div>
    <br /><asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
</asp:Content>
