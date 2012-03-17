namespace CommerceBuilder.Users
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of GroupRole objects.
    /// </summary>
    public partial class GroupRoleCollection : PersistentCollection<GroupRole>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="groupId">Value of GroupId of the required object.</param>
        /// <param name="roleId">Value of RoleId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 groupId, Int32 roleId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((groupId == this[i].GroupId) && (roleId == this[i].RoleId)) return i;
            }
            return -1;
        }
    }
}
