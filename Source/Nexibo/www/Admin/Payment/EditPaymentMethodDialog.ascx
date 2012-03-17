<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditPaymentMethodDialog.ascx.cs" Inherits="Admin_Payment_EditPaymentMethodDialog" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<table cellpadding="4" cellspacing="0" class="inputForm" width="100%">
    <tr>
        <th class="rowHeader" width="80px">
            <cb:ToolTipLabel ID="InstrumentLabel" runat="server" Text="Type:" AssociatedControlID="PaymentInstrumentList" ToolTip="Indicates the type of payment instrument" />
        </th>
        <td>
            <asp:DropDownList ID="PaymentInstrumentList" runat="server">
                <asp:ListItem Text=""></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <cb:ToolTipLabel ID="NameLabel" runat="server" Text="Name:" AssociatedControlID="Name" ToolTip="Name of the payment method" />
        </th>
        <td>
            <asp:TextBox ID="Name" runat="server" MaxLength="100"></asp:TextBox>
            <asp:RequiredFieldValidator ID="NameRequired" runat="server" Text="*" Display="Static" ErrorMessage="Name is required." ControlToValidate="Name" ValidationGroup="AddDialog"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <cb:ToolTipLabel ID="GatewayLabel" runat="server" Text="Gateway:" AssociatedControlID="GatewayList" ToolTip="Indicates the payment gateway that will be used for this method" />
        </th>
        <td>
            <asp:DropDownList ID="GatewayList" runat="server">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <th class="rowHeader" valign="top">
            <cb:ToolTipLabel ID="GroupsLabel" runat="server" Text="Groups:" AssociatedControlID="UseGroupRestriction" ToolTip="Indicate whether only users that belong to specific groups can have this discount applied." />
        </th>
        <td colspan="3">
            <ajax:UpdatePanel ID="GroupRestrictionAjax" runat="server" UpdateMode="conditional">
                <ContentTemplate>
                    <asp:RadioButtonList ID="UseGroupRestriction" runat="server" AutoPostBack="true" OnSelectedIndexChanged="UseGroupRestriction_SelectedIndexChanged">
                        <asp:ListItem Text="All Groups" Selected="true"></asp:ListItem>
                        <asp:ListItem Text="Selected Groups"></asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Panel ID="GroupListPanel" runat="server" Visible="false">
                        <div style="padding-left:20px">
                            <asp:CheckBoxList ID="GroupList" runat="server" DataTextField="Name" DataValueField="GroupId"></asp:CheckBoxList>
                        </div>
                    </asp:Panel>
                </ContentTemplate>
            </ajax:UpdatePanel>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="AddPaymentMethod" />
            <asp:Label ID="AddedMessage" runat="server" Text="Payment method {0} added." SkinID="GoodCondition" EnableViewState="false" Visible="false"></asp:Label>
        </td>
    </tr>
    <tr>
        <td>&nbsp;</td>
        <td class="dialogSubmit">            
            <asp:Button ID="SaveButton" runat="server" OnClick="SaveButton_Click" Text="Save" ValidationGroup="EditPaymentMethod"></asp:Button>
			<asp:Button ID="CancelButton" runat="server" OnClick="CancelButton_Click" Text="Cancel" ValidationGroup="EditPaymentMethod" CausesValidation="false"></asp:Button>
        </td>
    </tr>
</table>