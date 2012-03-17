using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CommerceBuilder.Catalog;
using CommerceBuilder.Products;
using CommerceBuilder.Utility;

public partial class Admin_Catalog_Browse : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private const bool isAjax = true;
    private bool redrawTree = false;
    protected bool productsBound = false;
    private string _IconPath = string.Empty;
    private int _LastCategoryId = -1;

    protected int CurrentCategoryId
    {
        get
        {
            if (ViewState["CurrentCategoryId"] == null)
            {
                int temp = AlwaysConvert.ToInt(Request.Form[LastCategory.UniqueID]);
                if (temp == 0) temp = PageHelper.GetCategoryId();
                ViewState["CurrentCategoryId"] = temp;
            }
            return (int)ViewState["CurrentCategoryId"];
        }
        set
        {
            ViewState["CurrentCategoryId"] = value;
            _CurrentCategory = null;
        }
    }

    private Category _CurrentCategory = null;
    protected Category CurrentCategory
    {
        get
        {
            if (_CurrentCategory == null)
            {
                if (CurrentCategoryId == 0)
                {
                    _CurrentCategory = new Category();
                    _CurrentCategory.CategoryId = 0;
                    _CurrentCategory.Name = "Catalog";
                    _CurrentCategory.Visibility = CatalogVisibility.Public;
                }
                else _CurrentCategory = CategoryDataSource.Load(CurrentCategoryId);
            }
            return _CurrentCategory;
        }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        string theme = Page.Theme;
        if (string.IsNullOrEmpty(theme)) theme = Page.StyleSheetTheme;
        if (string.IsNullOrEmpty(theme)) theme = "AbleCommerceAdmin";
        _IconPath = Page.ResolveUrl("~/App_Themes/" + theme + "/Images/Icons/");
        //REGISTER DELETE SCRIPTS
        StringBuilder script = new StringBuilder();
        script.Append("function CD1(n) { return confirm(\"Are you sure you want to delete '\" + n + \"'?\") }\r\n");
        script.Append("function CD2(n) { return confirm(\"Are you sure you want to delete '\" + n + \"' and all its contents?\") }\r\n");
        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ConfirmDelete", script.ToString(), true);
        //INITIALIZE CATEGORY TREE FOR POSTBACK
        BindCategoryTree();
    }
    

    protected string GetCatalogIconUrl(object dataItem)
    {
        CatalogNodeType nodeType = ((CatalogNode)dataItem).CatalogNodeType;
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

    protected string GetVisibilityIconUrl(object dataItem)
    {
        return _IconPath + "Cms" + (((CatalogNode)dataItem).Visibility) + ".gif";
    }

    protected string GetIconUrl(string icon)
    {
        return _IconPath + icon;
    }

    protected string GetBrowseUrl(object dataItem)
    {
        CatalogNode node = (CatalogNode)dataItem;
        string url = string.Empty;
        switch (node.CatalogNodeType)
        {
            case CatalogNodeType.Category:
                url = "~/Admin/Catalog/Browse.aspx?CategoryId=" + node.CatalogNodeId.ToString();
                break;
            case CatalogNodeType.Product:
                url = "~/Admin/Products/EditProduct.aspx?CategoryId=" + CurrentCategoryId.ToString() + "&ProductId=" + node.CatalogNodeId.ToString();
                break;
            case CatalogNodeType.Webpage:
                url = "~/Admin/Catalog/EditWebpage.aspx?CategoryId = " + CurrentCategoryId.ToString() + "&WebpageId=" + node.CatalogNodeId.ToString();
                break;
            case CatalogNodeType.Link:
                url = "~/Admin/Catalog/EditLink.aspx?CategoryId = " + CurrentCategoryId.ToString() + "&LinkId=" + node.CatalogNodeId.ToString();
                break;
        }
        return Page.ResolveUrl(url);
    }

    protected string GetEditUrl(object dataItem, object dataItemId)
    {
        CatalogNodeType nodeType = (CatalogNodeType)dataItem;
        int catalogNodeId = (int)dataItemId;
        string url;
        switch (nodeType)
        {
            case CatalogNodeType.Category:
                    url = "~/Admin/Catalog/EditCategory.aspx?CategoryId=" + catalogNodeId.ToString();
                break;
            case CatalogNodeType.Product:
                url = "~/Admin/products/EditProduct.aspx?ProductId=" + catalogNodeId.ToString() + "&CategoryId=" + this.CurrentCategoryId.ToString();
                break;
            case CatalogNodeType.Webpage:
                url = "~/Admin/Catalog/EditWebpage.aspx?WebpageId=" + catalogNodeId.ToString() + "&CategoryId=" + this.CurrentCategoryId.ToString();
                break;
            case CatalogNodeType.Link:
                url = "~/Admin/Catalog/EditLink.aspx?LinkId=" + catalogNodeId.ToString() + "&CategoryId=" + this.CurrentCategoryId.ToString();
                break;
            default:
                url = "~/Admin/Catalog/Browse.aspx";
                break;
        }
        return Page.ResolveUrl(url);
    }

	protected string GetPreviewUrl(object dataItem, object dataItemId, object dataItemName)
	{
        CatalogNodeType nodeType = (CatalogNodeType)dataItem;
        int catalogNodeId = (int)dataItemId;
		string nodeName = (string)dataItemName;
        string url = UrlGenerator.GetBrowseUrl(catalogNodeId, nodeType, nodeName);
		return Page.ResolveUrl(url);
	}

    protected void CGrid_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        if (e.CommandName.StartsWith("Do_"))
        {
            string[] args = e.CommandArgument.ToString().Split("|".ToCharArray());
            CatalogNodeType catalogNodeType = (CatalogNodeType)AlwaysConvert.ToInt(args[0]);
            int catalogNodeId = AlwaysConvert.ToInt(args[1]);
            int index;
            switch (e.CommandName)
            {
                case "Do_Open":
                    //IF CATEGORY, REBIND CURRENT PAGE,
                    //OTHERWISE REDIRECT TO CORRECT EDIT PAGE
                    switch (catalogNodeType)
                    {
                        case CatalogNodeType.Category:
                            //REBIND CURRENT CATEGORY
                            //if (!isAjax) Response.Redirect("~/Admin/Catalog/Browse.aspx?CategoryId=" + catalogNodeId.ToString());
                            CurrentCategoryId = catalogNodeId;
                            //RESET PAGE INDEX
                            CGrid.PageIndex = 0;
                            break;
                        case CatalogNodeType.Product:
                            Response.Redirect("~/Admin/Products/EditProduct.aspx?CategoryId=" + CurrentCategoryId.ToString() + "&ProductId=" + catalogNodeId.ToString());
                            break;
                        case CatalogNodeType.Webpage:
                            Response.Redirect("~/Admin/Catalog/EditWebpage.aspx?CategoryId=" + CurrentCategoryId.ToString() + "&WebpageId=" + catalogNodeId.ToString());
                            break;
                        case CatalogNodeType.Link:
                            Response.Redirect("~/Admin/Catalog/EditLink.aspx?CategoryId=" + CurrentCategoryId.ToString() + "&LinkId=" + catalogNodeId.ToString());
                            break;
                    }
                    break;
                case "Do_Copy":
                    CatalogDataSource.Copy(catalogNodeId, catalogNodeType, CurrentCategoryId);
                    break;
                case "Do_Delete":
                    DoDelete(catalogNodeType, catalogNodeId);
                    break;
                case "Do_Up":
                    index = CurrentCategory.CatalogNodes.IndexOf(CurrentCategoryId, catalogNodeId, (byte)catalogNodeType);
                    if (index > 0)
                    {
                        CatalogNode tempNode = CurrentCategory.CatalogNodes[index - 1];
                        CurrentCategory.CatalogNodes[index - 1] = CurrentCategory.CatalogNodes[index];
                        CurrentCategory.CatalogNodes[index] = tempNode;
                    }
                    index = 0;
                    foreach (CatalogNode node in CurrentCategory.CatalogNodes)
                    {
                        node.OrderBy = (short)index;
                        node.Save();
                        index++;
                    }
                    // THIS WILL CAUSE TO RE-RENDER THE CATEGORY TREE
                    _LastCategoryId = -1;
                    break;
                case "Do_Down":
                    index = CurrentCategory.CatalogNodes.IndexOf(CurrentCategoryId, catalogNodeId, (byte)catalogNodeType);
                    if (index < CurrentCategory.CatalogNodes.Count -1)
                    {
                        CatalogNode tempNode = CurrentCategory.CatalogNodes[index + 1];
                        CurrentCategory.CatalogNodes[index + 1] = CurrentCategory.CatalogNodes[index];
                        CurrentCategory.CatalogNodes[index] = tempNode;
                    }
                    index = 0;
                    foreach (CatalogNode node in CurrentCategory.CatalogNodes)
                    {
                        node.OrderBy = (short)index;
                        node.Save();
                        index++;
                    }
                    // THIS WILL CAUSE TO RE-RENDER THE CATEGORY TREE
                    _LastCategoryId = -1;
                    break;
                case "Do_Pub":
                    index = CurrentCategory.CatalogNodes.IndexOf(CurrentCategoryId, catalogNodeId, (byte)catalogNodeType);
                    if (index > -1)
                    {
                        CatalogNode node = CurrentCategory.CatalogNodes[index];
                        //FOR CATEGORIES, WE MUST FIND OUT MORE INFORMATION
                        if (node.CatalogNodeType == CatalogNodeType.Category)
                            Response.Redirect("ChangeVisibility.aspx?CategoryId=" + CurrentCategoryId.ToString()+String.Format("&Objects={0}:{1}",node.CatalogNodeId, (byte)node.CatalogNodeType));
                        //FOR OTHER OBJECTS, WE CAN ADJUST
                        switch (node.Visibility)
                        {
                            case CatalogVisibility.Public:
                                node.Visibility = CatalogVisibility.Hidden;
                                break;
                            case CatalogVisibility.Hidden:
                                node.Visibility = CatalogVisibility.Private;
                                break;
                            default:
                                node.Visibility = CatalogVisibility.Public;
                                break;
                        }
                        node.ChildObject.Visibility = node.Visibility;
                        node.Save(true);
                    }
                    break;
            }
        }
    }

    protected void CGrid_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //GET DATA ITEM
            CatalogNode node = (CatalogNode)e.Row.DataItem;
            //UPDATE DELETE BUTTON WARNING
            LinkButton deleteButton = e.Row.FindControl("D") as LinkButton;
            if (deleteButton != null)
            {
                if (node.CatalogNodeType != CatalogNodeType.Category) deleteButton.Attributes.Add("onclick", "return CD1(\"" + node.Name + "\")");
                else deleteButton.Attributes.Add("onclick", "return CD2(\"" + node.Name + "\")");
            }
        }
    }

    protected void DoDelete(CatalogNodeType catalogNodeType, int catalogNodeId)
    {
        switch (catalogNodeType)
        {
            case CatalogNodeType.Category:
                Category category = CategoryDataSource.Load(catalogNodeId);
                if (category != null) category.Delete();
                // THIS WILL CAUSE TO RE-RENDER THE CATEGORY TREE
                _LastCategoryId = -1;
                break;
            case CatalogNodeType.Product:
                Product product = ProductDataSource.Load(catalogNodeId);
                if (product != null)
                {
                    if (product.Categories.Count > 1) { product.Categories.Remove(CurrentCategoryId); product.Save(); }
                    else product.Delete();
                }
                break;
            case CatalogNodeType.Webpage:
                Webpage webpage = WebpageDataSource.Load(catalogNodeId);
                if (webpage != null){
                    if (webpage.Categories.Count > 1) { webpage.Categories.Remove(CurrentCategoryId); webpage.Save(); }
                    else webpage.Delete();
                }
                break;
            case CatalogNodeType.Link:
                Link link = LinkDataSource.Load(catalogNodeId);
                if (link != null) {
                    if (link.Categories.Count > 1) { link.Categories.Remove(CurrentCategoryId); link.Save(); }
                    else link.Delete();
                }
                break;
        }
        redrawTree = (catalogNodeType == CatalogNodeType.Category);
    }

    protected void AddCategory_Click(object sender, EventArgs e)
    {
        string tempName = AddCategoryName.Text.Trim();
        if (tempName.Length > 0)
        {
            Category newCategory = new Category();
            newCategory.ParentId = this.CurrentCategoryId; newCategory.Name = AddCategoryName.Text;
            newCategory.Visibility = CatalogVisibility.Public;
            newCategory.Save();
            CategoryAddedMessage.Visible = true;
            CategoryAddedMessage.Text = string.Format(CategoryAddedMessage.Text, newCategory.Name);
            redrawTree = true;
            AddCategoryName.Text = string.Empty;
            // THIS WILL CAUSE TO RE-RENDER THE CATEGORY TREE
            _LastCategoryId = -1;

        }
    }

    protected void Search_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(SearchPhrase.Text))
        {
            List<string> typeFilter = new List<string>();
            if (IncludeCategories.Checked) typeFilter.Add("0");
            if (IncludeProducts.Checked) typeFilter.Add("1");
            if (IncludeWebpages.Checked) typeFilter.Add("2");
            if (IncludeLinks.Checked) typeFilter.Add("3");
            string types = string.Join(",", typeFilter.ToArray());
            if (!string.IsNullOrEmpty(types))
            {
                Response.Redirect("Search.aspx?CategoryId=" + CurrentCategoryId.ToString() + "&k=" + Server.UrlEncode(SearchPhrase.Text) + "&titles=" + (TitlesOnly.Checked ? "1" : "0") + "&recurse=" + (Recursive.Checked ? "1" : "0") + "&types=" + types);
            }
        }
    }

    protected void CatalogDs_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        e.InputParameters["categoryId"] = CurrentCategoryId;
    }

    protected void ParentCategory_Click(object sender, EventArgs e)
    {
        CurrentCategoryId = CurrentCategory.ParentId;
    }

    #region "PreRender"
    protected void Page_PreRender(object sender, EventArgs e)
    {
        BindCurrentCategory();
    }

    protected void BindCurrentCategory()
    {
        BindCategoryTree();
        BindCategoryHeader();
        BindBreadCrumbs();
        BindCGrid();
        BindActionMenu();
        LastCategory.Value = CurrentCategoryId.ToString();
    }

    protected void BindCategoryTree()
    {
        //CHECK TO MAKE SURE WE ARE NOT RE-RENDERING THE SAME TREE
        //THAT WAS RENDERED IN THE INIT PHASE
        if (_LastCategoryId != CurrentCategoryId)
        {
            //GET THE SELECTED CATEGORY PATH
            List<CatalogPathNode> path = CatalogDataSource.GetPath(CurrentCategoryId, false);
            bool hasPath = ((path != null) && (path.Count > 0));
            phCategoryTree.Controls.Clear();
            phCategoryTree.Controls.Add(new LiteralControl("<div class=\"categoryTree\">\r\n"));
            CategoryCollection topLevelCategories = CategoryDataSource.LoadForParent(0, false);
            LinkButton categoryLink;
            string theme = Page.Theme;
            if (string.IsNullOrEmpty(theme)) theme = Page.StyleSheetTheme;
            if (string.IsNullOrEmpty(theme)) theme = "AbleCommerceAdmin";
            string minusIcon = "<img src=\"" + this.Page.ResolveUrl("~/App_Themes/" + theme + "/Images/minus.gif") + "\" border=\"0\" align=\"absmiddle\" />";
            string plusIcon = "<img src=\"" + this.Page.ResolveUrl("~/App_Themes/" + theme + "/Images/plus.gif") + "\" border=\"0\" align=\"absmiddle\" />";
            
            foreach (Category category in topLevelCategories)
            {
                if ((hasPath) && (path[0].CatalogNodeId == category.CategoryId))
                {
                    Category pathCategory;
                    int indent = 0;
                    bool hasChildren = (CategoryDataSource.CountForParent(path[path.Count - 1].CatalogNodeId) > 0);
                    bool isTopLevel = (path[path.Count-1].CategoryId == 0);
                    //OUTPUT SUBDIRS ALONG PATH
                    for (int i = 0; i < path.Count; i++)
                    {
                        if (hasChildren || isTopLevel || i < path.Count - 1)
                        {
                            pathCategory = path[i].ChildObject as Category;
                            categoryLink = new LinkButton();
                            categoryLink.ID = "C" + pathCategory.CategoryId.ToString();
                            categoryLink.Text = pathCategory.Name;
                            categoryLink.Click += new EventHandler(categoryLink_Click);
                            phCategoryTree.Controls.Add(new LiteralControl("<div class=\"" + ((i == path.Count - 1) ? "treeNodeSelected" : "treeNodeExpanded") + "\" style=\"padding-left:" + indent.ToString() + "px\">"));
                            phCategoryTree.Controls.Add(new LiteralControl(minusIcon));
                            phCategoryTree.Controls.Add(categoryLink);
                            phCategoryTree.Controls.Add(new LiteralControl("</div>\r\n"));
                            indent += 15;
                        }
                    }
                    int selectedId = path[path.Count - 1].CatalogNodeId;
                    int expandId = ((hasChildren || isTopLevel) ? selectedId : path[path.Count - 1].CategoryId);
                    //ADD ALL CHILDREN OF LAST PATH NODE
                    CategoryCollection childCategories = CategoryDataSource.LoadForParent(expandId, false);
                    foreach (Category childCategory in childCategories)
                    {
                        categoryLink = new LinkButton();
                        categoryLink.ID = "C" + childCategory.CategoryId.ToString();
                        categoryLink.Text = childCategory.Name;
                        categoryLink.Click += new EventHandler(categoryLink_Click);
                        phCategoryTree.Controls.Add(new LiteralControl("<div class=\"" + ((childCategory.CategoryId == selectedId) ? "treeNodeSelected" : "treeNode") + "\" style=\"padding-left:" + indent.ToString() + "px\">"));
                        phCategoryTree.Controls.Add(new LiteralControl((childCategory.CategoryId == selectedId) ? minusIcon : plusIcon));
                        phCategoryTree.Controls.Add(categoryLink);
                        phCategoryTree.Controls.Add(new LiteralControl("</div>\r\n"));
                    }
                }
                else
                {
                    categoryLink = new LinkButton();
                    categoryLink.ID = "C" + category.CategoryId.ToString();
                    categoryLink.Text = category.Name;
                    categoryLink.Click += new EventHandler(categoryLink_Click);
                    phCategoryTree.Controls.Add(new LiteralControl("<div class=\"treeNode\">"));
                    phCategoryTree.Controls.Add(new LiteralControl(plusIcon));
                    phCategoryTree.Controls.Add(categoryLink);
                    phCategoryTree.Controls.Add(new LiteralControl("</div>\r\n"));
                }
            }
            phCategoryTree.Controls.Add(new LiteralControl("</div>\r\n"));
            _LastCategoryId = CurrentCategoryId;
        }
        //
        //if (redrawTree) CategoryTreeView_Redraw();
        //CategoryTreeView_UpdateSelectedCategory();
    }

    void categoryLink_Click(object sender, EventArgs e)
    {
        CurrentCategoryId = AlwaysConvert.ToInt((sender as LinkButton).ID.Substring(1));
    }

    protected void BindCategoryHeader()
    {
        CategoryName.Text = CurrentCategory.Name;
        if (CurrentCategoryId != 0)
        {
            ViewCategory.Visible = true;
            EditCategory.Visible = true;
            ViewCategory.NavigateUrl = UrlGenerator.GetBrowseUrl(CurrentCategoryId, CurrentCategory.Name);
            EditCategory.NavigateUrl = "EditCategory.aspx?CategoryId=" + CurrentCategoryId;
        }
        else
        {
            ViewCategory.Visible = false;
            EditCategory.Visible = false;
        }
            
    }

    protected void BindCGrid()
    {
        ParentCategory.Visible = (CurrentCategoryId != 0);
        ContentsCaption.Text = string.Format(ContentsCaption.Text, CurrentCategory.Name);
        CGrid.DataBind();
    }

    protected void BindBreadCrumbs()
    {
        CategoryBreadCrumbs.CategoryId = CurrentCategoryId;
    }

    protected void BindActionMenu()
    {
        AddCategoryLink.NavigateUrl = "AddCategory.aspx?ParentCategoryId=" + CurrentCategoryId.ToString();
        if (CurrentCategoryId > 0)
        {
            AddProductLink.Visible = true;
            AddProductLink.NavigateUrl += "?CategoryId=" + CurrentCategoryId.ToString();
            AddWebpageLink.Visible = true;
            AddWebpageLink.NavigateUrl += "?CategoryId=" + CurrentCategoryId.ToString();
            AddLinkLink.Visible = true;
            AddLinkLink.NavigateUrl += "?CategoryId=" + CurrentCategoryId.ToString();
        }
        else
        {
            AddProductLink.Visible = false;
            AddWebpageLink.Visible = false;
            AddLinkLink.Visible = false;
        }
    }
