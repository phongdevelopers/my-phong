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
using CommerceBuilder.Messaging;
using CommerceBuilder.Utility;
using CommerceBuilder.Marketing;

public partial class Admin_Marketing_Email_AddEmailListDialog : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        PageHelper.ConvertEnterToTab(Name);
        if (!Page.IsPostBack)
        {
            //SignupEmailTemplate
            EmailTemplateCollection emailTemplates = EmailTemplateDataSource.LoadForStore();
            foreach (EmailTemplate template in emailTemplates)
            {
                ListItem li = new ListItem(template.Name, template.EmailTemplateId.ToString());
                SignupEmailTemplate.Items.Add(li);
            }
        }
    }

    private void ResetForm()
    {
        Name.Text = string.Empty;
        IsPublic.Checked = true;
        Description.Text = string.Empty;
        SignupRule.SelectedIndex = 0;
        SignupEmailTemplate.SelectedIndex = 0;
    }

    //DEFINE AN EVENT TO TRIGGER WHEN AN ITEM IS ADDED 
    public event PersistentItemEventHandler ItemAdded;

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            EmailList list = new EmailList();
            list.Name = Name.Text;
            list.IsPublic = IsPublic.Checked;
            list.Description = StringHelper.Truncate(Description.Text, 250);
            list.SignupRule = (EmailListSignupRule)Enum.Parse(typeof(EmailListSignupRule), SignupRule.SelectedValue);
            list.SignupEmailTemplateId = AlwaysConvert.ToInt(SignupEmailTemplate.SelectedValue);
            list.Save();
            if (ItemAdded != null) ItemAdded(this, new PersistentItemEventArgs(list.EmailListId, list.Name));
            AddedMessage.Visible = true;
            AddedMessage.Text = string.Format(AddedMessage.Text, list.Name);
            ResetForm();
        }
    }
}
