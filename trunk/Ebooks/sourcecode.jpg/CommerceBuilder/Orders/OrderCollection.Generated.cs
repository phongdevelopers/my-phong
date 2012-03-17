namespace CommerceBuilder.Orders
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of Order objects.
    /// </summary>
    public partial class OrderCollection : PersistentCollection<Order>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="orderId">Value of OrderId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 orderId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (orderId == this[i].OrderId) return i;
            }
            return -1;
        }
    }
}
