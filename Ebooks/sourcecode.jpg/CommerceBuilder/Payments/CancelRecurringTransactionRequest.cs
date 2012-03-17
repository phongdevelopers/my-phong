using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using CommerceBuilder.Products;

namespace CommerceBuilder.Payments
{
    /// <summary>
    /// Class that represents a request for cancelling a recurring subscription
    /// </summary>
    public class CancelRecurringTransactionRequest : BaseTransactionRequest
    {
        private TransactionOrigin _TransactionOrigin;
        private Transaction _RecurringTransaction;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="payment">The Payment object associated with this request</param>
        /// <param name="recurringTransaction">The original recurring transactions that is being cancelled</param>
        /// <param name="remoteIP">Remote IP of the user initiating the request</param>
        public CancelRecurringTransactionRequest(Payment payment, Transaction recurringTransaction, string remoteIP) : base(payment, remoteIP)
        {
            this._TransactionOrigin = TransactionOrigin.Internet;
            this._RecurringTransaction = recurringTransaction;
        }

        /// <summary>
        /// Origin of the transaction
        /// </summary>
        public TransactionOrigin TransactionOrigin
        {
            get { return _TransactionOrigin; }
            set { _TransactionOrigin = value; }
        }

        /// <summary>
        /// Type of transaction
        /// </summary>
        public override TransactionType TransactionType
        {
            get { return TransactionType.CancelRecurring; }
        }

        /// <summary>
        /// The original recurring transactions that is being cancelled
        /// </summary>
        public Transaction RecurringTransaction
        {
            get { return _RecurringTransaction; }
        }

    }
}
