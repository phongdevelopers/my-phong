using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.SessionState;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Users
{
    internal sealed class RoleCache
    {
        static readonly RoleCache _Instance = new RoleCache();

        static RoleCache()
        {
        }

        private Dictionary<string, int> roleIdLookup;

        private RoleCache()
        {
            this.InternalReload();
        }

        public static RoleCache Instance
        {
            get
            {
                return _Instance;
            }
        }

        /// <summary>
        /// Gets the role id given the name
        /// </summary>
        /// <param name="roleName">The lowered role name</param>
        /// <returns>The role id</returns>
        public int GetRoleId(string roleName)
        {
            if (roleIdLookup.ContainsKey(roleName)) return roleIdLookup[roleName];
            return 0;
        }

        /// <summary>
        /// Forces the cache to reload from the database
        /// </summary>
        public void Reload()
        {
            this.InternalReload();
        }

        private void InternalReload()
        {
            roleIdLookup = new Dictionary<string, int>();
            RoleCollection allRoles = RoleDataSource.LoadAll();
            foreach (Role role in allRoles)
            {
                roleIdLookup[role.LoweredName] = role.RoleId;
            }
        }
    }
}
