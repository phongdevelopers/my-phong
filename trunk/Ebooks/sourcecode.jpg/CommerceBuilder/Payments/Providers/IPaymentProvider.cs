using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Products;

namespace CommerceBuilder.Payments.Providers
{
    /// <summary>
    /// Interface that all payment providers implementations must implement
    /// </summary>
    public interface IPaymentProvider
    {
        /// <summary>
        /// Id of the payment gateway in database. Id is passed at the time of initialization.
        /// </summary>
        int PaymentGatewayId { get; }

        /// <summary>
        /// Name of the payment provider implementation
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Description of the payment provider implementation
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Version of the payment provider implementation
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Whether debug mode is enabled or not
        /// </summary>
        bool UseDebugMode { get; set; }

        /// <summary>
        /// Gets a flag indicating whether the provider requires account data (e.g. credit card number)
        /// in order to process a refund transaction request
        /// </summary>
        bool RefundRequiresAccountData { get; }

        /// <summary>
        /// The transactions that are supported by the payment provider implementation
        /// </summary>
        /// 
        SupportedTransactions SupportedTransactions { get; }

        /// <summary>
        /// Initializes the payment provider implementation. Called by AC at the time of initialization.
        /// </summary>
        /// <param name="PaymentGatewayId">Id of the payment gateway in database</param>
        /// <param name="ConfigurationData">Configuration data as name-value pairs</param>
        void Initialize(int PaymentGatewayId, Dictionary<String, String> ConfigurationData);

        /// <summary>
        /// Builds an input form for getting configuration data. Input form is buit inside the given
        /// ASP.NET parent control.
        /// </summary>
        /// <param name="parentControl">ASP.NET control to which to add the input form</param>
        void BuildConfigForm(System.Web.UI.Control parentControl);

        /// <summary>
        /// Gets a reference string to identify a particular configuration.
        /// </summary>
        string ConfigReference { get; }

        /// <summary>
        /// Gets a Url for the logo of the payment provider implementation
        /// </summary>
        /// <param name="cs">ClientScriptManager</param>
        /// <returns>A Url for the logo of the payment provider implementation</returns>
        string GetLogoUrl(System.Web.UI.ClientScriptManager cs);

        /// <summary>
        /// Submits an authorization request
        /// </summary>
        /// <param name="authorizeRequest">The authorization request</param>
        /// <returns>Transaction that represents the result of the authorization request</returns>
        Transaction DoAuthorize(AuthorizeTransactionRequest authorizeRequest);

        /// <summary>
        /// Submits a capture request for a previously authorized transaction
        /// </summary>
        /// <param name="captureRequest">The capture request</param>
        /// <returns>Transaction that represents the result of the capture request</returns>
        Transaction DoCapture(CaptureTransactionRequest captureRequest);

        /// <summary>
        /// Submits a refund request for a previously captured transaction
        /// </summary>
        /// <param name="creditRequest">The refund request</param>
        /// <returns>Transaction that represents the result of the refund request</returns>
        Transaction DoRefund(RefundTransactionRequest creditRequest);

        /// <summary>
        /// Submits a void request for a previously authorized transaction
        /// </summary>
        /// <param name="voidRequest">The void request</param>
        /// <returns>Transaction that represents the result of the void request</returns>
        Transaction DoVoid(VoidTransactionRequest voidRequest);

        /// <summary>
        /// Submits a recurring billing authorization request.
        /// </summary>
        /// <param name="authorizeRequest">An authorize request that contains the details of the order and payment.</param>
        /// <returns>A transaction instance that contains the results of the recurring billing request</returns>
        AuthorizeRecurringTransactionResponse DoAuthorizeRecurring(AuthorizeRecurringTransactionRequest authorizeRequest);

        /// <summary>
        /// Requests cancellation of a recurring billing authorization.
        /// </summary>
        /// <param name="cancelRequest">A cancel request that contains the details of the recurring payment.</param>
        /// <returns>A transaction instance that cnotains the results of the cancel recurring billing request.</returns>
        Transaction CancelAuthorizeRecurring(CancelRecurringTransactionRequest cancelRequest);
    }
}
