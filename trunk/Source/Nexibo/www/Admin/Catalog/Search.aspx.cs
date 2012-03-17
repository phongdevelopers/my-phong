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

public partial class Admin_Catalog_Search : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private string _IconPath = string.Empty;

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

    protected void BindSearchValidationScript()
    {
        StringBuilder scriptBuilder = new StringBuilder();
        scriptBuilder.Append("if(!(").Append(IncludeCategories.ClientID).Append(".checked ");
        scriptBuilder.Append("||").Append(IncludeProducts.ClientID).Append(".checked ");
        scriptBuilder.Append("||").Append(IncludeWebpages.ClientID).Append(".checked ");
        scriptBuilder.Append("||").Append(IncludeLinks.ClientID).Append(".checked)){");
        scriptBuilder.Append("alert('At least one catalog node type must be included in search.');");
        scriptBuilder.Append(" return false;}");

        scriptBuilder.Append("else this.value='Searching...';");
        Search.OnClientClick = scriptBuilder.ToString();
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        string theme = Page.Theme;
        if (string.IsNullOrEmpty(theme)) theme = Page.StyleSheetTheme;
        if (string.IsNullOrEmpty(theme)) theme = "AbleCommerceAdmin";
        _IconPath = Page.ResolveUrl("~/App_Themes/" + theme + "/Images/Icons/");
        CategoryName.Text = CurrentCategory.Name;
        CategoryBreadCrumbs.CategoryId = CurrentCategoryId;
        //REGISTER DELETE SCRIPTS
        StringBuilder script = new StringBuilder();
        script.Append("function CD1(n) { return confirm(\"Are you sure you want to delete \" + n + \"?\") }\r\n");
        script.Append("function CD2(n) { return confirm(\"Are you sure you want to delete \" + n + \" and ALL IT'S CONTENTS?\") }\r\n");
        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ConfirmDelete", script.ToString(), true);
        //LOAD SEARCH VALUES ON INITIAL VISIT
        if (!Page.IsPostBack)
        {
            SearchPhrase.Text = Request.QueryString["k"];
            TitlesOnly.Checked = (AlwaysConvert.ToInt(Request.QueryString["titles"], 1) == 1);
            Recursive.Checked = (AlwaysConvert.ToInt(Request.QueryString["recurse"], 1) == 1);
            if (!string.IsNullOrEmpty(Request.QueryString["types"]))
            {
                string[] types = Request.QueryString["types"].Split(",".ToCharArray());
                IncludeCategories.Checked = (Array.IndexOf(types, "0") > -1);
                IncludeProducts.Checked = (Array.IndexOf(types, "1") > -1);
                IncludeWebpages.Checked = (Array.IndexOf(types, "2") > -1);
                IncludeLinks.Checked = (Array.IndexOf(types, "3") > -1);
            }
            CGrid.Visible = (!string.IsNullOrEmpty(SearchPhrase.Text));
        }
        else CGrid.Visible = true;
        //HIDE RECURSE
        if (CurrentCategoryId == 0) Recursive.Visible = false;
        BindSearchValidationScript();
    }

    protected void CGrid_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        if (e.CommandName.StartsWith("Do_"))
        {
            string[] args = e.CommandArgument.ToString().Split("|".ToCharArray());
            CatalogNodeType catalogNodeType = (CatalogNodeType)AlwaysConvert.ToInt(args[0]);
            int catalogNodeId = AlwaysConvert.ToInt(args[1]);
            switch (e.CommandName)
            {
                case "Do_Open":
                    //IF CATEGORY, REBIND CURRENT PAGE,
                    //OTHERWISE REDIRECT TO CORRECT EDIT PAGE
                    switch (catalogNodeType)
                    {
                        case CatalogNodeType.Category:
                            //REBIND CURRENT CATEGORY
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
                    // THIS WILL COPY THE PRODUCTS WITH THE ORIGNAL CATEGORY INFORMATION PRESERVED
                    CatalogDataSource.Copy(catalogNodeId, catalogNodeType, 0);
                    break;
                case "Do_Delete":
                    DoDelete(catalogNodeType, catalogNodeId);
                    break;
                case "Do_Pub":
                    ICatalogable pubNode = CatalogDataSource.Load(catalogNodeId, catalogNodeType);
                    if (pubNode != null)
                    {
                        //FOR CATEGORIES, WE MUST FIND OUT MORE INFORMATION
                        if (pubNode is Category)
                            Response.Redirect("ChangeVisibility.aspx?CategoryId=" + ((Category)pubNode).ParentId.ToString() + String.Format("&Objects={0}:{1}", ((Category)pubNode).CategoryId, (byte)(CatalogNodeType.Category)));
                        //FOR OTHER OBJECTS, WE CAN ADJUST
                        switch (pubNode.Visibility)
                        {
                            case CatalogVisibility.Public:
                                pubNode.Visibility = CatalogVisibility.Hidden;
                                break;
                            case CatalogVisibility.Hidden:
                                pubNode.Visibility = CatalogVisibility.Private;
                                break;
                            default:
                                pubNode.Visibility = CatalogVisibility.Public;
                                break;
                        }
                        if (pubNode is Product)
                            ((Product)pubNode).Save();
                        else if (pubNode is Webpage)
                            ((Webpage)pubNode).Save();
                        else if (pubNode is Link)
                            ((Link)pubNode).Save();
                    }
                    break;
            }
        }
        CGrid.DataBind();
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
                break;
            case CatalogNodeType.Product:
                Product product = ProductDataSource.Load(catalogNodeId);
                if (product != null) product.Delete();
                break;
            case CatalogNodeType.Webpage:
                Webpage webpage = WebpageDataSource.Load(catalogNodeId);
                if ((webpage != null) && (webpage.Categories.Count < 2)) webpage.Delete();
                break;
            case CatalogNodeType.Link:
                Link link = LinkDataSource.Load(catalogNodeId);
                if ((link != null) && (link.Categories.Count < 2)) link.Delete();
                break;
        }
        CGrid.DataBind();
    }

    protected void CatalogDs_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        e.InputParameters["categoryId"] = CurrentCategoryId;
        //FILTER THE NODE TYPES
        CatalogNodeTypeFlags catalogNodeTypes = CatalogNodeTypeFlags.None;
        if (IncludeCategories.Checked) catalogNodeTypes = catalogNodeTypes | CatalogNodeTypeFlags.Category;
        if (IncludeProducts.Checked) catalogNodeTypes = catalogNodeTypes | CatalogNodeTypeFlags.Product;
        if (IncludeWebpages.Checked) catalogNodeTypes = catalogNodeTypes | CatalogNodeTypeFlags.Webpage;
        if (IncludeLinks.Checked) catalogNodeTypes = catalogNodeTypes | CatalogNodeTypeFlags.Link;
        e.InputParameters["catalogNodeTypes"] = catalogNodeTypes;
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
    protected string GetMoveUrl(object dataItem, object dataItemId)
    {
        CatalogNodeType nodeType = (CatalogNodeType)dataItem;
        int catalogNodeId = (int)dataItemId;
        string url = "~/Admin/Catalog/MoveCatalogObjects.aspx";
        url = Page.ResolveUrl(url);
        switch (nodeType)
        {
            case CatalogNodeType.Category:
                url += "?CategoryId=" + this.CurrentCategoryId.ToString() + string.Format("&Objects={0}:{1}",catalogNodeId,(byte)CatalogNodeType.Category);
                break;
            case CatalogNodeType.Product:
                url += "?CategoryId=" + this.CurrentCategoryId.ToString() + string.Format("&Objects={0}:{1}",catalogNodeId,(byte)CatalogNodeType.Product);
                break;
            case CatalogNodeType.Webpage:
                url += "?CategoryId=" + this.CurrentCategoryId.ToString() + string.Format("&Objects={0}:{1}", catalogNodeId, (byte)CatalogNodeType.Webpage);
                break;
            case CatalogNodeType.Link:
                url += "?CategoryId=" + this.CurrentCategoryId.ToString() + string.Format("&Objects={0}:{1}", catalogNodeId, (byte)CatalogNodeType.Link);
                break;
            default:
                url = "~/Admin/Catalog/Browse.aspx";
                break;
        }
        return url;
    }
    protected void Page_PreRender(object sender, EventArgs e)
    {
        BindCurrentCategory();
    }

    protected void BindCurrentCategory()
    {
        BindCategoryHeader();
        BindBreadCrumbs();
        BindCGrid();
        LastCategory.Value = CurrentCategoryId.ToString();
    }

    protected void BindCategoryHeader()
    {
        CategoryName.Text = CurrentCategory.Name;
    }

    protected void BindBreadCrumbs()
    {
        CategoryBreadCrumbs.CategoryId = CurrentCategoryId;
    }

    protected void BindCGrid()
    {
        ContentsCaption.Text = string.Format(ContentsCaption.Text, CurrentCategory.Name);
        CGrid.DataBind();
    }

}
