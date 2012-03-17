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

public partial class Admin_Reports_WhoIsOnline : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            //initialize form
            ActivityThreshold.Text = "30";
        }
    }

    protected void OnlineUserGrid_Sorting(object sender, GridViewSortEventArgs e)
    {
        if (e.SortExpression != OnlineUserGrid.SortExpression) e.SortDirection = SortDirection.Descending;
    }

    protected void ApplyButton_Click(object sender, EventArgs e)
    {
        OnlineUserGrid.DataBind();
    }

    protected string GetIpAddress(object usrObj)
    {
        User usr = usrObj as User;
        if (usr == null) return "";

        if (usr.PageViews.Count > 0)
        {
            CommerceBuilder.Reporting.PageView pv = usr.PageViews[0];
            return pv.RemoteIP;
        }

        return "";
    }    
}
