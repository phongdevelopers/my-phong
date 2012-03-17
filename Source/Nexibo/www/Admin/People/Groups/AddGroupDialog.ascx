<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AddGroupDialog.ascx.cs" Inherits="Admin_People_Groups_AddGroupDialog"  EnableViewState="true" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<ajax:UpdatePanel ID="AddGroupAjax" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table class="inputForm">
            <tr>
                <th class="rowHeader" nowrap>
                    <cb:ToolTipLabel ID="NameLabel" runat="server" Text="Group Name:" AssociatedControlID="Name" ToolTip="Name of the group."></cb:ToolTipLabel>
                </th>
                <td nowrap>
                    <asp:TextBox ID="Name" runat="server" MaxLength="100"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegExNameValidator" runat="server" ErrorMessage="Maximum length for Name is 100 characters." Text="*" ControlToValidate="Name" ValidationExpression=".{0,100}"  ValidationGroup="AddGroup"></asp:RegularExpressionValidator>
                    <asp:RequiredFieldValidator ID="NameValidator" runat="server" ControlToValidate="Name"
                        Display="Static" ErrorMessage="Name is required." ValidationGroup="AddGroup" Text="*"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <th class="rowHeader" valign="top" nowrap>
                    <cb:ToolTipLabel ID="RolesLabel" runat="server" Text="Permission(s):" AssociatedControlID="RoleList" ToolTip="Select any permissions that should be granted to members of this group."></cb:ToolTipLabel>
                </th>
                <td>
                    <asp:ListBox ID="RoleList" runat="server" SelectionMode="Multiple" Rows="6" DataTextField="Name" DataValueField="RoleId">
                    </asp:ListBox>
                </td>
            </tr>
            <tr>
                <th class="rowHeader" nowrap>
                    <cb:ToolTipLabel ID="TaxExemptLabel" runat="server" Text="Tax Exempt:" AssociatedControlID="IsTaxExempt" ToolTip="If checked, members of this group will not be charged tax."></cb:ToolTipLabel>
                </th>
                <td>
                    <asp:CheckBox runat="server" ID="IsTaxExempt" Checked="false" />
                </td>
            </tr>
        <tr>
            <td colspan="2" class="validation" align="center">
                <asp:Label ID="AddedMessage" runat="server" SkinID="GoodCondition" EnableViewState="false" Visible="false" Text="Group {0} added."></asp:Label>
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="AddGroup" />
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>
                <asp:Button ID="SaveButton" SkinId="AdminButton" runat="server" Text="Save" OnClick="SaveButton_Click" ValidationGroup="AddGroup" />
            </td>
        </tr>
    </table>
    </ContentTemplate>
</ajax:UpdatePanel>