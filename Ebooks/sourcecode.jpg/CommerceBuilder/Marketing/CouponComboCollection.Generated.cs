namespace CommerceBuilder.Marketing
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of CouponCombo objects.
    /// </summary>
    public partial class CouponComboCollection : PersistentCollection<CouponCombo>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="couponId">Value of CouponId of the required object.</param>
        /// <param name="comboCouponId">Value of ComboCouponId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 couponId, Int32 comboCouponId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((couponId == this[i].CouponId) && (comboCouponId == this[i].ComboCouponId)) return i;
            }
            return -1;
        }
    }
}
