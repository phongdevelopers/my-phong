<%@ Page Language="C#" MasterPageFile="~/Admin/Orders/Order.master" CodeFile="AddProduct.aspx.cs" Inherits="Admin_Orders_Shipments_AddProduct" Title="Add Product" EnableViewState="false" %>
<%@ Register Src="~/Admin/Orders/Edit/AddProductDialog.ascx" TagName="AddProductDialog" TagPrefix="uc1" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
        <div class="caption"><h1><asp:Localize ID="Caption" runat="server" Text="Add Product to Shipment #{0} of Order #{1}"></asp:Localize></h1></div>
    </div>
    <uc1:AddProductDialog ID="AddProductDialog1" runat="server" />
</asp:Content>