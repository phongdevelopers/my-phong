using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Orders;
using CommerceBuilder.Utility;
using CommerceBuilder.Marketing;
using CommerceBuilder.Users;
using CommerceBuilder.Products;
using System.Web;

namespace CommerceBuilder.Reporting
{
    /// <summary>
    /// DataSource class for various reporting tasks
    /// </summary>
    [DataObject(true)]
    public class ReportDataSource
    {
        internal static string GetStatusFilter(List<OrderStatus> statuses)
        {
            return GetStatusFilter(statuses, string.Empty);
        }

        internal static string GetStatusFilter(List<OrderStatus> statuses, string prefix)
        {
            StringBuilder filter = new StringBuilder();
            int index;
            if (statuses.Count > 0)
            {
                if (!string.IsNullOrEmpty(prefix)) prefix += ".";
                if (statuses.Count > 1) filter.Append("(");
                index = 0;
                foreach (OrderStatus status in statuses)
                {
                    if (index > 0) filter.Append(" OR ");
                    filter.Append(prefix + "OrderStatusId = @orderStatusId" + index);
                    index++;
                }
                if (statuses.Count > 1) filter.Append(")");
            }
            else
            {
                //this really shouldn't happen, merchant doesn't have an order status 
                //defined for reports.  so we have to filter the results to produce
                //an empty result.  Make a condition that will never come true...
                filter.Append("OrderStatusId = 0");
            }
            return filter.ToString();
        }

        internal static void SetStatusFilterParams(List<OrderStatus> statuses, Database database, DbCommand command)
        {
            int index = 0;
            foreach (OrderStatus status in statuses)
            {
                database.AddInParameter(command, "@orderStatusId" + index, DbType.Int32, status.OrderStatusId);
                index++;
            }
        }

        /// <summary>
        /// Gets the sales details per user
        /// </summary>
        /// <param name="startDate">Inclusive start date to check the sales from</param>
        /// <param name="endDate">Inclusive end date up to which to check the sales</param>
        /// <returns>A list of UserSaleSummary objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<UserSummary> GetSalesByUser(DateTime startDate, DateTime endDate)
        {
            return GetSalesByUser(startDate, endDate, 0, 0, string.Empty);
        }

        /// <summary>
        /// Gets the sales details per user
        /// </summary>
        /// <param name="startDate">Inclusive start date to check the sales from</param>
        /// <param name="endDate">Inclusive end date up to which to check the sales</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>        /// <returns></returns>
        /// <returns>A list of UserSaleSummary objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<UserSummary> GetSalesByUser(DateTime startDate, DateTime endDate, string sortExpression)
        {
            return GetSalesByUser(startDate, endDate, 0, 0, sortExpression);
        }

