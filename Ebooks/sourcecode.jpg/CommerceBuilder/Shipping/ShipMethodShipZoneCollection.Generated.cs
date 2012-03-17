namespace CommerceBuilder.Shipping
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of ShipMethodShipZone objects.
    /// </summary>
    public partial class ShipMethodShipZoneCollection : PersistentCollection<ShipMethodShipZone>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="shipMethodId">Value of ShipMethodId of the required object.</param>
        /// <param name="shipZoneId">Value of ShipZoneId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 shipMethodId, Int32 shipZoneId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((shipMethodId == this[i].ShipMethodId) && (shipZoneId == this[i].ShipZoneId)) return i;
            }
            return -1;
        }
    }
}
