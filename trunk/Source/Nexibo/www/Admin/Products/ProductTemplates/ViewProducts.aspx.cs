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

public partial class Admin_Products_ProductTemplates_ViewProducts : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private int _ProductTemplateId;
    private ProductTemplate _ProductTemplate;

    protected void Page_Init(object sender, EventArgs e)
    {
        _ProductTemplateId = AlwaysConvert.ToInt(Request.QueryString["ProductTemplateId"]);
        _ProductTemplate = ProductTemplateDataSource.Load(_ProductTemplateId);
        if (_ProductTemplate == null) Response.Redirect("Default.aspx");
        Caption.Text = string.Format(Caption.Text, _ProductTemplate.Name);
    }

    protected void FinishButton_Click(object sender, EventArgs e)
    {
        // TODO: REDIRECT TO APPROPRIATE SCREEN
        Response.Redirect("Default.aspx");
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
        UpdateProductTemplateAssociation(productId, true);
        ImageButton removeButton = attachButton.Parent.FindControl("RemoveButton") as ImageButton;
        if (removeButton != null) removeButton.Visible = true;
        attachButton.Visible = false;
        AssociatedProductGrid.DataBind();
    }

    protected void RemoveButton_Click(object sender, EventArgs e)
    {
        ImageButton removeButton = (ImageButton)sender;
        int dataItemIndex = AlwaysConvert.ToInt(removeButton.CommandArgument);
        GridView grid = SearchResultsGrid;
        dataItemIndex = (dataItemIndex - (grid.PageSize * grid.PageIndex));
        int productId = (int)grid.DataKeys[dataItemIndex].Value;
        UpdateProductTemplateAssociation(productId, false);
        ImageButton attachButton = removeButton.Parent.FindControl("AttachButton") as ImageButton;
        if (attachButton != null) attachButton.Visible = true;
        removeButton.Visible = false;
        AssociatedProductGrid.DataBind();
    }

    private void UpdateProductTemplateAssociation(int productId, bool linked)
    {
        Product product = ProductDataSource.Load(productId);
        if (product != null)
        {
            int templateIndex = product.ProductProductTemplates.IndexOf(productId, _ProductTemplateId);
            if (linked && templateIndex < 0)
            {
                ProductProductTemplate ppt = new ProductProductTemplate(productId, _ProductTemplateId);
                product.ProductProductTemplates.Add(ppt);
                ppt.Save();
            }
            else if (!linked && templateIndex > -1)
            {
                product.ProductProductTemplates.DeleteAt(templateIndex);
            }
        }
    }

    protected bool IsProductLinked(Product product)
    {
        if (product != null)
        {
            foreach (ProductProductTemplate ppt in product.ProductProductTemplates)
            {
                if (ppt.ProductTemplateId == _ProductTemplateId) return true;
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

    protected void AssociatedProductGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int relatedProductId = (int)e.Keys[0];
        UpdateProductTemplateAssociation(relatedProductId, false);
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
        AssociatedProductGrid.DataBind();
        e.Cancel = true;
    }

    protected void RemoveButton2_Click(object sender, ImageClickEventArgs e)
    {
        ImageButton removeButton = (ImageButton)sender;
        int productId = AlwaysConvert.ToInt(removeButton.CommandArgument);
        UpdateProductTemplateAssociation(productId, false);
        AssociatedProductGrid.DataBind();
        if (SearchResultsGrid.Visible) SearchResultsGrid.DataBind();
    }
}