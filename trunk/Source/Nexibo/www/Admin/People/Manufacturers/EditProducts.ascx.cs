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
using CommerceBuilder.Utility;
using CommerceBuilder.Users;
using CommerceBuilder.Common;
using CommerceBuilder.Products;

public partial class Admin_People_Manufacturers_EditProducts : System.Web.UI.UserControl
{
    private Manufacturer _Manufacturer;
    private int _ManufacturerId = 0;

    protected void Page_Init(object sender, EventArgs e)
    {
        _ManufacturerId = AlwaysConvert.ToInt(Request.QueryString["ManufacturerId"]);
        _Manufacturer = ManufacturerDataSource.Load(_ManufacturerId);
        if (_Manufacturer == null) Response.Redirect(NavigationHelper.GetAdminUrl("Default.aspx"));
        //Caption.Text = string.Format(Caption.Text, _Manufacturer.Name);
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
        SetManufacturer(productId, true);
        ImageButton removeButton = attachButton.Parent.FindControl("RemoveButton") as ImageButton;
        if (removeButton != null) removeButton.Visible = true;
        attachButton.Visible = false;

        Label nanufacturerNameLabel = attachButton.Parent.FindControl("ManufacturerName") as Label;
        nanufacturerNameLabel.Text = _Manufacturer.Name;

        RelatedProductGrid.DataBind();
    }

    protected void RemoveButton_Click(object sender, EventArgs e)
    {
        ImageButton removeButton = (ImageButton)sender;
        int dataItemIndex = AlwaysConvert.ToInt(removeButton.CommandArgument);
        GridView grid = SearchResultsGrid;
        dataItemIndex = (dataItemIndex - (grid.PageSize * grid.PageIndex));
        int productId = (int)grid.DataKeys[dataItemIndex].Value;
        SetManufacturer(productId, false);
        ImageButton attachButton = removeButton.Parent.FindControl("AttachButton") as ImageButton;
        if (attachButton != null) attachButton.Visible = true;
        removeButton.Visible = false;

        Label nanufacturerNameLabel = attachButton.Parent.FindControl("ManufacturerName") as Label;
        nanufacturerNameLabel.Text = String.Empty;

        RelatedProductGrid.DataBind();


    }


    private void SetManufacturer(int relatedProductId, bool linked)
    {
        Product product = ProductDataSource.Load(relatedProductId);
        if (product != null)
        {
            if (linked) product.ManufacturerId = _ManufacturerId;
            else product.ManufacturerId = 0;
            product.Save();
        }

    }

    protected bool IsProductLinked(Product product)
    {
        if (product != null)
        {
            return (product.ManufacturerId == _ManufacturerId);
        }
        return false;
    }

    protected void ProductSearchDs_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        SearchResultsGrid.Columns[0].Visible = ShowImages.Checked;
        SearchResultsGrid.Columns[2].Visible = !NoManufacturer.Checked;
        if (NoManufacturer.Checked)
            ProductSearchDs.SelectParameters["manufacturerId"].DefaultValue = "-1";
        else
            ProductSearchDs.SelectParameters["manufacturerId"].DefaultValue = "0";
    }

    private void RedirectToEdit()
    {
        Response.Redirect("Default.aspx");
    }


    protected void RelatedProductGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int relatedProductId = (int)e.Keys[0];
        SetManufacturer(relatedProductId, false);
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
        SetManufacturer(productId, false);
        RelatedProductGrid.DataBind();
        if (SearchResultsGrid.Visible) SearchResultsGrid.DataBind();

    }
}
