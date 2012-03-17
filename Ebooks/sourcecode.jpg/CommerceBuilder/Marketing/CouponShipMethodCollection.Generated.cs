namespace CommerceBuilder.Marketing
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of CouponShipMethod objects.
    /// </summary>
    public partial class CouponShipMethodCollection : PersistentCollection<CouponShipMethod>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="couponId">Value of CouponId of the required object.</param>
        /// <param name="shipMethodId">Value of ShipMethodId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 couponId, Int32 shipMethodId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((couponId == this[i].CouponId) && (shipMethodId == this[i].ShipMethodId)) return i;
            }
            return -1;
        }
    }
}
