using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Payments
{
    /// <summary>
    /// Enumeration that represents the status of a payment
    /// </summary>
    public enum PaymentStatus : short
    {
        /// <summary>
        /// Payment is unprocessed
        /// </summary>
        Unprocessed = 0,

        /// <summary>
        /// Payment Authorization is pending
        /// </summary>
        AuthorizationPending,

        /// <summary>
        /// Payment is Authorized
        /// </summary>
        Authorized,

        /// <summary>
        /// Payment Authorization has failed
        /// </summary>
        AuthorizationFailed,

        /// <summary>
        /// Payment Capture is Pending
        /// </summary>
        CapturePending,

        /// <summary>
        /// Payment is Captured
        /// </summary>
        Captured,

        /// <summary>
        /// Payment Capture has failed
        /// </summary>
        CaptureFailed,

        /// <summary>
        /// Payment Refund is pending
        /// </summary>
        RefundPending,

        /// <summary>
        /// Payment is refunded
        /// </summary>
        Refunded,
        //RefundFailed,

        /// <summary>
        /// Payment void is pending
        /// </summary>
        VoidPending,

        /// <summary>
        /// Payment is voided
        /// </summary>
        Void,
        //VoidFailed,

        /// <summary>
        /// Payment has been completed.
        /// </summary>
        Completed
    }
}
