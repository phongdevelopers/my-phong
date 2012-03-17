using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Orders;
using CommerceBuilder.Marketing;
using CommerceBuilder.Payments;
using CommerceBuilder.Payments.Providers.GoogleCheckout.AutoGen;
using System.Web;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Payments.Providers.GoogleCheckout.AC
{
    public class OrderAdjustmentHelper
    {
        public static void AdjustMerchantCalculatedShipping(Basket basket, MerchantCalculatedShippingAdjustment mcsa)
        {
            if (mcsa != null)
            {                
                BasketItem basketItem = new BasketItem();
                basketItem.OrderItemType = OrderItemType.Shipping;
                basketItem.Price = mcsa.shippingcost.Value;
                basketItem.Quantity = 1;
                basketItem.BasketId = basket.BasketId;
                //basketItem.Name = AcHelper.CleanShipMethodName(mcsa.shippingname);
                basketItem.Name = mcsa.shippingname;
                basketItem.Sku = basketItem.Name; // +"_Google";
                basket.Items.Add(basketItem);
            }
        }

        public static void AdjustFlatRateShipping(Basket basket, FlatRateShippingAdjustment frsa)
        {
            if (frsa != null)
            {
                BasketItem basketItem = new BasketItem();
                basketItem.OrderItemType = OrderItemType.Shipping;
                basketItem.Price = frsa.shippingcost.Value;
                basketItem.Quantity = 1;
                basketItem.BasketId = basket.BasketId;
                //basketItem.Name = AcHelper.CleanShipMethodName(frsa.shippingname);
                basketItem.Name = frsa.shippingname;
                basketItem.Sku = basketItem.Name; //frsa.shippingname + "_Google";
                basket.Items.Add(basketItem);
            }
        }

        public static void AdjustPickupShipping(Basket basket, PickupShippingAdjustment pusa)
        {
            if (pusa != null)
            {
                BasketItem basketItem = new BasketItem();
                basketItem.OrderItemType = OrderItemType.Shipping;
                basketItem.Price = pusa.shippingcost.Value;
                basketItem.Quantity = 1;
                basketItem.BasketId = basket.BasketId;
                //basketItem.Name = AcHelper.CleanShipMethodName(pusa.shippingname);
                basketItem.Name = pusa.shippingname;
                basketItem.Sku = basketItem.Name; //pusa.shippingname + "_Google";
                basket.Items.Add(basketItem);
            }
        }

        public static void AdjustCoupon(Basket basket, CouponAdjustment coupAdj)
        {
            if (coupAdj != null)
            {
                BasketItem basketItem = new BasketItem();
                basketItem.OrderItemType = OrderItemType.Coupon;
                basketItem.Price = -1 * coupAdj.appliedamount.Value;
                basketItem.Quantity = 1;
                basketItem.BasketId = basket.BasketId;
                basketItem.Sku = coupAdj.code;

                Coupon coupon = CouponDataSource.LoadForCouponCode(coupAdj.code);
                if (coupon == null)
                {
                    //shouldn't ever happen. However if it does we have 
                    //no option but to add as an unknown coupon
                    basketItem.Name = "Unknown Coupon '" + coupAdj.code + "' (GoogleCheckout)";
                    Logger.Warn("A coupon applied by Google Checkout is not recognized in the Store. Coupon Code : " + coupAdj.code + ", Applied Amount : " + coupAdj.appliedamount);
                }
                else
                {
                    basketItem.Name = coupon.Name;
                    basket.BasketCoupons.Add(new BasketCoupon(basket.BasketId, coupon.CouponId));
                }

                basket.Items.Add(basketItem);
            }
        }

        public static void AdjustGiftCertificate(Basket basket, GiftCertificateAdjustment giftCertAdj)
        {
            if (giftCertAdj != null)
            {
                BasketItem basketItem = new BasketItem();
                basketItem.OrderItemType = OrderItemType.GiftCertificatePayment;
                basketItem.Price = -1 * giftCertAdj.appliedamount.Value;
                basketItem.Quantity = 1;
                basketItem.BasketId = basket.BasketId;
                basketItem.Sku = giftCertAdj.code;
                basketItem.Name = "Gift Certificate";
                basket.Items.Add(basketItem);
            }
        }

        public static void AdjustTax(Basket basket, LSDecimal totalTax)
        {
            if (totalTax > 0)
            {
                BasketItem basketItem = new BasketItem();
                basketItem.OrderItemType = OrderItemType.Tax;
                basketItem.Price = totalTax;
                basketItem.Quantity = 1;
                basketItem.BasketId = basket.BasketId;
                //basketItem.ExtendedPrice = basketItem.Price;
                basketItem.Name = "Tax";
                basketItem.Sku = "Tax";
                basket.Items.Add(basketItem);
            }
        }
        
        public static void DoOrderAdjustments(OrderAdjustment orderAdj, Basket basket)
        {
            TraceContext trace = WebTrace.GetTraceContext();
            string traceKey = "OrderAdjustmentHelper.DoOrderAdjustments";

            if (orderAdj == null)
            {
                throw new ArgumentNullException("orderAdj", "OrderAdjustment can't be null");
            }

            OrderAdjustmentMerchantcodes oamcs = orderAdj.merchantcodes;
            if (oamcs != null && oamcs.Items!=null)
            {
                trace.Write(traceKey, "check merchant codes");
                Object[] merchantCodes = oamcs.Items;
                CouponAdjustment coupAdj;
                GiftCertificateAdjustment giftCertAdj;

                //coupon and giftcertificate adjustment
                foreach (Object obj in merchantCodes)
                {
                    if (obj == null) continue;
                    if (obj is CouponAdjustment)
                    {
                        coupAdj = (CouponAdjustment)obj;
                        trace.Write(traceKey, "Apply coupon: " + coupAdj.code + ", " + coupAdj.appliedamount);
                        OrderAdjustmentHelper.AdjustCoupon(basket, coupAdj);
                    }
                    else if (obj is GiftCertificateAdjustment)
                    {
                        giftCertAdj = (GiftCertificateAdjustment)obj;
                        trace.Write(traceKey, "Apply gift cert: " + giftCertAdj.code + " for " + giftCertAdj.appliedamount.Value);
                        OrderAdjustmentHelper.AdjustGiftCertificate(basket, giftCertAdj);
                    }
                }
            }

            OrderAdjustmentShipping oas = orderAdj.shipping;
            if (oas != null && oas.Item!=null)
            {
                trace.Write(traceKey, "check shipping adjustments");
                Object shipAdj = oas.Item;
                if (shipAdj is MerchantCalculatedShippingAdjustment)
                {
                    MerchantCalculatedShippingAdjustment mcsa = (MerchantCalculatedShippingAdjustment)shipAdj;
                    OrderAdjustmentHelper.AdjustMerchantCalculatedShipping(basket, mcsa);
                }else if (shipAdj is FlatRateShippingAdjustment)
                {
                    FlatRateShippingAdjustment frsa = (FlatRateShippingAdjustment)shipAdj;
                    OrderAdjustmentHelper.AdjustFlatRateShipping(basket, frsa);
                }else if (shipAdj is PickupShippingAdjustment)
                {
                    PickupShippingAdjustment pusa = (PickupShippingAdjustment)shipAdj;
                    OrderAdjustmentHelper.AdjustPickupShipping(basket, pusa);
                }
            }

            //tax adjustments
            if (orderAdj.totaltax!=null && orderAdj.totaltax.Value > 0)
            {
                trace.Write(traceKey, "process tax adjustments");
                OrderAdjustmentHelper.AdjustTax(basket, orderAdj.totaltax.Value);
            }
        }

    }
}
