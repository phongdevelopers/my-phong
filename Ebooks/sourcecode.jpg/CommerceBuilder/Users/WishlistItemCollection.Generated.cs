namespace CommerceBuilder.Users
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of WishlistItem objects.
    /// </summary>
    public partial class WishlistItemCollection : PersistentCollection<WishlistItem>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="wishlistItemId">Value of WishlistItemId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 wishlistItemId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (wishlistItemId == this[i].WishlistItemId) return i;
            }
            return -1;
        }
    }
}
