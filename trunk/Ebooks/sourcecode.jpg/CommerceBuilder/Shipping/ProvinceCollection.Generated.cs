namespace CommerceBuilder.Shipping
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of Province objects.
    /// </summary>
    public partial class ProvinceCollection : PersistentCollection<Province>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="provinceId">Value of ProvinceId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 provinceId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (provinceId == this[i].ProvinceId) return i;
            }
            return -1;
        }
    }
}
