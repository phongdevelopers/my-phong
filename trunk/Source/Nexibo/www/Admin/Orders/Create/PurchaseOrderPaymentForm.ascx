<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PurchaseOrderPaymentForm.ascx.cs" Inherits="Admin_Orders_Create_PurchaseOrderPaymentForm" %>
<table class="paymentForm">
    <tr>
        <th class="caption" colspan="2">
            <asp:Label ID="Caption" runat="server" Text="Pay by {0}"></asp:Label>
        </th>
    </tr>
    <tr>
        <td class="content" colspan="2">
            <asp:Label ID="PurchaseOrderHelpText" runat="server" Text="Enter the purchase order number below."></asp:Label>
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <asp:Label ID="PurchaseOrderNumberLabel" runat="server" Text="PO #:" AssociatedControlID="PurchaseOrderNumber"></asp:Label>
        </th>
        <td>
            <asp:TextBox ID="PurchaseOrderNumber" runat="server" MaxLength="50" ValidationGroup="PurchaseOrder"></asp:TextBox>
            <asp:RequiredFieldValidator ID="PurchaseOrderNumberRequired" runat="server" 
                ErrorMessage="You must enter the purchase order number." 
                ControlToValidate="PurchaseOrderNumber" Display="Static" Text="*" ValidationGroup="PurchaseOrder"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <td class="submit" colspan="2">
            <asp:ValidationSummary runat="server" ID="ValidationSummary1" ValidationGroup="PurchaseOrder" />
            <asp:Button ID="PurchaseOrderButton" runat="server" Text="Pay by {0}" ValidationGroup="PurchaseOrder" OnClick="PurchaseOrderButton_Click" />
        </td>
    </tr>
</table>