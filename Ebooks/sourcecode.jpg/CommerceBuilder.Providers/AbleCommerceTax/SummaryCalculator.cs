using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Orders;
using CommerceBuilder.Users;
using CommerceBuilder.Shipping;

namespace CommerceBuilder.Taxes.Providers.AbleCommerce
{
    internal static class SummaryCalculator
    {
        /// <summary>
        /// Calculates tax
        /// </summary>
        /// <param name="taxRule">The tax rule to process</param>
        /// <param name="basket">The basket being processed</param>
        /// <param name="shipmentId">The shipment ID to calculate tax for; pass 0 for unshippable or -1 to ignore shipments</param>
        /// <returns>A basket item for the tax entry, or null if no tax is charged.</returns>
        public static BasketItem Calculate(TaxRule taxRule, Basket basket, int shipmentId)
        {
            //THIS SHIPMENT QUALIFIES, GET TOTAL OF ALL APPLICABLE BASKET ITEMS
            LSDecimal totalPrice = 0;
            foreach (BasketItem bi in basket.Items)
            {
                //CHECK IF THE ITEM IS PART OF THE SHIPMENT
                if (bi.BasketShipmentId == shipmentId || shipmentId < 0)
                {
                    //CHECK WHETHER THE TAX CODE IS AFFECTED BY THIS Tax RULE
                    if (taxRule.AppliesToTaxCode(bi.TaxCodeId))
                    {
                        totalPrice += bi.ExtendedPrice;
                    }
                }
            }
            // GENERATE ITEM AND RETURN IF VALID
            BasketItem taxItem = GenerateBasketItem(basket.BasketId, shipmentId, taxRule, totalPrice);
            if (taxItem.ExtendedPrice > 0) return taxItem;
            return null;
        }

        /// <summary>
        /// Calculates tax
        /// </summary>
        /// <param name="taxRule">The tax rule to process</param>
        /// <param name="order">The order being processed</param>
        /// <param name="shipmentId">The shipment ID to calculate tax for; pass 0 for unshippable or -1 to ignore shipments</param>
        /// <returns>A basket item for the tax entry, or null if no tax is charged.</returns>
        public static OrderItem Calculate(TaxRule taxRule, Order order, int shipmentId)
        {
            //THIS SHIPMENT QUALIFIES, GET TOTAL OF ALL APPLICABLE BASKET ITEMS
            LSDecimal totalPrice = 0;
            foreach (OrderItem oi in order.Items)
            {
                //CHECK IF THE ITEM IS PART OF THE SHIPMENT
                if (oi.OrderShipmentId == shipmentId || shipmentId < 0)
                {
                    //CHECK WHETHER THE TAX CODE IS AFFECTED BY THIS Tax RULE
                    if (taxRule.AppliesToTaxCode(oi.TaxCodeId))
                    {
                        totalPrice += oi.ExtendedPrice;
                    }
                }
            }
            // GENERATE ITEM AND RETURN IF VALID
            OrderItem taxItem = GenerateOrderItem(order.OrderId, shipmentId, taxRule, totalPrice);
            if (taxItem.ExtendedPrice > 0) return taxItem;
            return null;
        }

        /// <summary>
        /// Generates a line item for the tax
        /// </summary>
        /// <param name="basketId">The basket id</param>
        /// <param name="shipmentId">The shipment id</param>
        /// <param name="taxRule">The tax rule</param>
        /// <param name="totalPrice">The total price being taxed</param>
        /// <returns>A line item representing the calculated tax</returns>
        private static BasketItem GenerateBasketItem(int basketId, int shipmentId, TaxRule taxRule, LSDecimal totalPrice)
        {
            BasketItem taxLineItem = new BasketItem();
            taxLineItem.BasketId = basketId;
            taxLineItem.OrderItemType = OrderItemType.Tax;
            if (shipmentId < 0) shipmentId = 0;
            taxLineItem.BasketShipmentId = shipmentId;
            taxLineItem.ParentItemId = 0;
            taxLineItem.Name = taxRule.Name;
            taxLineItem.Sku = taxRule.TaxRuleId.ToString();
            taxLineItem.Price = CalculateTax(taxRule, totalPrice);
            taxLineItem.Quantity = 1;
            taxLineItem.TaxCodeId = taxRule.TaxCodeId;
            return taxLineItem;
        }

        /// <summary>
        /// Generates a line item for the tax
        /// </summary>
        /// <param name="orderId">The order id</param>
        /// <param name="shipmentId">The shipment id</param>
        /// <param name="taxRule">The tax rule</param>
        /// <param name="totalPrice">The total price being taxed</param>
        /// <returns>A line item representing the calculated tax</returns>
        private static OrderItem GenerateOrderItem(int orderId, int shipmentId, TaxRule taxRule, LSDecimal totalPrice)
        {
            OrderItem taxLineItem = new OrderItem();
            taxLineItem.OrderId = orderId;
            taxLineItem.OrderItemType = OrderItemType.Tax;
            if (shipmentId < 0) shipmentId = 0;
            taxLineItem.OrderShipmentId = shipmentId;
            taxLineItem.ParentItemId = 0;
            taxLineItem.Name = taxRule.Name;
            taxLineItem.Sku = taxRule.TaxRuleId.ToString();
            taxLineItem.Price = CalculateTax(taxRule, totalPrice);
            taxLineItem.Quantity = 1;
            taxLineItem.TaxCodeId = taxRule.TaxCodeId;
            return taxLineItem;
        }

        private static LSDecimal CalculateTax(TaxRule taxRule, LSDecimal price)
        {
            // DETERMINE TAX FOR TOTAL PRICE
            LSDecimal tempTax = ((price * taxRule.TaxRate) / 100);
            return TaxHelper.Round((decimal)tempTax, 2, taxRule.RoundingRule);
        }
    }
}