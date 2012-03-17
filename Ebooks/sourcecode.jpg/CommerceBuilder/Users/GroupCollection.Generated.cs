namespace CommerceBuilder.Users
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of Group objects.
    /// </summary>
    public partial class GroupCollection : PersistentCollection<Group>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="groupId">Value of GroupId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 groupId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (groupId == this[i].GroupId) return i;
            }
            return -1;
        }
    }
}
