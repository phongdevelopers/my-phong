namespace CommerceBuilder.Marketing
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of VolumeDiscountLevel objects.
    /// </summary>
    public partial class VolumeDiscountLevelCollection : PersistentCollection<VolumeDiscountLevel>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="volumeDiscountLevelId">Value of VolumeDiscountLevelId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 volumeDiscountLevelId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (volumeDiscountLevelId == this[i].VolumeDiscountLevelId) return i;
            }
            return -1;
        }
    }
}
