namespace CommerceBuilder.Catalog
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of CustomUrl objects.
    /// </summary>
    public partial class CustomUrlCollection : PersistentCollection<CustomUrl>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="customUrlId">Value of CustomUrlId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 customUrlId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (customUrlId == this[i].CustomUrlId) return i;
            }
            return -1;
        }
    }
}
