namespace CommerceBuilder.Orders
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using CommerceBuilder.Data;
    using CommerceBuilder.Common;

    /// <summary>
    /// Represents a collection of OrderItem objects
    /// </summary>
    public partial class OrderItemCollection
    {
        /// <summary>
        /// Calculates the total price using the integer values for OrderItemType rather than the enum.
        /// </summary>
        /// <param name="itemTypeIds">The item types to include in the total.</param>
        /// <returns>The total price for the specified types</returns>
        /// <remarks>This method is required for NVelocity support, since the NVelocity template cannot refer 
        /// to the values through the enum.</remarks>
        public LSDecimal TotalPriceById(params int[] itemTypeIds)
        {
            if (itemTypeIds.Length == 0) return TotalPrice();
            OrderItemType[] itemTypes = new OrderItemType[itemTypeIds.Length];
            for (int i = 0; i < itemTypeIds.Length; i++)
            {
                itemTypes[i] = (OrderItemType)itemTypeIds[i];
            }
            return TotalPrice(itemTypes);
        }

        /// <summary>
        /// Calculates the total price for the collection, limited to the given types (if provided).
        /// </summary>
        /// <param name="itemTypes">Optional, list of item types included in the total.</param>
        /// <returns>The total price for the collection of items.</returns>
        public LSDecimal TotalPrice(params OrderItemType[] itemTypes)
        {
            LSDecimal total = 0;
            foreach (OrderItem item in this)
            {
                if ((itemTypes.Length == 0) || (Array.IndexOf(itemTypes, item.OrderItemType) > -1)) total += item.ExtendedPrice;
            }
            return total;
        }

        /// <summary>
        /// Calculates the total weight for the collection, limited to the given types (if provided).
        /// </summary>
        /// <param name="itemTypes">Optional, list of item types included in the total.</param>
        /// <returns>The total weight for the collection of items.</returns>
        public LSDecimal TotalWeight(params OrderItemType[] itemTypes)
        {
            LSDecimal total = 0;
            foreach (OrderItem item in this)
            {
                if ((itemTypes.Length == 0) || (Array.IndexOf(itemTypes, item.OrderItemType) > -1)) total += item.ExtendedWeight;
            }
            return total;
        }

        /// <summary>
        /// Gets a collection of all order items in this collection that are not associated with any shippment. 
        /// i.e; their orderShipmentId is 0
        /// </summary>
        /// <returns>Collection of order items</returns>
        public OrderItemCollection FilterByShipment()
        {
            return FilterByShipment(0);
        }

        /// <summary>
        /// Gets a collection of all order items in this collection that belong to a particular shipment. 
        /// </summary>
        /// <param name="orderShipmentId">Id of the order shipment for which to get the order items</param>
        /// <returns>Collection of order items</returns>
        public OrderItemCollection FilterByShipment(int orderShipmentId)
        {
            OrderItemCollection newCollection = new OrderItemCollection();
            foreach (OrderItem item in this)
            {
                if (item.OrderShipmentId == orderShipmentId) newCollection.Add(item);
            }
            return newCollection;
        }

        /// <summary>
        /// Gets a collection of all order items in this collection that are not associated with any shippment. 
        /// i.e; their orderShipmentId is 0 and sorts them
        /// </summary>
        /// <returns>Sorted collection of order items</returns>
        public OrderItemCollection FilterByShipmentAndSort()
        {
            return FilterByShipmentAndSort(0);
        }

        /// <summary>
        /// Gets a collection of all order items in this collection that belong to a particular shipment 
        /// and sorts them. 
        /// </summary>
        /// <param name="orderShipmentId">Id of the order shipment for which to get the order items</param>
        /// <returns>Sorted collection of order items</returns>
        public OrderItemCollection FilterByShipmentAndSort(int orderShipmentId)
        {
            OrderItemCollection newCollection = FilterByShipment(orderShipmentId);
            if (newCollection != null)
            {
                newCollection.Sort(new OrderItemComparer());
            }
            return newCollection;
        }

        /// <summary>
        /// Counts all quantites of all products in the collection
        /// </summary>
        public int ProductCount
        {
            get
            {
                int count = 0;
                foreach (OrderItem item in this)
                {
                    if (item.OrderItemType == OrderItemType.Product)
                        count += item.Quantity;
                }
                return count;
            }
        }

    }
}
