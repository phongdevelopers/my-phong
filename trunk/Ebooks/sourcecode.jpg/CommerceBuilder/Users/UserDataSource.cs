using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Orders;
using CommerceBuilder.Shipping;
using CommerceBuilder.Utility;
using System.ComponentModel;
using System.Web.Security;
using System.Web;
using CommerceBuilder.Marketing;
using CommerceBuilder.Stores;

namespace CommerceBuilder.Users
{
    /// <summary>
    /// DataSource class for User objects
    /// </summary>
    [DataObject(true)]
    public partial class UserDataSource
    {
        /// <summary>
        /// Enumeration that represents the field to search
        /// </summary>
        public enum NameSearchField
        {
            /// <summary>
            /// User name field
            /// </summary>
            UserName,

            /// <summary>
            /// Email field
            /// </summary>
            Email, 
            
            /// <summary>
            /// Last name field
            /// </summary>
            LastName,
            
            /// <summary>
            /// Company field
            /// </summary>
            Company
        }

        /// <summary>
        /// Gets the email address given the user id
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <returns>The email address of the user</returns>
        public static string GetEmail(int userId)
        {
            Database database = Token.Instance.Database;
            DbCommand loadCommand = database.GetSqlStringCommand("SELECT Email FROM ac_Users WHERE UserId = @userId");
            database.AddInParameter(loadCommand, "@userId", DbType.String, userId);
            return AlwaysConvert.ToString(database.ExecuteScalar(loadCommand));
        }

        /// <summary>
        /// Gets UserId of a User object given the user name
        /// </summary>
        /// <param name="userName">The name of the user for which to get the user Id</param>
        /// <returns>If user is found the Id of the User object is returned, otherwise '0' is returned.</returns>
        public static int GetUserId(string userName)
        {
            Database database = Token.Instance.Database;
            DbCommand loadCommand = database.GetSqlStringCommand("SELECT UserId FROM ac_Users WHERE LoweredUserName = @userName");
            database.AddInParameter(loadCommand, "@userName", DbType.String, userName.ToLowerInvariant());
            return AlwaysConvert.ToInt(database.ExecuteScalar(loadCommand));
        }

        /// <summary>
        /// Gets a UserId given a username.
        /// </summary>
        /// <param name="userName">The username to look up.</param>
        /// <returns>The user id if the user is found.  If the user is not found, returns 0.</returns>
        public static int GetUserIdByUserName(string userName)
        {
            Database database = Token.Instance.Database;
            DbCommand loadCommand = database.GetSqlStringCommand("SELECT UserId FROM ac_Users WHERE LoweredUserName = @userName");
            database.AddInParameter(loadCommand, "@userName", DbType.String, userName.ToLowerInvariant());
            return AlwaysConvert.ToInt(database.ExecuteScalar(loadCommand));
        }

        /// <summary>
        /// Gets a UserId given an email address.
        /// </summary>
        /// <param name="email">The email address to look up.</param>
        /// <returns>The user id if the user is found.  If the user is not found, returns 0.</returns>
        /// <remarks>By default, anonymous user records are not included in the email lookup.</remarks>
        public static int GetUserIdByEmail(string email)
        {
            return GetUserIdByEmail(email, false);
        }

        /// <summary>
        /// Gets a UserId given an email address.
        /// </summary>
        /// <param name="email">The email address to look up.</param>
        /// <param name="includeAnonymous">Whether to include anonymous users are not</param>
        /// <returns>The user id if the user is found.  If the user is not found, returns 0.</returns>
        public static int GetUserIdByEmail(string email, bool includeAnonymous)
        {
            User user = LoadMostRecentForEmail(email, includeAnonymous);
            if (user == null) return 0;
            return user.UserId;
        }

        /// <summary>
        /// Sets the primary address Id of a user
        /// </summary>
        /// <param name="userId">Id of the user for which to set the primary address Id</param>
        /// <param name="addressId">The address Id to set</param>
        public static void UpdatePrimaryAddressId(int userId, int addressId)
        {
            Database database = Token.Instance.Database;
            DbCommand updateCommand = database.GetSqlStringCommand("UPDATE ac_Users SET PrimaryAddressId = @addressId WHERE UserId = @userId");
            database.AddInParameter(updateCommand, "@userId", DbType.Int32, userId);
            database.AddInParameter(updateCommand, "@addressId", DbType.Int32, addressId);
            database.ExecuteScalar(updateCommand);
        }

        /// <summary>
        /// Sets the primary wishlist Id of a user
        /// </summary>
        /// <param name="userId">Id of the user for which to set the primary wishlist Id</param>
        /// <param name="wishlistId">The wishlist Id to set</param>
        public static void UpdatePrimaryWishlistId(int userId, int wishlistId)
        {
            Database database = Token.Instance.Database;
            DbCommand updateCommand = database.GetSqlStringCommand("UPDATE ac_Users SET PrimaryWishlistId = @wishlistId WHERE UserId = @userId");
            database.AddInParameter(updateCommand, "@userId", DbType.Int32, userId);
            database.AddInParameter(updateCommand, "@wishlistId", DbType.Int32, wishlistId);
            database.ExecuteScalar(updateCommand);
        }

        /// <summary>
        /// Updates the last activity date of the given user to current date
        /// </summary>
        /// <param name="userId">Id of the user for whom last activity date is to be updated</param>
        public static void UpdateLastActivityDate(int userId)
        {
            UpdateLastActivityDate(userId, LocaleHelper.LocalNow);
        }

