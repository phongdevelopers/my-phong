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
    public partial class ShipZoneProvinceDataSource
    {
        /// <summary>
        /// Indicates whether an association exists in the database
        /// </summary>
        /// <param name="shipZoneId">The ship zone ID to test</param>
        /// <param name="provinceId">The province ID to test</param>
        /// <returns>True if the association exists in the database, false otherwise.</returns>
        public static bool Exists(int shipZoneId, int provinceId)
        {
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT COUNT(*) As ExistsCount");
            selectQuery.Append(" FROM ac_ShipZoneProvinces");
            selectQuery.Append(" WHERE ShipZoneId = @shipZoneId");
            selectQuery.Append(" AND ProvinceId = @provinceId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@shipZoneId", System.Data.DbType.Int32, shipZoneId);
            database.AddInParameter(selectCommand, "@provinceId", System.Data.DbType.Int32, provinceId);
            return (AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand)) == 1);
        }
    }
}
