namespace CommerceBuilder.Products
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of ProductTemplateField objects.
    /// </summary>
    public partial class ProductTemplateFieldCollection : PersistentCollection<ProductTemplateField>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="productTemplateFieldId">Value of ProductTemplateFieldId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 productTemplateFieldId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (productTemplateFieldId == this[i].ProductTemplateFieldId) return i;
            }
            return -1;
        }
    }
}
