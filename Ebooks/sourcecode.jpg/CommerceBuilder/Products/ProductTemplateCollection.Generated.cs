namespace CommerceBuilder.Products
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of ProductTemplate objects.
    /// </summary>
    public partial class ProductTemplateCollection : PersistentCollection<ProductTemplate>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="productTemplateId">Value of ProductTemplateId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 productTemplateId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (productTemplateId == this[i].ProductTemplateId) return i;
            }
            return -1;
        }
    }
}
