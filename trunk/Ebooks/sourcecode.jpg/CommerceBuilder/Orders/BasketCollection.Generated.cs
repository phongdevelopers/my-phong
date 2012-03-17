namespace CommerceBuilder.Orders
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of Basket objects.
    /// </summary>
    public partial class BasketCollection : PersistentCollection<Basket>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="basketId">Value of BasketId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 basketId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (basketId == this[i].BasketId) return i;
            }
            return -1;
        }
    }
}
