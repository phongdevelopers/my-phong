using System;
using System.Collections.Generic;
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
    public partial class WarehouseDataSource
    {
        /// <summary>
        /// Moves all products from one warehouse to another.
        /// </summary>
        /// <param name="oldWarehouseId">The warehouse that products are to be moved from.</param>
        /// <param name="newWarehouseId">The warehouse that products are to be moved to.</param>
        public static void MoveProducts(int oldWarehouseId, int newWarehouseId)
        {
            if (oldWarehouseId != newWarehouseId)
            {
                int storeId = Token.Instance.StoreId;
                Database database = Token.Instance.Database;
                DbCommand selectCommand = database.GetSqlStringCommand("UPDATE ac_Products SET WarehouseId = @newWarehouseId WHERE StoreId=@storeId AND WarehouseId=@oldWarehouseId");
                database.AddInParameter(selectCommand, "@newWarehouseId", System.Data.DbType.Int32, newWarehouseId);
                database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, storeId);
                database.AddInParameter(selectCommand, "@oldWarehouseId", System.Data.DbType.Int32, oldWarehouseId);
                database.ExecuteNonQuery(selectCommand);
            }
        }
    }
}
