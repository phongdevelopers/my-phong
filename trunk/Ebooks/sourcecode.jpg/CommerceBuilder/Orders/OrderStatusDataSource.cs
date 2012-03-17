using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Utility;
using CommerceBuilder.Stores;

namespace CommerceBuilder.Orders
{
    [DataObject(true)]
    public partial class OrderStatusDataSource
    {
        /// <summary>
        /// Gets a list of order statuses that should be considered
        /// fraudulent, cancelled, or otherwise invalidated orders.
        /// </summary>
        /// <returns>A list of order statuses that should be considered
        /// cancelled or invalid orders.</returns>
        public static List<OrderStatus> GetCancelledStatuses()
        {
            int storeId = Token.Instance.StoreId;
            List<OrderStatus> cancelledStatuses = new List<OrderStatus>();
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + OrderStatus.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_OrderStatuses");
            selectQuery.Append(" WHERE StoreId = @storeId AND IsValid = 0");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, storeId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    OrderStatus orderStatus = new OrderStatus();
                    OrderStatus.LoadDataReader(orderStatus, dr);
                    cancelledStatuses.Add(orderStatus);
                }
                dr.Close();
            }
            return cancelledStatuses;
        }

        /// <summary>
        /// Gets a list of order statuses that should be considered
        /// valid sales and included in reports of sales totals.
        /// </summary>
        /// <returns>A list of order statuses that should be included
        /// in sales reports.</returns>
        public static List<OrderStatus> GetReportStatuses()
        {
            int storeId = Token.Instance.StoreId;
            List<OrderStatus> reportStatuses = new List<OrderStatus>();
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + OrderStatus.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_OrderStatuses");
            selectQuery.Append(" WHERE StoreId = @storeId AND IsActive = 1");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, storeId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    OrderStatus orderStatus = new OrderStatus();
                    OrderStatus.LoadDataReader(orderStatus, dr);
                    reportStatuses.Add(orderStatus);
                }
                dr.Close();
            }
            return reportStatuses;
        }

        /// <summary>
        /// Gets the order status that should be assigned to new orders.
        /// </summary>
        /// <returns>The order status that should be assigned to new orders.</returns>
        public static OrderStatus GetNewOrderStatus()
        {
            OrderStatus orderStatus = null;
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + OrderStatus.GetColumnNames("OS"));
            selectQuery.Append(" FROM ac_OrderStatuses OS INNER JOIN ac_OrderStatusTriggers OST");
            selectQuery.Append(" ON OS.OrderStatusId = OST.OrderStatusId");
            selectQuery.Append(" WHERE OS.StoreId = @storeId AND OST.StoreEventId = @orderPlaced");
            selectQuery.Append(" ORDER BY OS.OrderStatusId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "@orderPlaced", System.Data.DbType.Int32, StoreEvent.OrderPlaced);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                //ONLY READ THE FIRST RECORD
                if (dr.Read())
                {
                    orderStatus = new OrderStatus();
                    OrderStatus.LoadDataReader(orderStatus, dr);
                }
                dr.Close();
            }
            //CREATE AN ORDER STATUS FOR THIS EVENT AS THIS METHOD SHOULD NEVER RETURN NULL
            if (orderStatus == null)
            {
                orderStatus = new OrderStatus();
                orderStatus.Name = "New";
                orderStatus.DisplayName = "New";
                orderStatus.IsActive = true;
                orderStatus.IsValid = true;
                orderStatus.Triggers.Add(new OrderStatusTrigger(StoreEvent.OrderPlaced));
                orderStatus.Save();
            }
            return orderStatus;
        }

        
        /// <summary>
        /// Moves all orders from one orderstatus to another.
        /// </summary>
        /// <param name="oldOrderStatusId">The orderstatus that orders are to be moved from.</param>
        /// <param name="newOrderStatusId">The orderstatus that orders are to be moved to.</param>
        internal static void MoveOrders(int oldOrderStatusId, int newOrderStatusId)
        {
            if (oldOrderStatusId != newOrderStatusId)
            {
                int storeId = Token.Instance.StoreId;
                Database database = Token.Instance.Database;
                DbCommand selectCommand = database.GetSqlStringCommand("UPDATE ac_Orders SET OrderStatusId = @newOrderStatusId WHERE StoreId=@storeId AND OrderStatusId=@oldOrderStatusId");
                database.AddInParameter(selectCommand, "@newOrderStatusId", System.Data.DbType.Int32, newOrderStatusId);
                database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, storeId);
                database.AddInParameter(selectCommand, "@oldOrderStatusId", System.Data.DbType.Int32, oldOrderStatusId);
                database.ExecuteNonQuery(selectCommand);
            }
        }

        /// <summary>
        /// Load all valid order statuses.
        /// </summary>
        /// <returns>The collection of order statuses marked valid.</returns>
        public static OrderStatusCollection LoadValidOrderStatuses()
        {
            int storeId = Token.Instance.StoreId;
            OrderStatusCollection statuses = new OrderStatusCollection();
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + OrderStatus.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_OrderStatuses ");
            selectQuery.Append(" WHERE StoreId = @storeId AND IsValid = 1 ");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, storeId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    OrderStatus orderStatus = new OrderStatus();
                    OrderStatus.LoadDataReader(orderStatus, dr);
                    statuses.Add(orderStatus);
                }
                dr.Close();
            }
            return statuses;    
        }
    }
}