        /// <summary>
        /// Gets the sales details per user
        /// </summary>
        /// <param name="startDate">Inclusive start date to check the sales from</param>
        /// <param name="endDate">Inclusive end date up to which to check the sales</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>A list of UserSaleSummary objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<UserSummary> GetSalesByUser(DateTime startDate, DateTime endDate, int maximumRows, int startRowIndex, string sortExpression)
        {
            //GET ALL CUSTOMERS WHO PLACED ORDERS
            List<UserSummary> results = new List<UserSummary>();
            //DEFAULT SORT EXPRESSION
            if (string.IsNullOrEmpty(sortExpression)) sortExpression = "OrderTotal DESC";
            //GET INVALID ORDER STATUSES
            List<OrderStatus> reportStatuses = OrderStatusDataSource.GetReportStatuses();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" UserId, COUNT(*) AS OrderCount, SUM(TotalCharges) AS OrderTotal");
            selectQuery.Append(" FROM ac_Orders");
            selectQuery.Append(" WHERE StoreId = @storeId");
            selectQuery.Append(" AND UserId IS NOT NULL");
            selectQuery.Append(" AND " + GetStatusFilter(reportStatuses));
            if (startDate > DateTime.MinValue)
            {
                //CONVERT DATE TO UTC
                startDate = LocaleHelper.FromLocalTime(startDate);
                selectQuery.Append(" AND OrderDate >= @startDate");
            }
            if (endDate > DateTime.MinValue)
            {
                //CONVERT DATE TO UTC
                endDate = LocaleHelper.FromLocalTime(endDate);
                selectQuery.Append(" AND OrderDate <= @endDate");
            }
            selectQuery.Append(" GROUP BY UserId");
            selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            SetStatusFilterParams(reportStatuses, database, selectCommand);
            if (startDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@startDate", System.Data.DbType.DateTime, startDate);
            if (endDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@endDate", System.Data.DbType.DateTime, endDate);
            //EXECUTE THE COMMAND
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        UserSummary summary = new UserSummary();
                        summary.UserId = dr.GetInt32(0);
                        summary.OrderCount = dr.GetInt32(1);
                        summary.OrderTotal = dr.GetDecimal(2);
                        results.Add(summary);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Gets the number of users with sales between the given start and end dates
        /// </summary>
        /// <param name="startDate">Inclusive start date to consider</param>
        /// <param name="endDate">Inclusive end date to consider</param>
        /// <returns>The number of users with sales between the given start and end dates</returns>
        public static int GetSalesByUserCount(DateTime startDate, DateTime endDate)
        {
            List<OrderStatus> reportStatuses = OrderStatusDataSource.GetReportStatuses();
            Database database = Token.Instance.Database;
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT COUNT(*) AS UserCount FROM (SELECT DISTINCT UserId FROM ac_Orders");
            selectQuery.Append(" WHERE StoreId = @storeId");
            selectQuery.Append(" AND " + GetStatusFilter(reportStatuses));
            if (startDate > DateTime.MinValue)
            {
                //CONVERT DATE TO UTC
                startDate = LocaleHelper.FromLocalTime(startDate);
                selectQuery.Append(" AND OrderDate >= @startDate");
            }
            if (endDate > DateTime.MinValue)
            {
                //CONVERT DATE TO UTC
                endDate = LocaleHelper.FromLocalTime(endDate);
                selectQuery.Append(" AND OrderDate <= @endDate");
            }
            selectQuery.Append(") AS UniqueCustomers");
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            SetStatusFilterParams(reportStatuses, database, selectCommand);
            if (startDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@startDate", System.Data.DbType.DateTime, startDate);
            if (endDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@endDate", System.Data.DbType.DateTime, endDate);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        #region GetSalesByCoupon
        /// <summary>
        /// Gets details of sales by given coupon code
        /// </summary>
        /// <param name="startDate">Inclusive start date to consider</param>
        /// <param name="endDate">Inclusive end date to consider</param>
        /// <param name="couponCode">The coupon code to get sales details for</param>
        /// <returns>A list of CouponSalesSummary objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<CouponSummary> GetSalesByCoupon(DateTime startDate, DateTime endDate, string couponCode)
        {
            return GetSalesByCoupon(startDate, endDate, couponCode, 0, 0, string.Empty);
        }

        /// <summary>
        /// Gets details of sales by given coupon code
        /// </summary>
        /// <param name="startDate">Inclusive start date to consider</param>
        /// <param name="endDate">Inclusive end date to consider</param>
        /// <param name="couponCode">The coupon code to get sales details for</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>A list of CouponSalesSummary objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<CouponSummary> GetSalesByCoupon(DateTime startDate, DateTime endDate, string couponCode, string sortExpression)
        {
            return GetSalesByCoupon(startDate, endDate, couponCode, 0, 0, sortExpression);
        }

        /// <summary>
        /// Gets details of sales by given coupon code
        /// </summary>
        /// <param name="startDate">Inclusive start date to consider</param>
        /// <param name="endDate">Inclusive end date to consider</param>
        /// <param name="couponCode">The coupon code to get sales details for</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <returns>A list of CouponSalesSummary objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<CouponSummary> GetSalesByCoupon(DateTime startDate, DateTime endDate, string couponCode, int maximumRows, int startRowIndex)
        {
            return GetSalesByCoupon(startDate, endDate, couponCode, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Gets details of sales by given coupon code
        /// </summary>
        /// <param name="startDate">Inclusive start date to consider</param>
        /// <param name="endDate">Inclusive end date to consider</param>
        /// <param name="couponCode">The coupon code to get sales details for</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>A list of CouponSalesSummary objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<CouponSummary> GetSalesByCoupon(DateTime startDate, DateTime endDate, string couponCode, int maximumRows, int startRowIndex, string sortExpression)
        {
            //GET ALL CUSTOMERS WHO PLACED ORDERS
            List<CouponSummary> results = new List<CouponSummary>();
            //DEFAULT SORT EXPRESSION
            if (string.IsNullOrEmpty(sortExpression)) sortExpression = "OrderTotal DESC";
            //GET INVALID ORDER STATUSES
            List<OrderStatus> reportStatuses = OrderStatusDataSource.GetReportStatuses();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" ac_OrderCoupons.CouponCode, COUNT(*) AS OrderCount, SUM(TotalCharges) AS OrderTotal");
            selectQuery.Append(" FROM ac_Orders INNER JOIN ac_OrderCoupons ON ac_Orders.OrderId = ac_OrderCoupons.OrderId");
            selectQuery.Append(" WHERE StoreId = @storeId");
            selectQuery.Append(" AND " + GetStatusFilter(reportStatuses));
            if (startDate > DateTime.MinValue)
            {
                //CONVERT DATE TO UTC
                startDate = LocaleHelper.FromLocalTime(startDate);
                selectQuery.Append(" AND OrderDate >= @startDate");
            }
            if (endDate > DateTime.MinValue)
            {
                //CONVERT DATE TO UTC
                endDate = LocaleHelper.FromLocalTime(endDate);
                selectQuery.Append(" AND OrderDate <= @endDate");
            }
            if (!string.IsNullOrEmpty(couponCode)) selectQuery.Append(" AND CouponCode = @couponCode");
            selectQuery.Append(" GROUP BY ac_OrderCoupons.CouponCode");
            selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            SetStatusFilterParams(reportStatuses, database, selectCommand);
            if (startDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@startDate", System.Data.DbType.DateTime, startDate);
            if (endDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@endDate", System.Data.DbType.DateTime, endDate);
            if (!string.IsNullOrEmpty(couponCode)) database.AddInParameter(selectCommand, "@couponCode", System.Data.DbType.String, couponCode);
            //EXECUTE THE COMMAND
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        CouponSummary summary = new CouponSummary();
                        summary.CouponCode = dr.GetString(0);
                        summary.StartDate = startDate;
                        summary.EndDate = endDate;
                        summary.OrderCount = dr.GetInt32(1);
                        summary.OrderTotal = dr.GetDecimal(2);
                        results.Add(summary);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Gets the record count for sales by coupon report with given criteria
        /// </summary>
        /// <param name="startDate">Inclusive start date to consider</param>
        /// <param name="endDate">Inclusive end date to consider</param>
        /// <param name="couponCode">The coupon code to get sales details for</param>
        /// <returns>Number of records for sales by coupon report with given criteria</returns>
        public static int GetSalesByCouponCount(DateTime startDate, DateTime endDate, string couponCode)
        {
            List<OrderStatus> reportStatuses = OrderStatusDataSource.GetReportStatuses();
            Database database = Token.Instance.Database;
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT COUNT(*) AS CouponCount FROM (SELECT DISTINCT CouponCode FROM ac_Orders INNER JOIN ac_OrderCoupons ON ac_Orders.OrderId = ac_OrderCoupons.OrderId");
            selectQuery.Append(" WHERE StoreId = @storeId");
            selectQuery.Append(" AND " + GetStatusFilter(reportStatuses));
            if (startDate > DateTime.MinValue)
            {
                //CONVERT DATE TO UTC
                startDate = LocaleHelper.FromLocalTime(startDate);
                selectQuery.Append(" AND OrderDate >= @startDate");
            }
            if (endDate > DateTime.MinValue)
            {
                //CONVERT DATE TO UTC
                endDate = LocaleHelper.FromLocalTime(endDate);
                selectQuery.Append(" AND OrderDate <= @endDate");
            }
            if (!string.IsNullOrEmpty(couponCode)) selectQuery.Append(" AND CouponCode = @couponCode");
            selectQuery.Append(") AS UniqueCoupons");
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            SetStatusFilterParams(reportStatuses, database, selectCommand);
            if (startDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@startDate", System.Data.DbType.DateTime, startDate);
            if (endDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@endDate", System.Data.DbType.DateTime, endDate);
            if (!string.IsNullOrEmpty(couponCode)) database.AddInParameter(selectCommand, "@couponCode", System.Data.DbType.String, couponCode);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        #endregion

        /// <summary>
        /// Gets sales by product report
        /// </summary>
        /// <param name="startDate">Inclusive start date to consider</param>
        /// <param name="endDate">Inclusive end date to consider</param>
        /// <returns>A list of ProductSalesSummary objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<ProductSummary> GetSalesByProduct(DateTime startDate, DateTime endDate)
        {
            return GetSalesByProduct(startDate, endDate, 0, 0, string.Empty);
        }

        /// <summary>
        /// Gets sales by product report
        /// </summary>
        /// <param name="startDate">Inclusive start date to consider</param>
        /// <param name="endDate">Inclusive end date to consider</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>A list of ProductSalesSummary objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<ProductSummary> GetSalesByProduct(DateTime startDate, DateTime endDate, string sortExpression)
        {
            return GetSalesByProduct(startDate, endDate, 0, 0, sortExpression);
        }

        /// <summary>
        /// Gets sales by product report
        /// </summary>
        /// <param name="startDate">Inclusive start date to consider</param>
        /// <param name="endDate">Inclusive end date to consider</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>A list of ProductSalesSummary objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<ProductSummary> GetSalesByProduct(DateTime startDate, DateTime endDate, int maximumRows, int startRowIndex, string sortExpression)
        {
            //GET ALL CUSTOMERS WHO PLACED ORDERS
            List<ProductSummary> results = new List<ProductSummary>();
            //DEFAULT SORT EXPRESSION
            if (string.IsNullOrEmpty(sortExpression)) sortExpression = "TotalQuantity DESC";
            //GET INVALID ORDER STATUSES
            List<OrderStatus> reportStatuses = OrderStatusDataSource.GetReportStatuses();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" P.ProductId, P.Name, SUM(OI.Quantity) AS TotalQuantity, SUM(OI.Price * OI.Quantity) AS TotalPrice");
            selectQuery.Append(" FROM (ac_Orders O INNER JOIN ac_OrderItems OI ON O.OrderId = OI.OrderId)");
            selectQuery.Append(" INNER JOIN ac_Products P ON OI.ProductId = P.ProductId");
            selectQuery.Append(" WHERE O.StoreId = @storeId");
            selectQuery.Append(" AND " + GetStatusFilter(reportStatuses, "O"));
            if (startDate != DateTime.MinValue)
            {
                //CONVERT DATE TO UTC
                startDate = LocaleHelper.FromLocalTime(startDate);
                selectQuery.Append(" AND O.OrderDate >= @startDate");
            }
            if ((endDate != DateTime.MaxValue) && (endDate != DateTime.MinValue))
            {
                //CONVERT DATE TO UTC
                endDate = LocaleHelper.FromLocalTime(endDate);
                selectQuery.Append(" AND O.OrderDate <= @endDate");
            }
            selectQuery.Append(" GROUP BY P.ProductId, P.Name");
            selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            SetStatusFilterParams(reportStatuses, database, selectCommand);
            if (startDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@startDate", System.Data.DbType.DateTime, startDate);
            if (endDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@endDate", System.Data.DbType.DateTime, endDate);
            //EXECUTE THE COMMAND
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        ProductSummary summary = new ProductSummary();
                        summary.ProductId = dr.GetInt32(0);
                        summary.Name = dr.GetString(1);
                        summary.TotalQuantity = dr.GetInt32(2);
                        summary.TotalPrice = dr.GetDecimal(3);
                        results.Add(summary);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Gets the record count for sales by product report
        /// </summary>
        /// <param name="startDate">Inclusive start date to consider</param>
        /// <param name="endDate">Inclusive end date to consider</param>
        /// <returns>The record count for sales by product report</returns>
        public static int GetSalesByProductCount(DateTime startDate, DateTime endDate)
        {
            List<OrderStatus> reportStatuses = OrderStatusDataSource.GetReportStatuses();
            Database database = Token.Instance.Database;
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT COUNT(*) AS ProductCount FROM (SELECT DISTINCT P.ProductId FROM (ac_Orders O INNER JOIN ac_OrderItems OI ON O.OrderId = OI.OrderId) INNER JOIN ac_Products P ON OI.ProductId = P.ProductId");
            selectQuery.Append(" WHERE O.StoreId = @storeId");
            selectQuery.Append(" AND " + GetStatusFilter(reportStatuses, "O"));
            if (startDate > DateTime.MinValue)
            {
                //CONVERT DATE TO UTC
                startDate = LocaleHelper.FromLocalTime(startDate);
                selectQuery.Append(" AND O.OrderDate >= @startDate");
            }
            if (endDate > DateTime.MinValue)
            {
                //CONVERT DATE TO UTC
                endDate = LocaleHelper.FromLocalTime(endDate);
                selectQuery.Append(" AND O.OrderDate <= @endDate");
            }
            selectQuery.Append(") AS UniqueProducts");
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            SetStatusFilterParams(reportStatuses, database, selectCommand);
            if (startDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@startDate", System.Data.DbType.DateTime, startDate);
            if (endDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@endDate", System.Data.DbType.DateTime, endDate);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }


        /// <summary>
        /// Builds a sales summary for the given period.
        /// </summary>
        /// <param name="startDate">Inclusive start date for sales summary; if startDate.Kind is not DateTimeKind.Utc, the date is converted from store local time.</param>
        /// <param name="endDate">Inclusive end date for sales summary; if endDate.Kind is not DateTimeKind.Utc, the date is converted from store local time.</param>
        /// <param name="updateUserCount">Indicates the user count should be updated or not</param>
        /// <returns>A sales summary for the given period.</returns>
        /// <remarks>The StartDate and EndDate properies of the returned summary are in store local time to simplify use in reports.</remarks>
        public static SalesSummary GetSalesSummary(DateTime startDate, DateTime endDate, bool updateUserCount)
        {
            //ADJUST DATES TO UTC
            if (startDate != DateTime.MinValue) if (startDate.Kind != DateTimeKind.Utc) startDate = LocaleHelper.FromLocalTime(startDate);
            if (endDate != DateTime.MinValue) if (endDate.Kind != DateTimeKind.Utc) endDate = LocaleHelper.FromLocalTime(endDate);
            //CREATE RETURN OBJECT
            SalesSummary summary = new SalesSummary();
            summary.StartDate = LocaleHelper.ToLocalTime(startDate);
            summary.EndDate = LocaleHelper.ToLocalTime(endDate);
            //GET INVALID ORDER STATUSES
            List<OrderStatus> reportStatuses = OrderStatusDataSource.GetReportStatuses();
            //GET THE ORDER COUNT FOR THE PERIOD
            summary.OrderCount = GetOrderCount(startDate, endDate, reportStatuses);
            if (summary.OrderCount > 0)
            {
                //LOAD TABLE OF ORDERITEM TOTALS
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT OI.OrderItemTypeId, SUM(OI.Price * OI.Quantity), SUM(OI.CostOfGoods * OI.Quantity), SUM(OI.Quantity)");
                sql.Append(" FROM ac_Orders O INNER JOIN ac_OrderItems OI ON O.OrderId = OI.OrderId");
                sql.Append(" WHERE O.StoreId = @storeId");
                sql.Append(" AND " + GetStatusFilter(reportStatuses, "O"));
                if (startDate != DateTime.MinValue) sql.Append(" AND O.OrderDate >= @startDate");
                if (endDate != DateTime.MinValue) sql.Append(" AND O.OrderDate <= @endDate");
                sql.Append(" GROUP BY OI.OrderItemTypeId");
                Database database = Token.Instance.Database;
                DbCommand selectCommand = database.GetSqlStringCommand(sql.ToString());
                database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
                SetStatusFilterParams(reportStatuses, database, selectCommand);
                if (startDate != DateTime.MinValue) database.AddInParameter(selectCommand, "@startDate", System.Data.DbType.DateTime, startDate);
                if (endDate != DateTime.MinValue) database.AddInParameter(selectCommand, "@endDate", System.Data.DbType.DateTime, endDate);
                //EXECUTE THE COMMAND
                using (IDataReader dr = database.ExecuteReader(selectCommand))
                {
                    while (dr.Read())
                    {
                        OrderItemType totalType = (OrderItemType)dr.GetInt16(0);
                        LSDecimal amount = dr.GetDecimal(1);
                        LSDecimal costOfGoods = dr.GetDecimal(2);

                        int itemCount = dr.GetInt32(3);
                        switch (totalType)
                        {
                            case OrderItemType.Product:
                                summary.ProductTotal += amount;
                                summary.CostOfGoodTotal += costOfGoods;
                                summary.ProductCount += itemCount;
                                break;
                            case OrderItemType.Shipping:
                            case OrderItemType.Handling:
                                summary.ShippingTotal += amount;
                                break;
                            case OrderItemType.Tax:
                                summary.TaxTotal += amount;
                                break;
                            case OrderItemType.Coupon:
                                summary.CouponTotal += amount;
                                break;
                            case OrderItemType.Discount:
                                summary.DiscountTotal += amount;
                                break;
                            case OrderItemType.GiftWrap:
                                summary.GiftWrapTotal += amount;
                                break;
                            default:
                                summary.OtherTotal += amount;
                                break;
                        }
                        summary.GrandTotal += amount;
                    }
                    dr.Close();                    
                }
                                
                // COUNNT USERS
                if (updateUserCount)
                {
                    List<Int32> userIds = new List<int>();
                    sql = new StringBuilder();
                    sql.Append("SELECT DISTINCT UserId FROM ac_Orders");
                    sql.Append(" WHERE StoreId = @storeId");
                    sql.Append(" AND UserId IS NOT NULL"); // AC5X IMPOPRTED ORDERS CAN HAVE NULL VALUES FOR USER ID
                    sql.Append(" AND " + GetStatusFilter(reportStatuses));
                    if (startDate != DateTime.MinValue) sql.Append(" AND OrderDate >= @startDate");
                    if (endDate != DateTime.MinValue) sql.Append(" AND OrderDate < @endDate");
                    summary.UserCount = userIds.Count;
                    selectCommand = database.GetSqlStringCommand(sql.ToString());
                    database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
                    SetStatusFilterParams(reportStatuses, database, selectCommand);
                    if (startDate != DateTime.MinValue) database.AddInParameter(selectCommand, "@startDate", System.Data.DbType.DateTime, startDate);
                    if (endDate != DateTime.MinValue) database.AddInParameter(selectCommand, "@endDate", System.Data.DbType.DateTime, endDate);
                    //EXECUTE THE COMMAND
                    using (IDataReader dr = database.ExecuteReader(selectCommand))
                    {

                        while (dr.Read())
                        {
                            userIds.Add(dr.GetInt32(0));
                        }
                    }
                    summary.UserCount = userIds.Count;
                }
            }
            return summary;
        }

        /// <summary>
        /// Gets the number of orders for the given period.
        /// </summary>
        /// <param name="startDate">Inclusive start date for order count.</param>
        /// <param name="endDate">Inclusive end date for order count.</param>
        /// <param name="reportStatuses">List of order statuses that are considered valid sales and should be included in the count.</param>
        /// <returns>The number of orders for the given period.</returns>
        private static int GetOrderCount(DateTime startDate, DateTime endDate, List<OrderStatus> reportStatuses)
        {
            if (startDate != DateTime.MinValue) if (startDate.Kind != DateTimeKind.Utc) startDate = LocaleHelper.FromLocalTime(startDate);
            if (endDate != DateTime.MinValue) if (endDate.Kind != DateTimeKind.Utc) endDate = LocaleHelper.FromLocalTime(endDate);
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT COUNT(*) AS OrderCount");
            sql.Append(" FROM ac_Orders");
            sql.Append(" WHERE StoreId = @storeId");
            sql.Append(" AND " + GetStatusFilter(reportStatuses));
            if (startDate != DateTime.MinValue) sql.Append(" AND OrderDate >= @startDate");
            if (endDate != DateTime.MinValue) sql.Append(" AND OrderDate < @endDate");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(sql.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            SetStatusFilterParams(reportStatuses, database, selectCommand);
            if (startDate != DateTime.MinValue) database.AddInParameter(selectCommand, "@startDate", System.Data.DbType.DateTime, startDate);
            if (endDate != DateTime.MinValue) database.AddInParameter(selectCommand, "@endDate", System.Data.DbType.DateTime, endDate);
            return AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Generates sales summaries for each day in the given month.
        /// </summary>
        /// <param name="year">The year to generate sales summaries for.</param>
        /// <param name="month">The month to generate sales summaries for.</param>
        /// <returns>A list of SalesSummary, one for each day in the month.  StartDate and EndDate properties of the SalesSummary are in store local time.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<SalesSummary> GetMonthlySales(int year, int month)
        {
            if ((month < 1) || (month > 12)) throw new ArgumentOutOfRangeException("Month must fall between 1 and 12.", "month");
            List<SalesSummary> results = new List<SalesSummary>();
            int daysInMonth = GetDaysInMonth(month, year);
            for (int i = 1; i <= daysInMonth; i++)
            {
                DateTime startDate = new DateTime(year, month, i, 0, 0, 0, 0, DateTimeKind.Unspecified);
                DateTime endDate = new DateTime(year, month, i, 23, 59, 59, 999, DateTimeKind.Unspecified);
                results.Add(GetSalesSummary(startDate, endDate, false));
            }
            return results;
        }

        /// <summary>
        /// Gets a daily sales report for a given date
        /// </summary>
        /// <param name="reportDate">The date to get the report for</param>
        /// <returns>A list of OrderSalesSummary objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<OrderSummary> GetDailySales(DateTime reportDate)
        {
            return GetDailySales(reportDate.Year, reportDate.Month, reportDate.Day);
        }

        /// <summary>
        /// Gets a daily sales report for a given date
        /// </summary>
        /// <param name="year">The year to consider</param>
        /// <param name="month">The month to consider</param>
        /// <param name="day">The day to consider</param>
        /// <returns>A list of OrderSalesSummary objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<OrderSummary> GetDailySales(int year, int month, int day)
        {
            if ((month < 1) || (month > 12)) throw new ArgumentOutOfRangeException("Month must fall between 1 and 12.", "month");
            List<OrderSummary> results = new List<OrderSummary>();
            //CALCULATE START DATE
            DateTime startDate = new DateTime(year, month, day, 0, 0, 0, 0, DateTimeKind.Unspecified);
            //CONVERT DATE TO UTC
            startDate = LocaleHelper.FromLocalTime(startDate);
            //CALCULATE END DATE
            DateTime endDate = startDate.AddDays(1).AddSeconds(-1);
            //GET INVALID ORDER STATUSES
            List<OrderStatus> reportStatuses = OrderStatusDataSource.GetReportStatuses();
            //NEXT LOAD TABLE OF ORDERITEM TOTALS
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT O.OrderId, O.OrderNumber, OI.OrderItemTypeId, SUM(OI.Price * OI.Quantity), SUM(OI.CostOfGoods * OI.Quantity)");
            sql.Append(" FROM ac_Orders O INNER JOIN ac_OrderItems OI ON O.OrderId = OI.OrderId");
            sql.Append(" WHERE O.StoreId = @storeId");
            sql.Append(" AND " + GetStatusFilter(reportStatuses, "O"));
            sql.Append(" AND O.OrderDate >= @startDate");
            sql.Append(" AND O.OrderDate <= @endDate");
            sql.Append(" GROUP BY O.OrderId, O.OrderNumber, OI.OrderItemTypeId");
            sql.Append(" ORDER BY O.OrderNumber");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(sql.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            SetStatusFilterParams(reportStatuses, database, selectCommand);
            database.AddInParameter(selectCommand, "@startDate", System.Data.DbType.DateTime, startDate);
            database.AddInParameter(selectCommand, "@endDate", System.Data.DbType.DateTime, endDate);
            //EXECUTE THE COMMAND
            int lastOrderId = 0;
            OrderSummary summary = null;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    int thisOrderId = dr.GetInt32(0);
                    if (thisOrderId != lastOrderId)
                    {
                        summary = new OrderSummary();
                        summary.OrderId = thisOrderId;
                        summary.OrderNumber = dr.GetInt32(1);
                        results.Add(summary);
                        lastOrderId = thisOrderId;
                    }
                    OrderItemType totalType = (OrderItemType)dr.GetInt16(2);
                    LSDecimal amount = dr.GetDecimal(3);
                    LSDecimal costOfGoods = dr.GetDecimal(4);
                    switch (totalType)
                    {
                        case OrderItemType.Product:
                            summary.ProductTotal += amount;
                            summary.CostOfGoodTotal += costOfGoods;
                            break;
                        case OrderItemType.Shipping:
                        case OrderItemType.Handling:
                            summary.ShippingTotal += amount;
                            break;
                        case OrderItemType.Tax:
                            summary.TaxTotal += amount;
                            break;
                        case OrderItemType.Coupon:
                            summary.CouponTotal += amount;
                            break;
                        case OrderItemType.Discount:
                            summary.DiscountTotal += amount;
                            break;
                        default:
                            summary.OtherTotal += amount;
                            break;
                    }
                    summary.GrandTotal += amount;
                }
                dr.Close();
            }
            return results;
        }


        /// <summary>
        /// Generates abandoned basket summaries for each day in the given month.
        /// </summary>
        /// <param name="year">The year to generate sales summaries for.</param>
        /// <param name="month">The month to generate sales summaries for.</param>
        /// <returns>A list of AbandonedBasketsSummary, one for each day in the month.  StartDate and EndDate properties of the SalesSummary are in store local time.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<AbandonedBasketsSummary> GetMonthlyAbandonedBaskets(int year, int month)
        {
            if ((month < 1) || (month > 12)) throw new ArgumentOutOfRangeException("Month must fall between 1 and 12.", "month");
            List<AbandonedBasketsSummary> results = new List<AbandonedBasketsSummary>();
            int daysInMonth = GetDaysInMonth(month, year);
            for (int i = 1; i <= daysInMonth; i++)
            {
                DateTime startDate = new DateTime(year, month, i, 0, 0, 0, 0, DateTimeKind.Unspecified);
                DateTime endDate = new DateTime(year, month, i, 23, 59, 59, 999, DateTimeKind.Unspecified);
                results.Add(GetAbandonedBasketsSummary(startDate, endDate));
            }
            return results;
        }

        /// <summary>
        /// Builds an abandoned baskets summary for the given period.
        /// </summary>
        /// <param name="startDate">Inclusive start date for sales summary; if startDate.Kind is not DateTimeKind.Utc, the date is converted from store local time.</param>
        /// <param name="endDate">Inclusive end date for sales summary; if endDate.Kind is not DateTimeKind.Utc, the date is converted from store local time.</param>
        /// <returns>An abandoned baskets summary for the given period.</returns>
        /// <remarks>The StartDate and EndDate properies of the returned summary are in store local time to simplify use in reports.</remarks>
        private static AbandonedBasketsSummary GetAbandonedBasketsSummary(DateTime startDate, DateTime endDate)
        {
            //ADJUST DATES TO UTC
            if (startDate.Kind != DateTimeKind.Utc) startDate = LocaleHelper.FromLocalTime(startDate);
            if (endDate.Kind != DateTimeKind.Utc) endDate = LocaleHelper.FromLocalTime(endDate);
            //CREATE RETURN OBJECT
            AbandonedBasketsSummary summary = new AbandonedBasketsSummary();
            summary.StartDate = LocaleHelper.ToLocalTime(startDate);
            summary.EndDate = LocaleHelper.ToLocalTime(endDate);
            //get all the baskets between startDate and endDate for which the last activity
            //is more than 30 minutes old
            StringBuilder bSql = new StringBuilder();
            bSql.Append("SELECT b.BasketId FROM ac_Baskets b INNER JOIN ac_Users u on b.UserId=u.UserId ");
            bSql.Append(" WHERE u.StoreId = @storeId");
            bSql.Append(" AND u.LastActivityDate >= @startDate");
            bSql.Append(" AND u.LastActivityDate < @endDate");
            bSql.Append(" AND u.LastActivityDate < @thirtMinAgo");

            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(bSql.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "@startDate", System.Data.DbType.DateTime, startDate);
            database.AddInParameter(selectCommand, "@endDate", System.Data.DbType.DateTime, endDate);
            //TODO : instead of hard coding 30 minutes preferably get this from store configuration
            DateTime thirtyMinAgo = LocaleHelper.LocalNow.AddMinutes(-30);
            if (thirtyMinAgo.Kind != DateTimeKind.Utc)
            {
                thirtyMinAgo = LocaleHelper.FromLocalTime(thirtyMinAgo);
            }

            database.AddInParameter(selectCommand, "@thirtMinAgo", System.Data.DbType.DateTime, thirtyMinAgo);

            int basketCount = 0;
            LSDecimal totalAmount = 0;
            int thisBasketId;
            BasketItemCollection items;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    thisBasketId = dr.GetInt32(0);
                    items = BasketItemDataSource.LoadForBasket(thisBasketId);
                    if (items.Count > 0)
                    {
                        totalAmount += items.TotalPrice();
                        basketCount++;
                    }
                }
                dr.Close();
            }

            summary.BasketCount = basketCount;
            summary.Total = totalAmount;

            return summary;
        }

        /// <summary>
        /// Gets abandoned basket summary report for given day
        /// </summary>
        /// <param name="year">The year to consider</param>
        /// <param name="month">The month to consider</param>
        /// <param name="day">The day to consider</param>
        /// <returns>A list of AbandonedBasketSummary objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<BasketSummary> GetDailyAbandonedBaskets(int year, int month, int day)
        {
            if ((month < 1) || (month > 12)) throw new ArgumentOutOfRangeException("Month must fall between 1 and 12.", "month");
            List<BasketSummary> results = new List<BasketSummary>();
            //CALCULATE START DATE
            DateTime startDate = new DateTime(year, month, day, 0, 0, 0, 0, DateTimeKind.Unspecified);
            //CONVERT DATE TO UTC
            startDate = LocaleHelper.FromLocalTime(startDate);
            //CALCULATE END DATE
            DateTime endDate = startDate.AddDays(1);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);
            //TODO : calculate the daily abandoned baskets
            StringBuilder bSql = new StringBuilder();
            bSql.Append("SELECT b.BasketId, u.UserName, u.LastActivityDate FROM ac_Baskets b INNER JOIN ac_Users u on b.UserId=u.UserId ");
            bSql.Append(" WHERE u.StoreId = @storeId");
            bSql.Append(" AND u.LastActivityDate >= @startDate");
            bSql.Append(" AND u.LastActivityDate <= @endDate");
            bSql.Append(" AND u.LastActivityDate < @thirtMinAgo");

            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(bSql.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "@startDate", System.Data.DbType.DateTime, startDate);
            database.AddInParameter(selectCommand, "@endDate", System.Data.DbType.DateTime, endDate);
            //TODO : instead of hard coding 30 minutes preferably get this from store configuration
            DateTime thirtyMinAgo = LocaleHelper.LocalNow.AddMinutes(-30);
            if (thirtyMinAgo.Kind != DateTimeKind.Utc)
            {
                thirtyMinAgo = LocaleHelper.FromLocalTime(thirtyMinAgo);
            }

            database.AddInParameter(selectCommand, "@thirtMinAgo", System.Data.DbType.DateTime, thirtyMinAgo);

            BasketSummary summary;
            int thisBasketId;
            BasketItemCollection items;
            string userName;
            DateTime lastActivity;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    thisBasketId = dr.GetInt32(0);
                    items = BasketItemDataSource.LoadForBasket(thisBasketId);
                    if (items.Count > 0)
                    {
                        userName = dr.GetString(1);
                        lastActivity = LocaleHelper.ToLocalTime(NullableData.GetDateTime(dr, 2));
                        if (AlwaysConvert.ToInt(userName) == thisBasketId)
                        {
                            userName = "ANONYMOUS";
                        }
                        summary = new BasketSummary();
                        summary.BasketId = thisBasketId;
                        summary.BasketTotal = items.TotalPrice();
                        summary.Customer = userName;
                        summary.ItemCount = items.Count;
                        summary.LastActivity = lastActivity;
                        results.Add(summary);
                    }
                }
                dr.Close();
            }

            return results;
        }
        
        private static int GetDaysInMonth(int month, int year)
        {
            return (new DateTime(year, month, 1).AddMonths(1).AddDays(-1)).Day;
        }

        #region Affiliate Sales

        /// <summary>
        /// Builds sales by affiliate report for given period
        /// </summary>
        /// <param name="startDate">Inclusive start date to consider</param>
        /// <param name="endDate">Inclusive end date to consider</param>
        /// <param name="affiliateId">The affiliate to build the report for</param>
        /// <returns>A List of AffiliateSalesSummary objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<AffiliateSalesSummary> GetSalesByAffiliate(DateTime startDate, DateTime endDate, int affiliateId)
        {
            //GET ORDER TOTALS BY AFFILIATE
            Dictionary<int, AffiliateSalesSummary> summaries = new Dictionary<int, AffiliateSalesSummary>();
            //GET INVALID ORDER STATUSES
            List<OrderStatus> reportStatuses = OrderStatusDataSource.GetReportStatuses();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT AffiliateId, COUNT(*) AS OrderCount, SUM(ProductSubtotal) AS OrderSubtotal, SUM(TotalCharges) AS OrderTotal");
            selectQuery.Append(" FROM ac_Orders");
            selectQuery.Append(" WHERE StoreId = @storeId");
            if (affiliateId == 0) selectQuery.Append(" AND AffiliateId IS NOT NULL");
            else selectQuery.Append(" AND AffiliateId = @affiliateId");
            selectQuery.Append(" AND " + GetStatusFilter(reportStatuses));
            if (startDate > DateTime.MinValue)
            {
                //CONVERT DATE TO UTC
                selectQuery.Append(" AND OrderDate >= @startDate");
            }
            if (endDate > DateTime.MinValue)
            {
                //CONVERT DATE TO UTC
                selectQuery.Append(" AND OrderDate <= @endDate");
            }
            selectQuery.Append(" GROUP BY AffiliateId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            if (affiliateId > 0) database.AddInParameter(selectCommand, "@affiliateId", System.Data.DbType.Int32, affiliateId);
            SetStatusFilterParams(reportStatuses, database, selectCommand);
            if (startDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@startDate", System.Data.DbType.DateTime, LocaleHelper.FromLocalTime(startDate));
            if (endDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@endDate", System.Data.DbType.DateTime, LocaleHelper.FromLocalTime(endDate));
            //EXECUTE THE COMMAND
            AffiliateSalesSummary summary;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    summary = new AffiliateSalesSummary(dr.GetInt32(0), startDate, endDate, dr.GetInt32(1), dr.GetDecimal(2), dr.GetDecimal(3));
                    summaries.Add(summary.AffiliateId, summary);
                }
                dr.Close();
            }

            List<AffiliateSalesSummary> sortedResults = new List<AffiliateSalesSummary>();
            if (affiliateId > 0)
            {
                Affiliate affiliate = AffiliateDataSource.Load(affiliateId);
                if (summaries.ContainsKey(affiliateId)) summary = summaries[affiliate.AffiliateId];
                //BUILD AN EMPTY SUMMARY IF NO RESULTS FOUND
                else summary = new AffiliateSalesSummary(affiliate.AffiliateId, startDate, endDate, 0, 0, 0);
                summary.Affiliate = affiliate;
                summary.AffiliateName = affiliate.Name;
                sortedResults.Add(summary);
            }
            else
            {
                //GET ALL AFFILIATES ORDERED BY NAME
                AffiliateCollection affiliates = AffiliateDataSource.LoadForStore("Name");
                foreach (Affiliate affiliate in affiliates)
                {
                    if (summaries.ContainsKey(affiliate.AffiliateId)) summary = summaries[affiliate.AffiliateId];
                    //BUILD AN EMPTY SUMMARY IF NO RESULTS FOUND
                    else summary = new AffiliateSalesSummary(affiliate.AffiliateId, startDate, endDate, 0, 0, 0);
                    summary.Affiliate = affiliate;
                    summary.AffiliateName = affiliate.Name;
                    sortedResults.Add(summary);
                }
            }

            //RETURN THE SORTED RESULTS
            return sortedResults;
        }

        /// <summary>
        /// Gets the record count for sales by affiliate report for given period
        /// </summary>
        /// <param name="startDate">Inclusive start date to consider</param>
        /// <param name="endDate">Inclusive end date to consider</param>
        /// <returns>The record count for sales by affiliate report for given period</returns>
        public static int GetSalesByAffiliateCount(DateTime startDate, DateTime endDate)
        {
            List<OrderStatus> reportStatuses = OrderStatusDataSource.GetReportStatuses();
            Database database = Token.Instance.Database;
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT COUNT(*) AS AffiliateCount FROM (SELECT DISTINCT AffiliateId FROM ac_Orders");
            selectQuery.Append(" WHERE StoreId = @storeId");
            selectQuery.Append(" AND AffiliateId IS NOT NULL");
            selectQuery.Append(" AND " + GetStatusFilter(reportStatuses));
            if (startDate > DateTime.MinValue)
            {
                //CONVERT DATE TO UTC
                startDate = LocaleHelper.FromLocalTime(startDate);
                selectQuery.Append(" AND OrderDate >= @startDate");
            }
            if (endDate > DateTime.MinValue)
            {
                //CONVERT DATE TO UTC
                endDate = LocaleHelper.FromLocalTime(endDate);
                selectQuery.Append(" AND OrderDate <= @endDate");
            }
            selectQuery.Append(") AS UniqueAffiliates");
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            SetStatusFilterParams(reportStatuses, database, selectCommand);
            if (startDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@startDate", System.Data.DbType.DateTime, startDate);
            if (endDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@endDate", System.Data.DbType.DateTime, endDate);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        #endregion

        /// <summary>
        /// Gets a list of currently active users
        /// </summary>
        /// <param name="activityThreshold">Number of minutes for the last activity by the user beyond which the user is considered inactive</param>
        /// <returns>A list of currently active users</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static UserCollection GetActiveUsers(int activityThreshold)
        {
            return GetActiveUsers(activityThreshold, 0, 0, string.Empty);
        }

        /// <summary>
        /// Gets a list of currently active users
        /// </summary>
        /// <param name="activityThreshold">Number of minutes for the last activity by the user beyond which the user is considered inactive</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>A list of currently active users</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static UserCollection GetActiveUsers(int activityThreshold, string sortExpression)
        {
            return GetActiveUsers(activityThreshold, 0, 0, sortExpression);
        }

        /// <summary>
        /// Gets a list of currently active users
        /// </summary>
        /// <param name="activityThreshold">Number of minutes for the last activity by the user beyond which the user is considered inactive</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>A list of currently active users</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static UserCollection GetActiveUsers(int activityThreshold, int maximumRows, int startRowIndex, string sortExpression)
        {
            string sqlCriteria = " StoreId = {0} AND LastActivityDate > '{1}'";
            DateTime thresholdDt = LocaleHelper.LocalNow.AddMinutes(-1 * activityThreshold);
            if (thresholdDt.Kind != DateTimeKind.Utc)
            {
                thresholdDt = LocaleHelper.FromLocalTime(thresholdDt);
            }
            sqlCriteria = string.Format(sqlCriteria, Token.Instance.StoreId.ToString(), thresholdDt.ToString("MM/dd/yyyy HH:mm:ss tt"));

            //DEFAULT SORT EXPRESSION
            if (string.IsNullOrEmpty(sortExpression)) sortExpression = "LastActivityDate DESC";

            UserCollection results = UserDataSource.LoadForCriteria(sqlCriteria, maximumRows, startRowIndex, sortExpression);
            return results;
        }

        /// <summary>
        /// Gets number of users currently active
        /// </summary>
        /// <param name="activityThreshold">Number of minutes for the last activity by the user beyond which the user is considered inactive</param>
        /// <returns>Number of users currently active</returns>
        public static int GetActiveUsersCount(int activityThreshold)
        {
            string sqlCriteria = " StoreId = {0} AND LastActivityDate > '{1}'";
            DateTime thresholdDt = LocaleHelper.LocalNow.AddMinutes(-1 * activityThreshold);
            if (thresholdDt.Kind != DateTimeKind.Utc)
            {
                thresholdDt = LocaleHelper.FromLocalTime(thresholdDt);
            }
            sqlCriteria = string.Format(sqlCriteria, Token.Instance.StoreId.ToString(), thresholdDt.ToString("MM/dd/yyyy HH:mm:ss tt"));

            return UserDataSource.CountForCriteria(sqlCriteria);
        }

        /// <summary>
        /// Gets sales totals for the past months
        /// </summary>
        /// <param name="monthCount">Can be from 1 to 12</param>
        /// <param name="includeZeros">Whether to include zero amount sales or not</param>
        /// <returns>A collection of Key-Value pairs with each key representing the date(month) and each value representing the total sale</returns>
        public static SortableCollection<KeyValuePair<DateTime, decimal>> GetSalesForPastMonths(int monthCount, bool includeZeros)
        {
            if ((monthCount < 0) || (monthCount > 12)) throw new ArgumentOutOfRangeException("monthCount", "monthCount must be between 1 and 12");
            //DETERMINE THE START MONTH
            monthCount -= 1;
            DateTime localNow = LocaleHelper.LocalNow;
            DateTime startDate = localNow.AddMonths(-1 * monthCount);
            startDate = new DateTime(startDate.Year, startDate.Month, 1, 0, 0, 0);
            startDate = LocaleHelper.FromLocalTime(startDate);
            SortableCollection<KeyValuePair<DateTime, decimal>> sparseArray = new SortableCollection<KeyValuePair<DateTime, decimal>>();
            //FIX THE HOURS FOR THE TIMEZONE OFFSET
            //THIS IS IMPERFECT BECAUSE WE CANNOT TAKE INTO ACCOUNT PARTIAL HOURS
            //SO CAST THE TIMEZONE OFFSET TO AN INTEGER
            int hourOffset = (int)Token.Instance.Store.TimeZoneOffset;
            //GET INVALID ORDER STATUSES
            List<OrderStatus> reportStatuses = OrderStatusDataSource.GetReportStatuses();
            //BUILD QUERY TO GET SALES ARRANGED BY MONTH
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT DATEPART(mm, DATEADD(hh, " + hourOffset + ", OrderDate)) AS OrderMonth, SUM(TotalCharges) AS MonthlySales");
            sql.Append(" FROM ac_Orders");
            sql.Append(" WHERE StoreId = @storeId");
            sql.Append(" AND " + GetStatusFilter(reportStatuses));
            sql.Append(" AND OrderDate >= @startDate");
            sql.Append(" GROUP BY DATEPART(mm, DATEADD(hh, " + hourOffset + ", OrderDate))");
            //LOAD TABLE OF ORDERITEM TOTALS
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(sql.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            SetStatusFilterParams(reportStatuses, database, selectCommand);
            database.AddInParameter(selectCommand, "@startDate", System.Data.DbType.DateTime, startDate);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    int month = dr.GetInt32(0);
                    int year = startDate.Year;
                    if (month < startDate.Month) year += 1;
                    DateTime key = new DateTime(year, month, 1, 0, 0, 0);
                    sparseArray.Add(new KeyValuePair<DateTime, decimal>(key, dr.GetDecimal(1)));
                }
                dr.Close();
            }
            //SORT THE RESULTS BY MONTH ASCENDING
            sparseArray.Sort("Key");
            if (!includeZeros) return sparseArray;
            //MAKE A FULL ARRAY
            int sparseIndex = 0;
            SortableCollection<KeyValuePair<DateTime, decimal>> fullArray = new SortableCollection<KeyValuePair<DateTime, decimal>>();
            int currentMonth = startDate.Month;
            int currentYear = startDate.Year;
            for (int i = 0; i <= monthCount; i++)
            {
                DateTime key = new DateTime(currentYear, currentMonth, 1);
                if ((sparseIndex < sparseArray.Count) && (sparseArray[sparseIndex].Key == key))
                {
                    fullArray.Add(sparseArray[sparseIndex]);
                    sparseIndex++;
                }
                else
                {
                    fullArray.Add(new KeyValuePair<DateTime, decimal>(key, 0M));
                }
                currentMonth += 1;
                if (currentMonth > 12)
                {
                    currentMonth -= 12;
                    currentYear++;
                }
            }
            return fullArray;
        }

        /// <summary>
        /// Gets sales totals for the past days
        /// </summary>
        /// <param name="dayCount">Can be between 1 to 15</param>
        /// <param name="includeZeros">Whether to include zero amount sales or not</param>
        /// <returns>A collection of Key-Value pairs with each key representing the date(day) and each value representing the total sale</returns>
        public static SortableCollection<KeyValuePair<DateTime, decimal>> GetSalesForPastDays(int dayCount, bool includeZeros)
        {
            if ((dayCount < 0) || (dayCount > 15)) throw new ArgumentOutOfRangeException("dayCount", "dayCount must be between 1 and 15");
            //DETERMINE THE START MONTH
            dayCount -= 1;
            DateTime localNow = LocaleHelper.LocalNow;
            DateTime startDate = localNow.AddDays(-1 * dayCount);
            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            SortableCollection<KeyValuePair<DateTime, decimal>> sparseArray = new SortableCollection<KeyValuePair<DateTime, decimal>>();
            //FIX THE HOURS FOR THE TIMEZONE OFFSET
            //THIS IS IMPERFECT BECAUSE WE CANNOT TAKE INTO ACCOUNT PARTIAL HOURS
            //SO CAST THE TIMEZONE OFFSET TO AN INTEGER
            int hourOffset = (int)Token.Instance.Store.TimeZoneOffset;
            //GET INVALID ORDER STATUSES
            List<OrderStatus> reportStatuses = OrderStatusDataSource.GetReportStatuses();
            //BUILD QUERY TO GET SALES ARRANGED BY MONTH
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT DATEPART(dd, DATEADD(hh, " + hourOffset + ", OrderDate)) AS OrderDay, SUM(TotalCharges) AS DaylySales");
            sql.Append(" FROM ac_Orders");
            sql.Append(" WHERE StoreId = @storeId");
            sql.Append(" AND " + GetStatusFilter(reportStatuses));
            sql.Append(" AND OrderDate >= @startDate");
            sql.Append(" GROUP BY DATEPART(dd, DATEADD(hh, " + hourOffset + ", OrderDate))");
            //LOAD TABLE OF ORDERITEM TOTALS
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(sql.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            SetStatusFilterParams(reportStatuses, database, selectCommand);
            database.AddInParameter(selectCommand, "@startDate", System.Data.DbType.DateTime, LocaleHelper.FromLocalTime(startDate));
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    int day = dr.GetInt32(0);
                    int month = startDate.Month;
                    int year = startDate.Year;
                    if (day < startDate.Day) month += 1;
                    if (month > 12)
                    {
                        month -= 12;
                        year += 1;
                    }
                    DateTime key = new DateTime(year, month, day, 0, 0, 0);
                    sparseArray.Add(new KeyValuePair<DateTime, decimal>(key, dr.GetDecimal(1)));
                }
                dr.Close();
            }
            //SORT THE RESULTS BY MONTH ASCENDING
            sparseArray.Sort("Key");
            if (!includeZeros) return sparseArray;
            //MAKE A FULL ARRAY
            int sparseIndex = 0;
            SortableCollection<KeyValuePair<DateTime, decimal>> fullArray = new SortableCollection<KeyValuePair<DateTime, decimal>>();
            DateTime dateIndex = startDate;
            for (int i = 0; i <= dayCount; i++)
            {
                if ((sparseIndex < sparseArray.Count) && (sparseArray[sparseIndex].Key == dateIndex))
                {
                    fullArray.Add(sparseArray[sparseIndex]);
                    sparseIndex++;
                }
                else
                {
                    fullArray.Add(new KeyValuePair<DateTime, decimal>(dateIndex, 0M));
                }
                dateIndex = dateIndex.AddDays(1);
            }
            return fullArray;
        }

        /// <summary>
        /// Builds a product breakdown summary report for a given period
        /// </summary>
        /// <param name="fromDate">Inclusive start date to consider</param>
        /// <param name="toDate">Inclusive end date to consider</param>
        /// <param name="vendorId">If vendorId is greater than 0 includes products for this vendor only. All products are included otherwise.</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>A List of ProductBreakdownSummary objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<ProductBreakdownSummary> GetProductBreakdownSummary(DateTime fromDate, DateTime toDate, int vendorId, string sortExpression)
        {
            //GET INVALID ORDER STATUSES
            List<OrderStatus> reportStatuses = OrderStatusDataSource.GetReportStatuses();
            List<ProductBreakdownSummary> breakdownReport = new List<ProductBreakdownSummary>();
            StringBuilder sqlBuilder = new StringBuilder();
            
            sqlBuilder.Append(" SELECT ProductId,SUM(Quantity) AS Quantity,SUM((Price*Quantity)) AS Amount");
            sqlBuilder.Append(" FROM ac_OrderItems ");
            sqlBuilder.Append(" WHERE OrderId in (SELECT OrderId FROM ac_Orders ");
            sqlBuilder.Append(" WHERE " + GetStatusFilter(reportStatuses));
            if (fromDate != DateTime.MinValue || toDate != DateTime.MinValue)
            {
                sqlBuilder.Append(" AND ");
                if (fromDate != DateTime.MinValue)
                {
                    fromDate = LocaleHelper.FromLocalTime(fromDate);
                    sqlBuilder.Append(" OrderDate >= @FD ");
                    if (toDate != DateTime.MinValue)
                    {
                        sqlBuilder.Append(" AND ");    
                    }

                }
                if (toDate != DateTime.MinValue)
                {                    
                    sqlBuilder.Append(" OrderDate <= @TD ");
                    toDate = LocaleHelper.FromLocalTime(toDate);
                }
            }
            sqlBuilder.Append(" ) ");
            sqlBuilder.Append(" GROUP BY ProductId ");
            sqlBuilder.Append(" ORDER BY ");
            sqlBuilder.Append(sortExpression.Trim());
            sqlBuilder.Append(" DESC ");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(sqlBuilder.ToString());
            SetStatusFilterParams(reportStatuses, database, selectCommand);
            if (fromDate != DateTime.MinValue)
                database.AddInParameter(selectCommand, "@FD", System.Data.DbType.DateTime, fromDate);
            if (toDate != DateTime.MinValue)
                database.AddInParameter(selectCommand, "@TD", System.Data.DbType.DateTime, toDate);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                ProductBreakdownSummary breakdownSummary = null;
                string productId;
                string quantity;
                string amount;
                while (dr.Read())
                {
                    productId = string.Empty;
                    quantity = string.Empty;
                    amount = string.Empty;
                    breakdownSummary = new ProductBreakdownSummary();
                    productId = (dr["ProductId"] == null) ? String.Empty : dr["ProductId"].ToString();
                    quantity = (dr["Quantity"] == null) ? String.Empty : dr["Quantity"].ToString();
                    amount = (dr["Amount"] == null) ? String.Empty : dr["Amount"].ToString();
                    if (String.IsNullOrEmpty(productId)) continue;
                    CommerceBuilder.Products.Product product = null;
                    product = CommerceBuilder.Products.ProductDataSource.Load(AlwaysConvert.ToInt(productId));
                    if (product == null) continue;
                    if (vendorId > 0)
                    {
                        if (product.Vendor != null && !String.IsNullOrEmpty(product.Vendor.Name))
                        {
                            if (product.Vendor.VendorId == vendorId)
                            {
                                breakdownSummary.ProductId = AlwaysConvert.ToInt(productId);
                                breakdownSummary.Name = (product != null) ? product.Name : string.Empty;
                                breakdownSummary.Sku = (product != null) ? product.Sku : string.Empty;
                                breakdownSummary.Vendor = (product.Vendor != null) ? product.Vendor.Name : string.Empty;
                                breakdownSummary.Quantity = AlwaysConvert.ToInt(quantity);
                                breakdownSummary.Amount = AlwaysConvert.ToDecimal(amount);
                                breakdownReport.Add(breakdownSummary);
                            }
                        }
                    }
                    else
                    {
                        breakdownSummary.ProductId = AlwaysConvert.ToInt(productId);
                        breakdownSummary.Name = (product != null) ? product.Name : string.Empty;
                        breakdownSummary.Sku = (product != null) ? product.Sku : string.Empty;
                        breakdownSummary.Vendor = (product.Vendor != null) ? product.Vendor.Name : string.Empty;
                        breakdownSummary.Quantity = AlwaysConvert.ToInt(quantity);
                        breakdownSummary.Amount = AlwaysConvert.ToDecimal(amount);
                        breakdownReport.Add(breakdownSummary);
                    }
                }
                dr.Close();
            }
            return breakdownReport;
        }

        /// <summary>
        /// Builds report data for sales by referrer
        /// </summary>
        /// <param name="startDate">Inclusive start date to consider</param>
        /// <param name="endDate">Inclusive end date to consider</param>
        /// <returns>A List of ReferrerSalesSummary objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<ReferrerSalesSummary> GetSalesByReferrer(DateTime startDate, DateTime endDate)
        {
            return GetSalesByReferrer(startDate, endDate, 0, 0, string.Empty);
        }

        /// <summary>
        /// Builds report data for sales by referrer
        /// </summary>
        /// <param name="startDate">Inclusive start date to consider</param>
        /// <param name="endDate">Inclusive end date to consider</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>A List of ReferrerSalesSummary objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<ReferrerSalesSummary> GetSalesByReferrer(DateTime startDate, DateTime endDate, string sortExpression)
        {
            return GetSalesByReferrer(startDate, endDate, 0, 0, sortExpression);
        }

        /// <summary>
        /// Builds report data for sales by referrer
        /// </summary>
        /// <param name="startDate">Inclusive start date to consider</param>
        /// <param name="endDate">Inclusive end date to consider</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>A List of ReferrerSalesSummary objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<ReferrerSalesSummary> GetSalesByReferrer(DateTime startDate, DateTime endDate, int maximumRows, int startRowIndex, string sortExpression)
        {
            // CALCULATE CURRENT DOMAIN/HOST NAME, WE NEED TO EXCLUDE LOCAL RESULTS
            String urlStemToIgnore = String.Empty;
            HttpRequest request = HttpContextHelper.SafeGetRequest();
            if (request != null)
            {
                string port = request.ServerVariables["SERVER_PORT"];
                if (port == null || port == "80" || port == "443")
                    port = "";
                else
                    port = ":" + port;

                urlStemToIgnore = request.ServerVariables["SERVER_NAME"] +
                          port + request.ApplicationPath;
            }

            //GET INVALID ORDER STATUSES
            List<OrderStatus> reportStatuses = OrderStatusDataSource.GetReportStatuses();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" Referrer, COUNT(*) AS OrderCount, SUM(ProductSubtotal) AS ProductSubtotal, SUM(TotalCharges) AS OrderTotal");
            selectQuery.Append(" FROM ac_Orders");
            selectQuery.Append(" WHERE StoreId = @storeId");
            selectQuery.Append(" AND Referrer IS NOT NULL");
            selectQuery.Append(" AND " + GetStatusFilter(reportStatuses));
            if (startDate > DateTime.MinValue)
            {
                //CONVERT DATE TO UTC
                selectQuery.Append(" AND OrderDate >= @startDate");
            }
            if (endDate > DateTime.MinValue)
            {
                //CONVERT DATE TO UTC
                selectQuery.Append(" AND OrderDate <= @endDate");
            }
            if (!string.IsNullOrEmpty(urlStemToIgnore)) selectQuery.Append(" AND Referrer NOT LIKE @currentDomain");
            selectQuery.Append(" GROUP BY Referrer");
            if (string.IsNullOrEmpty(sortExpression)) sortExpression = "OrderCount DESC";
            selectQuery.Append(" ORDER BY " + sortExpression);

            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            SetStatusFilterParams(reportStatuses, database, selectCommand);
            if (startDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@startDate", System.Data.DbType.DateTime, LocaleHelper.FromLocalTime(startDate));
            if (endDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@endDate", System.Data.DbType.DateTime, LocaleHelper.FromLocalTime(endDate));
            if (!string.IsNullOrEmpty(urlStemToIgnore)) database.AddInParameter(selectCommand, "@currentDomain", System.Data.DbType.String, "%" + urlStemToIgnore + "%");
            
            //EXECUTE THE COMMAND
            int thisIndex = 0;
            int rowCount = 0;
            ReferrerSalesSummary summary;
            List<ReferrerSalesSummary> sortedResults = new List<ReferrerSalesSummary>();
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        summary = new ReferrerSalesSummary(dr.GetString(0), startDate, endDate, dr.GetInt32(1), dr.GetDecimal(2), dr.GetDecimal(3));
                        sortedResults.Add(summary);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }

            //RETURN THE SORTED RESULTS
            return sortedResults;

        }

        /// <summary>
        /// Get report data for user purchase history report
        /// </summary>
        /// <param name="userId">User Id to get report data for</param>
        /// <param name="forPaidOrders">if true only paid orders, otherwise only unpaid orders will be considered</param>
        /// <returns>A List of PurchaseSummary objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<PurchaseSummary> GetUserPurchaseHistory(int userId, bool forPaidOrders)
        {
            return GetUserPurchaseHistory(userId, forPaidOrders, 0, 0, string.Empty);
        }

        /// <summary>
        /// Get report data for user purchase history report
        /// </summary>
        /// <param name="userId">User Id to get report data for</param>
        /// <param name="forPaidOrders">if true only paid orders, otherwise only unpaid orders will be considered</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>A List of PurchaseSummary objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<PurchaseSummary> GetUserPurchaseHistory(int userId, bool forPaidOrders, string sortExpression)
        {
            return GetUserPurchaseHistory(userId, forPaidOrders, 0, 0, sortExpression);
        }
        
        /// <summary>
        /// Get report data for user purchase history report
        /// </summary>
        /// <param name="userId">User Id to get report data for</param>
        /// <param name="forPaidOrders">if true only paid orders, otherwise only unpaid orders will be considered</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>A List of PurchaseSummary objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<PurchaseSummary> GetUserPurchaseHistory(int userId, bool forPaidOrders, int maximumRows, int startRowIndex, string sortExpression)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT DISTINCT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" O.OrderId as OrderId, O.OrderNumber as OrderNumber, O.OrderDate as OrderDate, OI.Name as Name, OI.Price as Price, OI.Quantity as Quantity, (OI.Price * OI.Quantity) as Total");
            selectQuery.Append(" FROM ac_Orders O INNER JOIN ac_OrderItems OI ON O.OrderId = OI.OrderId");
            selectQuery.Append(" WHERE O.StoreId = @storeId AND O.UserId=@userId AND OI.OrderItemTypeId = ").Append((byte)OrderItemType.Product);
            selectQuery.Append(" AND O.PaymentStatusId = @paymentStatus");

            OrderStatusCollection statuses = OrderStatusDataSource.LoadValidOrderStatuses();
            if (statuses.Count > 0)
            {
                selectQuery.Append(" AND O.OrderStatusId IN ( ");
                foreach (OrderStatus status in statuses)
                    selectQuery.Append(status.OrderStatusId.ToString() + ",");
                selectQuery.Remove(selectQuery.Length - 1, 1);
                selectQuery.Append(")");
            }
            

            if (string.IsNullOrEmpty(sortExpression)) sortExpression = "O.OrderNumber DESC";
            else selectQuery.Append(" ORDER BY " + sortExpression);

            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "@userId", System.Data.DbType.Int32, userId);

            if (forPaidOrders) database.AddInParameter(selectCommand, "@paymentStatus", System.Data.DbType.Byte, OrderPaymentStatus.Paid);
            else database.AddInParameter(selectCommand, "@paymentStatus", System.Data.DbType.Byte, OrderPaymentStatus.Unpaid);

            //EXECUTE THE COMMAND
            int thisIndex = 0;
            int rowCount = 0;
            PurchaseSummary summary;
            List<PurchaseSummary> sortedResults = new List<PurchaseSummary>();
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        summary = new PurchaseSummary(dr.GetInt32(0), dr.GetInt32(1), dr.GetDateTime(2), dr.GetString(3), dr.GetDecimal(4), dr.GetInt16(5), dr.GetDecimal(6));
                        sortedResults.Add(summary);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }

            return sortedResults;
        }
        /// <summary>
        /// Calculates the different purchase totals against user's purchase history data.
        /// </summary>
        /// <param name="purchaseSummaries">List of PurchaseSummary items returned by Commercebuilder.Reporting.ReportDataSource.GetUserPurchaseHistory method.</param>
        /// <returns>Returns summary of totlas for user's purchase history</returns>
        public static PurchaseTotalSummary CalculatePurchaseHistoryTotals(List<PurchaseSummary> purchaseSummaries)
        {
            PurchaseTotalSummary purchaseTotalSummary = new PurchaseTotalSummary();

            foreach (PurchaseSummary summary in purchaseSummaries)
            {
                if (purchaseTotalSummary.OrderIds.Contains(summary.OrderId))
                    continue;
                Order order = OrderDataSource.Load(summary.OrderId);

                if (purchaseTotalSummary.FirstOrderDate == DateTime.MinValue)
                    purchaseTotalSummary.FirstOrderDate = order.OrderDate;

                LSDecimal discounts = 0;
                LSDecimal coupons = 0;
                LSDecimal prodcutCoupons = 0;
                LSDecimal shippings = 0;
                LSDecimal taxes = 0;
                LSDecimal costOfGoods = 0;
                LSDecimal others = 0;
                LSDecimal grossProduct = 0;

                foreach (OrderItem orderItem in order.Items)
                {
                    switch (orderItem.OrderItemType)
                    {
                        case OrderItemType.Discount:
                            discounts += orderItem.ExtendedPrice;
                            break;

                        case OrderItemType.Coupon:
                            coupons += orderItem.ExtendedPrice;
                            if (orderItem.GetParentItem(false).OrderItemType == OrderItemType.Product)
                                prodcutCoupons += orderItem.ExtendedPrice;
                            break;

                        case OrderItemType.Shipping:
                        case OrderItemType.Handling:
                            shippings += orderItem.NetExtendedPrice;
                            break;
                        case OrderItemType.Tax:
                            taxes += orderItem.ExtendedPrice;
                            break;
                        case OrderItemType.Product:
                            grossProduct += orderItem.ExtendedPrice;
                            costOfGoods += orderItem.CostOfGoods;
                            break;
                        default:
                            others += orderItem.ExtendedPrice;
                            break;
                    }
                }

                purchaseTotalSummary.DiscountsTotal += discounts;
                purchaseTotalSummary.CouponsTotal += coupons;
                purchaseTotalSummary.ProductCouponsTotal += prodcutCoupons;
                purchaseTotalSummary.ShippingTotal += shippings;
                purchaseTotalSummary.TaxesTotal += taxes;
                purchaseTotalSummary.GrossProductsTotal += grossProduct;
                purchaseTotalSummary.CostOfGoodsSoldTotal += costOfGoods;
                purchaseTotalSummary.OtherTotal += others;
                purchaseTotalSummary.TotalCharges += order.TotalCharges;
                purchaseTotalSummary.PaidTotal += order.Payments.TotalProcessed();
                purchaseTotalSummary.UnpaidTotal += order.Payments.TotalUnprocessed();
                purchaseTotalSummary.OrderIds.Add(order.OrderId);
            }

            return purchaseTotalSummary;
        }
    }
}
