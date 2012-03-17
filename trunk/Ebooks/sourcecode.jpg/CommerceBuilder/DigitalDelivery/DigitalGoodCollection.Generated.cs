namespace CommerceBuilder.DigitalDelivery
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of DigitalGood objects.
    /// </summary>
    public partial class DigitalGoodCollection : PersistentCollection<DigitalGood>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="digitalGoodId">Value of DigitalGoodId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 digitalGoodId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (digitalGoodId == this[i].DigitalGoodId) return i;
            }
            return -1;
        }
    }
}
