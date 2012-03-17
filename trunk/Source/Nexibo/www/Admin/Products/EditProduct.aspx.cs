using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CommerceBuilder.Common;
using CommerceBuilder.Catalog;
using CommerceBuilder.Products;
using CommerceBuilder.Shipping;
using CommerceBuilder.Stores;
using CommerceBuilder.Utility;

public partial class Admin_Products_EditProduct : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private int _ProductId = 0;
    private Product _Product;

    protected void Page_Init(object sender, EventArgs e)
    {
        _ProductId = PageHelper.GetProductId();
        _Product = ProductDataSource.Load(_ProductId);
        Page.Title = string.Format(Page.Title, _Product.Name);
        Caption.Text = string.Format(Caption.Text, _Product.Name);
        CancelButton.NavigateUrl = "~/Admin/Catalog/Browse.aspx?CategoryId=" + PageHelper.GetCategoryId();
        CancelButton1.NavigateUrl = "~/Admin/Catalog/Browse.aspx?CategoryId=" + PageHelper.GetCategoryId();
        PageHelper.SetHtmlEditor(Description, DescriptionHtml);
        PageHelper.SetHtmlEditor(ExtendedDescription, ExtendedDescriptionHtml);
        PageHelper.SetPageDefaultButton(Page, SaveButton);
        BindManufacturers();
        BindDisplayPage();
        BindThemes();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            //BASIC INFO
            Name.Text = _Product.Name;
            Sku.Text = _Product.Sku;
            Price.Text = String.Format("{0:F2}", _Product.Price);
            if (_Product.MSRP >= 0) Msrp.Text = string.Format("{0:F2}", _Product.MSRP);
            ModelNumber.Text = _Product.ModelNumber;
            Visibility.SelectedIndex = (int)_Product.Visibility;
            Featured.Checked = _Product.IsFeatured;
            ExcludeFromFeed.Checked = _Product.ExcludeFromFeed;
            DisablePurchase.Checked = _Product.DisablePurchase;
            GiftCertificate.Checked = _Product.IsGiftCertificate;
            IsProhibited.Checked = _Product.IsProhibited;
            HidePrice.Checked = _Product.HidePrice;
            UseVariablePrice.Checked = _Product.UseVariablePrice;
            VariablePriceFields.Visible = UseVariablePrice.Checked;
            if (_Product.MinimumPrice >= 0) MinPrice.Text = _Product.MinimumPrice.ToString("F2");
            if (_Product.MaximumPrice >= 0) MaxPrice.Text = _Product.MaximumPrice.ToString("F2");
            AllowReviews.Checked = _Product.AllowReviews;
            CustomUrl.Text = _Product.CustomUrl;
            CustomUrlValidator.OriginalValue = _Product.CustomUrl;
            //TAXES AND SHIPPING
            SetSelectedItem(IsShippable, _Product.ShippableId.ToString());
            Weight.Text = string.Format("{0:F2}", _Product.Weight);
            WeightUnit.Text = Token.Instance.Store.WeightUnit.ToString();
            Length.Text = string.Format("{0:F2}", _Product.Length);
            Width.Text = string.Format("{0:F2}", _Product.Width);
            Height.Text = string.Format("{0:F2}", _Product.Height);
            MeasurementUnit.Text = Token.Instance.Store.MeasurementUnit.ToString();
            //warehouse set by databbound event
            //wragpgroup set by deatabound event
            //taxcode set by databound event
            CostOfGoods.Text = String.Format("{0:F2}", _Product.CostOfGoods);
            //vendor set by databound event

            //INVENTORY CONTROL
            MinQuantity.Text = _Product.MinQuantity.ToString();
            MaxQuantity.Text = _Product.MaxQuantity.ToString();
            Store store = Token.Instance.Store;
            if (!store.EnableInventory)
            {
                trInventory.Visible = false;
                ProductInventoryPanel.Visible = false;
                VariantInventoryPanel.Visible = false;
            }
            else
            {
                ProductVariantManager vm = new ProductVariantManager(_Product.ProductId);
                if (vm.Count == 0)
                {
                    CurrentInventoryMode.Items.RemoveAt(2);
                    if (_Product.InventoryMode == InventoryMode.Variant) _Product.InventoryMode = InventoryMode.None;
                }
                int selectedIndex = 0;
                if (_Product.InventoryMode == InventoryMode.Product) selectedIndex = 1;
                if (_Product.InventoryMode == InventoryMode.Variant) selectedIndex = 2;
                CurrentInventoryMode.SelectedIndex = selectedIndex;
                ProductInventoryPanel.Visible = (selectedIndex == 1);
                VariantInventoryPanel.Visible = (selectedIndex == 2);
                if (ProductInventoryPanel.Visible)
                {
                    InStock.Text = _Product.InStock.ToString();
                    LowStock.Text = _Product.InStockWarningLevel.ToString();
                    BackOrder.Checked = _Product.AllowBackorder;
                }
                else if (VariantInventoryPanel.Visible)
                {
                    BackOrder2.Checked = _Product.AllowBackorder;
                }
            }

            //DESCRIPTIONS
            Summary.Text = _Product.Summary;
            Description.Text = _Product.Description;
            ExtendedDescription.Text = _Product.ExtendedDescription;
            HtmlHead.Text = _Product.HtmlHead;
            SearchKeywords.Text = _Product.SearchKeywords;
        }

        SummaryCharCount.Text = ((int)(Summary.MaxLength - Summary.Text.Length)).ToString();
        PageHelper.SetMaxLengthCountDown(Summary, SummaryCharCount);

        PageHelper.PreventFirefoxSubmitOnKeyPress(Summary, Summary.ClientID);
        PageHelper.PreventFirefoxSubmitOnKeyPress(Description, Description.ClientID);
        PageHelper.PreventFirefoxSubmitOnKeyPress(ExtendedDescription, ExtendedDescription.ClientID);
        PageHelper.PreventFirefoxSubmitOnKeyPress(HtmlHead, HtmlHead.ClientID);
        PageHelper.PreventFirefoxSubmitOnKeyPress(SearchKeywords, SearchKeywords.ClientID);
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        VariablePriceFields.Visible = UseVariablePrice.Checked;
        trAllowReviews.Visible = (Token.Instance.Store.Settings.ProductReviewEnabled != CommerceBuilder.Users.UserAuthFilter.None);
    }

    private void SetSelectedItem(DropDownList list, string selectedValue)
    {
        if (list.SelectedItem != null) list.SelectedItem.Selected = false;
        ListItem selectedItem = list.Items.FindByValue(selectedValue);
        if (selectedItem != null) selectedItem.Selected = true;
    }

    public void SaveButton_Click(object sender, EventArgs e)
    {
        ValidateFieldsLengths();
        if (Page.IsValid)
        {
            //BASIC INFO
            _Product.Name = Name.Text;
            _Product.Sku = Sku.Text;
            _Product.Price = AlwaysConvert.ToDecimal(Price.Text);
            _Product.MSRP = AlwaysConvert.ToDecimal(Msrp.Text);
            _Product.ManufacturerId = AlwaysConvert.ToInt(ManufacturerList.SelectedValue);
            _Product.ModelNumber = ModelNumber.Text;
            _Product.Visibility = (CatalogVisibility)Visibility.SelectedIndex;
            _Product.IsFeatured = Featured.Checked;
            _Product.ExcludeFromFeed = ExcludeFromFeed.Checked;
            _Product.DisplayPage = DisplayPage.SelectedValue;
            _Product.Theme = LocalTheme.SelectedValue;
            _Product.DisablePurchase = DisablePurchase.Checked;
            _Product.IsGiftCertificate = GiftCertificate.Checked;
            _Product.IsProhibited = IsProhibited.Checked;
            _Product.HidePrice = HidePrice.Checked;
            _Product.UseVariablePrice = UseVariablePrice.Checked;
            _Product.MinimumPrice = AlwaysConvert.ToDecimal(MinPrice.Text);
            _Product.MaximumPrice = AlwaysConvert.ToDecimal(MaxPrice.Text);
            _Product.AllowReviews = AllowReviews.Checked;

            _Product.CustomUrl = CustomUrl.Text.Trim();

            //TAXES AND SHIPPING
            _Product.ShippableId = (byte)AlwaysConvert.ToInt(IsShippable.SelectedIndex);
            _Product.Weight = AlwaysConvert.ToDecimal(Weight.Text);
            _Product.Length = AlwaysConvert.ToDecimal(Length.Text);
            _Product.Width = AlwaysConvert.ToDecimal(Width.Text);
            _Product.Height = AlwaysConvert.ToDecimal(Height.Text);
            _Product.WarehouseId = AlwaysConvert.ToInt(Warehouse.SelectedValue);
            _Product.WrapGroupId = AlwaysConvert.ToInt(WrapGroup.SelectedValue);
            _Product.TaxCodeId = AlwaysConvert.ToInt(TaxCode.SelectedValue);
            _Product.CostOfGoods = AlwaysConvert.ToDecimal(CostOfGoods.Text);
            _Product.VendorId = AlwaysConvert.ToInt(Vendor.SelectedValue);

            //INVENTORY CONTROL
            _Product.MinQuantity = AlwaysConvert.ToInt16(MinQuantity.Text);
            _Product.MaxQuantity = AlwaysConvert.ToInt16(MaxQuantity.Text);
            _Product.InventoryModeId = AlwaysConvert.ToByte(CurrentInventoryMode.SelectedIndex);
            if (_Product.InventoryMode == InventoryMode.Product)
            {
                _Product.InStock = AlwaysConvert.ToInt(InStock.Text);
                _Product.InStockWarningLevel = AlwaysConvert.ToInt(LowStock.Text);
                _Product.AllowBackorder = BackOrder.Checked;
            }
            else if (_Product.InventoryMode == InventoryMode.Variant)
            {
                _Product.AllowBackorder = BackOrder2.Checked;
            }

            //DESCRIPTIONS
            _Product.Summary = Summary.Text;
            _Product.Description = Description.Text;
            _Product.ExtendedDescription = ExtendedDescription.Text;
            //_Product.IsProhibited = IsProhibited.Checked;
            _Product.HtmlHead = HtmlHead.Text;
            _Product.SearchKeywords = SearchKeywords.Text;

            //SAVE PRODUCT
            _Product.Save();
            SavedMessage.Visible = true;
            SavedMessage.Text = string.Format(SavedMessage.Text, LocaleHelper.LocalNow);

            // UPDATE THE CAPTION
            Caption.Text = _Product.Name;
            Page.Title = "Edit Product '" + _Product.Name + "'";
            if (((Control)sender).ID == "SaveAndCloseButton" || ((Control)sender).ID == "SaveAndCloseButton1") Response.Redirect(CancelButton1.NavigateUrl);
        }
    }

    private void ValidateFieldsLengths()
    {
        if (Summary.Text.Length > 1024)
        {
            SummaryValidator.ErrorMessage = "Text for Summary  exceeds the maximum allowed length 1024 characters";
            SummaryValidator.IsValid = false;
        }
    }

    protected void Warehouse_DataBound(object sender, EventArgs e)
    {
        if (_Product != null) SetSelectedItem(Warehouse, _Product.WarehouseId.ToString());
    }

    protected void WrapGroup_DataBound(object sender, EventArgs e)
    {
        if (_Product != null) SetSelectedItem(WrapGroup, _Product.WrapGroupId.ToString());
    }

    protected void TaxCode_DataBound(object sender, EventArgs e)
    {
        if (_Product != null) SetSelectedItem(TaxCode, _Product.TaxCodeId.ToString());
    }

    protected void Vendor_DataBound(object sender, EventArgs e)
    {
        if (_Product != null) SetSelectedItem(Vendor, _Product.VendorId.ToString());
    }

    protected void CurrentInventoryMode_SelectedIndexChanged(object sender, EventArgs e)
    {
        ProductInventoryPanel.Visible = (CurrentInventoryMode.SelectedIndex == 1);
        VariantInventoryPanel.Visible = (CurrentInventoryMode.SelectedIndex == 2);
    }

    protected void BindDisplayPage()
    {
        List<CommerceBuilder.UI.Styles.DisplayPage> displayPages = CommerceBuilder.UI.Styles.DisplayPageDataSource.Load();
        foreach (CommerceBuilder.UI.Styles.DisplayPage displayPage in displayPages)
        {
            if (displayPage.NodeType == CatalogNodeType.Product)
            {
                string displayName = string.Format("{0}", displayPage.Name);
                DisplayPage.Items.Add(new ListItem(displayName, displayPage.DisplayPageFile));
            }
        }
        string currentDisplayPage = _Product.DisplayPage;
        ListItem selectedItem = DisplayPage.Items.FindByValue(currentDisplayPage);
        if (selectedItem != null) DisplayPage.SelectedIndex = DisplayPage.Items.IndexOf(selectedItem);
    }

    protected void BindThemes()
    {
        CommerceBuilder.UI.Styles.Theme[] themes = StoreDataHelper.GetStoreThemes();
        foreach (CommerceBuilder.UI.Styles.Theme theme in themes)
        {
            LocalTheme.Items.Add(new ListItem(theme.DisplayName, theme.Name));
        }
        if (!Page.IsPostBack)
        {
            string currentTheme = _Product.Theme;
            ListItem selectedItem = LocalTheme.Items.FindByValue(currentTheme);
            if (selectedItem != null) LocalTheme.SelectedIndex = LocalTheme.Items.IndexOf(selectedItem);
        }
    }

    private void BindManufacturers()
    {
        ManufacturerList.Items.Clear();
        ManufacturerList.Items.Add(new ListItem(string.Empty));
        ManufacturerList.DataSource = ManufacturerDataSource.LoadForStore("Name");
        ManufacturerList.DataBind();
        SetSelectedItem(ManufacturerList, _Product.ManufacturerId.ToString());
    }

    protected void NewManufacturerLink_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(NewManufacturerName.Value))
        {
            Manufacturer manufacturer = ManufacturerDataSource.LoadForName(NewManufacturerName.Value, true);
            _Product.ManufacturerId = manufacturer.ManufacturerId;
            BindManufacturers();
        }
    }
}
