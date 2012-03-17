// CUSTOMIZED
namespace CommerceBuilder.Marketing
{
    using System;
    using System.Collections.Generic;
    using CommerceBuilder.Common;

    /// <summary>
    /// This class implements a PersistentCollection of CouponProduct objects.
    /// </summary>
    public partial class CouponProductCollection : PersistentCollection<CouponProduct>
    {
        /// <summary>
        /// Gets an array of product Ids that are part of the collection
        /// </summary>
        /// <returns>An array of product Ids that are part of the collection</returns>
        public int[] GetProductIds()
        {
            List<int> productIds = new List<int>();
            for (int i = 0; i < this.Count; i++)
            {
                CouponProduct cp = this[i];
                if (cp.ProductId > 0 && !productIds.Contains(cp.ProductId))
                {
                    productIds.Add(cp.ProductId);
                }
            }
            if (productIds.Count == 0) return null;
            return productIds.ToArray();
        }

        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="couponId">Value of CouponId of the required object.</param>
        /// <param name="productId">Value of ProductId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 couponId, Int32 productId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((couponId == this[i].CouponId) && (productId == this[i].ProductId)) return i;
            }
            return -1;
        }
    }
}
