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
using CommerceBuilder.Catalog;

public partial class Admin_Products_ProductMenu : System.Web.UI.UserControl
{
    private int _ProductId = 0;
    private int _CategoryId = 0;

    private Product _Product;

    protected void Page_Load(object sender, EventArgs e)
    {
        _ProductId = PageHelper.GetProductId();
        _CategoryId = PageHelper.GetCategoryId();
        _Product = ProductDataSource.Load(_ProductId);
        if (_Product != null)
        {
            string urlSuffix = "?ProductId=" + _ProductId.ToString();
            if (_CategoryId > 0) urlSuffix = urlSuffix + "&CategoryId=" + _CategoryId.ToString();
            ProductDetails.NavigateUrl += urlSuffix;
            Assets.NavigateUrl += urlSuffix;
            Variants.NavigateUrl += urlSuffix;
            DigitalGoods.NavigateUrl += urlSuffix;
            Kitting.NavigateUrl += urlSuffix;
            Discounts.NavigateUrl += urlSuffix;
            PricingRules.NavigateUrl += urlSuffix;
            SimilarProducts.NavigateUrl += urlSuffix;
            ProductAccessories.NavigateUrl += urlSuffix;
            Categories.NavigateUrl += urlSuffix;
            Subscriptions.NavigateUrl += urlSuffix;
            ProductTemplate.NavigateUrl += urlSuffix;
            Preview.NavigateUrl = UrlGenerator.GetBrowseUrl(PageHelper.GetCategoryId(), _ProductId, CatalogNodeType.Product, _Product.Name);
            string confirmationJS = String.Format("return confirm('Are you sure you want to delete {0}');", _Product.Name);
            DeleteProduct.Attributes.Add("onclick", confirmationJS);
            HighlightMenu();
        }
        else this.Controls.Clear();
    }

    protected void HighlightMenu()
    {
        string fileName = Request.Url.Segments[Request.Url.Segments.Length - 1].ToLowerInvariant();
        switch (fileName)
        {
            case "editproduct.aspx": ProductDetails.CssClass = "contextMenuButtonSelected"; break;
            case "images.aspx":
            case "uploadimage.aspx":
            case "editassets.aspx":
            case "additionalimages.aspx":
            case "uploadadditionalimage.aspx":
            case "advancedimages.aspx":
            case "assetmanager.aspx":
                Assets.CssClass = "contextMenuButtonSelected"; break;
            case "options.aspx":
            case "choices.aspx":
            case "editoptions.aspx":
            case "editoption.aspx":
            case "variants.aspx":
                Variants.CssClass = "contextMenuButtonSelected"; break;
            case "digitalgoods.aspx":
            case "attachdigitalgood.aspx":
            case "selectvariant.aspx":
                DigitalGoods.CssClass = "contextMenuButtonSelected"; break;
            case "addcomponent.aspx":
            case "addpart.aspx":
            case "addpart2.aspx":
            case "attachcomponent.aspx":
            case "deletesharedcomponent.aspx":
            case "editcomponent.aspx":
            case "editkit.aspx":
            case "search.aspx":
            case "sortcomponents.aspx":
            case "sortparts.aspx":
            case "viewcomponent.aspx":
            case "viewpart.aspx":
                Kitting.CssClass = "contextMenuButtonSelected"; break;
            case "productdiscounts.aspx":
                Discounts.CssClass = "contextMenuButtonSelected"; break;
            case "addspecial.aspx":
            case "editspecial.aspx":
                PricingRules.CssClass = "contextMenuButtonSelected"; break;
            case "editsimilarproducts.aspx":
                SimilarProducts.CssClass = "contextMenuButtonSelected"; break;
            case "editproductaccessories.aspx":
                ProductAccessories.CssClass = "contextMenuButtonSelected"; break;
            case "editproductcategories.aspx":
                Categories.CssClass = "contextMenuButtonSelected"; break;
            case "editsubscription.aspx":
                Subscriptions.CssClass = "contextMenuButtonSelected"; break;
            case "editproducttemplate.aspx":
                ProductTemplate.CssClass = "contextMenuButtonSelected"; break;
            case "default.aspx":
                if (Request.Url.ToString().Contains("Specials")) PricingRules.CssClass = "contextMenuButtonSelected";
                else Kitting.CssClass = "contextMenuButtonSelected";
                break;
        }

    }
    protected void DeleteProduct_Click(Object sender, EventArgs e)
    {
        string navigateUrl = "~/Admin/Catalog/Browse.aspx?CategoryId=" + PageHelper.GetCategoryId();
        if (_Product.Delete())
            Response.Redirect(navigateUrl);
    }

    protected void Page_PreRender(Object sender,EventArgs e) 
    {
        if (_Product != null)
        {
            //PREVIEW LINK CAN CHANGE DUE TO CHANGE IN CUSTOM URL, WHICH CAN'T BE DETECTED BY PAGE_LOAD EVENT
            Preview.NavigateUrl = UrlGenerator.GetBrowseUrl(PageHelper.GetCategoryId(), _ProductId, CatalogNodeType.Product, _Product.Name);
        }
    }
}
