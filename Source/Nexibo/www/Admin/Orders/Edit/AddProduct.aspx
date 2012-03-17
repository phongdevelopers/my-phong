<%@ Page Language="C#" MasterPageFile="~/Admin/Orders/Order.master" CodeFile="AddProduct.aspx.cs" Inherits="Admin_Orders_Edit_AddProduct" Title="Add Product" EnableViewState="false" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Src="AddProductDialog.ascx" TagName="AddProductDialog" TagPrefix="uc1" %>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<div class="pageHeader">
    <div class="caption"><h1><asp:Localize ID="Caption" runat="server" Text="Add Product to Order #{0}"></asp:Localize></h1></div>
</div>
<uc1:AddProductDialog ID="AddProductDialog1" runat="server" />
</asp:Content>
