using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Orders
{
    /// <summary>
    /// Enumeration that represents the overall payment status of an order
    /// </summary>
    public enum OrderPaymentStatus
    {
        /// <summary>
        /// Order Payment status is unspecified.
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// Order Payment status is unpaid.
        /// </summary>
        Unpaid,

        /// <summary>
        /// Order Payment status is paid.
        /// </summary>
        Paid,

        /// <summary>
        /// Order Payment has some problem.
        /// </summary>
        Problem
    }
}
