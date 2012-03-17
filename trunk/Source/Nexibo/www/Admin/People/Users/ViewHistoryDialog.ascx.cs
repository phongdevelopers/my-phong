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
using CommerceBuilder.Utility;
using CommerceBuilder.Users;
using CommerceBuilder.Common;
using CommerceBuilder.Reporting;

public partial class Admin_People_Users_ViewHistoryDialog : System.Web.UI.UserControl
{
    private int _UserId;
    protected void Page_Load(object sender, EventArgs e)
    {
        _UserId = AlwaysConvert.ToInt(Request.QueryString["UserId"]);
        if (!Page.IsPostBack)
        {
            ViewsList.DataSource = PageViewDataSource.LoadForUser(_UserId, 10, 0, "ActivityDate DESC");
            ViewsList.DataBind();
            ViewsList.Visible = (ViewsList.Items.Count > 0);
            NoRecordedPageViewsPanel.Visible = !ViewsList.Visible;
        }
    }

    protected string GetReportLink()
    {
        return "~/Admin/Reports/CustomerHistory.aspx?UserId=" + _UserId.ToString();
    }

    protected string GetUri(object dataItem)
    {
        PageView pageView = (PageView)dataItem;
        if (!string.IsNullOrEmpty(pageView.UriQuery))
            return pageView.UriStem + "<wbr>?" + pageView.UriQuery.Replace("&", "<wbr>&").Replace("%", "<wbr>%");
        else return pageView.UriStem;
    }
}
