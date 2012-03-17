using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Marketing
{
    /// <summary>
    /// DataSource class for EmailListSignup objects
    /// </summary>
    [DataObject(true)]
    public partial class EmailListSignupDataSource
    {
        /// <summary>
        /// Gets the Id of the EmailListSignup object that represents the given email and email list Id
        /// </summary>
        /// <param name="emailListId">Id of the email list for the EmailListSignup object in question</param>
        /// <param name="email">The email field for the EmailListSignup object in question</param>
        /// <returns>Id of the EmailListSignup</returns>
        public static int GetEmailListSignupId(int emailListId, string email)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT EmailListSignupId FROM ac_EmailListSignups WHERE EmailListId = @emailListId AND Email = @email");
            database.AddInParameter(selectCommand, "@emailListId", System.Data.DbType.Int32, emailListId);
            database.AddInParameter(selectCommand, "@email", System.Data.DbType.String, email);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads an EmailListSignup object for given email list Id and the email address
        /// </summary>
        /// <param name="emailListId">Id of the email list for the EmailListSignup object to load</param>
        /// <param name="email">The email field for the EmailListSignup object to load</param>
        /// <returns>EmailListSignup object loaded</returns>
        public static EmailListSignup Load(int emailListId, string email)
        {
            int signupId = GetEmailListSignupId(emailListId, email);
            return EmailListSignupDataSource.Load(signupId);
        }
    }
}
