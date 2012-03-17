namespace CommerceBuilder.Shipping
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of ShipZone objects.
    /// </summary>
    public partial class ShipZoneCollection : PersistentCollection<ShipZone>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="shipZoneId">Value of ShipZoneId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 shipZoneId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (shipZoneId == this[i].ShipZoneId) return i;
            }
            return -1;
        }
    }
}
