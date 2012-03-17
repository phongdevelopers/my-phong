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
using System.Text.RegularExpressions;
using CommerceBuilder.Common;
using CommerceBuilder.Catalog;
using CommerceBuilder.Orders;
using CommerceBuilder.Products;
using CommerceBuilder.Stores;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;
using CommerceBuilder.Reporting;
using System.Collections.Generic;
using System.Net;

public partial class Admin_Reports_CustomerHistory : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private int _UserId;
    private User user;
    protected void Page_Load(object sender, EventArgs e)
    {
        _UserId = AlwaysConvert.ToInt(Request.QueryString["UserId"]);
        user = UserDataSource.Load(_UserId);
        if (user != null)
        {
            userLink.Text = (user.IsAnonymous ? "anonymous" : user.UserName);
            userLink.NavigateUrl = "~/Admin/People/Users/EditUser.aspx?UserId=" + user.UserId.ToString();
        }
    }
    protected void PageViewsGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        PageView pageView = (PageView)e.Row.DataItem;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {            
            ICatalogable catalogNode = pageView.CatalogNode;
            if (catalogNode != null)
            {
                PlaceHolder phCatalogNode = (PlaceHolder)e.Row.FindControl("phCatalogNode");
                if (phCatalogNode != null)
                {
                    HyperLink catalogLink = new HyperLink();
                    catalogLink.NavigateUrl = catalogNode.NavigateUrl;
                    catalogLink.EnableViewState = false;
                    Image catalogIcon = new Image();
                    catalogIcon.SkinID = pageView.CatalogNodeType.ToString() + "Icon";
                    catalogIcon.AlternateText = catalogNode.Name;
                    catalogIcon.EnableViewState = false;
                    catalogLink.Controls.Add(catalogIcon);
                    phCatalogNode.Controls.Add(catalogLink);
                }
            }
        }
    }

    private Dictionary<string, string> browsers = new Dictionary<string, string>();
    protected string GetBrowserName(string userAgent)
    {
        if (browsers.ContainsKey(userAgent)) return browsers[userAgent];
        string browserName;
        string url = "http://" + Request.Url.Authority + Page.ResolveUrl("~/IdentifyBrowser.ashx");
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        req.Timeout = 1000;
        try
        {
            req.UserAgent = userAgent;
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            using (System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream()))
            {
                browserName = sr.ReadToEnd();
                sr.Close();
            }
        }
        catch
        {
            browserName = "unknown";
        }
        browsers[userAgent] = browserName;
        return browserName;
    }

    protected string GetUri(object dataItem)
    {
        PageView pageView = (PageView)dataItem;
        if (!string.IsNullOrEmpty(pageView.UriQuery))
            return pageView.UriStem + "<wbr>?" + pageView.UriQuery.Replace("&", "<wbr>&").Replace("%", "<wbr>%");
        else return pageView.UriStem;
    }

}
