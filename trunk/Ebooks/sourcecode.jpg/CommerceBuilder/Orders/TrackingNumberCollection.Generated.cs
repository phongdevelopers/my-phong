namespace CommerceBuilder.Orders
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of TrackingNumber objects.
    /// </summary>
    public partial class TrackingNumberCollection : PersistentCollection<TrackingNumber>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="trackingNumberId">Value of TrackingNumberId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 trackingNumberId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (trackingNumberId == this[i].TrackingNumberId) return i;
            }
            return -1;
        }
    }
}
