namespace CommerceBuilder.Products
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of Option objects.
    /// </summary>
    public partial class OptionCollection : PersistentCollection<Option>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="optionId">Value of OptionId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 optionId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (optionId == this[i].OptionId) return i;
            }
            return -1;
        }
    }
}
