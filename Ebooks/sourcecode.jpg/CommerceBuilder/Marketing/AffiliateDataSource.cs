using System;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;
using System.ComponentModel;
using System.Web.Security;


namespace CommerceBuilder.Marketing
{
    [DataObject(true)]
    public partial class AffiliateDataSource
    {
        /// <summary>
        /// Gets a count of users who were referred by the affiliate during the given timeframe.
        /// </summary>
        /// <param name="affiliateId">Id of the affiliate to count users for.</param>
        /// <param name="startDate">Inclusive start date of timeframe to count.</param>
        /// <param name="endDate">Inclusive end date of timeframe to count.</param>
        /// <returns>The number of new visitors referred during the given timeframe.</returns>
        public static int GetReferralCount(int affiliateId, DateTime startDate, DateTime endDate)
        {
            Database database = Token.Instance.Database;
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT COUNT(*) AS TotalRecords FROM ac_Users WHERE AffiliateId = @affiliateId");
            if (startDate > DateTime.MinValue) sql.Append(" AND CreateDate >= @startDate");
            if (endDate > DateTime.MinValue) sql.Append(" AND CreateDate <= @endDate");
            DbCommand selectCommand = database.GetSqlStringCommand(sql.ToString());
            database.AddInParameter(selectCommand, "@affiliateId", System.Data.DbType.Int32, affiliateId);
            if (startDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@startDate", System.Data.DbType.DateTime, startDate);
            if (endDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@endDate", System.Data.DbType.DateTime, endDate);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Calculates the percentage of referred users who made purchases for a given timeframe.
        /// </summary>
        /// <param name="affiliateId">Id of the affiliate to calculate conversion rate for.</param>
        /// <param name="startDate">Inclusive start date of timeframe to calculate.</param>
        /// <param name="endDate">Inclusive end date of timeframe to calculate.</param>
        /// <param name="totalReferrals">The number of referrals for the timeframe.</param>
        /// <returns>The conversion rate for the affiliate for the given timeframe.</returns>
        public static LSDecimal GetConversionRate(int affiliateId, DateTime startDate, DateTime endDate, int totalReferrals)
        {
            if (totalReferrals == 0) return 0;
            Database database = Token.Instance.Database;
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT COUNT(DISTINCT ac_Users.UserId) AS TotalRecords");
            sql.Append(" FROM ac_Users INNER JOIN ac_Orders ON ac_Users.UserId = ac_Orders.UserId");
            sql.Append(" WHERE ac_Users.AffiliateId = @affiliateId");
            if (startDate > DateTime.MinValue) sql.Append(" AND ac_Users.CreateDate >= @startDate");
            if (endDate > DateTime.MinValue) sql.Append(" AND ac_Users.CreateDate <= @endDate");
            DbCommand selectCommand = database.GetSqlStringCommand(sql.ToString());
            database.AddInParameter(selectCommand, "@affiliateId", System.Data.DbType.Int32, affiliateId);
            if (startDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@startDate", System.Data.DbType.DateTime, startDate);
            if (endDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@endDate", System.Data.DbType.DateTime, endDate);
            int convertedCustomers = CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
            LSDecimal percentage = (LSDecimal)convertedCustomers / (LSDecimal)totalReferrals;
            percentage = percentage * 100;
            percentage = Math.Round((Decimal)percentage, 2);
            return percentage;
        }

        /// <summary>
        /// Get the total count of order for specified user associated with specifed affiliate
        /// </summary>
        /// <param name="affiliateId"></param>
        /// <param name="userId"></param>
        /// <param name="referralDate"></param>
        /// <returns>Number of orders user has with specified affiliate</returns>
        public static int GetOrderCountForUser(int affiliateId, int userId, DateTime referralDate) 
        {
            Database database = Token.Instance.Database;
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT COUNT(OrderId) AS OrderCount");
            sql.Append(" FROM ac_Orders");
            sql.Append(" WHERE UserId = @userId AND AffiliateId = @affiliateId");
            if (referralDate > DateTime.MinValue)
            {
                sql.Append(" AND OrderDate >= @referralDate");
            }
            
            DbCommand selectCommand = database.GetSqlStringCommand(sql.ToString());
            database.AddInParameter(selectCommand, "@userId", System.Data.DbType.Int32, userId);
            database.AddInParameter(selectCommand, "@affiliateId", System.Data.DbType.Int32, affiliateId);
            if (referralDate > DateTime.MinValue)
            {
                database.AddInParameter(selectCommand, "@referralDate", System.Data.DbType.DateTime, LocaleHelper.FromLocalTime(referralDate));
            }

            return AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

    }
}
