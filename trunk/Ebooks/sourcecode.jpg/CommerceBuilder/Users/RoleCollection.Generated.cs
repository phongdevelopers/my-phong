namespace CommerceBuilder.Users
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of Role objects.
    /// </summary>
    public partial class RoleCollection : PersistentCollection<Role>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="roleId">Value of RoleId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 roleId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (roleId == this[i].RoleId) return i;
            }
            return -1;
        }
    }
}
