namespace CommerceBuilder.Shipping
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of ShipMethod objects.
    /// </summary>
    public partial class ShipMethodCollection : PersistentCollection<ShipMethod>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="shipMethodId">Value of ShipMethodId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 shipMethodId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (shipMethodId == this[i].ShipMethodId) return i;
            }
            return -1;
        }
    }
}
