using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Orders;
using CommerceBuilder.Utility;
using CommerceBuilder.Search;

namespace CommerceBuilder.Reporting
{
    /// <summary>
    /// Class representing search criteria for orders
    /// </summary>
    /// 
    [Serializable]
    public class OrderSearchCriteria
    {
        private DateTime _OrderDateStart;
        private DateTime _OrderDateEnd;
        private string _OrderIdRange;
        private int _OrderIdStart;
        private int _OrderIdEnd;
        private string _OrderNumberRange;
        private int _OrderNumberStart;
        private int _OrderNumberEnd;
        private IdList _OrderStatus;
        private OrderPaymentStatus _PaymentStatus;
        private OrderShipmentStatus _ShipmentStatus;
        private Collection<MatchCriteria> _Filter;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrderSearchCriteria()
        {
            _OrderDateStart = DateTime.MinValue;
            _OrderDateEnd = DateTime.MaxValue;
            _OrderIdRange = string.Empty;
            _OrderIdStart = 0;
            _OrderIdEnd = 0;
            _OrderNumberRange = string.Empty;
            _OrderNumberStart = 0;
            _OrderNumberEnd = 0;
            _PaymentStatus = OrderPaymentStatus.Unspecified;
            _ShipmentStatus = OrderShipmentStatus.Unspecified;
            _OrderStatus = new IdList();
            _Filter = new Collection<MatchCriteria>();
        }

        /// <summary>
        /// Start date for orders
        /// </summary>
        public DateTime OrderDateStart
        {
            get { return _OrderDateStart; }
            set { _OrderDateStart = value; }
        }

        /// <summary>
        /// End date for orders
        /// </summary>
        public DateTime OrderDateEnd
        {
            get { return _OrderDateEnd; }
            set { _OrderDateEnd = value; }
        }

        /// <summary>
        /// Starting order Id
        /// </summary>
        [Obsolete("Use OrderIdRange instead.")]
        public int OrderIdStart
        {
            get { return _OrderIdStart; }
            set { _OrderIdStart = value; }
        }

        /// <summary>
        /// Ending order Id
        /// </summary>
        [Obsolete("Use OrderIdRange instead.")]
        public int OrderIdEnd
        {
            get { return _OrderIdEnd; }
            set { _OrderIdEnd = value; }
        }

        /// <summary>
        /// Order Id Range
        /// </summary>
        public string OrderIdRange
        {
            get { return _OrderIdRange; }
            set { _OrderIdRange = value; }
        }

        /// <summary>
        /// Starting order number
        /// </summary>
        [Obsolete("Use OrderNumberRange instead.")]
        public int OrderNumberStart
        {
            get { return _OrderNumberStart; }
            set { _OrderNumberStart = value; }
        }

        /// <summary>
        /// Ending order number
        /// </summary>
        [Obsolete("Use OrderNumberRange instead.")]
        public int OrderNumberEnd
        {
            get { return _OrderNumberEnd; }
            set { _OrderNumberEnd = value; }
        }

        /// <summary>
        /// Order Number Range
        /// </summary>
        public string OrderNumberRange
        {
            get { return _OrderNumberRange; }
            set { _OrderNumberRange = value; }
        }

        /// <summary>
        /// Payment status of the orders to search
        /// </summary>
        public OrderPaymentStatus PaymentStatus
        {
            get { return _PaymentStatus; }
            set { _PaymentStatus = value; }
        }

        /// <summary>
        /// Shipment status of the orders to search
        /// </summary>
        public OrderShipmentStatus ShipmentStatus
        {
            get { return _ShipmentStatus; }
            set { _ShipmentStatus = value; }
        }

