namespace CommerceBuilder.Orders
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of OrderCoupon objects.
    /// </summary>
    public partial class OrderCouponCollection : PersistentCollection<OrderCoupon>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="orderId">Value of OrderId of the required object.</param>
        /// <param name="couponCode">Value of CouponCode of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 orderId, String couponCode)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((orderId == this[i].OrderId) && (couponCode == this[i].CouponCode)) return i;
            }
            return -1;
        }
    }
}
