<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ucProcessPayment.ascx.cs" Inherits="Admin_Orders_Payments_ucProcessPayment" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<ajax:UpdatePanel ID="PaymentAjaxPanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:ValidationSummary ID="CreditCardValidationSummary" runat="server" ValidationGroup="CreditCard" />
        <asp:ValidationSummary ID="CheckValidationSummary" runat="server" ValidationGroup="Check" />
        <table>
            <tr>
                <th class="rowHeader">
                    <asp:Label ID="SelectedPaymentMethodLabel" runat="server" Text="Payment Method:"></asp:Label>
                </th>
                <td>
                    <asp:DropDownList ID="SelectedPaymentMethod" runat="server" DataValueField="PaymentMethodId" DataTextField="Name" 
                        AppendDataBoundItems="true" OnSelectedIndexChanged="SelectedPaymentMethod_SelectedIndexChanged" AutoPostBack="true">
                        <asp:ListItem Value="" Text=""></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
        <asp:PlaceHolder ID="CreditCardPanel" runat="server">
            <table>
                <tr>
                    <th class="rowHeader">
                        <asp:Label ID="CreditCardPaymentAmountLabel" runat="server" Text="Payment Amount:" AssociatedControlID="CreditCardPaymentAmount"></asp:Label>
	                </th>
                    <td>
                        <asp:TextBox ID="CreditCardPaymentAmount" runat="server" Columns="8" ValidationGroup="CreditCard"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="CreditCardPaymentAmountValidator" runat="server" Text="*"
                            ErrorMessage="Amount is required." Display="Static" ControlToValidate="CreditCardPaymentAmount"
                            ValidationGroup="CreditCard"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader">
                        <asp:Label ID="CardNameLabel" runat="server" Text="Name&nbsp;on&nbsp;Card:" AssociatedControlID="CardName"></asp:Label>
	                </th>
                    <td>
                        <asp:TextBox ID="CardName" runat="server" MaxLength="50" ValidationGroup="CreditCard" AutoCompleteType="Disabled"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="CardNameRequired" runat="server" 
                            ErrorMessage="You must enter the name as it appears on the card." 
                            ControlToValidate="CardName" Display="Static" Text="*" ValidationGroup="CreditCard"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader">
                        <asp:Label ID="CardNumberLabel" runat="server" Text="Card Number:" AssociatedControlID="CardNumber"></asp:Label>
	                </th>
                    <td>
                        <asp:TextBox ID="CardNumber" runat="server" MaxLength="19" ValidationGroup="CreditCard"></asp:TextBox>
                        <cb:CreditCardValidator ID="CardNumberValidator1" runat="server" 
                            ControlToValidate="CardNumber" ErrorMessage="You must enter a valid card number."
                            Display="Dynamic" Text="*" ValidationGroup="CreditCard"></cb:CreditCardValidator>
                        <cb:RequiredRegularExpressionValidator ID="CardNumberValidator2" runat="server" ValidationExpression="\d{12,19}"
                            ErrorMessage="Card number is required and should be between 12 and 19 digits (no dashes or spaces)." ControlToValidate="CardNumber"
                            Display="Static" Text="*" Required="true" ValidationGroup="CreditCard"></cb:RequiredRegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader">
                        <asp:Label ID="ExpirationLabel" runat="server" Text="Expiration:" AssociatedControlID="ExpirationMonth"></asp:Label>
	                </th>
                    <td>
                        <asp:dropdownlist id="ExpirationMonth" runat="server" ValidationGroup="CreditCard">
		                    <asp:ListItem Value="">Month</asp:ListItem>
		                    <asp:ListItem Value="01">01</asp:ListItem>
		                    <asp:ListItem Value="02">02</asp:ListItem>
		                    <asp:ListItem Value="03">03</asp:ListItem>
		                    <asp:ListItem Value="04">04</asp:ListItem>
		                    <asp:ListItem Value="05">05</asp:ListItem>
		                    <asp:ListItem Value="06">06</asp:ListItem>
		                    <asp:ListItem Value="07">07</asp:ListItem>
		                    <asp:ListItem Value="08">08</asp:ListItem>
		                    <asp:ListItem Value="09">09</asp:ListItem>
		                    <asp:ListItem Value="10">10</asp:ListItem>
		                    <asp:ListItem Value="11">11</asp:ListItem>
		                    <asp:ListItem Value="12">12</asp:ListItem>
	                    </asp:dropdownlist>
                        <asp:RequiredFieldValidator ID="MonthValidator" runat="server" ErrorMessage="You must select the expiration month." 
                            ControlToValidate="ExpirationMonth" Display="Static" Text="*" ValidationGroup="CreditCard"></asp:RequiredFieldValidator>
	                    &nbsp;/&nbsp;
	                    <asp:dropdownlist id="ExpirationYear" Runat="server" ValidationGroup="CreditCard">
		                    <asp:ListItem Value="">Year</asp:ListItem>
	                    </asp:dropdownlist>
                        <cb:expirationdropdownvalidator ID="ExpirationDropDownValidator1" runat="server"
                            Display="Dynamic" errormessage="You must enter an expiration date in the future."
                            monthcontroltovalidate="ExpirationMonth" yearcontroltovalidate="ExpirationYear"
                            Text="*" ValidationGroup="CreditCard"></cb:expirationdropdownvalidator>
                        <asp:RequiredFieldValidator ID="YearValidator" runat="server" ErrorMessage="You must select the expiration year." 
                            ControlToValidate="ExpirationYear" Display="Static" Text="*" ValidationGroup="CreditCard"></asp:RequiredFieldValidator>
	                </td>
                </tr>
                <tr>
                    <th class="rowHeader">
                        <asp:Label ID="SecurityCodeLabel" runat="server" Text="Security Code:" AssociatedControlID="SecurityCode"></asp:Label>
	                </th>
                    <td>
                        <asp:textbox id="SecurityCode" runat="server" Columns="4" ValidationGroup="CreditCard"></asp:textbox>
                        <cb:RequiredRegularExpressionValidator ID="SecurityCodeValidator" runat="server" ValidationExpression="\d{3,4}"
                            ErrorMessage="Card security code should be a 3 or 4 digit number." ControlToValidate="SecurityCode"
                            Display="Static" Text="*" Required="true" ValidationGroup="CreditCard"></cb:RequiredRegularExpressionValidator>
                        <asp:CustomValidator ID="SecurityCodeValidator2" runat="server" Text="*" 
                            ErrorMessage="Card security code should be a 3 or 4 digit number." ValidationGroup="CreditCard"></asp:CustomValidator>
                        <asp:Literal ID="IntlCVVMessage" runat="server" Text="(optional / if available)"></asp:Literal>
                    </td>
                </tr>
                <tr id="trIssueNumber" runat="server" visible="true">
                    <th class="rowHeader">
                        <asp:Label ID="IssueNumberLabel" runat="server" Text="Issue Number:" AssociatedControlID="IssueNumber" SkinID="fieldHeader"></asp:Label>
                    </th>
                    <td>
                        <asp:TextBox ID="IssueNumber" runat="server" MaxLength="2" Width="40px" ValidationGroup="CreditCard"></asp:TextBox>
                        <asp:CustomValidator ID="IntlDebitValidator1" runat="server" Text="*" ValidationGroup="CreditCard"></asp:CustomValidator>
                    </td>
                </tr>
                <tr id="trStartDate" runat="server" visible="true">
                    <td>
                        <asp:Label ID="StartDateLabel" runat="server" Text="OR Start Date: " AssociatedControlID="StartDateMonth" SkinID="fieldheader"></asp:Label>
                    </td>
                    <td>
                        <asp:dropdownlist ID="StartDateMonth" runat="server" ValidationGroup="CreditCard">
                            <asp:ListItem Text="Month" Value=""></asp:ListItem>
                            <asp:ListItem Value="01">01</asp:ListItem>
                            <asp:ListItem Value="02">02</asp:ListItem>
                            <asp:ListItem Value="03">03</asp:ListItem>
                            <asp:ListItem Value="04">04</asp:ListItem>
                            <asp:ListItem Value="05">05</asp:ListItem>
                            <asp:ListItem Value="06">06</asp:ListItem>
                            <asp:ListItem Value="07">07</asp:ListItem>
                            <asp:ListItem Value="08">08</asp:ListItem>
                            <asp:ListItem Value="09">09</asp:ListItem>
                            <asp:ListItem Value="10">10</asp:ListItem>
                            <asp:ListItem Value="11">11</asp:ListItem>
                            <asp:ListItem Value="12">12</asp:ListItem>
                        </asp:dropdownlist>
                        <asp:dropdownlist ID="StartDateYear" Runat="server" ValidationGroup="CreditCard">
                            <asp:ListItem Text="Year" Value=""></asp:ListItem>
                        </asp:dropdownlist>
                        <asp:CustomValidator ID="IntlDebitValidator2" runat="server" Text="*" ErrorMessage="Issue number or start date is required." ValidationGroup="CreditCard"></asp:CustomValidator>
                    </td>
                </tr>
            </table>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="CheckPanel" runat="server">
            <table>
                <tr>
                    <th class="rowHeader">
                        <asp:Label ID="CheckPaymentAmountLabel" runat="server" Text="Payment Amount:" AssociatedControlID="CheckPaymentAmount"></asp:Label>
	                </th>
                    <td>
                        <asp:TextBox ID="CheckPaymentAmount" runat="server" ValidationGroup="Check"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="AmountRequired" runat="server" Text="*"
                            ErrorMessage="Amount is required." Display="Static" ControlToValidate="CheckPaymentAmount"
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
                <tr>
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
                <tr>
                    <th class="rowHeader">
                        <asp:Label ID="BankAccountNumberLabel" runat="server" Text="Account Number:" AssociatedControlID="BankAccountNumber"></asp:Label>
	                </th>
                    <td>
                        <asp:TextBox id="BankAccountNumber" runat="server" ValidationGroup="Check"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="BankAccountNumberValidator" runat="server" ErrorMessage="You must enter the account number." 
                            ControlToValidate="BankAccountNumber" Display="Static" Text="*" ValidationGroup="Check" ></asp:RequiredFieldValidator>
                    </td>
                </tr>
            </table>
        </asp:PlaceHolder>
        <br />
        <asp:HyperLink ID="CancelLink" runat="server" Text="Cancel" SkinID="Button" NavigateUrl="~/Admin/Orders/Payments/Default.aspx"></asp:HyperLink>
        <asp:Button ID="SaveButton" runat="server" Text="Process" Visible="false" OnClick="SaveButton_Click" CausesValidation="true"></asp:Button>
    </ContentTemplate>
</ajax:UpdatePanel>