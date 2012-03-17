namespace CommerceBuilder.Products
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of ProductCustomField objects.
    /// </summary>
    public partial class ProductCustomFieldCollection : PersistentCollection<ProductCustomField>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="productFieldId">Value of ProductFieldId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 productFieldId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (productFieldId == this[i].ProductFieldId) return i;
            }
            return -1;
        }
    }
}
