namespace CommerceBuilder.Catalog
{
    using System;
    using System.Collections.Generic;
    using CommerceBuilder.Common;

    /// <summary>
    /// Collection of CategoryLevelNode objects
    /// </summary>
    public partial class CategoryLevelNodeCollection : SortableCollection<CategoryLevelNode>
    {
        /// <summary>
        /// Gets the index of CategoryLevelNode object in this collection for which the category id 
        /// is the id specified
        /// </summary>
        /// <param name="categoryId">Category id to match in the CategoryLevelNode</param>
        /// <returns></returns>
        public int IndexOf(Int32 categoryId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (categoryId == this[i].CategoryId) return i;
            }
            return -1;
        }
    }
}
