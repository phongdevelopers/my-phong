using CommerceBuilder.Users;
using System;
using CommerceBuilder.Utility;
using CommerceBuilder.Stores;
using CommerceBuilder.Common;

namespace CommerceBuilder.Products
{
    /// <summary>
    /// This class represents a ProductReview object in the database
    /// </summary>
    public partial class ProductReview
    {
        /// <summary>
        /// Saves this ProductReview object
        /// </summary>
        /// <returns></returns>
        public SaveResult Save()
        {
            return this.Save(null, false, false);
        }

        /// <summary>
        /// Saves this ProductReview object for the given user
        /// </summary>
        /// <param name="user">The user for which to save this ProductReview</param>
        /// <param name="checkEmail">Whether to validate the user's email address or not?</param>
        /// <param name="checkApproval">Should the review require approval?</param>
        /// <returns></returns>
        public SaveResult Save(User user, bool checkEmail, bool checkApproval)
        {
            bool sendMail = false;
            if (user != null)
            {
                bool flagged = false;
                if (checkEmail)
                {
                    if (ProductReviewHelper.EmailVerificationRequired(this.ReviewerProfile, user))
                    {
                        this.ReviewerProfile.EmailVerificationCode = Guid.NewGuid();
                        this.ReviewerProfile.Save();
                        flagged = true;
                        sendMail = (this.ProductReviewId == 0);
                    }
                }
                if (checkApproval)
                {
                    flagged = flagged || ProductReviewHelper.ApprovalRequired(user);
                }
                this.IsApproved = !flagged;
            }
            SaveResult result = this.BaseSave();
            if (sendMail) this.ReviewerProfile.SendVerificationEmail(this);
            return result;
        }
    }
}
