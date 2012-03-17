using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Stores
{
    /// <summary>
    /// DataSource class for Currency objects
    /// </summary>
    [DataObject(true)]
    public partial class CurrencyDataSource
    {
        /// <summary>
        /// Gets exchange rate of the given currency
        /// </summary>
        /// <param name="currencyId">Id of the currency to get exchange rate for</param>
        /// <returns>Exchange rate of the given currency</returns>
        public static Decimal GetExchangeRate(int currencyId)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT ExchangeRate FROM ac_Currencies WHERE CurrencyId = @currencyId");
            database.AddInParameter(selectCommand, "@currencyId", System.Data.DbType.Int32, currencyId);
            return CommerceBuilder.Utility.AlwaysConvert.ToDecimal(database.ExecuteScalar(selectCommand));
        }

    }
}
