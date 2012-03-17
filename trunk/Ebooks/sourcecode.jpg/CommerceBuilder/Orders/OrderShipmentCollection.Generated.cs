namespace CommerceBuilder.Orders
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of OrderShipment objects.
    /// </summary>
    public partial class OrderShipmentCollection : PersistentCollection<OrderShipment>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="orderShipmentId">Value of OrderShipmentId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 orderShipmentId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (orderShipmentId == this[i].OrderShipmentId) return i;
            }
            return -1;
        }
    }
}
