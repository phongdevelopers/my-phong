namespace CommerceBuilder.Catalog
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of CatalogNode objects.
    /// </summary>
    public partial class CatalogNodeCollection : PersistentCollection<CatalogNode>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="categoryId">Value of CategoryId of the required object.</param>
        /// <param name="catalogNodeId">Value of CatalogNodeId of the required object.</param>
        /// <param name="catalogNodeTypeId">Value of CatalogNodeTypeId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 categoryId, Int32 catalogNodeId, Byte catalogNodeTypeId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((categoryId == this[i].CategoryId) && (catalogNodeId == this[i].CatalogNodeId) && (catalogNodeTypeId == this[i].CatalogNodeTypeId)) return i;
            }
            return -1;
        }
    }
}
