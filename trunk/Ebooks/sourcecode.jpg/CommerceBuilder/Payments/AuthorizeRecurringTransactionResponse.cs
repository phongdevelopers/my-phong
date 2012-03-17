using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Payments
{
    /// <summary>
    /// Class that represents a response to Authorize-Recurring request
    /// </summary>
    public class AuthorizeRecurringTransactionResponse
    {
        private TransactionStatus _Status;
        private TransactionCollection _Transactions;

        /// <summary>
        /// Constructor
        /// </summary>
        public AuthorizeRecurringTransactionResponse()
        {
            _Status = TransactionStatus.Failed;
            _Transactions = new TransactionCollection();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="transaction">The transaction to use for creating the response</param>
        public AuthorizeRecurringTransactionResponse(Transaction transaction)
        {
            if (transaction == null)
            {
                _Status = TransactionStatus.Failed;
                _Transactions = new TransactionCollection();
            }
            else
            {
                _Status = transaction.TransactionStatus;
                _Transactions = new TransactionCollection();
                _Transactions.Add(transaction);
            }
        }

        /// <summary>
        /// Status of the transaction
        /// </summary>
        public TransactionStatus Status {
            get { return _Status; }
            set { _Status = value; }
        }

        /// <summary>
        /// A collection of transactions associated with this response
        /// </summary>
        public TransactionCollection Transactions
        {
            get { return _Transactions; }
            set
            {
                if (value == null)
                {
                    _Transactions.Clear();
                }
                else
                {
                    _Transactions = value;
                }
            }
        }

        /// <summary>
        /// Add a transaction to this response object
        /// </summary>
        /// <param name="transaction">The transaction to add</param>
        public void AddTransaction(Transaction transaction)
        {
            _Transactions.Add(transaction);
        }

        /// <summary>
        /// Clear any transactions associated with this response object
        /// </summary>
        public void ClearTransactions()
        {
            _Transactions.Clear();
        }

    }
}
