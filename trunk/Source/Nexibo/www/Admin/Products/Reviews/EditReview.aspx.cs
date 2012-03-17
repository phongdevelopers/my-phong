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

public partial class Admin_Products_Reviews_EditReview : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{


    private int _ProductReviewId;
    private ProductReview _ProductReview;
    
    protected void Page_Load(object sender, EventArgs e)
    {
        _ProductReviewId = AlwaysConvert.ToInt(Request.QueryString["ReviewId"]);
        _ProductReview = ProductReviewDataSource.Load(_ProductReviewId);
        if (_ProductReview == null) Response.Redirect("Default.aspx");
        if (!Page.IsPostBack)
        {
            ReviewerProfile profile = _ProductReview.ReviewerProfile;
            ProductLink.NavigateUrl = string.Format(ProductLink.NavigateUrl, _ProductReview.ProductId);
            ProductLink.Text = _ProductReview.Product.Name;
            ReviewDate.Text = string.Format(ReviewDate.Text, _ProductReview.ReviewDate);
            ReviewerEmail.Text = profile.Email;
            ReviewerName.Text = profile.DisplayName;
            ReviewerLocation.Text = profile.Location;
            Rating.ImageUrl = NavigationHelper.GetRatingImage(AlwaysConvert.ToDecimal(_ProductReview.Rating));
            Rating.AlternateText = _ProductReview.Rating.ToString();
            ReviewTitle.Text = _ProductReview.ReviewTitle;
            ReviewBody.Text = _ProductReview.ReviewBody;
        }
    }

    protected void Save()
    {
        //FIRST UPDATE THE PROFILE
        ReviewerProfile profile = _ProductReview.ReviewerProfile;
        profile.Email = ReviewerEmail.Text;
        profile.DisplayName = ReviewerName.Text;
        profile.Location = ReviewerLocation.Text;
        profile.Save();
        //NEXT UPDATE THE REVIEW
        _ProductReview.ReviewTitle = ReviewTitle.Text;
        _ProductReview.ReviewBody = ReviewBody.Text;
        _ProductReview.Save();
        SuccessMessage.Text = string.Format(SuccessMessage.Text, LocaleHelper.LocalNow);
        SuccessMessage.Visible = true;
    }
    
    protected void SaveButton_Click(object sender, EventArgs e)
    {
        Save();        
    }
    
    protected void CancelButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }

    protected void SaveAndCloseButton_Click(object sender, EventArgs e)
    {
        Save();
        Response.Redirect("Default.aspx");
    }

}
