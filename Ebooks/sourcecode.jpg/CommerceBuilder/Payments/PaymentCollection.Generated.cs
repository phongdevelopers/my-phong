namespace CommerceBuilder.Payments
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of Payment objects.
    /// </summary>
    public partial class PaymentCollection : PersistentCollection<Payment>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="paymentId">Value of PaymentId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 paymentId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (paymentId == this[i].PaymentId) return i;
            }
            return -1;
        }
    }
}
