<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CheckPaymentForm.ascx.cs" Inherits="Admin_Orders_Create_CheckPaymentForm" EnableViewState="false" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<table class="paymentForm">
    <tr>
        <th class="caption" colspan="3">
            <asp:Label ID="Caption" runat="server" Text="Pay by {0}"></asp:Label>
        </th>
    </tr>
    <tr>
        <td class="content" colspan="3">
            <asp:Label ID="CheckHelpText" runat="server" Text="Enter your checking account information below."></asp:Label>
        </td>
    </tr>
    <tr id="trAmount" runat="server">
        <th class="rowHeader">
            <asp:Label ID="AmountLabel" runat="server" Text="Amount:"></asp:Label>
        </th>
        <td>
            <asp:TextBox ID="Amount" runat="server" Text="" Width="60px" MaxLength="10" ValidationGroup="Check"></asp:TextBox>
            <asp:RequiredFieldValidator ID="AmountRequired" runat="server" Text="*"
                ErrorMessage="Amount is required." Display="Static" ControlToValidate="Amount"
                ValidationGroup="Check"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <asp:Label ID="AccountHolderLabel" runat="server" Text="Account Holder:" AssociatedControlID="AccountHolder"></asp:Label>
	    </th>
        <td>
            <asp:TextBox id="AccountHolder" runat="server" MaxLength="50" ValidationGroup="Check"></asp:TextBox>
            <asp:RequiredFieldValidator ID="AccountHolderValidator" runat="server" ErrorMessage="You must enter the account holder name." 
                ControlToValidate="AccountHolder" Display="Static" Text="*" ValidationGroup="Check" ></asp:RequiredFieldValidator>
        </td>
        <td rowspan="5">
            <asp:Image ID="CheckHelpImage" runat="server" ImageUrl="~/images/PaymentInstruments/checkhelp.jpg" Width="250" />
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <asp:Label ID="BankNameLabel" runat="server" Text="Bank Name:" AssociatedControlID="BankName"></asp:Label>
	    </th>
        <td>
            <asp:TextBox id="BankName" runat="server" MaxLength="50" ValidationGroup="Check"></asp:TextBox>
            <asp:RequiredFieldValidator ID="BankNameRequiredValidator" runat="server" ErrorMessage="You must enter the bank name." 
                ControlToValidate="BankName" Display="Static" Text="*" ValidationGroup="Check" ></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr id="trRoutingNumber" runat="server">
        <th class="rowHeader">
            <asp:Label ID="RoutingNumberLabel" runat="server" Text="Routing Number:" AssociatedControlID="RoutingNumber"></asp:Label>
	    </th>
        <td>
            <asp:TextBox id="RoutingNumber" runat="server" MaxLength="9" ValidationGroup="Check"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RoutingNumberValidator2" runat="server" ErrorMessage="You must enter a valid routing number." 
                ControlToValidate="RoutingNumber" Display="Static" Text="*" ValidationGroup="Check" ></asp:RequiredFieldValidator>
            <cb:RoutingNumberValidator ID="RoutingNumberValidator" runat="server" ErrorMessage="You must enter a valid routing number."
                ControlToValidate="RoutingNumber" Display="Static" Text="*" ValidationGroup="Check" ></cb:RoutingNumberValidator>
        </td>
    </tr>
    <tr id="trSortCode" runat="server">
        <th class="rowHeader">
            <asp:Label ID="SortCodeLabel" runat="server" Text="Sort Code:" AssociatedControlID="SortCode"></asp:Label>
	    </th>
        <td>
            <asp:TextBox id="SortCode" runat="server" MaxLength="20" ValidationGroup="Check"></asp:TextBox>
            <asp:RequiredFieldValidator ID="SortCodeValidator" runat="server" ErrorMessage="You must enter a bank sort code or transit/routing number."
                ControlToValidate="SortCode" Display="Static" Text="*" ValidationGroup="Check"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <asp:Label ID="AccountNumberLabel" runat="server" Text="Account Number:" AssociatedControlID="AccountNumber"></asp:Label>
	    </th>
        <td>
            <asp:TextBox id="AccountNumber" runat="server" ValidationGroup="Check"></asp:TextBox>
            <asp:RequiredFieldValidator ID="AccountNumberValidator" runat="server" ErrorMessage="You must enter the account number." 
                ControlToValidate="AccountNumber" Display="Static" Text="*" ValidationGroup="Check" ></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <td class="submit" colspan="2">
            <asp:ValidationSummary runat="server" ID="ValidationSummary1" ValidationGroup="Check" />
            <asp:Button ID="CheckButton" runat="server" Text="Pay by {0}" ValidationGroup="Check" OnClick="CheckButton_Click" OnClientClick="if(Page_ClientValidate()){this.value='Processing...';this.onclick='return false;'}" />
        </td>
    </tr>
</table>