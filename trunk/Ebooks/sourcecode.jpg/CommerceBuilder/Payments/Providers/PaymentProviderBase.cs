using System;
using System.Collections.Generic;
using System.IO;
using CommerceBuilder.Products;

namespace CommerceBuilder.Payments.Providers
{
    /// <summary>
    /// A base class that can be extended by payment provider implementations
    /// </summary>
    public abstract class PaymentProviderBase : IPaymentProvider
    {

        #region IPaymentProvider Members

        private int _PaymentGatewayId;
        /// <summary>
        /// Id of the payment gateway in database. Id is passed at the time of initialization.
        /// </summary>
        public int PaymentGatewayId { get { return _PaymentGatewayId;}}
                
        /// <summary>
        /// Name of the payment provider implementation
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Description of the payment provider implementation
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        ///  Version of the payment provider implementation
        /// </summary>
        public abstract string Version { get; }

        bool _UseDebugMode;
        /// <summary>
        /// Whether debug mode is enabled or not
        /// </summary>
        public bool UseDebugMode
        {
            get { return _UseDebugMode; }
            set { _UseDebugMode = value; }
        }

        /// <summary>
        /// Gets a flag indicating whether the provider requires account data (e.g. credit card number)
        /// in order to process a refund transaction request
        /// </summary>
        public virtual bool RefundRequiresAccountData
        {
            get { return false; }
        }

        /// <summary>
        /// The transactions that are supported by the payment provider implementation
        /// </summary>
        public abstract SupportedTransactions SupportedTransactions { get; }

        /// <summary>
        /// Initializes the payment provider implementation. Called by AC at the time of initialization.
        /// </summary>
        /// <param name="PaymentGatewayId">Id of the payment gateway in database</param>
        /// <param name="ConfigurationData">Configuration data as name-value pairs</param>
        public virtual void Initialize(int PaymentGatewayId, Dictionary<String, String> ConfigurationData)
        {
            this._PaymentGatewayId = PaymentGatewayId;
            if (ConfigurationData.ContainsKey("UseDebugMode")) UseDebugMode = bool.Parse(ConfigurationData["UseDebugMode"]);
        }

        /// <summary>
        /// Builds an input form for getting configuration data. Input form is buit inside the given
        /// ASP.NET parent control.
        /// </summary>
        /// <param name="parentControl">ASP.NET control to which to add the input form</param>
        public abstract void BuildConfigForm(System.Web.UI.Control parentControl);

        /// <summary>
        /// Gets a reference string to identify a particular configuration.
        /// </summary>
        public abstract string ConfigReference { get; }

        /// <summary>
        /// Gets a Url for the logo of the payment provider implementation
        /// </summary>
        /// <param name="cs">ClientScriptManager</param>
        /// <returns>A Url for the logo of the payment provider implementation</returns>
        public abstract string GetLogoUrl(System.Web.UI.ClientScriptManager cs);

        /// <summary>
        /// Submits an authorization request
        /// </summary>
        /// <param name="authorizeRequest">The authorization request</param>
        /// <returns>Transaction that represents the result of the authorization request</returns>
        public abstract Transaction DoAuthorize(AuthorizeTransactionRequest authorizeRequest);

        /// <summary>
        /// Submits a capture request for a previously authorized transaction
        /// </summary>
        /// <param name="captureRequest">The capture request</param>
        /// <returns>Transaction that represents the result of the capture request</returns>
        public abstract Transaction DoCapture(CaptureTransactionRequest captureRequest);

        /// <summary>
        /// Submits a refund request for a previously captured transaction
        /// </summary>
        /// <param name="creditRequest">The refund request</param>
        /// <returns>Transaction that represents the result of the refund request</returns>
        public abstract Transaction DoRefund(RefundTransactionRequest creditRequest);

        /// <summary>
        /// Submits a void request for a previously authorized transaction
        /// </summary>
        /// <param name="voidRequest">The void request</param>
        /// <returns>Transaction that represents the result of the void request</returns>
        public abstract Transaction DoVoid(VoidTransactionRequest voidRequest);


