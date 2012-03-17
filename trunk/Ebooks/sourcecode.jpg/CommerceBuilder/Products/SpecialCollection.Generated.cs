namespace CommerceBuilder.Products
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of Special objects.
    /// </summary>
    public partial class SpecialCollection : PersistentCollection<Special>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="specialId">Value of SpecialId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 specialId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (specialId == this[i].SpecialId) return i;
            }
            return -1;
        }
    }
}
