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
using System.Text;
using System.Collections.Generic;
using CommerceBuilder.Utility;
using CommerceBuilder.Products;

public partial class Admin_Catalog_ChangeVisibility : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    int _CategoryId = 0;
    List<ICatalogable> _CatalogItems = new List<ICatalogable>();
    CategoryCollection _Categories;
    ProductCollection _Products;
    LinkCollection _Links;
    WebpageCollection _Webpages;
    String _IconPath = String.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        _CategoryId = AlwaysConvert.ToInt(Request.QueryString["CategoryId"]);
        InitializeCatalogItems();

        // INITIALIZE ICON PATH
        string theme = Page.Theme;
        if (string.IsNullOrEmpty(theme)) theme = Page.StyleSheetTheme;
        if (string.IsNullOrEmpty(theme)) theme = "AbleCommerceAdmin";
        _IconPath = Page.ResolveUrl("~/App_Themes/" + theme + "/Images/Icons/");

        CGrid.DataSource = _CatalogItems;
        CGrid.DataBind();

        if (!Page.IsPostBack)
        {
            //Caption.Text = string.Format(Caption.Text, CatalogNode.Name);

            //SET THE CURRENT PATH
            List<CatalogPathNode> currentPath = CatalogDataSource.GetPath(_CategoryId, false);
            CurrentPath.DataSource = currentPath;
            CurrentPath.DataBind();

        }
    }

    protected void InitializeCatalogItems()
    {
        String items = Request.QueryString["Objects"];
        if (String.IsNullOrEmpty(items)) return;
        String[] itemsArray = items.Split(',');
        List<String> categoryIds = new List<String>();
        List<String> productIds = new List<String>();
        List<String> linkIds = new List<String>();
        List<String> webpageIds = new List<String>();
        // Sort out items in saperate lists
        foreach (String item in itemsArray)
        {
            String[] itemValues = item.Split(':');
            int catalogNodeId = AlwaysConvert.ToInt(itemValues[0]);
            CatalogNodeType catalogNodeType = (CatalogNodeType)AlwaysConvert.ToByte(itemValues[1]);
            switch (catalogNodeType)
            {
                case CatalogNodeType.Category:
                    categoryIds.Add(catalogNodeId.ToString());
                    break;
                case CatalogNodeType.Product:
                    productIds.Add(catalogNodeId.ToString());
                    break;
                case CatalogNodeType.Link:
                    linkIds.Add(catalogNodeId.ToString());
                    break;
                case CatalogNodeType.Webpage:
                    webpageIds.Add(catalogNodeId.ToString());
                    break;
            }
        }

        trIncludeContents.Visible = (categoryIds.Count > 0);

        if (categoryIds.Count > 0)
        {
            _Categories = CategoryDataSource.LoadForCriteria(" CategoryId IN (" + String.Join(",", categoryIds.ToArray()) + ")");
            if (_Categories != null && _Categories.Count > 0)
            {
                foreach (Category category in _Categories)
                {
                    _CatalogItems.Add(category);
                }
            }
        }

        if (productIds.Count > 0)
        {
            _Products = ProductDataSource.LoadForCriteria(" ProductId IN (" + String.Join(",", productIds.ToArray()) + ")");
            if (_Products != null && _Products.Count > 0)
            {
                foreach (Product product in _Products)
                {
                    _CatalogItems.Add(product);
                }
            }
        }

        if (linkIds.Count > 0)
        {
            _Links = LinkDataSource.LoadForCriteria(" LinkId IN (" + String.Join(",", linkIds.ToArray()) + ")");
            if (_Links != null && _Links.Count > 0)
            {
                foreach (Link link in _Links)
                {
                    _CatalogItems.Add(link);
                }
            }
        }

        if (webpageIds.Count > 0)
        {
            _Webpages = WebpageDataSource.LoadForCriteria(" WebpageId IN (" + String.Join(",", webpageIds.ToArray()) + ")");
            if (_Webpages != null && _Webpages.Count > 0)
            {
                foreach (Webpage webpage in _Webpages)
                {
                    _CatalogItems.Add(webpage);
                }
            }
        }
    }

    protected string GetCatalogIconUrl(object dataItem)
    {
        ICatalogable catalogItem = ((ICatalogable)dataItem);
        Type itemType = catalogItem.GetType();         
        
        if(itemType == typeof(Category)) return _IconPath + "Category.gif";
        else if(itemType == typeof(Product)) return _IconPath + "Product.gif";
        else if (itemType == typeof(Webpage)) return _IconPath + "Webpage.gif";
        else if (itemType == typeof(Link)) return _IconPath + "Link.gif";
        
        return string.Empty;
    }
    protected string GetNavigateUrl(object dataItem)
    {

        ICatalogable catalogItem = ((ICatalogable)dataItem);
        Type itemType = catalogItem.GetType();

        if (itemType == typeof(Category)) return "~/Admin/Catalog/Browse.aspx?CategoryId=" + ((Category)catalogItem).CategoryId;
        else if (itemType == typeof(Product)) return "~/Admin/Products/EditProduct.aspx?ProductId=" + ((Product)catalogItem).ProductId;
        else if (itemType == typeof(Webpage)) return "~/Admin/Catalog/EditWebpage.aspx?WebpageId=" + ((Webpage)catalogItem).WebpageId;
        else if (itemType == typeof(Link)) return "~/Admin/Catalog/EditLink.aspx?LinkId=" + ((Link)catalogItem).LinkId;

        return string.Empty;
    }

    protected string GetVisibilityIconUrl(object dataItem)
    {
        return _IconPath + "Cms" + (((ICatalogable)dataItem).Visibility) + ".gif";
    }  

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("Browse.aspx?CategoryId=" + _CategoryId.ToString());
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        CatalogVisibility visibility = CatalogVisibility.Public;
        if (VisPublic.Checked) visibility = CatalogVisibility.Public;
        else if (VisHidden.Checked) visibility = CatalogVisibility.Hidden;
        else visibility = CatalogVisibility.Private;

        foreach (ICatalogable catalogItem in _CatalogItems)
        {
            if (catalogItem is Category)
            {
                Category item = (Category)catalogItem;
                item.Visibility = visibility;
                item.Save();
                //CHECK IF WE SHOULD RECURSE
                if (Scope.SelectedValue == "1")
                    UpdateChildNodes(item.CategoryId, visibility);
            }
            else if (catalogItem is Product)
            {
                Product item = (Product)catalogItem;
                item.Visibility = visibility;
                item.Save();
            }
            else if (catalogItem is Webpage)
            {
                Webpage item = (Webpage)catalogItem;
                item.Visibility = visibility;
                item.Save();
            }
            else if (catalogItem is Link)
            {
                Link item = (Link)catalogItem;
                item.Visibility = visibility;
                item.Save();
            }
        }
        
        //RETURN TO BROWSE MODE
        Response.Redirect("Browse.aspx?CategoryId=" + _CategoryId.ToString());
    }

    private void UpdateChildNodes(int categoryId, CatalogVisibility visibility)
    {
        CatalogNodeCollection children = CatalogDataSource.LoadForCategory(categoryId, false);
        foreach (CatalogNode child in children)
        {
            child.Visibility = visibility;
            child.ChildObject.Visibility = visibility;
            child.Save(true);
            if (child.CatalogNodeType == CatalogNodeType.Category)
                UpdateChildNodes(child.CatalogNodeId, visibility);
        }
    }    
}
