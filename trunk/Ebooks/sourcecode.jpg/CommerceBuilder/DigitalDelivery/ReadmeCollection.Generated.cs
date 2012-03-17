namespace CommerceBuilder.DigitalDelivery
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of Readme objects.
    /// </summary>
    public partial class ReadmeCollection : PersistentCollection<Readme>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="readmeId">Value of ReadmeId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 readmeId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (readmeId == this[i].ReadmeId) return i;
            }
            return -1;
        }
    }
}
