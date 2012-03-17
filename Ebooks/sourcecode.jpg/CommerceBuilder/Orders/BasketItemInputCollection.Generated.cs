namespace CommerceBuilder.Orders
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of BasketItemInput objects.
    /// </summary>
    public partial class BasketItemInputCollection : PersistentCollection<BasketItemInput>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="basketItemInputId">Value of BasketItemInputId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 basketItemInputId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (basketItemInputId == this[i].BasketItemInputId) return i;
            }
            return -1;
        }
    }
}
