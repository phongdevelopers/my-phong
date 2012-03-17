using System;
using System.Data;
using System.Data.Common;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Utility;
using CommerceBuilder.Users;
using System.ComponentModel;
using System.Web.Profile;

namespace CommerceBuilder.Personalization
{
    /// <summary>
    /// DataSource class for Profile objects
    /// </summary>
    [DataObject(true)]
    public partial class ProfileDataSource
    {
        
        /// <summary>
        /// Loads profile for the given user
        /// </summary>
        /// <param name="userName">Name of the user to load profile for</param>
        /// <param name="createMissing">If <b>true</b> a profile will be created if it does not already exist.</param>
        /// <returns></returns>
        public static Profile LoadForUserName(string userName, bool createMissing)
        {
            User user = UserDataSource.LoadForUserName(userName, createMissing);
            if (user == null) return null;
            return user.Profile;
        }

        /// <summary>
        /// Deletes inactive profiles from database
        /// </summary>
        /// <param name="authenticationOption">The option that tells whether to include anonymous, non-anonymous or both users for deletion</param>
        /// <param name="userInactiveSinceDate">Date since the user is inactive</param>
        /// <returns>Number of profiles deleted</returns>
        public static int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            string inClause = "SELECT UserId FROM ac_Users WHERE StoreId = @storeId AND LastActivityDate <= @lastActivityDate " + GetClauseForAuthenticationOptions(authenticationOption);
            string sqlQuery = "DELETE FROM ac_Profiles WHERE UserId IN (" + inClause + ")";
            Database database = Token.Instance.Database;
            DbCommand deleteCommand = database.GetSqlStringCommand(sqlQuery);
            database.AddInParameter(deleteCommand, "storeId", DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(deleteCommand, "lastActivityDate", DbType.DateTime, userInactiveSinceDate.ToUniversalTime());
            return database.ExecuteNonQuery(deleteCommand);
        }

        /// <summary>
        /// Counts number of inactive profiles in the database
        /// </summary>
        /// <param name="authenticationOption">The option that tells whether to include anonymous, non-anonymous or both users</param>
        /// <param name="userInactiveSinceDate">Date since the user is inactive</param>
        /// <returns>Number of inactive profiles</returns>
        public static int CountInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            string sqlQuery = "SELECT COUNT(*) FROM ac_Users U, ac_Profiles P WHERE U.StoreId = @storeId AND U.LastActivityDate <= @lastActivityDate AND U.UserId = P.UserId " + GetClauseForAuthenticationOptions(authenticationOption);
            Database database = Token.Instance.Database;
            DbCommand countCommand = database.GetSqlStringCommand(sqlQuery);
            database.AddInParameter(countCommand, "storeId", DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(countCommand, "lastActivityDate", DbType.DateTime, userInactiveSinceDate.ToUniversalTime());
            return (int)database.ExecuteScalar(countCommand);
        }

