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

public partial class Admin_Products_Assets_AdditionalImages : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private int _ProductId;
    private Product _Product;

    protected void Page_Init(object sender, EventArgs e)
    {
        _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        _Product = ProductDataSource.Load(_ProductId);
        if (_Product == null) NavigationHelper.GetAdminUrl("Catalog/Browse.aspx");
        Caption.Text = string.Format(Caption.Text, _Product.Name);
        BindImages();
    }

    protected void UploadButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("UploadAdditionalImage.aspx?ProductId=" + _ProductId.ToString());
    }

    protected void BasicButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("Images.aspx?ProductId=" + _ProductId.ToString());
    }

    protected void AdvancedButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("AdvancedImages.aspx?ProductId=" + _ProductId.ToString());
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

    private void BindImages()
    {
        AdditionalImagesRepeater.DataSource = _Product.Images;
        AdditionalImagesRepeater.DataBind();
        trEmpty.Visible = (AdditionalImagesRepeater.Items.Count == 0);
    }

    protected String GetImageUrl(object dataItem)
    {
        ProductImage image = (ProductImage)dataItem;
        string url = image.ImageUrl;
        if (!string.IsNullOrEmpty(url))
        {
            if (!url.Contains("?")) return url + "?ts=" + DateTime.Now.ToString("hhmmss");
            else return url + "&ts=" + DateTime.Now.ToString("hhmmss");
        }
        else return string.Empty;
    }
}
