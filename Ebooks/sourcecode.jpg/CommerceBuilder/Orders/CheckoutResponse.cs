using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Orders
{
    /// <summary>
    /// Class representing checkout response
    /// </summary>
    public class CheckoutResponse
    {
        private bool _Success;
        private int _OrderId;
        private int _OrderNumber;
        private List<string> _WarningMessages;

        /// <summary>
        /// Indicates whether checkout was successful or not
        /// </summary>
        public bool Success
        {
            get { return _Success; }
            set { _Success = value; }
        }

        /// <summary>
        /// Id of the order associated with this checkout
        /// </summary>
        public int OrderId
        {
            get { return _OrderId; }
            set { _OrderId = value; }
        }

        /// <summary>
        /// OrderNumber assigned to the order associated with this checkout
        /// </summary>
        public int OrderNumber
        {
            get { return _OrderNumber; }
            set { _OrderNumber = value; }
        }

        /// <summary>
        /// Warning messages that may have occured during the checkout
        /// </summary>
        public List<string> WarningMessages
        {
            get { return _WarningMessages; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="success">true will create successful response, false will create failed response</param>
        /// <param name="orderId">The order id associated with this checkout</param>
        /// <param name="orderNumber">The order number associated with this checkout</param>
        public CheckoutResponse(bool success, int orderId, int orderNumber)
        {
            _Success = success;
            _OrderId = orderId;
            _OrderNumber = orderNumber;
            _WarningMessages = new List<string>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="success">true will create successful response, false will create failed response</param>
        /// <param name="warningMessages">A list of warning messages to associate with this checkout response</param>
        public CheckoutResponse(bool success, List<string> warningMessages)
        {
            _Success = success;
            _OrderId = 0;
            _OrderNumber = 0;
            _WarningMessages = warningMessages;
        }
    }
}
