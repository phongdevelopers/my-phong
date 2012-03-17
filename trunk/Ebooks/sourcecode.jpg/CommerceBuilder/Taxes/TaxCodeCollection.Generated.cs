namespace CommerceBuilder.Taxes
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of TaxCode objects.
    /// </summary>
    public partial class TaxCodeCollection : PersistentCollection<TaxCode>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="taxCodeId">Value of TaxCodeId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 taxCodeId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (taxCodeId == this[i].TaxCodeId) return i;
            }
            return -1;
        }
    }
}
