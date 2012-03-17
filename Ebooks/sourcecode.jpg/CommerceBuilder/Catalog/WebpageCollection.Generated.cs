namespace CommerceBuilder.Catalog
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of Webpage objects.
    /// </summary>
    public partial class WebpageCollection : PersistentCollection<Webpage>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="webpageId">Value of WebpageId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 webpageId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (webpageId == this[i].WebpageId) return i;
            }
            return -1;
        }
    }
}
