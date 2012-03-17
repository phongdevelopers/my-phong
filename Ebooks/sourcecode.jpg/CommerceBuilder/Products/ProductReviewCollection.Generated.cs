namespace CommerceBuilder.Products
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of ProductReview objects.
    /// </summary>
    public partial class ProductReviewCollection : PersistentCollection<ProductReview>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="productReviewId">Value of ProductReviewId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 productReviewId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (productReviewId == this[i].ProductReviewId) return i;
            }
            return -1;
        }
    }
}
