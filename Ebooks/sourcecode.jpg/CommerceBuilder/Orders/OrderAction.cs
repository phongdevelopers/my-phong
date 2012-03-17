using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Orders
{
    /// <summary>
    /// Enumeration that represents an action on order
    /// </summary>
    public enum OrderAction : int
    {
        /// <summary>
        /// No action.
        /// </summary>
        None = 0,

        /// <summary>
        /// Process the order payments.
        /// </summary>
        ProcessPayments,

        /// <summary>
        /// Ship the order.
        /// </summary>
        ShipOrder
    }
}
