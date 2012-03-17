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

public partial class Admin_People_Vendors_EditProducts : System.Web.UI.UserControl
{
    private Vendor _Vendor;
    private int _VendorId = 0;

    protected void Page_Init(object sender, EventArgs e)
    {
        _VendorId = AlwaysConvert.ToInt(Request.QueryString["VendorId"]);
        _Vendor = VendorDataSource.Load(_VendorId);
        if (_Vendor == null) Response.Redirect(NavigationHelper.GetAdminUrl("Default.aspx"));
        //Caption.Text = string.Format(Caption.Text, _Vendor.Name);
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
        UpdateAssociation(productId, true);
        ImageButton removeButton = attachButton.Parent.FindControl("RemoveButton") as ImageButton;
        if (removeButton != null) removeButton.Visible = true;
        attachButton.Visible = false;

        Label nanufacturerNameLabel = attachButton.Parent.FindControl("VendorName") as Label;
        nanufacturerNameLabel.Text = _Vendor.Name;

        RelatedProductGrid.DataBind();
    }

    protected void RemoveButton_Click(object sender, EventArgs e)
    {
        ImageButton removeButton = (ImageButton)sender;
        int dataItemIndex = AlwaysConvert.ToInt(removeButton.CommandArgument);
        GridView grid = SearchResultsGrid;
        dataItemIndex = (dataItemIndex - (grid.PageSize * grid.PageIndex));
        int productId = (int)grid.DataKeys[dataItemIndex].Value;
        UpdateAssociation(productId, false);
        ImageButton attachButton = removeButton.Parent.FindControl("AttachButton") as ImageButton;
        if (attachButton != null) attachButton.Visible = true;
        removeButton.Visible = false;

        Label nanufacturerNameLabel = attachButton.Parent.FindControl("VendorName") as Label;
        nanufacturerNameLabel.Text = String.Empty;

        RelatedProductGrid.DataBind();


    }


    private void UpdateAssociation(int relatedProductId, bool linked)
    {
        Product product = ProductDataSource.Load(relatedProductId);
        if (product != null)
        {
            if (linked) product.VendorId = _VendorId;
            else product.VendorId = 0;
            product.Save();
        }

    }

    protected bool IsProductLinked(Product product)
    {
        if (product != null)
        {
            return (product.VendorId == _VendorId);
        }
        return false;
    }

    protected void ProductSearchDs_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        SearchResultsGrid.Columns[0].Visible = ShowImages.Checked;
        SearchResultsGrid.Columns[2].Visible = !NoVendor.Checked;
        if (NoVendor.Checked)
            ProductSearchDs.SelectParameters["vendorId"].DefaultValue = "-1";
        else
            ProductSearchDs.SelectParameters["vendorId"].DefaultValue = "0";
    }

    private void RedirectToEdit()
    {
        Response.Redirect("Default.aspx");
    }


    protected void RelatedProductGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int relatedProductId = (int)e.Keys[0];
        UpdateAssociation(relatedProductId, false);
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
        UpdateAssociation(productId, false);
        RelatedProductGrid.DataBind();
        if (SearchResultsGrid.Visible) SearchResultsGrid.DataBind();

    }
}
