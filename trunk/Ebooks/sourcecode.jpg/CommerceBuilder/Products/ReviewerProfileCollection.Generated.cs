namespace CommerceBuilder.Products
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of ReviewerProfile objects.
    /// </summary>
    public partial class ReviewerProfileCollection : PersistentCollection<ReviewerProfile>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="reviewerProfileId">Value of ReviewerProfileId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 reviewerProfileId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (reviewerProfileId == this[i].ReviewerProfileId) return i;
            }
            return -1;
        }
    }
}
