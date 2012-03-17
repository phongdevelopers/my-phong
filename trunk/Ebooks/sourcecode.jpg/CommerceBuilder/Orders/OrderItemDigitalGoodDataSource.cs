using System.ComponentModel;
using CommerceBuilder.Common;
using System.Text;
using System;
using CommerceBuilder.Data;
using System.Data.Common;
using System.Data;

namespace CommerceBuilder.Orders
{
    /// <summary>
    /// DataSource class for OrderItemDigitalGood objects
    /// </summary>
    [DataObject(true)]
    public partial class OrderItemDigitalGoodDataSource
    {

        /// <summary>
        /// Counts number of OrderItemDigitalGood objects for the given user
        /// </summary>
        /// <param name="userId">Id of the user to count the OrderItemDigitalGood objects for</param>
        /// <returns>Number of OrderItemDigitalGood objects for the given user</returns>
        public static int CountForUser(Int32 userId)
        {
            Database database = Token.Instance.Database;
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT COUNT(*) AS TotalRecords ");
            selectQuery.Append(" FROM ac_OrderItemDigitalGoods OIDG INNER JOIN ac_OrderItems OI ON OIDG.OrderItemId = OI.OrderItemId ");
            selectQuery.Append(" INNER JOIN ac_Orders O ON OI.OrderId = O.OrderId ");
            selectQuery.Append(" WHERE O.UserId = @userId");
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@userId", System.Data.DbType.Int32, userId);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of OrderItemDigitalGood objects for the given user
        /// </summary>
        /// <param name="userId">Id of the user to load the objects for</param>
        /// <returns>A collection of OrderItemDigitalGood objects for the given user</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderItemDigitalGoodCollection LoadForUser(Int32 userId)
        {
            return LoadForUser(userId, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of OrderItemDigitalGood objects for the given user
        /// </summary>
        /// <param name="userId">Id of the user to load the objects for</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of OrderItemDigitalGood objects for the given user</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderItemDigitalGoodCollection LoadForUser(Int32 userId, string sortExpression)
        {
            return LoadForUser(userId, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of OrderItemDigitalGood objects for the given user
        /// </summary>
        /// <param name="userId">Id of the user to load the objects for</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of OrderItemDigitalGood objects for the given user</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderItemDigitalGoodCollection LoadForUser(Int32 userId, int maximumRows, int startRowIndex)
        {
            return LoadForUser(userId, maximumRows, startRowIndex, string.Empty);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId">Id of the user to load the objects for</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of OrderItemDigitalGood objects for the given user</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderItemDigitalGoodCollection LoadForUser(Int32 userId, int maximumRows, int startRowIndex, string sortExpression)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + OrderItemDigitalGood.GetColumnNames("OIDG"));
            selectQuery.Append(" FROM ac_OrderItemDigitalGoods OIDG INNER JOIN ac_OrderItems OI ON OIDG.OrderItemId = OI.OrderItemId ");
            selectQuery.Append(" INNER JOIN ac_Orders O ON OI.OrderId = O.OrderId ");
            selectQuery.Append(" WHERE O.UserId = @userId");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);

            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@userId", System.Data.DbType.Int32, userId);
            //EXECUTE THE COMMAND
            OrderItemDigitalGoodCollection results = new OrderItemDigitalGoodCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        OrderItemDigitalGood orderItemDigitalGood = new OrderItemDigitalGood();
                        OrderItemDigitalGood.LoadDataReader(orderItemDigitalGood, dr);
                        results.Add(orderItemDigitalGood);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

    }
}
