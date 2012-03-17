namespace CommerceBuilder.Payments
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of GiftCertificateTransaction objects.
    /// </summary>
    public partial class GiftCertificateTransactionCollection : PersistentCollection<GiftCertificateTransaction>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="giftCertificateTransactionId">Value of GiftCertificateTransactionId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 giftCertificateTransactionId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (giftCertificateTransactionId == this[i].GiftCertificateTransactionId) return i;
            }
            return -1;
        }
    }
}
