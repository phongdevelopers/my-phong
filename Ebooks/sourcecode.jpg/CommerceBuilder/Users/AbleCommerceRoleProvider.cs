using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Stores;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Users
{
    /// <summary>
    /// Custom implementation of an ASP.NET Role Provider for AbleCommerce
    /// </summary>
    public class AbleCommerceRoleProvider : System.Web.Security.RoleProvider
    {
        /// <summary>
        /// Adds the specified user names to the specified roles for the configured application Name. 
        /// </summary>
        /// <param name="usernames">A string array of user names to be added to the specified roles.</param>
        /// <param name="roleNames">A string array of the role names to add the specified user names to.</param>
        /// <remarks>This method is not implemented in AbleCommerce 7.</remarks>
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new RoleProviderException("The method AddUsersToRoles is not implemented.");
        }

        /// <summary>
        /// Gets or sets the name of the application configured for the provider.
        /// </summary>
        /// <remarks>AbleCommerce only supports a single application configured at "/".</remarks>
        public override string ApplicationName
        {
            get { return "/"; }
            set { }
        }

        /// <summary>
        /// Adds a new role to the data source for the configured applicationName
        /// </summary>
        /// <param name="roleName">The role to add</param>
        public override void CreateRole(string roleName)
        {
            Role role = new Role();
            role.Name = roleName;
            role.Save();
        }

        /// <summary>
        /// Removes a role from the data source for the configured application Name
        /// </summary>
        /// <param name="roleName">The role to delete</param>
        /// <param name="throwOnPopulatedRole">If true, throw an exception if roleName has one or more members and do not delete roleName.</param>
        /// <returns><b>true</b> if role is deleted <b>false</b> otherwise</returns>
        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            Role role = RoleDataSource.LoadForRolename(roleName);
            if (role == null) return false;
            return role.Delete();
        }

        /// <summary>
        /// Gets an array of user names in a role where the user name contains the specified user name to match.
        /// </summary>
        /// <param name="roleName">The role to search in.</param>
        /// <param name="usernameToMatch">The user name to search for.</param>
        /// <returns>A string array containing the names of all the users where the user name matches usernameToMatch and the user is a member of the specified role.</returns>
        /// <remarks>This method is not implemented in AbleCommerce 7.</remarks>
        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new RoleProviderException("The method FindUsersInRole is not implemented.");
            /*
            Role role = RoleDataSource.LoadForRolename(roleName);
            if (role == null) return null;
            UserRoleCollection userList = role.UserRoles;
            List<string> usersInRole = new List<string>();
            string pattern = usernameToMatch.Replace("?", ".?").Replace("*", ".*");
            for (int index = 0; index < userList.Count; index++)
            {
                if (Regex.Match(userList[index].User.UserName, pattern).Success)
                {
                    usersInRole.Add(userList[index].User.UserName);
                }
            }
            if (usersInRole.Count == 0) return null;
            return usersInRole.ToArray();
            */
        }

        /// <summary>
        /// Gets a list of all the roles for the configured applicationName. 
        /// </summary>
        /// <returns>A string array containing the names of all the roles stored in the data source for the configured applicationName.</returns>
        public override string[] GetAllRoles()
        {
            RoleCollection roleCollection = RoleDataSource.LoadAll();
            string[] allRoles = new string[roleCollection.Count];
            for (int index = 0; index < roleCollection.Count; index++)
            {
                allRoles[index] = roleCollection[index].Name;
            }
            return allRoles;
        }

        /// <summary>
        /// Gets a list of the roles that a specified user is in for the configured applicationName. 
        /// </summary>
        /// <param name="username">The user to return a list of roles for.</param>
        /// <returns>A string array containing the names of all the roles that the specified user is in for the configured applicationName.</returns>
        public override string[] GetRolesForUser(string username)
        {
            string[] userRoles = new string[0];
            User user = UserDataSource.LoadForUserName(username);
            if (user != null)
            {
                RoleCollection roles = RoleDataSource.LoadForUser(user.UserId);
                if (roles.Count > 0)
                {
                    userRoles = new string[roles.Count];
                    for (int i = 0; i < roles.Count; i++)
                    {
                        userRoles[i] = roles[i].Name;
                    }
                }
            }
            return userRoles;
        }

        /// <summary>
        /// Gets a list of users in the specified role for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The name of the role to get the list of users for.</param>
        /// <returns>A string array containing the names of all the users who are members of the specified role for the configured applicationName.</returns>
        /// <remarks>This method is not implemented in AbleCommerce 7.</remarks>
        public override string[] GetUsersInRole(string roleName)
        {
            throw new RoleProviderException("The method GetUsersInRole is not implemented.");
            /*
            Role role = RoleDataSource.LoadForRolename(roleName);
            if (role == null) return null;
            UserRoleCollection userList = role.UserRoles;
            string[] usersInRole = new string[userList.Count];
            for (int index = 0; index < userList.Count; index++)
            {
                usersInRole[index] = userList[index].User.UserName;
            }
            return usersInRole; 
            */
        }

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="name">The friendly name of the provider.</param>
        /// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);
        }

        /// <summary>
        /// Gets a value indicating whether the specified user is in the specified role for the configured applicationName. 
        /// </summary>
        /// <param name="username">The name of the user to check for</param>
        /// <param name="roleName">The role to check for</param>
        /// <returns><b>true</b> if user is in role, <b>false</b> otherwise</returns>
        public override bool IsUserInRole(string username, string roleName)
        {
            User user = UserDataSource.LoadForUserName(username);
            if (user == null) return false;
            return RoleDataSource.IsUserInRole(user.UserId, roleName);
        }

        /// <summary>
        /// Gets a value indicating whether the specified user is in the specified role for the configured applicationName. 
        /// </summary>
        /// <param name="user">The user to check for</param>
        /// <param name="roleName">The role to check for</param>
        /// <returns><b>true</b> if user is in role, <b>false</b> otherwise</returns>
        public bool IsUserInRole(User user, string roleName)
        {
            if (user == null) return false;
            return RoleDataSource.IsUserInRole(user.UserId, roleName);
        }

        /// <summary>
        /// Removes the specified user names from the specified roles for the configured applicationName.
        /// </summary>
        /// <param name="usernames">A string array of user names to be removed from the specified roles.</param>
        /// <param name="roleNames">A string array of role names to remove the specified user names from.</param>
        /// <remarks>This method is not implemented in AbleCommerce 7.</remarks>
        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new RoleProviderException("The method RemoveUsersFromRoles is not implemented.");
        }

        /// <summary>
        /// Gets a value indicating whether the specified role name already exists in the role data source for the configured applicationName. 
        /// </summary>
        /// <param name="roleName">The role name to check</param>
        /// <returns><b>true</b> if role already exists, <b>false</b> otherwise</returns>
        public override bool RoleExists(string roleName)
        {
            return (RoleDataSource.GetIdByName(roleName) > 0);
        }
    }
}
