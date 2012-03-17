using System.Collections.Generic;
namespace CommerceBuilder.Users
{
    public partial class Group
    {
        /// <summary>
        /// Determines whether this group has the given role assigned.
        /// </summary>
        /// <param name="name">The name of the role to check.</param>
        /// <returns>True if the group has the given role; false otherwise.</returns>
        public bool IsInRole(string name)
        {
            return RoleDataSource.IsGroupInRole(this.GroupId, name);
        }

        /// <summary>
        /// Determines whether this group has any of the given roles.
        /// </summary>
        /// <param name="roleCollection">A collection of role names to check.</param>
        /// <returns>True if the group has any of the given roles; false otherwise.</returns>
        public bool IsInRole(ICollection<string> roleCollection)
        {
            foreach (string roleName in roleCollection)
            {
                if (this.IsInRole(roleName)) return true;
            }
            return false;
        }
    }
}
