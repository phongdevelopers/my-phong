namespace CommerceBuilder.Utility
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of ErrorMessage objects.
    /// </summary>
    public partial class ErrorMessageCollection : PersistentCollection<ErrorMessage>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="errorMessageId">Value of ErrorMessageId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 errorMessageId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (errorMessageId == this[i].ErrorMessageId) return i;
            }
            return -1;
        }
    }
}
