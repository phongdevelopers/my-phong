namespace CommerceBuilder.Orders
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of BasketShipment objects.
    /// </summary>
    public partial class BasketShipmentCollection : PersistentCollection<BasketShipment>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="basketShipmentId">Value of BasketShipmentId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 basketShipmentId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (basketShipmentId == this[i].BasketShipmentId) return i;
            }
            return -1;
        }
    }
}
