namespace CommerceBuilder.Personalization
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of PersonalizationPath objects.
    /// </summary>
    public partial class PersonalizationPathCollection : PersistentCollection<PersonalizationPath>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="personalizationPathId">Value of PersonalizationPathId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 personalizationPathId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (personalizationPathId == this[i].PersonalizationPathId) return i;
            }
            return -1;
        }
    }
}
