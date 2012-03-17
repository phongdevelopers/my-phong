using System;
using CommerceBuilder.Marketing;
using CommerceBuilder.Utility;

public partial class Admin_Marketing_Email_ManageList : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private int _EmailListId;
    private EmailList _EmailList;
    protected void Page_Load(object sender, EventArgs e)
    {
        _EmailListId = AlwaysConvert.ToInt(Request.QueryString["EmailListId"]);
        _EmailList = EmailListDataSource.Load(_EmailListId);
        if (_EmailList == null) Response.Redirect("Default.aspx");
        if (!Page.IsPostBack)
        {
            Caption.Text = string.Format(Caption.Text, _EmailList.Name);
            int userCount = EmailListUserDataSource.CountForEmailList(_EmailListId);
            Members.Text = userCount.ToString();
            ManageMembersLink.NavigateUrl += "?EmailListId=" + _EmailListId.ToString();
            if (userCount > 0)
            {
                ExportListLink.NavigateUrl += "?EmailListId=" + _EmailListId.ToString();
                SendMessageLink.NavigateUrl += "?EmailListId=" + _EmailListId.ToString() + "&ReturnUrl=" + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("~/Admin/Marketing/Email/ManageList.aspx?EmailListId=" + _EmailListId.ToString()));
            }
            else
            {
                ExportListLink.Visible = false;
                SendMessageLink.Visible = false;
            }
            if (_EmailList.LastSendDate == null || _EmailList.LastSendDate.Equals(DateTime.MinValue))
            {
                LastSendDate.Text = "No mail sent yet.";
            }
            else
            {
                LastSendDate.Text = string.Format(LastSendDate.Text, _EmailList.LastSendDate);
            }
        }
        EditEmailListDialog1.ItemUpdated += new PersistentItemEventHandler(EditEmailListDialog1_ItemUpdated);
    }

    void EditEmailListDialog1_ItemUpdated(object sender, PersistentItemEventArgs e)
    {
        UpdatedMessage.Text = string.Format(UpdatedMessage.Text, LocaleHelper.LocalNow);
        UpdatedMessage.Visible = true;
        MainContentAjax.Update();
    }
}
