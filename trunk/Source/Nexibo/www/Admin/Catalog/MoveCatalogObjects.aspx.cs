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

public partial class Admin_Catalog_MoveCatalogObjects : CommerceBuilder.Web.UI.AbleCommerceAdminPage
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

            //INITIALIZE THE TREE
            InitializeCategoryTree();
        }

        if (_Categories != null && _Categories.Count > 0 && MoveOptions.Items.Count > 2)
        {
            MoveOptions.Items.RemoveAt(2);
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

    protected void InitializeCategoryTree()
    {
        int st = 1;
        if ((_Categories !=null && _Categories.Count > 0) && (_Products == null || _Products.Count == 0) && (_Webpages == null || _Webpages.Count == 0) && (_Links == null || _Links.Count == 0))
        {
            ListItem item = new ListItem("Top Level", "0");
            NewPath.Items.Add(item);
            st = 0;
        }

        CategoryLevelNodeCollection categories = CategoryParentDataSource.GetCategoryLevels(0);
        foreach (CategoryLevelNode node in categories)
        {
            string prefix = string.Empty;
            for (int i = st; i <= node.CategoryLevel; i++) prefix += " . . ";
            NewPath.Items.Add(new ListItem(prefix + node.Name, node.CategoryId.ToString()));            
        }
        //FIND CURRENT CATEGORY AND REMOVE
        ListItem current = NewPath.Items.FindByValue(_CategoryId.ToString());
        if (current != null) NewPath.SelectedIndex = NewPath.Items.IndexOf(current);
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("Browse.aspx?CategoryId=" + _CategoryId.ToString());
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        int _NewCategoryId = AlwaysConvert.ToInt(NewPath.SelectedValue);
        if (_CategoryId != _NewCategoryId)
        {
            Category currentCategory = CategoryDataSource.Load(_CategoryId);
            Category targetCategory = CategoryDataSource.Load(_NewCategoryId);
            // CATEGORIES CAN ONLY BE MOVED, NOT MULTIPARENTED
            if (_Categories != null)
            {
                foreach (Category category in _Categories)
                {
                    category.ParentId = _NewCategoryId;
                    category.Save();
                }
            }
            bool moveAll = (MoveOptions.SelectedValue == "MoveAll");
            bool moveSingle = (MoveOptions.SelectedValue == "MoveSingle");
            // LOOP SELECTED ITEMS AND MOVE AS NEEDED
            foreach (ICatalogable catalogItem in _CatalogItems)
            {
                if (catalogItem is Product)
                {
                    Product product = (Product)catalogItem;
                    if (moveAll) product.Categories.Clear();
                    else if (moveSingle) product.Categories.Remove(_CategoryId);
                    if (!product.Categories.Contains(_NewCategoryId)) product.Categories.Add(_NewCategoryId);
                    product.Categories.Save();
                }
                else if (catalogItem is Webpage)
                {
                    Webpage webpage = (Webpage)catalogItem;
                    if (moveAll) webpage.Categories.Clear();
                    else if (moveSingle) webpage.Categories.Remove(_CategoryId);
                    if (!webpage.Categories.Contains(_NewCategoryId)) webpage.Categories.Add(_NewCategoryId);
                    webpage.Categories.Save();
                }
                else if (catalogItem is Link)
                {
                    Link link = (Link)catalogItem;
                    if (moveAll) link.Categories.Clear();
                    else if (moveSingle) link.Categories.Remove(_CategoryId);
                    if (!link.Categories.Contains(_NewCategoryId)) link.Categories.Add(_NewCategoryId);
                    link.Categories.Save();
                }
            }
        }
        Response.Redirect("Browse.aspx?CategoryId=" + _NewCategoryId.ToString());
    }

    private bool MatchTypes(CatalogNodeType catalogNodeType, ICatalogable catalogItem)
    {
        Type catalogItemType = catalogItem.GetType();
        if (catalogItemType == typeof(Category) && catalogNodeType == CatalogNodeType.Category) return true;
        else if (catalogItemType == typeof(Product) && catalogNodeType == CatalogNodeType.Product) return true;
        else if (catalogItemType == typeof(Webpage) && catalogNodeType == CatalogNodeType.Webpage) return true;
        else if (catalogItemType == typeof(Link) && catalogNodeType == CatalogNodeType.Link) return true;
        return false;
    }
}
