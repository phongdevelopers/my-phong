using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Payments;
using CommerceBuilder.Orders;
using CommerceBuilder.Marketing;
using CommerceBuilder.Users;
using CommerceBuilder.Stores;

namespace CommerceBuilder.Products
{
    /// <summary>
    /// DataSource class for ReviewerProfile objects
    /// </summary>
    [DataObject(true)]
    public partial class ReviewerProfileDataSource
    {
        /// <summary>
        /// Load a ReviewerProfile object for given email address
        /// </summary>
        /// <param name="emailAddress">Email address to load ReviewerProfile object for</param>
        /// <returns>ReviewerProfile object for given email address</returns>
        public static ReviewerProfile LoadForEmail(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress)) return null;
            StringBuilder selectQuery = new StringBuilder();                
            selectQuery.Append("SELECT " + ReviewerProfile.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_ReviewerProfiles");
            selectQuery.Append(" WHERE Email = @emailAddress");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@emailAddress", DbType.String, emailAddress);
            ReviewerProfile reviewerProfile = null;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                if (dr.Read())
                {
                    reviewerProfile = new ReviewerProfile();
                    ReviewerProfile.LoadDataReader(reviewerProfile, dr);                   
                }
                dr.Close();
            }
            return reviewerProfile;
        }

        /// <summary>
        /// Gets the stored email address associated with a reviewer profile.
        /// </summary>
        /// <param name="reviewerProfileId">ID of the profile to get the store email value for.</param>
        /// <returns>The email address stored in the database for the given profile.</returns>
        /// <remarks>This function is used to help determine if the email address of a profile has changed.</remarks>
        public static string GetEmail(int reviewerProfileId)
        {
            if (reviewerProfileId == 0) return string.Empty;
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT Email FROM ac_ReviewerProfiles WHERE ReviewerProfileId = @id");
            database.AddInParameter(selectCommand, "@id", DbType.Int32, reviewerProfileId);
            return database.ExecuteScalar(selectCommand).ToString();
        }
    }
}
