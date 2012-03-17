namespace CommerceBuilder.Payments
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of Transaction objects.
    /// </summary>
    public partial class TransactionCollection : PersistentCollection<Transaction>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="transactionId">Value of TransactionId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 transactionId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (transactionId == this[i].TransactionId) return i;
            }
            return -1;
        }
    }
}
