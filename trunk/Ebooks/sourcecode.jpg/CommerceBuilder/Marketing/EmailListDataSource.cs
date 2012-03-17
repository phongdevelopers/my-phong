using System.Data;
using System.Data.Common;
using CommerceBuilder.Data;
using System.ComponentModel;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Marketing
{
    [DataObject(true)]
    public partial class EmailListDataSource
    {
        /// <summary>
        /// Determines whether the given user is a member of the given list.
        /// </summary>
        /// <param name="emailListId">The list to check</param>
        /// <param name="email">The email address to check for list membership</param>
        /// <returns>True if the user is a member of the list; false otherwise.</returns>
        public static bool IsMember(int emailListId, string email)
        {
            string loweredEmail = email.ToLowerInvariant();
            EmailListUser elu = EmailListUserDataSource.Load(emailListId, loweredEmail);
            return (elu != null);
        }

        /// <summary>
        /// Removes the member from the list 
        /// </summary>
        /// <param name="emailListId">The list to remove the member from</param>
        /// <param name="email">The email address to remove from list membership</param>
        public static EmailListUser RemoveMember(int emailListId, string email)
        {
            string loweredEmail = email.ToLowerInvariant();
            EmailListUser elu = EmailListUserDataSource.Load(emailListId, loweredEmail);
            if (elu != null) elu.Delete();
            return elu;
        }

        /// <summary>
        /// Gets the number of email lists associated with the email address
        /// </summary>
        /// <param name="email">The email address to count lists for</param>
        /// <returns>The number of email lists associated with the email address</returns>
        public static int CountForEmail(string email)
        {
            string loweredEmail = email.ToLowerInvariant();
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT COUNT(DISINCT EmailListId) AS TotalRecords FROM ac_EmailListUsers WHERE Email = @email");
            database.AddInParameter(selectCommand, "@email", System.Data.DbType.String, email);
            return AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Gets a collection of email lists associated with the email address
        /// </summary>
        /// <param name="email">The email address to load lists for</param>
        /// <returns>A collection of email lists associated with the email address</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static EmailListCollection LoadForEmail(string email)
        {
            return LoadForEmail(email, 0, 0, string.Empty);
        }

        /// <summary>
        /// Gets a collection of email lists associated with the email address
        /// </summary>
        /// <param name="email">The email address to load lists for</param>
        /// <param name="sortExpression">Sort expression for the result set</param>
        /// <returns>A collection of email lists associated with the email address</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static EmailListCollection LoadForEmail(string email, string sortExpression)
        {
            return LoadForEmail(email, 0, 0, sortExpression);
        }

        /// <summary>
        /// Gets a collection of email lists associated with the email address
        /// </summary>
        /// <param name="email">The email address to load lists for</param>
        /// <param name="maximumRows">The maximum number of results</param>
        /// <param name="startRowIndex">The starting row of the result set</param>
        /// <returns>A collection of email lists associated with the email address</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static EmailListCollection LoadForEmail(string email, int maximumRows, int startRowIndex)
        {
            return LoadForEmail(email, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Gets a collection of email lists associated with the email address
        /// </summary>
        /// <param name="email">The email address to load lists for</param>
        /// <param name="maximumRows">The maximum number of results</param>
        /// <param name="startRowIndex">The starting row of the result set</param>
        /// <param name="sortExpression">Sort expression for the result set</param>
        /// <returns>A collection of email lists associated with the email address</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static EmailListCollection LoadForEmail(string email, int maximumRows, int startRowIndex, string sortExpression)
        {
            string loweredEmail = email.ToLowerInvariant();
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT DISTINCT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + EmailList.GetColumnNames("EL"));
            selectQuery.Append(" FROM ac_EmailListUsers ELU, ac_EmailLists EL");
            selectQuery.Append(" WHERE ELU.EmailListId = EL.EmailListId");
            selectQuery.Append(" AND ELU.Email = @email");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@email", System.Data.DbType.String, loweredEmail);
            //EXECUTE THE COMMAND
            EmailListCollection results = new EmailListCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        EmailList emailList = new EmailList();
                        EmailList.LoadDataReader(emailList, dr);
                        results.Add(emailList);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }
    }
}