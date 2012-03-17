using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Products
{
    [DataObject(true)]
    public partial class OptionDataSource
    {
        /// <summary>
        /// Deletes options that have no associated products.
        /// </summary>
        internal static void DeleteOrphanedOptions()
        {
            Database database = Token.Instance.Database;
            StringBuilder deleteSql = new StringBuilder();
            deleteSql.Append("DELETE FROM ac_Options WHERE OptionId IN (");
            deleteSql.Append("SELECT O.OptionId FROM ac_Options O LEFT JOIN ac_ProductOptions PO");
            deleteSql.Append(" ON O.OptionId = PO.OptionId");
            deleteSql.Append(" WHERE PO.OptionId IS NULL)");
            database.ExecuteScalar(database.GetSqlStringCommand(deleteSql.ToString()));
        }
    }
}
