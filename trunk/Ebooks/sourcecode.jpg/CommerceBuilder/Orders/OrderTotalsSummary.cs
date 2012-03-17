using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Common;

namespace CommerceBuilder.Orders
{
    /// <summary>
    /// Class representing summary of order totals
    /// </summary>
    public class OrderTotalsSummary
    {
        private LSDecimal _TotalCharges;
        private LSDecimal _TotalPayments;
        private LSDecimal _ProcessedPayments;
        private LSDecimal _UnprocessedPayments;
        private LSDecimal _Balance;

        /// <summary>
        /// Total charges
        /// </summary>
        public LSDecimal TotalCharges
        {
            get { return _TotalCharges; }
            set { _TotalCharges = value; _Balance = _TotalCharges - _ProcessedPayments; }
        }

        /// <summary>
        /// Total payments
        /// </summary>
        public LSDecimal TotalPayments
        {
            get { return _TotalPayments; }            
        }

        /// <summary>
        /// Total processed payments
        /// </summary>
        public LSDecimal ProcessedPayments
        {
            get { return _ProcessedPayments; }            
        }

        /// <summary>
        /// Total unprocessed payments
        /// </summary>
        public LSDecimal UnprocessedPayments
        {
            get { return _UnprocessedPayments; }            
        }

        /// <summary>
        /// Remaining balance
        /// </summary>
        public LSDecimal Balance
        {
            get { return _Balance; }
        }

        /// <summary>
        /// Adds a payment of given amount to this total summary and recalculates.
        /// </summary>
        /// <param name="amount">The amount of payment to add</param>
        /// <param name="processed">If <b>true</b> payment is considered processed, otherwise payment is considered unprocessed.</param>
        public void AddPayment(LSDecimal amount, bool processed)
        {
            _TotalPayments += amount;
            if (processed)
            {
                _ProcessedPayments += amount;
            }
            else
            {
                _UnprocessedPayments += amount;
            }

            _Balance = _TotalCharges - _ProcessedPayments;
        }

    }
}
