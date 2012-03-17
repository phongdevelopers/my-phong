namespace CommerceBuilder.Orders
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of BasketItem objects.
    /// </summary>
    public partial class BasketItemCollection : PersistentCollection<BasketItem>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="basketItemId">Value of BasketItemId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 basketItemId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (basketItemId == this[i].BasketItemId) return i;
            }
            return -1;
        }
    }
}
