<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AddEmailListDialog.ascx.cs" Inherits="Admin_Marketing_Email_AddEmailListDialog" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<ajax:UpdatePanel ID="AddEmailListAjax" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table class="inputForm" width="100%">
            <tr>
                <td align="center" colspan="2">
                    <asp:Label ID="AddedMessage" runat="server" Text="Email list {0} added." SkinID="GoodCondition" EnableViewState="false" Visible="false"></asp:Label>
                </td>
            </tr>
            <tr>
                <th class="rowHeader">
                    <cb:ToolTipLabel ID="NameLabel" runat="server" Text="Name:" AssociatedControlID="Name" ToolTip="The name of the email list." />
                </th>
                <td>
                    <asp:TextBox ID="Name" runat="server" Columns="30" MaxLength="100" TabIndex="1"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="NameRequired" runat="server" ControlToValidate="Name"
                        ErrorMessage="List name is required." ToolTip="List name is required." 
                        Display="Static" Text="*" ValidationGroup="AddEmailList"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <th class="rowHeader" valign="top" nowrap>
                    <cb:ToolTipLabel ID="IsPublicLabel" runat="server" Text="Public List:" AssociatedControlID="IsPublic" ToolTip="Indicates whether the list is public and visible to customers." />
                </th>
                <td>
                    <asp:CheckBox ID="IsPublic" runat="server" Checked="true" TabIndex="2" />
                </td>
            </tr>
            <tr>
                <th class="rowHeader" valign="top">
                    <cb:ToolTipLabel ID="DescriptionLabel" runat="server" Text="Description:" AssociatedControlID="Description" ToolTip="Provides a description of the email list." />
                </th>
                <td>
                    <asp:TextBox ID="Description" runat="server" TextMode="MultiLine" Rows="5" width="100%" TabIndex="3" Wrap="true"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <th class="rowHeader">
                    <cb:ToolTipLabel ID="SignupRuleLabel" runat="server" Text="Signup Rule:" AssociatedControlID="SignupRule" ToolTip="Indicates the method of verification used for list signups initiated by customers." />
                </th>
                <td>
                    <asp:DropDownList ID="SignupRule" runat="server" TabIndex="4">
                        <asp:ListItem Value="VerifiedOptIn" Text="Opt-In with Verification" Selected="true"></asp:ListItem>
                        <asp:ListItem Value="ConfirmedOptIn" Text="Opt-In with Confirmation"></asp:ListItem>                        
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <th class="rowHeader">
                    <cb:ToolTipLabel ID="SignupEmailTemplateLabel" runat="server" Text="Signup Email:" AssociatedControlID="SignupEmailTemplate" ToolTip="If required, specifies the email message template used to verify list signups." />
                </th>
                <td>
                    <asp:DropDownList ID="SignupEmailTemplate" runat="server" TabIndex="5">
                        <asp:ListItem Value="0" Text=">Default Signup Message<"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td colspan="2" class="validation">
                    <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="AddEmailList" />
                </td>
            </tr>
            <tr>
                <td colspan="2" class="submit">
                    <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ValidationGroup="AddEmailList" OnClientClick="if (Page_ClientValidate()) { this.value = 'Saving...'; this.enabled= false; }" TabIndex="6" />
                </td>
            </tr>
        </table>
    </ContentTemplate>
</ajax:UpdatePanel>
