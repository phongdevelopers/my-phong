namespace CommerceBuilder.Products
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of Kit objects.
    /// </summary>
    public partial class KitCollection : PersistentCollection<Kit>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="productId">Value of ProductId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 productId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (productId == this[i].ProductId) return i;
            }
            return -1;
        }
    }
}
