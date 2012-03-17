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

public partial class Admin_Products_Assets_Images : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private int _ProductId;
    private Product _Product;

    protected void Page_Load(object sender, EventArgs e)
    {
        _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        _Product = ProductDataSource.Load(_ProductId);
        if (_Product == null) NavigationHelper.GetAdminUrl("Catalog/Browse.aspx");
        Caption.Text = string.Format(Caption.Text, _Product.Name);
        if (_Product.IconUrl.Length > 0)
        {
            IconPreview.ImageUrl = _Product.IconUrl;
        }
        else
        {
            IconPreview.Visible = false;
            IconPreviewNoImage.Visible = true;
        }

        if (_Product.ThumbnailUrl.Length > 0)
        {
            ThumbnailPreview.ImageUrl = _Product.ThumbnailUrl;
        }
        else
        {
            ThumbnailPreview.Visible = false;
            ThumbnailPreviewNoImage.Visible = true;
        }
        if (_Product.ImageUrl.Length > 0)
        {
            ImagePreview.ImageUrl = _Product.ImageUrl;
        }
        else
        {
            ImagePreview.Visible = false;
            ImagePreviewNoImage.Visible = true;
        }
        
    }

    protected void UploadButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("UploadImage.aspx?ProductId=" + _ProductId.ToString());
    }

    protected void AdditionalButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("AdditionalImages.aspx?ProductId=" + _ProductId.ToString());
    }

    protected void AdvancedButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("AdvancedImages.aspx?ProductId=" + _ProductId.ToString());
    }


}
