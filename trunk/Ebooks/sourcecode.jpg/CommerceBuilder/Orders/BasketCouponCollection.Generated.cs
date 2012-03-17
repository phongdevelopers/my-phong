namespace CommerceBuilder.Orders
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of BasketCoupon objects.
    /// </summary>
    public partial class BasketCouponCollection : PersistentCollection<BasketCoupon>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="basketId">Value of BasketId of the required object.</param>
        /// <param name="couponId">Value of CouponId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 basketId, Int32 couponId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((basketId == this[i].BasketId) && (couponId == this[i].CouponId)) return i;
            }
            return -1;
        }
    }
}
