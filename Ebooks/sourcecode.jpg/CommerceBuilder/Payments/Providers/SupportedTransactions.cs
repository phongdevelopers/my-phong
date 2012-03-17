using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Payments.Providers
{
    /// <summary>
    /// Enumeration that represents the transactions supported by a payment provider.
    /// This enumeration can be used in bitwise comparisons
    /// </summary>
    [Flags]
    public enum SupportedTransactions : int
    {
        /// <summary>
        /// No transactions are supported
        /// </summary>
        None = 0,

        /// <summary>
        /// Authorize transaction is supported
        /// </summary>
        Authorize = 1,

        /// <summary>
        /// Authorize and Capture in one step (i.e; Sale) transaction is supported
        /// </summary>
        AuthorizeCapture = 2,

        /// <summary>
        /// Partial capture of previously authorized transactions is supported
        /// </summary>
        PartialCapture = 4,

        /// <summary>
        /// Full capture of of previously authorized transactions is supported
        /// </summary>
        Capture = 8,

        /// <summary>
        /// Partial refund of previously captured transactions is supported
        /// </summary>
        PartialRefund = 16,

        /// <summary>
        /// Full refund of previously captured transactions is supported
        /// </summary>
        Refund = 32,

        /// <summary>
        /// Voiding of previously authorized transactions is supported
        /// </summary>
        Void = 64,

        /// <summary>
        /// Recurring Billing (subscriptions) are supported
        /// </summary>
        RecurringBilling = 128,

        /// <summary>
        /// All transactions are supported
        /// </summary>
        All = (Authorize | AuthorizeCapture | PartialCapture | Capture | Capture | PartialRefund | Refund | Void | RecurringBilling)
    }
}
