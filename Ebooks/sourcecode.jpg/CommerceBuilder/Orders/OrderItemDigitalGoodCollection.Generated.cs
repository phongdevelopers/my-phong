namespace CommerceBuilder.Orders
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of OrderItemDigitalGood objects.
    /// </summary>
    public partial class OrderItemDigitalGoodCollection : PersistentCollection<OrderItemDigitalGood>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="orderItemDigitalGoodId">Value of OrderItemDigitalGoodId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 orderItemDigitalGoodId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (orderItemDigitalGoodId == this[i].OrderItemDigitalGoodId) return i;
            }
            return -1;
        }
    }
}
