namespace CommerceBuilder.Orders
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of OrderStatusTrigger objects.
    /// </summary>
    public partial class OrderStatusTriggerCollection : PersistentCollection<OrderStatusTrigger>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="storeEventId">Value of StoreEventId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 storeEventId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (storeEventId == this[i].StoreEventId) return i;
            }
            return -1;
        }
    }
}
