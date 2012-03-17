<%@ Page Language="C#" MasterPageFile="~/Admin/Orders/Order.master" CodeFile="FindProduct.aspx.cs" Inherits="Admin_Orders_Edit_FindProduct" Title="Add Product" %>
<%@ Register Src="FindProduct.ascx" TagName="FindProduct" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
        <div class="caption"><h1><asp:Localize ID="Caption" runat="server" Text="Add Product to Order #{0}"></asp:Localize></h1></div>
    </div>
    <uc1:FindProduct ID="FindProduct1" runat="server" />    
</asp:Content>

