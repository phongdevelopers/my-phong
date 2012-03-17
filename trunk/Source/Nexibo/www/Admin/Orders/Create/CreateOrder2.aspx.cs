using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommerceBuilder.Common;
using CommerceBuilder.Orders;
using CommerceBuilder.Products;
using CommerceBuilder.Stores;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;

public partial class Admin_Orders_Create_CreateOrder2 : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private int _UserId;
    private User _User;
    private Basket _Basket;
    private OrderItemType[] displayItemTypes = { OrderItemType.Product, OrderItemType.Discount };
    private Dictionary<int, int> _SelectedOptions = null;
    List<int> _SelectedKitProducts = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        // LOCATE THE USER THAT THE ORDER IS BEING PLACED FOR
        _UserId = AlwaysConvert.ToInt(Request.QueryString["UID"]);
        _User = UserDataSource.Load(_UserId);
        if (_User == null) Response.Redirect("CreateOrder1.aspx");
        _Basket = _User.Basket;

        // INITIALIZE THE CAPTION
        string userName = _User.IsAnonymous ? "Unregistered User" : _User.UserName;
        Caption.Text = string.Format(Caption.Text, userName);

        // SEE IF THE ADD PRODUCT FORM SHOULD BE VISIBLE
        int productId = AlwaysConvert.ToInt(Request.Form[AddProductId.UniqueID]);
        Product product = ProductDataSource.Load(productId);
        if (product != null)
        {
            ShowProductForm(product);
        }
    }

    protected void Page_PreRender(object sender, System.EventArgs e)
    {
        //GET THE BASKET AND RECALCULATE IT
        _Basket.Recalculate();
        //BIND THE BASKET GRID
        BindBasketGrid();
        InventoryAlertUpdate();
    }

    protected string GetConfirmDelete(object dataItem)
    {
        BasketItem item = (BasketItem)dataItem;
        string name = item.Name;
        if (item.OrderItemType == OrderItemType.Product && item.ProductVariant != null)
        {
            name += " (" + item.ProductVariant.VariantName + ")";
        }
        return string.Format("return confirm('Are you sure you want to delete {0}?\')", name.Replace("'", "\\'"));
    }

    private void BindBasketGrid()
    {
        _Basket.Package();
        _Basket.Recalculate();
        BasketItemCollection displayItems = new BasketItemCollection();
        foreach (BasketItem item in _Basket.Items)
        {
            if (Array.IndexOf(displayItemTypes, item.OrderItemType) > -1)
            {
                if (item.OrderItemType == OrderItemType.Product && item.IsChildItem)
                {
                    // WHETHER THE CHILD ITEM DISPLAYS DEPENDS ON THE ROOT
                    BasketItem rootItem = item.GetParentItem(true);
                    if (rootItem.Product != null && rootItem.Product.Kit.ItemizeDisplay)
                    {
                        // ITEMIZED DISPLAY ENABLED, SHOW THIS CHILD ITEM
                        displayItems.Add(item);
                    }
                }
                else
                {
                    // NO ADDITIONAL CHECK REQUIRED TO INCLUDE ROOT PRODUCTS OR NON-PRODUCTS
                    displayItems.Add(item);
                }
            }
        }
        displayItems.Sort(new BasketItemComparer());
        BasketGrid.DataSource = displayItems;
        BasketGrid.DataBind();
    }

    protected void BasketGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //COMBINE FOOTER CELLS FOR SUBTOTAL
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            if (e.Row.Cells.Count > 2)
            {
                int colspan = e.Row.Cells.Count - 1;
                int iterations = e.Row.Cells.Count - 3;
                for (int i = 0; i <= iterations; i++)
                {
                    e.Row.Cells.RemoveAt(0);
                }
                e.Row.Cells[0].ColumnSpan = colspan;
            }
        }
    }

    protected bool CanDeleteBasketItem(object dataItem)
    {
        BasketItem basketItem = (BasketItem)dataItem;
        switch (basketItem.OrderItemType)
        {
            case OrderItemType.Discount:
            case OrderItemType.Handling:
            case OrderItemType.Shipping:
            case OrderItemType.Tax:
                return false;
            case OrderItemType.Product:
                return !basketItem.IsChildItem;
            default:
                return true;
        }
    }

    protected void BasketGrid_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        if (e.CommandName == "DeleteItem")
        {
            int index = _Basket.Items.IndexOf(AlwaysConvert.ToInt(e.CommandArgument.ToString()));
            if (index > -1) _Basket.Items.DeleteAt(index);
        }
    }

    protected void BasketGrid_DataBound(object sender, EventArgs e)
    {
        OrderButtonPanel.Visible = (BasketGrid.Rows.Count > 0);
    }

    protected LSDecimal GetBasketSubtotal()
    {
        return _Basket.Items.TotalPrice(OrderItemType.Product, OrderItemType.Discount);
    }

    protected void CheckoutButton_Click(object sender, EventArgs e)
    {
        SaveBasket();
        Response.Redirect("CreateOrder3.aspx?UID=" + _UserId);
    }

    protected void ClearBasketButton_Click(object sender, EventArgs e)
    {
        _Basket.Clear();
    }

    protected void UpdateButton_Click(object sender, EventArgs e)
    {
        SaveBasket();
    }

    protected void FindProductSearchButton_Click(object sender, EventArgs e)
    {
        FindProductSearchResults.Visible = true;
        FindProductSearchResults.DataBind();
    }

    protected void AddProductSaveButton_Click(object sender, EventArgs e)
    {
        // SAVE THE BASKET FOR A NEW USER
        if (_Basket.BasketId == 0)
        {
            _Basket.Save();
        }

        if (Page.IsValid)
        {
            BasketItem basketItem = CreateBasketItem();
            if (basketItem != null)
            {
                _Basket.Items.Add(basketItem);
                SaveBasket();
            }
            BasketAjax.Update();
            HideProductForm();
        }
    }

    protected void AddProductCancelButton_Click(object sender, EventArgs e)
    {
        HideProductForm();
    }

    private void SaveBasket()
    {
        int rowIndex = 0;
        foreach (GridViewRow saverow in BasketGrid.Rows)
        {
            int basketItemId = (int)BasketGrid.DataKeys[rowIndex].Value;
            int itemIndex = _Basket.Items.IndexOf(basketItemId);
            if ((itemIndex > -1))
            {
                BasketItem item = _Basket.Items[itemIndex];
                if ((item.OrderItemType == OrderItemType.Product))
                {
                    TextBox quantity = (TextBox)saverow.FindControl("Quantity");
                    if (quantity != null)
                    {
                        item.Quantity = AlwaysConvert.ToInt16(quantity.Text);
                        // Update for Minimum Maximum quantity of product
                        if (item.Quantity < item.Product.MinQuantity)
                        {
                            item.Quantity = item.Product.MinQuantity;
                            quantity.Text = item.Quantity.ToString();
                        }
                        else if ((item.Product.MaxQuantity > 0) && (item.Quantity > item.Product.MaxQuantity))
                        {
                            item.Quantity = item.Product.MaxQuantity;
                            quantity.Text = item.Quantity.ToString();
                        }
                    }
                }
                rowIndex++;
            }
        }
        // SAVE THE WHOLE BASKET
        _Basket.Save();
    }
       
    protected BasketItem CreateBasketItem()
    {
        //GET THE PRODUCT ID
        int productId = AlwaysConvert.ToInt(AddProductId.Value);
        Product product = ProductDataSource.Load(productId);
        if (product == null) return null;
        //GET THE QUANTITY
        short tempQuantity = AlwaysConvert.ToInt16(AddProductQuantity.Text);
        if (tempQuantity < 1) return null;
        //RECALCULATE SELECTED KIT OPTIONS
        GetSelectedKitOptions(product);
        // DETERMINE THE OPTION LIST
        string optionList = ProductVariantDataSource.GetOptionList(productId, _SelectedOptions, false);

        //CREATE THE BASKET ITEM WITH GIVEN OPTIONS
        BasketItem basketItem = BasketItemDataSource.CreateForProduct(productId, tempQuantity, optionList, AlwaysConvert.ToList(",", _SelectedKitProducts), _UserId);
        if (basketItem != null)
        {
            //BASKET ID
            basketItem.BasketId = _Basket.BasketId;

            // PRODUCT PRICE FOR VARIABLE PRICE PRODUCT
            if (product.UseVariablePrice) basketItem.Price = AlwaysConvert.ToDecimal(AddProductVariablePrice.Text);

            // COLLECT ANY ADDITIONAL INPUTS            
            ProductHelper.CollectProductTemplateInput(basketItem, this);
        }
        return basketItem;
    }

    protected void GetSelectedKitOptions(Product product)
    {
        _SelectedKitProducts = new List<int>();
        //COLLECT ANY KIT VALUES
        foreach (ProductKitComponent pkc in product.ProductKitComponents)
        {
            // FIND THE CONTROL
            KitComponent component = pkc.KitComponent;
            if (component.InputType == KitInputType.IncludedHidden)
            {
                foreach (KitProduct choice in component.KitProducts)
                {
                    _SelectedKitProducts.Add(choice.KitProductId);
                }
            }
            else
            {
                System.Web.UI.WebControls.WebControl inputControl = (System.Web.UI.WebControls.WebControl)PageHelper.RecursiveFindControl(phOptions, component.UniqueId);
                if (inputControl != null)
                {
                    List<int> kitProducts = component.GetControlValue(inputControl);
                    foreach (int selectedKitProductId in kitProducts)
                    {
                        _SelectedKitProducts.Add(selectedKitProductId);
                    }
                }
            }
        }
    }

    protected void FindProductSearchResults_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Add"))
        {
            int productId = AlwaysConvert.ToInt(e.CommandArgument);
            Product product = ProductDataSource.Load(productId);
            if (product != null)
            {
                if (product.UseVariablePrice || product.HasChoices)
                {
                    // NEED TO COLLECT ADDITIONAL INPUT TO ADD PRODUCT
                    ShowProductForm(product);
                }
                else
                {
                    // JUST ADD THE PRODUCT TO THE BASKET
                    BasketItem basketItem = BasketItemDataSource.CreateForProduct(productId, 1, String.Empty, String.Empty, _UserId);
                    if (basketItem != null)
                    {
                        _Basket.Items.Add(basketItem);
                        SaveBasket();
                        BasketAjax.Update();
                    }
                }
            }
        }
        FindProductSearchResults.DataBind();
    }

    protected void ShowProductForm(Product product)
    {
        AddPopup.Show();
        //SET NAME AND PRICE
        AddProductName.Text = product.Name;
        AddProductId.Value = product.ProductId.ToString();
        //BUILD PRODUCT ATTRIBUTES
        _SelectedOptions = ProductHelper.BuildProductOptions(product, phOptions, true);
        //BUILD PRODUCT CHOICES
        ProductHelper.BuildProductChoices(product, phOptions);
        //BUILD KIT OPTIONS, IGNORING INVENTORY
        _SelectedKitProducts = ProductHelper.BuildKitOptions(product, phOptions, true);
        //SET PRICE
        string optionList = ProductVariantDataSource.GetOptionList(product.ProductId, _SelectedOptions, true);
        // IF ALL OPTIONS HAVE A VALUE SELECTED, SHOW THE BASKET CONTROLS
        AddProductSaveButton.Visible = (_SelectedOptions.Count == product.ProductOptions.Count);
    }

    protected void HideProductForm()
    {
        AddProductId.Value = string.Empty;
        AddPopup.Hide();
        // HIDE THIS PANEL TO PREVENT VALUES FROM POSTED BACK
        AddProductPanel.Visible = false;
    }

    protected void AddProductPrice_PreRender(object sender, EventArgs e)
    {
        int productId = AlwaysConvert.ToInt(AddProductId.Value);
        Product product = ProductDataSource.Load(productId);
        if (product != null)
        {
            //GET THE SELECTED KIT OPTIONS
            GetSelectedKitOptions(product);
            //SET THE CURRENT CALCULATED PRICE
            string optionList = ProductVariantDataSource.GetOptionList(productId, _SelectedOptions, true);
            ProductCalculator pcalc = ProductCalculator.LoadForProduct(productId, 1, optionList, AlwaysConvert.ToList(",", _SelectedKitProducts), _UserId);
            AddProductPrice.Text = string.Format("{0:F2}", pcalc.Price);
            if (product.UseVariablePrice)
            {
                AddProductVariablePrice.Text = string.Format("{0:F2}", pcalc.Price);
                AddProductVariablePrice.Visible = true;
                string varPriceText = string.Empty;
                if (product.MinimumPrice > 0)
                {
                    if (product.MaximumPrice > 0)
                    {
                        varPriceText = string.Format("(between {0:lcf} and {1:lcf})", product.MinimumPrice, product.MaximumPrice);
                    }
                    else
                    {
                        varPriceText = string.Format("(at least {0:lcf})", product.MinimumPrice);
                    }
                }
                else if (product.MaximumPrice > 0)
                {
                    varPriceText = string.Format("({0:lcf} maximum)", product.MaximumPrice);
                }
                phVariablePrice.Controls.Add(new LiteralControl(varPriceText));
            }
            AddProductPrice.Visible = !AddProductVariablePrice.Visible;
        }
    }

    protected void InventoryAlertUpdate()
    {
        int productId = AlwaysConvert.ToInt(AddProductId.Value);
        Product product = ProductDataSource.Load(productId);
        if (product != null)
        {
            // WE HAVE A VALID PRODUCT, ARE ANY AVAILABLE OPTIONS SELECTED?
            bool allProductOptionsSelected = (_SelectedOptions.Count == product.ProductOptions.Count);
            if (allProductOptionsSelected)
            {
                // OPTIONS ARE GOOD, VERIFY ANY REQUIRED KIT OPTIONS ARE SELECTED
                GetSelectedKitOptions(product);
                bool requiredKitOptionsSelected = ProductHelper.RequiredKitOptionsSelected(product, _SelectedKitProducts);
                if (requiredKitOptionsSelected)
                {
                    // KIT OPTIONS ARE ALSO VALID, DETERMINE APPROPRIATE WARNINGS
                    Store store = Token.Instance.Store;
                    List<string> warningMessages = new List<string>();

                    string optionList = string.Empty;
                    if (product.ProductOptions.Count > 0)
                    {
                        // OPTIONS ARE PRESENT, CHECK AVAILABLILITY
                        optionList = ProductVariantDataSource.GetOptionList(productId, _SelectedOptions, true);
                        ProductVariant variant = ProductVariantDataSource.LoadForOptionList(product.ProductId, optionList);
                        if (!variant.Available) warningMessages.Add("The selected variant is marked as unavailable.");
                        
                        // WE ALSO NEED TO ALERT INVENTORY IF ENABLED AT VARIANT LEVEL AND THIS IS NOT A KIT
                        if (store.EnableInventory 
                            && product.KitStatus != KitStatus.Master
                            && product.InventoryMode == InventoryMode.Variant) 
                            warningMessages.Add("The selected variant has a current stock level of " + variant.InStock + ".");
                    }
                    
                    // CHECK STOCK QUANTITY FOR PRODUCT, IF STORE INVENTORY IS ENABLED
                    // AND THE STOCK IS MANAGED AT THE PRODUCT LEVEL OR THIS IS A KIT
                    if (store.EnableInventory && (product.InventoryMode == InventoryMode.Product || product.KitStatus == KitStatus.Master))
                    {
                        InventoryManagerData inv = InventoryManager.CheckStock(productId, optionList, _SelectedKitProducts);
                        if (!inv.AllowBackorder)
                        {
                            if (product.KitStatus == KitStatus.Master)
                            {
                                // INVENTORY MESSAGE FOR KIT PRODUCTS
                                warningMessages.Add("The selected configuration has a current stock level of " + inv.InStock + ".");
                            }
                            else
                            {
                                // NOT KIT OR VARIANT
                                warningMessages.Add("This product has a current stock level of " + inv.InStock + ".");
                            }
                        }
                    }

                    // SHOW ANY WARNING MESSAGES
                    if (warningMessages.Count > 0)
                    {
                        InventoryWarningMessage.Text = "Note: " + string.Join(" ", warningMessages.ToArray());
                        trInventoryWarning.Visible = true;
                    }
                }
            }
        }
    }

    protected void FindProductSearchResults_RowCreated(Object sender,GridViewRowEventArgs e) 
    {
        if(e.Row.RowType == DataControlRowType.DataRow)
        {
            Product product = (Product)e.Row.DataItem;
            if (product == null)
                return;
            CommerceBuilder.Stores.Store store = Token.Instance.Store;
            if (store.EnableInventory && (product.InventoryMode != InventoryMode.None) && product.ProductOptions.Count == 0 && product.ProductKitComponents.Count == 0 && (!product.AllowBackorder))
            {
                InventoryManagerData inv = InventoryManager.CheckStock(product.ProductId);
                if (inv.InStock < 1)
                {
                    ImageButton imageButton = (ImageButton)e.Row.FindControl("AddButton");
                    string outofstockWarnningScript = "return confirm('{0}')";
                    string outOfStockMessage = string.Format("Product {0} currently has {1} in stock.  Click OK to add to this order anyway.",product.Name,inv.InStock);
                    imageButton.OnClientClick = string.Format(outofstockWarnningScript, outOfStockMessage);
                    BasketAjax.Update();
                }
            }
        }
    }
}
