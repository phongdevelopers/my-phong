using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Marketing
{
    /// <summary>
    /// Enumeration that represents the discount mode when discount is set on a category.
    /// </summary>
    public enum VolumeDiscountMode
    {
        /// <summary>
        /// Discount mode is line item. In line item mode, eligibility 
        /// for a discount is calculated for all products from a category on 
        /// a line item basis. 
        /// </summary>
        LineItem = 0, 
        
        /// <summary>
        /// Discount mode is grouping. In grouping mode, the quantity and values of 
        /// all products in that category are added together for the purpose of 
        /// determining eligibility for the discount. 
        /// </summary>
        Grouping
    }
}
