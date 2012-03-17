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
    /// <summary>
    /// DataSource class for ProductProductTemplate objects
    /// </summary>
    [DataObject(true)]
    public partial class ProductProductTemplateDataSource
    {
        /// <summary>
        /// Delete any templates associated with the product
        /// </summary>
        public static void DeleteForProduct(int productId)
        {
            Database database = Token.Instance.Database;
            string sql = "DELETE FROM ac_ProductProductTemplates WHERE ProductId = @productId";
            using (DbCommand deleteCommand = database.GetSqlStringCommand(sql))
            {
                database.AddInParameter(deleteCommand, "productId", DbType.Int32, productId);
                database.ExecuteNonQuery(deleteCommand);
            }
        }
    }
}
