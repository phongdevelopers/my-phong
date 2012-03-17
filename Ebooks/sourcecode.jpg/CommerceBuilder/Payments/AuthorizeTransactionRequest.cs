using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace CommerceBuilder.Payments
{
    /// <summary>
    /// Class that represents a request for payment Authorization
    /// </summary>
    public class AuthorizeTransactionRequest : BaseTransactionRequest
    {
        private bool _Capture;
        private TransactionOrigin _TransactionOrigin;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="payment">The Payment for this transaction</param>
        /// <param name="remoteIP">The remote IP of the user initiating the request</param>
        public AuthorizeTransactionRequest(Payment payment, string remoteIP) : base(payment, remoteIP)
        {
            this._TransactionOrigin = TransactionOrigin.Internet;
        }

        /// <summary>
        /// Whether to do authorize and capture in the same step
        /// </summary>
        public bool Capture
        {
            get { return _Capture; }
            set { _Capture= value; }
        }

        /// <summary>
        /// The origin of the transaction
        /// </summary>
        public TransactionOrigin TransactionOrigin
        {
            get { return _TransactionOrigin; }
            set { _TransactionOrigin = value; }
        }

        /// <summary>
        /// The type of transaction. TransactionType.AuthorizeCapture or TransactionType.Authorize; 
        /// </summary>
        public override TransactionType TransactionType
        {
            get
            {
                return this.Capture ? TransactionType.AuthorizeCapture : TransactionType.Authorize;
            }
        }
    }
}
