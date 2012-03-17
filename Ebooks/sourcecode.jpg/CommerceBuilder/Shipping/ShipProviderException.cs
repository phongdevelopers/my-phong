using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Shipping
{
    /// <summary>
    /// Exception class representing errors related to shipping providers
    /// </summary>
    public class ShipProviderException : System.Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ShipProviderException() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The exception message</param>
        public ShipProviderException(string message) : base(message) {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public ShipProviderException(string message, Exception innerException) : base(message, innerException) { }
    }
}
