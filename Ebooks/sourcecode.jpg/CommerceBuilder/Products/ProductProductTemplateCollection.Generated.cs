namespace CommerceBuilder.Products
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of ProductProductTemplate objects.
    /// </summary>
    public partial class ProductProductTemplateCollection : PersistentCollection<ProductProductTemplate>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="productId">Value of ProductId of the required object.</param>
        /// <param name="productTemplateId">Value of ProductTemplateId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 productId, Int32 productTemplateId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((productId == this[i].ProductId) && (productTemplateId == this[i].ProductTemplateId)) return i;
            }
            return -1;
        }
    }
}
