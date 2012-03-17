using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Taxes
{
    /// <summary>
    /// DataSource class for TaxGateway objects
    /// </summary>
    [DataObject(true)]
    public partial class TaxGatewayDataSource
    {
        /// <summary>
        /// Gets Id of a TaxGateway given its class Id
        /// </summary>
        /// <param name="classId">The class Id of the TaxGateway</param>
        /// <returns>Id of the TaxGateway if found or '0' if not found</returns>
        public static int GetTaxGatewayIdByClassId(string classId)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT TaxGatewayId FROM ac_TaxGateways WHERE StoreId=@storeId AND ClassId=@classId");
            database.AddInParameter(selectCommand, "@storeId", DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "@classId", DbType.String, classId);
            return AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }
    }
}
