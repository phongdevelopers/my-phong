using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Marketing;
using CommerceBuilder.Orders;
using CommerceBuilder.Utility;
using CommerceBuilder.Products;
using CommerceBuilder.Stores;
using System.Data;
using System.Data.Common;
using System.Web.Security;
using CommerceBuilder.Reporting;

namespace CommerceBuilder.Users
{
    /// <summary>
    /// This class represents a User object in the database.
    /// </summary>
    public partial class User
    {
        /// <summary>
        /// The user's basket
        /// </summary>
        public Basket Basket
        {
            get
            {
                if (this.Baskets.Count == 0)
                {
                    //CREATE THE DEFAULT BASKET ITEM
                    Basket basket = new Basket();
                    basket.UserId = this.UserId;
                    this.Baskets.Add(basket);
                    return basket;
                }
                else return this.Baskets[0];
            }
        }

        /// <summary>
        /// User's primary address
        /// </summary>
        public Address PrimaryAddress
        {
            get
            {
                Address primaryAddress = null;
                int index = this.Addresses.IndexOf(this.PrimaryAddressId);
                if (index < 0)
                {
                    if (this.Addresses.Count > 0)
                    {
                        primaryAddress = this.Addresses[0];
                    }
                    else
                    {
                        primaryAddress = new Address();
                        primaryAddress.UserId = this.UserId;
                        this.Addresses.Add(primaryAddress);
                        primaryAddress.Save();
                    }
                    bool saveDirty = this.IsDirty;
                    this.PrimaryAddressId = primaryAddress.AddressId;
                    UserDataSource.UpdatePrimaryAddressId(this.UserId, this.PrimaryAddressId);
                    this.IsDirty = saveDirty;
                }
                else
                {
                    primaryAddress = this.Addresses[index];
                }
                return primaryAddress;
            }
        }

        /// <summary>
        /// User's primary wish-list
        /// </summary>
        public Wishlist PrimaryWishlist
        {
            get
            {
                Wishlist primaryWishlist = null;
                int index = this.Wishlists.IndexOf(this.PrimaryWishlistId);
                if (index < 0)
                {
                    if (this.Wishlists.Count > 0)
                    {
                        primaryWishlist = this.Wishlists[0];
                    }
                    else
                    {
                        primaryWishlist = new Wishlist();
                        primaryWishlist.UserId = this.UserId;
                        primaryWishlist.Save();
                        this.Wishlists.Add(primaryWishlist);
                    }
                    bool saveDirty = this.IsDirty;
                    this.PrimaryWishlistId = primaryWishlist.WishlistId;
                    UserDataSource.UpdatePrimaryWishlistId(this.UserId, this.PrimaryWishlistId);
                    this.IsDirty = saveDirty;
                }
                else
                {
                    primaryWishlist = this.Wishlists[index];
                }
                return primaryWishlist;
            }
        }

        /// <summary>
        /// Gets the reviewer profile associated with the user.
        /// </summary>
        /// <remarks>This property will be null if there is no associated profile.</remarks>
        public ReviewerProfile ReviewerProfile
        {
            get
            {
                if (_ReviewerProfile == null)
                {
                    _ReviewerProfile = ReviewerProfileDataSource.LoadForEmail(this.Email);
                }
                return _ReviewerProfile;
            }
        }
        private ReviewerProfile _ReviewerProfile;

        /// <summary>
        /// Updates the user password
        /// </summary>
        /// <param name="newPassword">new password</param>
        /// <returns>True if the password was set successfully; false otherwise</returns>
        public bool SetPassword(string newPassword)
        {
            return SetPassword(newPassword, false);
        }

