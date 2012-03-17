using System.Configuration;
using System;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Collections.ObjectModel;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;
using System.Web.Security;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace CommerceBuilder.Users
{
    /// <summary>
    /// Custom implementation of an ASP.NET Membership Provider for AbleCommerce
    /// </summary>
    public class AbleCommerceMembershipProvider : System.Web.Security.MembershipProvider
    {
        private PasswordPolicy _PasswordPolicy;

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
        /// Processes a request to update the password for a membership user.
        /// </summary>
        /// <param name="username">The user to update the password for. </param>
        /// <param name="oldPassword">The current password for the specified user.</param>
        /// <param name="newPassword">The new password for the specified user. </param>
        /// <returns>true if the password was updated successfully; otherwise, false.</returns>
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            User user = UserDataSource.LoadForUserName(username);
            if ((user != null) && user.CheckPassword(oldPassword))
            {
                return user.SetPassword(newPassword);
            }
            return false;
        }

        /// <summary>
        /// Processes a request to update the password question and answer for a membership user.
        /// </summary>
        /// <param name="username">The user to change the password question and answer for. </param>
        /// <param name="password">The password for the specified user.</param>
        /// <param name="newPasswordQuestion">The new password question for the specified user</param>
        /// <param name="newPasswordAnswer">The new password answer for the specified user. </param>
        /// <returns>true if the password question and answer are updated successfully; otherwise, false.</returns>
        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            User user = UserDataSource.LoadForUserName(username);
            if ((user != null) && user.CheckPassword(password))
            {
                user.PasswordQuestion = newPasswordQuestion;
                //password answer is always encoded in SHA1
                user.PasswordAnswer = UserPasswordHelper.EncodePassword(newPasswordAnswer, "SHA1");
                return (user.Save() != SaveResult.Failed);
            }
            return false;
        }

        /// <summary>
        /// Adds a new membership user to the data source.
        /// </summary>
        /// <param name="username">The user name for the new user. </param>
        /// <param name="password">The password for the new user. </param>
        /// <param name="email">The e-mail address for the new user.</param>
        /// <param name="passwordQuestion">The password question for the new user.</param>
        /// <param name="passwordAnswer">The password answer for the new user.</param>
        /// <param name="isApproved">Whether or not the new user is approved to be validated.</param>
        /// <param name="providerUserKey">The unique identifier from the membership data source for the user.</param>
        /// <param name="status">A MembershipCreateStatus enumeration value indicating whether the user was created successfully.</param>
        /// <returns>A MembershipUser object populated with the information for the newly created user.</returns>
        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            //validate the provider user key
            if (!(providerUserKey is int))
            {
                status = MembershipCreateStatus.InvalidProviderUserKey;
                return null;
            }

            //validation is done in UserDataSource.CreateUser
            User user = UserDataSource.CreateUser(username, email, password, passwordQuestion, passwordAnswer, isApproved, 0, out status);
            if (status == MembershipCreateStatus.Success)
            {
                return new MembershipUser(this.Name, username, user.UserId, email, passwordQuestion, null, isApproved, false, user.CreateDate, user.CreateDate, user.CreateDate, user.CreateDate, DateTime.MinValue);
            }

            //something went wrong.
            return null;
        }

        /// <summary>
        /// Removes a user from the membership data source.
        /// </summary>
        /// <param name="username">The name of the user to delete.</param>
        /// <param name="deleteAllRelatedData">This parameter is ignored by the provider.  Related user data is always deleted.</param>
        /// <returns>true if the user was successfully deleted; otherwise, false.</returns>
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            User user = UserDataSource.LoadForUserName(username);
            if (user != null) return user.Delete();
            return false;
        }

        /// <summary>
        /// Indicates whether the membership provider is configured to allow users to reset their passwords.
        /// </summary>
        public override bool EnablePasswordReset
        {
            get { return true; }
        }

        /// <summary>
        /// Indicates whether the membership provider is configured to allow users to retrieve their passwords.
        /// </summary>
        /// <remarks>For 7.0, this should always be false.  Encrypted password feature is not completely functional.</remarks>
        public override bool EnablePasswordRetrieval
        {
            get { return (this.PasswordFormat != MembershipPasswordFormat.Hashed); }
        }

        /// <summary>
        /// Gets a collection of membership users where the e-mail address contains the specified e-mail address to match.
        /// </summary>
        /// <param name="emailToMatch">The e-mail address to search for.</param>
        /// <param name="pageIndex">The index of the page of results to return. pageIndex is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        /// <returns>A MembershipUserCollection collection that contains a page of pageSizeMembershipUser objects beginning at the page specified by pageIndex.</returns>
        /// <remarks>In AbleCommerce 7, this is equivalent to FindUsersByName because email address is always the username.</remarks>
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            return this.FindUsersByName(emailToMatch, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// Gets a collection of membership users where the user name contains the specified user name to match.
        /// </summary>
        /// <param name="usernameToMatch">The user name to search for.</param>
        /// <param name="pageIndex">The index of the page of results to return. pageIndex is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        /// <returns>A MembershipUserCollection collection that contains a page of pageSizeMembershipUser objects beginning at the page specified by pageIndex.</returns>
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            totalRecords = UserDataSource.FindUsersByNameCount(usernameToMatch, UserDataSource.NameSearchField.UserName);
            UserCollection users = UserDataSource.FindUsersByName(usernameToMatch, UserDataSource.NameSearchField.UserName, pageSize, (pageSize * (pageIndex - 1)), "UserName");
            MembershipUserCollection matchingUsers = new MembershipUserCollection();
            foreach (User user in users)
            {
                matchingUsers.Add(new MembershipUser(this.Name, user.UserName, user.UserId, user.Email, user.PasswordQuestion, user.Comment, user.IsApproved, user.IsLockedOut, user.CreateDate, user.LastLoginDate, user.LastActivityDate, user.LastPasswordChangedDate, user.LastLockoutDate));
            }
            return matchingUsers;
        }

        /// <summary>
        /// Gets a collection of all the users in the data source in pages of data.
        /// </summary>
        /// <param name="pageIndex">The index of the page of results to return. pageIndex is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        /// <returns>A MembershipUserCollection collection that contains a page of pageSizeMembershipUser objects beginning at the page specified by pageIndex.</returns>
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection allUsers = new MembershipUserCollection();
            PersistentCollection<User> users = UserDataSource.LoadForStore(pageSize, (pageSize * (pageIndex - 1)), string.Empty);
            foreach (User user in users)
            {
                allUsers.Add(new MembershipUser(this.Name, user.UserName, user.UserId, user.Email, user.PasswordQuestion, user.Comment, user.IsApproved, user.IsLockedOut, user.CreateDate, user.LastLoginDate, user.LastActivityDate, user.LastPasswordChangedDate, user.LastLockoutDate));
            }
            totalRecords = UserDataSource.CountForStore();
            return allUsers;
        }

        /// <summary>
        /// Gets the number of users currently accessing the application.
        /// </summary>
        /// <returns>The number of users currently accessing the application.</returns>
        /// <remarks>This method is not implemented in AbleCommerce 7.</remarks>
        public override int GetNumberOfUsersOnline()
        {
            throw new MembershipProviderException("The method GetNumberOfUsersOnline is not implemented.");
        }

        /// <summary>
        /// Gets the password for the specified user name from the data source.
        /// </summary>
        /// <param name="username">The user to retrieve the password for. </param>
        /// <param name="answer">The password answer for the user. </param>
        /// <returns>The password for the specified user name.</returns>
        /// <remarks>This method is not implemented in AbleCommerce 7.</remarks>
        public override string GetPassword(string username, string answer)
        {
            throw new MembershipProviderException("The method GetPassword is not implemented.");
        }

        /// <summary>
        /// Gets information from the data source for a user. Provides an option to update the last-activity date/time stamp for the user.
        /// </summary>
        /// <param name="username">The name of the user to get information for. </param>
        /// <param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user.</param>
        /// <returns>A MembershipUser object populated with the specified user's information from the data source.</returns>
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            //validate the provider user key
            User user = UserDataSource.LoadForUserName(username);
            if (user == null) return null;
            if (userIsOnline)
            {
                user.LastActivityDate = LocaleHelper.LocalNow;
                user.Save();
            }
            return new MembershipUser(this.Name, user.UserName, user.UserId, user.Email, user.PasswordQuestion, user.Comment, user.IsApproved, user.IsLockedOut, user.CreateDate, user.LastLoginDate, user.LastActivityDate, user.Passwords[0].CreateDate, user.LastLockoutDate);
        }

        /// <summary>
        /// Gets user information from the data source based on the unique identifier for the membership user. Provides an option to update the last-activity date/time stamp for the user.
        /// </summary>
        /// <param name="providerUserKey">The unique identifier for the membership user to get information for.</param>
        /// <param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user.</param>
        /// <returns>A MembershipUser object populated with the specified user's information from the data source.</returns>
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            //validate the provider user key
            if ((providerUserKey == null) || (!(providerUserKey is int))) return null;
            User user = UserDataSource.Load((int)providerUserKey);
            if (user == null) return null;
            return new MembershipUser(this.Name, user.UserName, user.UserId, user.Email, user.PasswordQuestion, user.Comment, user.IsApproved, user.IsLockedOut, user.CreateDate, user.LastLoginDate, user.LastActivityDate, user.Passwords[0].CreateDate, user.LastLockoutDate);
        }

        /// <summary>
        /// Gets the user name associated with the specified e-mail address.
        /// </summary>
        /// <param name="email">The e-mail address to search for.</param>
        /// <returns>The user name associated with the specified e-mail address. If no match is found, return a null reference (Nothing in Visual Basic)</returns>
        public override string GetUserNameByEmail(string email)
        {
            UserCollection users = UserDataSource.LoadForEmail(email);
            if(users.Count == 0) return null;
            return users[0].UserName;
        }

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="name">The friendly name of the provider.</param>
        /// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
        public override void Initialize(string name, NameValueCollection config)
        {
            WebTrace.Write("Initializing AbleCommerceMembershipProvider");
            if (config == null) throw new ArgumentNullException("config");
            if (string.IsNullOrEmpty(name)) name = "AbleCommerceMembershipProvider";
            config.Remove("name");
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "A membership provider for the AbleCommerce application.");
            }

            base.Initialize(name, config);

            //SET THE PASSWORD FORMAT
            _PasswordPolicy = new MerchantPasswordPolicy();
        }

        /// <summary>
        /// Gets the number of invalid password or password-answer attempts allowed before the membership user is locked out.
        /// </summary>
        public override int MaxInvalidPasswordAttempts
        {
            get { return _PasswordPolicy.MaxAttempts; }
        }

        /// <summary>
        /// Gets the minimum number of special characters that must be present in a valid password.
        /// </summary>
        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return ((_PasswordPolicy.RequireNumber || _PasswordPolicy.RequireSymbol || _PasswordPolicy.RequireNonAlpha) ? 1 : 0); }
        }

        /// <summary>
        /// Gets the minimum length required for a password.
        /// </summary>
        public override int MinRequiredPasswordLength
        {
            get { return _PasswordPolicy.MinLength; }
        }

        /// <summary>
        /// Friendly name of the provider.
        /// </summary>
        public override string Name
        {
            get { return "AbleCommerceMembershipProvider"; }
        }
        
        /// <summary>
        /// Gets the number of minutes in which a maximum number of invalid password or password-answer attempts are allowed before the membership user is locked out.
        /// </summary>
        /// <remarks>In AbleCommerce 7, invalid password attempt count does not reset after a specified time period.</remarks>
        public override int PasswordAttemptWindow
        {
            get { return int.MaxValue; }
        }

        /// <summary>
        /// Gets a value indicating the format for storing passwords in the membership data store.
        /// </summary>
        public override MembershipPasswordFormat PasswordFormat
        {
            get
            {
                switch (_PasswordPolicy.PasswordFormat)
                {
                    case "CLEAR":
                        return MembershipPasswordFormat.Clear;
                    case "AES":
                        return MembershipPasswordFormat.Encrypted;
                    default:
                        return MembershipPasswordFormat.Hashed;
                }
            }
        }

        /// <summary>
        /// Gets the regular expression used to evaluate a password.
        /// </summary>
        /// <remarks>Regular expression validation for passwords is not supported in AbleCommerce 7.</remarks>
        public override string PasswordStrengthRegularExpression
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Gets a value indicating whether the membership provider is configured to require the user to answer a password question for password reset and retrieval.
        /// </summary>
        /// <remarks>AbleCommerce 7 never requires a question and answer.</remarks>
        public override bool RequiresQuestionAndAnswer
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the membership provider is configured to require a unique e-mail address for each user name.
        /// </summary>
        /// <remarks>AbleCommerce 7 (as of 7.2) does not requires a unique email.</remarks>
        public override bool RequiresUniqueEmail
        {
            get { return false; }
        }

        /// <summary>
        /// Resets a user's password to a new, automatically generated password.
        /// </summary>
        /// <param name="username">The user to reset the password for. </param>
        /// <param name="answer">The password answer for the specified user.</param>
        /// <returns>The new password for the specified user.</returns>
        /// <remarks>Password question and answer is not implemented in AbleCommerce 7.</remarks>
        /// <remarks>This method is not implemented in AbleCommerce 7.</remarks>
        public override string ResetPassword(string username, string answer)
        {
            throw new MembershipProviderException("The method ResetPassword is not implemented.");
        }

        /// <summary>
        /// Clears a lock so that the membership user can be validated.
        /// </summary>
        /// <param name="username">The membership user whose lock status you want to clear.</param>
        /// <returns>true if the membership user was successfully unlocked; otherwise, false.</returns>
        public override bool UnlockUser(string username)
        {
            User user = UserDataSource.LoadForUserName(username);
            if (user != null)
            {
                user.IsLockedOut = false;
                return (user.Save() != SaveResult.Failed);
            }
            return false;
        }

        /// <summary>
        /// Updates information about a user in the data source.
        /// </summary>
        /// <param name="user">A MembershipUser object that represents the user to update and the updated information for the user.</param>
        public override void UpdateUser(MembershipUser user)
        {
            User acUser = UserDataSource.Load((int)user.ProviderUserKey);
            if (acUser != null)
            {
                acUser.Comment = user.Comment;
                acUser.CreateDate = user.CreationDate;                
                acUser.UserName = user.UserName;
                acUser.Email = user.Email;
                acUser.IsApproved = user.IsApproved;
                acUser.IsLockedOut = user.IsLockedOut;
                //TODO: ISONLINE?
                acUser.LastActivityDate = user.LastActivityDate;
                acUser.LastLockoutDate = user.LastLockoutDate;
                acUser.LastLoginDate = user.LastLoginDate;
                acUser.LastPasswordChangedDate = user.LastPasswordChangedDate;
                acUser.PasswordQuestion = user.PasswordQuestion;
                acUser.Save();
            }
        }

        /// <summary>
        /// Verifies that the specified user name and password exist in the data source.
        /// </summary>
        /// <param name="username">The name of the user to validate.</param>
        /// <param name="password">The password for the specified user.</param>
        /// <returns>true if the specified username and password are valid; otherwise, false.</returns>
        public override bool ValidateUser(string username, string password)
        {
            //WE MUST USE USER.LOGIN METHOD TO TAKE ADVANTAGE OF AUDIT LOGGING
            return User.Login(username, password);
        }
    }
}
