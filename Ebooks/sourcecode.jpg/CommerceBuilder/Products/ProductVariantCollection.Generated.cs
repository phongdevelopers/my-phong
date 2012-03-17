namespace CommerceBuilder.Products
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of ProductVariant objects.
    /// </summary>
    public partial class ProductVariantCollection : PersistentCollection<ProductVariant>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="productVariantId">Value of ProductVariantId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 productVariantId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (productVariantId == this[i].ProductVariantId) return i;
            }
            return -1;
        }
    }
}
