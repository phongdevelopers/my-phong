using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CommerceBuilder.Users;
using CommerceBuilder.Common;
using System.Collections.Generic;
using CommerceBuilder.Utility;

public partial class Admin_Dashboard_UserStatus : System.Web.UI.UserControl
{
    protected void ShowChangePassword_Click(object sender, System.EventArgs e)
    {
        ChangePasswordPanel.Visible = true;
        ShowChangePassword.Visible = false;
    }

    protected void CancelButton_Click(object sender, System.EventArgs e)
    {
        ChangePasswordPanel.Visible = false;
        ShowChangePassword.Visible = true;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        //DO NOT SCROLL TO VALIDATION SUMMARY
        PageHelper.DisableValidationScrolling(this.Page);
    }

    protected void ChangePasswordButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            //VERIFY CURRENT PASSWORD IS CORRECT
            User u = Token.Instance.User;
            if (u.CheckPassword(CurrentPassword.Text))
            {
                //VERIFY THE NEW PASSWORD MEETS POLICY
                PasswordPolicy policy = new MerchantPasswordPolicy();
                PasswordTestResult result = policy.TestPasswordWithFeedback(u, NewPassword.Text);
                if ((result & PasswordTestResult.Success) == PasswordTestResult.Success)
                {
                    u.SetPassword(NewPassword.Text);
                    ShowChangePassword.Visible = false;
                    ChangePasswordPanel.Visible = false;
                    PasswordChangedMessage.Visible = true;
                }
                else
                {
                    if (CurrentPassword.Text.Equals(NewPassword.Text))
                    {
                        AddPasswordValidator("Your new password is the same as your current password.");
                    }
                    else
                    {
                        //"Your new password did not meet the following minimum requirements:<br>";
                        if ((result & PasswordTestResult.PasswordTooShort) == PasswordTestResult.PasswordTooShort) AddPasswordValidator(string.Format(PasswordPolicyLength.Text, policy.MinLength));
                        if ((result & PasswordTestResult.RequireLower) == PasswordTestResult.RequireLower) AddPasswordValidator("New password must contain at least one lowercase letter.<br>");
                        if ((result & PasswordTestResult.RequireUpper) == PasswordTestResult.RequireUpper) AddPasswordValidator("New password must contain at least one uppercase letter.<br> ");
                        if ((result & PasswordTestResult.RequireNonAlpha) == PasswordTestResult.RequireNonAlpha) AddPasswordValidator("New password must contain at least one non-letter.<br> ");
                        if ((result & PasswordTestResult.RequireNumber) == PasswordTestResult.RequireNumber) AddPasswordValidator("New password must contain at least one number.<br> ");
                        if ((result & PasswordTestResult.RequireSymbol) == PasswordTestResult.RequireSymbol) AddPasswordValidator("New password must contain at least one symbol.<br> ");

                        if ((result & PasswordTestResult.PasswordHistoryLimitation) == PasswordTestResult.PasswordHistoryLimitation)
                        {
                            AddPasswordValidator("You have recently used this password.<br/>");
                        }
                    }
                    ChangePasswordPanel.Visible = true;
                    ShowChangePassword.Visible = false;
                }
            }
            else
            {
                CustomValidator validator = new CustomValidator();
                validator.ErrorMessage = "You did not type your current password correctly.";
                validator.Text = "*";
                validator.IsValid = false;
                validator.ValidationGroup = "UserStatus";
                phCustomValidator.Controls.Add(validator);

                ChangePasswordPanel.Visible = true;
                ShowChangePassword.Visible = false;
            }
        }
    }


    private void AddPasswordValidator(String message)
    {
        CustomValidator validator = new CustomValidator();
        validator.ValidationGroup = "UserStatus";
        validator.ErrorMessage = message;
        validator.Text = "*";
        validator.IsValid = false;
        phNewPasswordValidators.Controls.Add(validator);
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {

        User u = Token.Instance.User;
        UserName.Text = u.UserName;
        LastLogin.Text = string.Format("{0:f}", u.LastLoginDate);
        if (u.LastPasswordChangedDate == null || u.LastPasswordChangedDate.Equals(DateTime.MinValue))
        {
            LastPassword.Text = "You did not change your password yet.";
        }
        else
        {
            LastPassword.Text = string.Format(LastPassword.Text, LocaleHelper.LocalNow.Subtract(u.LastPasswordChangedDate).Days.ToString());
        }

        // UPDATE THE INSTRUCTIONS 
        if (ChangePasswordPanel.Visible)
        {
            PasswordPolicy policy = new MerchantPasswordPolicy();
            PasswordPolicyLength.Text = string.Format(PasswordPolicyLength.Text, policy.MinLength);
            PasswordPolicyHistoryCount.Visible = (policy.HistoryCount > 0);
            if (PasswordPolicyHistoryCount.Visible) PasswordPolicyHistoryCount.Text = string.Format(PasswordPolicyHistoryCount.Text, policy.HistoryCount);
            PasswordPolicyHistoryDays.Visible = (policy.HistoryDays > 0);
            if (PasswordPolicyHistoryDays.Visible) PasswordPolicyHistoryDays.Text = string.Format(PasswordPolicyHistoryDays.Text, policy.HistoryDays);
            List<string> requirements = new List<string>();
            if (policy.RequireUpper) requirements.Add("uppercase letter");
            if (policy.RequireLower) requirements.Add("lowercase letter");
            if (policy.RequireNumber) requirements.Add("number");
            if (policy.RequireSymbol) requirements.Add("symbol");
            if (!policy.RequireNumber && !policy.RequireSymbol && policy.RequireNonAlpha) requirements.Add("non-letter");
            PasswordPolicyRequired.Visible = (requirements.Count > 0);
            if (PasswordPolicyRequired.Visible)
            {
                if (requirements.Count > 1) requirements[requirements.Count - 1] = "and " + requirements[requirements.Count - 1];
                PasswordPolicyRequired.Text = string.Format(PasswordPolicyRequired.Text, string.Join(", ", requirements.ToArray()));
            }
        }
    }
}
