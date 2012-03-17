namespace CommerceBuilder.Orders
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of OrderItem objects.
    /// </summary>
    public partial class OrderItemCollection : PersistentCollection<OrderItem>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="orderItemId">Value of OrderItemId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 orderItemId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (orderItemId == this[i].OrderItemId) return i;
            }
            return -1;
        }
    }
}
