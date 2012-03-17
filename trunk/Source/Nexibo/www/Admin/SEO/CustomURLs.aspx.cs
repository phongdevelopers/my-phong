using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using CommerceBuilder.Catalog;
using CommerceBuilder.Products;


public partial class Admin_SEO_CustomURLs: CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    string _IconPath;
    protected void Page_Load(object sender, EventArgs e)
    {
        string theme = Page.Theme;
        if (string.IsNullOrEmpty(theme)) theme = Page.StyleSheetTheme;
        if (string.IsNullOrEmpty(theme)) theme = "AbleCommerceAdmin";
        _IconPath = Page.ResolveUrl("~/App_Themes/" + theme + "/Images/Icons/");
    }

    protected string GetCatalogItemName(Object dataItem) 
    {
        CustomUrl customUrl = (CustomUrl)dataItem;
        ICatalogable node = CatalogDataSource.Load(customUrl.CatalogNodeId, customUrl.CatalogNodeType);
        return node.Name;
    }

    protected string GetCatalogIconUrl(object dataItem)
    {
        CatalogNodeType nodeType = ((CustomUrl)dataItem).CatalogNodeType;
        switch (nodeType)
        {
            case CatalogNodeType.Category:
                return _IconPath + "Category.gif";
            case CatalogNodeType.Product:
                return _IconPath + "Product.gif";
            case CatalogNodeType.Webpage:
                return _IconPath + "Webpage.gif";
            case CatalogNodeType.Link:
                return _IconPath + "Link.gif";
        }
        return string.Empty;
    }

    protected string GetEditUrl(object dataItem)
    {
        CustomUrl customUrl = (CustomUrl)dataItem;
        string editUrl = string.Empty;
        switch (customUrl.CatalogNodeType)
        { 
            case CatalogNodeType.Category:
                editUrl = string.Format("~/Admin/Catalog/EditCategory.aspx?CategoryId={0}",customUrl.CatalogNodeId);
                break;

            case CatalogNodeType.Product:
                editUrl = string.Format("~/Admin/products/EditProduct.aspx?ProductId={0}", customUrl.CatalogNodeId);
                break;

            case CatalogNodeType.Webpage:
                editUrl = string.Format("~/Admin/Catalog/EditWebpage.aspx?WebpageId={0}", customUrl.CatalogNodeId);
                break;
                            
            case CatalogNodeType.Link:
                editUrl = string.Format("~/Admin/Catalog/EditLink.aspx?LinkId={0}", customUrl.CatalogNodeId);
                break;
        }
        return Page.ResolveUrl(editUrl);
    } 
}
