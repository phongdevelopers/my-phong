namespace CommerceBuilder.Marketing
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of CouponGroup objects.
    /// </summary>
    public partial class CouponGroupCollection : PersistentCollection<CouponGroup>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="couponId">Value of CouponId of the required object.</param>
        /// <param name="groupId">Value of GroupId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 couponId, Int32 groupId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((couponId == this[i].CouponId) && (groupId == this[i].GroupId)) return i;
            }
            return -1;
        }
    }
}
