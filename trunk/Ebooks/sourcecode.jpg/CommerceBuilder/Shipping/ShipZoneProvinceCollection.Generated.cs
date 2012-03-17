namespace CommerceBuilder.Shipping
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of ShipZoneProvince objects.
    /// </summary>
    public partial class ShipZoneProvinceCollection : PersistentCollection<ShipZoneProvince>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="shipZoneId">Value of ShipZoneId of the required object.</param>
        /// <param name="provinceId">Value of ProvinceId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 shipZoneId, Int32 provinceId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((shipZoneId == this[i].ShipZoneId) && (provinceId == this[i].ProvinceId)) return i;
            }
            return -1;
        }
    }
}
