namespace CommerceBuilder.Orders
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of OrderNote objects.
    /// </summary>
    public partial class OrderNoteCollection : PersistentCollection<OrderNote>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="orderNoteId">Value of OrderNoteId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 orderNoteId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (orderNoteId == this[i].OrderNoteId) return i;
            }
            return -1;
        }
    }
}
