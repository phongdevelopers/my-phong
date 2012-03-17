using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Payments
{
   /// <summary>
   /// Class representing a request for capturing a previously authorized transaction
   /// </summary>
    public class CaptureTransactionRequest : BaseTransactionRequest
    {
        private Transaction _AuthorizeTransaction;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="payment">The Payment object associated with this transaction</param>
        /// <param name="remoteIP">Remote IP of the user initiating the request</param>
        public CaptureTransactionRequest(Payment payment, string remoteIP) : base(payment, remoteIP)
        {
            this._AuthorizeTransaction = payment.Transactions.LastAuthorization;
        }

        /// <summary>
        /// The authorize transaction that is to be captured
        /// </summary>
        public Transaction AuthorizeTransaction
        {
            get { return _AuthorizeTransaction; }
            set { _AuthorizeTransaction = value; }
        }

        /// <summary>
        /// Type of transaction
        /// </summary>
        public override TransactionType TransactionType
        {
            get
            {
                if (this.IsFinal) return TransactionType.Capture;
                else return TransactionType.PartialCapture;
            }
        }

        private bool _IsFinal = true;
        /// <summary>
        /// Indicates whether this is the final capture for the given autohrize transaction
        /// </summary>
        public bool IsFinal
        {
            get {
                //FORCE ISFINAL TO TRUE IF THE CAPTURE IS EQUAL TO OR GREATER THAN THE ORIGINAL PAYMENT AMOUNT
                if (!_IsFinal && this.Amount >= this.Payment.Amount) _IsFinal = true;
                return _IsFinal;
            }
            set { _IsFinal = value; }
        }

        /// <summary>
        /// Creates a clone of this CaptureTransactionRequest object
        /// </summary>
        /// <returns>A clone of this CaptureTransactionRequest object</returns>
        public CaptureTransactionRequest Clone()
        {
            Payment payment = PaymentDataSource.Load(this.Payment.PaymentId);
            CaptureTransactionRequest clone = new CaptureTransactionRequest(payment, this.RemoteIP);
            clone.Amount = this.Amount;
            clone.CurrencyCode = this.CurrencyCode;
            clone.IsFinal = this.IsFinal;
            foreach (string key in this.ExtendedProperties.Keys)
            {
                clone.ExtendedProperties[key] = this.ExtendedProperties[key];
            }
            return clone;
        }

    }
}
