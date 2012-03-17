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
using System.Collections.Generic;
using CommerceBuilder.Orders;
using CommerceBuilder.Products;
using CommerceBuilder.Utility;
using CommerceBuilder.Shipping;
using CommerceBuilder.Stores;
using CommerceBuilder.Common;

public partial class Admin_Orders_Edit_AddProductDialog : System.Web.UI.UserControl
{
    Dictionary<int, int> _SelectedOptions = null;
    List<int> _SelectedKitProducts = null;
    private int _OrderId;
    private Order _Order;
    private int _ProductId;
    private Product _Product;
    private int _OrderShipmentId;
    private OrderShipment _OrderShipment;

    protected int OrderId
    {
        set
        {
            _OrderId = value;
            _Order = OrderDataSource.Load(_OrderId);
        }
        get { return _OrderId; }
    }

    protected int OrderShipmentId
    {
        set
        {
            _OrderShipmentId = value;
            _OrderShipment = OrderShipmentDataSource.Load(_OrderShipmentId);
            if (_OrderShipment != null) _Order = _OrderShipment.Order;
        }
        get { return _OrderShipmentId; }
    }

    public void Page_Init(object sender, EventArgs e)
    {
        if (_Order == null) OrderId = AlwaysConvert.ToInt(Request.QueryString["OrderId"]);
        if (_OrderShipment == null) OrderShipmentId = AlwaysConvert.ToInt(Request.QueryString["OrderShipmentId"]);
        if (_Order == null) Response.Redirect("~/Admin/Orders/Default.aspx");
        _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        _Product = ProductDataSource.Load(_ProductId);


        if (_Product == null)
        {
            if (_OrderShipment == null) Response.Redirect("~/Admin/Orders/Edit/FindProduct.aspx?OrderNumber=" + _Order.OrderNumber.ToString() + "&OrderId=" + _OrderId.ToString());
            else Response.Redirect("~/Admin/Orders/Shipments/FindProduct.aspx?OrderShipmentId=" + _OrderShipmentId.ToString());
        }

        if (_OrderShipment == null) BackButton.NavigateUrl = "~/Admin/Orders/Edit/EditOrderItems.aspx?OrderNumber=" + _Order.OrderNumber.ToString() + "&OrderId=" + _OrderId.ToString();
        else BackButton.NavigateUrl = "~/Admin/Orders/Shipments/EditShipment.aspx?OrderShipmentId=" + _OrderShipmentId.ToString();

        //SET NAME AND PRICE
        Name.Text = _Product.Name;
        HiddenProductId.Value = _Product.ProductId.ToString();
        //BUILD PRODUCT ATTRIBUTES
        _SelectedOptions = ProductHelper.BuildProductOptions(_Product, phOptions, true);
        //BUILD PRODUCT CHOICES
        ProductHelper.BuildProductChoices(_Product, phOptions);
        //BUILD KIT OPTIONS
        _SelectedKitProducts = ProductHelper.BuildKitOptions(_Product, phOptions);
        //SET PRICE
        string optionList = ProductVariantDataSource.GetOptionList(_Product.ProductId, _SelectedOptions, true);
        ProductCalculator pcalc = ProductCalculator.LoadForProduct(_Product.ProductId, 1, optionList, AlwaysConvert.ToList(",", _SelectedKitProducts), _Order.UserId);
        Price.Text = string.Format("{0:F2}", pcalc.Price);
        // IF ALL OPTIONS HAVE A VALUE SELECTED, SHOW THE BASKET CONTROLS
        SaveButton.Visible = (_SelectedOptions.Count >= _Product.ProductOptions.Count);
        //BackButton.NavigateUrl += "?OrderNumber=" + _Order.OrderNumber.ToString() + "&OrderId=" + _OrderId.ToString();


        //CHECK IF SHIPMENTS NEED TO BE DISPLAYED
        trShipment.Visible = (_OrderShipment == null && _Product.Shippable != Shippable.No);
        if (trShipment.Visible)
        {
            //BIND SHIPMENT LIST
            foreach (OrderShipment shipment in _Order.Shipments)
            {
                string address = string.Format("{0} {1} {2} {3}", shipment.ShipToFirstName, shipment.ShipToLastName, shipment.ShipToAddress1, shipment.ShipToCity);
                if (address.Length > 50) address = address.Substring(0, 47) + "...";
                string name = "Shipment #" + shipment.ShipmentNumber + " to " + address;
                ShipmentsList.Items.Add(new ListItem(name, shipment.OrderShipmentId.ToString()));
            }

            if (_Order.Shipments != null && _Order.Shipments.Count == 1)
            {
                // IF THERE IS JUST ONLY ONE SHIPMENT THEN SELECT IT
                ShipmentsList.SelectedIndex = 1;
            }
        }
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        List<OrderItem> orderItems = GetOrderItems();
        if ((orderItems != null) && (orderItems.Count > 0))
        {
            int shipmentId;
            if (_OrderShipment == null) shipmentId = AlwaysConvert.ToInt(ShipmentsList.SelectedValue);
            else shipmentId = _OrderShipmentId;
            foreach (OrderItem item in orderItems)
            {
                item.OrderShipmentId = shipmentId;
                _Order.Items.Add(item);
            }
            _Order.Save();

            // IF IT IS A GIFT CERTIFICATE PRODUCT ITEM, GENERATE GIFT CERTIFICATES
            //NOTE: HAVE TO DO IT AFTER (ADDING THE ORDER ITEM TO ORDER + SAVING THE ORDER)
            foreach (OrderItem item in orderItems)
            {
                if (item.Product != null && item.Product.IsGiftCertificate)
                {
                    item.GenerateGiftCertificates(false);
                    item.Save();
                }

                //GENERATE (BUT DO NOT ACTIVATE) SUBSCRIPTIONS (IF THERE ARE ANY)
                item.GenerateSubscriptions(false);
            }            
        }

        if (_OrderShipment == null) Response.Redirect("~/Admin/Orders/Edit/EditOrderItems.aspx?OrderNumber=" + _Order.OrderNumber.ToString() + "&OrderId=" + _OrderId.ToString());
        else Response.Redirect("~/Admin/Orders/Shipments/EditShipment.aspx?OrderShipmentId=" + _OrderShipmentId.ToString());
    }

