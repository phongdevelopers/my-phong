using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Users
{
    /// <summary>
    /// DataSource class for WishlistItem objects
    /// </summary>
    [DataObject(true)]
    public partial class WishlistItemDataSource
    {
        /// <summary>
        /// Delets all WishlistItem objects from database for given product Id
        /// </summary>
        /// <param name="productId">Id of the product for which to delete all WishlistItem objects</param>
        public static void DeleteForProduct(int productId)
        {
            Database database = Token.Instance.Database;
            DbCommand deleteCommand = database.GetSqlStringCommand("DELETE FROM ac_WishlistItems WHERE ProductId = @productId");
            database.AddInParameter(deleteCommand, "@productId", System.Data.DbType.Int32, productId);
            database.ExecuteNonQuery(deleteCommand);
        }
    }
}
