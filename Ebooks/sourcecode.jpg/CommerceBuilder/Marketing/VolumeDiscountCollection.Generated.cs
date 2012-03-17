namespace CommerceBuilder.Marketing
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of VolumeDiscount objects.
    /// </summary>
    public partial class VolumeDiscountCollection : PersistentCollection<VolumeDiscount>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="volumeDiscountId">Value of VolumeDiscountId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 volumeDiscountId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (volumeDiscountId == this[i].VolumeDiscountId) return i;
            }
            return -1;
        }
    }
}