        /// <summary>
        /// Gets a collection of all the profiles in the store
        /// </summary>
        /// <param name="authenticationOption">The option that tells whether to include anonymous, non-anonymous or both users</param>
        /// <param name="pageIndex">Index of the page for reading the data</param>
        /// <param name="pageSize">Size of a page to read</param>
        /// <param name="totalRecords">Total number of records read is returned in this parameter</param>
        /// <returns>A collection of all the profiles in the store</returns>
        public static ProfileInfoCollection GetAllProfiles(ProfileAuthenticationOption authenticationOption, int pageIndex, int pageSize, out int totalRecords)
        {
            string sqlQuery = @"SELECT U.UserName, U.IsAnonymous, U.LastActivityDate, P.LastUpdatedDate, LEN(P.PropertyNames) + LEN(P.PropertyValuesString) + LEN(P.PropertyValuesBinary) " +
                              @"FROM ac_Users U, ac_Profiles P " +
                              @"WHERE StoreId = @storeId AND U.UserId = P.UserId " + GetClauseForAuthenticationOptions(authenticationOption);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(sqlQuery);
            database.AddInParameter(selectCommand, "storeId", DbType.Int32, Token.Instance.StoreId);
            return GetProfilesForCommand(selectCommand, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// Gets a collection of inactive profiles in the store
        /// </summary>
        /// <param name="authenticationOption">The option that tells whether to include anonymous, non-anonymous or both users</param>
        /// <param name="userInactiveSinceDate">Date since the user is inactive</param>
        /// <param name="pageIndex">Index of the page for reading the data</param>
        /// <param name="pageSize">Size of a page to read</param>
        /// <param name="totalRecords">Total number of records read is returned in this parameter</param>
        /// <returns>A collection of all inactive profiles in the store</returns>
        public static ProfileInfoCollection GetAllInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            string sqlQuery = @"SELECT U.UserName, U.IsAnonymous, U.LastActivityDate, P.LastUpdatedDate, LEN(P.PropertyNames) + LEN(P.PropertyValuesString) + LEN(P.PropertyValuesBinary) " +
                              @"FROM ac_Users U, ac_Profiles P " +
                              @"WHERE StoreId = @storeId AND U.UserId = P.UserId AND U.LastActivityDate <= @lastActivityDate " + GetClauseForAuthenticationOptions(authenticationOption);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(sqlQuery);
            database.AddInParameter(selectCommand, "storeId", DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "lastActivityDate", DbType.DateTime, userInactiveSinceDate.ToUniversalTime());
            return GetProfilesForCommand(selectCommand, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// Gets a collection of profiles matching the given user name
        /// </summary>
        /// <param name="authenticationOption">The option that tells whether to include anonymous, non-anonymous or both users</param>
        /// <param name="usernameToMatch">The user name to match</param>
        /// <param name="pageIndex">Index of the page for reading the data</param>
        /// <param name="pageSize">Size of a page to read</param>
        /// <param name="totalRecords">Total number of records read is returned in this parameter</param>
        /// <returns>A collection of profiles matching the given user name</returns>
        public static ProfileInfoCollection FindProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            string sqlQuery = @"SELECT U.UserName, U.IsAnonymous, U.LastActivityDate, P.LastUpdatedDate, LEN(P.PropertyNames) + LEN(P.PropertyValuesString) + LEN(P.PropertyValuesBinary) " +
                              @"FROM ac_Users U, ac_Profiles P " +
                              @"WHERE StoreId = @storeId AND U.UserId = P.UserId AND U.UserName LIKE @userName " + GetClauseForAuthenticationOptions(authenticationOption);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(sqlQuery);
            database.AddInParameter(selectCommand, "storeId", DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "userName", DbType.String, usernameToMatch);
            return GetProfilesForCommand(selectCommand, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// Gets a collection of inactive profiles matching the given user name
        /// </summary>
        /// <param name="authenticationOption">The option that tells whether to include anonymous, non-anonymous or both users</param>
        /// <param name="usernameToMatch">The user name to match</param>
        /// <param name="userInactiveSinceDate">Date since the user is inactive</param>
        /// <param name="pageIndex">Index of the page for reading the data</param>
        /// <param name="pageSize">Size of a page to read</param>
        /// <param name="totalRecords">Total number of records read is returned in this parameter</param>
        /// <returns>A collection of inactive profiles matching the given user name</returns>
        public static ProfileInfoCollection FindInactiveProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            string sqlQuery = @"SELECT U.UserName, U.IsAnonymous, U.LastActivityDate, P.LastUpdatedDate, LEN(P.PropertyNames) + LEN(P.PropertyValuesString) + LEN(P.PropertyValuesBinary) " +
                              @"FROM ac_Users U, ac_Profiles P " +
                              @"WHERE StoreId = @storeId AND U.UserId = P.UserId AND U.UserName LIKE @userName AND U.LastActivityDate <= @lastActivityDate " + GetClauseForAuthenticationOptions(authenticationOption);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(sqlQuery);
            database.AddInParameter(selectCommand, "storeId", DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "userName", DbType.String, usernameToMatch);
            database.AddInParameter(selectCommand, "lastActivityDate", DbType.DateTime, userInactiveSinceDate.ToUniversalTime());
            return GetProfilesForCommand(selectCommand, pageIndex, pageSize, out totalRecords);
        }

        private static ProfileInfoCollection GetProfilesForCommand(DbCommand selectCommand, int pageIndex, int pageSize, out int totalRecords)
        {
            if (pageIndex < 0) throw new ArgumentException("Page index must be non-negative", "pageIndex");
            if (pageSize < 1) throw new ArgumentException("Page size must be positive", "pageSize");

            long lBound = (long)pageIndex * pageSize;
            long uBound = lBound + pageSize - 1;

            if (uBound > System.Int32.MaxValue)
            {
                throw new ArgumentException("pageIndex*pageSize too large");
            }

            Database database = Token.Instance.Database;
            ProfileInfoCollection profiles = new ProfileInfoCollection();
            using (IDataReader reader = database.ExecuteReader(selectCommand))
            {
                totalRecords = 0;
                while (reader.Read())
                {
                    totalRecords++;
                    if (totalRecords - 1 < lBound || totalRecords - 1 > uBound)
                        continue;

                    string username = reader.GetString(0);
                    bool isAnon = reader.GetBoolean(1);
                    DateTime dtLastActivity = LocaleHelper.ToLocalTime(reader.GetDateTime(2));
                    DateTime dtLastUpdated = LocaleHelper.ToLocalTime(reader.GetDateTime(3));
                    int size = reader.GetInt32(4);
                    profiles.Add(new ProfileInfo(username, isAnon, dtLastActivity, dtLastUpdated, size));
                }
                reader.Close();
            }

            return profiles;
        }


        private static string GetClauseForAuthenticationOptions(ProfileAuthenticationOption authenticationOption)
        {
            switch (authenticationOption)
            {
                case ProfileAuthenticationOption.Anonymous:
                    return " AND IsAnonymous=Yes ";

                case ProfileAuthenticationOption.Authenticated:
                    return " AND IsAnonymous=No ";

                case ProfileAuthenticationOption.All:
                    return " ";
            }
            return " ";
        }


    }
}
