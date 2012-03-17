using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Marketing
{
    /// <summary>
    /// Enum representing coupon rules
    /// </summary>
    public enum CouponRule
    {
        /// <summary>
        /// All coupons are allowed
        /// </summary>
        All,
 
        /// <summary>
        /// Only selected coupons are allowed
        /// </summary>
        AllowSelected, 
        
        /// <summary>
        /// Selected coupons are not allowed
        /// </summary>
        ExcludeSelected
    }
}
