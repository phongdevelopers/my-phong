namespace CommerceBuilder.Users
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of Wishlist objects.
    /// </summary>
    public partial class WishlistCollection : PersistentCollection<Wishlist>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="wishlistId">Value of WishlistId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 wishlistId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (wishlistId == this[i].WishlistId) return i;
            }
            return -1;
        }
    }
}
