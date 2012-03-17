//-----------------------------------------------------------------------
// <copyright file="Role.cs" company="Able Solutions Corporation">
//     Copyright (c) 2009 Able Solutions Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace CommerceBuilder.Users
{
    using CommerceBuilder.Common;

    /// <summary>
    /// Class that represents a Role object in the database
    /// </summary>
    public partial class Role
    {
        /// <summary>
        /// All merchant admin related roles
        /// </summary>
        public static string[] AllAdminRoles = { "System", "Admin", "Jr. Admin", "Manage Orders", "Manage Catalog", "Manage Website", "View Reports" };
        
        /// <summary>
        /// System Administrator roles
        /// </summary>
        public static string[] SystemAdminRoles = { "System" };

        /// <summary>
        /// Admin roles
        /// </summary>
        public static string[] AdminRoles = { "System", "Admin" };

        /// <summary>
        /// Junior admin roles
        /// </summary>
        public static string[] JrAdminRoles = { "System", "Admin", "Jr. Admin" };

        /// <summary>
        /// Website admin roles
        /// </summary>
        public static string[] WebsiteAdminRoles = { "System", "Admin", "Jr. Admin", "Manage Website" };

        /// <summary>
        /// Catalog admin roles
        /// </summary>
        public static string[] CatalogAdminRoles = { "System", "Admin", "Jr. Admin", "Manage Catalog" };

        /// <summary>
        /// Order admin roles
        /// </summary>
        public static string[] OrderAdminRoles = { "System", "Admin", "Jr. Admin", "Manage Orders" };

        /// <summary>
        /// Report admin roles
        /// </summary>
        public static string[] ReportAdminRoles = { "System", "Admin", "Jr. Admin", "View Reports" };

        /// <summary>
        /// Deletes this Role object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public bool Delete()
        {
            bool result = this.BaseDelete();
            RoleCache.Instance.Reload();
            return result;
        }

        /// <summary>
        /// Saves this Role object to the database.
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public SaveResult Save()
        {
            SaveResult result = this.BaseSave();
            RoleCache.Instance.Reload();
            return result;
        }
    }
}
