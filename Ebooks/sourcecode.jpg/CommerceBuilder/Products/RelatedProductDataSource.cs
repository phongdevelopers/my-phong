using System.ComponentModel;
using System.Text;
using System.Data.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Common;
using System.Data;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Products
{
    [DataObject(true)]
    public partial class RelatedProductDataSource
    {
        public static int GetRelatedCount(int productId, int childProductId)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" SELECT COUNT(RP.ProductId) AS ProductCount ");
            sb.Append(" FROM ac_RelatedProducts AS RP ");
            sb.Append(" WHERE RP.ProductId = @productId AND RP.ChildProductId = @childProductId ");
            
            Database database = Token.Instance.Database;
            DbCommand command = database.GetSqlStringCommand(sb.ToString());
            database.AddInParameter(command, "@productId",DbType.Int32, productId);
            database.AddInParameter(command, "@childProductId", DbType.Int32, childProductId);
            return AlwaysConvert.ToInt(database.ExecuteScalar(command));
        }
    }
}
