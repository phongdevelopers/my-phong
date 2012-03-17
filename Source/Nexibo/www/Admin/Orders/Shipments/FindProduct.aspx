<%@ Page Language="C#" MasterPageFile="~/Admin/Orders/Order.master" CodeFile="FindProduct.aspx.cs" Inherits="Admin_Orders_Shipments_FindProduct" Title="Add Product" %>

<%@ Register Src="~/Admin/Orders/Edit/FindProduct.ascx" TagName="FindProduct" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
        <div class="caption"><h1><asp:Localize ID="Caption" runat="server" Text="Add Product to Shipment #{0} of Order #{1}"></asp:Localize></h1>
        </div>
    </div>
    <uc1:FindProduct ID="FindProduct1" runat="server" />
</asp:Content>

