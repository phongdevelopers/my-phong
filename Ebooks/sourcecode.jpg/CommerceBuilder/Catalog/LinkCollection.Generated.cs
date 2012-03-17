namespace CommerceBuilder.Catalog
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of Link objects.
    /// </summary>
    public partial class LinkCollection : PersistentCollection<Link>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="linkId">Value of LinkId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 linkId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (linkId == this[i].LinkId) return i;
            }
            return -1;
        }
    }
}
