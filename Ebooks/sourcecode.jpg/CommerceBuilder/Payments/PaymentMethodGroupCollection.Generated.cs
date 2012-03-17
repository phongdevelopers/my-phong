namespace CommerceBuilder.Payments
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of PaymentMethodGroup objects.
    /// </summary>
    public partial class PaymentMethodGroupCollection : PersistentCollection<PaymentMethodGroup>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="paymentMethodId">Value of PaymentMethodId of the required object.</param>
        /// <param name="groupId">Value of GroupId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 paymentMethodId, Int32 groupId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((paymentMethodId == this[i].PaymentMethodId) && (groupId == this[i].GroupId)) return i;
            }
            return -1;
        }
    }
}