    protected void Price_PreRender(object sender, EventArgs e)
    {
        int productId = AlwaysConvert.ToInt(HiddenProductId.Value);
        Product product = ProductDataSource.Load(productId);
        if (product != null)
        {
            //GET THE SELECTED KIT OPTIONS
            GetSelectedKitOptions(product);
        }
        //SET THE CURRENT CALCULATED PRICE
        string optionList = ProductVariantDataSource.GetOptionList(productId, _SelectedOptions, true);
        ProductCalculator pcalc = ProductCalculator.LoadForProduct(productId, 1, optionList, AlwaysConvert.ToList(",", _SelectedKitProducts));
        Price.Text = string.Format("{0:F2}", pcalc.Price);

        if (product.UseVariablePrice)
        {
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

        // CHANGING PRODUCT PRICE OPTION SHOULD NOT AVAILABLE FOR KIT PRODUCTS
        Price.Enabled = (product.KitStatus != KitStatus.Master);

        InventoryAlertUpdate();
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

    protected List<OrderItem> GetOrderItems()
    {
        //GET THE PRODUCT ID
        int productId = AlwaysConvert.ToInt(HiddenProductId.Value);
        Product product = ProductDataSource.Load(productId);
        if (product == null) return null;
        //GET THE QUANTITY
        short tempQuantity = AlwaysConvert.ToInt16(Quantity.Text);
        if (tempQuantity < 1) return null;
        if (tempQuantity > System.Int16.MaxValue) tempQuantity = System.Int16.MaxValue;

        //RECALCULATE SELECTED KIT OPTIONS
        GetSelectedKitOptions(product);
        // DETERMINE THE OPTION LIST
        string optionList = ProductVariantDataSource.GetOptionList(productId, _SelectedOptions, false);
        //CREATE THE BASKET ITEM WITH GIVEN OPTIONS

        List<OrderItem> orderItems = OrderItemDataSource.CreateForProduct(productId, tempQuantity, optionList, AlwaysConvert.ToList(",", _SelectedKitProducts));
        if (orderItems.Count > 0)
        {
            // COLLECT ANY ADDITIONAL INPUTS FOR BASE ITEM
            ProductHelper.CollectProductTemplateInput(orderItems[0], phOptions);

            // UPADATE THE PRICE OF BASE ITEM IF NEEDED ( KIT PRICE WILL NOT BE MODIFIED)
            if (orderItems[0].Price != AlwaysConvert.ToDecimal(Price.Text) && (product.KitStatus != KitStatus.Master))
            {
                orderItems[0].Price = AlwaysConvert.ToDecimal(Price.Text);
            }
        }
        return orderItems;
    }

    protected void InventoryAlertUpdate()
    {
        if (_Product != null)
        {
            // WE HAVE A VALID PRODUCT, ARE ANY AVAILABLE OPTIONS SELECTED?
            bool allProductOptionsSelected = (_SelectedOptions.Count == _Product.ProductOptions.Count);
            if (allProductOptionsSelected)
            {
                // OPTIONS ARE GOOD, VERIFY ANY REQUIRED KIT OPTIONS ARE SELECTED
                GetSelectedKitOptions(_Product);
                bool requiredKitOptionsSelected = ProductHelper.RequiredKitOptionsSelected(_Product, _SelectedKitProducts);
                if (requiredKitOptionsSelected)
                {
                    // KIT OPTIONS ARE ALSO VALID, DETERMINE APPROPRIATE WARNINGS
                    Store store = Token.Instance.Store;
                    List<string> warningMessages = new List<string>();

                    string optionList = string.Empty;
                    if (_Product.ProductOptions.Count > 0)
                    {
                        // OPTIONS ARE PRESENT, CHECK AVAILABLILITY
                        optionList = ProductVariantDataSource.GetOptionList(_ProductId, _SelectedOptions, true);
                        ProductVariant variant = ProductVariantDataSource.LoadForOptionList(_Product.ProductId, optionList);
                        if (!variant.Available) warningMessages.Add("The selected variant is marked as unavailable.");

                        // WE ALSO NEED TO ALERT INVENTORY IF ENABLED AT VARIANT LEVEL AND THIS IS NOT A KIT
                        if (store.EnableInventory
                            && _Product.KitStatus != KitStatus.Master
                            && _Product.InventoryMode == InventoryMode.Variant)
                            warningMessages.Add("The selected variant has a current stock level of " + variant.InStock + ".");
                    }

                    // CHECK STOCK QUANTITY FOR PRODUCT, IF STORE INVENTORY IS ENABLED
                    // AND THE STOCK IS MANAGED AT THE PRODUCT LEVEL OR THIS IS A KIT
                    if (store.EnableInventory && (_Product.InventoryMode == InventoryMode.Product || _Product.KitStatus == KitStatus.Master))
                    {
                        InventoryManagerData inv = InventoryManager.CheckStock(_ProductId, optionList, _SelectedKitProducts);
                        if (!inv.AllowBackorder)
                        {
                            if (_Product.KitStatus == KitStatus.Master)
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
}
