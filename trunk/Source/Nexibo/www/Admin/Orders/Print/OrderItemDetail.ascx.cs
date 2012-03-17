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
using CommerceBuilder.DigitalDelivery;
using CommerceBuilder.Orders;
using CommerceBuilder.Products;
using CommerceBuilder.Users;

public partial class Admin_UserControls_OrderItemDetail : System.Web.UI.UserControl
{
    OrderItem _OrderItem;
    public OrderItem OrderItem
    {
        get { return _OrderItem; }
        set { _OrderItem = value; }
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

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (_OrderItem != null)
        {
            Product product = _OrderItem.Product;
            if (product != null)
            {
                string productName = _OrderItem.Name;
                if (!string.IsNullOrEmpty(_OrderItem.VariantName))
                {
                    string variantName = string.Format(" ({0})", _OrderItem.VariantName);
                    if (!productName.EndsWith(variantName)) productName += variantName;
                }
                if (this.LinkProducts)
                {
                    ProductLink.NavigateUrl = "~/Admin/Products/EditProduct.aspx?ProductId=" + product.ProductId.ToString();
                    ProductLink.Text = productName;
                    ProductName.Visible = false;
                }
                else
                {
                    ProductName.Text = productName;
                    ProductLink.Visible = false;
                }
                //SHOW INPUTS
                if (_OrderItem.Inputs.Count > 0)
                {
                    InputList.DataSource = _OrderItem.Inputs;
                    InputList.DataBind();
                }
                else
                {
                    InputList.Visible = false;
                }
                //SHOW KIT PRODUCTS IF AVAILABLE, AND THE PRODUCT DOES NOT USE ITEMIZED DISPLAY
                if (!string.IsNullOrEmpty(_OrderItem.KitList) && !_OrderItem.ItemizeChildProducts)
                {
                    OrderItemCollection kitProductList = GetKitProducts(_OrderItem);
                    if (kitProductList.Count > 0)
                    {
                        KitProductPanel.Visible = true;
                        KitProductRepeater.DataSource = kitProductList;
                        KitProductRepeater.DataBind();
                    }
                }
                //SET THE KIT MEMBER LABEL
                if (_OrderItem.OrderItemType == OrderItemType.Product && _OrderItem.IsChildItem)
                {
                    OrderItem parentItem = _OrderItem.GetParentItem(true);
                    if (parentItem.ItemizeChildProducts 
                        || _OrderItem.OrderShipmentId != parentItem.OrderShipmentId)
                    {
                        //SET THE WISHLIST NAME
                        KitMemberLabel.Visible = true;
                        KitMemberLabel.Text = string.Format(KitMemberLabel.Text, parentItem.Name);
                    }
                }
                //SET THE WISHLIST LABEL
                WishlistLabel.Visible = (_OrderItem.WishlistItem != null);
                if (WishlistLabel.Visible)
                {
                    //SET THE WISHLIST NAME
                    WishlistLabel.Text = string.Format(WishlistLabel.Text, GetWishlistName(_OrderItem.WishlistItem.Wishlist));
                }
                //SET THE SHIPS TO PANEL
                Order basket = _OrderItem.Order;
                OrderShipment shipment = _OrderItem.OrderShipment;
                ShipsToPanel.Visible = this.ShowShipTo;
                if (ShipsToPanel.Visible)
                {
                    ShipsTo.Text = shipment.ShipToFullName;
                }
                //SHOW GIFT WRAP
                GiftWrapPanel.Visible = (_OrderItem.WrapStyle != null);
                if (GiftWrapPanel.Visible)
                {
                    GiftWrap.Text = _OrderItem.WrapStyle.Name;
                    GiftWrapPrice.Visible = (_OrderItem.WrapStyle.Price != 0);
                    GiftWrapPrice.Text = string.Format("&nbsp;({0})", _OrderItem.WrapStyle.Price);
                }
                //SHOW GIFT MESSAGE
                GiftMessagePanel.Visible = (!string.IsNullOrEmpty(_OrderItem.GiftMessage));
                if (GiftMessagePanel.Visible)
                {
                    GiftMessage.Text = _OrderItem.GiftMessage;
                }
                //SHOW ASSETS
				List<ProductAssetWrapper> assets = ProductHelper.GetAssets(this.Page, _OrderItem.Product, _OrderItem.OptionList, _OrderItem.KitList, "~/Members/MyOrder.aspx?OrderNumber=" + _OrderItem.Order.OrderNumber.ToString() + "&OrderId=" + _OrderItem.OrderId.ToString());
                AssetsPanel.Visible = (this.ShowAssets && assets.Count > 0);
                if (AssetsPanel.Visible)
                {
                    AssetLinkList.DataSource = assets;
                    AssetLinkList.DataBind();
                }
            }
            else
            {
                ProductLink.Visible = false;
                ProductName.Text = _OrderItem.Name;
                InputList.Visible = false;
                KitProductPanel.Visible = false;
                WishlistLabel.Visible = false;
                ShipsToPanel.Visible = false;
                GiftWrapPanel.Visible = false;
                GiftMessagePanel.Visible = false;
                AssetsPanel.Visible = false;
            }
        }
        else
        {
            //NO ITEM TO DISPLAY
            this.Controls.Clear();
        }
    }

    private static OrderItemCollection GetKitProducts(OrderItem orderItem)
    {
        OrderItemCollection orderItemKitProducts = new OrderItemCollection();
        foreach (OrderItem item in orderItem.Order.Items)
        {
            if (item.OrderItemId != orderItem.OrderItemId
                && item.ParentItemId == orderItem.OrderItemId
                && item.OrderItemType == OrderItemType.Product
                && !item.IsHidden
                && item.OrderShipmentId == orderItem.OrderShipmentId)
            {
                orderItemKitProducts.Add(item);
            }
        }
        orderItemKitProducts.Sort(new OrderItemComparer());
        return orderItemKitProducts;
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