        /// <summary>
        /// Updates the user password
        /// </summary>
        /// <param name="newPassword">new password</param>
        /// <param name="forceExpiration">force expiration</param>
        /// <returns>True if the password was set successfully; false otherwise</returns>
        public bool SetPassword(string newPassword,bool forceExpiration)
        {
            bool isAdmin = this.IsAdmin;
            PasswordPolicy policy;
            if (isAdmin) policy = new MerchantPasswordPolicy();
            else policy = new CustomerPasswordPolicy();
            int historyDays = policy.HistoryDays;
            int historyCount = policy.HistoryCount;
            DateTime lastPasswordDate = LocaleHelper.LocalNow.AddDays(-1 * historyDays);
            UserPasswordCollection passwordCollection = this.Passwords;
            int passwordCount = passwordCollection.Count;
            for (int i = passwordCount - 1; i >= 0; i--)
            {
                UserPassword oldPassword = passwordCollection[i];
                if ((oldPassword.PasswordNumber >= historyCount) && (oldPassword.CreateDate <= lastPasswordDate))
                {
                    passwordCollection[i].Delete();
                    passwordCollection.RemoveAt(i);
                }
                else
                {
                    passwordCollection[i].PasswordNumber++;
                }
            }
            UserPassword userPassword = new UserPassword();
            userPassword.Password = UserPasswordHelper.EncodePassword(newPassword, policy.PasswordFormat);
            userPassword.PasswordFormat = policy.PasswordFormat;
            userPassword.PasswordNumber = 1;
            userPassword.CreateDate = LocaleHelper.LocalNow;
            userPassword.ForceExpiration = forceExpiration;
            passwordCollection.Add(userPassword);
            this.LastPasswordChangedDate = userPassword.CreateDate;
            bool result = (this.Save() != SaveResult.Failed);
            if (isAdmin)
            {
                Logger.Audit(AuditEventType.PasswordChanged, result, string.Empty);
            }
            return result;
        }

        /// <summary>
        /// Updates the password question and answer.
        /// </summary>
        /// <param name="passwordQuestion">The password question</param>
        /// <param name="passwordAnswer">The password answer</param>
        /// <remarks>This method does not save the user record.</remarks>
        public void SetPasswordQuestion(string passwordQuestion, string passwordAnswer)
        {
            this.PasswordQuestion = passwordQuestion;
            string encodedPasswordAnswer = string.Empty;
            if (!string.IsNullOrEmpty(passwordAnswer)) encodedPasswordAnswer = UserPasswordHelper.EncodePassword(passwordAnswer, "SHA1");
            this.PasswordAnswer = encodedPasswordAnswer;
        }

        /// <summary>
        /// Delete this User object from database
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise</returns>
        public virtual bool Delete()
        {
            Database database = Token.Instance.Database;
            using (System.Data.Common.DbCommand deleteCommand = database.GetSqlStringCommand("DELETE FROM ac_Baskets WHERE UserId = @userId"))
            {
                database.AddInParameter(deleteCommand, "@userId", System.Data.DbType.Int32, this.UserId);
                database.ExecuteNonQuery(deleteCommand);
            }
            using (System.Data.Common.DbCommand deleteCommand = database.GetSqlStringCommand("DELETE FROM ac_Wishlists WHERE UserId = @userId"))
            {
                database.AddInParameter(deleteCommand, "@userId", System.Data.DbType.Int32, this.UserId);
                database.ExecuteNonQuery(deleteCommand);
            }
            using (System.Data.Common.DbCommand deleteCommand = database.GetSqlStringCommand("UPDATE ac_Orders SET UserId = NULL WHERE UserId = @userId"))
            {
                database.AddInParameter(deleteCommand, "@userId", System.Data.DbType.Int32, this.UserId);
                database.ExecuteNonQuery(deleteCommand);
            }
            return this.BaseDelete();
        }

