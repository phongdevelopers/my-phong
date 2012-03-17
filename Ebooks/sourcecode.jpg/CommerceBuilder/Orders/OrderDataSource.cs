using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Utility;
using CommerceBuilder.Reporting;
using CommerceBuilder.Products;
using CommerceBuilder.Stores;

namespace CommerceBuilder.Orders
{
    /// <summary>
    /// DataSource class for Order objects
    /// </summary>
    [DataObject(true)]
    public partial class OrderDataSource
    {
        /// <summary>
        /// Gets OrderStatusId for the given order
        /// </summary>
        /// <param name="orderId">Id of the order for which to get the OrderStatusId</param>
        /// <returns>OrderStatus Id for the given order</returns>
        public static int GetOrderStatusId(int orderId)
        {
            Database database = Token.Instance.Database;
            using (DbCommand selectCommand = database.GetSqlStringCommand("SELECT OrderStatusId FROM ac_Orders WHERE OrderId = @orderId"))
            {
                database.AddInParameter(selectCommand, "@orderId", System.Data.DbType.Int32, orderId);
                return AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
            }
        }

        /// <summary>
        /// Updates order status of the given order
        /// </summary>
        /// <param name="order">The order for which to update the order status</param>
        /// <param name="newStatusId">The new OrderStatusId to set fort the order</param>
        public static void UpdateOrderStatus(Order order, int newStatusId)
        {
            OrderStatus newStatus = OrderStatusDataSource.Load(newStatusId);
            UpdateOrderStatus(order, newStatus);
        }

