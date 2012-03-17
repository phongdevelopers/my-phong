<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UserStatus.ascx.cs" Inherits="Admin_Dashboard_UserStatus" EnableViewState="false" %>
<asp:Label ID="UserNameLabel" runat="server" Text="User:" SkinID="FieldHeader" EnableViewState="false"></asp:Label>
<asp:Label ID="UserName" runat="server" Text="" EnableViewState="false"></asp:Label><br />
<asp:Label ID="LastLoginLabel" runat="server" Text="Last Login:" SkinID="FieldHeader" EnableViewState="false"></asp:Label>
<asp:Label ID="LastLogin" runat="server" Text="" EnableViewState="false"></asp:Label><br />
<ajax:UpdatePanel ID="ChangePasswordAjax" runat="server">
    <ContentTemplate>
        <asp:Label ID="LastPassword" runat="server" Text="You changed your password {0} days ago." EnableViewState="false"></asp:Label><br />
        <asp:LinkButton ID="ShowChangePassword" runat="server" OnClick="ShowChangePassword_Click" Text="Change Password"></asp:LinkButton>
        <asp:Label ID="PasswordChangedMessage" runat="server" Text="Password changed." SkinID="GoodCondition" Visible="False" EnableViewState="False"></asp:Label>
        <br />
        <asp:Panel ID="ChangePasswordPanel" runat="server" HorizontalAlign="center" visible="false" DefaultButton="ChangePasswordButton">
            <table class="dialog" cellpadding="4" cellspacing="0" width="100%">
                <tr>
                    <th class="caption" colspan="2">
                        <asp:Label ID="ChangePasswordCaption" runat="server" Text="Change Your Password" EnableViewState="false"></asp:Label>
                    </th>
                </tr>
                <tr>
                    <th align="right">
                        <asp:Label ID="CurrentPasswordLabel" runat="server" AssociatedControlID="CurrentPassword" Text="Current Password:" EnableViewState="false"></asp:Label>
                    </th>
                    <td align="left">
                        <asp:TextBox ID="CurrentPassword" runat="server" Font-Size="0.8em" TextMode="Password" ValidationGroup="UserStatus"></asp:TextBox>
                        <asp:PlaceHolder ID="phCustomValidator" runat="server"></asp:PlaceHolder>
                        <asp:RequiredFieldValidator ID="CurrentPasswordRequired" runat="server" ControlToValidate="CurrentPassword"
                            ErrorMessage="Current password is required." ToolTip="Password is required." Text="*" ValidationGroup="UserStatus"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td align="left" colspan="2">
                        <asp:Localize ID="PasswordPolicyLength" runat="server" Text="The new password must be <b>at least {0} characters long</b>.<br/>"></asp:Localize>
                        <asp:Localize ID="PasswordPolicyHistoryCount" runat="server" Text="  You may <b>not use any of your previous {0} passwords</b>.<br/>"></asp:Localize>
                        <asp:Localize ID="PasswordPolicyHistoryDays" runat="server" Text="  You may <b>not reuse any passwords used within the last {0} days</b>.<br/>"></asp:Localize>
                        <asp:Localize ID="PasswordPolicyRequired" runat="server" Text="  The password <b>must include at least one {0}</b>."></asp:Localize>
                    </td>
                </tr>
                <tr>
                    <th align="right">
                        <asp:Label ID="NewPasswordLabel" runat="server" AssociatedControlID="NewPassword" Text="New Password" EnableViewState="false"></asp:Label>
                    </th>
                    <td align="left">
                        <asp:TextBox ID="NewPassword" runat="server" Font-Size="0.8em" TextMode="Password" ValidationGroup="UserStatus"></asp:TextBox>
                        <asp:PlaceHolder ID="phNewPasswordValidators" runat="server"></asp:PlaceHolder>
                        <asp:RequiredFieldValidator ID="NewPasswordRequired" runat="server" ControlToValidate="NewPassword"
                            ErrorMessage="New password is required." ToolTip="New Password is required."
                            Text="*" ValidationGroup="UserStatus"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <th align="right">
                        <asp:Label ID="ConfirmNewPasswordLabel" runat="server" AssociatedControlID="ConfirmNewPassword" Text="Retype:" EnableViewState="false"></asp:Label></td>
                    </th>
                    <td align="left">
                        <asp:TextBox ID="ConfirmNewPassword" runat="server" Font-Size="0.8em" TextMode="Password" ValidationGroup="UserStatus"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="ConfirmNewPasswordRequired" runat="server" ControlToValidate="ConfirmNewPassword"
                            ErrorMessage="Confirm New Password is required." ToolTip="Confirm New Password is required."
                            Text="*" ValidationGroup="UserStatus"></asp:RequiredFieldValidator >
                        <asp:CompareValidator ID="NewPasswordCompare" runat="server" ControlToCompare="NewPassword"
                            ControlToValidate="ConfirmNewPassword" Display="Dynamic" ErrorMessage="You did not retype the new password correctly."
                            Text="*" ValidationGroup="UserStatus"></asp:CompareValidator>
                    </td>
                </tr>
                <tr>
                    <td align="center" colspan="2">
                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" EnableViewState="false" ValidationGroup="UserStatus" />                        
                        <asp:Button ID="ChangePasswordButton" runat="server" Text="Update" OnClick="ChangePasswordButton_Click" EnableViewState="false" ValidationGroup="UserStatus" />
						<asp:Button ID="CancelButton" runat="server" CausesValidation="False" OnClick="CancelButton_Click" Text="Cancel" EnableViewState="false" ValidationGroup="UserStatus" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </ContentTemplate>
</ajax:UpdatePanel>
