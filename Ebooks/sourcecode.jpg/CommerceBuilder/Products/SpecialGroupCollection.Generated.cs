namespace CommerceBuilder.Products
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of SpecialGroup objects.
    /// </summary>
    public partial class SpecialGroupCollection : PersistentCollection<SpecialGroup>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="specialId">Value of SpecialId of the required object.</param>
        /// <param name="groupId">Value of GroupId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 specialId, Int32 groupId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((specialId == this[i].SpecialId) && (groupId == this[i].GroupId)) return i;
            }
            return -1;
        }
    }
}
