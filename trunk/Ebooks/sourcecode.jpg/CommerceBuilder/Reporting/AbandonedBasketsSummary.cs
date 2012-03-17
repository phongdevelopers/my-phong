using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Common;

namespace CommerceBuilder.Reporting
{
    /// <summary>
    /// Summary data of abandoned baskets
    /// </summary>
    public class AbandonedBasketsSummary
    {
        private DateTime _StartDate;
        private DateTime _EndDate;
        private int _BasketCount;
        private LSDecimal _Total;

        /// <summary>
        /// Start date of the summary
        /// </summary>
        public DateTime StartDate
        {
            get { return _StartDate; }
            set { _StartDate = value; }
        }

        /// <summary>
        /// End date of the summary
        /// </summary>
        public DateTime EndDate
        {
            get { return _EndDate; }
            set { _EndDate = value; }
        }

        /// <summary>
        /// Number of baskets
        /// </summary>
        public int BasketCount
        {
            get { return _BasketCount; }
            set { _BasketCount = value; }
        }

        /// <summary>
        /// Total amount
        /// </summary>
        public LSDecimal Total
        {
            get { return _Total; }
            set { _Total = value; }
        }
        
    }
}
