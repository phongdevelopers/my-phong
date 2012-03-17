namespace CommerceBuilder.Stores
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of Store objects.
    /// </summary>
    public partial class StoreCollection : PersistentCollection<Store>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="storeId">Value of StoreId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 storeId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (storeId == this[i].StoreId) return i;
            }
            return -1;
        }
    }
}