#endregion

    protected void SortCategoryButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("SortCategory.aspx?CategoryId=" + CurrentCategoryId.ToString());
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        BulkOptions.Attributes.Add("onchange", "if(!confirmSelection()) return false;");
    }

    protected void CGrid_DataBound(object sender, EventArgs e)
    {
        List<string> categories = new List<string>();
        List<string> products = new List<string>();
        List<string> links = new List<string>();
        List<string> webpages = new List<string>();
        for(int i = 0; i < CGrid.Rows.Count; i++)
        {
            GridViewRow gvr = CGrid.Rows[i];
            CheckBox cb = (CheckBox)gvr.FindControl("Selected");

            DataKey dataKey = ((GridView)sender).DataKeys[gvr.DataItemIndex];            
            int catalogNodeId = (int)dataKey.Values[0];
            CatalogNodeType catalogNodeType = (CatalogNodeType)dataKey.Values[1];
            switch (catalogNodeType)
            {
                case CatalogNodeType.Category:
                    categories.Add(cb.ClientID);
                    break;
                case CatalogNodeType.Product:
                    products.Add(cb.ClientID);
                    break;
                case CatalogNodeType.Link:
                    links.Add(cb.ClientID);
                    break;
                case CatalogNodeType.Webpage:
                    webpages.Add(cb.ClientID);
                    break;
            }
        }
        if (categories.Count > 0)
        {
            string catsArray = "new Array('" + String.Join("','", categories.ToArray()) + "')";
            SelectCategories.OnClientClick = "selectCatalogItems(" + catsArray + ");return false;";
        }
        else
            SelectCategories.OnClientClick = "return false;";

        if (products.Count > 0)
        {
            string productsArray = "new Array('" + String.Join("','", products.ToArray()) + "')";
            SelectProducts.OnClientClick = "selectCatalogItems(" + productsArray + ");return false;";
        }
        else
            SelectProducts.OnClientClick = "return false;";

        if (links.Count > 0)
        {
            string linksArray = "new Array('" + String.Join("','", links.ToArray()) + "')";
            SelectLinks.OnClientClick = "selectCatalogItems(" + linksArray + ");return false;";
        }
        else
            SelectLinks.OnClientClick = "return false;";

        if (webpages.Count > 0)
        {
            string catsArray = "new Array('" + String.Join("','", webpages.ToArray()) + "')";
            SelectWebpages.OnClientClick = "selectCatalogItems(" + catsArray + ");return false;";
        }
        else
            SelectWebpages.OnClientClick = "return false;";
    }

    protected void BulkOptions_SelectedIndexChanged(Object sender, EventArgs e)
    {
        String selectedOption = BulkOptions.SelectedValue;
        if (!String.IsNullOrEmpty(selectedOption))
        {
            List<DataKey> selectedItems = GetSelectedItems();
            if (selectedItems.Count > 0)
            {
                switch (selectedOption)
                {
                    case "Move":
                        Response.Redirect("MoveCatalogObjects.aspx?CategoryId=" + CurrentCategoryId + "&Objects=" + FormatSelectedItems(selectedItems));
                        break;
                    case "Delete":
                        foreach (DataKey item in selectedItems)
                        {
                            DoDelete((CatalogNodeType)item.Values[1], (int)item.Values[0]);
                        }
                        break;
                    case "ChangeVisibility":
                        Response.Redirect("ChangeVisibility.aspx?CategoryId=" + CurrentCategoryId + "&Objects=" + FormatSelectedItems(selectedItems));
                        break;
                }
            }
        }
        BulkOptions.SelectedIndex = 0;

        // DETERMINE IF THE CURRENT PAGE INDEX IS TOO FAR
        if (CGrid.PageIndex > 0)
        {
            decimal pageSize = (decimal)CGrid.PageSize;
            decimal totalObjects = (decimal)CatalogNodeDataSource.CountForCategory(CurrentCategoryId);
            int lastPageIndex = (int)Math.Ceiling(totalObjects / pageSize) - 1;
            if (CGrid.PageIndex > lastPageIndex)
            {
                // ALL ITEMS ON CURRENT PAGE > 1 WERE DELETED
                // SET TO LAST CALCULATED PAGE INDEX
                CGrid.PageIndex = lastPageIndex;
            }
        }
    }

    private string FormatSelectedItems(List<DataKey> selectedItems)
    {
        StringBuilder itemsBuilder = new StringBuilder();
        for(int i =0; i < selectedItems.Count; i++)
        {
            DataKey item = selectedItems[i];
            // APPEND IN FORMAT "CatalogNodeId:CatalogNodeType"
            if (i == selectedItems.Count - 1) itemsBuilder.AppendFormat("{0}:{1}", item.Values[0], (byte)item.Values[1]);
            else itemsBuilder.AppendFormat("{0}:{1},", item.Values[0], (byte)item.Values[1]);            
        }
        return itemsBuilder.ToString();
    }

    protected List<DataKey> GetSelectedItems()
    {        

        List<DataKey> selectedItems = new List<DataKey>();
        foreach (GridViewRow row in CGrid.Rows)
        {
            CheckBox selected = (CheckBox)PageHelper.RecursiveFindControl(row, "Selected");
            if ((selected != null) && selected.Checked)
            {
                selectedItems.Add(CGrid.DataKeys[row.DataItemIndex]);
            }
        }
        return selectedItems;
    }
}
