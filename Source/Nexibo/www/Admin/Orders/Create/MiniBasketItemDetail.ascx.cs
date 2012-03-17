using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommerceBuilder.Users;
using CommerceBuilder.Catalog;
using CommerceBuilder.DigitalDelivery;
using CommerceBuilder.Orders;
using CommerceBuilder.Products;

public partial class Admin_Orders_Create_MiniBasketItemDetail : System.Web.UI.UserControl
{
    BasketItem _BasketItem;
    public BasketItem BasketItem
    {
        get { return _BasketItem; }
        set { _BasketItem = value; }
    }

    public bool _LinkProducts = false;
    public bool LinkProducts
    {
        get { return _LinkProducts; }
        set { _LinkProducts = value; }
    }

    private bool _ShowShipTo = false;
    public bool ShowShipTo
    {
        get { return _ShowShipTo; }
        set { _ShowShipTo = value; }
    }

    private bool _ShowAssets;
    public bool ShowAssets
    {
        get { return _ShowAssets; }
        set { _ShowAssets = value; }
    }

    private bool _ShowSubscription;
    public bool ShowSubscription
    {
        get { return _ShowSubscription; }
        set { _ShowSubscription = value; }
    }

    private BasketItemInputCollection GetCustomerInputs()
    {
        BasketItemInputCollection inputs = new BasketItemInputCollection();
        foreach (BasketItemInput input in _BasketItem.Inputs)
        {
            if ((input.InputField != null) && (!input.InputField.IsMerchantField))
                inputs.Add(input);
        }
        return inputs;
    }
    
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (_BasketItem != null)
        {
            Product product = _BasketItem.Product;
            if (product != null)
            {
                //OUTPUT THE PRODUCT NAME
                string productName = product.Name;
                if (_BasketItem.ProductVariant != null)
                {
                    productName += string.Format(" ({0})", _BasketItem.ProductVariant.VariantName);
                }
                if (this.LinkProducts)
                {
                    //OUTPUT NAME AS LINK                    
                    string link = string.Format("<a target=\"_blank\" href=\"{0}\">{1}</a>", Page.ResolveUrl("~/Admin/Products/EditProduct.aspx?ProductId=" + product.ProductId), productName);
                    phProductName.Controls.Add(new LiteralControl(link));
                }
                else
                {
                    //OUTPUT NAME
                    phProductName.Controls.Add(new LiteralControl(productName));
                }
                //SHOW INPUTS
                BasketItemInputCollection inputs = GetCustomerInputs();
                if (inputs.Count > 0)
                {
                    InputList.DataSource = inputs;
                    InputList.DataBind();
                }
                else
                {
                    InputList.Visible = false;
                }
                //SHOW KIT PRODUCTS
                BasketItemCollection kitProductList = GetKitProducts(_BasketItem);
                KitProductPanel.Visible = (kitProductList.Count > 0);
                if (KitProductPanel.Visible)
                {
                    KitProductRepeater.DataSource = kitProductList;
                    KitProductRepeater.DataBind();
                }
                //SET THE WISHLIST LABEL
                WishlistLabel.Visible = (_BasketItem.WishlistItem != null);
                if (WishlistLabel.Visible)
                {
                    //SET THE WISHLIST NAME
                    WishlistLabel.Text = string.Format(WishlistLabel.Text, GetWishlistName(_BasketItem.WishlistItem.Wishlist));
                }
                //SET THE SHIPS TO PANEL
                Basket basket = _BasketItem.Basket;

                //SHOW ASSETS
                List<ProductAssetWrapper> assets = ProductHelper.GetAssets(this.Page, _BasketItem.Product, _BasketItem.OptionList, _BasketItem.KitList, "javascript:window.close()");
                AssetsPanel.Visible = (this.ShowAssets && assets.Count > 0);
                if (AssetsPanel.Visible)
                {
                    AssetLinkList.DataSource = assets;
                    AssetLinkList.DataBind();
                }
                //SHOW SUBSCRIPTIONS
                SubscriptionPlan sp = _BasketItem.Product.SubscriptionPlan;
                SubscriptionPanel.Visible = (this.ShowSubscription && sp != null && sp.IsRecurring);
                if (SubscriptionPanel.Visible)
                {
                    InitialPayment.Visible = (sp.RecurringChargeSpecified);
                    if (InitialPayment.Visible) InitialPayment.Text = string.Format(InitialPayment.Text, _BasketItem.Price);
                    string period;
                    if (sp.PaymentFrequency > 1) period = sp.PaymentFrequency + " " + sp.PaymentFrequencyUnit.ToString().ToLowerInvariant() + "s";
                    else period = sp.PaymentFrequencyUnit.ToString().ToLowerInvariant();
                    int numPayments = (sp.RecurringChargeSpecified ? sp.NumberOfPayments - 1 : sp.NumberOfPayments);
                    if (sp.NumberOfPayments == 0)
                    {
                        RecurringPayment.Text = string.Format("Recurring Payment: {0:ulc}, every {1} until canceled", sp.CalculateRecurringCharge(_BasketItem.Price), period);
                    }
                    else
                    {
                        RecurringPayment.Text = string.Format(RecurringPayment.Text, numPayments, sp.CalculateRecurringCharge(_BasketItem.Price), period);
                    }
                }
            }
            else
            {
                //OUTPUT NAME
                phProductName.Controls.Add(new LiteralControl(_BasketItem.Name));
                InputList.Visible = false;
                KitProductPanel.Visible = false;
                WishlistLabel.Visible = false;                
                AssetsPanel.Visible = false;
                SubscriptionPanel.Visible = false;
            }
        }
        else
        {
            //NO ITEM TO DISPLAY
            this.Controls.Clear();
        }

    }

    private static BasketItemCollection GetKitProducts(BasketItem basketItem)
    {
        BasketItemCollection basketItemKitProducts = new BasketItemCollection();
        foreach (BasketItem item in basketItem.Basket.Items)
        {
            if (item.BasketItemId != basketItem.BasketItemId
                && item.ParentItemId == basketItem.BasketItemId
                && item.OrderItemType == OrderItemType.Product)
            {
                basketItemKitProducts.Add(item);
            }
        }
        basketItemKitProducts.Sort(new BasketItemComparer());
        return basketItemKitProducts;
    }

    private string GetWishlistName(Wishlist wishlist)
    {
        if (!String.IsNullOrEmpty(wishlist.Name)) return wishlist.Name;
        else
        {
            User u = wishlist.User;
            if (u == null) return string.Empty;
            if (u.IsAnonymous) return "Anonymous";
            string fullName = u.PrimaryAddress.FullName;
            if (!string.IsNullOrEmpty(fullName)) return fullName;
            return u.UserName;
        }
    }
}
