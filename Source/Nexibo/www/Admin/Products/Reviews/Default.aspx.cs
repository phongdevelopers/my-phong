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

public partial class Admin_Products_Reviews_Default : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private int _ProductId;
    private Product _Product;

    protected void Page_Init(object sender, EventArgs e)
    {
        _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        _Product = ProductDataSource.Load(_ProductId);
        HiddenProductId.Value = _ProductId.ToString();
        if (_Product == null)
        {
            ProductCaption.Visible = false;
        }
        else
        {
            ProductCaption.Text = string.Format(ProductCaption.Text, _Product.Name);
        }
        if (!Page.IsPostBack)
        {
            //CHECK FOR SEARCH CACE
            SearchCache.SearchCriteria criteria = SearchCache.GetCriteria();
            if ((criteria != null) && (criteria.Arguments.ContainsKey("ShowApproved")))
            {
                ListItem item = ShowApproved.Items.FindByValue(criteria.Arguments["ShowApproved"].ToString());
                if (item != null) ShowApproved.SelectedIndex = ShowApproved.Items.IndexOf(item);
                ReviewGrid.PageIndex = criteria.PageIndex;
                ReviewGrid.DefaultSortDirection = criteria.SortDirection;
                ReviewGrid.DefaultSortExpression = criteria.SortExpression;
            }
        }
    }

    protected void ReviewGrid_DataBound(object sender, EventArgs e)
    {
        GridView grid = (GridView)sender;
        if (grid.Rows.Count > 0)
        {
            foreach (GridViewRow gvr in (grid.Rows))
            {
                CheckBox cb = (CheckBox)gvr.FindControl("Selected");
                ScriptManager.RegisterArrayDeclaration(grid, "CheckBoxIDs", String.Concat("'", cb.ClientID, "'"));
            }
        }
        else
        {
            ReviewActionPanel.Visible = false;
        }
        //CACHE THE SEARCH ARGUMENTS FOR NEXT VISIT
        SearchCache.SearchCriteria criteria = new SearchCache.SearchCriteria();
        criteria.Arguments.Add("ShowApproved", ShowApproved.SelectedValue);
        criteria.PageIndex = grid.PageIndex;
        criteria.SortDirection = ReviewGrid.SortDirection;
        criteria.SortExpression = ReviewGrid.SortExpression;
        SearchCache.SetCriteria(criteria);
    }

    protected void GoButton_Click(object sender, EventArgs e)
    {
        List<int> reviewIds = GetSelectedReviewIds();
        if (reviewIds.Count > 0)
        {
            if (ReviewAction.SelectedValue == "Delete")
            {
                foreach (int reviewId in reviewIds)
                {
                    ProductReviewDataSource.Delete(reviewId);
                }
            }
            else if ((ReviewAction.SelectedValue == "Approve") || (ReviewAction.SelectedValue == "Disapprove"))
            {
                bool approved = (ReviewAction.SelectedValue == "Approve");
                foreach (int reviewId in reviewIds)
                {
                    ProductReview review = ProductReviewDataSource.Load(reviewId);
                    review.IsApproved = approved;
                    review.Save();
                }
            }
            ReviewGrid.DataBind();
        }
        ReviewAction.SelectedIndex = 0;
    }

    private List<int> GetSelectedReviewIds()
    {
        List<int> selectedReviewIds = new List<int>();
        foreach (GridViewRow row in ReviewGrid.Rows)
        {
            CheckBox selected = (CheckBox)PageHelper.RecursiveFindControl(row, "Selected");
            if ((selected != null) && selected.Checked)
            {
                selectedReviewIds.Add((int)ReviewGrid.DataKeys[row.DataItemIndex].Values[0]);
            }
        }
        return selectedReviewIds;
    }
}
    

