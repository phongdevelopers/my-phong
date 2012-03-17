<%@ Page Language="C#" MasterPageFile="Product.master" CodeFile="EditProductCategories.aspx.cs" Inherits="Admin_Products_EditProductCategories" Title="Edit Product Categories"  %>
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>

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

