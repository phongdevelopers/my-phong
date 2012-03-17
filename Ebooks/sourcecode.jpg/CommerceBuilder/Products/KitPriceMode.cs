using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Products
{    
    /// <summary>
    /// Enumeration that represents how the price is modified by a kit item
    /// </summary>
    public enum KitPriceMode
    {
        //TODO ??
        /// <summary>
        /// 
        /// </summary>
        Inherit, 
        
        /// <summary>
        /// The price of the base product will be modified using the price of the kit item
        /// </summary>
        Modify, 
        
        /// <summary>
        /// The price of the base product will be overridden by the price of the kit item
        /// </summary>
        Override
    }
}
