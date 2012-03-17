using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Orders;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Shipping
{
    /// <summary>
    /// Utility class for supporting shipping rate calculations
    /// </summary>
    public static class ShipRateCalculator
    {
        /// <summary>
        /// Recalculates shipping charges for the given basket.
        /// </summary>
        /// <param name="basket">The basket to calculate shipping charges for.</param>
        public static void Calculate(Basket basket)
        {
            ClearExistingShipping(basket);
            foreach (BasketShipment shipment in basket.Shipments)
            {
                if (shipment.Warehouse != null)
                {
                    ShipMethod shipMethod = shipment.ShipMethod;
                    if (shipMethod != null)
                    {
                        ShipRateQuote rate = shipMethod.GetShipRateQuote(shipment);
                        if (rate != null)
                        {
                            BasketItem shipRateLineItem = new BasketItem();
                            shipRateLineItem.BasketId = basket.BasketId;
                            shipRateLineItem.OrderItemType = OrderItemType.Shipping;
                            shipRateLineItem.BasketShipmentId = shipment.BasketShipmentId;
                            shipRateLineItem.Name = shipMethod.Name;
                            shipRateLineItem.Price = rate.Rate;
                            shipRateLineItem.Quantity = 1;
                            shipRateLineItem.TaxCodeId = shipMethod.TaxCodeId;
                            shipRateLineItem.Save();
                            basket.Items.Add(shipRateLineItem);
                            if (rate.Surcharge > 0)
                            {
                                shipRateLineItem = new BasketItem();
                                shipRateLineItem.BasketId = basket.BasketId;
                                shipRateLineItem.OrderItemType = OrderItemType.Handling;
                                shipRateLineItem.BasketShipmentId = shipment.BasketShipmentId;
                                shipRateLineItem.Name = shipMethod.Name;
                                shipRateLineItem.Price = rate.Surcharge;
                                shipRateLineItem.Quantity = 1;
                                shipRateLineItem.TaxCodeId = shipMethod.SurchargeTaxCodeId;
                                shipRateLineItem.Save();
                                basket.Items.Add(shipRateLineItem);
                            }
                        }
                        else
                        {
                            //rate quote could not be obtained for some reason.
                            Logger.Warn("Failed to obtain rate quote for the given ship method '" + shipMethod.Name + "'");
                            //here we need to communicate back to the caller that the selected ship method can't be used
                            shipment.ShipMethodId = 0;
                            shipment.Save();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Clears any existing shipping or handling items from the basket.
        /// </summary>
        /// <param name="basket">The basket to clear items from</param>
        private static void ClearExistingShipping(Basket basket)
        {
            for (int i = basket.Items.Count - 1; i >= 0; i--)
            {
                BasketItem item = basket.Items[i];
                if ((item.OrderItemType == OrderItemType.Shipping) || (item.OrderItemType == OrderItemType.Handling))
                {
                    basket.Items.DeleteAt(i);
                }
            }
        }
    }
}