        /// <summary>
        /// Updates order status of the given order
        /// </summary>
        /// <param name="order">The order for which to update the order status</param>
        /// <param name="newStatus">The new OrderStatus to set for the order</param>
        public static void UpdateOrderStatus(Order order, OrderStatus newStatus)
        {
            if (newStatus != null)
                {
                bool saveDirty = order.IsDirty;
                string originalStatusName = string.Empty;
                if (order.OrderStatus != null) originalStatusName = order.OrderStatus.Name;
                order.OrderStatusId = newStatus.OrderStatusId;
                Database database = Token.Instance.Database;
                DbCommand update = database.GetSqlStringCommand("UPDATE ac_Orders SET OrderStatusId = @orderStatusId WHERE OrderId = @orderId");
                database.AddInParameter(update, "orderStatusId", System.Data.DbType.Int32, order.OrderStatusId);
                database.AddInParameter(update, "orderId", System.Data.DbType.Int32, order.OrderId);
                if (database.ExecuteNonQuery(update) > 0)
                {
                    order.IsDirty = saveDirty;
                    //ADJUST STOCK IF NEEDED
                    if (Token.Instance.Store.EnableInventory)
                    {
                        if (newStatus.InventoryAction != InventoryAction.None)
                        {
                            try
                            {

                                string statusId, updateStatusId;
                                if (newStatus.InventoryAction == InventoryAction.Destock)
                                {
                                    statusId = "0";
                                    updateStatusId = "1";
                                }
                                else
                                {
                                    statusId = "1";
                                    updateStatusId = "0";
                                }

                                Dictionary<int, string> LowStockProducts = new Dictionary<int, string>();
                                StringBuilder query = new StringBuilder();
                                query.Append("SELECT OI.OrderItemId,OI.Quantity,OI.ProductId,OI.OptionList");
                                query.Append(" FROM ac_OrderItems OI INNER JOIN ac_Products P ON OI.ProductId = P.ProductId");
                                query.Append(" WHERE OI.OrderId = @orderId AND OI.InventoryStatusId = " + statusId + " AND P.InventoryModeId > 0");
                                DbCommand select = database.GetSqlStringCommand(query.ToString());
                                database.AddInParameter(select, "@orderId", System.Data.DbType.Int32, order.OrderId);
                                DataSet inventoriedProducts = database.ExecuteDataSet(select);
                                foreach (DataRow row in inventoriedProducts.Tables[0].Rows)
                                {
                                    if (newStatus.InventoryAction == InventoryAction.Destock)
                                    {
                                        int productId = (int)row[2];
                                        bool lowStock;
                                        InventoryManager.Destock((short)row[1], productId, (row[3] == DBNull.Value) ? string.Empty : (string)row[3], out lowStock);
                                        if (lowStock)
                                        {
                                            LowStockProducts.Add(productId, (row[3] == DBNull.Value) ? string.Empty : (string)row[3]);
                                        }
                                    }
                                    else InventoryManager.Restock((short)row[1], (int)row[2], (row[3] == DBNull.Value) ? string.Empty : (string)row[3]);
                                    update = database.GetSqlStringCommand("UPDATE ac_OrderItems set InventoryStatusId = " + updateStatusId + " WHERE OrderItemId = @orderItemId");
                                    database.AddInParameter(update, "@orderItemId", DbType.Int32, row[0]);
                                    database.ExecuteNonQuery(update);
                                }
                                if (LowStockProducts.Count > 0)
                                {
                                    StoreEventEngine.LowInventoryItemPurchased(LowStockProducts);
                                }
                            }
                            catch (Exception ex)
                            {
                                //IGNORE ERRORS
                                Logger.Error("Error adjusting inventory levels for order number " + order.OrderId.ToString(), ex);
                            }
                        }
                    }
                    //CHECK IF ORDER STATUS IS CHANGING FROM ONE TO ANOTHER
                    if (!string.IsNullOrEmpty(originalStatusName) && !originalStatusName.Equals(newStatus.Name))
                    {
                        //UPDATE ORDER NOTES
                        order.Notes.Add(new OrderNote(order.OrderId, Token.Instance.UserId, LocaleHelper.LocalNow, string.Format(Properties.Resources.OrderStatusUpdated, originalStatusName, newStatus.Name), NoteType.SystemPrivate));
                        order.Notes.Save();
                        //TRIGGER ORDER STATUS EVENT
                        StoreEventEngine.OrderStatusUpdated(order, originalStatusName);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the GoogleOrderNumber for the given order. 
        /// </summary>
        /// <param name="order">The order for which to update the GoogleOrderNumber</param>
        /// <param name="googleOrderNumber">The new GoogleOrderNumber to set</param>
        public static void UpdateGoogleOrderNumber(Order order, string googleOrderNumber)
        {
            UpdateGoogleOrderNumber(order.OrderId, googleOrderNumber);
        }

        /// <summary>
        /// Updates the GoogleOrderNumber for the given order. 
        /// </summary>
        /// <param name="orderId">Id of the order for which to update the GoogleOrderNumber</param>
        /// <param name="googleOrderNumber">The new GoogleOrderNumber to set</param>
        public static void UpdateGoogleOrderNumber(int orderId, string googleOrderNumber)
        {
            string updateQuery = "UPDATE ac_Orders SET GoogleOrderNumber = @googleOrderNumber WHERE OrderId = @orderId";
            Database database = Token.Instance.Database;
            using (System.Data.Common.DbCommand updateCommand = database.GetSqlStringCommand(updateQuery))
            {
                database.AddInParameter(updateCommand, "@googleOrderNumber", System.Data.DbType.String, googleOrderNumber);
                database.AddInParameter(updateCommand, "@orderId", System.Data.DbType.Int32, orderId);
                int recordsAffected = database.ExecuteNonQuery(updateCommand);
            }
        }

        /// <summary>
        /// Reassign orders from one user to another.
        /// </summary>
        /// <param name="oldUserId">The user to assign orders from</param>
        /// <param name="newUserId">The user to assign orders to</param>
        public static void UpdateUser(int oldUserId, int newUserId)
        {
            string updateQuery = "UPDATE ac_Orders SET UserId = @newUserId WHERE UserId = @oldUserId";
            Database database = Token.Instance.Database;
            using (System.Data.Common.DbCommand updateCommand = database.GetSqlStringCommand(updateQuery))
            {
                database.AddInParameter(updateCommand, "@newUserId", System.Data.DbType.Int32, newUserId);
                database.AddInParameter(updateCommand, "@oldUserId", System.Data.DbType.Int32, oldUserId);
                database.ExecuteNonQuery(updateCommand);
            }
            //MIGRATE ANY ORDER NOTES
            updateQuery = "UPDATE ac_OrderNotes SET UserId = @newUserId WHERE UserId = @oldUserId";
            using (System.Data.Common.DbCommand updateCommand = database.GetSqlStringCommand(updateQuery))
            {
                database.AddInParameter(updateCommand, "@newUserId", System.Data.DbType.Int32, newUserId);
                database.AddInParameter(updateCommand, "@oldUserId", System.Data.DbType.Int32, oldUserId);
                database.ExecuteNonQuery(updateCommand);
            }
            //MIGRATE ANY SUBSCRIPTIONS
            updateQuery = "UPDATE ac_Subscriptions SET UserId = @newUserId WHERE UserId = @oldUserId";
            using (System.Data.Common.DbCommand updateCommand = database.GetSqlStringCommand(updateQuery))
            {
                database.AddInParameter(updateCommand, "@newUserId", System.Data.DbType.Int32, newUserId);
                database.AddInParameter(updateCommand, "@oldUserId", System.Data.DbType.Int32, oldUserId);
                database.ExecuteNonQuery(updateCommand);
            }
            //ATTEMPT TO MIGRATE ANY GROUP ASSOCIATIONS (TRY/CATCH IN CASE ASSOCIATIONS ALREADY EXIST FOR TARGET USER)
            try
            {
                updateQuery = "UPDATE ac_UserGroups SET UserId = @newUserId WHERE UserId = @oldUserId";
                using (System.Data.Common.DbCommand updateCommand = database.GetSqlStringCommand(updateQuery))
                {
                    database.AddInParameter(updateCommand, "@newUserId", System.Data.DbType.Int32, newUserId);
                    database.AddInParameter(updateCommand, "@oldUserId", System.Data.DbType.Int32, oldUserId);
                    database.ExecuteNonQuery(updateCommand);
                }
            }
            catch { }
            //MIGRATE ANY GIFTCERTIFICATES
            updateQuery = "UPDATE ac_GiftCertificates SET CreatedBy = @newUserId WHERE CreatedBy = @oldUserId";
            using (System.Data.Common.DbCommand updateCommand = database.GetSqlStringCommand(updateQuery))
            {
                database.AddInParameter(updateCommand, "@newUserId", System.Data.DbType.Int32, newUserId);
                database.AddInParameter(updateCommand, "@oldUserId", System.Data.DbType.Int32, oldUserId);
                database.ExecuteNonQuery(updateCommand);
            }
        }

        internal static void UpdatePaymentStatus(Order order, OrderPaymentStatus paymentStatus)
        {
            bool saveDirty = order.IsDirty;
            order.PaymentStatus = paymentStatus;
            string updateQuery = "UPDATE ac_Orders SET PaymentStatusId = @paymentStatus WHERE OrderId = @orderId";
            Database database = Token.Instance.Database;
            using (System.Data.Common.DbCommand updateCommand = database.GetSqlStringCommand(updateQuery))
            {
                database.AddInParameter(updateCommand, "@paymentStatus", System.Data.DbType.Byte, (byte)paymentStatus);
                database.AddInParameter(updateCommand, "@orderId", System.Data.DbType.Int32, order.OrderId);
                if (database.ExecuteNonQuery(updateCommand) > 0) order.IsDirty = saveDirty;
            }
        }

        internal static void UpdateShipmentStatus(Order order, OrderShipmentStatus ShipmentStatus)
        {
            bool saveDirty = order.IsDirty;
            order.ShipmentStatus = ShipmentStatus;
            string updateQuery = "UPDATE ac_Orders SET ShipmentStatusId = @ShipmentStatus WHERE OrderId = @orderId";
            Database database = Token.Instance.Database;
            using (System.Data.Common.DbCommand updateCommand = database.GetSqlStringCommand(updateQuery))
            {
                database.AddInParameter(updateCommand, "@ShipmentStatus", System.Data.DbType.Byte, (byte)ShipmentStatus);
                database.AddInParameter(updateCommand, "@orderId", System.Data.DbType.Int32, order.OrderId);
                if (database.ExecuteNonQuery(updateCommand) > 0) order.IsDirty = saveDirty;
            }
        }

        /// <summary>
        /// Updates the calcualted totals for an order with the given values
        /// </summary>
        /// <param name="order">The order to be updated</param>
        /// <param name="productSubtotal">The new product subtotal</param>
        /// <param name="totalCharges">The new total</param>
        /// <param name="totalPayments">The new total of processed payments</param>
        /// <returns>True if a database update was required, false otherwise.</returns>
        public static bool UpdateCalculatedTotals(Order order, LSDecimal productSubtotal, LSDecimal totalCharges, LSDecimal totalPayments)
        {
            bool dataRead = false;
            LSDecimal currentProductSubtotal = 0;
            LSDecimal currentTotalCharges = 0;
            LSDecimal currentTotalPayments = 0;
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT ProductSubtotal, TotalCharges, TotalPayments FROM ac_Orders WHERE OrderId = @orderId");
            database.AddInParameter(selectCommand, "@orderId", System.Data.DbType.Int32, order.OrderId);
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                if (dr.Read())
                {
                    dataRead = true;
                    currentProductSubtotal = AlwaysConvert.ToDecimal(dr.GetDecimal(0));
                    currentTotalCharges = AlwaysConvert.ToDecimal(dr.GetDecimal(1));
                    currentTotalPayments = AlwaysConvert.ToDecimal(dr.GetDecimal(2));
                }
            }
            if (dataRead)
            {
                if ((productSubtotal != currentProductSubtotal) ||
                    (totalCharges != currentTotalCharges) ||
                    (totalPayments != currentTotalPayments))
                {
                    //THE VALUES WERE RECALCULATED, WE MUST UPDATE THE DATABASE
                    bool saveDirty = order.IsDirty;
                    order.ProductSubtotal = productSubtotal;
                    order.TotalCharges = totalCharges;
                    order.TotalPayments = totalPayments;
                    database = Token.Instance.Database;
                    DbCommand update = database.GetSqlStringCommand("UPDATE ac_Orders SET ProductSubtotal = @productSubtotal, TotalCharges = @totalCharges, TotalPayments = @totalPayments WHERE OrderId = @orderId");
                    database.AddInParameter(update, "productSubtotal", System.Data.DbType.Decimal, productSubtotal);
                    database.AddInParameter(update, "totalCharges", System.Data.DbType.Decimal, totalCharges);
                    database.AddInParameter(update, "totalPayments", System.Data.DbType.Decimal, totalPayments);
                    database.AddInParameter(update, "orderId", System.Data.DbType.Int32, order.OrderId);
                    if (database.ExecuteNonQuery(update) > 0) order.IsDirty = saveDirty;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Search orders for given criteria
        /// </summary>
        /// <param name="orderStatusId">order status Id of the orders to search</param>
        /// <param name="paymentStatus">payment status of the orders to search</param>
        /// <param name="shipmentStatus">shipment status of the orders to search</param>
        /// <param name="startDate">starting date for search</param>
        /// <param name="endDate">ending date for search</param>
        /// <returns>Collection of orders for given criteria</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderCollection Search(int orderStatusId, OrderPaymentStatus paymentStatus, OrderShipmentStatus shipmentStatus, DateTime startDate, DateTime endDate)
        {
            return OrderDataSource.Search(orderStatusId, paymentStatus, shipmentStatus, startDate, endDate, 0, 0, string.Empty);
        }

        /// <summary>
        /// Search orders for given criteria
        /// </summary>
        /// <param name="orderStatusId">order status Id of the orders to search</param>
        /// <param name="paymentStatus">payment status of the orders to search</param>
        /// <param name="shipmentStatus">shipment status of the orders to search</param>
        /// <param name="startDate">starting date for search</param>
        /// <param name="endDate">ending date for search</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>Collection of orders for given criteria</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderCollection Search(int orderStatusId, OrderPaymentStatus paymentStatus, OrderShipmentStatus shipmentStatus, DateTime startDate, DateTime endDate, string sortExpression)
        {
            return OrderDataSource.Search(orderStatusId, paymentStatus, shipmentStatus, startDate, endDate, 0, 0, sortExpression);
        }

        /// <summary>
        /// Search orders for given criteria
        /// </summary>
        /// <param name="orderStatusId">order status Id of the orders to search</param>
        /// <param name="paymentStatus">payment status of the orders to search</param>
        /// <param name="shipmentStatus">shipment status of the orders to search</param>
        /// <param name="startDate">starting date for search</param>
        /// <param name="endDate">ending date for search</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>Collection of orders for given criteria</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderCollection Search(int orderStatusId, OrderPaymentStatus paymentStatus, OrderShipmentStatus shipmentStatus, DateTime startDate, DateTime endDate, int maximumRows, int startRowIndex, string sortExpression)
        {
            //SET DEFAULT SORT
            if (string.IsNullOrEmpty(sortExpression)) sortExpression = "OrderDate DESC";
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + Order.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Orders");
            selectQuery.Append(" WHERE StoreId = @storeId");
            if (orderStatusId != 0) selectQuery.Append(" AND OrderStatusId = @orderStatusId");
            if (paymentStatus != OrderPaymentStatus.Unspecified) selectQuery.Append(" AND PaymentStatusId = @paymentStatus");
            if (shipmentStatus != OrderShipmentStatus.Unspecified) selectQuery.Append(" AND ShipmentStatusId = @shipmentStatus");
            if (startDate > DateTime.MinValue) selectQuery.Append(" AND OrderDate >= @startDate");
            if (endDate > DateTime.MinValue) selectQuery.Append(" AND OrderDate <= @endDate");
            selectQuery.Append(" ORDER BY " + sortExpression);
            //BUILD THE COMMAND
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            if (orderStatusId != 0) database.AddInParameter(selectCommand, "@orderStatusId", System.Data.DbType.Int32, orderStatusId);
            if (startDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@startDate", System.Data.DbType.DateTime, startDate);
            if (endDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@endDate", System.Data.DbType.DateTime, endDate);
            if (paymentStatus != OrderPaymentStatus.Unspecified) database.AddInParameter(selectCommand, "@paymentStatus", System.Data.DbType.Byte, paymentStatus);
            if (shipmentStatus != OrderShipmentStatus.Unspecified) database.AddInParameter(selectCommand, "@shipmentStatus", System.Data.DbType.Byte, shipmentStatus);
            //EXECUTE THE COMMAND
            OrderCollection results = new OrderCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        Order order = new Order();
                        Order.LoadDataReader(order, dr);
                        results.Add(order);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Counts the number of orders for given search criteria
        /// </summary>
        /// <param name="orderStatusId">order status Id of the orders to search</param>
        /// <param name="paymentStatus">payment status of the orders to search</param>
        /// <param name="shipmentStatus">shipment status of the orders to search</param>
        /// <param name="startDate">starting date for search</param>
        /// <param name="endDate">ending date for search</param>
        /// <returns>The number of orders for given search criteria</returns>
        public static int SearchCount(int orderStatusId, OrderPaymentStatus paymentStatus, OrderShipmentStatus shipmentStatus, DateTime startDate, DateTime endDate)
        {
            //COUNT ORDER IDS THAT FIT THE CRITERIA
            StringBuilder countQuery = new StringBuilder();
            countQuery.Append("SELECT COUNT(*) AS SearchCount");
            countQuery.Append(" FROM ac_Orders");
            countQuery.Append(" WHERE StoreId = @storeId");
            if (orderStatusId != 0) countQuery.Append(" AND OrderStatusId = @orderStatusId");
            if (paymentStatus != OrderPaymentStatus.Unspecified) countQuery.Append(" AND PaymentStatusId = @paymentStatusId");
            if (shipmentStatus != OrderShipmentStatus.Unspecified) countQuery.Append(" AND ShipmentStatusId = @shipmentStatusId");
            if (startDate > DateTime.MinValue) countQuery.Append(" AND OrderDate >= @startDate");
            if (endDate > DateTime.MinValue) countQuery.Append(" AND OrderDate <= @endDate");
            Database database = Token.Instance.Database;
            DbCommand countCommand = database.GetSqlStringCommand(countQuery.ToString());
            database.AddInParameter(countCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            if (orderStatusId != 0) database.AddInParameter(countCommand, "@orderStatusId", System.Data.DbType.Int32, orderStatusId);
            if (startDate > DateTime.MinValue) database.AddInParameter(countCommand, "@startDate", System.Data.DbType.DateTime, startDate);
            if (endDate > DateTime.MinValue) database.AddInParameter(countCommand, "@endDate", System.Data.DbType.DateTime, endDate);
            if (paymentStatus != OrderPaymentStatus.Unspecified) database.AddInParameter(countCommand, "@paymentStatus", System.Data.DbType.Byte, paymentStatus);
            if (shipmentStatus != OrderShipmentStatus.Unspecified) database.AddInParameter(countCommand, "@shipmentStatus", System.Data.DbType.Byte, shipmentStatus);
            return AlwaysConvert.ToInt(database.ExecuteScalar(countCommand));
        }

        /// <summary>
        /// Search orders for given criteria
        /// </summary>
        /// <param name="criteria">The search criteria for orders to search</param>
        /// <returns>Collection of orders for given criteria</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public OrderCollection Search(OrderSearchCriteria criteria)
        {
            return this.Search(criteria, 0, 0, string.Empty);
        }

        /// <summary>
        /// Search orders for given criteria
        /// </summary>
        /// <param name="criteria">The search criteria for orders to search</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>Collection of orders for given criteria</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public OrderCollection Search(OrderSearchCriteria criteria, string sortExpression)
        {
            return this.Search(criteria, 0, 0, sortExpression);
        }

        /// <summary>
        /// Search orders for given criteria
        /// </summary>
        /// <param name="criteria">The search criteria for orders to search</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>Collection of orders for given criteria</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public OrderCollection Search(OrderSearchCriteria criteria, int maximumRows, int startRowIndex, string sortExpression)
        {
            //CREATE RETURN SET
            OrderCollection orders = new OrderCollection();

            //GET LIST OF ORDER IDS THAT FIT THE CRITERIA
            Database database = Token.Instance.Database;
            DbCommand selectCommand = criteria.BuildSelectCommand(sortExpression);
            List<int> orderIdList = new List<int>();
            //EXECUTE THE COMMAND TO OBTAIN ORDER ID List
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    orderIdList.Add(dr.GetInt32(0));
                }
                dr.Close();
            }

            //NOW GET ORDER DETAIL FOR CORRECT INDEXES
            int rowIndex = startRowIndex;
            int rowCount = 0;
            if (maximumRows < 1) maximumRows = 1000;
            while ((rowCount < maximumRows) && (rowIndex < orderIdList.Count))
            {
                Order order = OrderDataSource.Load(orderIdList[rowIndex]);
                orders.Add(order);
                rowIndex++;
                rowCount++;
            }

            //RETURN ORDER DETAIL MATCHING CRITERIA
            return orders;
        }

        /// <summary>
        /// Counts the number of orders for given search criteria
        /// </summary>
        /// <param name="criteria">The search criteria for orders to search</param>
        /// <returns>The number of orders for given search criteria</returns>
        public static int SearchCount(OrderSearchCriteria criteria)
        {
            //COUNT ORDER IDS THAT FIT THE CRITERIA
            Database database = Token.Instance.Database;
            DbCommand countCommand = criteria.BuildSelectCommand(string.Empty, true);
            return AlwaysConvert.ToInt(database.ExecuteScalar(countCommand));
        }

        /// <summary>
        /// Loads an order object for given GoogleOrderNumber
        /// </summary>
        /// <param name="googleOrderNumber">The GoogleOrderNumber to load and Order object for</param>
        /// <returns>The loaded Order object or null if no order is found for the given GoogleOrderNumber</returns>
        public static Order LoadForGoogleOrderNumber(string googleOrderNumber)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT ");
            selectQuery.Append(Order.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Orders");
            selectQuery.Append(" WHERE GoogleOrderNumber = @googleOrderNumber");
            selectQuery.Append(" AND StoreId = @storeId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@googleOrderNumber", System.Data.DbType.String, googleOrderNumber);
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                if (dr.Read())
                {
                    Order order = new Order();
                    Order.LoadDataReader(order, dr);
                    return order;
                }
                dr.Close();
            }
            return null;
        }

        /// <summary>
        /// Loads a collection of orders associated to a given affiliate
        /// </summary>
        /// <param name="affiliateId">Affiliate for which to load the orders</param>
        /// <param name="startDate">start date to consider when loading orders</param>
        /// <param name="endDate">end date to consider when loading orders</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>A collection of orders associated to a given affiliate</returns>
        public static OrderCollection LoadForAffiliate(int affiliateId, DateTime startDate, DateTime endDate, string sortExpression)
        {
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT ");
            selectQuery.Append(Order.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Orders");
            selectQuery.Append(" WHERE AffiliateId = @affiliateId");
            selectQuery.Append(" AND StoreId = @storeId");
            if (startDate > DateTime.MinValue) selectQuery.Append(" AND OrderDate >= @startDate");
            if (endDate > DateTime.MinValue) selectQuery.Append(" AND OrderDate <= @endDate");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@affiliateId", System.Data.DbType.Int32, affiliateId);
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            if (startDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@startDate", System.Data.DbType.DateTime, LocaleHelper.FromLocalTime(startDate));
            if (endDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@endDate", System.Data.DbType.DateTime, LocaleHelper.FromLocalTime(endDate));
            OrderCollection results = new OrderCollection();
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    Order order = new Order();
                    Order.LoadDataReader(order, dr);
                    results.Add(order);
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Loads a collection of orders associated to the given coupon code
        /// </summary>
        /// <param name="couponCode">Coupon Code for which to load the associated orders</param>
        /// <param name="startDate">start date to consider when loading orders</param>
        /// <param name="endDate">end date to consider when loading orders</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>A collection of orders associated to the given coupon code</returns>
        public static OrderCollection LoadForCouponCode(string couponCode, DateTime startDate, DateTime endDate, string sortExpression)
        {
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT ");
            selectQuery.Append(Order.GetColumnNames("O"));
            selectQuery.Append(" FROM ac_Orders O INNER JOIN ac_OrderCoupons C ON O.OrderId = C.OrderId");
            selectQuery.Append(" WHERE C.CouponCode = @couponCode");
            selectQuery.Append(" AND O.StoreId = @storeId");
            List<OrderStatus> reportStatuses = OrderStatusDataSource.GetReportStatuses();
            selectQuery.Append(" AND " + ReportDataSource.GetStatusFilter(reportStatuses, "O"));
            if (startDate > DateTime.MinValue) selectQuery.Append(" AND O.OrderDate >= @startDate");
            if (endDate > DateTime.MinValue) selectQuery.Append(" AND O.OrderDate <= @endDate");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@couponCode", System.Data.DbType.String, couponCode);
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            if (startDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@startDate", System.Data.DbType.DateTime, LocaleHelper.FromLocalTime(startDate));
            if (endDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@endDate", System.Data.DbType.DateTime, LocaleHelper.FromLocalTime(endDate));
            ReportDataSource.SetStatusFilterParams(reportStatuses, database, selectCommand);
            OrderCollection results = new OrderCollection();
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    Order order = new Order();
                    Order.LoadDataReader(order, dr);
                    results.Add(order);
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Gets coupons codes used for orders
        /// </summary>
        /// <returns>An array of string with the codes of coupons used</returns>
        public static string[] GetCouponCodes()
        {
            return OrderDataSource.GetCouponCodes(DateTime.MinValue, DateTime.MinValue);
        }

        /// <summary>
        /// Gets coupons codes used for orders
        /// </summary>
        /// <param name="startDate">The starting date for orders to be checked for coupons</param>
        /// <param name="endDate">The ending date for orders to be checked for coupons</param>
        /// <returns>An array of string with the codes of coupons used</returns>
        public static string[] GetCouponCodes(DateTime startDate, DateTime endDate)
        {
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT DISTINCT(C.CouponCode)");
            selectQuery.Append(" FROM ac_Orders O INNER JOIN ac_OrderCoupons C ON O.OrderId = C.OrderId");
            selectQuery.Append(" WHERE O.StoreId = @storeId");
            if (startDate > DateTime.MinValue) selectQuery.Append(" AND O.OrderDate >= @startDate");
            if (endDate > DateTime.MinValue) selectQuery.Append(" AND O.OrderDate <= @endDate");
            List<OrderStatus> reportStatuses = OrderStatusDataSource.GetReportStatuses();
            selectQuery.Append(" AND " + ReportDataSource.GetStatusFilter(reportStatuses, "O"));
            selectQuery.Append(" ORDER BY C.CouponCode");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            if (startDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@startDate", System.Data.DbType.DateTime, LocaleHelper.FromLocalTime(startDate));
            if (endDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@endDate", System.Data.DbType.DateTime, LocaleHelper.FromLocalTime(endDate));
            ReportDataSource.SetStatusFilterParams(reportStatuses, database, selectCommand);
            List<string> results = new List<string>();
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    results.Add(dr.GetString(0));
                }
                dr.Close();
            }
            if (results.Count == 0) return null;
            return results.ToArray();
        }

        /// <summary>
        /// Gets the OrderNumber for given the OrderId
        /// </summary>
        /// <param name="orderId">The OrderId</param>
        /// <returns>The OrderNumber for the given OrderId</returns>
        public static int LookupOrderNumber(int orderId)
        {
            Database database = Token.Instance.Database;
            using (DbCommand selectCommand = database.GetSqlStringCommand("SELECT OrderNumber FROM ac_Orders WHERE StoreId = @storeId AND OrderId = @orderId"))
            {
                database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
                database.AddInParameter(selectCommand, "@orderId", System.Data.DbType.Int32, orderId);
                return AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
            }
        }

        /// <summary>
        /// Gets the OrderId for given the order number
        /// </summary>
        /// <param name="orderNumber">The order number</param>
        /// <returns>The OrderId for the given order number</returns>
        public static int LookupOrderId(int orderNumber)
        {
            Database database = Token.Instance.Database;
            using (DbCommand selectCommand = database.GetSqlStringCommand("SELECT OrderId FROM ac_Orders WHERE StoreId = @storeId AND OrderNumber = @orderNumber"))
            {
                database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
                database.AddInParameter(selectCommand, "@orderNumber", System.Data.DbType.Int32, orderNumber);
                return AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
            }
        }

        /// <summary>
        /// Gets a lookup dictionary to translate order numbers into order ids
        /// </summary>
        /// <param name="orderNumbers">An array of order numbers</param>
        /// <returns>A dictionary where the key is the OrderNumber and the value is the OrderId</returns>
        public static Dictionary<int, int> LookupOrderIds(int[] orderNumbers)
        {
            Dictionary<int, int> orderIdLookup = new Dictionary<int, int>();
            if ((orderNumbers == null) || (orderNumbers.Length == 0)) return orderIdLookup;
            if (orderNumbers.Length == 1)
            {
                orderIdLookup.Add(orderNumbers[0], LookupOrderId(orderNumbers[0]));
                return orderIdLookup;
            }
            string orderNumberList = AlwaysConvert.ToList(",", orderNumbers);
            Database database = Token.Instance.Database;
            using (DbCommand selectCommand = database.GetSqlStringCommand("SELECT OrderNumber,OrderId FROM ac_Orders WHERE StoreId = @storeId AND OrderNumber IN (" + orderNumberList + ")"))
            {
                database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
                using (IDataReader dr = database.ExecuteReader(selectCommand))
                {
                    while (dr.Read())
                    {
                        orderIdLookup.Add(dr.GetInt32(0), dr.GetInt32(1));
                    }
                    dr.Close();
                }
            }
            return orderIdLookup;
        }

        /// <summary>
        /// Gets a lookup dictionary to translate order Ids into order numbers
        /// </summary>
        /// <param name="orderIds">An array of order Ids</param>
        /// <returns>A dictionary where the key is the OrderId and the value is the OrderNumber</returns>
        public static Dictionary<int, int> LookupOrderNumbers(int[] orderIds)
        {
            Dictionary<int, int> orderNumberLookup = new Dictionary<int, int>();
            if ((orderIds == null) || (orderIds.Length == 0)) return orderNumberLookup;
            if (orderIds.Length == 1)
            {
                orderNumberLookup.Add(orderIds[0], LookupOrderNumber(orderIds[0]));
                return orderNumberLookup;
            }
            string orderIdList = AlwaysConvert.ToList(",", orderIds);
            Database database = Token.Instance.Database;
            using (DbCommand selectCommand = database.GetSqlStringCommand("SELECT OrderId,OrderNumber FROM ac_Orders WHERE StoreId = @storeId AND OrderId IN (" + orderIdList + ")"))
            {
                database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
                using (IDataReader dr = database.ExecuteReader(selectCommand))
                {
                    while (dr.Read())
                    {
                        orderNumberLookup.Add(dr.GetInt32(0), dr.GetInt32(1));
                    }
                    dr.Close();
                }
            }
            return orderNumberLookup;
        }

        /// <summary>
        /// Returns a comma delimited list of OrderNumber given a comma delimited list of OrderId
        /// </summary>
        /// <param name="orderIdList">A comma delimited list of OrderId</param>
        /// <returns>A comma delimited list of OrderNumber</returns>
        public static string GetOrderNumberList(string orderIdList)
        {
            int[] orderIds = AlwaysConvert.ToIntArray(orderIdList);
            Dictionary<int, int> orderNumberLookup = LookupOrderNumbers(orderIds);
            List<string> orderNumbers = new List<string>();
            foreach (int id in orderNumberLookup.Keys)
            {
                orderNumbers.Add(orderNumberLookup[id].ToString());
            }
            return string.Join(",", orderNumbers.ToArray());
        }

        /// <summary>
        /// Returns a comma delimited list of OrderId given a comma delimited list of OrderNumber
        /// </summary>
        /// <param name="orderNumberList">A comma delimited list of OrderNumber</param>
        /// <returns>A comma delimited list of OrderId</returns>
        public static string GetOrderIdList(string orderNumberList)
        {
            int[] orderNumbers = AlwaysConvert.ToIntArray(orderNumberList);
            Dictionary<int, int> orderIdLookup = LookupOrderIds(orderNumbers);
            List<string> orderIds = new List<string>();
            foreach (int id in orderIdLookup.Keys)
            {
                orderIds.Add(orderIdLookup[id].ToString());
            }
            return string.Join(",", orderIds.ToArray());
        }
    }
}