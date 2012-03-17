using System;
using System.Web.UI.WebControls;
using CommerceBuilder.Marketing;
using CommerceBuilder.Utility;

public partial class Admin_Marketing_Email_Default : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    protected int GetUserCount(object dataItem)
    {
        EmailList m = (EmailList)dataItem;
        return EmailListUserDataSource.CountForEmailList(m.EmailListId);
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        AddEmailListDialog1.ItemAdded += new PersistentItemEventHandler(AddEmailListDialog1_ItemAdded);
    }

    protected void AddEmailListDialog1_ItemAdded(object sender, PersistentItemEventArgs e)
    {
        EmailListGrid.DataBind();
        MainContentAjax.Update();
    }

    protected void EmailListGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Delete"))
        {
            EmailListDataSource.Delete(AlwaysConvert.ToInt(e.CommandArgument.ToString()));
        }
    }
}