        /// <summary>
        /// Save this User object to database
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public virtual SaveResult Save()
        {
            //IF USERNAME IS GUID, THIS IS AN ANONYMOUS USER
            this.IsAnonymous = (CommerceBuilder.Utility.AlwaysConvert.ToGuid(this.UserName) != Guid.Empty);
            //CHECK WHETHER WE ARE SAVING AN EXISTING USER
            if (this.UserId > 0)
            {
                //GET THE EMAIL VALUE STORED IN DATABASE
                string existingEmail = UserDataSource.GetEmail(this.UserId).ToLowerInvariant();
                string newEmail = this.Email.ToLowerInvariant();
                //SEE WHETHER THE NEW AND EXISTING EMAILS MATCH
                if (newEmail != existingEmail)
                {
                    //EMAILS ARE DIFFERENT, IS THE NEW EMAIL VALID?
                    bool newAddressValid = ValidationHelper.IsValidEmail(newEmail);
                    //GET ALL EMAIL LISTS ASSOCIATED WITH EXISTING ADDRESS
                    EmailListCollection emailLists = EmailListDataSource.LoadForEmail(existingEmail);
                    //LOOP THE LISTS
                    foreach (EmailList list in emailLists)
                    {
                        //REMOVE EXISTING ADDRESS FROM LISTS
                        EmailListUser elu = list.RemoveMember(existingEmail);
                        //IF NEW ADDRESS WAS VALID RE-ADD TO SAME LIST
                        if (newAddressValid && (elu != null))
                        {
                            list.AddMember(newEmail, elu.SignupDate, elu.SignupIP);
                        }
                    }

                    //if the user is registered and the new email address is also valid
                    if (newAddressValid && !this.IsAnonymous)
                    {
                        OrderCollection orders = OrderDataSource.LoadForUser(this.UserId);
                        foreach (Order order in orders)
                        {
                            if (order.BillToEmail.ToLowerInvariant() == existingEmail)
                            {
                                order.BillToEmail = newEmail;
                                order.Save();
                            }
                        }
                    }
                }

                // ENSURE THE AFFILIATE ASSOCIATION IS VALID
                this.ValidateAffiliate();
            }
            return this.BaseSave(); ;
        }

        /// <summary>
        /// Determines whether a user is in the given role.
        /// </summary>
        /// <param name="name">The name of the role to check.</param>
        /// <returns>True if the user is in the given role; false otherwise.</returns>
        public bool IsInRole(string name)
        {
            return RoleDataSource.IsUserInRole(this.UserId, name);
        }

        /// <summary>
        /// Determines whether a user is in the given role.
        /// </summary>
        /// <param name="roleId">The unique ID of the role to check.</param>
        /// <returns>True if the user is in the given role; false otherwise.</returns>
        public bool IsInRole(int roleId)
        {
            return RoleDataSource.IsUserInRole(this.UserId, roleId);
        }

