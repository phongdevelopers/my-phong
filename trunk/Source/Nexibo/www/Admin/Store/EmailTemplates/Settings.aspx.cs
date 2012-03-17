using System;
using System.Net.Mail;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommerceBuilder.Common;
using CommerceBuilder.Stores;
using CommerceBuilder.Marketing;
using CommerceBuilder.Messaging;
using CommerceBuilder.Utility;

public partial class Admin_Store_EmailTemplates_Settings : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private Store _Store;
    private StoreSettingCollection _Settings;

    protected void Page_Init(object sender, EventArgs e)
    {
        _Store = Token.Instance.Store;
        _Settings = _Store.Settings;

        // INITIALIZE GENERAL SETTINGS
        DefaultAddress.Text = _Settings.DefaultEmailAddress;
        SubscriptionAddress.Text = _Settings.SubscriptionEmailAddress;
        SubscriptionRequestExpirationDays.Text = _Settings.SubscriptionRequestExpirationDays.ToString();

        // BIND THE SEND TO FRIEND SELECTBOX
        EmailTemplateCollection templates = EmailTemplateDataSource.LoadForStore();
        EmailTemplatesList.DataSource = templates;
        EmailTemplatesList.DataBind();

        // BIND THE DEFAULT EMAIL LIST SELECTBOX
        EmailListCollection emailLists = EmailListDataSource.LoadForStore();
        DefaultEmailList.DataSource = emailLists;
        DefaultEmailList.DataBind();

        // INITIALIZE SERVER CONFIGURATION
        SmtpServer.Text = _Settings.SmtpServer;
        if (String.IsNullOrEmpty(_Settings.SmtpPort)) SmtpPort.Text = "25";
        else SmtpPort.Text = _Settings.SmtpPort;
        SmtpEnableSSL.Checked = _Settings.SmtpEnableSSL;
        SmtpUserName.Text = _Settings.SmtpUserName;
        RequiresAuth.Checked = _Settings.SmtpRequiresAuthentication;

        // DISABLE SCROLLING DURING VALIDATION
        PageHelper.DisableValidationScrolling(this.Page);
    }

    /// <summary>
    /// Saves the general settings
    /// </summary>
    private void SaveGeneralSettings()
    {
        string oldDefaultAddress = string.Empty;
        oldDefaultAddress = _Settings.DefaultEmailAddress;
        _Settings.DefaultEmailAddress = DefaultAddress.Text;
        _Settings.SubscriptionEmailAddress = SubscriptionAddress.Text;
        _Settings.SubscriptionRequestExpirationDays = AlwaysConvert.ToInt(SubscriptionRequestExpirationDays.Text);
        _Settings.DefaultEmailListId = AlwaysConvert.ToInt(DefaultEmailList.SelectedValue);
        _Settings.ProductTellAFriendEmailTemplateId = AlwaysConvert.ToInt(EmailTemplatesList.SelectedValue);
        _Settings.ProductTellAFriendCaptcha = TellAFriendCaptcha.Checked;
        _Settings.Save();
        if (UpdateAllEmailTemplates.Checked && _Settings.DefaultEmailAddress != oldDefaultAddress)
        {
            if (!String.IsNullOrEmpty(oldDefaultAddress))
            {
                string criteria = " StoreId = " + _Store.StoreId.ToString() + " AND FromAddress = '" + StringHelper.SafeSqlString(oldDefaultAddress) + "'";
                EmailTemplateCollection emailTemplates = EmailTemplateDataSource.LoadForCriteria(criteria);
                foreach (EmailTemplate emailTemplate in emailTemplates)
                {
                    emailTemplate.FromAddress = _Settings.DefaultEmailAddress;
                    emailTemplate.Save();
                }
            }
        }
    }

    /// <summary>
    /// Saves the SMTP settings
    /// </summary>
    private void SaveSmtpSettings()
    {
        _Settings.SmtpServer = SmtpServer.Text;
        _Settings.SmtpPort = SmtpPort.Text;
        _Settings.SmtpEnableSSL = SmtpEnableSSL.Checked;
        _Settings.SmtpUserName = SmtpUserName.Text;
        _Settings.SmtpPassword = SmtpPassword.Text;
        _Settings.SmtpRequiresAuthentication = RequiresAuth.Checked;
        _Settings.Save(_Store.StoreId);
    }

    protected void SaveGeneralButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            SaveGeneralSettings();
            GeneralSavedMessage.Text = string.Format(GeneralSavedMessage.Text, LocaleHelper.LocalNow);
            GeneralSavedMessage.Visible = true;
        }
    }

    protected void TestButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            SaveSmtpSettings();
            ToggleTestForm(true);
        }
    }

    protected void CancelTestButton_Click(object sender, EventArgs e)
    {
        ToggleTestForm(false);
    }

    private void ToggleTestForm(bool showForm)
    {
        TestButton.Visible = !showForm;
        TestPanel.Visible = showForm;
        SmtpServer.Enabled = !showForm;
        SmtpPort.Enabled = !showForm;
        SmtpUserName.Enabled = !showForm;
        SmtpPassword.Enabled = !showForm;
        RequiresAuth.Enabled = !showForm;
        SmtpEnableSSL.Enabled = !showForm;
        
        //IF THE FORM IS SHOWN, FOCUS TO THE SEND TO FIELD
        if (showForm) ScriptManager.GetCurrent(this.Page).SetFocus(TestSendTo);
    }

    /// <summary>
    /// Gets the most appropriate 'from' address to use with the test email
    /// </summary>
    /// <returns>A string containing the most appropriate 'from' address</returns>
    private string FindFromAddress()
    {
        if (!string.IsNullOrEmpty(_Settings.SubscriptionEmailAddress))
        {
            return _Settings.SubscriptionEmailAddress;
        }
        if (!string.IsNullOrEmpty(_Settings.DefaultEmailAddress))
        {
            return _Settings.DefaultEmailAddress;
        }
        return TestSendTo.Text;
    }

    protected void SendTestButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            // CREATE THE MAIL MESSAGE INSTANCE
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(FindFromAddress());
            mailMessage.To.Add(TestSendTo.Text);
            mailMessage.Subject = _Store.Name + ": SMTP Settings Updated";
            mailMessage.Body = string.Format("The SMTP Settings for {0} were recently updated.  Receipt of this email confirms the new settings are working properly.", _Store.Name);

            // PREPARE THE SMTP SETTINGS
            SmtpSettings smtpSettings = SmtpSettings.DefaultSettings;
            smtpSettings.Server = _Settings.SmtpServer;
            smtpSettings.Port = AlwaysConvert.ToInt(_Settings.SmtpPort, 25);
            smtpSettings.EnableSSL = _Settings.SmtpEnableSSL;
            if (_Settings.SmtpRequiresAuthentication)
            {
                smtpSettings.RequiresAuthentication = true;
                smtpSettings.UserName = _Settings.SmtpUserName;
                smtpSettings.Password = _Settings.SmtpPassword;
            }

            // ATTEMPT TO SEND THE TEST MESSAGE
            Label resultMessage = new Label();
            try
            {
                EmailClient.Send(mailMessage, smtpSettings, true);
                resultMessage.SkinID = "GoodCondition";
                resultMessage.Text = string.Format("Server configuration is updated and a test message has been sent to '{0}'.", TestSendTo.Text);
            }
            catch (Exception ex)
            {
                resultMessage.SkinID = "GoodCondition";
                resultMessage.Text = string.Format("Server configuration was updated but the test resulted in error: " + ex.Message);
            }
            SMTPTestResultPanel.Controls.Add(resultMessage);

            // HIDE THE TEST FORM
            ToggleTestForm(false);
        }
    }

    protected void RemoveButton_Click(object sender, EventArgs e)
    {
        SmtpServer.Text = String.Empty;
        SmtpPort.Text = string.Empty;
        SmtpEnableSSL.Checked = false;
        SmtpUserName.Text = String.Empty;
        SmtpPassword.Text = String.Empty;
        RequiresAuth.Checked = false;        

        SaveSmtpSettings();
        WarningLabel.Visible = true;
    }
        
    protected void Page_PreRender(object sender, EventArgs e)
    {
        // SET THE SMTP PASSWORD FIELD VALUE
        SmtpPassword.Attributes["Value"] = _Settings.SmtpPassword;

        // SELECT THE TELL A FRIEND EMAIL TEMPLATE
        int templateId = _Settings.ProductTellAFriendEmailTemplateId;
        ListItem item = EmailTemplatesList.Items.FindByValue(templateId.ToString());
        if (item != null) EmailTemplatesList.SelectedIndex = EmailTemplatesList.Items.IndexOf(item);
        TellAFriendCaptcha.Checked = _Settings.ProductTellAFriendCaptcha;

        // SELECT THE DEFAULT EMAIL LIST
        int listId = _Settings.DefaultEmailListId;
        item = DefaultEmailList.Items.FindByValue(listId.ToString());
        if (item != null) DefaultEmailList.SelectedIndex = DefaultEmailList.Items.IndexOf(item);

        // SHOW/HIDE REMOVE SETTINGS BUTTON
        RemoveButton.Visible = (!String.IsNullOrEmpty(SmtpServer.Text)  && !TestPanel.Visible);
    }
}