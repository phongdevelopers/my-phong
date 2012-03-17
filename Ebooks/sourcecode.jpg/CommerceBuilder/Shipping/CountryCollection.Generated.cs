namespace CommerceBuilder.Shipping
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of Country objects.
    /// </summary>
    public partial class CountryCollection : PersistentCollection<Country>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="countryCode">Value of CountryCode of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(String countryCode)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (countryCode == this[i].CountryCode) return i;
            }
            return -1;
        }
    }
}
