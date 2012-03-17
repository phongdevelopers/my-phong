namespace CommerceBuilder.Reporting
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of PageView objects.
    /// </summary>
    public partial class PageViewCollection : PersistentCollection<PageView>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="pageViewId">Value of PageViewId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 pageViewId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (pageViewId == this[i].PageViewId) return i;
            }
            return -1;
        }
    }
}
