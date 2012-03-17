<%@ Page Language="C#" MasterPageFile="~/Admin/Orders/Order.master" CodeFile="VoidPayment.aspx.cs" Inherits="Admin_Orders_Payments_VoidPayment" Title="Void Payment" %>
<asp:Content ID="Content2" ContentPlaceHolderID="PrimarySidebarContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
        <div class="caption"><h1><asp:Localize ID="Caption" runat="server"></asp:Localize></h1></div>
    </div>
    <div style="clear: both;">
    <table cellpadding="0" cellspacing="5">
        <tr id="trProcessVoid" runat="server">
            <td colspan="2">
                By initiating this void, you are canceling this authorization and will be unable to capture any funds remaining on the authorization.  You may optionally enter a message to the customer, then click Void to process the request.
            </td>
        </tr>
        <tr id="trForceVoid" runat="server">
            <td colspan="2">
                <asp:Label ID="ForcedVoidPaymentLabel" runat="Server" Text="The current payment method or processor does not support void transactions.  You may optionally enter a message to the customer, then submit the form to force the status of this payment to Void.  Be sure to void the payment offline if necessary." /> 
                <asp:Label ID="ForcedVoidAuthorizationLabel" runat="Server" Text="The current payment method or processor does not support void transactions.  You may optionally enter a message to the customer, then submit the form to force the status of this authorization to Void.  Be sure to void the authorization offline if necessary." /> 
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Label ID="PaymentMethodLabel" runat="server" Text="Method:" SkinID="FieldHeader"></asp:Label>
            </td>
            <td>
                <asp:Label ID="PaymentReference" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Label ID="AuthorizationAmountLabel" runat="server" Text="Authorization Amount:" SkinID="FieldHeader"></asp:Label>
            </td>
            <td>
                <asp:Label ID="AuthorizationAmount" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Label ID="CaptureAmountLabel" runat="server" Text="Prior Captures:" SkinID="FieldHeader"></asp:Label>
            </td>
            <td>
                <asp:Label ID="CaptureAmount" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Label ID="VoidAmountLabel" runat="server" Text="Void Amount:" SkinID="FieldHeader"></asp:Label>
            </td>
            <td>
                <asp:Label ID="VoidAmount" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="right" valign="top">
                <asp:Label ID="CustomerNoteLabel" runat="server" Text="Note to Customer:" SkinID="FieldHeader"></asp:Label><br />
                (Optional)
            </td>
            <td>
                <asp:TextBox ID="CustomerNote" runat="server" TextMode="MultiLine" Rows="6" Columns="50"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td align="left">
                <asp:Button ID="SubmitVoidButton" runat="server" Text="Void" OnClick="SubmitVoidButton_Click"></asp:Button>
                <asp:Button ID="CancelVoidButton" runat="server" Text="Cancel" OnClick="CancelVoidButton_Click"></asp:Button>
            </td>
        </tr>
    </table>
    </div>
</asp:Content>

