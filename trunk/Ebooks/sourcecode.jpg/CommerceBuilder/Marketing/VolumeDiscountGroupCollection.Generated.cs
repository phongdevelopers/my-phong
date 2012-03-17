namespace CommerceBuilder.Marketing
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of VolumeDiscountGroup objects.
    /// </summary>
    public partial class VolumeDiscountGroupCollection : PersistentCollection<VolumeDiscountGroup>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="volumeDiscountId">Value of VolumeDiscountId of the required object.</param>
        /// <param name="groupId">Value of GroupId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 volumeDiscountId, Int32 groupId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((volumeDiscountId == this[i].VolumeDiscountId) && (groupId == this[i].GroupId)) return i;
            }
            return -1;
        }
    }
}
