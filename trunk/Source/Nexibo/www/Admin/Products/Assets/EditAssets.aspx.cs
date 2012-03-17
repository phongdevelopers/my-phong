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

public partial class Admin_Products_Assets_EditAssets : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private int _ProductId;
    private Product _Product;

    protected void Page_Init(object sender, EventArgs e)
    {
        PageHelper.SetPickImageButton(ThumbnailImageUrl, BrowseThumbnailUrl);
        PageHelper.SetPickImageButton(StandardImageUrl, BrowseStandardUrl);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        _Product = ProductDataSource.Load(_ProductId);
        if (_Product == null) NavigationHelper.GetAdminUrl("Catalog/Browse.aspx");
        BindImages();
    }

    private void SaveImages()
    {
        _Product.IconUrl = IconImageUrl.Text;
        _Product.IconAltText = IconImageAltText.Text;
        _Product.ThumbnailUrl = ThumbnailImageUrl.Text;
        _Product.ThumbnailAltText = ThumbnailImageAltText.Text;
        _Product.ImageUrl = StandardImageUrl.Text;
        _Product.ImageAltText = StandardImageAltText.Text;
        _Product.Save();
        SavedMessage.Visible = true;
        SavedMessage.Text = string.Format(SavedMessage.Text, LocaleHelper.LocalNow);
    }
   
    protected void SaveButton_Click(object sender, EventArgs e)
    {
        SaveImages();
        BindImages();
    }

    protected void UploadImageButton_Click(object sender, EventArgs e)
    {
        SaveImages();
        Response.Redirect("UploadImage.aspx?ProductId=" + _ProductId.ToString());
    }

    public void BindImages()
    {
        IconImageUrl.Text = _Product.IconUrl;
        IconImageAltText.Text = _Product.IconAltText;
        ThumbnailImageUrl.Text = _Product.ThumbnailUrl;
        ThumbnailImageAltText.Text = _Product.ThumbnailAltText;
        StandardImageUrl.Text = _Product.ImageUrl;
        StandardImageAltText.Text = _Product.ImageAltText;
        AdditionalImagesRepeater.DataSource = _Product.Images;
        AdditionalImagesRepeater.DataBind();
        ProductAssetsGridView.DataSource = _Product.Assets;
        ProductAssetsGridView.DataBind();
    }

    protected void ProductAssetsGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int productAssetId = AlwaysConvert.ToInt(ProductAssetsGridView.DataKeys[e.RowIndex].Value);
        ProductAsset productAsset = ProductAssetDataSource.Load(productAssetId);
        if (productAsset != null) productAsset.Delete();
        ProductAssetsGridView.DataSource = _Product.Assets;
        ProductAssetsGridView.DataBind();
        e.Cancel = true;
    }

    protected void AddProductAssetButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("AssetManager.aspx?ProductId=" + _ProductId.ToString() + "&ImageId=PA");
    }

    protected void AdditionalImagesRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            ImageButton browse = e.Item.FindControl("Browse") as ImageButton;
            TextBox url = e.Item.FindControl("ImageUrl") as TextBox;
            if (browse != null && url != null)
            {
                browse.OnClientClick = "return PickImage('" + url.ClientID + "')";
            }
        }
    }

    protected void AdditionalImagesRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Delete")
        {
            int productImageId = AlwaysConvert.ToInt(e.CommandArgument);
            int index = _Product.Images.IndexOf(productImageId);
            if (index > -1) _Product.Images.DeleteAt(index);
            BindImages();
        }
    }

    protected void AddAdditionalImageButton_Click(object sender, EventArgs e)
    {
        SaveImages();
        ProductImage image = new ProductImage();
        image.ProductId = _ProductId;
        image.Save();
        _Product.Images.Add(image);
        BindImages();
    }
    

}
