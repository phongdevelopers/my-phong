namespace CommerceBuilder.Orders
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of Subscription objects.
    /// </summary>
    public partial class SubscriptionCollection : PersistentCollection<Subscription>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="subscriptionId">Value of SubscriptionId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 subscriptionId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (subscriptionId == this[i].SubscriptionId) return i;
            }
            return -1;
        }
    }
}
