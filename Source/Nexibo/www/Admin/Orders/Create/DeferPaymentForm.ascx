<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DeferPaymentForm.ascx.cs" Inherits="Admin_Orders_Create_DeferPaymentForm" %>
<table class="paymentForm">
    <tr>
        <th class="caption">
            <asp:Label ID="Caption" runat="server" Text="Defer Payment"></asp:Label>
        </th>
    </tr>
    <tr>
        <td class="content">
		    <p align="justify"><asp:Label ID="DeferHelpText" runat="server" Text="Click below to complete the order without providing payment details."></asp:Label></p>
        </td>
    </tr>
    <tr>
        <td class="submit">
            <asp:Button ID="SubmitButton" runat="server" Text="Defer Payment" OnClick="SubmitButton_Click" ValidationGroup="MailFax" />
        </td>
    </tr>
</table>