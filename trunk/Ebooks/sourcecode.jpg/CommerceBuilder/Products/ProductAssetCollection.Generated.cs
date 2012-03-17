namespace CommerceBuilder.Products
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of ProductAsset objects.
    /// </summary>
    public partial class ProductAssetCollection : PersistentCollection<ProductAsset>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="productAssetId">Value of ProductAssetId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 productAssetId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (productAssetId == this[i].ProductAssetId) return i;
            }
            return -1;
        }
    }
}