        /// <summary>
        /// Updates the last activity date of the given user to the given date
        /// </summary>
        /// <param name="userId">Id of the user for whom last activity date is to be updated</param>
        /// <param name="date">The value of last activity date to set</param>
        public static void UpdateLastActivityDate(int userId, DateTime date)
        {
            Database database = Token.Instance.Database;
            DbCommand updateCommand = database.GetSqlStringCommand("UPDATE ac_Users SET LastActivityDate = @lastActivityDate WHERE UserId = @userId");
            database.AddInParameter(updateCommand, "@userId", DbType.Int32, userId);
            database.AddInParameter(updateCommand, "@lastActivityDate", DbType.DateTime, NullableData.DbNullify(LocaleHelper.FromLocalTime(date)));
            database.ExecuteScalar(updateCommand);
        }

        /// <summary>
        /// Loads a User object given the user name
        /// </summary>
        /// <param name="userName">User name to look up for</param>
        /// <returns>The user object loaded or null if user with given name is not found</returns>
        public static User LoadForUserName(string userName)
        {
            return UserDataSource.LoadForUserName(userName, false);
        }

        /// <summary>
        /// Loads a User object given the user name
        /// </summary>
        /// <param name="userName">User name to look up for</param>
        /// <param name="createMissing">Whether to create a new user if a user with given name does not exist?</param>
        /// <returns>The user object loaded or null if user with given name is not found and is not created either</returns>
        public static User LoadForUserName(string userName, bool createMissing)
        {
            //LOOKUP UserName
            Database database = Token.Instance.Database;
            DbCommand loadCommand = database.GetSqlStringCommand("SELECT UserId FROM ac_Users WHERE LoweredUserName = @userName");
            database.AddInParameter(loadCommand, "@userName", DbType.String, userName.ToLowerInvariant());
            int userId = AlwaysConvert.ToInt(database.ExecuteScalar(loadCommand), 0);
            if (userId == 0)
            {
                if (!createMissing) return null;
                return CreateUserInstance(userName);
            }
            else return UserDataSource.Load(userId);
        }

        /// <summary>
        /// Loads User objects associated with the given email address
        /// </summary>
        /// <param name="email">Email address to lookup for</param>
        /// <returns>The collection user objects associated with the given email address</returns>
        /// <remarks>By default, anonymous user records are not included in the email lookup.</remarks>
        public static UserCollection LoadForEmail(string email)
        {
            return LoadForEmail(email, false);
        }

        /// <summary>
        /// Loads User objects associated with the given email address
        /// </summary>
        /// <param name="email">Email address to lookup for</param>
        /// <param name="includeAnonymous">Whether to include anonymous users or not</param>
        /// <returns>The collection user objects associated with the given email address</returns>
        public static UserCollection LoadForEmail(string email, bool includeAnonymous)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + User.GetColumnNames(""));
            selectQuery.Append(" FROM ac_Users ");
            selectQuery.Append(" WHERE LoweredEmail = @email AND StoreId = @storeId ");
            if (!includeAnonymous) selectQuery.Append(" AND IsAnonymous = 0 AND LoweredUserName NOT LIKE 'zz_anonymous_%domain.xyz'");
            selectQuery.Append(" ORDER BY CreateDate DESC");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@email", System.Data.DbType.String, email.ToLowerInvariant());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            //EXECUTE THE COMMAND
            UserCollection results = new UserCollection();
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    User user = new User();
                    User.LoadDataReader(user, dr);
                    results.Add(user);
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Loads a User object that was created last for the given email address
        /// </summary>
        /// <param name="email">Email address to lookup for</param>
        /// <returns>From the list of users associated with the given email address the user that was created last is returned</returns>
        public static User LoadMostRecentForEmail(string email)
        {
            return LoadMostRecentForEmail(email, true);
        }

        /// <summary>
        /// Loads a User object that was created last for the given email address
        /// </summary>
        /// <param name="email">Email address to lookup for</param>
        /// <param name="includeAnonymous">Whether to include anonymous users or not</param>
        /// <returns>From the list of users associated with the given email address the user that was created last is returned</returns>
        public static User LoadMostRecentForEmail(string email, bool includeAnonymous)
        {
            UserCollection users = LoadForEmail(email, includeAnonymous);
            if (users.Count > 0) return users[0];
            else return null;
        }

        /// <summary>
        /// Count users that match a particular search pattern.
        /// </summary>
        /// <param name="searchPattern">The search pattern to check against the user table.</param>
        /// <param name="searchField">The name field to check against the search pattern.</param>
        /// <returns>The number of users that match the search.</returns>
        public static int CountUsersByName(string searchPattern, NameSearchField searchField)
        {
            return CountUsersByName(searchPattern,searchField,false);            
        }

