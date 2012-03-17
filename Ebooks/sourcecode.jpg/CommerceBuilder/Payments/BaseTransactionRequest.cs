using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Payments
{
    /// <summary>
    /// Abstract class that implements ITransactionRequest interface
    /// </summary>
    public abstract class BaseTransactionRequest : ITransactionRequest
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="payment">The payment associated with the transaction request</param>
        /// <param name="remoteIP">The remote IP of the user initiating the transaction request</param>
        public BaseTransactionRequest(Payment payment, string remoteIP)
        {
            this._Payment = payment;
            this._RemoteIP = StringHelper.Truncate(remoteIP, 39);
            this._Amount = payment.Amount;
            this._CurrencyCode = payment.CurrencyCode;
        }

        #region ITransactionRequest Members

        private LSDecimal _Amount;
        private string _CurrencyCode;
        private Dictionary<string, string> _ExtendedProperties = new Dictionary<string, string>();
        private Payment _Payment;
        private string _RemoteIP;

        /// <summary>
        /// The amount of transaction
        /// </summary>
        public LSDecimal Amount
        {
            get { return _Amount; }
            set { _Amount = value; }
        }

        /// <summary>
        /// The currency used
        /// </summary>
        public string CurrencyCode
        {
            get
            {
                if (String.IsNullOrEmpty(_CurrencyCode)) 
                    _CurrencyCode = Token.Instance.Store.BaseCurrency.ISOCode;
                return _CurrencyCode;
            }
            set { _CurrencyCode = value; }
        }

        /// <summary>
        /// Name-Value pairs containg additional configuration properties
        /// </summary>
        public Dictionary<string, string> ExtendedProperties
        {
            get
            {
                return _ExtendedProperties;
            }
        }

        /// <summary>
        /// Payment associated with this transaction
        /// </summary>
        public Payment Payment
        {
            get { return _Payment; }
        }

        /// <summary>
        /// Remote IP of the User initiating the transaction
        /// </summary>
        public string RemoteIP
        {
            get
            {
                return _RemoteIP;
            }
            set
            {
                _RemoteIP = StringHelper.Truncate(value, 39);
            }
        }

        /// <summary>
        /// The type of transaction
        /// </summary>
        public abstract TransactionType TransactionType { get; }

        #endregion
    }
}
