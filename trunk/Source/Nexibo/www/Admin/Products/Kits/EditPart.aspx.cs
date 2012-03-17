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

public partial class Admin_Products_Kits_EditPart : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private int _CategoryId;
    private Category _Category;
    private int _ProductId;
    private Product _Product;
    private int _KitComponentId;
    private KitComponent _KitComponent;
    private int _KitProductId;
    private KitProduct _KitProduct;
    private Dictionary<int, int> selectedOptions;
    
    protected void Page_Init(object sender, EventArgs e)
    {
        //INITIALIZE VARIABLES
        _CategoryId = PageHelper.GetCategoryId();
        _Category = CategoryDataSource.Load(_CategoryId);
        _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        _Product = ProductDataSource.Load(_ProductId);
        if (_Product == null) Response.Redirect(NavigationHelper.GetAdminUrl("Catalog/Browse.aspx"));
        _KitProductId = AlwaysConvert.ToInt(Request.QueryString["PartId"]);
        _KitProduct = KitProductDataSource.Load(_KitProductId);
        if (_KitProduct == null) Response.Redirect("EditKit.aspx?CategoryId=" + _CategoryId.ToString() + "&ProductId=" + _ProductId.ToString());
        _KitComponentId = _KitProduct.KitComponentId;
        _KitComponent = _KitProduct.KitComponent;
        //INITIALIZE PAGE ELEMENTS
        Caption.Text = string.Format(Caption.Text, _KitComponent.Name);
        KitName.Text = _Product.Name;
        KitComponentName.Text = _KitComponent.Name;
        SelectedProductName.Text = _KitProduct.Product.Name;
        DisplayName.Text = _KitProduct.Name;
        KitQuantity.Text = _KitProduct.Quantity.ToString();
        ProductPrice.Text = string.Format("{0:lc}", _KitProduct.Product.Price);
        PriceMode.SelectedIndex = (int)_KitProduct.PriceMode;
        PriceMode_SelectedIndexChanged(sender, e);
        Price.Text = string.Format("{0:F2}", _KitProduct.Price);
        ProductWeight.Text = string.Format("{0:F2}", _KitProduct.Product.Weight);
        WeightMode.SelectedIndex = (int)_KitProduct.WeightMode;
        WeightMode_SelectedIndexChanged(sender, e);
        Weight.Text = string.Format("{0:F2}", _KitProduct.Weight);
        phOptions.Controls.Clear();
        selectedOptions = ProductHelper.BuildProductOptions(_KitProduct, phOptions);
        IsSelected.Checked = _KitProduct.IsSelected;
		WeightUnit.Text = Token.Instance.Store.WeightUnit.ToString();
    }

    protected void PriceMode_SelectedIndexChanged(object sender, EventArgs e)
    {
        switch (PriceMode.SelectedIndex)
        {
            case 0:
                FixedPriceLabel.Visible = false;
                ModifyPriceLabel.Visible = false;
                Price.Visible = false;
                break;
            case 1:
                FixedPriceLabel.Visible = false;
                ModifyPriceLabel.Visible = true;
                Price.Visible = true;
                break;
            case 2:
                FixedPriceLabel.Visible = true;
                ModifyPriceLabel.Visible = false;
                Price.Visible = true;
                break;
        }
    }

    protected void WeightMode_SelectedIndexChanged(object sender, EventArgs e)
    {
        switch(WeightMode.SelectedIndex) 
        {
            case 0:
                FixedWeightLabel.Visible = false;
                ModifyWeightLabel.Visible = false;
                Weight.Visible = false;
                break;
            case 1:
                FixedWeightLabel.Visible = false;
                ModifyWeightLabel.Visible = true;
                Weight.Visible = true;
                break;
            case 2:
                FixedWeightLabel.Visible = true;
                ModifyWeightLabel.Visible = false;
                Weight.Visible = true;
                break;
        }
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        Response.Redirect(string.Format("EditKit.aspx?CategoryId={0}&ProductId={1}", _CategoryId, _ProductId));
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        short quantity = AlwaysConvert.ToInt16(KitQuantity.Text);
        if (quantity > 0)
        {
            ProductVariant productVariant = null;
            if (_KitProduct.Product.ProductOptions.Count > 0)
            {
                productVariant = new ProductVariant();
                productVariant.Load(_KitProduct.ProductId, selectedOptions);
            }
            if (productVariant != null) _KitProduct.OptionList = productVariant.OptionList;
            _KitProduct.Name = DisplayName.Text;
            _KitProduct.Quantity = quantity;
            _KitProduct.PriceModeId = AlwaysConvert.ToByte(PriceMode.SelectedIndex);
            _KitProduct.Price = AlwaysConvert.ToDecimal(Price.Text);
            _KitProduct.WeightModeId = AlwaysConvert.ToByte(WeightMode.SelectedIndex);
            _KitProduct.Weight = AlwaysConvert.ToDecimal(Weight.Text);
            _KitProduct.IsSelected = IsSelected.Checked;
            _KitProduct.Save();
        }
        Response.Redirect(string.Format("EditKit.aspx?CategoryId={0}&ProductId={1}", _CategoryId, _ProductId));
    }


}
