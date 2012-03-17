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

public partial class Admin_Reports_LowInventory : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{


    protected string GetName(object dataItem)
    {
        ProductInventoryDetail detail = (ProductInventoryDetail)dataItem;
        if (string.IsNullOrEmpty(detail.VariantName)) return detail.Name;
        return string.Format("{0} ({1})", detail.Name, detail.VariantName);
    }

    protected void InventoryGrid_DataBound(object sender, EventArgs e)
    {
        SaveButton.Visible = (InventoryGrid.Rows.Count > 0);
        InventoryGrid.GridLines = (SaveButton.Visible) ? GridLines.Both : GridLines.None;
    }

    private int GetControlValue(GridViewRow row, string controlId)
    {
        TextBox tb = row.FindControl(controlId) as TextBox;
        if (tb != null)
        {
            return AlwaysConvert.ToInt(tb.Text);
        }
        return 0;
    }    

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        foreach (GridViewRow row in InventoryGrid.Rows)
        {
            int dataItemIndex = row.DataItemIndex;
            dataItemIndex = (dataItemIndex - (InventoryGrid.PageSize * InventoryGrid.PageIndex));
            int productId = (int)InventoryGrid.DataKeys[dataItemIndex].Values[0];
            int productVariantId = AlwaysConvert.ToInt(InventoryGrid.DataKeys[dataItemIndex].Values[1].ToString());            
            int inStock = GetControlValue(row, "InStock");
            int lowStock = GetControlValue(row, "LowStock");
            if (productVariantId.Equals(0))
            {
                Product product = ProductDataSource.Load(productId);
                product.InStock = inStock;
                product.InStockWarningLevel = lowStock;
                product.Save();
            }
            else
            {
                ProductVariant variant = new ProductVariant();
                variant.Load(productVariantId);
                variant.InStock = inStock;
                variant.InStockWarningLevel = lowStock;
                variant.Save();
            }
        }
        SavedMessage.Text = string.Format(SavedMessage.Text, DateTime.Now);
        SavedMessage.Visible = true;
        InventoryGrid.DataBind();
    }

}
