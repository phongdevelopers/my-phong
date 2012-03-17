namespace CommerceBuilder.Orders
{
    using System;
    using CommerceBuilder.Common;

    /// <summary>
    /// Collection of OrderPullItem objects
    /// </summary>
    public class OrderPullItemCollection : SortableCollection<OrderPullItem>
    {
        /// <summary>
        /// Returns the index of the matching item.
        /// </summary>
        /// <param name="sku">Sku of the item.</param>
        /// <param name="name">Name of the item.</param>
        /// <param name="variantName">Variant name of the item.</param>
        /// <returns>The index of the matching item. -1 if not found.</returns>
        public int IndexOf(string sku, string name, string variantName)
        {
            OrderPullItem thisItem;
            for (int i = 0; i < this.Count; i++)
            {
                thisItem = this[i];
                if ((sku == thisItem.Sku) && (name == thisItem.Name) && (variantName == thisItem.VariantName)) return i;
            }
            return -1;
        }

        /// <summary>
        /// Adds an orderitem to the pull item collection.
        /// </summary>
        /// <param name="orderItem">The order item to add</param>
        /// <returns>The index at which the item was added.</returns>
        /// <remarks>If a match to the order item is detected, the existing item is updated.</remarks>
        public int Add(OrderItem orderItem)
        {
            int index = this.IndexOf(orderItem.Sku, orderItem.Name, orderItem.VariantName);
            if (index < 0)
            {
                return this.Add(new OrderPullItem(orderItem));
            } else {
                this[index].Quantity += orderItem.Quantity;
                return index;
            }
        }
    }
}