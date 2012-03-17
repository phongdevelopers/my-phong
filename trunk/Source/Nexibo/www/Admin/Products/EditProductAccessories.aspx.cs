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

public partial class Admin_Products_EditProductAccessories : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private Product _Product;
    private int _ProductId = 0;
    
    protected void Page_Init(object sender, EventArgs e)
    {
        _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        _Product = ProductDataSource.Load(_ProductId);
        if (_Product == null) Response.Redirect(NavigationHelper.GetAdminUrl("Catalog/Browse.aspx"));
        Caption.Text = string.Format(Caption.Text, _Product.Name);
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
        SetLink(productId, true);
        ImageButton removeButton = attachButton.Parent.FindControl("RemoveButton") as ImageButton;
        if (removeButton != null) removeButton.Visible = true;
        attachButton.Visible = false;
        UpsellProductGrid.DataBind();
    }

    protected void RemoveButton_Click(object sender, EventArgs e)
    {
        ImageButton removeButton = (ImageButton)sender;
        int dataItemIndex = AlwaysConvert.ToInt(removeButton.CommandArgument);
        GridView grid = SearchResultsGrid;
        dataItemIndex = (dataItemIndex - (grid.PageSize * grid.PageIndex));
        int productId = (int)grid.DataKeys[dataItemIndex].Value;
        SetLink(productId, false);
        ImageButton attachButton = removeButton.Parent.FindControl("AttachButton") as ImageButton;
        if (attachButton != null) attachButton.Visible = true;
        removeButton.Visible = false;
        UpsellProductGrid.DataBind();
    }
    
    protected void UpsellProductGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "MoveUp")
        {
            UpsellProductCollection UpsellProducts = _Product.UpsellProducts;
            int itemIndex = AlwaysConvert.ToInt(e.CommandArgument);
            if ((itemIndex < 1) || (itemIndex > UpsellProducts.Count - 1)) return;
            UpsellProduct selectedItem = UpsellProducts[itemIndex];
            UpsellProduct swapItem = UpsellProducts[itemIndex - 1];
            UpsellProducts.RemoveAt(itemIndex - 1);
            UpsellProducts.Insert(itemIndex, swapItem);
            for (int i = 0; i < UpsellProducts.Count; i++)
            {
                UpsellProducts[i].OrderBy = (short)i;
            }
            UpsellProducts.Save();
            UpsellProductGrid.DataBind();
        }
        else if (e.CommandName == "MoveDown")
        {
            UpsellProductCollection UpsellProducts = _Product.UpsellProducts;
            int itemIndex = AlwaysConvert.ToInt(e.CommandArgument);
            if ((itemIndex > UpsellProducts.Count - 2) || (itemIndex < 0)) return;
            UpsellProduct selectedItem = UpsellProducts[itemIndex];
            UpsellProduct swapItem = UpsellProducts[itemIndex + 1];
            UpsellProducts.RemoveAt(itemIndex + 1);
            UpsellProducts.Insert(itemIndex, swapItem);
            for (int i = 0; i < UpsellProducts.Count; i++)
            {
                UpsellProducts[i].OrderBy = (short)i;
            }
            UpsellProducts.Save();
            UpsellProductGrid.DataBind();
        }
    }    

    private void SetLink(int upsellProductId, bool linked)
    {
        int index = _Product.UpsellProducts.IndexOf(_ProductId, upsellProductId);
        if (linked && (index < 0))
        {
            _Product.UpsellProducts.Add(new UpsellProduct(_ProductId, upsellProductId));
            _Product.Save();
        }
        else if (!linked && (index > -1))
        {
            _Product.UpsellProducts.DeleteAt(index);
        }
    }

    protected bool IsProductLinked(int upsellProductId)
    {
        return (_Product.UpsellProducts.IndexOf(_ProductId, upsellProductId) > -1);
    }

    protected void ProductSearchDs_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        SearchResultsGrid.Columns[0].Visible = ShowImages.Checked;
    }

    private void RedirectToEdit()
    {
        Response.Redirect("EditProduct.aspx?ProductId=" + _ProductId.ToString());
    }

    protected void FinishButton_Click(object sender, EventArgs e)
    {
        RedirectToEdit();
    }

    protected void UpsellProductGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int upsellProductId = (int)e.Keys[0];
        SetLink(upsellProductId, false);
        //CHECK THE SEARCH RESULTS GRID TO SEE IF THIS ITEMS APPEARS
        int tempIndex = 0;
        foreach (DataKey key in SearchResultsGrid.DataKeys)
        {
            int tempId = (int)key.Value;
            if (upsellProductId == tempId)
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
        UpsellProductGrid.DataBind();
        e.Cancel = true;
    }

}
