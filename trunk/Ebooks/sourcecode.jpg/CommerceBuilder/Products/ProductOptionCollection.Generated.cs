namespace CommerceBuilder.Products
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of ProductOption objects.
    /// </summary>
    public partial class ProductOptionCollection : PersistentCollection<ProductOption>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="productId">Value of ProductId of the required object.</param>
        /// <param name="optionId">Value of OptionId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 productId, Int32 optionId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((productId == this[i].ProductId) && (optionId == this[i].OptionId)) return i;
            }
            return -1;
        }
    }
}
