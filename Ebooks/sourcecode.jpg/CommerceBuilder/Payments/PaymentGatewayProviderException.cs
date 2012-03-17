using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Payments
{
    /// <summary>
    /// Exception class related to Payment Gateway Provider errors
    /// </summary>
    public class PaymentGatewayProviderException : CommerceBuilder.Exceptions.CommerceBuilderException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The exception message</param>
        public PaymentGatewayProviderException(string message) : base(message) {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public PaymentGatewayProviderException(string message, Exception innerException) : base(message, innerException) { }
    }
}
