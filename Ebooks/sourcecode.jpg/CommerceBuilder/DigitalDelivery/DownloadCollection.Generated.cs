namespace CommerceBuilder.DigitalDelivery
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of Download objects.
    /// </summary>
    public partial class DownloadCollection : PersistentCollection<Download>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="downloadId">Value of DownloadId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 downloadId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (downloadId == this[i].DownloadId) return i;
            }
            return -1;
        }
    }
}
