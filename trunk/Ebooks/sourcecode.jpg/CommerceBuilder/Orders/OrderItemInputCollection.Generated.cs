namespace CommerceBuilder.Orders
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of OrderItemInput objects.
    /// </summary>
    public partial class OrderItemInputCollection : PersistentCollection<OrderItemInput>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="orderItemInputId">Value of OrderItemInputId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 orderItemInputId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (orderItemInputId == this[i].OrderItemInputId) return i;
            }
            return -1;
        }
    }
}
