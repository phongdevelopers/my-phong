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
using CommerceBuilder.Web.UI;
using CommerceBuilder.Marketing;
using CommerceBuilder.Utility;
using CommerceBuilder.Catalog;
using System.Collections.Generic;
using ComponentArt.Web.UI;

public partial class Admin_Marketing_Discounts_EditDiscountScope : AbleCommerceAdminPage
{
    private VolumeDiscount _VolumeDiscount;
    private int _VolumeDiscountId = 0;

    protected void Page_Init(object sender, EventArgs e)
    {
        _VolumeDiscountId = AlwaysConvert.ToInt(Request.QueryString["VolumeDiscountId"]);
        _VolumeDiscount = VolumeDiscountDataSource.Load(_VolumeDiscountId);
        if (_VolumeDiscount == null) Response.Redirect("Default.aspx");
        Caption.Text = string.Format(Caption.Text, _VolumeDiscount.Name);
        if (!Page.IsPostBack) InitCategoryTree();
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        PageHelper.SetDefaultButton(SearchName, SearchButton.ClientID);
    }

    private void InitCategoryTree()
    {
        //CREATE DUMMY NODE TO POPULATE ROOT
        ComponentArt.Web.UI.TreeViewNode rootNode = new ComponentArt.Web.UI.TreeViewNode();
        rootNode.Text = "Global Discount";
        rootNode.ID = "0";
        PopulateAndExpandCategoryNode(0, rootNode);
        //ADD TOP LEVEL CATEGORIES TO TREE
        foreach (TreeViewNode node in rootNode.Nodes)
        {
            CategoryTree.Nodes.Add(node);
        }
        //NOW EXPAND/SELECT ANY LINKED CATEGORIES
        foreach (CategoryVolumeDiscount cvd in _VolumeDiscount.CategoryVolumeDiscounts)
        {
            SelectCategoryNode(cvd.CategoryId);
        }
    }

    protected void SelectCategoryNode(int categoryId)
    {
        List<CatalogPathNode> path = CatalogDataSource.GetPath(categoryId, false);
        int pathIndex = 1;
        int pathCount = path.Count;
        foreach (CatalogPathNode pathNode in path)
        {
            ComponentArt.Web.UI.TreeViewNode treeNode = CategoryTree.FindNodeById(pathNode.CatalogNodeId.ToString());
            if (treeNode != null)
            {
                if (pathIndex == pathCount) treeNode.Checked = true;
                else if (!treeNode.Expanded)
                {
                    PopulateAndExpandCategoryNode(pathNode.CatalogNodeId, treeNode);
                }
            }
            pathIndex++;
        }
    }

    protected void PopulateAndExpandCategoryNode(int categoryId, ComponentArt.Web.UI.TreeViewNode parentNode)
    {
        CategoryCollection children = CategoryDataSource.LoadForParent(categoryId, false);
        foreach (Category child in children)
        {
            ComponentArt.Web.UI.TreeViewNode newNode = new ComponentArt.Web.UI.TreeViewNode();
            newNode.Text = child.Name;
            newNode.ID = child.CategoryId.ToString();
            if (CategoryDataSource.CountForParent(child.CategoryId) > 0)
                newNode.ContentCallbackUrl = "GetCategories.ashx?CategoryId=" + newNode.ID + "&VolumeDiscountId=" + _VolumeDiscountId.ToString();
            newNode.ShowCheckBox = true;
            parentNode.Nodes.Add(newNode);
        }
        parentNode.ContentCallbackUrl = string.Empty;
        parentNode.Expanded = true;
    }

    protected void SearchButton_Click(object sender, EventArgs e)
    {
        SearchResultsGrid.Visible = true;
        SearchResultsGrid.PageIndex = 0;
        SearchResultsGrid.DataBind();
    }

    protected void AttachButton_Click(object sender, EventArgs e)
    {
        Button attachButton = (Button)sender;
        int dataItemIndex = AlwaysConvert.ToInt(attachButton.CommandArgument);
        GridView grid = SearchResultsGrid;
        dataItemIndex = (dataItemIndex - (grid.PageSize * grid.PageIndex));
        int productId = (int)grid.DataKeys[dataItemIndex].Value;
        SetLink(productId, true);
        Button removeButton = attachButton.Parent.FindControl("RemoveButton") as Button;
        if (removeButton != null) removeButton.Visible = true;
        attachButton.Visible = false;
        ProductGrid.DataBind();
    }

