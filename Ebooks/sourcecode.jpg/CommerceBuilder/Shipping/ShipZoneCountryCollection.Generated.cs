namespace CommerceBuilder.Shipping
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of ShipZoneCountry objects.
    /// </summary>
    public partial class ShipZoneCountryCollection : PersistentCollection<ShipZoneCountry>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="shipZoneId">Value of ShipZoneId of the required object.</param>
        /// <param name="countryCode">Value of CountryCode of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 shipZoneId, String countryCode)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((shipZoneId == this[i].ShipZoneId) && (countryCode == this[i].CountryCode)) return i;
            }
            return -1;
        }
    }
}
