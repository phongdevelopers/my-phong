namespace CommerceBuilder.Personalization
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of UserPersonalization objects.
    /// </summary>
    public partial class UserPersonalizationCollection : PersistentCollection<UserPersonalization>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="personalizationPathId">Value of PersonalizationPathId of the required object.</param>
        /// <param name="userId">Value of UserId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 personalizationPathId, Int32 userId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((personalizationPathId == this[i].PersonalizationPathId) && (userId == this[i].UserId)) return i;
            }
            return -1;
        }
    }
}
