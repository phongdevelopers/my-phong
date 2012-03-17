namespace CommerceBuilder.Stores
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of StoreSetting objects.
    /// </summary>
    public partial class StoreSettingCollection : PersistentCollection<StoreSetting>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="storeSettingId">Value of StoreSettingId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 storeSettingId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (storeSettingId == this[i].StoreSettingId) return i;
            }
            return -1;
        }
    }
}
