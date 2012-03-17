<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ucRecordPayment.ascx.cs" Inherits="Admin_Orders_Payments_ucRecordPayment" %>

<ajax:UpdatePanel ID="PaymentAjaxPanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
<table class="contentPanel" cellspacing="0">
    <tr>
        <td colspan="2" align="center">
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <asp:Label ID="PaymentMethodLabel" runat="server" Text="Payment Method:"></asp:Label>
        </th>
        <td>
            <asp:DropDownList ID="SelectedPaymentMethod" runat="server" DataValueField="PaymentMethodId" DataTextField="Name" AppendDataBoundItems="true" OnSelectedIndexChanged="SelectedPaymentMethod_SelectedIndexChanged" AutoPostBack="true">
                <asp:ListItem Value="" Text=""></asp:ListItem>
            </asp:DropDownList>
            <asp:RequiredFieldValidator ID="PaymentMethodRequiredValidator" runat="server"
                ErrorMessage="You must select a payment method." ControlToValidate="SelectedPaymentMethod" Display="Dynamic">*</asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <asp:Label ID="ReferenceNumberLabel" runat="server" Text="Reference Number:"></asp:Label>
        </th>
        <td>
            <asp:TextBox ID="ReferenceNumber" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <asp:Label ID="AmountLabel" runat="server" Text="Amount:"></asp:Label>
        </th>
        <td>
            <asp:TextBox ID="Amount" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ID="AmountRequiredValidator" runat="server" 
            ErrorMessage="You must enter the amount of payment." ControlToValidate="Amount">*</asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <asp:Label ID="PaymentStatusLabel" runat="server" Text="Status:"></asp:Label>
        </th>
        <td>
            <asp:DropDownList ID="selPaymentStatus" runat="server">
                <asp:ListItem Value=""></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <asp:Label ID="PaymentStatusReasonLabel" runat="server" Text="Status Notes:"></asp:Label>
        </th>
        <td>
            <asp:TextBox ID="PaymentStatusReason" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td colspan="2" align="center" style="padding-top:10px;padding-bottom:10px;">
            <asp:HyperLink ID="CancelLink" runat="server" SkinID="Button" NavigateUrl="Default.aspx">Cancel</asp:HyperLink>
            <asp:Button ID="SaveButton" runat="server" OnClick="SaveButton_Click" Text="Save"></asp:Button>
        </td>
    </tr>
</table>
</ContentTemplate>
</ajax:UpdatePanel>