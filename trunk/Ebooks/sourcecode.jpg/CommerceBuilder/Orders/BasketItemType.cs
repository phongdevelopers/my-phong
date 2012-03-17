using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Orders
{
    /// <summary>
    /// Enumeration that represents the type of basket item
    /// </summary>
    public enum BasketItemType : byte
    {
        /// <summary>
        /// Basket item is a product.
        /// </summary>
        Product, 
        
        /// <summary>
        /// Basket item is a tax.
        /// </summary>
        Tax, 
        
        /// <summary>
        /// Basket item is a shipping charge.
        /// </summary>
        Shipping, 
        
        /// <summary>
        /// Basket item is a coupon.
        /// </summary>
        Coupon, 
        
        /// <summary>
        /// Basket item is a discount.
        /// </summary>
        Discount, 
        
        /// <summary>
        /// Basket item is a gift-wrap charge.
        /// </summary>
        GiftWrap, 
        
        /// <summary>
        /// Basket item is a gift certificate.
        /// </summary>
        GiftCertificate
    }
}
