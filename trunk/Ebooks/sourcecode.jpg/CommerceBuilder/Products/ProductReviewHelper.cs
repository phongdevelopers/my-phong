using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Stores;
using CommerceBuilder.Users;

namespace CommerceBuilder.Products
{
    /// <summary>
    /// Helper class for product reviews
    /// </summary>
    public class ProductReviewHelper
    {
        /// <summary>
        /// Finds out whether review approval is required for the given user
        /// </summary>
        /// <param name="user">The user to check the approval for</param>
        /// <returns>true if approval is required, false otherwise</returns>
        public static bool ApprovalRequired(User user)
        {
            StoreSettingCollection settings = Token.Instance.Store.Settings;
            switch (settings.ProductReviewApproval)
            {
                case UserAuthFilter.None: return false;
                case UserAuthFilter.Anonymous: return ((user == null) || user.IsAnonymous);
                case UserAuthFilter.Registered: return ((user == null) || !user.IsAnonymous);
                default: return true;
            }
        }

        /// <summary>
        /// Finds out whether image verification is required for the given user
        /// </summary>
        /// <param name="user">The user to check for</param>
        /// <returns>true if image verification is required, false otherwise</returns>
        public static bool ImageVerificationRequired(User user)
        {
            StoreSettingCollection settings = Token.Instance.Store.Settings;
            switch (settings.ProductReviewImageVerification)
            {
                case UserAuthFilter.None: return false;
                case UserAuthFilter.Anonymous: return ((user == null) || user.IsAnonymous);
                case UserAuthFilter.Registered: return ((user == null) || !user.IsAnonymous);
                default: return true;
            }
        }

        /// <summary>
        /// Finds out if email verification is required for given user
        /// </summary>
        /// <param name="profile">The reviewer profile to check for</param>
        /// <param name="user">The user to check for</param>
        /// <returns>true if email verification is required, false otherwise</returns>
        public static bool EmailVerificationRequired(ReviewerProfile profile, User user)
        {
            if (profile.EmailVerified) return false;
            StoreSettingCollection settings = Token.Instance.Store.Settings;
            switch (settings.ProductReviewEmailVerification)
            {
                case UserAuthFilter.None: return false;
                case UserAuthFilter.Anonymous: return user.IsAnonymous;
                case UserAuthFilter.Registered: return !user.IsAnonymous;
                default: return true;
            }
        }

    }
}
