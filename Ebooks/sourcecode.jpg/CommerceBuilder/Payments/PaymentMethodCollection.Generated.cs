namespace CommerceBuilder.Payments
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of PaymentMethod objects.
    /// </summary>
    public partial class PaymentMethodCollection : PersistentCollection<PaymentMethod>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="paymentMethodId">Value of PaymentMethodId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 paymentMethodId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (paymentMethodId == this[i].PaymentMethodId) return i;
            }
            return -1;
        }
    }
}
