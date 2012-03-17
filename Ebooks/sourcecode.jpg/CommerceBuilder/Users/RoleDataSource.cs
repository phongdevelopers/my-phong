using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Web;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Users
{
    [DataObject(true)]
    public partial class RoleDataSource
    {
        /// <summary>
        /// Makes sure that the default roles expected for a store are available
        /// in the database
        /// </summary>
        internal static void EnsureDefaultRoles()
        {
            string[] defaultRoles = Role.AllAdminRoles;
            //make sure the admin role exists
            foreach (string roleName in defaultRoles)
            {
                Role role = RoleDataSource.LoadForRolename(roleName);
                if (role == null)
                {
                    role = new Role();
                    role.Name = roleName;
                    role.Save();
                }
            }
        }

        /// <summary>
        /// Loads a role instance given the name.
        /// </summary>
        /// <param name="name">The case-insensitive role name.</param>
        /// <returns>The role instance, or null if the role is not found.</returns>
        public static Role LoadForRolename(string name)
        {
            int roleId = RoleDataSource.GetIdByName(name);
            return RoleDataSource.Load(roleId);
        }

        /// <summary>
        /// Gets the ID of a role given the name.
        /// </summary>
        /// <param name="name">The case-insensitive role name.</param>
        /// <returns>The int ID that represents the role, or 0 if the role is not found. </returns>
        public static int GetIdByName(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            return RoleCache.Instance.GetRoleId(name.ToLower());
        }

        /// <summary>
        /// Checks if the given user is in the specified role
        /// </summary>
        /// <param name="userId">Id of the user to check</param>
        /// <param name="roleName">The role for which to check user's membership</param>
        /// <returns><b>true</b> if user is in the specified role, <b>false</b> otherwise</returns>
        public static bool IsUserInRole(Int32 userId, string roleName)
        {
            int roleId = GetIdByName(roleName);
            return IsUserInRole(userId, roleId);
        }

        /// <summary>
        /// Checks if the given user is in the specified role
        /// </summary>
        /// <param name="userId">Id of the user to check</param>
        /// <param name="roleId">Id of the role for which to check user's membership</param>
        /// <returns><b>true</b> if user is in the specified role, <b>false</b> otherwise</returns>
        public static bool IsUserInRole(Int32 userId, Int32 roleId)
        {
            string sessionKey = "ac_Roles_User_" + userId;
            if (HttpContext.Current != null)
            {
                List<int> userRoles = HttpContext.Current.Items[sessionKey] as List<int>;
                // CHECK FOR THE SESSION ROLES
                if (userRoles == null)
                {
                    // ROLES NOT PRESENT, BUILD ARRAY
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SELECT DISTINCT GR.RoleId");
                    selectQuery.Append(" FROM ac_UserGroups UG INNER JOIN ac_GroupRoles GR ON UG.GroupId = GR.GroupId");
                    selectQuery.Append(" WHERE UG.UserId = @userId");
                    Database database = Token.Instance.Database;
                    DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
                    database.AddInParameter(selectCommand, "@userId", System.Data.DbType.Int32, userId);
                    userRoles = new List<int>();
                    using (IDataReader reader = database.ExecuteReader(selectCommand))
                    {
                        while (reader.Read())
                        {
                            userRoles.Add(reader.GetInt32(0));
                        }
                        reader.Close();
                    }
                    HttpContext.Current.Items[sessionKey] = userRoles;
                }
                return userRoles.Contains(roleId);
            }
            else
            {
                // NO SESSION AVAILABLE, RESORT TO QUERY
                StringBuilder selectQuery = new StringBuilder();
                selectQuery.Append("SELECT COUNT(*) AS RoleCount");
                selectQuery.Append(" FROM ac_UserGroups UG INNER JOIN ac_GroupRoles GR ON UG.GroupId = GR.GroupId");
                selectQuery.Append(" WHERE UG.UserId = @userId");
                selectQuery.Append(" AND GR.RoleId = @roleId");
                Database database = Token.Instance.Database;
                DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
                database.AddInParameter(selectCommand, "@userId", System.Data.DbType.Int32, userId);
                database.AddInParameter(selectCommand, "@roleId", System.Data.DbType.Int32, roleId);
                return (CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand)) > 0);
            }
        }

        /// <summary>
        /// Checks if the given group is in the specified role
        /// </summary>
        /// <param name="groupId">Id of the group to check</param>
        /// <param name="roleName">The role for which to check group's membership</param>
        /// <returns><b>true</b> if group is in the specified role, <b>false</b> otherwise</returns>
        public static bool IsGroupInRole(Int32 groupId, string roleName)
        {
            int roleId = GetIdByName(roleName);
            return IsGroupInRole(groupId, roleId);
        }

        /// <summary>
        /// Checks if the given group is in the specified role
        /// </summary>
        /// <param name="groupId">Id of the group to check</param>
        /// <param name="roleId">Id of the role for which to check group's membership</param>
        /// <returns><b>true</b> if group is in the specified role, <b>false</b> otherwise</returns>
        public static bool IsGroupInRole(Int32 groupId, Int32 roleId)
        {
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT COUNT(*) AS RoleCount");
            selectQuery.Append(" FROM ac_GroupRoles");
            selectQuery.Append(" WHERE GroupId = @groupId");
            selectQuery.Append(" AND RoleId = @roleId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@groupId", System.Data.DbType.Int32, groupId);
            database.AddInParameter(selectCommand, "@roleId", System.Data.DbType.Int32, roleId);
            return (CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand)) > 0);
        }

        /// <summary>
        /// Counts the number of roles in the system
        /// </summary>
        /// <returns>The total number of roles in the system</returns>
        public static int CountAll()
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalRecords FROM ac_Roles");
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads all roles in a RoleCollection
        /// </summary>
        /// <returns>A collection of all roles</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static RoleCollection LoadAll()
        {
            return RoleDataSource.LoadAll(0, 0, string.Empty);
        }

        /// <summary>
        /// Loads all roles in a RoleCollection
        /// </summary>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of all roles</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static RoleCollection LoadAll(string sortExpression)
        {
            return RoleDataSource.LoadAll(0, 0, sortExpression);
        }

        /// <summary>
        /// Loads all roles in a RoleCollection
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of all roles</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static RoleCollection LoadAll(int maximumRows, int startRowIndex)
        {
            return RoleDataSource.LoadAll(maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads all roles in a RoleCollection
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of all roles</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static RoleCollection LoadAll(int maximumRows, int startRowIndex, string sortExpression)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + Role.GetColumnNames("ac_Roles"));
            selectQuery.Append(" FROM ac_Roles");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            //EXECUTE THE COMMAND
            RoleCollection results = new RoleCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        Role role = new Role();
                        Role.LoadDataReader(role, dr);
                        results.Add(role);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Loads a collection of Role objects for the given user
        /// </summary>
        /// <param name="userId">Id of the user to load the roles for</param>
        /// <returns>A collection of Role objects for the given user</returns>
        public static RoleCollection LoadForUser(Int32 userId)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + Role.GetColumnNames("R"));
            selectQuery.Append(" FROM (ac_Roles R INNER JOIN ac_GroupRoles GR ON R.RoleId = GR.RoleId)");
            selectQuery.Append(" INNER JOIN ac_UserGroups UG ON GR.GroupId = UG.GroupId");
            selectQuery.Append(" WHERE UG.UserId = @userId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@userId", System.Data.DbType.Int32, userId);
            //EXECUTE THE COMMAND
            RoleCollection results = new RoleCollection();
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    Role role = new Role();
                    Role.LoadDataReader(role, dr);
                    results.Add(role);
                }
                dr.Close();
            }
            return results;
        }
    }
}
