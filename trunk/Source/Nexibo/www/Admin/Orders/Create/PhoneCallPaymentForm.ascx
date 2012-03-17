<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PhoneCallPaymentForm.ascx.cs" Inherits="Admin_Orders_Create_PhoneCallPaymentForm" %>
<table class="paymentForm">
    <tr>
        <th class="caption">
            <asp:Label ID="Caption" runat="server" Text="Pay by {0}"></asp:Label>
        </th>
    </tr>
    <tr>
        <td class="content">
		    <p align="justify"><asp:Label ID="PhoneCallHelpText" runat="server" Text="Click below to submit your order for payment by Phone."></asp:Label></p>
        </td>
    </tr>
    <tr>
        <td class="submit">
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="Phone" Visible="false" />
            <asp:Button ID="PhoneCallButton" runat="server" Text="Pay by {0}" OnClick="PhoneCallButton_Click" ValidationGroup="Phone" />
        </td>
    </tr>
</table>