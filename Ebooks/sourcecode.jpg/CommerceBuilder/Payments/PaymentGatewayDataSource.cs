using System;
using System.ComponentModel;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using System.Data;
using System.Data.Common;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Payments
{
    /// <summary>
    /// DataSource class for PaymentGateway objects.
    /// </summary>
    [DataObject(true)]
    public partial class PaymentGatewayDataSource
    {
        /// <summary>
        /// Given the class Id of a payment gateway implementation, gets the corresponding object Id in database.
        /// </summary>
        /// <param name="classId">Class Id of a payment gateway implementation</param>
        /// <returns>The corresponding object Id in database</returns>
        public static int GetPaymentGatewayIdByClassId(string classId)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT PaymentGatewayId FROM ac_PaymentGateways WHERE StoreId=@storeId AND ClassId=@classId");
            database.AddInParameter(selectCommand, "@storeId", DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "@classId", DbType.String, classId);
            return AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }
    }
}
