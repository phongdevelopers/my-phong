namespace CommerceBuilder.Products
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of WrapGroup objects.
    /// </summary>
    public partial class WrapGroupCollection : PersistentCollection<WrapGroup>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="wrapGroupId">Value of WrapGroupId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 wrapGroupId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (wrapGroupId == this[i].WrapGroupId) return i;
            }
            return -1;
        }
    }
}
