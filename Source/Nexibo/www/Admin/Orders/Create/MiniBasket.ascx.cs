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
using CommerceBuilder.Orders;
using CommerceBuilder.Taxes;
using CommerceBuilder.Utility;

public partial class Admin_Orders_Create_MiniBasket : System.Web.UI.UserControl
{
    private int _BasketId = 0;

    public int BasketId
    {
        get { return _BasketId; }
        set { _BasketId = value; }
    }

    //BUILD THE BASKET ON PRERENDER SO THAT WE CAN ACCOUNT
    //FOR ANY PRODUCTS ADDED DURING THE POSTBACK CYCLE
    protected void Page_PreRender(object sender, System.EventArgs e)
    {
        //PREPARE BASKET FOR DISPLAY
        Basket basket = BasketDataSource.Load(_BasketId);
        if (basket != null) BindBasket(basket);
    }

    private void BindBasket(Basket basket)
    {
        //GET LIST OF PRODUCTS
        BasketItemCollection _Products = new BasketItemCollection();
        LSDecimal _ProductTotal = 0;
        LSDecimal _DiscountTotal = 0;
        LSDecimal _ShippingTotal = 0;
        LSDecimal _TaxesTotal = 0;
        LSDecimal _CouponsTotal = 0;
        LSDecimal _OtherTotal = 0;
        LSDecimal _GrandTotal = 0;

        // MAKE SURE ITEMS ARE PROPERTY SORTED BEFORE DISPLAY
        basket.Items.Sort(new BasketItemComparer());
        foreach (BasketItem item in basket.Items)
        {
            switch (item.OrderItemType)
            {
                case OrderItemType.Product:
                    if (!item.IsChildItem)
                    {
                        // ROOT LEVEL ITEMS GET ADDED
                        _Products.Add(item);
                        _ProductTotal += GetItemShopPrice(item);
                    }
                    else
                    {
                        // CHILD PRODUCTS SHOULD HAVE THEIR TOTAL ADDED TO THE ROOT PRODUCT
                        int parentIndex = _Products.IndexOf(item.GetParentItem(true).BasketItemId);
                        if (parentIndex > -1)
                        {
                            BasketItem parentItem = _Products[parentIndex];
                            LSDecimal shopPrice = GetItemShopPrice(item);
                            parentItem.Price += shopPrice;
                            _ProductTotal += shopPrice;
                        }
                    }
                    break;
                case OrderItemType.Discount:
                    _DiscountTotal += GetItemShopPrice(item);
                    break;
                case OrderItemType.Shipping:
                case OrderItemType.Handling:
                    _ShippingTotal += GetItemShopPrice(item);
                    break;
                case OrderItemType.Tax:
                    _TaxesTotal += item.ExtendedPrice;
                    break;
                case OrderItemType.Coupon:
                    _CouponsTotal += item.ExtendedPrice;
                    break;
                default:
                    _OtherTotal += item.ExtendedPrice;
                    break;
            }
            _GrandTotal += item.ExtendedPrice;
        }

        //BIND BASKET ITEMS 
        BasketRepeater.DataSource = _Products;
        BasketRepeater.DataBind();
        if (_DiscountTotal != 0)
        {
            trDiscounts.Visible = true;
            Discounts.Text = _DiscountTotal.ToString("ulc");
        }
        if (_ShippingTotal != 0)
        {
            trShipping.Visible = true;
            Shipping.Text = _ShippingTotal.ToString("ulc");
        }
        if (_TaxesTotal != 0)
        {
            trTaxes.Visible = true;
            Taxes.Text = _TaxesTotal.ToString("lc");
        }
        if (_CouponsTotal != 0)
        {
            trCoupons.Visible = true;
            Coupons.Text = _CouponsTotal.ToString("lc");
        }
        if (_OtherTotal != 0)
        {
            trOther.Visible = true;
            Other.Text = _OtherTotal.ToString("lc");
        }
        Total.Text = _GrandTotal.ToString("lc");
        EditOrderLink.NavigateUrl += "?UID=" + basket.UserId;
    }

    protected LSDecimal GetItemShopPrice(BasketItem item)
    {
        return TaxHelper.GetShopPrice(item.ExtendedPrice, item.TaxCodeId);
    }
}