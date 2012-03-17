namespace CommerceBuilder.Marketing
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of EmailListUser objects.
    /// </summary>
    public partial class EmailListUserCollection : PersistentCollection<EmailListUser>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="emailListId">Value of EmailListId of the required object.</param>
        /// <param name="email">Value of Email of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 emailListId, String email)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((emailListId == this[i].EmailListId) && (email == this[i].Email)) return i;
            }
            return -1;
        }
    }
}
