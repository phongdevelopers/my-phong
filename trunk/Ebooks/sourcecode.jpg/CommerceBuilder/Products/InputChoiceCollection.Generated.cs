namespace CommerceBuilder.Products
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of InputChoice objects.
    /// </summary>
    public partial class InputChoiceCollection : PersistentCollection<InputChoice>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="inputChoiceId">Value of InputChoiceId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 inputChoiceId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (inputChoiceId == this[i].InputChoiceId) return i;
            }
            return -1;
        }
    }
}
