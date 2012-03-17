namespace CommerceBuilder.Stores
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of CustomField objects.
    /// </summary>
    public partial class CustomFieldCollection : PersistentCollection<CustomField>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="customFieldId">Value of CustomFieldId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 customFieldId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (customFieldId == this[i].CustomFieldId) return i;
            }
            return -1;
        }
    }
}