        /// <summary>
        /// Submits a recurring billing authorization request.
        /// </summary>
        /// <param name="authorizeRequest">An authorize request that contains the details of the order and payment.</param>
        /// <returns>A transaction instance that contains the results of the recurring billing request</returns>
        public virtual AuthorizeRecurringTransactionResponse DoAuthorizeRecurring(AuthorizeRecurringTransactionRequest authorizeRequest)
        {
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = authorizeRequest.TransactionType;
            transaction.TransactionStatus = TransactionStatus.Failed;
            transaction.ResponseCode = "U";
            transaction.ResponseMessage = "Recurring billing is unsupported by this gateway integration.";
            transaction.RemoteIP = authorizeRequest.RemoteIP;
            AuthorizeRecurringTransactionResponse response = new AuthorizeRecurringTransactionResponse();
            response.Status = transaction.TransactionStatus;
            response.AddTransaction(transaction);
            return response;
        }

        /// <summary>
        /// Requests cancellation of a recurring billing authorization.
        /// </summary>
        /// <param name="cancelRequest">A cancel request that contains the details of the recurring payment.</param>
        /// <returns>A transaction instance that cnotains the results of the cancel recurring billing request.</returns>
        public virtual Transaction CancelAuthorizeRecurring(CancelRecurringTransactionRequest cancelRequest)
        {
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = cancelRequest.TransactionType;
            transaction.TransactionStatus = TransactionStatus.Failed;
            transaction.ResponseCode = "U";
            transaction.ResponseMessage = "Cancellation of recurring billing is unsupported by this gateway integration.";
            transaction.RemoteIP = cancelRequest.RemoteIP;
            return transaction;
        }

        #endregion

        /// <summary>
        /// Records a transaction message to the debug log.
        /// </summary>
        /// <param name="providerName">Name of the provider or gateway</param>
        /// <param name="direction">Indicates whether the data was sent or received</param>
        /// <param name="message">Content of the message</param>
        /// <param name="sensitiveData">A dictionary of key/value pairs that contains sensitive data that exists within the message (key) and the desired replacement (value).  Pass null if no replacements are required.</param>
        protected void RecordCommunication(string providerName, CommunicationDirection direction, string message, Dictionary<string,string> sensitiveData)
        {
            //CHECK FOR SENSITIVE DATA THAT MUST BE SCRUBBED
            if (sensitiveData != null)
            {
                foreach (string key in sensitiveData.Keys)
                {
                    message = message.Replace(key, sensitiveData[key]);
                }
            }
            //GET LOG DIRECTORY
            string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\Logs\\");
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            string fileName = Path.Combine(directory, providerName + ".Log");
            using (StreamWriter sw = File.AppendText(fileName))
            {
                sw.WriteLine(direction.ToString() + ": " + message);
                sw.WriteLine(string.Empty);
                sw.Close();
            }
        }

        /// <summary>
        /// Creates a reference number from the given credit card account number. 
        /// Leaves the last 4 characters in place and replaces the rest with x
        /// </summary>
        /// <param name="accountNumber">The account number to make reference for</param>
        /// <returns>The reference number</returns>
        protected string MakeReferenceNumber(string accountNumber)
        {
            if (string.IsNullOrEmpty(accountNumber)) return string.Empty;
            int length = accountNumber.Length;
            if (length < 5)
            {
                return new string('x', length);
            }
            return new string('x', length - 4) + accountNumber.Substring(length - 4);
        }

        /// <summary>
        /// Enumeration that represents the direction of communication between AC and the gateway
        /// </summary>
        public enum CommunicationDirection : int { 
            /// <summary>
            /// Sending data to the gateway
            /// </summary>
            Send,
 
            /// <summary>
            /// Receiving data from the gateway
            /// </summary>
            Receive 
        };
    }
}
