<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PayPalPaymentForm.ascx.cs" Inherits="Admin_Orders_Create_PayPalPaymentForm" %>
<table class="paymentForm">
    <tr>
        <th class="caption">
            <asp:Label ID="Caption" runat="server" Text="Pay with {0}"></asp:Label>
        </th>
    </tr>
    <tr>
        <td class="pFcontent">
    	    <p align="justify"><asp:Label ID="PayPalHelpText" runat="server" Text="Click below to finalize your order.  From the invoice page, you can continue on to PayPal to pay the order balance."></asp:Label></p>
        </td>
    </tr>
    <tr>
        <td class="submit">
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="PayPal" Visible="false" />
            <asp:ImageButton ID="PayPalButton" runat="server" OnClick="PayPalButton_Click" ValidationGroup="PayPal"/>
            <asp:HiddenField runat="server"  ID="FormIsSubmitted" value="0" />
        </td>
    </tr>
</table>