    protected void RemoveButton_Click(object sender, EventArgs e)
    {
        Button removeButton = (Button)sender;
        int dataItemIndex = AlwaysConvert.ToInt(removeButton.CommandArgument);
        GridView grid = SearchResultsGrid;
        dataItemIndex = (dataItemIndex - (grid.PageSize * grid.PageIndex));
        int productId = (int)grid.DataKeys[dataItemIndex].Value;
        SetLink(productId, false);
        Button attachButton = removeButton.Parent.FindControl("AttachButton") as Button;
        if (attachButton != null) attachButton.Visible = true;
        removeButton.Visible = false;
        ProductGrid.DataBind();
    }

    protected void RemoveImageButton_Click(object sender, EventArgs e)
    {
        ImageButton removeImageButton = (ImageButton)sender;
        int dataItemIndex = AlwaysConvert.ToInt(removeImageButton.CommandArgument);
        dataItemIndex = (dataItemIndex - (ProductGrid.PageSize * ProductGrid.PageIndex));
        int productId = (int)ProductGrid.DataKeys[dataItemIndex].Value;
        SetLink(productId, false);
        //CHECK THE SEARCH RESULTS GRID TO SEE IF THIS ITEMS APPEARS
        int tempIndex = 0;
        foreach (DataKey key in SearchResultsGrid.DataKeys)
        {
            int tempId = (int)key.Value;
            if (productId == tempId)
            {
                //CHANGE THE REMOVE BUTTON TO ADD FOR THIS ROW
                Button removeButton = SearchResultsGrid.Rows[tempIndex].FindControl("RemoveButton") as Button;
                if (removeButton != null) removeButton.Visible = false;
                Button attachButton = SearchResultsGrid.Rows[tempIndex].FindControl("AttachButton") as Button;
                if (attachButton != null) attachButton.Visible = true;
                break;
            }
            tempIndex++;
        }
        ProductGrid.DataBind();
    }

    private void SetLink(int productId, bool linked)
    {
        int index = _VolumeDiscount.ProductVolumeDiscounts.IndexOf(productId, _VolumeDiscountId);
        if (linked && (index < 0))
        {
            _VolumeDiscount.ProductVolumeDiscounts.Add(new ProductVolumeDiscount(productId, _VolumeDiscountId));
            _VolumeDiscount.Save();
        }
        else if (!linked && (index > -1))
        {
            _VolumeDiscount.ProductVolumeDiscounts.DeleteAt(index);
        }
    }

    protected bool IsProductLinked(int productId)
    {
        return (_VolumeDiscount.ProductVolumeDiscounts.IndexOf(productId, _VolumeDiscountId) > -1);
    }

    protected void ProductSearchDs_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        string pattern = SearchName.Text.Trim();
        if (string.IsNullOrEmpty(pattern)) pattern = "*";
        //IMPLEMENT A SUBSTRING MATCH UNLESS OTHERWISE SPECIFIED
        if ((!pattern.Contains("*")) && (!pattern.Contains("?"))) pattern = "*" + pattern + "*";
        e.InputParameters["name"] = pattern;
        SearchResultsGrid.Columns[2].Visible = ShowImages.Checked;
    }

    private void RedirectToEdit()
    {
        if (!string.IsNullOrEmpty(Request.QueryString["Edit"]))
        {
            Response.Redirect("EditDiscount.aspx?VolumeDiscountId=" + _VolumeDiscountId.ToString());
        }
        Response.Redirect("Default.aspx");
    }

    protected void FinishButton_Click(object sender, EventArgs e)
    {
        //CLEAR OUT EXISTING ASSIGNMENTS
        _VolumeDiscount.CategoryVolumeDiscounts.DeleteAll();
        foreach (TreeViewNode node in CategoryTree.CheckedNodes)
        {
            int categoryId = AlwaysConvert.ToInt(node.ID);
            CategoryVolumeDiscount cp = new CategoryVolumeDiscount(categoryId, _VolumeDiscountId);
            _VolumeDiscount.CategoryVolumeDiscounts.Add(cp);
        }
        _VolumeDiscount.Save();
        RedirectToEdit();
    }
}
