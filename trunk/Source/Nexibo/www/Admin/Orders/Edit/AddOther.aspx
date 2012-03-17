<%@ Page Language="C#" MasterPageFile="~/Admin/Orders/Order.master" CodeFile="AddOther.aspx.cs" Inherits="Admin_Orders_Edit_AddOther" Title="Add Item to Order" %>
<%@ Register Src="AddOther.ascx" TagName="AddOther" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<div class="pageHeader">
    <div class="caption"><h1><asp:Localize ID="Caption" runat="server" Text="Add Item to Order #{0}"></asp:Localize></h1></div>
</div>
<uc1:AddOther id="AddOther1" runat="server"></uc1:AddOther>
</asp:Content>

