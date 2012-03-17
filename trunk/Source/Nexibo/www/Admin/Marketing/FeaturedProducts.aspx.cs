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
using CommerceBuilder.Utility;
using CommerceBuilder.Products;
using CommerceBuilder.Common;

public partial class Admin_Marketing_FeaturedProducts : AbleCommerceAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PageHelper.SetDefaultButton(SearchName, SearchButton.ClientID);
    }

    protected void SearchButton_Click(object sender, EventArgs e)
    {
        SearchResultsGrid.Visible = true;
        RebindPage();
    }

    protected void AttachButton_Click(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        int dataItemIndex = AlwaysConvert.ToInt(button.CommandArgument);
        GridView grid = SearchResultsGrid;
        dataItemIndex = (dataItemIndex - (grid.PageSize * grid.PageIndex));
        int productId = (int)grid.DataKeys[dataItemIndex].Value;
        SetFeatured(productId, true);
        RebindPage();
    }

    protected void RemoveButton_Click(object sender, EventArgs e)
    {
        int dataItemIndex;
        if (sender is Button) dataItemIndex = AlwaysConvert.ToInt(((Button)sender).CommandArgument);
        else dataItemIndex = AlwaysConvert.ToInt(((ImageButton)sender).CommandArgument);
        GridView grid = (GridView)((GridViewRow)((Control)sender).NamingContainer).NamingContainer;
        dataItemIndex = (dataItemIndex - (grid.PageSize * grid.PageIndex));
        int productId = (int)grid.DataKeys[dataItemIndex].Value;
        SetFeatured(productId, false);
        RebindPage();
    }

    private void SetFeatured(int productId, bool featured)
    {
        Product product = ProductDataSource.Load(productId);
        if (product != null)
        {
            product.IsFeatured = featured;
            product.Save();
        }
    }

    private void RebindPage()
    {
        ProductGrid.DataBind();
        SearchResultsGrid.DataBind();
    }

    protected bool IsProductFeatured(int productId)
    {
        Product product = ProductDataSource.Load(productId);
        if (product != null) return product.IsFeatured;
        return false;
    }

    protected void ProductSearchDs_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        string pattern = SearchName.Text.Trim();
        if (string.IsNullOrEmpty(pattern)) pattern = "*";
        //IMPLEMENT A SUBSTRING MATCH UNLESS OTHERWISE SPECIFIED
        if ((!pattern.Contains("*")) && (!pattern.Contains("?"))) pattern = "*" + pattern + "*";
        e.InputParameters["name"] = pattern;
        switch (FeaturedFilter.SelectedIndex)
        {
            case 1: e.InputParameters["featured"] = BitFieldState.False; break;
            case 2: e.InputParameters["featured"] = BitFieldState.True; break;
            default: e.InputParameters["featured"] = BitFieldState.Any; break;
        }
        SearchResultsGrid.Columns[1].Visible = ShowImages.Checked;
    }


    protected void ProductGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if ((e.Row != null) && (e.Row.RowType == DataControlRowType.DataRow) && (e.Row.DataItem != null))
        {
            Product product = (Product)e.Row.DataItem;
            ImageButton imgButton = PageHelper.RecursiveFindControl(e.Row, "RemoveButton2") as ImageButton;
            if (imgButton != null)
            {
                string confirmationJS = String.Format("return confirm('Are you sure you want to remove \"{0}\" from featured products?');", product.Name);
                imgButton.Attributes.Add("onclick", confirmationJS);
            }
        }        
    }
}
