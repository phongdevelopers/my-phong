namespace CommerceBuilder.Marketing
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of ProductVolumeDiscount objects.
    /// </summary>
    public partial class ProductVolumeDiscountCollection : PersistentCollection<ProductVolumeDiscount>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="productId">Value of ProductId of the required object.</param>
        /// <param name="volumeDiscountId">Value of VolumeDiscountId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 productId, Int32 volumeDiscountId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((productId == this[i].ProductId) && (volumeDiscountId == this[i].VolumeDiscountId)) return i;
            }
            return -1;
        }
    }
}
