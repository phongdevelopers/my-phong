namespace CommerceBuilder.Seo
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of Redirect objects.
    /// </summary>
    public partial class RedirectCollection : PersistentCollection<Redirect>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="redirectId">Value of RedirectId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 redirectId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (redirectId == this[i].RedirectId) return i;
            }
            return -1;
        }
    }
}
