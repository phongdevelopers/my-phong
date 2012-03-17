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
using System.Text;
using System.IO;

public partial class Admin_Products_EditSimilarProducts : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private Product _Product;
    private int _ProductId = 0;
    private string _IconPath = string.Empty;

    protected void Page_Init(object sender, EventArgs e)
    {
        _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        _Product = ProductDataSource.Load(_ProductId);
        if (_Product == null) Response.Redirect(NavigationHelper.GetAdminUrl("Catalog/Browse.aspx"));
        Caption.Text = string.Format(Caption.Text, _Product.Name);
        string theme = Page.Theme;
        _IconPath = Page.ResolveUrl("~/App_Themes/" + theme + "/Images/Icons/");
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            InitializeCategoryTree();
        }
        
        InItIcons();
        
        if (!Page.IsPostBack)
        {
            SearchButton_Click(null, null);
        }
    }

    protected void SearchButton_Click(object sender, EventArgs e)
    {
        SearchResultsGrid.Visible = true;
        SearchResultsGrid.PageIndex = 0;
        SearchResultsGrid.DataBind();
    }
    
    protected void ProductSearchDs_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        CrossSellState csf = (CrossSellState)AlwaysConvert.ToByte(Filter.SelectedValue);
        e.InputParameters.Add("crossSellFilter", csf);
        SearchResultsGrid.Columns[2].Visible = ShowImages.Checked;
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        GridView grid = SearchResultsGrid;
        foreach (GridViewRow row in grid.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                int dataItemIndex = row.DataItemIndex;
                dataItemIndex = (dataItemIndex - (grid.PageSize * grid.PageIndex));
                int productId = (int)grid.DataKeys[dataItemIndex].Value;
                Product product = ProductDataSource.Load(productId);
                if (product == null)
                    continue;
                LinkButton crossLinkButton = (LinkButton)grid.Rows[row.RowIndex].FindControl("CrossSellLink");
                HiddenField hdnState = (HiddenField)crossLinkButton.FindControl("CrossSellState");
                int state = AlwaysConvert.ToInt(hdnState.Value);
                switch (state)
                { 
                    case 0:
                        AdjustCrossSellLinking(_ProductId, product.ProductId, CrossSellState.Unlinked);
                        break;
                    case 1:
                        AdjustCrossSellLinking(_ProductId, product.ProductId, CrossSellState.CrossLinked);
                        break;
                    case 2:
                        AdjustCrossSellLinking(_ProductId, product.ProductId, CrossSellState.LinksTo);
                        break;
                    case 3:
                        AdjustCrossSellLinking(_ProductId, product.ProductId, CrossSellState.LinkedFrom);
                        break;
                }
            }
        }
        grid.DataBind();
    }

    protected void AdjustCrossSellLinking(int firstProductId,int secondProductId,CrossSellState crossSellState) 
    {
        RelatedProduct toReleatedProduct = RelatedProductDataSource.Load(firstProductId, secondProductId);
        RelatedProduct fromReleatedProduct = RelatedProductDataSource.Load(secondProductId, firstProductId);
        switch (crossSellState)
        { 
            case CrossSellState.Unlinked:
                
                if (toReleatedProduct != null)
                    toReleatedProduct.Delete();
                    
                if (fromReleatedProduct != null)
                    fromReleatedProduct.Delete();
                    
                break;
            case CrossSellState.CrossLinked:

                if (toReleatedProduct == null)
                {
                    toReleatedProduct = new RelatedProduct(firstProductId, secondProductId);
                    toReleatedProduct.Save();
                }

                if (fromReleatedProduct == null)
                {
                    fromReleatedProduct = new RelatedProduct(secondProductId, firstProductId);
                    fromReleatedProduct.Save();
                }

                break;
            case CrossSellState.LinksTo:
                if (toReleatedProduct == null)
                {
                    toReleatedProduct = new RelatedProduct(firstProductId, secondProductId);
                    toReleatedProduct.Save();
                }
                if (fromReleatedProduct != null)
                    fromReleatedProduct.Delete();
                break;
            case CrossSellState.LinkedFrom:
                if (toReleatedProduct != null)
                    toReleatedProduct.Delete();
                if (fromReleatedProduct == null)
                {
                    fromReleatedProduct = new RelatedProduct(secondProductId, firstProductId);
                    fromReleatedProduct.Save();
                }
                break;
        }
    }

    protected void InitializeCategoryTree()
    {
        CategoryLevelNodeCollection categories = CategoryParentDataSource.GetCategoryLevels(0, true);
        foreach (CategoryLevelNode node in categories)
        {
            string prefix = string.Empty;
            for (int i = 0; i <= node.CategoryLevel; i++) prefix += " . . ";
            CategoryList.Items.Add(new ListItem(prefix + node.Name, node.CategoryId.ToString()));
        }
    }

    protected void SearchResultsGrid_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Product product = (Product)e.Row.DataItem;
            if (product == null)
                return;

            LinkButton hyperlink = (LinkButton)e.Row.FindControl("CrossSellLink");
            ImageButton imageButton = (ImageButton)hyperlink.FindControl("CrossSellIcon");
            HiddenField hdnState = (HiddenField)hyperlink.FindControl("CrossSellState");
            
            CrossSellState crossSellState = GetCrossSellState(_ProductId, product.ProductId);
            imageButton.ImageUrl = GetIcon(crossSellState);

            switch (crossSellState)
            {
                case CrossSellState.Linked:
                case CrossSellState.CrossLinked:
                    hdnState.Value = "1";
                    break;
                case CrossSellState.LinksTo:
                    hdnState.Value = "2";
                    break;
                case CrossSellState.LinkedFrom:
                    hdnState.Value = "3";
                    break;
                case CrossSellState.Unlinked:
                    hdnState.Value = "0";
                    break;
            }
        }
    }

    protected void InItIcons() 
    {
        string noLinkImage = GetIcon(CrossSellState.Unlinked);
        string crossLinkedImage = GetIcon(CrossSellState.CrossLinked);
        string toLinkImage = GetIcon(CrossSellState.LinksTo);
        string fromLinkImage = GetIcon(CrossSellState.LinkedFrom);
        string iconImages = string.Format("'{0}','{1}','{2}','{3}'",noLinkImage,crossLinkedImage,toLinkImage,fromLinkImage);
        ScriptManager.RegisterArrayDeclaration(Page,"iconImages",iconImages);
        
        UnlinkedIconLegend.ImageUrl = noLinkImage;
        LinksToIconLegend.ImageUrl = toLinkImage;
        LinksFromIconLegend.ImageUrl = fromLinkImage;
        CrossLinkedIconLegend.ImageUrl = crossLinkedImage;
    }

    protected CrossSellState GetCrossSellState(int firstProductId, int secondProductId)
    {
        int masterCount = RelatedProductDataSource.GetRelatedCount(firstProductId, secondProductId);
        int childCount = RelatedProductDataSource.GetRelatedCount(secondProductId, firstProductId);

        CrossSellState crossSellState = CrossSellState.Unlinked;

        if (masterCount > 0 && childCount > 0)
            crossSellState = CrossSellState.CrossLinked;
        else
            if (masterCount > 0 && childCount == 0)
                crossSellState = CrossSellState.LinksTo;
            else
                if (masterCount == 0 && childCount > 0)
                    crossSellState = CrossSellState.LinkedFrom;
        return crossSellState;
    }

    protected string GetIcon(CrossSellState crossSellState) 
    {
        string fullIconPath = string.Empty;
        switch (crossSellState)
        { 
            case CrossSellState.Linked:
            case CrossSellState.CrossLinked:
                fullIconPath = _IconPath + "green_both_arrow.png";
                break;

            case CrossSellState.LinksTo:
                fullIconPath = _IconPath + "green_right_arrow.png";
                break;

            case CrossSellState.LinkedFrom:
                fullIconPath = _IconPath + "green_left_arrow.png";
                break;
            case CrossSellState.Unlinked:
                fullIconPath = _IconPath + "green_dashes.png";
                break;
        }
        return fullIconPath;
    }
}
