namespace CommerceBuilder.Products
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of RelatedProduct objects.
    /// </summary>
    public partial class RelatedProductCollection : PersistentCollection<RelatedProduct>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="productId">Value of ProductId of the required object.</param>
        /// <param name="childProductId">Value of ChildProductId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 productId, Int32 childProductId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((productId == this[i].ProductId) && (childProductId == this[i].ChildProductId)) return i;
            }
            return -1;
        }
    }
}
