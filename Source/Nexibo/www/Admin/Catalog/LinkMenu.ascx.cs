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
using CommerceBuilder.Catalog;

public partial class Admin_Catalog_LinkMenu : System.Web.UI.UserControl
{
    private int _LinkId = 0;
    private Link _Link;

    protected void Page_Load(object sender, EventArgs e)
    {
        _LinkId = PageHelper.GetLinkId();
        _Link = LinkDataSource.Load(_LinkId);
        if (_Link != null)
        {
            string durl = "~/Admin/Catalog/EditLink.aspx?LinkId={0}";
            durl = string.Format(durl, _LinkId);
            string murl = "~/Admin/Catalog/EditLinkCategories.aspx?LinkId={0}";
            murl = string.Format(murl, _LinkId);
            LinkDetails.NavigateUrl = durl;
            ManageCategories.NavigateUrl = murl;
            string confirmationJS = String.Format("return confirm('Are you sure you want to delete \\'{0}\\'?');", _Link.Name);
            DeleteLink.Attributes.Add("onclick", confirmationJS);
            Preview.NavigateUrl = UrlGenerator.GetBrowseUrl(PageHelper.GetCategoryId(), _LinkId, CatalogNodeType.Link, _Link.Name);
            HighlightMenu();
        }
        else this.Controls.Clear();
    }

    private void HighlightMenu()
    {
        string fileName = Request.Url.Segments[Request.Url.Segments.Length - 1].ToLowerInvariant();
        switch (fileName)
        {
            case "editlink.aspx":
                LinkDetails.CssClass = "contextMenuButtonSelected";
                break;
            case "editlinkcategories.aspx":
                ManageCategories.CssClass = "contextMenuButtonSelected";
                break;
            default:
                LinkDetails.CssClass = "contextMenuButtonSelected";
                break;
        }
    }

    protected void DeleteLink_Click(Object sender, EventArgs e)
    {
        string navigateUrl = "~/Admin/Catalog/Browse.aspx?CategoryId=" + PageHelper.GetCategoryId();
        if (_Link.Delete())
            Response.Redirect(navigateUrl);
    }

}
