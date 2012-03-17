<%@ Page Language="C#" MasterPageFile="../Order.master" CodeFile="EditPayment.aspx.cs" Inherits="Admin_Orders_Payments_EditPayment" Title="Edit Payment" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<div class="pageHeader">
    <div class="caption"><h1><asp:Localize ID="Caption" runat="server" Text="Edit Payment {0} {1}"></asp:Localize></h1></div>
</div>
<table class="inputForm">
    <tr>
        <th class="rowHeader">
            <asp:Label ID="CurrentPaymentStatusLabel" runat="server" Text="Status: "></asp:Label>
        </th>
        <td>
            <asp:DropDownList ID="CurrentPaymentStatus" runat="server"></asp:DropDownList>
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <asp:Label ID="PaymentDateLabel" runat="server" Text="Date: "></asp:Label>
        </th>
        <td>
            <asp:Label ID="PaymentDate" runat="server" Text=""></asp:Label>
        </td>
    </tr>                
    <tr>
        <th class="rowHeader">
            <asp:Label ID="AmountLabel" runat="server" Text="Amount: "></asp:Label>
        </th>
        <td>
            <asp:Label ID="Amount" runat="server" Text=""></asp:Label>
        </td>
    </tr>                
    <tr>
        <th class="rowHeader">
            <asp:Label ID="PaymentMethodLabel" runat="server" Text="Method: "></asp:Label>
        </th>
        <td>
            <asp:Label ID="PaymentMethod" runat="server" Text=""></asp:Label>
        </td>
   </tr>
</table>
<asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />     
<asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
</asp:Content>

