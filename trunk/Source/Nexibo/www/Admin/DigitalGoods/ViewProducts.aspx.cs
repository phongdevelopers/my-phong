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
using CommerceBuilder.DigitalDelivery;
using CommerceBuilder.Utility;
using CommerceBuilder.Products;

public partial class Admin_DigitalGoods_ViewProducts : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private int _DigitalGoodId;
    private DigitalGood _DigitalGood;
    protected void Page_Load(object sender, EventArgs e)
    {
        _DigitalGoodId = AlwaysConvert.ToInt(Request.QueryString["DigitalGoodId"]);
        _DigitalGood = DigitalGoodDataSource.Load(_DigitalGoodId);
        if (_DigitalGood == null) Response.Redirect("Default.aspx");
        Caption.Text = string.Format(Caption.Text, _DigitalGood.Name);
        
    }   

    protected void SearchButton_Click(object sender, EventArgs e)
    {
        SearchResultsGrid.Visible = true;
        SearchResultsGrid.PageIndex = 0;
        SearchResultsGrid.DataBind();
    }

    protected void AttachButton_Click(object sender, EventArgs e)
    {
        ImageButton attachButton = (ImageButton)sender;
        int dataItemIndex = AlwaysConvert.ToInt(attachButton.CommandArgument);
        GridView grid = SearchResultsGrid;
        dataItemIndex = (dataItemIndex - (grid.PageSize * grid.PageIndex));
        int productId = (int)grid.DataKeys[dataItemIndex].Value;
        AssociateProduct(productId);
        ImageButton removeButton = attachButton.Parent.FindControl("RemoveButton") as ImageButton;
        if (removeButton != null) removeButton.Visible = true;
        attachButton.Visible = false;        

        RelatedProductGrid.DataBind();
    }

    protected void RemoveButton_Click(object sender, EventArgs e)
    {
        ImageButton removeButton = (ImageButton)sender;
        int dataItemIndex = AlwaysConvert.ToInt(removeButton.CommandArgument);
        GridView grid = SearchResultsGrid;
        dataItemIndex = (dataItemIndex - (grid.PageSize * grid.PageIndex));
        int productId = (int)grid.DataKeys[dataItemIndex].Value;
        ReleaseProduct(productId);
        ImageButton attachButton = removeButton.Parent.FindControl("AttachButton") as ImageButton;
        if (attachButton != null) attachButton.Visible = true;
        removeButton.Visible = false;        

        RelatedProductGrid.DataBind();
    }

    private void AssociateProduct(int relatedProductId)
    {
        Product product = ProductDataSource.Load(relatedProductId);
        if (product != null)
        {
            if(!IsProductAssciated(product))
            {
                ProductDigitalGood pgd = new ProductDigitalGood();
                pgd.ProductId = product.ProductId;
                pgd.DigitalGoodId = _DigitalGoodId;
                pgd.Save();
                _DigitalGood.ProductDigitalGoods.Add(pgd);
            }
        }
    }

    private void ReleaseProduct(int relatedProductId)
    {
        Product product = ProductDataSource.Load(relatedProductId);
        if (product != null)
        {
            int index = -1;
            for (int i = 0; i < _DigitalGood.ProductDigitalGoods.Count; i++ )
            {
                ProductDigitalGood pgd = _DigitalGood.ProductDigitalGoods[i];
                if (pgd.DigitalGoodId == _DigitalGoodId && pgd.ProductId == relatedProductId)
                {
                    index = i;
                    break;
                }
            }
            if (index > -1)
            {
                _DigitalGood.ProductDigitalGoods.DeleteAt(index);
                _DigitalGood.ProductDigitalGoods.Save();
            }
        }
    }

    protected bool IsProductAssciated(Product product)
    {
        if (product != null)
        {
            foreach (ProductDigitalGood pgd in product.DigitalGoods)
            {
                if (pgd.DigitalGoodId == _DigitalGoodId) return true;
            }
        }
        return false;
    }

    protected void ProductSearchDs_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        SearchResultsGrid.Columns[0].Visible = ShowImages.Checked;        
    }

    private void RedirectToEdit()
    {
        Response.Redirect("Default.aspx");
    }


    protected void RelatedProductGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int relatedProductId = (int)e.Keys[0];
        ReleaseProduct(relatedProductId);
        //CHECK THE SEARCH RESULTS GRID TO SEE IF THIS ITEMS APPEARS
        int tempIndex = 0;
        foreach (DataKey key in SearchResultsGrid.DataKeys)
        {
            int tempId = (int)key.Value;
            if (relatedProductId == tempId)
            {
                //CHANGE THE REMOVE BUTTON TO ADD FOR THIS ROW
                ImageButton removeButton = SearchResultsGrid.Rows[tempIndex].FindControl("RemoveButton") as ImageButton;
                if (removeButton != null) removeButton.Visible = false;
                ImageButton attachButton = SearchResultsGrid.Rows[tempIndex].FindControl("AttachButton") as ImageButton;
                if (attachButton != null) attachButton.Visible = true;
                break;
            }
            tempIndex++;
        }
        RelatedProductGrid.DataBind();
        e.Cancel = true;
    }

    protected void RemoveButton2_Click(object sender, ImageClickEventArgs e)
    {
        ImageButton removeButton = (ImageButton)sender;
        int productId = AlwaysConvert.ToInt(removeButton.CommandArgument);
        ReleaseProduct(productId);
        RelatedProductGrid.DataBind();
        if (SearchResultsGrid.Visible) SearchResultsGrid.DataBind();

    }
}
