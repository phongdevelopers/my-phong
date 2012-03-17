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
using CommerceBuilder.Catalog;
using CommerceBuilder.Common;
using CommerceBuilder.Products;
using CommerceBuilder.Utility;

public partial class Admin_Products_Kits_AddKitProducts : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private int _CategoryId;
    private Category _Category;
    private int _ProductId;
    private Product _Product;
    private int _KitComponentId;
    private KitComponent _KitComponent;
    private List<RowOptionChoice> _SelectedOptions = new List<RowOptionChoice>();
    private List<Product> _SelectedProducts = new List<Product>();
    private List<int> _SelectedProductIds = new List<int>();

    protected void Page_Init(object sender, EventArgs e)
    {
        //INITIALIZE VARIABLES
        _CategoryId = PageHelper.GetCategoryId();
        _Category = CategoryDataSource.Load(_CategoryId);
        _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        _Product = ProductDataSource.Load(_ProductId);
        if (_Product == null) Response.Redirect(NavigationHelper.GetAdminUrl("Catalog/Browse.aspx?CategoryId=" + _CategoryId.ToString()));
        _KitComponentId = AlwaysConvert.ToInt(Request.QueryString["KitComponentId"]);
        _KitComponent = KitComponentDataSource.Load(_KitComponentId);
        if (_KitComponent == null) Response.Redirect("EditKit.aspx?CategoryId=" + _CategoryId.ToString() + "&ProductId=" + _ProductId.ToString());
        CancelLink.NavigateUrl += "?CategoryId=" + _CategoryId.ToString() + "&ProductId=" + _ProductId.ToString();
        Caption.Text = string.Format(Caption.Text, _Product.Name);
        KitName.Text = _Product.Name;
        KitComponentName.Text = _KitComponent.Name;

        // INITIALIZE DROP DOWN LISTS
        InitializeCategoryTree();
        ManufacturerFilter.DataSource = ManufacturerDataSource.LoadForStore("Name");
        ManufacturerFilter.DataBind();
        VendorFilter.DataSource = VendorDataSource.LoadForStore("Name");
        VendorFilter.DataBind();

        // LOAD CUSTOM VIEWSTATE VARIABLES
        LoadCustomViewState();
        if (_SelectedProducts.Count > 0)
        {
            // REBIND THE REPEATER
            BindSelectedProducts(_SelectedProducts);
        }

        PageHelper.SetDefaultButton(NameFilter, SearchButton.ClientID);
        PageHelper.SetDefaultButton(SkuFilter, SearchButton.ClientID);
    }

    private void AddProductToList(int productId)
    {
        if (!_SelectedProductIds.Contains(productId))
        {
            Product product = ProductDataSource.Load(productId);
            if (product != null && product.KitStatus != KitStatus.Master)
            {
                _SelectedProducts.Add(product);
                _SelectedProductIds.Add(productId);
            }
        }
    }

    private void LoadCustomViewState()
    {
        if (Page.IsPostBack)
        {
            string vsContent = Request.Form[VS.UniqueID];
            string decodedContent = EncryptionHelper.DecryptAES(vsContent);
            UrlEncodedDictionary customViewState = new UrlEncodedDictionary(decodedContent);
            string selectedProducts = customViewState.TryGetValue("SP");
            if (!string.IsNullOrEmpty(selectedProducts))
            {
                int[] tempProductIds = AlwaysConvert.ToIntArray(selectedProducts);
                if (tempProductIds != null && tempProductIds.Length > 0)
                {
                    for (int i = 0; i < tempProductIds.Length; i++)
                    {
                        AddProductToList(tempProductIds[i]);
                    }
                    BindSelectedProducts(_SelectedProducts);
                }
            }
        }
    }


    protected void InitializeCategoryTree()
    {
        CategoryLevelNodeCollection categories = CategoryParentDataSource.GetCategoryLevels(0);
        foreach (CategoryLevelNode node in categories)
        {
            string prefix = string.Empty;
            for (int i = 0; i <= node.CategoryLevel; i++) prefix += " . . ";
            CategoryFilter.Items.Add(new ListItem(prefix + node.Name, node.CategoryId.ToString()));
        }
    }

    protected void SearchButton_Click(object sender, EventArgs e)
    {
        ConfigurePanel.Visible = false;
        SearchPanel.Visible = true;
        ProductSearchResults.Visible = true;
        ProductSearchResults.DataBind();
        // ONLY SHOW THE ADD BUTTON IF RESULTS ARE AVAILABLE
        AddProductsButton.Visible = (ProductSearchResults.Rows.Count > 0);
        foreach (GridViewRow gvr in ProductSearchResults.Rows)
        {
            CheckBox cb = (CheckBox)gvr.FindControl("Selected");
            ScriptManager.RegisterArrayDeclaration(ProductSearchResults, "CheckBoxIDs", "'" + cb.ClientID + "'");
        }
    }

    protected void AddProductsButton_Click(object sender, EventArgs e)
    {
        // LOOP ROWS AND BUILD LIST OF CHECKED ITEMS
        _SelectedProducts = new List<Product>();
        _SelectedProductIds = new List<int>();
        for (int i = 0; i < ProductSearchResults.Rows.Count; i++)
        {
            CheckBox selected = (CheckBox)ProductSearchResults.Rows[i].FindControl("Selected");
            if (selected.Checked)
            {
                AddProductToList(AlwaysConvert.ToInt(ProductSearchResults.DataKeys[i].Value));
            }
        }

        // BIND PRODUCTS THAT WERE SELECTED
        BindSelectedProducts(_SelectedProducts);
    }

    private void BindSelectedProducts(List<Product> productList)
    {
        if (productList != null && productList.Count > 0)
        {
            // SHOW THE ADD FORM FOR THESE PRODUCTS
            SearchPanel.Visible = false;
            ConfigurePanel.Visible = true;
            SelectedProductRepeater.DataSource = productList;
            SelectedProductRepeater.DataBind();
        }
        else
        {
            // MAKE SURE THE SEARCH FORM IS PROPERY DISPLAYED
            SearchPanel.Visible = true;
            ConfigurePanel.Visible = false;
        }
    }

    protected string GetNameFormat(object dataItem)
    {
        Product p = (Product)dataItem;
        if (p.ProductOptions.Count > 0)
        {
            return "$name ($options)";
        }
        return "$name";
    }

    protected bool IsNotKit(object dataItem)
    {
        return ((Product)dataItem).KitStatus != KitStatus.Master;
    }

    protected bool IsNotHiddenPart(object dataItem)
    {
        return !(_KitComponent.InputType == KitInputType.IncludedHidden || _KitComponent.InputType == KitInputType.IncludedShown);
    }

    #region Configure Products

    protected void SelectedProductRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            PlaceHolder phOptions = (PlaceHolder)PageHelper.RecursiveFindControl(e.Item, "phOptions");
            Product p = (Product)e.Item.DataItem;
            phOptions.Controls.Clear();
            Dictionary<int, int> selectedOptions = ProductHelper.BuildProductOptions(p, phOptions, true);
            // REMOVE ANY SELECTED OPTIONS FOR THIS ROW
            for (int i = _SelectedOptions.Count - 1; i >= 0; i--)
            {
                if (_SelectedOptions[i].ItemIndex == e.Item.ItemIndex)
                    _SelectedOptions.RemoveAt(i);
            }
            // GENERATE THE ROW OPTION CHOICES
            foreach (int optionId in selectedOptions.Keys)
            {
                _SelectedOptions.Add(new RowOptionChoice(e.Item.ItemIndex, p.ProductId, optionId, selectedOptions[optionId]));
            }
        }
    }

    protected void PriceMode_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList priceMode = (DropDownList)sender;
        RepeaterItem ri = (RepeaterItem)priceMode.Parent;
        Label FixedPriceLabel = (Label)ri.FindControl("FixedPriceLabel");
        Label ModifyPriceLabel = (Label)ri.FindControl("ModifyPriceLabel");
        HtmlTableRow trPrice = (HtmlTableRow)ri.FindControl("trPrice");
        switch (priceMode.SelectedIndex)
        {
            case 0:
                FixedPriceLabel.Visible = false;
                ModifyPriceLabel.Visible = false;
                trPrice.Visible = false;
                break;
            case 1:
                FixedPriceLabel.Visible = false;
                ModifyPriceLabel.Visible = true;
                trPrice.Visible = true;
                break;
            case 2:
                FixedPriceLabel.Visible = true;
                ModifyPriceLabel.Visible = false;
                trPrice.Visible = true;
                break;
        }
    }

    protected void WeightMode_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList weightMode = (DropDownList)sender;
        RepeaterItem ri = (RepeaterItem)weightMode.Parent;
        Label FixedWeightLabel = (Label)ri.FindControl("FixedWeightLabel");
        Label ModifyWeightLabel = (Label)ri.FindControl("ModifyWeightLabel");
        HtmlTableRow trWeight = (HtmlTableRow)ri.FindControl("trWeight");
        switch (weightMode.SelectedIndex)
        {
            case 0:
                FixedWeightLabel.Visible = false;
                ModifyWeightLabel.Visible = false;
                trWeight.Visible = false;
                break;
            case 1:
                FixedWeightLabel.Visible = false;
                ModifyWeightLabel.Visible = true;
                trWeight.Visible = true;
                break;
            case 2:
                FixedWeightLabel.Visible = true;
                ModifyWeightLabel.Visible = false;
                trWeight.Visible = true;
                break;
        }
    }

    private struct RowOptionChoice
    {
        public int ItemIndex;
        public int ProductId;
        public int OptionId;
        public int OptionChoiceId;
        public RowOptionChoice(int itemIndex, int productId, int optionId, int optionChoiceId)
        {
            ItemIndex = itemIndex;
            ProductId = productId;
            OptionId = optionId;
            OptionChoiceId = optionChoiceId;
        }
    }

    #endregion

    private Dictionary<int, int> GetSelectedOptions(int itemIndex)
    {
        Dictionary<int, int> selectedOptions = new Dictionary<int, int>();
        foreach (RowOptionChoice roc in _SelectedOptions)
        {
            if (roc.ItemIndex == itemIndex)
                selectedOptions.Add(roc.OptionId, roc.OptionChoiceId);
        }
        return selectedOptions;
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        foreach (RepeaterItem ri in SelectedProductRepeater.Items)
        {
            HiddenField pid = (HiddenField)ri.FindControl("PID");
            int productId = AlwaysConvert.ToInt(pid.Value);
            Product p = ProductDataSource.Load(productId);
            if (p != null && p.ProductOptions.Count > 0)
            {
                Dictionary<int, int> selectedOptions = GetSelectedOptions(ri.ItemIndex);
                bool allOptionsSelected = (selectedOptions.Count == p.ProductOptions.Count);
                string optionList = ProductVariantDataSource.GetOptionList(p.ProductId, selectedOptions, true);
                ProductCalculator pcalc = ProductCalculator.LoadForProduct(p.ProductId, 1, optionList, string.Empty);
                Literal productPrice = (Literal)ri.FindControl("ProductPrice");
                productPrice.Text = pcalc.Price.ToString("lc");
                if (allOptionsSelected)
                {
                    ProductVariant pv = ProductVariantDataSource.LoadForOptionList(p.ProductId, optionList);
                    if (pv != null && !pv.Available)
                    {
                        HtmlTableRow trInvalidVariant = (HtmlTableRow)ri.FindControl("trInvalidVariant");
                        if (trInvalidVariant != null) trInvalidVariant.Visible = true;
                    }
                }
            }
        }

        // SAVE THE CUSTOM VIEWSTATE
        SaveCustomViewState();
    }

    private void SaveCustomViewState()
    {
        UrlEncodedDictionary customViewState = new UrlEncodedDictionary();
        if (_SelectedProductIds.Count > 0)
            customViewState["SP"] = AlwaysConvert.ToList(",", _SelectedProductIds.ToArray());
        else customViewState["SP"] = string.Empty;
        VS.Value = EncryptionHelper.EncryptAES(customViewState.ToString());
    }

    protected void FinishButton_Click(object sender, EventArgs e)
    {
        foreach (RepeaterItem ri in SelectedProductRepeater.Items)
        {
            HiddenField pid = (HiddenField)ri.FindControl("PID");
            int productId = AlwaysConvert.ToInt(pid.Value);
            Product product = ProductDataSource.Load(productId);
            if (product == null) return;
            if (!AllProductOptionsSelected(product, ri)) return;
            TextBox KitQuantity = (TextBox)ri.FindControl("KitQuantity");
            short quantity = AlwaysConvert.ToInt16(KitQuantity.Text);
            if (quantity > 0)
            {
                ProductVariant productVariant = null;
                string optionList = string.Empty;
                if (product.ProductOptions.Count > 0)
                {
                    productVariant = new ProductVariant();
                    Dictionary<int, int> selectedOptions = GetSelectedOptions(ri.ItemIndex);
                    productVariant.Load(productId, selectedOptions);
                    optionList = ProductVariantDataSource.GetOptionList(productId, selectedOptions, true);
                }
                TextBox NameFormat = (TextBox)ri.FindControl("NameFormat");
                DropDownList PriceMode = (DropDownList)ri.FindControl("PriceMode");
                TextBox Price = (TextBox)ri.FindControl("Price");
                DropDownList WeightMode = (DropDownList)ri.FindControl("WeightMode");
                TextBox Weight = (TextBox)ri.FindControl("Weight");
                CheckBox IsSelected = (CheckBox)ri.FindControl("IsSelected");
                KitProduct kitProduct = new KitProduct();
                kitProduct.ProductId = productId;
                kitProduct.KitComponentId = _KitComponentId;
                if (productVariant != null) kitProduct.OptionList = productVariant.OptionList;
                kitProduct.Name = NameFormat.Text;
                kitProduct.Quantity = quantity;
                ProductCalculator pcalc = ProductCalculator.LoadForProduct(productId, 1, optionList, string.Empty);
                kitProduct.PriceModeId = AlwaysConvert.ToByte(PriceMode.SelectedIndex);
                if (kitProduct.PriceMode == InheritanceMode.Inherit)
                    kitProduct.Price = pcalc.Price;
                else
                    kitProduct.Price = AlwaysConvert.ToDecimal(Price.Text);
                kitProduct.WeightModeId = AlwaysConvert.ToByte(WeightMode.SelectedIndex);
                if (kitProduct.WeightMode == InheritanceMode.Inherit)
                    kitProduct.Weight = pcalc.Weight;
                else kitProduct.Weight = AlwaysConvert.ToDecimal(Weight.Text);
                kitProduct.IsSelected = IsSelected.Checked;
                kitProduct.Save();
            }
        }
        Response.Redirect(string.Format("EditKit.aspx?CategoryId={0}&ProductId={1}", _CategoryId, _ProductId));
    }

    private bool AllProductOptionsSelected(Product selectedProduct, RepeaterItem ri)
    {
        if (selectedProduct.ProductOptions.Count > 0)
        {
            HtmlTableRow trOptionWarning = (HtmlTableRow)ri.FindControl("trOptionWarning");
            Dictionary<int, int> selectedOptions = GetSelectedOptions(ri.ItemIndex);
            trOptionWarning.Visible = (selectedOptions.Count != selectedProduct.ProductOptions.Count);
            return !trOptionWarning.Visible;
        }
        return true;
    }
}
