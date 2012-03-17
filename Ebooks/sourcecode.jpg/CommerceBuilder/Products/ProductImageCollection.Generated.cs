namespace CommerceBuilder.Products
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of ProductImage objects.
    /// </summary>
    public partial class ProductImageCollection : PersistentCollection<ProductImage>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="productImageId">Value of ProductImageId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 productImageId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (productImageId == this[i].ProductImageId) return i;
            }
            return -1;
        }
    }
}
