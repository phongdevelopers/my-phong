namespace CommerceBuilder.Payments
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of GiftCertificate objects.
    /// </summary>
    public partial class GiftCertificateCollection : PersistentCollection<GiftCertificate>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="giftCertificateId">Value of GiftCertificateId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 giftCertificateId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (giftCertificateId == this[i].GiftCertificateId) return i;
            }
            return -1;
        }
    }
}
