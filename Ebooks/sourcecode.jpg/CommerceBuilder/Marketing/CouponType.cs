using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Marketing
{
    /// <summary>
    /// Enum that represents the type of coupons
    /// </summary>
    public enum CouponType
    {
        /// <summary>
        /// This is an order coupon
        /// </summary>
        Order, 
        
        /// <summary>
        /// This is a product coupon
        /// </summary>
        Product, 
        
        /// <summary>
        /// This is a shipping coupon
        /// </summary>
        Shipping
    }
}
