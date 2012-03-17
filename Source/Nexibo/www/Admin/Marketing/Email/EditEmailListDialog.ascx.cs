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
using CommerceBuilder.Marketing;
using CommerceBuilder.Utility;
using CommerceBuilder.Messaging;

public partial class Admin_Marketing_Email_EditEmailListDialog : System.Web.UI.UserControl
{
    private int _EmailListId;
    private EmailList _EmailList;
    protected void Page_Load(object sender, EventArgs e)
    {
        _EmailListId = AlwaysConvert.ToInt(Request.QueryString["EmailListId"]);
        _EmailList = EmailListDataSource.Load(_EmailListId);
        if (_EmailList != null)
        {
            PageHelper.ConvertEnterToTab(Name);
            if (!Page.IsPostBack)
            {
                Name.Text = _EmailList.Name;
                IsPublic.Checked = _EmailList.IsPublic;
                Description.Text = _EmailList.Description;
                ListItem item = SignupRule.Items.FindByValue(_EmailList.SignupRule.ToString());
                if (item != null)
                {
                    SignupRule.SelectedIndex = SignupRule.Items.IndexOf(item);
                }

                EmailTemplateCollection emailTemplates = EmailTemplateDataSource.LoadForStore();
                foreach (EmailTemplate template in emailTemplates)
                {
                    ListItem li = new ListItem(template.Name, template.EmailTemplateId.ToString());
                    SignupEmailTemplate.Items.Add(li);
                }

                item = SignupEmailTemplate.Items.FindByValue(_EmailList.SignupEmailTemplateId.ToString());
                if (item != null)
                {
                    SignupEmailTemplate.SelectedIndex = SignupEmailTemplate.Items.IndexOf(item);
                }
            }
        }
        else
        {
            this.Controls.Clear();
        }
    }

    //DEFINE AN EVENT TO TRIGGER WHEN AN ITEM IS ADDED 
    public event PersistentItemEventHandler ItemUpdated;


    protected void CloseButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Admin/Marketing/Email/Default.aspx");
    }
    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            _EmailList.Name = Name.Text;
            _EmailList.IsPublic = IsPublic.Checked;
            _EmailList.Description = StringHelper.Truncate(Description.Text, 250);
            _EmailList.SignupRule = (EmailListSignupRule)Enum.Parse(typeof(EmailListSignupRule), SignupRule.SelectedValue);
            _EmailList.SignupEmailTemplateId = AlwaysConvert.ToInt(SignupEmailTemplate.SelectedValue);
            _EmailList.Save();
            if (ItemUpdated != null) ItemUpdated(this, new PersistentItemEventArgs(_EmailList.EmailListId, _EmailList.Name));
        }
    }
}
