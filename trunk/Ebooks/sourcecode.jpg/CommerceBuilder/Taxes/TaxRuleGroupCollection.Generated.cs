namespace CommerceBuilder.Taxes
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of TaxRuleGroup objects.
    /// </summary>
    public partial class TaxRuleGroupCollection : PersistentCollection<TaxRuleGroup>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="taxRuleId">Value of TaxRuleId of the required object.</param>
        /// <param name="groupId">Value of GroupId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 taxRuleId, Int32 groupId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((taxRuleId == this[i].TaxRuleId) && (groupId == this[i].GroupId)) return i;
            }
            return -1;
        }
    }
}
