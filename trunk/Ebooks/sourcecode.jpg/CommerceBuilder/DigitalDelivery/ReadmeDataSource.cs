using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Utility;

namespace CommerceBuilder.DigitalDelivery
{
    [DataObject(true)]
    public partial class ReadmeDataSource
    {
        /// <summary>
        /// Changes the readme for a group of digital goods
        /// </summary>
        /// <param name="oldReadmeId">The readme that digital goods are to be moved from.</param>
        /// <param name="newReadmeId">The readme that digital goods are to be moved to.</param>
        public static void MoveDigitalGoods(int oldReadmeId, int newReadmeId)
        {
            if (oldReadmeId != newReadmeId)
            {
                int storeId = Token.Instance.StoreId;
                Database database = Token.Instance.Database;
                DbCommand selectCommand = database.GetSqlStringCommand("UPDATE ac_DigitalGoods SET ReadmeId = @newReadmeId WHERE ReadmeId = @oldReadmeId");
                database.AddInParameter(selectCommand, "@newReadmeId", System.Data.DbType.Int32, NullableData.DbNullify(newReadmeId));
                database.AddInParameter(selectCommand, "@oldReadmeId", System.Data.DbType.Int32, oldReadmeId);
                database.ExecuteNonQuery(selectCommand);
            }
        }
    }
}
