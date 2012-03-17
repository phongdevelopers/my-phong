using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using CommerceBuilder.Common;
using CommerceBuilder.Stores;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Users
{
    /// <summary>
    /// Class that represents password policy
    /// </summary>
    public abstract class PasswordPolicy
    {
        /// <summary>
        /// Store settings
        /// </summary>
        protected StoreSettingCollection StoreSettings;

        /// <summary>
        /// Constructor
        /// </summary>
        protected PasswordPolicy()
        {
            StoreSettings = Token.Instance.Store.Settings;
        }

        /// <summary>
        /// Minimum password length
        /// </summary>
        public abstract int MinLength { get; set; }
        /// <summary>
        /// Whether upper case is required?
        /// </summary>
        public abstract bool RequireUpper { get; set;}
        /// <summary>
        /// Whether lower case is required?
        /// </summary>
        public abstract bool RequireLower { get; set;}
        /// <summary>
        /// Whether a number is required?
        /// </summary>
        public abstract bool RequireNumber { get; set;}
        /// <summary>
        /// Whether a special character or symbol is required?
        /// </summary>
        public abstract bool RequireSymbol { get; set;}
        /// <summary>
        /// Whether a non alphabet character is required?
        /// </summary>
        public abstract bool RequireNonAlpha { get; set;}
        /// <summary>
        /// The maximum age of a password
        /// </summary>
        public abstract int MaxAge { get; set;}
        /// <summary>
        /// Number of days password is kept in history
        /// </summary>
        public abstract int HistoryDays { get; set; }
        /// <summary>
        /// Number of passwords to keep in history
        /// </summary>
        public abstract int HistoryCount { get; set;}
        /// <summary>
        /// Maximum login attempts before the account is locked
        /// </summary>
        public abstract int MaxAttempts { get; set; }
        /// <summary>
        /// Lockup period when maximum login attempts fail
        /// </summary>
        public abstract int LockoutPeriod { get; set;}
        /// <summary>
        /// Number of months that if the account remains unused, it will be deactivated
        /// </summary>
        public abstract int InactivePeriod { get; set; }
        /// <summary>
        /// Whether to use image captcha verification at the time of login?
        /// </summary>
        public abstract bool ImageCaptcha { get; set; }

        /// <summary>
        /// Password format
        /// </summary>
        public string PasswordFormat
        {
            get { return this.StoreSettings.PasswordFormat; }
            set { this.StoreSettings.PasswordFormat = value; }
        }

        /// <summary>
        /// Tests a user new password to see if it conforms with the rules established by the policy.
        /// </summary>
        /// <param name="password">password to test</param>
        /// <returns>PasswordTestResult flag indicating the test result</returns>
        public PasswordTestResult TestPasswordWithFeedback(string password)
        {
            return TestPasswordWithFeedback(null, password);
        }

        /// <summary>
        /// Tests a user new password to see if it conforms with the rules established by the policy.
        /// </summary>
        /// <param name="user">user</param>
        /// <param name="password">password to test</param>
        /// <returns>PasswordTestResult flag indicating the test result</returns>
        public PasswordTestResult TestPasswordWithFeedback(User user, string password)
        {
            PasswordTestResult result = PasswordTestResult.Success;
            if (password.Length < this.MinLength) result = UpdatePasswordTestResult(result, PasswordTestResult.PasswordTooShort);
            if ((this.RequireUpper) && (!Regex.IsMatch(password, "[A-Z]"))) result = UpdatePasswordTestResult(result, PasswordTestResult.RequireUpper);
            if ((this.RequireLower) && (!Regex.IsMatch(password, "[a-z]"))) result = UpdatePasswordTestResult(result, PasswordTestResult.RequireLower);
            if ((this.RequireNumber) && (!Regex.IsMatch(password, "[0-9]"))) result = UpdatePasswordTestResult(result, PasswordTestResult.RequireNumber);
            if ((this.RequireSymbol) && (!Regex.IsMatch(password, "[^0-9a-zA-Z]"))) result = UpdatePasswordTestResult(result, PasswordTestResult.RequireSymbol);
            if ((this.RequireNonAlpha) && (!Regex.IsMatch(password, "[^a-zA-Z]"))) result = UpdatePasswordTestResult(result, PasswordTestResult.RequireNonAlpha);
            if (user != null)
            {
                //VERIFY THE GIVEN PASSWORD DOES NOT EXIST IN HISTORY
                DateTime historyExpirationDate = LocaleHelper.LocalNow.AddDays(-1 * this.HistoryDays);
                foreach (UserPassword oldPassword in user.Passwords)
                {
                    if ((oldPassword.PasswordNumber - 1) <= this.HistoryCount)
                    {
                        if ((this.HistoryDays == 0) || (historyExpirationDate <= oldPassword.CreateDate))
                        {
                            if (oldPassword.VerifyPassword(password)) result = UpdatePasswordTestResult(result, PasswordTestResult.PasswordHistoryLimitation);
                        }
                    }
                }
            }
            return result;
        }

        private PasswordTestResult UpdatePasswordTestResult(PasswordTestResult oldResult, PasswordTestResult newResult)
        {
            if ((oldResult & PasswordTestResult.Success) == PasswordTestResult.Success) return newResult;
            else return (oldResult | newResult);
        }

        /// <summary>
        /// Tests a password to see if it conforms with the rules established by the policy.
        /// </summary>
        /// <param name="user">The user to set the password for.</param>
        /// <param name="password">The password to validate against the policy.</param>
        /// <returns>True if the password meets the policy requirements for the user.</returns>
        public bool TestPassword(User user, string password)
        {
            return ((TestPasswordWithFeedback(user, password) & PasswordTestResult.Success) == PasswordTestResult.Success);
            //if (password.Length < this.MinLength) return false;
            //if ((this.RequireUpper) && (!Regex.IsMatch(password, "[A-Z]"))) return false;
            //if ((this.RequireLower) && (!Regex.IsMatch(password, "[a-z]"))) return false;
            //if ((this.RequireNumber) && (!Regex.IsMatch(password, "[0-9]"))) return false;
            //if ((this.RequireSymbol) && (!Regex.IsMatch(password, "[^0-9a-zA-Z]"))) return false;
            //if ((this.RequireNonAlpha) && (!Regex.IsMatch(password, "[^a-zA-Z]"))) return false;
            //if (user != null)
            //{
            //    //VERIFY THE GIVEN PASSWORD DOES NOT EXIST IN HISTORY
            //    DateTime historyExpirationDate = LocaleHelper.LocalNow.AddDays(-1 * this.HistoryDays);
            //    foreach (UserPassword oldPassword in user.Passwords)
            //    {
            //        if ((oldPassword.PasswordNumber - 1) <= this.HistoryCount)
            //        {
            //            if ((this.HistoryDays == 0) || (historyExpirationDate <= oldPassword.CreateDate))
            //            {
            //                if (oldPassword.VerifyPassword(password)) return false;
            //            }
            //        }
            //    }
            //}
            //return true;
        }

        /// <summary>
        /// Is password expired for the given user?
        /// </summary>
        /// <param name="user">The user for whom to check the password expiry</param>
        /// <returns><b>true</b> if password is expired, <b>false</b> otherwise</returns>
        public bool IsPasswordExpired(User user)
        {
            //IF NO PASSWORD EXISTS, THEN CONSIDER IT "EXPIRED"
            if (user.Passwords.Count == 0) return true;

            // IF FORCE EXPIRATION IS FLAGGED, THE PASSWORD IS EXPIRED
            if (user.Passwords[0].ForceExpiration) return true;

            // IF THERE IS NO PASSWORD EXPIRATION IN THE POLICY, THE PASSWORD IS NOT EXIPRED
            if (this.MaxAge == 0) return false;

            // DETERMINE IF PASSWORD IS EXPIRED BASED ON POLICY AND CREATE DATE
            DateTime expirationDate = user.Passwords[0].CreateDate.AddDays(this.MaxAge);
            return (expirationDate <= LocaleHelper.LocalNow);
        }

        /// <summary>
        /// Saves any changes made to the password policy.
        /// </summary>
        /// <returns>True if the save was successful; false otherwise.</returns>
        public abstract bool Save();
    }
}