        /// <summary>
        /// Determines whether a user is in any of the given roles.
        /// </summary>
        /// <param name="roleIdCollection">A collection of unique role IDs to check.</param>
        /// <returns>True if the user is in any of the given roles; false otherwise.</returns>
        public bool IsInRole(ICollection<int> roleIdCollection)
        {
            foreach (int roleId in roleIdCollection)
            {
                if (this.IsInRole(roleId)) return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether a user is in any of the given roles.
        /// </summary>
        /// <param name="roleCollection">A collection of role names to check.</param>
        /// <returns>True if the user is in any of the given roles; false otherwise.</returns>
        public bool IsInRole(ICollection<string> roleCollection)
        {
            foreach (string roleName in roleCollection)
            {
                if (this.IsInRole(roleName)) return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether a user is in the given group.
        /// </summary>
        /// <param name="groupId">The unique ID of the group to check.</param>
        /// <returns>True if the user is in the given group; false otherwise.</returns>
        public bool IsInGroup(int groupId)
        {
            foreach (UserGroup userGroup in this.UserGroups)
            {
                if (userGroup.GroupId == groupId) return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether a user is in any of the given groups.
        /// </summary>
        /// <param name="groupIdCollection">A collection of unique group IDs to check.</param>
        /// <returns>True if the user is in any of the given groups; false otherwise.</returns>
        public bool IsInGroup(ICollection<int> groupIdCollection)
        {
            foreach (int groupId in groupIdCollection)
            {
                if (this.IsInGroup(groupId)) return true;
            }
            return false;
        }

        /// <summary>
        /// Converts the existing user record from an anonymous user to a registered user.
        /// </summary>
        /// <param name="username">The username to associate with the user account.</param>
        /// <param name="email">The email address for the user.</param>
        /// <param name="password">The password for the user.</param>
        /// <param name="securityQuestion">Question for use in password recovery.</param>
        /// <param name="securityAnswer">Answer to the security question.</param>
        /// <param name="isApproved">Flag indicating whether the user is approved.</param>
        /// <returns>The result of the registration.</returns>
        public MembershipCreateStatus Register(string username, string email, string password, string securityQuestion, string securityAnswer, bool isApproved)
        {
            //get database reference
            Database database = Token.Instance.Database;

            //validate email
            if (!ValidationHelper.IsValidEmail(email))
            {
                //email address not valid
                return MembershipCreateStatus.InvalidEmail;
            }
            //check for dupliate email
            //we won't prevent duplicate emails anymore bug 7567
            /*
            int userId = UserDataSource.GetUserIdByEmail(email);
            if (userId != 0) return MembershipCreateStatus.DuplicateEmail;
             */

            //validate user name
            if (string.IsNullOrEmpty(username))
            {
                return MembershipCreateStatus.InvalidUserName;
            }
            else 
            {
                //CHECK FOR DUPLICATE USER NAME
                int userId = UserDataSource.GetUserIdByUserName(username);
                if (userId != 0) return MembershipCreateStatus.DuplicateUserName;
            }

            //PASSED CHECKS, UPDATE USER RECORD
            this.UserName = username;
            this.Email = email;
            this.IsApproved = isApproved;
            this.CreateDate = LocaleHelper.LocalNow;
            this.SetPasswordQuestion(securityQuestion, securityAnswer);
            if (this.Save() == SaveResult.Failed) return MembershipCreateStatus.ProviderError;
            this.SetPassword(password);

            //USER CREATED SUCCESSFULLY
            return MembershipCreateStatus.Success;
        }

        /// <summary>
        /// Generates a customer password request for this user
        /// </summary>
        public void GeneratePasswordRequest()
        {
            this.Comment = StringHelper.RandomString(12);
            this.Save();
            string sep = Token.Instance.Store.StoreUrl.EndsWith("/") ? "" : "/";
            string resetPasswordLink = Token.Instance.Store.StoreUrl + sep + "PasswordHelp.aspx?Key={0}&Check={1}";
            resetPasswordLink = string.Format(resetPasswordLink, this.UserId, this.Comment);
            if (this.IsAdmin) Logger.Audit(AuditEventType.PasswordRequest, true, string.Empty);
            StoreEventEngine.CustomerPasswordRequest(this, resetPasswordLink);
        }

        /// <summary>
        /// Migrates data such as profile settings and basket contents from one user to another.
        /// </summary>
        /// <param name="oldUser">The user that provides the source data.</param>
        /// <param name="newUser">The user to receive the data.</param>
        public static void Migrate(User oldUser, User newUser)
        {
            User.Migrate(oldUser, newUser, false, false);
        }

        /// <summary>
        /// Migrates data such as profile settings and basket contents from one user to another.
        /// </summary>
        /// <param name="oldUser">The user that provides the source data.</param>
        /// <param name="newUser">The user to receive the data.</param>
        /// <param name="includeOrderData">If true, order history and address book are migrated.</param>
        /// <param name="isNewUserAccount">If true, newUser represents an account just created.</param>
        public static void Migrate(User oldUser, User newUser, bool includeOrderData, bool isNewUserAccount)
        {
            //FAIL MIGRATION IF REQUIRED PARAMETERS MISSING
            if (oldUser == null) throw new ArgumentNullException("oldUser");
            if (newUser == null) throw new ArgumentNullException("newUser");

            //ONLY MIGRATE IF USERID DOES NOT MATCH
            if (oldUser.UserId != newUser.UserId)
            {
                //MIGRATE AFFILIATE SETTINGS
                if (oldUser.Affiliate != null && oldUser.AffiliateId != newUser.AffiliateId)
                {
                    // A VALID AFFILIATE WAS SET ON THE OLD USER AND IS NOT THE ONE ASSOCIATED WITH NEW USER
                    // SHOULD WE UPDATE THE USER?
                    StoreSettingCollection settings = Store.GetCachedSettings();
                    if (isNewUserAccount 
                        || settings.AffiliateReferralRule == ReferralRule.NewSignupsOrExistingUsersOverrideAffiliate
                        || (settings.AffiliateReferralRule == ReferralRule.NewSignupsOrExistingUsersNoOverride && newUser.AffiliateId == 0))
                    {
                        // EITHER A NEW SIGNUP
                        // OR THE RULE IS TO ALWAYS OVERRIDE
                        // OR AN EXISTING USER WITH NO AFFILIATE SET WITH EXISTING USERS NO OVERRIDE OPTION
                        // AFFILIATE SHOULD BE UPDATED FOR THE TARGET USER
                        newUser.AffiliateId = oldUser.AffiliateId;
                        newUser.AffiliateReferralDate = oldUser.AffiliateReferralDate;
                    }

                    // UPDATE USERS WITH NEW AFFILIATE ASSOCIATIONS
                    newUser.Save();
                    oldUser.AffiliateId = 0;
                    oldUser.AffiliateReferralDate = DateTime.MinValue;
                    oldUser.Save();
                }

                //TRANSFER BASKET IF NEEDED
                Basket.Transfer(oldUser.UserId, newUser.UserId);
                Wishlist.Transfer(oldUser.UserId, newUser.UserId);

                // TRANSFER PAGE VIEW HISTORY
                PageViewDataSource.UpdateUser(oldUser.UserId, newUser.UserId);

                //SHOULD WE TRANSFER ORDER DATA?
                if (includeOrderData)
                {
                    //REASSIGN ORDERS AND ADDRESSES TO NEW USER
                    OrderDataSource.UpdateUser(oldUser.UserId, newUser.UserId);
                    AddressDataSource.UpdateUser(oldUser.UserId, newUser.UserId);
                }
                else if (oldUser.IsAnonymous)
                {
                    //BUG 7740, ERASE ANY ADDRESS INFO ASSOCIATED WITH ANON ACCOUNT
                    oldUser.Addresses.DeleteAll();
                    oldUser.PrimaryAddressId = 0;
                    oldUser.Save();
                }
            }
        }

        /// <summary>
        /// Checks a password for the user.
        /// </summary>
        /// <param name="password">The password to test.</param>
        /// <returns>True if the password is correct for the user.</returns>
        public bool CheckPassword(string password)
        {
            if (this.Passwords.Count == 0) return false;
            UserPassword storedPassword = this.Passwords[0];
            return storedPassword.VerifyPassword(password);
        }

        private static bool AuditLogin_InvalidUsername(string username)
        {
            Logger.Audit(AuditEventType.Login, false, "Invalid username: " + username);
            return false;
        }

        private static bool AuditLogin_Unapproved(User user)
        {
            if (user.IsAdmin) Logger.Audit(AuditEventType.Login, false, "User not approved.", user.UserId);
            return false;
        }

        private static bool AuditLogin_NoPassword(User user)
        {
            Logger.Error("No password set for user: " + user.UserName);
            if (user.IsAdmin) Logger.Audit(AuditEventType.Login, false, "No password set for user.", user.UserId);
            return false;
        }

        private static bool AuditLogin_AccountLocked(User user)
        {
            if (user.IsAdmin) Logger.Audit(AuditEventType.Login, false, "Account locked", user.UserId);
            return false;
        }

        private static bool AuditLogin_InvalidPassword(User user)
        {
            if (user.IsAdmin) Logger.Audit(AuditEventType.Login, false, "Invalid password", user.UserId);
            return false;
        }

        private static bool AuditLogin_Success(User user)
        {
            if (user.IsAdmin) Logger.Audit(AuditEventType.Login, true, string.Empty, user.UserId);
            return true;
        }

        /// <summary>
        /// Validates the given username and password
        /// </summary>
        /// <param name="username">Name of user attempting login</param>
        /// <param name="password">Password provided by user</param>
        /// <returns>True if the login succeeds; false otherwise.</returns>
        public static bool Login(string username, string password)
        {
            User user = UserDataSource.LoadForUserName(username);
            if (user == null) return AuditLogin_InvalidUsername(username);
            if (!user.IsApproved) return AuditLogin_Unapproved(user);
            UserPasswordCollection passwordCollection = user.Passwords;
            if (passwordCollection.Count == 0) return AuditLogin_NoPassword(user);
            UserPassword storedPassword = passwordCollection[0];
            bool isPasswordValid = storedPassword.VerifyPassword(password);
            PasswordPolicy policy;
            if (user.IsAdmin) policy = new MerchantPasswordPolicy();
            else policy = new CustomerPasswordPolicy();
            if (user.IsLockedOut)
            {
                if (user.LastLockoutDate >= LocaleHelper.LocalNow.AddMinutes(-1 * policy.LockoutPeriod))
                {
                    //STILL LOCKED OUT
                    // BUG # 6688 (DONT RESET THE LOCKOUT TIME IF ACCOUNT ALREADY LOCKED)
                    // ALSO IGNORE THE LOGIN ATTEMPTS
                    //if (!isPasswordValid)
                    //{
                    //    user.LastLockoutDate = LocaleHelper.LocalNow;
                    //    user.FailedPasswordAttemptCount += 1;
                    //    user.Save();
                    //}
                    return AuditLogin_AccountLocked(user);
                }
                user.IsLockedOut = false;
            }
            if (isPasswordValid)
            {
                user.FailedPasswordAttemptCount = 0;
                user.LastLoginDate = LocaleHelper.LocalNow;
                user.Save();
                return AuditLogin_Success(user);
            }
            else
            {
                user.FailedPasswordAttemptCount += 1;
                if (user.FailedPasswordAttemptCount >= policy.MaxAttempts)
                {
                    user.IsLockedOut = true;
                    // RESET THE FAILED ATTEMPTS COUNT
                    user.FailedPasswordAttemptCount = 0;
                    user.LastLockoutDate = LocaleHelper.LocalNow;
                }
                user.Save();
                return AuditLogin_InvalidPassword(user);
            }
        }

        /// <summary>
        /// Logs out the current user
        /// </summary>
        public static void Logout()
        {
            User user = Token.Instance.User;
            if (user != null)
            {
                if (user.IsAdmin) Logger.Audit(AuditEventType.Logout, true, string.Empty, Token.Instance.UserId);
            }
            FormsAuthentication.SignOut();
        }

        /// <summary>
        /// Is this user a System Admin?
        /// </summary>
        public bool IsSystemAdmin
        {
            get
            {
                return this.IsInRole(Role.SystemAdminRoles);
            }
        }

        /// <summary>
        /// Is this user a Security Admin?
        /// </summary>
        public bool IsSecurityAdmin
        {
            get
            {
                return this.IsInRole(Role.AdminRoles);
            }
        }

        /// <summary>
        /// Is this user an Admin user?
        /// </summary>
        public bool IsAdmin
        {
            get
            {
                return this.IsInRole(Role.AllAdminRoles);
            }
        }

        /// <summary>
        /// Id of the user's selected currency
        /// </summary>
        public int UserCurrencyId
        {
            get { return this.Settings.UserCurrencyId; }
            set
            {
                _UserCurrency = null;
                this.Settings.UserCurrencyId = value;
            }
        }

        private Currency _UserCurrency;
        /// <summary>
        /// User's selected currency
        /// </summary>
        public Currency UserCurrency
        {
            get
            {
                if (_UserCurrency == null)
                {
                    _UserCurrency = CurrencyDataSource.Load(this.UserCurrencyId);
                    if (_UserCurrency == null) _UserCurrency = Token.Instance.Store.BaseCurrency;
                }
                return _UserCurrency;
            }
        }

        /// <summary>
        /// Checks the associated affiliate and resets the association
        /// if the referral is expired
        /// </summary>
        internal void ValidateAffiliate()
        {
            // ValidateAffiliate is called by Basket.Checkout and User.Save
            // We do not need to validate if affiliate is not set
            if (this.AffiliateId == 0) return;

            bool affiliateValid = false;
            Affiliate affiliate = this.Affiliate;
            if (affiliate != null)
            {
                int orderCount;
                DateTime referralExpiration;
                switch (affiliate.ReferralPeriod)
                { 
                    case AffiliateReferralPeriod.FirstOrder:
                        // IF AFFILIATE ORDER HAS BEEN PLACED BY THIS CUSTOMER SINCE REFERRAL THE ASSOCIATION IS INVALID
                        orderCount = AffiliateDataSource.GetOrderCountForUser(affiliate.AffiliateId, this.UserId, this.AffiliateReferralDate);
                        affiliateValid = (orderCount == 0);
                        break;

                    case AffiliateReferralPeriod.NumberOfDays:
                        // AFFILIATE ASSOCIATION IS ONLY VALID WITHIN A SET NUMBER OF DAYS
                        referralExpiration = this.AffiliateReferralDate.AddDays(affiliate.ReferralDays);
                        affiliateValid = (referralExpiration > LocaleHelper.LocalNow);
                        break;

                    case AffiliateReferralPeriod.FirstOrderWithinNumberOfDays:
                        // AFFILIATE IS VALID IF WITHIN THE TIME PERIOD AND FIRST ORDER HAS NOT BEEN PLACED
                        referralExpiration = this.AffiliateReferralDate.AddDays(affiliate.ReferralDays);
                        orderCount = AffiliateDataSource.GetOrderCountForUser(affiliate.AffiliateId, this.UserId, this.AffiliateReferralDate);
                        affiliateValid = (referralExpiration > LocaleHelper.LocalNow && orderCount == 0);
                        break;

                    case AffiliateReferralPeriod.Persistent:
                        // AFFILIATE IS ALWAYS VALID
                        affiliateValid = true;
                        break;
                }
            }

            // IF AFFILIATE IS INVALID, MAKE SURE IT IS NOT SET FOR THIS USER
            if (!affiliateValid)
            {
                Database database = Token.Instance.Database;
                DbCommand command = database.GetSqlStringCommand("UPDATE ac_Users SET AffiliateId = NULL, AffiliateReferralDate = NULL WHERE UserId = @userId");
                database.AddInParameter(command, "@userId", System.Data.DbType.Int32, this.UserId);
                database.ExecuteNonQuery(command);
                _AffiliateId = 0;
                _AffiliateReferralDate = DateTime.MinValue;
                _Affiliate = null;
            }
        }

        /// <summary>
        /// Gets the best email address for this user
        /// </summary>
        /// <returns>The best known email address for this user</returns>
        /// <remarks>Email addresses may be found in a variety of locations, this function
        /// pulls the best email address in order of preference.  Ideally, all users should have the
        /// best email address stored in the "Email" property but this is not always the case especially
        /// with legacy data.</remarks>
        public string GetBestEmailAddress()
        {
            if (ValidationHelper.IsValidEmail(this.Email)) return this.Email;
            return this.UserName;
        }
    }
}