namespace CommerceBuilder.Taxes
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of TaxRuleTaxCode objects.
    /// </summary>
    public partial class TaxRuleTaxCodeCollection : PersistentCollection<TaxRuleTaxCode>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="taxRuleId">Value of TaxRuleId of the required object.</param>
        /// <param name="taxCodeId">Value of TaxCodeId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 taxRuleId, Int32 taxCodeId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((taxRuleId == this[i].TaxRuleId) && (taxCodeId == this[i].TaxCodeId)) return i;
            }
            return -1;
        }
    }
}
