using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Payments
{
    /// <summary>
    /// Enumeration that represents the type of a transaction
    /// </summary>
    public enum TransactionType : int
    {
        /// <summary>
        /// This is an authorize transaction
        /// </summary>
        Authorize = 0, 

        /// <summary>
        /// This is an Authorize and Capture (Sale) transaction
        /// </summary>
        AuthorizeCapture,

        /// <summary>
        /// This is an Authorize transaction for recurring payment
        /// </summary>
        AuthorizeRecurring,

        /// <summary>
        /// This is a partial capture transaction
        /// </summary>
        PartialCapture, 

        /// <summary>
        /// This is a capture transaction
        /// </summary>
        Capture, 

        /// <summary>
        /// This is a partial refund transaction
        /// </summary>
        PartialRefund, 

        /// <summary>
        /// This is a refund transaction
        /// </summary>
        Refund, 

        /// <summary>
        /// This is a void transaction
        /// </summary>
        Void,

        /// <summary>
        /// This is a Cancel/Void transaction for a recurring payment
        /// </summary>
        CancelRecurring
    }
}
