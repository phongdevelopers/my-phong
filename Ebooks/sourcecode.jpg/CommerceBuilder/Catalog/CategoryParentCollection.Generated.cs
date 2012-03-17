namespace CommerceBuilder.Catalog
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of CategoryParent objects.
    /// </summary>
    public partial class CategoryParentCollection : PersistentCollection<CategoryParent>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="categoryId">Value of CategoryId of the required object.</param>
        /// <param name="parentId">Value of ParentId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 categoryId, Int32 parentId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((categoryId == this[i].CategoryId) && (parentId == this[i].ParentId)) return i;
            }
            return -1;
        }
    }
}
