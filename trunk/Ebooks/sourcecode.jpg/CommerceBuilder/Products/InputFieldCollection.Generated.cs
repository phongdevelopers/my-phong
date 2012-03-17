namespace CommerceBuilder.Products
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of InputField objects.
    /// </summary>
    public partial class InputFieldCollection : PersistentCollection<InputField>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="inputFieldId">Value of InputFieldId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 inputFieldId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (inputFieldId == this[i].InputFieldId) return i;
            }
            return -1;
        }
    }
}
