<%@ Page Language="C#" MasterPageFile="~/Admin/Orders/Order.master" CodeFile="RefundPayment.aspx.cs" Inherits="Admin_Orders_Payments_RefundPayment" Title="Refund Payment" %>
<asp:Content ID="Content2" ContentPlaceHolderID="PrimarySidebarContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
        <div class="caption"><h1><asp:Localize ID="Caption" runat="server" Text="Refund Payment for Payment #{0}: {1}"></asp:Localize></h1></div>
    </div>
    <div style="clear: both;">
    <table cellpadding="0" cellspacing="5" class="inputForm">
        <tr>
            <td colspan="2">
                Enter the refund amount you wish to send the customer and click <b>Refund</b>.
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
            </td>
        </tr>
        <tr>
            <th class="rowHeader">
                <asp:Label ID="PaymentMethodLabel" runat="server" Text="Method:"></asp:Label>
            </th>
            <td>
                <asp:Label ID="PaymentReference" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <th class="rowHeader">
                <asp:Label ID="PaymentAmountLabel" runat="server" Text="Payment Amount:"></asp:Label>
            </th>
            <td>
                <asp:Label ID="PaymentAmount" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <th class="rowHeader">
                <asp:Label ID="RefundAmountLabel" runat="server" Text="Refund Amount:"></asp:Label>
            </th>
            <td>
                <asp:TextBox ID="RefundAmount" runat="server" Columns="6"></asp:TextBox>
            </td>
        </tr>
        <asp:PlaceHolder ID="CreditCardFields" runat="server">
        <tr>
            <th class="rowHeader">
                <asp:Label ID="CreditCardNumberLabel" runat="server" Text="Card Number:" AssociatedControlID="CreditCardNumber"></asp:Label>
            </th>
            <td>
                <asp:TextBox ID="CreditCardNumber" runat="server" width="150px" MaxLength="19"></asp:TextBox>
                <asp:RequiredFieldValidator ID="CredtCardNumberRequired" runat="server" ErrorMessage="You must enter the credit card number." 
                    ControlToValidate="CreditCardNumber" Display="Dynamic" Text="*"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="CreditCardNumberValidator" runat="server" ErrorMessage="You must enter a credit card number between 13 and 19 digits long." 
                    ControlToValidate="CreditCardNumber" Display="Dynamic" Text="*" ValidationExpression="^\d{13,19}$"></asp:RegularExpressionValidator>
                <asp:Literal ID="CreditCardNumberLiteral" runat="server"></asp:Literal>
            </td>
        </tr>
        <tr id="trCreditCardExpiration" runat="server">
            <th class="rowHeader">
                <asp:Label ID="CreditCardExpirationLabel" runat="server" Text="Expiration:" AssociatedControlID="CreditCardExpirationMonth"></asp:Label>
            </th>
            <td>
                <asp:DropDownList ID="CreditCardExpirationMonth" runat="server">
                    <asp:ListItem Text="month" Value=""></asp:ListItem>
                    <asp:ListItem Text="01 (Jan)" Value="1"></asp:ListItem>
                    <asp:ListItem Text="02 (Feb)" Value="2"></asp:ListItem>
                    <asp:ListItem Text="03 (Mar)" Value="3"></asp:ListItem>
                    <asp:ListItem Text="04 (Apr)" Value="4"></asp:ListItem>
                    <asp:ListItem Text="05 (May)" Value="5"></asp:ListItem>
                    <asp:ListItem Text="06 (Jun)" Value="6"></asp:ListItem>
                    <asp:ListItem Text="07 (Jul)" Value="7"></asp:ListItem>
                    <asp:ListItem Text="08 (Aug)" Value="8"></asp:ListItem>
                    <asp:ListItem Text="09 (Sep)" Value="9"></asp:ListItem>
                    <asp:ListItem Text="10 (Oct)" Value="10"></asp:ListItem>
                    <asp:ListItem Text="11 (Nov)" Value="11"></asp:ListItem>
                    <asp:ListItem Text="12 (Dec)" Value="12"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="CreditCardExpirationMonthRequired" runat="server" ErrorMessage="You must select the expiration month." 
                    ControlToValidate="CreditCardExpirationMonth" Display="Dynamic" Text="*"></asp:RequiredFieldValidator>
                <asp:DropDownList ID="CreditCardExpirationYear" runat="server">
                    <asp:ListItem Text="year" Value=""></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="CreditCardExpirationYearRequired" runat="server" ErrorMessage="You must select the expiration year." 
                    ControlToValidate="CreditCardExpirationYear" Display="Dynamic" Text="*"></asp:RequiredFieldValidator>
                <asp:Literal ID="CreditCardExpirationLiteral" runat="server"></asp:Literal>
            </td>
        </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="DebitCardFields" runat="server">
        <tr id="trDebitCardIssueNumber" runat="server">
            <th class="rowHeader">
                <asp:Label ID="DebitCardIssueNumberLabel" runat="server" Text="Issue Number:" AssociatedControlID="DebitCardIssueNumber"></asp:Label>
            </th>
            <td>
                <asp:TextBox ID="DebitCardIssueNumber" runat="server" width="40px" MaxLength="2"></asp:TextBox>
                <asp:Literal ID="DebitCardIssueNumberLiteral" runat="server"></asp:Literal>
            </td>
        </tr>
        <tr id="trDebitCardStartDate" runat="server">
            <th class="rowHeader">
                <asp:Label ID="DebitCardStartDateLabel" runat="server" Text="Start Date:" AssociatedControlID="DebitCardStartMonth"></asp:Label>
            </th>
            <td>
                <asp:DropDownList ID="DebitCardStartMonth" runat="server">
                    <asp:ListItem Text="month" Value=""></asp:ListItem>
                    <asp:ListItem Text="01 (Jan)" Value="1"></asp:ListItem>
                    <asp:ListItem Text="02 (Feb)" Value="2"></asp:ListItem>
                    <asp:ListItem Text="03 (Mar)" Value="3"></asp:ListItem>
                    <asp:ListItem Text="04 (Apr)" Value="4"></asp:ListItem>
                    <asp:ListItem Text="05 (May)" Value="5"></asp:ListItem>
                    <asp:ListItem Text="06 (Jun)" Value="6"></asp:ListItem>
                    <asp:ListItem Text="07 (Jul)" Value="7"></asp:ListItem>
                    <asp:ListItem Text="08 (Aug)" Value="8"></asp:ListItem>
                    <asp:ListItem Text="09 (Sep)" Value="9"></asp:ListItem>
                    <asp:ListItem Text="10 (Oct)" Value="10"></asp:ListItem>
                    <asp:ListItem Text="11 (Nov)" Value="11"></asp:ListItem>
                    <asp:ListItem Text="12 (Dec)" Value="12"></asp:ListItem>
                </asp:DropDownList>
                <asp:DropDownList ID="DebitCardStartYear" runat="server">
                    <asp:ListItem Text="year" Value=""></asp:ListItem>
                </asp:DropDownList>
                <asp:Literal ID="DebitCardStartDateLiteral" runat="server"></asp:Literal>
            </td>
        </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="CheckFields" runat="server" Visible="false">
        <tr>
            <th class="rowHeader" valign="top">
                <asp:Label ID="CheckBankNameLabel" runat="server" Text="Bank Name:" AssociatedControlID="CheckBankName"></asp:Label>
            </th>
            <td>
                <asp:TextBox ID="CheckBankName" runat="server" Width="100px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="CheckBankNameRequired" runat="server" ErrorMessage="You must enter the bank name." 
                    ControlToValidate="CheckBankName" Display="Dynamic" Text="*"></asp:RequiredFieldValidator>
                <asp:Literal ID="CheckBankNameLiteral" runat="server"></asp:Literal>
            </td>
        </tr>
        <tr>
            <th class="rowHeader" valign="top">
                <asp:Label ID="CheckRoutingNumberLabel" runat="server" Text="Routing Number:" AssociatedControlID="CheckRoutingNumber"></asp:Label>
            </th>
            <td>
                <asp:TextBox ID="CheckRoutingNumber" runat="server" Width="100px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="CheckRoutingNumberRequired" runat="server" ErrorMessage="You must enter a valid routing number." 
                    ControlToValidate="CheckRoutingNumber" Display="Dynamic" Text="*"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="CheckRoutingNumberValidator" runat="server" ErrorMessage="The routing number can only contain the digits 0-9." 
                    ControlToValidate="CheckRoutingNumber" Display="Dynamic" Text="*" ValidationExpression="^\d+$"></asp:RegularExpressionValidator>
                <asp:Literal ID="CheckRoutingNumberLiteral" runat="server"></asp:Literal>
            </td>
        </tr>
        <tr>
            <th class="rowHeader" valign="top">
                <asp:Label ID="CheckAccountHolderNameLabel" runat="server" Text="Account Holder Name:" AssociatedControlID="CheckAccountHolderName"></asp:Label>
            </th>
            <td>
                <asp:TextBox ID="CheckAccountHolderName" runat="server" Width="100px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="CheckAccountHolderRequired" runat="server" ErrorMessage="You must enter the account holder name." 
                    ControlToValidate="CheckAccountHolderName" Display="Dynamic" Text="*"></asp:RequiredFieldValidator>
                <asp:Literal ID="CheckAccountHolderNameLiteral" runat="server"></asp:Literal>
            </td>
        </tr>
        <tr>
            <th class="rowHeader" valign="top">
                <asp:Label ID="CheckAccountNumberLabel" runat="server" Text="Account Number:" AssociatedControlID="CheckAccountNumber"></asp:Label>
            </th>
            <td>
                <asp:TextBox ID="CheckAccountNumber" runat="server" Width="100px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="CheckAccountNumberRequired" runat="server" ErrorMessage="You must enter the account number." 
                    ControlToValidate="CheckAccountNumber" Display="Dynamic" Text="*"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="CheckAccountNumberValidator" runat="server" ErrorMessage="The account number can only contain the digits 0-9." 
                    ControlToValidate="CheckRoutingNumber" Display="Dynamic" Text="*" ValidationExpression="^\d+$"></asp:RegularExpressionValidator>
                <asp:Literal ID="CheckAccountNumberLiteral" runat="server"></asp:Literal>
            </td>
        </tr>
        <tr>
            <th class="rowHeader" valign="top">
                <asp:Label ID="CheckAccountTypeLabel" runat="server" Text="Account Type:" AssociatedControlID="CheckAccountType"></asp:Label>
            </th>
            <td>
                <asp:DropDownList ID="CheckAccountType" runat="server">
                    <asp:ListItem Text="Personal Checking" Value="C"></asp:ListItem>
                    <asp:ListItem Text="Personal Savings" Value="S"></asp:ListItem>
                    <asp:ListItem Text="Business Checking" Value="B"></asp:ListItem>
                </asp:DropDownList>
                <asp:Literal ID="CheckAccountTypeLiteral" runat="server"></asp:Literal>
            </td>
        </tr>
        </asp:PlaceHolder>
        <tr>
            <th class="rowHeader" valign="top">
                <asp:Label ID="CustomerNoteLabel" runat="server" Text="Note to Customer:"></asp:Label><br />
                <span style="font-weight:normal;font-style:italic">(Optional)</span>
            </th>
            <td>
                <asp:TextBox ID="CustomerNote" runat="server" TextMode="MultiLine" Rows="6" Columns="50"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>
                <asp:Button ID="SubmitRefundButton" runat="server" Text="Refund" OnClick="SubmitRefundButton_Click"></asp:Button>
                <asp:Button ID="CancelRefundButton" runat="server" Text="Cancel" OnClick="CancelRefundButton_Click" CausesValidation="false"></asp:Button>
            </td>
        </tr>
    </table>
    </div>
</asp:Content>

