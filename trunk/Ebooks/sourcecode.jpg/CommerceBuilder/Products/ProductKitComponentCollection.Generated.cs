namespace CommerceBuilder.Products
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of ProductKitComponent objects.
    /// </summary>
    public partial class ProductKitComponentCollection : PersistentCollection<ProductKitComponent>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="productId">Value of ProductId of the required object.</param>
        /// <param name="kitComponentId">Value of KitComponentId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 productId, Int32 kitComponentId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((productId == this[i].ProductId) && (kitComponentId == this[i].KitComponentId)) return i;
            }
            return -1;
        }
    }
}
