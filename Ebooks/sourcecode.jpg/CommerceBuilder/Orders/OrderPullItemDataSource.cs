using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Orders
{
    /// <summary>
    /// DataSource class for OrderPullItem objects
    /// </summary>
    public static class OrderPullItemDataSource
    {
        /// <summary>
        /// Generates a pull sheet for the given orders.
        /// </summary>
        /// <param name="orders">The OrderId int values for all orders to be included in the pullsheet.</param>
        /// <returns>An OrderPullItemCollection that represents the pullsheet for the given orders.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderPullItemCollection GeneratePullSheet(params int[] orders)
        {
            OrderPullItemCollection pullSheet = new OrderPullItemCollection();
            foreach (int orderId in orders)
            {
                Order order = OrderDataSource.Load(orderId);
                if (order != null)
                {
                    foreach (OrderItem item in order.Items)
                    {
                        if (IsPullItem(item)) pullSheet.Add(item);
                    }
                }
            }
            return pullSheet;
        }

        /// <summary>
        /// Determine if an order item should be added to a pull sheet.
        /// </summary>
        /// <param name="item">The order item to check.</param>
        /// <returns>True if the item is a pull item.  False otherwise.</returns>
        private static bool IsPullItem(OrderItem item)
        {
            //ONLY INCLUDE ITEMS THAT ARE PRODUCTS
            if (item.OrderItemType != OrderItemType.Product) return false;
            //ONLY INCLUDE ITEMS THAT ARE ASSOCIATED WITH A SHIPMENT
            OrderShipment shipment = item.OrderShipment;
            if (shipment == null) return false;
            //ONLY INCLUDE ITEMS THAT HAVE NOT BEEN SHIPPED
            return (shipment.ShipDate == System.DateTime.MinValue);
        }
    }
}
