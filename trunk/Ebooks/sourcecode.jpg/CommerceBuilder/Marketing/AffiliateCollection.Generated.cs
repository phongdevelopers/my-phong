namespace CommerceBuilder.Marketing
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of Affiliate objects.
    /// </summary>
    public partial class AffiliateCollection : PersistentCollection<Affiliate>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="affiliateId">Value of AffiliateId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 affiliateId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (affiliateId == this[i].AffiliateId) return i;
            }
            return -1;
        }
    }
}
