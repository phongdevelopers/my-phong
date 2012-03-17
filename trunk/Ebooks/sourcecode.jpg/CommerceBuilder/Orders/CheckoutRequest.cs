using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Payments;

namespace CommerceBuilder.Orders
{
    /// <summary>
    /// Class representing a checkout request
    /// </summary>
    public class CheckoutRequest
    {
        private Payment _Payment;

        /// <summary>
        /// Payment associated with this checkout request
        /// </summary>
        public Payment Payment
        {
            get { return _Payment; }
            set { _Payment = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="payment">Payment object for the checkout request</param>
        public CheckoutRequest(Payment payment)
        {
            _Payment = payment;
        }
    }
}