        /// <summary>
        /// Order status of the orders to search
        /// </summary>
        public IdList OrderStatus
        {
            get { return this._OrderStatus; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Collection<MatchCriteria> Filter
        {
            get { return this._Filter; }
            set { this._Filter = value; }
        }

        /// <summary>
        /// Builds a select command using this object and the given sort expression.
        /// </summary>
        /// <param name="sortExpression">The sort expression to use in the select command</param>
        /// <returns>The DbCommand object representing the required select command</returns>
        public DbCommand BuildSelectCommand(string sortExpression)
        {
            return this.BuildSelectCommand(sortExpression, false);
        }

        /// <summary>
        /// Builds a select command using this object, the given sort expression and the count value.
        /// </summary>
        /// <param name="sortExpression">The sort expression to use in the select command</param>
        /// <param name="count">The number of rows to retrieve</param>
        /// <returns>The DbCommand object representing the required select command</returns>
        public DbCommand BuildSelectCommand(string sortExpression, bool count)
        {
            //INITIALIZE VARIABLES

            // ADD THE ORDER TABLE PREFIX TO AVOID AMBIGIOUS COLUMN SQL ERROR
            if (string.IsNullOrEmpty(sortExpression)) sortExpression = "ac_Orders.OrderDate DESC";
            else if (!sortExpression.StartsWith("ac_Orders.")) sortExpression = "ac_Orders." + sortExpression;

            Database database = Token.Instance.Database;

            //CREATE A BLANK SQL COMMAND TO BE POPULATED
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT");

            //BUILD A LIST OF TABLES TO SELECT FROM
            List<string> tables = new List<string>();
            tables.Add("ac_Orders");

            //BUILD A LIST OF ANY JOIN CRITERIA
            List<string> tableJoins = new List<string>();

            //BUILD THE WHERE CRITERIA
            List<string> whereCriteria = new List<string>();

            //ALL FILTERS MUST INCLUDE STOREID
            whereCriteria.Add("ac_Orders.StoreId = @storeId");
            database.AddInParameter(selectCommand, "storeId", DbType.Int32, Token.Instance.StoreId);

            //ADD DATE FILTER
            if (OrderDateStart > DateTime.MinValue)
            {
                whereCriteria.Add("ac_Orders.OrderDate >= @orderDateStart");
                database.AddInParameter(selectCommand, "orderDateStart", DbType.DateTime, this.OrderDateStart);
            }
            if ((OrderDateEnd != DateTime.MaxValue) || (OrderDateEnd != DateTime.MinValue))
            {
                whereCriteria.Add("ac_Orders.OrderDate <= @orderDateEnd");
                database.AddInParameter(selectCommand, "orderDateEnd", DbType.DateTime, this.OrderDateEnd);
            }

            //ADD ORDERID FILTER
            if (_OrderIdStart > 0)
            {
                whereCriteria.Add("ac_Orders.OrderId >= @orderIdStart");
                database.AddInParameter(selectCommand, "orderIdStart", DbType.Int32, _OrderIdStart);
            }
            if (_OrderIdEnd > 0)
            {
                whereCriteria.Add("ac_Orders.OrderId <= @orderIdEnd");
                database.AddInParameter(selectCommand, "orderIdEnd", DbType.Int32, _OrderIdEnd);
            }

            // ORDER ID RANGE FILTER
            IdRangeParser orderIdRangeParser = new IdRangeParser("ac_Orders.OrderId", _OrderIdRange, "oi");
            if (orderIdRangeParser.RangeCount > 0)
            {
                whereCriteria.Add(orderIdRangeParser.GetSqlString(string.Empty));
                orderIdRangeParser.AddParameters(database, selectCommand);
            }

            //ADD ORDERNUMBER FILTER
            if (_OrderNumberStart > 0)
            {
                whereCriteria.Add("ac_Orders.OrderNumber >= @orderNumberStart");
                database.AddInParameter(selectCommand, "orderNumberStart", DbType.Int32, _OrderNumberStart);
            }
            if (_OrderNumberEnd > 0)
            {
                whereCriteria.Add("ac_Orders.OrderNumber <= @orderNumberEnd");
                database.AddInParameter(selectCommand, "orderNumberEnd", DbType.Int32, _OrderNumberEnd);
            }

            // ORDER NUMBER RANGE FILTER
            IdRangeParser orderNumberRangeFilter = new IdRangeParser("ac_Orders.OrderNumber", _OrderNumberRange, "on");
            if (orderNumberRangeFilter.RangeCount > 0)
            {
                whereCriteria.Add(orderNumberRangeFilter.GetSqlString(string.Empty));
                orderNumberRangeFilter.AddParameters(database, selectCommand);
            }

            //ADD ORDER STATUS FILTER
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

            //ADD PAYMENT STATUS FILTER
            if (this.PaymentStatus != OrderPaymentStatus.Unspecified)
            {
                whereCriteria.Add("PaymentStatusId = @paymentStatus");
                database.AddInParameter(selectCommand, "paymentStatus", System.Data.DbType.Byte, this.PaymentStatus);
            }

            //ADD SHIPMENT STATUS FILTER
            if (this.ShipmentStatus != OrderShipmentStatus.Unspecified)
            {
                whereCriteria.Add("ShipmentStatusId = @shipmentStatus");
                database.AddInParameter(selectCommand, "shipmentStatus", System.Data.DbType.Byte, this.ShipmentStatus);
            }

            bool distinctQuery = false;
            foreach (MatchCriteria filter in this.Filter)
            {
                string tableName = string.Empty;
                string columnName = string.Empty;
                ParseFieldName(filter.FieldName, out tableName, out columnName);
                if (!string.IsNullOrEmpty(tableName))
                {
                    string fieldValue = StringHelper.FixSearchPattern(filter.FieldValue);
                    if (!string.IsNullOrEmpty(fieldValue))
                    {
                        bool addField = false;
                        string sqlOperator = " = ";
                        if (fieldValue.Contains("%") || fieldValue.Contains("_")) sqlOperator = " LIKE ";
                        switch (tableName)
                        {
                            case "ac_Orders":
                                addField = true;
                                break;
                            case "ac_OrderShipments":
                                addField = true;
                                distinctQuery = true;
                                if (!tables.Contains("ac_OrderShipments"))
                                {
                                    tables.Add("ac_OrderShipments");
                                    tableJoins.Add("ac_Orders.OrderId = ac_OrderShipments.OrderId");
                                }
                                break;
                            case "ac_OrderNotes":
                                addField = true;
                                distinctQuery = true;
                                if (!tables.Contains("ac_OrderNotes"))
                                {
                                    tables.Add("ac_OrderNotes");
                                    tableJoins.Add("ac_Orders.OrderId = ac_OrderNotes.OrderId");
                                }
                                break;
                        }
                        if (addField)
                        {
                            whereCriteria.Add(tableName + "." + columnName + sqlOperator + "@" + filter.FieldName);
                            database.AddInParameter(selectCommand, filter.FieldName, DbType.String, fieldValue);
                        }
                    }
                }
            }

            StringBuilder sqlBuilder = new StringBuilder();
            if (!count)
            {
                if (!distinctQuery)
                {
                    sqlBuilder.Append("SELECT TOP 1000 ac_Orders.OrderId ");
                }
                else
                {
                    sqlBuilder.Append("SELECT DISTINCT TOP 1000 ac_Orders.OrderId, " + sortExpression.Replace(" ASC", string.Empty).Replace(" DESC", string.Empty) + " ");
                }
            }
            else
            {
                sqlBuilder.Append("SELECT COUNT(*) As OrderCount ");
            }
            sqlBuilder.Append("FROM " + string.Join(",", tables.ToArray()) + " ");
            sqlBuilder.Append("WHERE " + string.Join(" AND ", whereCriteria.ToArray()) + " ");
            if (tableJoins.Count > 0) sqlBuilder.Append(" AND " + string.Join(" AND ", tableJoins.ToArray()) + " ");
            //add in sort expression
            if (!count) sqlBuilder.Append("ORDER BY " + sortExpression);

            //SET SQL STRING AND RETURN COMMAND
            selectCommand.CommandText = sqlBuilder.ToString();
            return selectCommand;
        }

        private void ParseFieldName(string fieldName, out string tableName, out string columnName)
        {
            switch (fieldName.ToLowerInvariant())
            {
                case "billtofirstname":
                case "billtolastname":
                case "billtocompany":
                case "billtoaddress1":
                case "billtoaddress2":
                case "billtocity":
                case "billtoprovince":
                case "billtocountrycode":
                    tableName = "ac_Orders";
                    columnName = fieldName;
                    break;
                case "shiptofirstname":
                case "shiptolastname":
                case "shiptocompany":
                case "shiptoaddress1":
                case "shiptoaddress2":
                case "shiptocity":
                case "shiptoprovince":
                case "shiptocountrycode":
                    tableName = "ac_OrderShipments";
                    columnName = fieldName;
                    break;
                case "ordernotes":
                    tableName = "ac_OrderNotes";
                    columnName = "Comment";
                    break;
                default:
                    tableName = string.Empty;
                    columnName = string.Empty;
                    break;
            }
        }
    }
}
