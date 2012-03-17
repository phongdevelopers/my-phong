namespace CommerceBuilder.Orders
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of OrderStatusAction objects.
    /// </summary>
    public partial class OrderStatusActionCollection : PersistentCollection<OrderStatusAction>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="orderStatusId">Value of OrderStatusId of the required object.</param>
        /// <param name="orderActionId">Value of OrderActionId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 orderStatusId, Int16 orderActionId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((orderStatusId == this[i].OrderStatusId) && (orderActionId == this[i].OrderActionId)) return i;
            }
            return -1;
        }
    }
}
