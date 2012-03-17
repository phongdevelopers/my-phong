<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GiftCertificatePaymentForm.ascx.cs" Inherits="Admin_Orders_Create_GiftCertificatePaymentForm" %>
<table class="paymentForm">
    <tr>
        <th class="caption" colspan="2">
            <asp:Label ID="Caption" runat="server" Text="Pay With Gift Certificate"></asp:Label>
        </th>
    </tr>
    <tr>
        <td class="pFcontent" colspan="2">
            <asp:Label ID="GiftCertificateHelpText" runat="server" Text="Enter the gift certificate number below."></asp:Label>
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <asp:Label ID="GiftCertificateNumberLabel" runat="server" Text="Gift Certificate #:" AssociatedControlID="GiftCertificateNumber"></asp:Label>
        </th>
        <td>
            <asp:TextBox ID="GiftCertificateNumber" runat="server" MaxLength="50" ValidationGroup="GiftCertificate"></asp:TextBox>
            <asp:RequiredFieldValidator ID="GiftCertificateNumberRequired" runat="server" 
                ErrorMessage="You must enter the gift certificate number." 
                ControlToValidate="GiftCertificateNumber" Display="Static" Text="*" ValidationGroup="GiftCertificate"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <td class="submit" colspan="2">
            <asp:ValidationSummary runat="server" ID="ValidationSummary1" ValidationGroup="GiftCertificate" />
            <asp:Button ID="GiftCertificateButton" runat="server" Text="Pay With Gift Certificate" ValidationGroup="GiftCertificate" OnClick="GiftCertificateButton_Click" />
            <asp:Panel runat="server" Visible="false" ID="GiftCertErrorsPanel">                
                <asp:Label SkinID="ErrorCondition" ID="GiftCertPaymentErrors" runat="server" Text=""></asp:Label>
            </asp:Panel>            
        </td>
    </tr>
</table>