namespace CommerceBuilder.Marketing
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of EmailList objects.
    /// </summary>
    public partial class EmailListCollection : PersistentCollection<EmailList>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="emailListId">Value of EmailListId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 emailListId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (emailListId == this[i].EmailListId) return i;
            }
            return -1;
        }
    }
}
