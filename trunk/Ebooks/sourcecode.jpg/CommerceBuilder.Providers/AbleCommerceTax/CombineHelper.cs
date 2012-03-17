using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Orders;
using CommerceBuilder.Products;
using CommerceBuilder.Shipping;
using CommerceBuilder.Users;

namespace CommerceBuilder.Taxes.Providers.AbleCommerce
{
    internal static class CombineHelper
    {
        public static void CombineTaxItems(Basket basket, List<int> existingBasketItemIds)
        {
            for (int i = basket.Items.Count - 1; i >= 0; i--)
            {
                BasketItem item = basket.Items[i];
                //IS THIS A TAX ITEM WE JUST GENERATED?
                if (item.OrderItemType == OrderItemType.Tax &&
                    existingBasketItemIds.IndexOf(item.BasketItemId) < 0)
                {
                    //THIS IS A TAX ITEM WE GENERATED, SEE IF WE CAN COMBINE IT
                    BasketItem match = FindFirstMatch(basket, item, existingBasketItemIds);
                    if (match != null)
                    {
                        match.Price += item.Price;
                        basket.Items.DeleteAt(i);
                    }
                }
            }
        }

        public static void CombineTaxItems(Order order, List<int> existingOrderItemIds)
        {
            for (int i = order.Items.Count - 1; i >= 0; i--)
            {
                OrderItem item = order.Items[i];
                //IS THIS A TAX ITEM WE JUST GENERATED?
                if (item.OrderItemType == OrderItemType.Tax &&
                    existingOrderItemIds.IndexOf(item.OrderItemId) < 0)
                {
                    //THIS IS A TAX ITEM WE GENERATED, SEE IF WE CAN COMBINE IT
                    OrderItem match = FindFirstMatch(order, item, existingOrderItemIds);
                    if (match != null)
                    {
                        match.Price += item.Price;
                        order.Items.DeleteAt(i);
                    }
                }
            }
        }

        private static BasketItem FindFirstMatch(Basket basket, BasketItem find, List<int> existingBasketItemIds)
        {
            //LOOK FOR MATCHING TAX ITEM
            foreach (BasketItem item in basket.Items)
            {
                //LOOK FOR GENERATED TAX ITEMS
                if (existingBasketItemIds.IndexOf(item.BasketItemId) < 0)
                {
                    if (IsTaxItemMatch(item, find)) return item;
                }
            }
            return null;
        }

        private static OrderItem FindFirstMatch(Order order, OrderItem find, List<int> existingOrderItemIds)
        {
            //LOOK FOR MATCHING TAX ITEM
            foreach (OrderItem item in order.Items)
            {
                //LOOK FOR GENERATED TAX ITEMS
                if (existingOrderItemIds.IndexOf(item.OrderItemId) < 0)
                {
                    if (IsTaxItemMatch(item, find)) return item;
                }
            }
            return null;
        }

        private static bool IsTaxItemMatch(BasketItem x, BasketItem y)
        {
            if (x.BasketItemId == y.BasketItemId) return false;
            if (x.OrderItemType != OrderItemType.Tax) return false;
            if (y.OrderItemType != OrderItemType.Tax) return false;
            if (x.BasketShipmentId != y.BasketShipmentId) return false;
            if (x.ParentItemId != y.ParentItemId) return false;
            if (x.Name != y.Name) return false;
            if (x.Sku != y.Sku) return false;
            if (x.Quantity != y.Quantity) return false;
            if (x.TaxCodeId != y.TaxCodeId) return false;
            return true;
        }

        private static bool IsTaxItemMatch(OrderItem x, OrderItem y)
        {
            if (x.OrderItemId == y.OrderItemId) return false;
            if (x.OrderItemType != OrderItemType.Tax) return false;
            if (y.OrderItemType != OrderItemType.Tax) return false;
            if (x.OrderShipmentId != y.OrderShipmentId) return false;
            if (x.ParentItemId != y.ParentItemId) return false;
            if (x.Name != y.Name) return false;
            if (x.Sku != y.Sku) return false;
            if (x.Quantity != y.Quantity) return false;
            if (x.TaxCodeId != y.TaxCodeId) return false;
            return true;
        }
    }
}
