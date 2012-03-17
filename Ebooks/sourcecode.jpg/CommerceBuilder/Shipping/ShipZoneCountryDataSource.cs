using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Shipping
{
    [DataObject(true)]
    public partial class ShipZoneCountryDataSource
    {
        /// <summary>
        /// Indicates whether an association exists in the database
        /// </summary>
        /// <param name="shipZoneId">The ship zone ID to test</param>
        /// <param name="countryCode">The country code to test</param>
        /// <returns>True if the association exists in the database, false otherwise.</returns>
        public static bool Exists(int shipZoneId, string countryCode)
        {
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT COUNT(*) As ExistsCount");
            selectQuery.Append(" FROM ac_ShipZoneCountries");
            selectQuery.Append(" WHERE ShipZoneId = @shipZoneId");
            selectQuery.Append(" AND CountryCode = @countryCode");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@shipZoneId", System.Data.DbType.Int32, shipZoneId);
            database.AddInParameter(selectCommand, "@countryCode", System.Data.DbType.String, countryCode);
            return (AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand)) == 1);
        }
    }
}
