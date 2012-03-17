using System;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Payments
{
    public partial class Transaction
    {
        /// <summary>
        /// Indicates the type of transaction.
        /// </summary>
        public TransactionType TransactionType
        {
            get { return (TransactionType)this.TransactionTypeId; }
            set { this.TransactionTypeId = (short)value; }
        }

        /// <summary>
        /// Indicates the status of the transaction.
        /// </summary>
        public TransactionStatus TransactionStatus
        {
            get { return (TransactionStatus)this.TransactionStatusId; }
            set { this.TransactionStatusId = (short)value; }
        }

        private Dictionary<string, string> _AdditionalDataDictionary = null;
        /// <summary>
        /// Collection of additional data as name-value pairs
        /// </summary>
        public Dictionary<string, string> AdditionalDataDictionary
        {
            get
            {
                if (_AdditionalDataDictionary == null)
                {
                    ParseAdditionalData();
                }
                return _AdditionalDataDictionary;
            }
        }

        /// <summary>
        /// Updates the additional data for this transaction object
        /// </summary>
        /// <param name="additionalData">The additional data to use</param>
        public void UpdateAdditionalData(Dictionary<string, string> additionalData)
        {            
            if (additionalData == null)
            {
                this.AdditionalData = string.Empty;                
            }
            else
            {
                StringBuilder configBuilder = new StringBuilder();
                //urlencode the dictionary
                foreach (string key in additionalData.Keys)
                {
                    if (configBuilder.Length > 0) configBuilder.Append("&");
                    configBuilder.Append(key + "=" + System.Web.HttpUtility.UrlEncode(additionalData[key]));
                }
                this.AdditionalData = configBuilder.ToString();
            }
            _AdditionalDataDictionary = null;
        }

        /// <summary>
        /// Parses the additional data from a string (as saved in database) to name value pairs
        /// </summary>
        /// <returns>The parsed additional data</returns>
        public Dictionary<string, string> ParseAdditionalData()
        {
            Dictionary<string, string> additionalData = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(this.AdditionalData))
            {
                string[] pairs = this.AdditionalData.Split("&".ToCharArray());
                foreach (string thisPair in pairs)
                {
                    if (!string.IsNullOrEmpty(thisPair) && thisPair.Contains("="))
                    {
                        string[] AdditionalDataItem = thisPair.Split("=".ToCharArray());
                        string key = AdditionalDataItem[0];
                        string keyValue = System.Web.HttpUtility.UrlDecode(AdditionalDataItem[1]);
                        if (!string.IsNullOrEmpty(key))
                        {
                            additionalData.Add(key, keyValue);
                        }
                    }
                }
            }
            _AdditionalDataDictionary = additionalData;
            return additionalData;
        }

        /// <summary>
        /// Creates a transaction to report an error for a payment gateway.
        /// </summary>
        /// <param name="paymentGatewayId">The id of the payment gateway.</param>
        /// <param name="request">The request for the failed transaction.</param>
        /// <param name="errorCode">The error code to report.</param>
        /// <param name="errorMessage">The error message to report.</param>
        /// <returns>A transaction object configured with the error message.</returns>
        public static Transaction CreateErrorTransaction(int paymentGatewayId, ITransactionRequest request, string errorCode, string errorMessage)
        {
            return Transaction.CreateErrorTransaction(paymentGatewayId, request.TransactionType, request.Amount, errorCode, errorMessage, request.RemoteIP);
        }

        /// <summary>
        /// Creates a transaction to report an error for a payment gateway.
        /// </summary>
        /// <param name="paymentGatewayId">The id of the payment gateway.</param>
        /// <param name="transactionType">The type of the transaction attempted.</param>
        /// <param name="amount">The amount of the transaction attempted.</param>
        /// <param name="errorCode">The error code to report.</param>
        /// <param name="errorMessage">The error message to report.</param>
        /// <param name="remoteIP">The IP of the remote user initiating the transaction.</param>
        /// <returns>A transaction object configured with the error message.</returns>
        public static Transaction CreateErrorTransaction(int paymentGatewayId, TransactionType transactionType, LSDecimal amount, string errorCode, string errorMessage, string remoteIP)
        {
            Transaction transaction = new Transaction();
            transaction.Amount = amount;
            transaction.PaymentGatewayId = paymentGatewayId;
            transaction.RemoteIP = remoteIP;
            transaction.ResponseCode = errorCode;
            transaction.ResponseMessage = errorMessage;
            transaction.TransactionStatus = TransactionStatus.Failed;
            transaction.TransactionType = transactionType;
            transaction.TransactionDate = CommerceBuilder.Utility.LocaleHelper.LocalNow;
            return transaction;
        }

        /// <summary>
        /// Delete this transaction object from database
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public virtual bool Delete()
        {
            Database database = Token.Instance.Database;
            using (System.Data.Common.DbCommand deleteCommand = database.GetSqlStringCommand("UPDATE ac_Subscriptions SET TransactionId = NULL WHERE TransactionId = @transactionId"))
            {
                database.AddInParameter(deleteCommand, "@transactionId", System.Data.DbType.Int32, this.TransactionId);
                database.ExecuteNonQuery(deleteCommand);
            }
            return this.BaseDelete();
        }

    }
}
