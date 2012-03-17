//-----------------------------------------------------------------------
// <copyright file="OrderFilter.cs" company="Able Solutions Corporation">
//     Copyright (c) 2009 Able Solutions Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace CommerceBuilder.Search
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Text;
    using CommerceBuilder.Common;
    using CommerceBuilder.Data;
    using CommerceBuilder.Orders;
    using CommerceBuilder.Reporting;
    using CommerceBuilder.Stores;
    using CommerceBuilder.Utility;

    /// <summary>
    /// Class representing the filter to apply to the order table
    /// </summary>
    [Serializable]
    public class OrderFilter
    {
        private DateTime _OrderDateStart;
        private DateTime _OrderDateEnd;
        private string _OrderIdRange;
        private string _OrderNumberRange;
        private IdList _OrderStatus;
        private OrderPaymentStatus _PaymentStatus;
        private OrderShipmentStatus _ShipmentStatus;
        private string _Keyword;
        private KeywordFieldType _KeywordField;

        /// <summary>
        /// Defines the keyword fields supported by this filter
        /// </summary>
        [Flags]
        public enum KeywordFieldType
        {
            /// <summary>
            /// Billing fields such as BillToName, BillToEmail, etc.
            /// </summary>
            BillingInfo = 0x1,

            /// <summary>
            /// Shipping fields such as ShipToName, ShipToEmail, etc.
            /// </summary>
            ShippingInfo = 0x2,

            /// <summary>
            /// Order notes
            /// </summary>
            Notes = 0x4,

            /// <summary>
            /// All relevant text columns
            /// </summary>
            All = BillingInfo | ShippingInfo | Notes
        }

        /// <summary>
        /// Initializes a new instance of the OrderFilter class.
        /// </summary>
        public OrderFilter()
        {
            _OrderDateStart = DateTime.MinValue;
            _OrderDateEnd = DateTime.MaxValue;
            _OrderIdRange = string.Empty;
            _OrderNumberRange = string.Empty;
            _PaymentStatus = OrderPaymentStatus.Unspecified;
            _ShipmentStatus = OrderShipmentStatus.Unspecified;
            _OrderStatus = new IdList();
            _Keyword = string.Empty;
            _KeywordField = KeywordFieldType.All;
        }

        /// <summary>
        /// Gets or sets the order start date for the result set
        /// </summary>
        public DateTime OrderDateStart
        {
            get { return _OrderDateStart; }
            set { _OrderDateStart = value; }
        }

        /// <summary>
        /// Gets or sets the order end date for the result set
        /// </summary>
        public DateTime OrderDateEnd
        {
            get { return _OrderDateEnd; }
            set { _OrderDateEnd = value; }
        }

        /// <summary>
        /// Gets or sets the order ID range for the result set
        /// </summary>
        public string OrderIdRange
        {
            get { return _OrderIdRange; }
            set { _OrderIdRange = value; }
        }

        /// <summary>
        /// Gets or sets the order number range for the result set
        /// </summary>
        public string OrderNumberRange
        {
            get { return _OrderNumberRange; }
            set { _OrderNumberRange = value; }
        }

        /// <summary>
        /// Gets or sets the payment status of orders to search
        /// </summary>
        public OrderPaymentStatus PaymentStatus
        {
            get { return _PaymentStatus; }
            set { _PaymentStatus = value; }
        }

        /// <summary>
        /// Gets or sets the shipment status of orders to search
        /// </summary>
        public OrderShipmentStatus ShipmentStatus
        {
            get { return _ShipmentStatus; }
            set { _ShipmentStatus = value; }
        }

        /// <summary>
        /// Gets the collection of order statuses to search for
        /// </summary>
        public IdList OrderStatus
        {
            get { return this._OrderStatus; }
        }

        /// <summary>
        /// Gets or sets the search keyword(s)
        /// </summary>
        public string Keyword
        {
            get { return this._Keyword; }
            set { this._Keyword = value; }
        }

        /// <summary>
        /// Gets or sets the field(s) to search for the keyword
        /// </summary>
        public KeywordFieldType KeywordField
        {
            get { return this._KeywordField; }
            set { this._KeywordField = value; }
        }

        /// <summary>
        /// Builds a command to count the total records that are within the bounds of the filter
        /// </summary>
        /// <returns>A DbCommand object that will return the count of records for the filter</returns>
        public int Count()
        {
            DbCommand countCommand = GetFilteredOrderIds(0, 0, string.Empty, true);
            int orderCount = AlwaysConvert.ToInt(Token.Instance.Database.ExecuteScalar(countCommand));
            return orderCount;
        }

        /// <summary>
        /// Loads orders based on the current filter.
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to return</param>
        /// <param name="startRowIndex">Starting index of first record in result set</param>
        /// <param name="sortExpression">Sort expression to apply to the result set</param>
        /// <returns>A collection of orders matching the filter</returns>
        public OrderCollection Load(int maximumRows, int startRowIndex, string sortExpression)
        {
            Database database = Token.Instance.Database;

            // GET THE IDS OF THE ORDERS IN THE REQUESTED SET
            int thisIndex = 0;
            int rowCount = 0;
            List<string> orderIds = new List<string>();
            DbCommand selectCommand = GetFilteredOrderIds(maximumRows, startRowIndex, sortExpression, false);
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        orderIds.Add(dr.GetInt32(0).ToString());
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }

            // LOAD THE ORDERS IN THE REQUESTED SET
            OrderCollection orders = new OrderCollection();
            if (orderIds.Count > 1)
            {
                string criteria = "OrderId IN (" + string.Join(",", orderIds.ToArray()) + ")";
                orders = OrderDataSource.LoadForCriteria(criteria, sortExpression);
            }
            else if (orderIds.Count == 1)
            {
                orders.Add(OrderDataSource.Load(Convert.ToInt32(orderIds[0])));
            }
            return orders;
        }

        /// <summary>
        /// Builds a select command using this object, the given sort expression and the count value.
        /// </summary>
        /// <param name="sortExpression">The sort expression to use in the select command</param>
        /// <param name="count">The number of rows to retrieve</param>
        /// <returns>The DbCommand object representing the required select command</returns>
        private DbCommand GetFilteredOrderIds(int maximumRows, int startRowIndex, string sortExpression, bool count)
        {
            // ADD THE ORDER TABLE PREFIX TO AVOID AMBIGIOUS COLUMN SQL ERROR
            if (string.IsNullOrEmpty(sortExpression)) sortExpression = "ac_Orders.OrderDate DESC";
            else if (!sortExpression.StartsWith("ac_Orders.")) sortExpression = "ac_Orders." + sortExpression;

            Token token = Token.Instance;
            Database database = token.Database;
            Store store = token.Store;

            // CREATE A BLANK SQL COMMAND TO BE POPULATED
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT");

            // BUILD A LIST OF THE SOURCE TABLE(S)
            bool distinct = false;
            StringBuilder fromTables = new StringBuilder();
            fromTables.Append("ac_Orders");
            // DETERMINE FTS PATH
            bool useFtsSearch = false;
            if (!string.IsNullOrEmpty(this.Keyword))
            {
                useFtsSearch = Store.GetCachedSettings().FullTextSearch && KeywordSearchHelper.IsSearchPatternSupportedForFts(this.Keyword);
                if (!useFtsSearch)
                {
                    // BASIC SEARCH REQUIRES TABLE JOINS
                    if ((this.KeywordField & KeywordFieldType.ShippingInfo) == KeywordFieldType.ShippingInfo)
                    {
                        distinct = true;
                        fromTables.Append(" INNER JOIN ac_OrderShipments ON ac_Orders.OrderId = ac_OrderShipments.OrderId");
                        if ((this.KeywordField & KeywordFieldType.Notes) == KeywordFieldType.Notes)
                        {
                            fromTables.Insert(0, "(");
                            fromTables.Append(") INNER JOIN ac_OrderNotes ON ac_Orders.OrderId = ac_OrderNotes.OrderId");
                        }
                    }
                    else if ((this.KeywordField & KeywordFieldType.Notes) == KeywordFieldType.Notes)
                    {
                        distinct = true;
                        fromTables.Append(" INNER JOIN ac_OrderNotes ON ac_Orders.OrderId = ac_OrderNotes.OrderId");
                    }
                }
            }
            fromTables.Insert(0, " FROM ");

            // BUILD THE WHERE CRITERIA
            List<string> whereCriteria = new List<string>();

            // ALL FILTERS MUST INCLUDE STOREID
            whereCriteria.Add("ac_Orders.StoreId = @storeId");
            database.AddInParameter(selectCommand, "storeId", DbType.Int32, Token.Instance.StoreId);

            // ADD DATE FILTER
            if (OrderDateStart > DateTime.MinValue)
            {
                whereCriteria.Add("ac_Orders.OrderDate >= @orderDateStart");
                database.AddInParameter(selectCommand, "orderDateStart", DbType.DateTime, this.OrderDateStart);
            }
            if (OrderDateEnd > DateTime.MinValue && OrderDateEnd < DateTime.MaxValue)
            {
                whereCriteria.Add("ac_Orders.OrderDate <= @orderDateEnd");
                database.AddInParameter(selectCommand, "orderDateEnd", DbType.DateTime, this.OrderDateEnd);
            }

            // ORDER ID RANGE FILTER
            IdRangeParser orderIdRangeParser = new IdRangeParser("ac_Orders.OrderId", _OrderIdRange, "oi");
            if (orderIdRangeParser.RangeCount > 0)
            {
                whereCriteria.Add(orderIdRangeParser.GetSqlString(string.Empty));
                orderIdRangeParser.AddParameters(database, selectCommand);
            }

            // ORDER NUMBER RANGE FILTER
            IdRangeParser orderNumberRangeFilter = new IdRangeParser("ac_Orders.OrderNumber", _OrderNumberRange, "on");
            if (orderNumberRangeFilter.RangeCount > 0)
            {
                whereCriteria.Add(orderNumberRangeFilter.GetSqlString(string.Empty));
                orderNumberRangeFilter.AddParameters(database, selectCommand);
            }

            // ADD ORDER STATUS FILTER
            if (this.OrderStatus.Count > 0)
            {
                if (OrderStatus.Count > 1)
                {
                    whereCriteria.Add("ac_Orders.OrderStatusId IN ('" + this.OrderStatus.ToList("','") + "')");
                }
                else
                {
                    whereCriteria.Add("ac_Orders.OrderStatusId = @orderStatusId");
                    database.AddInParameter(selectCommand, "orderStatusId", DbType.Int32, this.OrderStatus[0]);
                }
            }

            // ADD PAYMENT STATUS FILTER
            if (this.PaymentStatus != OrderPaymentStatus.Unspecified)
            {
                whereCriteria.Add("PaymentStatusId = @paymentStatus");
                database.AddInParameter(selectCommand, "paymentStatus", System.Data.DbType.Byte, this.PaymentStatus);
            }

            // ADD SHIPMENT STATUS FILTER
            if (this.ShipmentStatus != OrderShipmentStatus.Unspecified)
            {
                whereCriteria.Add("ShipmentStatusId = @shipmentStatus");
                database.AddInParameter(selectCommand, "shipmentStatus", System.Data.DbType.Byte, this.ShipmentStatus);
            }

            if (!string.IsNullOrEmpty(this.Keyword))
            {
                // DETERMINE THE SEARCH TYPE
                if (useFtsSearch)
                {
                    // CREATE A SEARCH FILTER USING FTS
                    string searchPattern = (new FTSQueryParser(this.Keyword)).NormalForm;
                    List<string> containsCriteria = new List<string>();
                    if ((this.KeywordField & KeywordFieldType.BillingInfo) == KeywordFieldType.BillingInfo)
                    {
                        containsCriteria.Add("ac_Orders.OrderId IN (SELECT OrderId FROM ac_Orders WHERE CONTAINS(*, @keyword))");
                    }
                    if ((this.KeywordField & KeywordFieldType.ShippingInfo) == KeywordFieldType.ShippingInfo)
                    {
                        containsCriteria.Add("ac_Orders.OrderId IN (SELECT OrderId FROM ac_OrderShipments WHERE CONTAINS(*, @keyword))");
                    }
                    if ((this.KeywordField & KeywordFieldType.Notes) == KeywordFieldType.Notes)
                    {
                        containsCriteria.Add("ac_Orders.OrderId IN (SELECT OrderId FROM ac_OrderNotes WHERE CONTAINS(*, @keyword))");
                    }
                    if (containsCriteria.Count == 1)
                    {
                        whereCriteria.Add(containsCriteria[0]);
                    }
                    else
                    {
                        whereCriteria.Add("(" + string.Join(" OR ", containsCriteria.ToArray()) + ")");
                    }
                    database.AddInParameter(selectCommand, "keyword", DbType.String, searchPattern);
                }
                else
                {
                    // BUILD COLUMNS FOR BASIC (NON-FTS) SEARCH
                    List<string> columns = new List<string>();
                    if ((this.KeywordField & KeywordFieldType.BillingInfo) == KeywordFieldType.BillingInfo)
                    {
                        columns.Add("BillToFirstName");
                        columns.Add("BillToLastName");
                        columns.Add("BillToCompany");
                        columns.Add("BillToAddress1");
                        columns.Add("BillToAddress2");
                        columns.Add("BillToCity");
                        columns.Add("BillToProvince");
                        columns.Add("BillToPostalCode");
                        columns.Add("BillToPhone");
                        columns.Add("BillToFax");
                        columns.Add("BillToEmail");
                    }
                    if ((this.KeywordField & KeywordFieldType.ShippingInfo) == KeywordFieldType.ShippingInfo)
                    {
                        columns.Add("ShipToFirstName");
                        columns.Add("ShipToLastName");
                        columns.Add("ShipToCompany");
                        columns.Add("ShipToAddress1");
                        columns.Add("ShipToAddress2");
                        columns.Add("ShipToCity");
                        columns.Add("ShipToProvince");
                        columns.Add("ShipToPostalCode");
                        columns.Add("ShipToPhone");
                        columns.Add("ShipToFax");
                        columns.Add("ShipToEmail");
                        columns.Add("ShipMessage");
                    }
                    if ((this.KeywordField & KeywordFieldType.Notes) == KeywordFieldType.Notes)
                    {
                        columns.Add("Comment");
                    }

                    // PARSE THE CRITERIA AND ADD TO THE QUERY
                    KeywordCriterion criterion = KeywordSearchHelper.ParseKeywordCriterion(this.Keyword, columns.ToArray());
                    whereCriteria.Add(criterion.WhereClause);
                    foreach (DatabaseParameter param in criterion.Parameters)
                    {
                        database.AddInParameter(selectCommand, param.Name, param.DbType, param.Value);
                    }
                }
            }

            // COMPILE THE FINAL QUERY
            StringBuilder sqlBuilder = new StringBuilder();
            if (!count)
            {
                // BUILD SELECT TO RETRIEVE ALL DATA
                sqlBuilder.Append("SELECT");
                if (distinct) sqlBuilder.Append(" DISTINCT");
                if (maximumRows > 0) sqlBuilder.Append(" TOP " + (startRowIndex + maximumRows).ToString());
                sqlBuilder.Append(" ac_Orders.OrderId");
                string sortColumn = sortExpression.Replace(" ASC", "").Replace(" DESC", "");
                if (sortColumn != "ac_Orders.OrderId")
                {
                    sqlBuilder.Append("," + sortColumn);
                }
            }
            else
            {
                // BUILD COUNT COMMAND
                sqlBuilder.Append("SELECT COUNT(");
                if (distinct) sqlBuilder.Append("DISTINCT ");
                sqlBuilder.Append("ac_Orders.OrderId) AS OrderCount");
            }
            sqlBuilder.Append(fromTables.ToString());
            sqlBuilder.Append(" WHERE " + string.Join(" AND ", whereCriteria.ToArray()) + " ");

            // ADD IN SORT EXPRESSION
            if (!count) sqlBuilder.Append("ORDER BY " + sortExpression);

            // SET SQL STRING AND RETURN COMMAND
            selectCommand.CommandText = sqlBuilder.ToString();
            return selectCommand;
        }
    }
}