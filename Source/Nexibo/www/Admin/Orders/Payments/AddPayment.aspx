<%@ Page Language="C#" MasterPageFile="../Order.master" CodeFile="AddPayment.aspx.cs" Inherits="Admin_Orders_Payments_AddPayment" Title="Add Payment" %>
<%@ Register Src="ucProcessPayment.ascx" TagName="ucProcessPayment" TagPrefix="uc1" %>
<%@ Register Src="ucRecordPayment.ascx" TagName="ucRecordPayment" TagPrefix="uc2" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
        <div class="caption"><h1><asp:Localize ID="Caption" runat="server" Text="Add Payment"></asp:Localize></h1></div>
    </div>
    <ajax:UpdatePanel ID="PaymentAjax" runat="server">
        <ContentTemplate>
            <table class="inputForm" cellpadding="4" cellspacing="0" width="400">
                <tr>
                    <td>
                        <asp:Label ID="BalanceLabel" runat="server" Text="Order Balance:" SkinID="FieldHeader"></asp:Label>
                        <asp:Label ID="Balance" runat="server"></asp:Label>
                        <asp:Label ID="PendingMessage" runat="server" Text="One or more payments are in a pending state."></asp:Label>
                    </td>
                </tr>
                <tr class="sectionHeader">
                    <td>
                        <asp:RadioButton ID="ProcessPayment" runat="server" Text="Process Payment" GroupName="PaymentType" AutoPostBack="true" Checked="true" OnCheckedChanged="ProcessPayment_CheckedChanged" /><br />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="ProcessPaymentHelpText" runat="server" Text="Process a credit card payment online.  This is only available for payment methods that are linked to a real-time payment gateway." EnableViewState="false" CssClass="helpText"></asp:Label>
                        <asp:Panel ID="ProcessPaymentPanel" runat="server">
                            <uc1:ucProcessPayment id="UcProcessPayment1" runat="server">
                            </uc1:ucProcessPayment>
                        </asp:Panel>
                    </td>
                </tr>
                <tr class="sectionHeader">
                    <td>
                        <asp:RadioButton ID="RecordPayment" runat="server" Text="Record Payment" GroupName="PaymentType" AutoPostBack="true" OnCheckedChanged="RecordPayment_CheckedChanged" ToolTip="Record payment, sent in by check or other method outside the store." />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="RecordPaymentHelpText" runat="server" Text="Sent in by check or some other offline method." EnableViewState="false" CssClass="helpText"></asp:Label>
                        <asp:Panel ID="RecordPaymentPanel" runat="server" Visible="false">
                            <uc2:ucRecordPayment id="UcRecordPayment1" runat="server">
                            </uc2:ucRecordPayment>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>

