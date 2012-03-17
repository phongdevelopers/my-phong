using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Orders
{
    /// <summary>
    /// Enum representing the types of Order Items
    /// </summary>
    public enum OrderItemType
    {
        /// <summary>
        /// The order item is a product
        /// </summary>
        Product,

        /// <summary>
        /// The order item is a shipping charge
        /// </summary>
        Shipping,

        /// <summary>
        /// The order item is a handling charge
        /// </summary>
        Handling,

        /// <summary>
        /// The order item is a Tax charge
        /// </summary>
        Tax,

        /// <summary>
        /// The order item is a discount
        /// </summary>
        Discount,

        /// <summary>
        /// The order item is a coupen
        /// </summary>
        Coupon,

        /// <summary>
        /// The order item is a general charge
        /// </summary>
        Charge,

        /// <summary>
        /// The order item is a gift certificate
        /// </summary>
        GiftCertificate,

        /// <summary>
        /// The order item is a gift wrap
        /// </summary>
        GiftWrap,

        /// <summary>
        /// The order item is a credit
        /// </summary>
        Credit,

        /// <summary>
        /// The order item is a payment made via gift certificate
        /// </summary>
        GiftCertificatePayment
    }
}
