<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MailPaymentForm.ascx.cs" Inherits="Admin_Orders_Create_MailPaymentForm" %>
<table class="paymentForm">
    <tr>
        <th class="caption">
            <asp:Label ID="Caption" runat="server" Text="Pay by {0}"></asp:Label>
        </th>
    </tr>
    <tr>
        <td class="content">
		    <p align="justify"><asp:Label ID="MailHelpText" runat="server" Text="Click below to submit your order.  You can then print an invoice and send in your payment."></asp:Label></p>
        </td>
    </tr>
    <tr>
        <td class="submit">
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="MailFax" Visible="false" />
            <asp:Button ID="MailButton" runat="server" Text="Pay by {0}" OnClick="MailButton_Click" ValidationGroup="MailFax" />
        </td>
    </tr>
</table>