        /// <summary>
        /// Count users that match a particular search pattern.
        /// </summary>
        /// <param name="searchPattern">The search pattern to check against the user table.</param>
        /// <param name="searchField">The name field to check against the search pattern.</param>
        /// <param name="includeAnonymous">If <b>true</b> then the count will include the anonymous users as well</param>
        /// <returns>The number of users that match the search.</returns>
        public static int CountUsersByName(string searchPattern, NameSearchField searchField, bool includeAnonymous)
        {
            searchPattern = StringHelper.FixSearchPattern(searchPattern, false).ToLowerInvariant();            
            string whereClause = String.Empty;
            if (!string.IsNullOrEmpty(searchPattern))
            {
                if (searchField == NameSearchField.LastName)
                {
                    whereClause = " WHERE LOWER(A.LastName) LIKE @searchPattern";
                    if (!includeAnonymous) whereClause += " AND IsAnonymous = @isAnonymous";
                }
                else if (searchField == NameSearchField.Company)
                {
                    whereClause = " WHERE LOWER(A.Company) LIKE @searchPattern";
                    if (!includeAnonymous) whereClause += " AND IsAnonymous = @isAnonymous";
                }
                else if (searchField == NameSearchField.Email)
                {
                    whereClause = " WHERE U.LoweredEmail LIKE @searchPattern";
                    if (!includeAnonymous) whereClause += " AND IsAnonymous = @isAnonymous";
                }
                else if (searchField == NameSearchField.UserName)
                {
                    whereClause = " WHERE U.LoweredUserName LIKE @searchPattern";
                    if (!includeAnonymous) whereClause += " AND IsAnonymous = @isAnonymous";
                }
            }
            else
            {
                if (!includeAnonymous) whereClause = " WHERE U.IsAnonymous = @isAnonymous";
            }
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT COUNT(*) AS TotalRecords ");
            selectQuery.Append(" FROM ac_Users U LEFT JOIN ac_Addresses A ON U.PrimaryAddressId = A.AddressId");
            selectQuery.Append(whereClause);            
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            if (!string.IsNullOrEmpty(searchPattern))
            {
                database.AddInParameter(selectCommand, "searchPattern", DbType.String, searchPattern);
            }
            if (!includeAnonymous) database.AddInParameter(selectCommand, "isAnonymous", DbType.Boolean, false);

            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        
        /// <summary>
        /// Loads users that match a particular search pattern
        /// </summary>
        /// <param name="searchPattern">The search pattern to check against the user table.</param>
        /// <param name="searchField">The name field to check against the search pattern.</param>
        /// <param name="includeAnonymous">If <b>true</b> then the search results will include anonymous users as well</param>
        /// <returns>A collection of User objects matching the search criteria</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static UserCollection FindUsersByName(string searchPattern, NameSearchField searchField, bool includeAnonymous)
        {
            return FindUsersByName(searchPattern, searchField, includeAnonymous, 0, 0, string.Empty);
        }


        /// <summary>
        /// Loads users that match a particular search pattern
        /// </summary>
        /// <param name="searchPattern">The search pattern to check against the user table.</param>
        /// <param name="searchField">The name field to check against the search pattern.</param>
        /// <returns>A collection of User objects matching the search criteria</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static UserCollection FindUsersByName(string searchPattern, NameSearchField searchField)
        {
            return FindUsersByName(searchPattern, searchField, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads users that match a particular search pattern
        /// </summary>
        /// <param name="searchPattern">The search pattern to check against the user table.</param>
        /// <param name="searchField">The name field to check against the search pattern.</param>
        /// <param name="includeAnonymous">If <b>true</b> then the search results will include anonymous users as well</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of User objects matching the search criteria</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static UserCollection FindUsersByName(string searchPattern, NameSearchField searchField, bool includeAnonymous, string sortExpression)
        {
            return FindUsersByName(searchPattern, searchField, includeAnonymous, 0, 0, sortExpression);
        }


        /// <summary>
        /// Loads users that match a particular search pattern
        /// </summary>
        /// <param name="searchPattern">The search pattern to check against the user table.</param>
        /// <param name="searchField">The name field to check against the search pattern.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of User objects matching the search criteria</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static UserCollection FindUsersByName(string searchPattern, NameSearchField searchField, string sortExpression)
        {
            return FindUsersByName(searchPattern, searchField, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads users that match a particular search pattern
        /// </summary>
        /// <param name="searchPattern">The search pattern to check against the user table.</param>
        /// <param name="searchField">The name field to check against the search pattern.</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of User objects matching the search criteria</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static UserCollection FindUsersByName(string searchPattern, NameSearchField searchField, int maximumRows, int startRowIndex, string sortExpression)
        {
            return FindUsersByName(searchPattern, searchField,false, maximumRows, startRowIndex, sortExpression);
        }

        /// <summary>
        /// Loads users that match a particular search pattern
        /// </summary>
        /// <param name="searchPattern">The search pattern to check against the user table.</param>
        /// <param name="searchField">The name field to check against the search pattern.</param>
        /// <param name="includeAnonymous">If <b>true</b> then the search results will include anonymous users as well</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of User objects matching the search criteria</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static UserCollection FindUsersByName(string searchPattern, NameSearchField searchField, bool includeAnonymous, int maximumRows, int startRowIndex, string sortExpression)
        {
            UserCollection userCollection = new UserCollection();
            searchPattern = StringHelper.FixSearchPattern(searchPattern, false).ToLowerInvariant();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT            
            string whereClause = String.Empty;
            if (!string.IsNullOrEmpty(searchPattern))
            {
                if (searchField == NameSearchField.LastName)
                {                    
                    whereClause = " WHERE LOWER(A.LastName) LIKE @searchPattern";
                    if (!includeAnonymous) whereClause += " AND IsAnonymous = @isAnonymous";
                }
                else if (searchField == NameSearchField.Company)
                {
                    whereClause = " WHERE LOWER(A.Company) LIKE @searchPattern";
                    if (!includeAnonymous) whereClause += " AND IsAnonymous = @isAnonymous";
                }
                else if (searchField == NameSearchField.Email)
                {
                    whereClause = " WHERE U.LoweredEmail LIKE @searchPattern";
                    if (!includeAnonymous) whereClause += " AND IsAnonymous = @isAnonymous";                    
                }
                else if(searchField == NameSearchField.UserName)
                {
                    whereClause = " WHERE U.LoweredUserName LIKE @searchPattern";
                    if (!includeAnonymous) whereClause += " AND IsAnonymous = @isAnonymous";
                }
            }
            else
            {
                if (!includeAnonymous) whereClause = " WHERE U.IsAnonymous = @isAnonymous";
            }
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + User.GetColumnNames("U"));
            selectQuery.Append(" FROM ac_Users U LEFT JOIN ac_Addresses A ON U.PrimaryAddressId = A.AddressId");
            selectQuery.Append(whereClause);
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            if (!string.IsNullOrEmpty(searchPattern))
            {
                database.AddInParameter(selectCommand, "searchPattern", DbType.String, searchPattern);
            }
            if (!includeAnonymous) database.AddInParameter(selectCommand, "isAnonymous", DbType.Boolean, false);
            //EXECUTE THE COMMAND
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        User user = new User();
                        User.LoadDataReader(user, dr);
                        userCollection.Add(user);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return userCollection;
        }


        /// <summary>
        /// Count users that match a particular search pattern.
        /// </summary>
        /// <param name="searchPattern">The search pattern to check against the user table.</param>
        /// <param name="searchField">The name field to check against the search pattern.</param>
        /// <returns>The number of users that match the search.</returns>
        public static int FindUsersByNameCount(string searchPattern, NameSearchField searchField)
        {
            searchPattern = StringHelper.FixSearchPattern(searchPattern, false).ToLowerInvariant();
            string tables;
            string whereClause;
            if (!string.IsNullOrEmpty(searchPattern))
            {
                if (searchField == NameSearchField.LastName)
                {
                    tables = "ac_Users U INNER JOIN ac_Addresses A ON U.PrimaryAddressId = A.AddressId";
                    whereClause = " WHERE LOWER(A.LastName) LIKE @searchPattern AND U.IsAnonymous = 0";
                }
                else if (searchField == NameSearchField.Company)
                {
                    tables = "ac_Users U";
                    whereClause = " WHERE LOWER(A.Company) LIKE @searchPattern AND U.IsAnonymous = 0";                    
                }
                else if (searchField == NameSearchField.Email)
                {
                    tables = "ac_Users U";
                    whereClause = " WHERE U.LoweredEmail LIKE @searchPattern AND U.IsAnonymous = 0";
                }
                else
                {
                    tables = "ac_Users U";
                    whereClause = " WHERE U.LoweredUserName LIKE @searchPattern AND U.IsAnonymous = 0";
                }
            }
            else
            {
                tables = "ac_Users U";
                whereClause = " WHERE U.IsAnonymous = 0";
            }
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT COUNT(*) As RecordCount FROM " + tables);
            selectQuery.Append(whereClause);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            if (!string.IsNullOrEmpty(searchPattern))
            {
                database.AddInParameter(selectCommand, "searchPattern", DbType.String, searchPattern);
            }
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Create a new user, with a random generated password.
        /// </summary>
        /// <param name="email">Email address of the user to create.</param>
        /// <returns>A user instance if successful, or a null reference if the create fails.</returns>
        public static User CreateUser(string email)
        {
            MembershipCreateStatus status;
            string randomPassword = StringHelper.RandomString(10);
            return CreateUser(email, email, randomPassword, string.Empty, string.Empty, true, 0, out status);
        }

        /// <summary>
        /// Create a new user with the specified password.
        /// </summary>
        /// <param name="email">Email address of the user to create.</param>
        /// <param name="password">Password for the new user.</param>
        /// <returns>A user instance if successful, or a null reference if the create fails.</returns>
        public static User CreateUser(string email, string password)
        {
            MembershipCreateStatus status;
            return CreateUser(email, email, password, string.Empty, string.Empty, true, 0, out status);
        }

        /// <summary>
        /// Create a new user with the specified password, question, and answer.
        /// </summary>
        /// <param name="email">Email address of the user to create.</param>
        /// <param name="password">Password for the new user.</param>
        /// <param name="passwordQuestion">A challenge question to assist in recovery of lost passwords.</param>
        /// <param name="passwordAnswer">Answer to the challenge question.</param>
        /// <returns>A user instance if successful, or a null reference if the create fails.</returns>
        public static User CreateUser(string email, string password, string passwordQuestion, string passwordAnswer)
        {
            MembershipCreateStatus status;
            return CreateUser(email, email, password, passwordQuestion, passwordAnswer, true, 0, out status);
        }

        /// <summary>
        /// Create a new user with the specified password, question, and answer.
        /// </summary>
        /// <param name="username">User name of the user to create.</param>
        /// <param name="email">Email address of the user to create.</param>
        /// <param name="password">Password for the new user.</param>
        /// <param name="passwordQuestion">A challenge question to assist in recovery of lost passwords.</param>
        /// <param name="passwordAnswer">Answer to the challenge question.</param>
        /// <returns>A user instance if successful, or a null reference if the create fails.</returns>
        public static User CreateUser(string username, string email, string password, string passwordQuestion, string passwordAnswer)
        {
            MembershipCreateStatus status;
            return CreateUser(username, email, password, passwordQuestion, passwordAnswer, true, 0, out status);
        }

        /// <summary>
        /// Creates a new user with the specified parameters.
        /// </summary>
        /// <param name="username">User name of the user to create.</param>
        /// <param name="email">Email address of the user to create.  Pass in <b>zz_anonymous</b> to create a user for anonymous checkouts.</param>
        /// <param name="password">Password for the new user.</param>
        /// <param name="passwordQuestion">A challenge question to assist in recovery of lost passwords.</param>
        /// <param name="passwordAnswer">Answer to the challenge question.</param>
        /// <param name="isApproved">Indicates whether the new user is approved.</param>
        /// <param name="affiliateId">The ID of the affiliate</param>
        /// <param name="status">Result of the create operation.</param>
        /// <returns>A user instance if successful, or a null reference if the create fails.</returns>
        public static User CreateUser(string username, string email, string password, string passwordQuestion, string passwordAnswer, bool isApproved, int affiliateId, out System.Web.Security.MembershipCreateStatus status)
        {
            //get database reference
            Database database = Token.Instance.Database;

            //validate email
            bool anonUser = false;
            if (email != null && email.StartsWith("zz_anonymous"))
            {
                anonUser = true;
                //make sure we have a unique anonymous email
                email = "zz_anonymous_" + Guid.NewGuid().ToString("N") + "@domain.xyz";
                username = email;
            }
            else if (!ValidationHelper.IsValidEmail(email))
            {
                //email address not valid
                status = MembershipCreateStatus.InvalidEmail;
                return null;
            }

            //validate username
            if (string.IsNullOrEmpty(username))
            {
                status = MembershipCreateStatus.InvalidUserName;
                return null;
            }
            else if(!anonUser)
            {
                //CHECK FOR DUPLICATE USER NAME
                int userId = UserDataSource.GetUserIdByUserName(username);
                if (userId != 0)
                {
                    //error condition, duplicate user name provided to create method
                    status = MembershipCreateStatus.DuplicateUserName;
                    return null;
                }
            }

            //We allow duplicate email addresses bug 7567
            /*if (!anonUser)
            {
                //CHECK FOR DUPLICATE EMAIL
                DbCommand selectEmail = database.GetSqlStringCommand("SELECT Email FROM ac_Users WHERE Email = @email");
                database.AddInParameter(selectEmail, "email", DbType.String, email);
                string checkEmail = AlwaysConvert.ToString(database.ExecuteScalar(selectEmail));
                if (!string.IsNullOrEmpty(checkEmail))
                {
                    //error condition, duplicate userid provided to create method
                    status = MembershipCreateStatus.DuplicateEmail;
                    return null;
                }
            }*/
            
            try
            {
                // ENCODE THE PASSWORD AND PASSWORD ANSWER
                CustomerPasswordPolicy policy = new CustomerPasswordPolicy();
                string encodedPassword = UserPasswordHelper.EncodePassword(password, policy.PasswordFormat);
                string encodedPasswordAnswer = string.Empty;
                if (!string.IsNullOrEmpty(passwordAnswer)) encodedPasswordAnswer = UserPasswordHelper.EncodePassword(passwordAnswer, "SHA1");

                // CREATE THE USER INSTANCE
                User user = new User();
                user.CreateDate = LocaleHelper.LocalNow;
                user.UserName = username;
                user.Email = email;
                user.PasswordQuestion = passwordQuestion;
                user.PasswordAnswer = encodedPasswordAnswer;
                user.IsApproved = isApproved;

                // VALIDATE AND SET AFFILIATE
                if (AffiliateDataSource.Load(affiliateId) != null)
                {
                    user.AffiliateId = affiliateId;
                    user.AffiliateReferralDate = user.CreateDate;
                }

                // SET THE INITIAL PASSWORD
                UserPassword userPassword = new UserPassword();
                userPassword.Password = encodedPassword;
                userPassword.PasswordFormat = policy.PasswordFormat;
                userPassword.PasswordNumber = 1;
                userPassword.CreateDate = user.CreateDate;
                user.Passwords.Add(userPassword);

                // SAVE THE USER TO THE DATABASE
                user.Save();

                // MARK THE CREATION SUCCESSFUL
                status = MembershipCreateStatus.Success;
                
                //UDPATE USER ID FOR ANONYMOUS CHECKOUTS
                /* I don't see why we need to do this? Do we parse user id from user name somewhere?
                if (anonUser)
                {
                    user.UserName = "zz_anonymous_" + user.UserId.ToString() + "@domain.xyz";
                    user.Email = user.UserName;
                    user.Save();
                }*/
                return user;
            }
            catch
            {
                status = MembershipCreateStatus.ProviderError;
                return null;
            }
        }

        /// <summary>
        /// Creates a new user instance.
        /// </summary>
        /// <returns>A new user instance.</returns>
        /// <remarks>The user instance is only saved to the database if there is a new associated affiliate.</remarks>
        public static User CreateUserInstance()
        {
            return CreateUserInstance(Guid.NewGuid().ToString());
        }
        
        /// <summary>
        /// Creates a new user instance.
        /// </summary>
        /// <param name="userName">The username to assign to the user.</param>
        /// <returns>A new user instance.</returns>
        /// <remarks>The user instance is only saved to the database if cookies are enabled.</remarks>
        private static User CreateUserInstance(string userName)
        {
            User user = new User();
            user.UserName = userName;            
            user.IsAnonymous = (CommerceBuilder.Utility.AlwaysConvert.ToGuid(userName) != Guid.Empty);
            user.CreateDate = LocaleHelper.LocalNow;

            // MUST DETERMINE IF COOKIES ARE ENABLED IN THE REQUEST
            bool foundCookies = false;
            HttpRequest request = HttpContextHelper.SafeGetRequest();
            if (request != null)
            {
                // LOOK FOR COOKIES IN REQUEST
                string cookieString = request.ServerVariables["HTTP_COOKIE"];
                foundCookies = (!string.IsNullOrEmpty(cookieString));

                // ALSO CHECK FOR A VALID AFFILIATE INDICATOR
                Affiliate affiliate = AffiliateDataSource.Load(AlwaysConvert.ToInt(request.QueryString[Store.GetCachedSettings().AffiliateParameterName]));
                if (affiliate != null)
                {
                    // THIS NEW USER INSTANCE SHOULD ASSOCIATE TO AN AFFILIATE
                    user.AffiliateId = affiliate.AffiliateId;
                    user.AffiliateReferralDate = user.CreateDate;
                }
            }

            // ONLY COMMIT THE RECORD TO STORAGE IF COOKIES ARE ENABLED
            // IF WE SAVE WITHOUT COOKIES IT WILL CREATE A USER RECORD FOR EACH REQUEST
            if (foundCookies || user.AffiliateId > 0) user.Save();

            // RETURN THE NEW USER INSTANCE
            return user;
        }

        /// <summary>
        /// Checks whether there is a record for the specified user id.
        /// </summary>
        /// <param name="userId">The user ID to check.</param>
        /// <returns>True if the user exists in database, false otherwise.</returns>
        public static bool UserExists(int userId)
        {
            Database database = Token.Instance.Database;
            DbCommand loadCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS UserCount FROM ac_Users WHERE UserId = @userId");
            database.AddInParameter(loadCommand, "@userId", DbType.Int32, userId);
            return ((int)database.ExecuteScalar(loadCommand) == 1);
        }

        /// <summary>
        /// Loads a User object given its PayPal Id
        /// </summary>
        /// <param name="payPalId">The PayPal id to look up for</param>
        /// <returns>The User object loaded or null if no user with given PayPal Id is found</returns>
        public static User LoadForPayPalId(string payPalId)
        {
            //LOOKUP PayPalId
            Database database = Token.Instance.Database;
            DbCommand loadCommand = database.GetSqlStringCommand("SELECT UserId FROM ac_Users WHERE PayPalId = @payPalId");
            database.AddInParameter(loadCommand, "@payPalId", DbType.String, payPalId);
            int userId = AlwaysConvert.ToInt(database.ExecuteScalar(loadCommand), 0);
            if (!userId.Equals(0))
            {
                User user = new User();
                user.Load(userId);
                return user;
            }
            return null;
        }

        /// <summary>
        /// Loads a user object tiven a shipment
        /// </summary>
        /// <param name="shipment">The shipment to get the associated user for</param>
        /// <returns>The user object, or null if no associated user is available</returns>
        public static User LoadForShipment(IShipment shipment)
        {
            BasketShipment basketShipment = shipment as BasketShipment;
            if (basketShipment != null)
            {
                if (basketShipment.Basket != null) return basketShipment.Basket.User;
                return null;
            }
            OrderShipment orderShipment = shipment as OrderShipment;
            if (orderShipment != null)
            {
                if (orderShipment.Order != null) return orderShipment.Order.User;
                return null;
            }
            return null;
        }

        /// <summary>
        /// Gets a count of users who have active subscriptions associated with the given plan.
        /// </summary>
        /// <param name="subscriptionPlanId">ID of subscription plan to query</param>
        /// <returns>A count of users who have active subscriptions for the given plan.</returns>
        public static int CountForSubscriptionPlan(Int32 subscriptionPlanId)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT COUNT(DISTINCT UR.UserId) AS TotalRecords FROM ac_UserRoles UR INNER JOIN ac_Subscriptions S ON UR.SubscriptionId = S.SubscriptionId WHERE S.SubscriptionPlanId = @subscriptionPlanId");
            database.AddInParameter(selectCommand, "@subscriptionPlanId", System.Data.DbType.Int32, subscriptionPlanId);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads users that match a particular search pattern or group
        /// </summary>
        /// <param name="searchPattern">The search pattern to check against the user table.</param>
        /// <param name="searchField">The name field to check against the search pattern.</param>
        /// <param name="groupId">Id of the group to match</param>
        /// <param name="byName">If <b>true</b> match is made for search pattern otherwise it is made on group</param>
        /// <param name="includeAnonymous">If <b>true</b> search results will include the anonymous users</param>
        /// <returns>A collection of User objects matching the search criteria or group</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static UserCollection FindUsersByNameOrGroup(string searchPattern, NameSearchField searchField, int groupId, bool byName,  bool includeAnonymous)
        {
            return FindUsersByNameOrGroup(searchPattern, searchField, groupId, byName,includeAnonymous, string.Empty);
        }

        /// <summary>
        /// Loads users that match a particular search pattern or group
        /// </summary>
        /// <param name="searchPattern">The search pattern to check against the user table.</param>
        /// <param name="searchField">The name field to check against the search pattern.</param>
        /// <param name="groupId">Id of the group to match</param>
        /// <param name="byName">If <b>true</b> match is made for search pattern otherwise it is made on group</param>
        /// <returns>A collection of User objects matching the search criteria or group</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static UserCollection FindUsersByNameOrGroup(string searchPattern, NameSearchField searchField, int groupId, bool byName)
        {
            return FindUsersByNameOrGroup(searchPattern, searchField, groupId, byName,false, string.Empty);
        }

        /// <summary>
        /// Loads users that match a particular search pattern or group
        /// </summary>
        /// <param name="searchPattern">The search pattern to check against the user table.</param>
        /// <param name="searchField">The name field to check against the search pattern.</param>
        /// <param name="groupId">Id of the group to match</param>
        /// <param name="byName">If <b>true</b> match is made for search pattern otherwise it is made on group</param>
        /// <param name="includeAnonymous">If <b>true</b> search results will include the anonymous users</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of User objects matching the search criteria or group</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static UserCollection FindUsersByNameOrGroup(string searchPattern, NameSearchField searchField, int groupId, bool byName, bool includeAnonymous, string sortExpression)
        {
            return FindUsersByNameOrGroup(searchPattern, searchField, groupId, byName, includeAnonymous, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads users that match a particular search pattern or group
        /// </summary>
        /// <param name="searchPattern">The search pattern to check against the user table.</param>
        /// <param name="searchField">The name field to check against the search pattern.</param>
        /// <param name="groupId">Id of the group to match</param>
        /// <param name="byName">If <b>true</b> match is made for search pattern otherwise it is made on group</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of User objects matching the search criteria or group</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static UserCollection FindUsersByNameOrGroup(string searchPattern, NameSearchField searchField, int groupId, bool byName, string sortExpression)
        {
            return FindUsersByNameOrGroup(searchPattern, searchField, groupId, byName,false, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads users that match a particular search pattern or group
        /// </summary>
        /// <param name="searchPattern">The search pattern to check against the user table.</param>
        /// <param name="searchField">The name field to check against the search pattern.</param>
        /// <param name="groupId">Id of the group to match, pass zero (0) to search users having no group</param>
        /// <param name="byName">If <b>true</b> match is made for search pattern otherwise it is made on group</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of User objects matching the search criteria or group</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static UserCollection FindUsersByNameOrGroup(string searchPattern, NameSearchField searchField, int groupId, bool byName, int maximumRows, int startRowIndex, string sortExpression)
        {
            return FindUsersByNameOrGroup(searchPattern, searchField, groupId, byName, false, maximumRows, startRowIndex, sortExpression);
        }

        /// <summary>
        /// Loads users that match a particular search pattern or group
        /// </summary>
        /// <param name="searchPattern">The search pattern to check against the user table.</param>
        /// <param name="searchField">The name field to check against the search pattern.</param>
        /// <param name="groupId">Id of the group to match, pass zero (0) to search users having no group</param>
        /// <param name="byName">If <b>true</b> match is made for search pattern otherwise it is made on group</param>
        /// <param name="includeAnonymous">If <b>true</b> search results will include the anonymous users</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of User objects matching the search criteria or group</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static UserCollection FindUsersByNameOrGroup(string searchPattern, NameSearchField searchField, int groupId, bool byName, bool includeAnonymous, int maximumRows, int startRowIndex, string sortExpression)
        {
            if (byName)
            {
                return FindUsersByName(searchPattern, searchField,includeAnonymous, maximumRows, startRowIndex, sortExpression);
            }
            else
            {
                //CREATE THE DYNAMIC SQL TO LOAD OBJECT
                Database database = Token.Instance.Database;
                DbCommand selectCommand = null;
                StringBuilder selectQuery = new StringBuilder();
                if (groupId > 0)
                {
                    selectQuery.Append("SELECT");
                    if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
                    selectQuery.Append(" " + User.GetColumnNames("U"));
                    selectQuery.Append(" FROM ac_Users U ");
                    selectQuery.Append(" LEFT JOIN ac_UserGroups UG ON U.UserId = UG.UserId");
                    selectQuery.Append(" LEFT JOIN ac_Addresses A ON U.PrimaryAddressId = A.AddressId");
                    selectQuery.Append(" WHERE UG.GroupId = @groupId");
                    selectQuery.Append(" AND U.StoreId = @storeId");
                    if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
                    selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
                    database.AddInParameter(selectCommand, "@groupId", System.Data.DbType.Int32, groupId);
                    database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
                }
                else
                {
                    selectQuery.Append("SELECT");
                    if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
                    selectQuery.Append(" " + User.GetColumnNames("U"));
                    selectQuery.Append(" FROM ac_Users U ");
                    selectQuery.Append(" LEFT JOIN ac_Addresses A ON U.PrimaryAddressId = A.AddressId");
                    selectQuery.Append(" WHERE U.UserId NOT IN(SELECT UserId From ac_UserGroups)");
                    selectQuery.Append(" AND U.StoreId = @storeId");
                    if(!includeAnonymous) selectQuery.Append(" AND U.IsAnonymous = @isAnonymous");
                    if(!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
                    selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
                    database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
                    if(!includeAnonymous) database.AddInParameter(selectCommand, "@isAnonymous", System.Data.DbType.Boolean, false);
                }
                //EXECUTE THE COMMAND
                UserCollection results = new UserCollection();
                int thisIndex = 0;
                int rowCount = 0;
                using (IDataReader dr = database.ExecuteReader(selectCommand))
                {
                    while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                    {
                        if (thisIndex >= startRowIndex)
                        {
                            User user = new User();
                            User.LoadDataReader(user, dr);
                            results.Add(user);
                            rowCount++;
                        }
                        thisIndex++;
                    }
                    dr.Close();
                }
                return results;
            }
        }
        /// <summary>
        /// Count users that match a particular search pattern.
        /// </summary>
        /// <param name="searchPattern">The search pattern to check against the user table.</param>
        /// <param name="searchField">The name field to check against the search pattern.</param>
        /// <param name="groupId">Id of the group to match (pass zero (0) to count users having no group)</param>
        /// <param name="byName">If <b>true</b> match is made for search pattern otherwise it is made on group</param>        
        /// <returns>The number of users that match the search.</returns>
        public static int CountUsersByNameOrGroup(string searchPattern, NameSearchField searchField, int groupId, bool byName)
        {
            return CountUsersByNameOrGroup(searchPattern,searchField,groupId,byName,false);
        }


        /// <summary>
        /// Count users that match a particular search pattern.
        /// </summary>
        /// <param name="searchPattern">The search pattern to check against the user table.</param>
        /// <param name="searchField">The name field to check against the search pattern.</param>
        /// <param name="groupId">Id of the group to match (pass zero (0) to count users having no group)</param>
        /// <param name="byName">If <b>true</b> match is made for search pattern otherwise it is made on group</param>
        /// <param name="includeAnonymous">If <b>true</b> seach will include anonymous users</param>
        /// <returns>The number of users that match the search.</returns>
        public static int CountUsersByNameOrGroup(string searchPattern, NameSearchField searchField, int groupId, bool byName, bool includeAnonymous)
        {
            if (byName)
            {
                return CountUsersByName(searchPattern, searchField, includeAnonymous);
            }
            else
            {
                if (groupId > 0)
                    return CountForGroup(groupId);
                else
                {
                    Database database = Token.Instance.Database;
                    DbCommand selectCommand = null;
                    String sql = "SELECT COUNT(*) AS TotalRecords FROM ac_Users WHERE UserId NOT IN(SELECT UserId FROM ac_UserGroups)";
                    if(!includeAnonymous) sql += " AND IsAnonymous = 0";
                    selectCommand = database.GetSqlStringCommand(sql);
                    return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
                }
            }
        }

        /// <summary>
        /// Counts the total number of records that will be returned by the search criteria
        /// </summary>
        /// <param name="criteria">The user search criteria</param>
        /// <returns>The number of records that will be returned by the search criteria</returns>
        public static int SearchCount(UserSearchCriteria criteria)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = criteria.GenerateDatabaseCommand(true, string.Empty);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Searches the database for users that match the specified criteria
        /// </summary>
        /// <param name="criteria">The user search criteria</param>
        /// <returns>A collection of users that match the specified criteria</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static UserCollection Search(UserSearchCriteria criteria)
        {
            return Search(criteria, 0, 0, string.Empty);
        }

        /// <summary>
        /// Searches the database for users that match the specified criteria
        /// </summary>
        /// <param name="criteria">The user search criteria</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of users that match the specified criteria</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static UserCollection Search(UserSearchCriteria criteria, int maximumRows, int startRowIndex)
        {
            return Search(criteria, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Searches the database for users that match the specified criteria
        /// </summary>
        /// <param name="criteria">The user search criteria</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of users that match the specified criteria</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static UserCollection Search(UserSearchCriteria criteria, string sortExpression)
        {
            return Search(criteria, 0, 0, sortExpression);
        }

        /// <summary>
        /// Searches the database for users that match the specified criteria
        /// </summary>
        /// <param name="criteria">The user search criteria</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of users that match the specified criteria</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static UserCollection Search(UserSearchCriteria criteria, int maximumRows, int startRowIndex, string sortExpression)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = criteria.GenerateDatabaseCommand(false, sortExpression);
            // BUILD A LIST OF USER IDS THAT ARE WITHIN THE RANGE
            List<int> userIds = new List<int>();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        userIds.Add(dr.GetInt32(0));
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            // GET THE USERS IN THE RESULT SET
            // LOOP ONE BY ONE TO PRESERVE THE ORDER OF THE ITEMS (LOADBYCRITERIA CANNOT USE THE SAME SORTEXPRESSION)
            UserCollection results = new UserCollection();
            foreach (int userId in userIds)
            {
                User u = UserDataSource.Load(userId);
                if (u != null) results.Add(u);
            }
            return results;
        }

        public static AffiliateCollection LoadUserAffiliateAccounts(int userId)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" SELECT "+Affiliate.GetColumnNames("A"));
            sb.Append(" FROM ac_Affiliates AS A ");
            sb.Append(" INNER JOIN ac_Groups AS G ON A.GroupId = G.GroupId ");
            sb.Append(" INNER JOIN ac_UserGroups AS UG ON UG.GroupId = G.GroupId ");
            sb.Append(" WHERE UG.UserId = @userId AND A.StoreId = @storeId ");

            Database database = Token.Instance.Database;
            DbCommand command = database.GetSqlStringCommand(sb.ToString());
            database.AddInParameter(command, "@userId", DbType.Int32,userId);
            database.AddInParameter(command, "@storeId", DbType.Int32,Token.Instance.Store.StoreId);

            AffiliateCollection affiliates = new AffiliateCollection();

            using(IDataReader dr = database.ExecuteReader(command))
            {
                while(dr.Read())
                {
                    Affiliate affiliate = new Affiliate();
                    Affiliate.LoadDataReader(affiliate, dr);
                    affiliates.Add(affiliate);
                }
                dr.Close();
            }

            return affiliates;
        }
    }
}
