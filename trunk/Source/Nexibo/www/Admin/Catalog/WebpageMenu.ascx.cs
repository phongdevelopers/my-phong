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

public partial class Admin_Catalog_WebpageMenu : System.Web.UI.UserControl
{
    private int _WebPageId = 0;
    private Webpage _Webpage;

    protected void Page_Load(object sender, EventArgs e)
    {
        _WebPageId = PageHelper.GetWebpageId();
        _Webpage = WebpageDataSource.Load(_WebPageId);
        if (_Webpage != null)
        {
            string durl = "~/Admin/Catalog/EditWebPage.aspx?WebpageId={0}";
            durl = string.Format(durl, _WebPageId);

            string murl = "~/Admin/Catalog/EditWebpageCategories.aspx?WebpageId={0}";
            murl = string.Format(murl, _WebPageId);

            WebpageDetails.NavigateUrl = durl;
            ManageCategories.NavigateUrl = murl;
            string confirmationJS = String.Format("return confirm('Are you sure you want to delete {0}');", _Webpage.Name);
            DeleteWebpage.Attributes.Add("onclick", confirmationJS);
            Preview.NavigateUrl = UrlGenerator.GetBrowseUrl(PageHelper.GetCategoryId(), _WebPageId, CatalogNodeType.Webpage, _Webpage.Name);
            HighlightMenu();
        }
        else this.Controls.Clear();
    }

    private void HighlightMenu()
    {
        string fileName = Request.Url.Segments[Request.Url.Segments.Length - 1].ToLowerInvariant();
        switch (fileName)
        {
            case "editwebpage.aspx":
                WebpageDetails.CssClass = "contextMenuButtonSelected";
                break;
            case "editwebpagecategories.aspx":
                ManageCategories.CssClass = "contextMenuButtonSelected";
                break;
            default:
                WebpageDetails.CssClass = "contextMenuButtonSelected";
                break;
        }
    }

    protected void DeleteWebpage_Click(Object sender, EventArgs e)
    {
        string navigateUrl = "~/Admin/Catalog/Browse.aspx?CategoryId=" + PageHelper.GetCategoryId();
        if (_Webpage.Delete())
            Response.Redirect(navigateUrl);
    }

    protected void Page_PreRender(Object sender, EventArgs e)
    {
        if (_Webpage != null)
        {
            //PREVIEW LINK CAN CHANGE DUE TO CHANGE IN CUSTOM URL, WHICH CAN'T BE DETECTED BY PAGE_LOAD EVENT
            Preview.NavigateUrl = UrlGenerator.GetBrowseUrl(PageHelper.GetCategoryId(), _WebPageId, CatalogNodeType.Webpage, _Webpage.Name);
        }
    }

}
