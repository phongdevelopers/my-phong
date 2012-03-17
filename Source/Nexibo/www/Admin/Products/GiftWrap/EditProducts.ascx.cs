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
using CommerceBuilder.Products;
using CommerceBuilder.Utility;
using System.Collections.Generic;

public partial class Admin_Products_GiftWrap_EditProducts : System.Web.UI.UserControl
{
    private WrapGroup _WrapGroup;
    private int _WrapGroupId = 0;

    protected void Page_Init(object sender, EventArgs e)
    {
        _WrapGroupId = AlwaysConvert.ToInt(Request.QueryString["WrapGroupId"]);
        _WrapGroup = WrapGroupDataSource.Load(_WrapGroupId);
        if (_WrapGroup == null) Response.Redirect(NavigationHelper.GetAdminUrl("Default.aspx"));
        //Caption.Text = string.Format(Caption.Text, _WrapGroup.Name);
        if (!Page.IsPostBack)
        {
            List<WrapGroup> targets = new List<WrapGroup>();
            WrapGroupCollection allWrapGroups = WrapGroupDataSource.LoadForStore();
            foreach (WrapGroup w in allWrapGroups)
            {
                if (w.WrapGroupId != AlwaysConvert.ToInt(_WrapGroupId)) targets.Add(w);
            }
            NewWrapGroup.DataSource = targets;
            NewWrapGroup.DataBind();
        }
    }

    protected void SearchButton_Click(object sender, EventArgs e)
    {
        SearchResultsGrid.Visible = true;
        SearchResultsGrid.PageIndex = 0;
        UpdateSearchCriteria();

        SearchResultsGrid.DataBind();
    }

    protected void AttachButton_Click(object sender, EventArgs e)
    {
        ImageButton attachButton = (ImageButton)sender;
        int dataItemIndex = AlwaysConvert.ToInt(attachButton.CommandArgument);
        GridView grid = SearchResultsGrid;
        dataItemIndex = (dataItemIndex - (grid.PageSize * grid.PageIndex));
        int productId = (int)grid.DataKeys[dataItemIndex].Value;
        SetWrapGroup(productId, true);
        ImageButton removeButton = attachButton.Parent.FindControl("RemoveButton") as ImageButton;
        if (removeButton != null) removeButton.Visible = true;
        attachButton.Visible = false;

        Label nanufacturerNameLabel = attachButton.Parent.FindControl("WrapGroupName") as Label;
        nanufacturerNameLabel.Text = _WrapGroup.Name;

        RelatedProductGrid.DataBind();
    }

    protected void RemoveButton_Click(object sender, EventArgs e)
    {
        ImageButton removeButton = (ImageButton)sender;
        int dataItemIndex = AlwaysConvert.ToInt(removeButton.CommandArgument);
        GridView grid = SearchResultsGrid;
        dataItemIndex = (dataItemIndex - (grid.PageSize * grid.PageIndex));
        int productId = (int)grid.DataKeys[dataItemIndex].Value;
        SetWrapGroup(productId, false);
        ImageButton attachButton = removeButton.Parent.FindControl("AttachButton") as ImageButton;
        if (attachButton != null) attachButton.Visible = true;
        removeButton.Visible = false;

        Label nanufacturerNameLabel = attachButton.Parent.FindControl("WrapGroupName") as Label;
        nanufacturerNameLabel.Text = String.Empty;

        RelatedProductGrid.DataBind();


    }


    private void SetWrapGroup(int relatedProductId, bool linked)
    {
        Product product = ProductDataSource.Load(relatedProductId);
        if (product != null)
        {
            if (linked) product.WrapGroupId = _WrapGroupId;
            else product.WrapGroupId = 0;
            product.Save();
        }

    }

    protected bool IsProductLinked(Product product)
    {
        if (product != null)
        {
            return (product.WrapGroupId == _WrapGroupId);
        }
        return false;
    }

    protected void ProductSearchDs_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        //UpdateSearchCriteria();
    }

    private void UpdateSearchCriteria()
    {
        SearchResultsGrid.Columns[0].Visible = ShowImages.Checked;
        SearchResultsGrid.Columns[2].Visible = !NoWrapGroup.Checked;
        String criteria = "";
        if (!String.IsNullOrEmpty(SearchName.Text))
        {
            criteria += "Name LIKE '%" + StringHelper.SafeSqlString(SearchName.Text) + "%'";
        }

        if (NoWrapGroup.Checked)
            ProductSearchDs.SelectParameters["sqlCriteria"].DefaultValue = (String.IsNullOrEmpty(criteria) ? "" : criteria + " AND ") + "WrapGroupId IS NULL";
        else
            ProductSearchDs.SelectParameters["sqlCriteria"].DefaultValue = criteria;
    }

    private void RedirectToEdit()
    {
        Response.Redirect("Default.aspx");
    }

    private void UpdateSearchGrid(int relatedProductId)
    {
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
    }


    protected void RelatedProductGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int relatedProductId = (int)e.Keys[0];
        SetWrapGroup(relatedProductId, false);
        UpdateSearchGrid(relatedProductId);
        RelatedProductGrid.DataBind();
        e.Cancel = true;
    }

    protected void RemoveButton2_Click(object sender, ImageClickEventArgs e)
    {
        ImageButton removeButton = (ImageButton)sender;
        int productId = AlwaysConvert.ToInt(removeButton.CommandArgument);
        SetWrapGroup(productId, false);
        RelatedProductGrid.DataBind();
        if (SearchResultsGrid.Visible) SearchResultsGrid.DataBind();

    }

    protected void RelatedProductGrid_DataBound(object sender, EventArgs e)
    {
        foreach (GridViewRow gvr in RelatedProductGrid.Rows)
        {
            CheckBox cb = (CheckBox)gvr.FindControl("Selected");
            ScriptManager.RegisterArrayDeclaration(RelatedProductGrid, "CheckBoxIDs", String.Concat("'", cb.ClientID, "'"));
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        NewWrapGroupPanel.Visible = ((NewWrapGroup.Items.Count > 0) && (RelatedProductGrid.Rows.Count > 0));
    }

    protected void BackButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("EditWrapGroup.aspx?WrapGroupId=" + _WrapGroupId.ToString());
    }

    protected void NewWrapGroupUpdateButton_Click(object sender, EventArgs e)
    {
        int newWrapGroupId = AlwaysConvert.ToInt(NewWrapGroup.SelectedValue);
        WrapGroup newWrapGroup = WrapGroupDataSource.Load(newWrapGroupId);
        if (newWrapGroup != null)
        {
            foreach (GridViewRow row in RelatedProductGrid.Rows)
            {
                CheckBox selected = (CheckBox)row.FindControl("Selected");
                if ((selected != null) && (selected.Checked))
                {
                    int productId = AlwaysConvert.ToInt(RelatedProductGrid.DataKeys[row.DataItemIndex].Value);
                    Product product = ProductDataSource.Load(productId);
                    if (product != null)
                    {
                        product.WrapGroupId = newWrapGroupId;
                        product.Save();
                    }
                }
            }
            RelatedProductGrid.DataBind();
            if (SearchResultsGrid.Visible) SearchResultsGrid.DataBind();
        }
    }
}
