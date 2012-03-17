namespace CommerceBuilder.Users
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of UserGroup objects.
    /// </summary>
    public partial class UserGroupCollection : PersistentCollection<UserGroup>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="userId">Value of UserId of the required object.</param>
        /// <param name="groupId">Value of GroupId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 userId, Int32 groupId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((userId == this[i].UserId) && (groupId == this[i].GroupId)) return i;
            }
            return -1;
        }
    }
}
