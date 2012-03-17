using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Users;
using CommerceBuilder.Orders;
using CommerceBuilder.Common;

namespace CommerceBuilder.Reporting
{
    /// <summary>
    /// Class representing sales summary by referrer
    /// </summary>
    public class ReferrerSalesSummary
    {        
        private int _OrderCount;
        /// <summary>
        /// Number of orders
        /// </summary>
        public int OrderCount
        {
            get { return _OrderCount; }
            set { _OrderCount = value;}
        }
        
        private LSDecimal _SalesTotal;
        /// <summary>
        /// Total of all sales
        /// </summary>
        public LSDecimal SalesTotal
        {
            get { return _SalesTotal; }
            set { _SalesTotal = value; }
        }

        private LSDecimal _ProductSubtotal = -1;
        /// <summary>
        /// Total of products sold
        /// </summary>
        public LSDecimal ProductSubtotal
        {
            get { return _ProductSubtotal; }
            set { _ProductSubtotal = value; }
        }

        private String _Referrer;
        /// <summary>
        /// The referrer
        /// </summary>
        public String Referrer
        {
            get { return _Referrer; }
            set { _Referrer = value; }
        }

        private DateTime _StartDate;
        /// <summary>
        /// Start date for this summary
        /// </summary>
        public DateTime StartDate
        {
            get { return _StartDate; }
        }

        private DateTime _EndDate;
        /// <summary>
        /// End date for this summary
        /// </summary>
        public DateTime EndDate
        {
            get { return _EndDate; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="referrer">The referrer</param>
        /// <param name="startDate">The start date</param>
        /// <param name="endDate">The end date</param>
        /// <param name="orderCount">The order count</param>
        /// <param name="productSubtotal">The total of products sold</param>
        /// <param name="orderTotal">Total of all orders</param>
        public ReferrerSalesSummary(String referrer, DateTime startDate, DateTime endDate, int orderCount, LSDecimal productSubtotal, LSDecimal orderTotal)
        {
            _Referrer = referrer;
            _StartDate = startDate;
            _EndDate = endDate;
            _OrderCount = orderCount;
            _ProductSubtotal = productSubtotal;
            _SalesTotal = orderTotal;
        }
    }
}
